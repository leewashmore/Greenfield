set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00329'
declare @CurrentScriptVersion as nvarchar(100) = '00330'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO
            IF OBJECT_ID ( 'INTERNAL_FISCAL_EXTRACT', 'P' ) IS NOT NULL 
DROP PROCEDURE INTERNAL_FISCAL_EXTRACT;
GO
------------------------------------------------------------------------
-- Purpose:	Insert data for the PRIMARY analyst DATA_SOURCE.
--
-- Author:	David Muench
-- Date:	July 1, 2012
------------------------------------------------------------------------
create procedure INTERNAL_FISCAL_EXTRACT (
	@ISSUER_ID			varchar(20)		= NULL			-- The company identifier		
,	@VERBOSE			char			= 'Y'
)
as

	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()

	-- Get the constant information about this Issuer.
	declare @XREF	varchar(20)
	declare @ReportNumber	varchar(20)
	declare @ISO_COUNTRY_CODE	varchar(3)
	select @XREF = max(XREF)
		,  @ReportNumber = max(REPORTNUMBER)
		,  @ISO_COUNTRY_CODE = max(ISO_COUNTRY_CODE)
	  from dbo.GF_SECURITY_BASEVIEW
	 where ISSUER_ID = @ISSUER_ID
	 group by ISSUER_ID

	declare @CURRENCY_CODE	varchar(3)
	select @CURRENCY_CODE = max(CURRENCY_CODE)
	  from Country_Master
	 where COUNTRY_CODE = @ISO_COUNTRY_CODE


	-- Prepare to gather the data
	
	-- Gather the COA_TYPES, use INTERNAL_ISSUER first
	select ii.ISSUER_ID, ii.COA_TYPE
	  into #COA_TYPE
	  from dbo.INTERNAL_ISSUER ii
	 where ii.ISSUER_ID = @ISSUER_ID

	;
	if @VERBOSE = 'Y'
		BEGIN
			print 'After Gather the COA_TYPES, use INTERNAL_ISSUER first' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


--modified 1/29/13 (JM):  this should not be needed as internal_issuer should be populated on the init of get_data for each issuer
 
	---- select 'COA_TYPE' as A, * from #COA_TYPE
	---- Also include COA types from Reuters
	--insert into #COA_TYPE
	--select distinct sb.ISSUER_ID, max(sci.COAType)
	--  from dbo.GF_SECURITY_BASEVIEW sb 
	-- inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = sb.REPORTNUMBER
	-- where sb.ISSUER_ID = @ISSUER_ID
	--   and sb.ISSUER_ID not in (select ISSUER_ID from #COA_TYPE)
	-- group by sb.ISSUER_ID

	--if @VERBOSE = 'Y'
	--	BEGIN
	--		print 'After Also include COA types from Reuters' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	--		set @START = GETDATE()
	--	END

	-- Gather the required COA codes by COA_TYPE
	select distinct dm.COA, DATA_ID
	  into #DM
	  from dbo.DATA_MASTER dm
	 inner join #COA_TYPE ct on (ct.COA_TYPE = 'BNK' and dm.BANK = 'Y')
							 or (ct.COA_TYPE = 'IND' and dm.INDUSTRIAL = 'Y')
							 or (ct.COA_TYPE = 'FIN' and dm.INSURANCE = 'Y')
							 or (ct.COA_TYPE = 'UTL' and dm.UTILITY = 'Y')
	 where (dm.COA is not NULL and dm.COA > ' ')
	--   and dm.QUARTERLY = 'N'

	if @VERBOSE = 'Y'
		BEGIN
			print 'After Gather the required COA codes by COA_TYPE' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	-- Gather data that we have
		
	-- STEP 1:   Collect Actual Annual data from the Internal Analysts
	Select i.*, i.CURRENCY as SOURCE_CURRENCY, id.PERIOD_TYPE, id.AMOUNT, dm.DATA_ID, dm.COA
		,  cast(PERIOD_YEAR as varchar(4)) + 'A' as PERIOD, @CURRENCY_CODE as COUNTRY_CURRENCY
		,  'INTERNAL_FISCAL_EXTRACT #A 1                                                ' as CALCULATION_DIAGRAM
	  into #A
	  from INTERNAL_STATEMENT i
	 inner join dbo.INTERNAL_DATA id on id.ISSUER_ID = i.ISSUER_ID and id.REF_NO = i.REF_NO
	 inner join #DM dm on dm.COA = id.COA		-- limits the data collected
	 where i.ISSUER_ID = @ISSUER_ID
	   and i.ROOT_SOURCE = 'PRIMARY'
	   and i.AMOUNT_TYPE = 'ACTUAL'
	   and id.PERIOD_TYPE = 'A'
	   and i.ROOT_SOURCE_DATE > DATEADD(day, -120, getdate())	-- Stale date 1/7/13 (JM): Modified from 90 to 120 days per FM
	;

	if @VERBOSE = 'Y'
		BEGIN
			print 'After Gather the Actual Annual data from the Internal Analysts' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


--Modified 1/29/13 (JM) to simplify this to first convert to USD no matter the source currency
	update #A
	   set CURRENCY = 'USD' 
		,  AMOUNT = case when dm.FX_CONV_TYPE = 'NONE' then a.AMOUNT
						 when dm.FX_CONV_TYPE = 'AVG' and isnull(fxs.AVG12MonthRATE, 0.0) <> 0.0 then (a.AMOUNT / fxs.AVG12MonthRATE)
						 when dm.FX_CONV_TYPE = 'PIT' and isnull(fxs.FX_RATE, 0.0) <> 0.0 then (a.AMOUNT / fxs.FX_RATE)
						 else 0.0 end 
		,  CALCULATION_DIAGRAM = case when FX_CONV_TYPE = 'NONE' then CALCULATION_DIAGRAM
								when FX_CONV_TYPE = 'AVG' and isnull(fxs.AVG12MonthRATE, 0.0) = 0.0 then '12 Month Avg FX rate bad ' + ISNULL(a.SOURCE_CURRENCY, ' ')
								when FX_CONV_TYPE = 'PIT' and isnull(fxs.FX_RATE, 0.0) = 0.0 then 'Daily FX Rate is bad ' + ISNULL(a.SOURCE_CURRENCY, ' ')
								else 'INTERNAL_FISCAL_EXTRACT #A 1A' end 
	  from #A a
	 inner join dbo.DATA_MASTER dm on dm.DATA_ID = a.DATA_ID
	  left join dbo.FX_RATES fxs on fxs.CURRENCY = a.SOURCE_CURRENCY and fxs.FX_DATE = a.PERIOD_END_DATE
	 where a.amount is not null
	 and isnull(fxs.FX_RATE, 0.0) <> 0.0
	  and isnull(fxs.AVG12MonthRATE, 0.0) <> 0.0
	

	if @VERBOSE = 'Y'
		BEGIN
			print 'After convert source currency to USD (ACTUAL)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END
	 

--Modified 1/29/13 (JM) to convert the USD data to Local
	insert into #A
	select 	a.ISSUER_ID
		,	a.REF_NO
		,	a.PERIOD_YEAR
		,	a.ROOT_SOURCE
		,	a.ROOT_SOURCE_DATE
		,	a.PERIOD_LENGTH
		,	a.PERIOD_END_DATE
		,	a.COUNTRY_CURRENCY as CURRENCY
		,	a.AMOUNT_TYPE
		,	a.SOURCE_CURRENCY
		,	a.PERIOD_TYPE
		,  case when dm.FX_CONV_TYPE = 'NONE' then a.AMOUNT
				when dm.FX_CONV_TYPE = 'AVG' and isnull(fx.AVG12MonthRATE, 0.0) <> 0.0 then a.AMOUNT * fx.AVG12MonthRATE
				when dm.FX_CONV_TYPE = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then a.AMOUNT * fx.FX_RATE
				else 0.0 end as AMOUNT
		,	a.DATA_ID
		,	a.COA
		,   a.PERIOD
		,  a.COUNTRY_CURRENCY
		,  case when FX_CONV_TYPE = 'NONE' then CALCULATION_DIAGRAM
				when FX_CONV_TYPE = 'AVG' and isnull(fx.AVG12MonthRATE, 0.0) = 0.0 then '12 Month Avg FX rate bad'
				when FX_CONV_TYPE = 'PIT' and isnull(FX_RATE, 0.0) = 0.0 then 'Daily FX Rate is bad'
				else 'INERNAL_FISCAL_EXTRACT #A 2' end as CALCULATION_DIAGRAM
	  from #A a
	 inner join dbo.DATA_MASTER dm on dm.DATA_ID = a.DATA_ID
	  left join dbo.FX_RATES fx on fx.CURRENCY = a.COUNTRY_CURRENCY and fx.FX_DATE = a.PERIOD_END_DATE
	where 1=1
	 and isnull(fx.FX_RATE, 0.0) <> 0.0
	  and isnull(fx.AVG12MonthRATE, 0.0) <> 0.0
	  
	  
	if @VERBOSE = 'Y'
		BEGIN
			print 'After Convert the Analysts data to local (ACTUAL)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		
-- Debug statement
-- select 'INTERNAL_FISCAL_EXTRACT A' as A, * from #A order by PERIOD_YEAR, PERIOD_TYPE, DATA_ID
	   

	-- STEP 2:   Collect Actual Annual data from Reuters that is not already gathered
	Select PERIOD_FINANCIALS.*
		,  cast(PERIOD_YEAR as varchar(4)) + 'A' as PERIOD
		,  'INTERNAL_FISCAL_EXTRACT #B 1' as TXT
	  into #B
	  from PERIOD_FINANCIALS 
	  inner join #DM on #DM.DATA_ID = PERIOD_FINANCIALS.DATA_ID
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_SOURCE = 'REUTERS'
	   and AMOUNT_TYPE = 'ACTUAL'
	   and PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #A)  ;
	;

	if @VERBOSE = 'Y'
		BEGIN
			print 'After Gather any Actual Annual data from Reuters' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

-- Debug statment
--select 'INTERNAL_FISCAL_EXTRACT B' as B, * from #B order by PERIOD_YEAR, PERIOD_TYPE, DATA_ID



	-- STEP 3:  Gather the Estimated Estimated data from the Internal Analysts

	Select i.*, i.CURRENCY as SOURCE_CURRENCY, id.PERIOD_TYPE, id.AMOUNT, dm.DATA_ID, dm.COA
		,  cast(PERIOD_YEAR as varchar(4)) + 'A' as PERIOD, @CURRENCY_CODE as COUNTRY_CURRENCY
		,  'INTERNAL_FISCAL_EXTRACT #C 1                                         ' as CALCULATION_DIAGRAM
	  into #C
	  from INTERNAL_STATEMENT i
	 inner join dbo.INTERNAL_DATA id on id.ISSUER_ID = i.ISSUER_ID and id.REF_NO = i.REF_NO
	 inner join #DM dm on dm.COA = id.COA		-- limits the data collected
	 where i.ISSUER_ID = @ISSUER_ID
	   and i.ROOT_SOURCE = 'PRIMARY'
	   and i.AMOUNT_TYPE = 'ESTIMATE'
	   and id.PERIOD_TYPE = 'A'
	   and i.ROOT_SOURCE_DATE > DATEADD(day, -120, getdate())	-- Stale date 1/7/13 (JM): Modified from 90 to 120 days per FM
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #A)  
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #B)  
	;

	if @VERBOSE = 'Y'
		BEGIN
			print 'After Gather the Estimated data from the Internal Analysts' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

-- Debug statment
-- select 'INTERNAL_FISCAL_EXTRACT C' as C, * from #C

--Modified 1/29/13 (JM) to simplify this to first convert to USD no matter the source currency
	update #C
	   set CURRENCY = 'USD'  
		,  AMOUNT = case when dm.FX_CONV_TYPE = 'NONE' then c.AMOUNT
						 when dm.FX_CONV_TYPE = 'AVG' and isnull(fxs.AVG12MonthRATE, 0.0) <> 0.0 then (c.AMOUNT / fxs.AVG12MonthRATE) 
						 when dm.FX_CONV_TYPE = 'PIT' and isnull(fxs.FX_RATE, 0.0) <> 0.0  then (c.AMOUNT / fxs.FX_RATE) 
						 else 0.0 end 
		,  CALCULATION_DIAGRAM = case when FX_CONV_TYPE = 'NONE' then CALCULATION_DIAGRAM
								when FX_CONV_TYPE = 'AVG' and isnull(fxs.AVG12MonthRATE, 0.0) = 0.0 then '12 Month Avg FX rate bad ' + isnull(c.SOURCE_CURRENCY, ' ')
								when FX_CONV_TYPE = 'PIT' and isnull(fxs.FX_RATE, 0.0) = 0.0 then 'Daily FX Rate is bad ' + isnull(c.SOURCE_CURRENCY, ' ')
								else 'INTERNAL_FISCAL_EXTRACT #C 2' + CALCULATION_DIAGRAM end 
	  from #C c
	 inner join dbo.DATA_MASTER dm on dm.DATA_ID = c.DATA_ID
	  left join dbo.FX_RATES fxs on fxs.CURRENCY = c.SOURCE_CURRENCY and fxs.FX_DATE = c.PERIOD_END_DATE
	 where c.AMOUNT is not NULL
	  and isnull(fxs.AVG12MonthRATE, 0.0) <> 0.0
	  and isnull(fxs.FX_RATE, 0.0) <> 0.0

	if @VERBOSE = 'Y'
		BEGIN
			print 'After convert source currency to USD (ESTIMATES)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END
	
	
	-- Convert the Analysts' Estimate data into Local Currency
	insert into #C
	select 	c.ISSUER_ID
		,	c.REF_NO
		,	c.PERIOD_YEAR
		,	c.ROOT_SOURCE
		,	c.ROOT_SOURCE_DATE
		,	c.PERIOD_LENGTH
		,	c.PERIOD_END_DATE
		,	c.COUNTRY_CURRENCY as CURRENCY
		,	c.AMOUNT_TYPE
		,	c.SOURCE_CURRENCY
		,	c.PERIOD_TYPE
		,  case when dm.FX_CONV_TYPE = 'NONE' then c.AMOUNT
				when dm.FX_CONV_TYPE = 'AVG' and isnull(fx.AVG12MonthRATE, 0.0) <> 0.0 then c.AMOUNT * fx.AVG12MonthRATE
				when dm.FX_CONV_TYPE = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then c.AMOUNT * fx.FX_RATE
				else 0.0 end as AMOUNT
		,	c.DATA_ID
		,	c.COA
		,   c.PERIOD
		,  c.COUNTRY_CURRENCY
		
		,  case when FX_CONV_TYPE = 'NONE' then isnull(CALCULATION_DIAGRAM, ' ')
				when FX_CONV_TYPE = 'AVG' and isnull(AVG12MonthRATE, 0.0) = 0.0 then '12 Month Avg FX rate bad'
				when FX_CONV_TYPE = 'PIT' and isnull(FX_RATE, 0.0) = 0.0 then 'Daily FX Rate is bad'
				else 'INTERNAL_FISCAL_EXTRACT #C 3'  end as CALCULATION_DIAGRAM
	  from #C c
	 inner join dbo.DATA_MASTER dm on dm.DATA_ID = c.DATA_ID
	 inner join dbo.FX_RATES fx on fx.CURRENCY = c.COUNTRY_CURRENCY and fx.FX_DATE = c.PERIOD_END_DATE


	if @VERBOSE = 'Y'
		BEGIN
			print 'After Convert the Analysts data to local (ESTIMATE)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	
-- Debug statment
--select 'CCC' as CCC, * from #C order by PERIOD_YEAR, PERIOD_TYPE, DATA_ID



	--Step 4:  Gather any Estimated Annual data from Reuters that is not already gathered
	Select PERIOD_FINANCIALS.*
		,  cast(PERIOD_YEAR as varchar(4)) + 'A' as PERIOD
		,  'INTERNAL_FISCAL_EXTRACT #D 1 ' + CALCULATION_DIAGRAM as TXT
	  into #D
	  from PERIOD_FINANCIALS 
	  inner join #DM on #DM.DATA_ID = PERIOD_FINANCIALS.DATA_ID
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_SOURCE = 'REUTERS'
	   and AMOUNT_TYPE = 'ESTIMATE'
	   and PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #A)
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #B)
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #C)
	;

	if @VERBOSE = 'Y'
		BEGIN
			print 'After Gather any Estimated Annual data from Reuters that is not already gathered' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

-- Debug statment
-- select 'INTERNAL_FISCAL_EXTRACT D' as D, * from #D order by PERIOD_YEAR, PERIOD_TYPE, DATA_ID
												

	-- Gather it all together
	SELECT ISSUER_ID, ROOT_SOURCE, ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR
		,  PERIOD_END_DATE, AMOUNT, CURRENCY, AMOUNT_TYPE, COA, SOURCE_CURRENCY
		,  CALCULATION_DIAGRAM
	  into #R
	  from (
			select a.ISSUER_ID, a.ROOT_SOURCE, a.ROOT_SOURCE_DATE, a.PERIOD_YEAR, a.COA
				,  a.PERIOD_END_DATE, a.CURRENCY, a.AMOUNT_TYPE, a.PERIOD_TYPE, a.AMOUNT, a.SOURCE_CURRENCY
				,  CALCULATION_DIAGRAM
			 from #A a
		union
			select b.ISSUER_ID, b.ROOT_SOURCE, b.ROOT_SOURCE_DATE, b.PERIOD_YEAR, dm.COA
				,  b.PERIOD_END_DATE, b.CURRENCY, b.AMOUNT_TYPE, b.PERIOD_TYPE, b.AMOUNT, b.SOURCE_CURRENCY
				,  TXT as CALCULATION_DIAGRAM
			  from #B b
			 inner join dbo.DATA_MASTER dm on dm.DATA_ID = b.DATA_ID
		union
			select c.ISSUER_ID, c.ROOT_SOURCE, c.ROOT_SOURCE_DATE, c.PERIOD_YEAR, c.COA
				,  c.PERIOD_END_DATE, c.CURRENCY, c.AMOUNT_TYPE, c.PERIOD_TYPE, c.AMOUNT, c.SOURCE_CURRENCY
				,  CALCULATION_DIAGRAM
			 from #C c
		union
			select d.ISSUER_ID, d.ROOT_SOURCE, d.ROOT_SOURCE_DATE, d.PERIOD_YEAR, dm.COA
				,  d.PERIOD_END_DATE, d.CURRENCY, d.AMOUNT_TYPE, d.PERIOD_TYPE, d.AMOUNT, d.SOURCE_CURRENCY
				,  TXT as CALCULATION_DIAGRAM
			  from #D d
			 inner join dbo.DATA_MASTER dm on dm.DATA_ID = d.DATA_ID
			) z
	;

	if @VERBOSE = 'Y'
		BEGIN
			print 'After Gathering it all together' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END
-- select 'R' as R, * from #R r
	 
	-- Now bring in the FX and DATA_ID
	select r.ISSUER_ID
		,  ' ' as SECURITY_ID
		,  ct.COA_TYPE
		,  'PRIMARY' as DATA_SOURCE
		,  r.ROOT_SOURCE
		,  r.ROOT_SOURCE_DATE
		,  r.PERIOD_TYPE
		,  r.PERIOD_YEAR
		,  r.PERIOD_END_DATE
		,  'FISCAL' as FISCAL_TYPE
		,  r.CURRENCY
		,  dm.DATA_ID
		,  r.AMOUNT
		,  CALCULATION_DIAGRAM
		,  r.SOURCE_CURRENCY as SOURCE_CURRENCY
		,  r.AMOUNT_TYPE
	  into #RESULT
	  from #R r
	 inner join #COA_TYPE ct on ct.ISSUER_ID = r.ISSUER_ID
--	 inner join (select distinct ISSUER_ID, ISO_COUNTRY_CODE from dbo.GF_SECURITY_BASEVIEW) i
--				on i.ISSUER_ID = r.ISSUER_ID
	 inner join dbo.DATA_MASTER dm on dm.COA = r.COA 
	;

	if @VERBOSE = 'Y'
		BEGIN
			print 'After bringing in the FX' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

-- Debug statment
-- select 'INTERNAL_FISCAL_EXTRACT RESULT' as A,* from #RESULT order by DATA_ID, PERIOD_YEAR, PERIOD_TYPE, CURRENCY
	 
	-- Finally insert it all into the PERIOD_FINANCIALS table for the Local Currency

	insert into dbo.PERIOD_FINANCIALS (ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
									, ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR,PERIOD_END_DATE
									, FISCAL_TYPE, CURRENCY, DATA_ID, AMOUNT, CALCULATION_DIAGRAM
									, SOURCE_CURRENCY, AMOUNT_TYPE)

	select ISSUER_ID
		,  SECURITY_ID
		,  ' ' as COA_TYPE
		,  DATA_SOURCE
		,  ROOT_SOURCE
		,  ROOT_SOURCE_DATE
		,  PERIOD_TYPE
		,  PERIOD_YEAR
		,  PERIOD_END_DATE
		,  FISCAL_TYPE
		,  CURRENCY
		,  DATA_ID
		,  AMOUNT
		,  CALCULATION_DIAGRAM
		,  SOURCE_CURRENCY
		,  AMOUNT_TYPE
	  from #RESULT
	 where 1=1


	if @VERBOSE = 'Y'
		BEGIN
			print 'After Insert into PERIOD_FINANCIALS' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	
	-- Create temp table with index instead of subquery on PERIOD_FINANCIALS
	select *
	  into #PFn
	  from dbo.PERIOD_FINANCIALS
	 where ISSUER_ID = @ISSUER_ID;
	create index PFN_idx on #PFn( ISSUER_ID, SECURITY_ID, DATA_ID, CURRENCY, DATA_SOURCE, FISCAL_TYPE, PERIOD_TYPE, PERIOD_YEAR)

	if @VERBOSE = 'Y'
		BEGIN
			print 'After ' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

			print 'Calendarize PRIMARY ANNUAL'
			-- Calendarize
			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before Calendarize'
			insert into PERIOD_FINANCIALS
			select DISTINCT pfc.ISSUER_ID
				,  pfc.SECURITY_ID
				,  pfc.COA_TYPE
				,  pfc.DATA_SOURCE
				,  pfc.ROOT_SOURCE
				,  pfc.ROOT_SOURCE_DATE
				,  pfc.PERIOD_TYPE
				,  pfc.PERIOD_YEAR
				,  cast('12/31/' + cast(pfc.PERIOD_YEAR as varchar) as datetime) as PERIOD_END_DATE
				,  'CALENDAR' as FISCAL_TYPE
				,  pfc.CURRENCY
				,  pfc.DATA_ID
				,  (pfc.AMOUNT  * (cast(datepart(month, pfc.PERIOD_END_DATE) as decimal(32,6))/12.0))
					+  case when pfn.DATA_ID is not null 
								then (pfn.AMOUNT * (cast(12-datepart(month, pfn.PERIOD_END_DATE) as decimal(32,6))/12.0))
								else (pfc.AMOUNT * (cast(12-datepart(month, pfc.PERIOD_END_DATE) as decimal(32,6))/12.0)) end as AMOUNT
				,  case when pfn.DATA_ID is not null 
								then 'Current(' + cast(pfc.AMOUNT as varchar(20))  + ' * ' + cast((cast(datepart(month, pfc.PERIOD_END_DATE) as decimal(32,6))/12.0) as varchar(20)) + ') + Next1(' + cast(pfn.AMOUNT as varchar(20)) + ' * ' + cast((cast(12-datepart(month, pfn.PERIOD_END_DATE) as decimal(32,6))/12.0) as varchar(20)) + ')'
								else 'Current(' + cast(pfc.AMOUNT as varchar(20))  + ' * ' + cast((cast(datepart(month, pfc.PERIOD_END_DATE) as decimal(32,6))/12.0) as varchar(20)) + ') + Next2(' + cast(pfc.AMOUNT as varchar(20)) + ' * ' + cast((cast(12-datepart(month, pfc.PERIOD_END_DATE) as decimal(32,6))/12.0) as varchar(20)) + ')' end  as CALCULATION_DIAGRAM
				,  pfc.SOURCE_CURRENCY
--				,  pfc.AMOUNT_TYPE
				,  case when pfn.AMOUNT_TYPE is not null 
								then pfn.AMOUNT_TYPE
								else pfc.AMOUNT_TYPE end as AMOUNT_TYPE --1/7/13 (JM): modified from pfc.amount_type to new case statement 1/7/13 (JM)
			  from dbo.PERIOD_FINANCIALS pfc
			  left join #PFn pfn on pfn.ISSUER_ID = pfc.ISSUER_ID     and pfn.SECURITY_ID = pfc.SECURITY_ID 
								 and pfn.DATA_ID = pfc.DATA_ID         and pfn.CURRENCY = pfc.CURRENCY
								 and pfn.DATA_SOURCE = pfc.DATA_SOURCE and pfn.FISCAL_TYPE = pfc.FISCAL_TYPE
								 and pfn.PERIOD_TYPE = pfc.PERIOD_TYPE and pfn.PERIOD_YEAR = pfc.PERIOD_YEAR+1
												 
			 inner join dbo.DATA_MASTER dm on dm.DATA_ID = pfc.DATA_ID
			 where 1=1 --dm.CALENDARIZE = 'Y'
			   and pfc.ISSUER_ID = @ISSUER_ID
			   and pfc.DATA_SOURCE = 'PRIMARY'							-- Only the newly created values for PRIMARY
			   and pfc.PERIOD_TYPE = 'A'								-- Only Annual, not Quarterly
			   and pfc.FISCAL_TYPE = 'FISCAL'							-- Only FISCAL
--			 order by pfc.ISSUER_ID, pfc.AMOUNT_TYPE, pfc.DATA_SOURCE, pfc.CURRENCY, pfc.PERIOD_YEAR, pfc.PERIOD_TYPE, pfc.DATA_ID

	if @VERBOSE = 'Y'
		BEGIN
			print 'After Calendarize' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


	-- Clean up temp tables
	drop table #A
	drop table #B
	drop table #C
	drop table #D
	drop table #COA_TYPE
	drop table #DM
	drop table #R
	drop table #RESULT
	drop table #PFn

Go
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00330'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())