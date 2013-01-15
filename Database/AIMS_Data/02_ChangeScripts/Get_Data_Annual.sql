IF OBJECT_ID ( 'Get_Data_Annual', 'P' ) IS NOT NULL 
DROP PROCEDURE Get_Data_Annual;
GO
-----------------------------------------------------------------------------------
-- Purpose:	To move the data from the Reuters raw tables into the display tables:
--			PERIOD_FINANCIALS.  This procedure works as an interface to load the 
--			Reuters data received in file format into the AIMS database.
--
-- Author:	David Muench
-- Date:	July 2, 2012
-- Modified:  Justin Machata 1/14/2013
-----------------------------------------------------------------------------------

create procedure Get_Data_Annual(
	@ISSUER_ID			varchar(20) = NULL					-- The company identifier		
,	@VERBOSE			char		= 'Y'
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
	 
--	raiserror( 'before Delete', 1, 2) WITH NOWAIT

	-- Delete the existing data 
	delete from PERIOD_FINANCIALS
	 where ISSUER_ID = @ISSUER_ID;

	if @VERBOSE = 'Y'
		BEGIN
--			raiserror( 'After Delete ISSUER_ID', 1, 2) WITH NOWAIT
			print 'After Delete ISSUER_ID' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	delete from PERIOD_FINANCIALS
	 where SECURITY_ID in (select distinct SECURITY_ID from GF_SECURITY_BASEVIEW where ISSUER_ID = @ISSUER_ID);

	if @VERBOSE = 'Y'
		BEGIN
--			raiserror( 'After Delete SECURITY_ID', 1, 2) WITH NOWAIT
			print 'After Delete SECURITY_ID' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



	-- Create a temp table that limits the ref data to the latest version
	--select r.*, s.RefNo
	--  into #LATEST
	--  from Reuters.dbo.tblStatementRef s 
	-- inner join (select ReportNumber, FiscalYear, StatementType, Max(UpdateDate) as UpdateDate	
	--			   from Reuters.dbo.tblStatementRef
	--			  where ReportNumber = @ReportNumber
	--			  group by ReportNumber, FiscalYear, StatementType
	--			) r on r.ReportNumber = s.ReportNumber and r.FiscalYear = s.FiscalYear 
	--				and r.StatementType = s.StatementType and r.UpdateDate = s.UpdateDate
	
	
	
	--Modified 1/7/13 (JM) to add periodenddate to query 
	select r.*, s.RefNo, 'M' as PeriodLengthCode
	, case when s.PeriodLengthCode = 'W' and s.PeriodLength in (11,12,13,14,15) then cast(3 as decimal(32,6))
					when s.PeriodLengthCode = 'W' and s.PeriodLength in (24,25,26,27,28) then cast(6 as decimal(32,6))
					when s.PeriodLengthCode = 'W' and s.PeriodLength in (37,38,39,40,41) then cast(9 as decimal(32,6))
					when s.PeriodLengthCode = 'W' and s.PeriodLength in (50,51,52,53,54) then cast(12 as decimal(32,6))
					when s.StatementType = 'BAL' then cast(12 as decimal(32,6))
					else s.PeriodLength end as PeriodLength
	into #LATEST
	from Reuters.dbo.tblStatementRef s 
	inner join (select sr.ReportNumber, sr.FiscalYear, sr.StatementType, sr.PeriodEndDate, MAX(UpdateDate) as UpdateDate	
					from Reuters..tblStatementRef sr
					where sr.ReportNumber = @ReportNumber
					group by sr.ReportNumber, sr.FiscalYear, sr.StatementType, sr.PeriodEndDate) r
	on  r.ReportNumber = s.ReportNumber and r.FiscalYear = s.FiscalYear 
					and r.StatementType = s.StatementType and r.PeriodEndDate = s.PeriodEndDate
					and r.UpdateDate = s.UpdateDate
	where 1=1
		   and (   s.StatementType = 'BAL'
				or s.PeriodLengthCode = 'M'
				or ((s.PeriodLengthCode = 'W') and (s.PeriodLength IN (11,12,13,14,15,24,25,26,27,28,37,38,39,40,41,50,51,52,53,54)))
			   )
		  	
	--Added 1/7/13 (JM) to check for multiple reporting statements in one fiscal year, i.e. fiscal year end changes 
	select ReportNumber, FiscalYear, StatementType, COUNT(*) as tally, MAX(PeriodEndDate) as MaxPeriod, MIN(PeriodEndDate) as MinPeriod
	into #MultipleCheck
	from #Latest
	group by ReportNumber, FiscalYear, StatementType

	
	if @VERBOSE = 'Y'
		BEGIN
--			raiserror( 'After Delete SECURITY_ID', 1, 2) WITH NOWAIT
			print 'After Create #LATEST' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


	-- Collect a list of all the data that will be populated.
	if @VERBOSE = 'Y'
		print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before #Reuters select'
	 
	select @ISSUER_ID as ISSUER_ID
		,  sr.CurrencyReported as LocalCurrency
		,  sr.ReportNumber
		,  sr.FiscalYear
		,  s.COA
		,  sci.COAType
		--,  case when isnull(sr.RepToConvExRate, 0.0) = 0.0 then 0.0
		--		when isnull(s.Amount, 0.0) = 0.0 then 0.0 
		--		else (s.Amount/(case when sr.RepToConvExRate <> 0.0 then sr.RepToConvExRate else 1.0 end)) end as ReportedAmount
		,  sr.CurrencyReported
		,  sr.UpdateDate
		,  sr.PeriodEndDate
		,  sr.CurrencyConvertedTo
		,  sr.RepToConvExRate
		,  sr.PeriodLengthCode
		,  sr.PeriodLength
		,  dm.FX_CONV_TYPE
		,  dm.DATA_ID
		,  fx.FX_RATE
		,  fx.AVG90DAYRATE
		,  fx.AVG12MonthRATE
		,  ((s.Amount)/case when RepToConvExRate <> 0.0 then RepToConvExRate else 1.0 end) * (case when isnull(sr.PeriodLengthCode,'M') = 'M' then 12 else 52 end) / isnull(sr.PeriodLength, 12)
		   as Amount12Month 
		,  @ISO_COUNTRY_CODE as ISO_COUNTRY_CODE
		,  @CURRENCY_CODE as CountryCurrency
	  into #Reuters
	  from Reuters.dbo.tblStd s
	 inner join Reuters.dbo.tblStatementRef sr on sr.RefNo = s.RefNo and sr.ReportNumber = s.ReportNumber
	 inner join #LATEST a on a.ReportNumber = sr.ReportNumber and a.RefNo = sr.RefNo and a.UpdateDate = sr.UpdateDate
	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = s.ReportNumber
	  left join dbo.FX_RATES fx on fx.FX_DATE = sr.PeriodEndDate and fx.CURRENCY = sr.CurrencyReported
	 inner join dbo.DATA_MASTER dm on dm.COA = s.COA
	 inner join #MultipleCheck m on m.ReportNumber = sr.ReportNumber and m.FiscalYear = sr.FiscalYear and m.StatementType = sr.StatementType -- added 1/7/13 (JM) to join to multiple check to only pull for period where single reporting per period
	 where 1=1
	   and s.ReportNumber = @ReportNumber
	   and @CURRENCY_CODE is not NULL
	   and m.tally = 1  --added 1/7/13 (JM) to pull for period where single reporting per period


	--Added 1/8/13 (JM) Special Processing for companies with multiple reporting statements per period, i.e. fiscal year end changes
	--Create #LatestMultiple temp table
	select a.ReportNumber, a.FiscalYear, a.StatementType, 
		b.PeriodEndDate as PeriodEndDate1, b.UpdateDate as UpdateDate1, b.RefNo as RefNo1, b.PeriodLength as PeriodLength1, b.PeriodLengthCode as PeriodLengthCode1,
		c.PeriodEndDate as PeriodEndDate2, c.UpdateDate as UpdateDate2, c.RefNo as RefNo2, c.PeriodLength as PeriodLength2, c.PeriodLengthCode as PeriodLengthCode2,
		case when c.PeriodLength > 12 then 12/c.PeriodLength  
		else cast(1 as decimal(32,6)) end as percent2,
		case when c.PeriodLength > 12 then cast(0 as decimal(32,6))
		else (12-c.PeriodLength)/b.PeriodLength end as percent1
	into #MultipleLatest
	from #MultipleCheck a
	inner join #LATEST b on a.ReportNumber = b.ReportNumber and a.StatementType = b.StatementType and a.MinPeriod = b.PeriodEndDate and a.FiscalYear = b.FiscalYear
	inner join #LATEST c on a.ReportNumber = c.ReportNumber and a.StatementType = c.StatementType and a.MaxPeriod = c.PeriodEndDate and a.FiscalYear = c.FiscalYear
	where a.tally = 2
	
	insert into #Reuters			
	select @ISSUER_ID as ISSUER_ID
		,  sr.CurrencyReported as LocalCurrency
		,  sr.ReportNumber
		,  sr.FiscalYear
		,  s.COA
		,  sci.COAType
		--,  case when isnull(sr.RepToConvExRate, 0.0) = 0.0 then 0.0
		--		when isnull(s.Amount, 0.0) = 0.0 then 0.0 
		--		else (s.Amount/(case when sr.RepToConvExRate <> 0.0 then sr.RepToConvExRate else 1.0 end)) end as ReportedAmount
		,  sr.CurrencyReported
		,  sr.UpdateDate
		,  sr.PeriodEndDate
		,  sr.CurrencyConvertedTo
		,  sr.RepToConvExRate
		,  'M' as PeriodLengthCode
		,  12 as PeriodLength
		,  dm.FX_CONV_TYPE
		,  dm.DATA_ID
		,  fx.FX_RATE
		,  fx.AVG90DAYRATE
		,  fx.AVG12MonthRATE
		,  case when a1.Amount12Month is null then ((s.Amount)/case when RepToConvExRate <> 0.0 then RepToConvExRate else 1.0 end) * (12/a2.PeriodLength2)
		   else (((s.Amount)/case when RepToConvExRate <> 0.0 then RepToConvExRate else 1.0 end) * (a2.percent2)) + a1.Amount12Month end
		   as Amount12Month 
		,  @ISO_COUNTRY_CODE as ISO_COUNTRY_CODE
		,  @CURRENCY_CODE as CountryCurrency
	  from Reuters.dbo.tblStd s
	 inner join Reuters.dbo.tblStatementRef sr on sr.RefNo = s.RefNo and sr.ReportNumber = s.ReportNumber
	 inner join #MultipleLatest a2 on a2.ReportNumber = sr.ReportNumber and a2.RefNo2 = sr.RefNo and a2.PeriodEndDate2 = sr.PeriodEndDate
	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = s.ReportNumber
	  left join dbo.FX_RATES fx on fx.FX_DATE = sr.PeriodEndDate and fx.CURRENCY = sr.CurrencyReported
	 inner join dbo.DATA_MASTER dm on dm.COA = s.COA
	 inner join (select sr.ReportNumber
			, sr.FiscalYear
			, s.COA
			, sr.CurrencyReported
			, ((s.Amount)/case when RepToConvExRate <> 0.0 then RepToConvExRate else 1.0 end) * (a.percent1) as Amount12Month 
			from Reuters.dbo.tblStd s
			inner join Reuters.dbo.tblStatementRef sr on sr.RefNo = s.RefNo and sr.ReportNumber = s.ReportNumber
			inner join #MultipleLatest a on a.ReportNumber = sr.ReportNumber and a.RefNo1 = sr.RefNo and a.PeriodEndDate1 = sr.PeriodEndDate) a1
			on s.ReportNumber = a1.ReportNumber and s.ReportYear = a1.FiscalYear and s.COA = a1.COA
	 where 1=1
	   and s.ReportNumber = @ReportNumber
	   and @CURRENCY_CODE is not NULL
	   and sr.CurrencyReported = a1.CurrencyReported  --only if the currency reported is the same for the two periods
	
	
	if @VERBOSE = 'Y'
		BEGIN
			print 'After #Reuters select' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


	-- Verify that there is work to do
	declare @cnt integer
	select @cnt = COUNT(*) from #Reuters

	if @cnt > 0 
		BEGIN
			-- Create a temp table that mirrors PERIOD_FINAINCIALS
			select * 
			  into #PF
			  from PERIOD_FINANCIALS
			 where 1=0

			-- Create it specifically instead of copying from PERIOD_FINANCIALS
			-- This temp table definition must be maintained when ever the PERIOD_FINANCIALS table is updated.
/*			CREATE TABLE #PF 
			(
				ISSUER_ID			varchar(20)		NOT NULL,
				SECURITY_ID			varchar(20)		NOT NULL,
				COA_TYPE			varchar(3)		NOT NULL,
				DATA_SOURCE			varchar(10)		NOT NULL,
				ROOT_SOURCE			varchar(10)		NOT NULL,
				ROOT_SOURCE_DATE	datetime		NOT NULL,
				PERIOD_TYPE			char(2)			NOT NULL,
				PERIOD_YEAR			integer			NOT NULL,
				PERIOD_END_DATE		datetime		NOT NULL,
				FISCAL_TYPE			char(8)			NOT NULL,
				CURRENCY			char(3)			NOT NULL,
				DATA_ID				integer			NOT NULL,
				AMOUNT				decimal(32, 6)	NOT NULL,
				CALCULATION_DIAGRAM	varchar(255)		NULL,
				SOURCE_CURRENCY		char(3)			NOT NULL,
				AMOUNT_TYPE			char(10)		NOT NULL
			)
*/

			if @VERBOSE = 'Y'
				BEGIN
					print 'After Create empty #PF' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
					set @START = GETDATE()
					print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert USD1'
				END



select 'get_data_annual #Reuters' as A, r.*
  from #Reuters r
 order by r.PeriodEndDate, r.DATA_ID

			-- Insert the Reuters data for the currency it was reported in
			-- So long as that currency is the Country currency or USD
			insert into #PF --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
				,  'REUTERS' as DATA_SOURCE
				,  'REUTERS' as ROOT_SOURCE
				,  r.UpdateDate as ROOT_SOURCE_DATE
				,  'A' as PERIOD_TYPE
				,  r.FiscalYear as PERIOD_YEAR
				,  r.PeriodEndDate as PERIOD_END_DATE
				,  'FISCAL' as FISCAL_TYPE
				,  r.CurrencyReported as CURRENCY
				,  r.DATA_ID as DATA_ID
				,  r.Amount12Month	as AMOUNT
				,  'Get_Data_Annual Insert into #PF 1' as CALCULATION_DIAGRAM
				,  r.CurrencyReported as SOURCE_CURRENCY
				,  'ACTUAL' as AMOUNT_TYPE
			  from #Reuters r
			 where (r.CurrencyReported = @CURRENCY_CODE or r.CurrencyReported = 'USD')

			if @VERBOSE = 'Y'
				BEGIN
					print 'After insert USD1' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
					set @START = GETDATE()
					print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert USD2'
				END
				
			-- Insert the Reuters data, converted to Local currency, for those reported in USD currency
			insert into #PF  --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
				,  'REUTERS' as DATA_SOURCE
				,  'REUTERS' as ROOT_SOURCE
				,  r.UpdateDate as ROOT_SOURCE_DATE
				,  'A' as PERIOD_TYPE
				,  r.FiscalYear as PERIOD_YEAR
				,  r.PeriodEndDate as PERIOD_END_DATE
				,  'FISCAL' as FISCAL_TYPE
				,  r.CountryCurrency as CURRENCY
				,  r.DATA_ID as DATA_ID
				,  case when FX_CONV_TYPE = 'PIT' then r.Amount12Month * r.FX_RATE
						when FX_CONV_TYPE = 'AVG' then r.Amount12Month * r.AVG12MonthRATE
						else r.Amount12Month end
				,  'Get_Data_Annual Insert into #PF 2' as CALCULATION_DIAGRAM
				,  r.CurrencyReported as SOURCE_CURRENCY
				,  'ACTUAL' as AMOUNT_TYPE
			  from #Reuters r
			 where CurrencyReported = 'USD'
			   and isnull(r.FX_RATE, 0.0) <> 0.0
			   and isnull(r.AVG12MonthRATE, 0.0) <> 0.0

			if @VERBOSE = 'Y'
				BEGIN
					print 'After insert USD2' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
					set @START = GETDATE()
					print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert non-USD1'
				END
				
			-- Insert the Reuters data, converted to USD, for those reported in the country currency
			insert into #PF  --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
				,  'REUTERS' as DATA_SOURCE
				,  'REUTERS' as ROOT_SOURCE
				,  r.UpdateDate as ROOT_SOURCE_DATE
				,  'A' as PERIOD_TYPE
				,  r.FiscalYear as PERIOD_YEAR
				,  r.PeriodEndDate as PERIOD_END_DATE
				,  'FISCAL' as FISCAL_TYPE
				,  'USD' as CURRENCY
				,  r.DATA_ID as DATA_ID
--				,  case when FX_CONV_TYPE = 'PIT' then r.ReportedAmount * r.FX_RATE
--						when FX_CONV_TYPE = 'AVG' then r.ReportedAmount * r.AVG12MonthRATE
--						else r.ReportedAmount end
				,  case when FX_CONV_TYPE = 'PIT' then r.Amount12Month / r.FX_RATE
						when FX_CONV_TYPE = 'AVG' then r.Amount12Month / r.AVG12MonthRATE
						else r.Amount12Month end
				,  'Get_Data_Annual Insert into #PF 3' as CALCULATION_DIAGRAM
				,  r.CurrencyReported as SOURCE_CURRENCY
				,  'ACTUAL' as AMOUNT_TYPE
			  from #Reuters r
			 where r.CurrencyReported = r.CountryCurrency
			   and CurrencyReported <> 'USD'
			   and isnull(r.FX_RATE, 0.0) <> 0.0
			   and isnull(r.AVG12MonthRATE, 0.0) <> 0.0


			if @VERBOSE = 'Y'
				BEGIN
					print 'After insert non-USD1' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
					set @START = GETDATE()
					print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert non-USD2'
				END

		-- Otherwise
			-- Insert the Reuters data, converted to Country currency, for those that have a different non-USD currency
			insert into #PF  --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
				,  'REUTERS' as DATA_SOURCE
				,  'REUTERS' as ROOT_SOURCE
				,  r.UpdateDate as ROOT_SOURCE_DATE
				,  'A' as PERIOD_TYPE
				,  r.FiscalYear as PERIOD_YEAR
				,  r.PeriodEndDate as PERIOD_END_DATE
				,  'FISCAL' as FISCAL_TYPE
				,  @currency_code as CURRENCY
				,  r.DATA_ID as DATA_ID
				,  case when FX_CONV_TYPE = 'PIT' then r.Amount12Month / r.FX_RATE
						when FX_CONV_TYPE = 'AVG' then r.Amount12Month / r.AVG12MonthRATE
						else r.Amount12Month end
				,  'Get_Data_Annual Insert into #PF 5' as CALCULATION_DIAGRAM
				,  r.CurrencyReported as SOURCE_CURRENCY
				,  'ACTUAL' as AMOUNT_TYPE
			  from #Reuters r
			 inner join dbo.FX_RATES fx on fx.CURRENCY = @CURRENCY_CODE and fx.FX_DATE = r.PeriodEndDate
			 where r.CurrencyReported <> r.CountryCurrency
			   and r.CurrencyReported <> 'USD'
			   and isnull(r.FX_RATE, 0.0) <> 0.0
			   and isnull(r.AVG12MonthRATE, 0.0) <> 0.0

			if @VERBOSE = 'Y'
				BEGIN
					print 'After insert Otherwise1' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
					set @START = GETDATE()
					print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Copy Consensus data '
				END


			-- Collect the Consensus data that is not in PF already.
			select distinct cce.ISSUER_ID, cce.PERIOD_YEAR, min(cce.AMOUNT_TYPE) as AMOUNT_TYPE
			  into #CCE
			  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
			  left join #PF pf --dbo.PERIOD_FINANCIALS pf 
							on pf.ISSUER_ID = @ISSUER_ID and pf.FISCAL_TYPE = cce.FISCAL_TYPE
							and pf.PERIOD_YEAR = cce.PERIOD_YEAR and pf.PERIOD_TYPE = cce.PERIOD_TYPE
			 where cce.PERIOD_TYPE = 'A'
			   and pf.DATA_ID is null
			   and cce.ISSUER_ID = @ISSUER_ID
			 group by cce.ISSUER_ID, cce.PERIOD_YEAR	


			-- Copy Consensus data in where the year is missing from PERIOD_FINANCIALS
			insert into #PF  --dbo.PERIOD_FINANCIALS
						 (ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE,
						  ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, 
						  FISCAL_TYPE, CURRENCY, DATA_ID, AMOUNT, CALCULATION_DIAGRAM, 
						  SOURCE_CURRENCY, AMOUNT_TYPE)
			select cce.ISSUER_ID
				,  cce.SECURITY_ID
				,  ' ' as COA_TYPE
				,  cce.DATA_SOURCE
				,  'CONSENSUS' as ROOT_SOURCE
				,  cce.DATA_SOURCE_DATE as ROOT_SOURCE_DATE
				,  cce.PERIOD_TYPE
				,  cce.PERIOD_YEAR
				,  cce.PERIOD_END_DATE
				,  cce.FISCAL_TYPE
				,  cce.CURRENCY
				,  case ESTIMATE_ID when  2 then 118
									when  6 then 29
									when 11 then 44
									when 14 then 36
									when 17 then 11
									when  1 then 104
									when  3 then 117
									when  4 then 124
											end as DATA_ID
				,  case ESTIMATE_ID when  1 then cce.AMOUNT * ci.OutstandingShares / 1000000
									when  3 then cce.AMOUNT * ci.OutstandingShares / 1000000
									when  4 then cce.AMOUNT * ci.OutstandingShares / 1000000
											else cce.AMOUNT end as AMOUNT
				,  'Get_Data_Annual Insert into #PF 6' asCALCULATION_DIAGRAM
				,  cce.SOURCE_CURRENCY
				,  cce.AMOUNT_TYPE
			  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
			 inner join #CCE 
						 a on a.ISSUER_ID = cce.ISSUER_ID and a.PERIOD_YEAR = cce.PERIOD_YEAR and a.AMOUNT_TYPE = cce.AMOUNT_TYPE
			 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = @XREF
			 where cce.ESTIMATE_ID in (2, 6, 11, 14, 17, 1, 3, 4)
			   and cce.ISSUER_ID = @ISSUER_ID	-- If ISSUER_ID is not provided run all issuers
			   and cce.PERIOD_TYPE = 'A'

			 
			-- Copy Consensus data in where the year is missing from PERIOD_FINANCIALS
			-- For ESTIMATE_ID 11 must also create a DATA_ID 47
			insert into #PF  --dbo.PERIOD_FINANCIALS
						 (ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE,
						  ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, 
						  FISCAL_TYPE, CURRENCY, DATA_ID, AMOUNT, CALCULATION_DIAGRAM, 
						  SOURCE_CURRENCY, AMOUNT_TYPE)
			select cce.ISSUER_ID
				,  cce.SECURITY_ID
				,  ' ' as COA_TYPE
				,  cce.DATA_SOURCE
				,  'CONSENSUS' as ROOT_SOURCE
				,  cce.DATA_SOURCE_DATE as ROOT_SOURCE_DATE
				,  cce.PERIOD_TYPE
				,  cce.PERIOD_YEAR
				,  cce.PERIOD_END_DATE
				,  cce.FISCAL_TYPE
				,  cce.CURRENCY
				,  47 as DATA_ID
				,  cce.AMOUNT
				,  'Get_Data_Annual Insert into #PF special 47' asCALCULATION_DIAGRAM
				,  cce.SOURCE_CURRENCY
				,  cce.AMOUNT_TYPE
			  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
			 inner join #CCE 
						 a on a.ISSUER_ID = cce.ISSUER_ID and a.PERIOD_YEAR = cce.PERIOD_YEAR and a.AMOUNT_TYPE = cce.AMOUNT_TYPE
			 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = @XREF
			 where cce.ESTIMATE_ID = 11
			   and cce.ISSUER_ID = @ISSUER_ID
			   and cce.PERIOD_TYPE = 'A'


			if @VERBOSE = 'Y'
				BEGIN
					print 'After Copy Consensus data' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
					set @START = GETDATE()
					print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before Calendarize'
				END
/*
select * from #PF 
 where issuer_id = @ISSUER_ID
--   and PERIOD_YEAR = 2005
--   and DATA_ID = 121
   and FISCAL_TYPE = 'FISCAL'
 order by PERIOD_YEAR, PERIOD_TYPE, DATA_SOURCE, FISCAL_TYPE, CURRENCY, DATA_ID
*/

			-- Calendarize
			insert into #PF  --PERIOD_FINANCIALS
			select pfc.ISSUER_ID
				,  pfc.SECURITY_ID
				,  ' ' as COA_TYPE
				,  pfc.DATA_SOURCE
				,  pfc.ROOT_SOURCE
				,  pfc.ROOT_SOURCE_DATE
				,  pfc.PERIOD_TYPE
				,  pfc.PERIOD_YEAR
				-- calculate the correct new date
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
				--,  pfc.AMOUNT_TYPE
				,  case when pfn.AMOUNT_TYPE is not null 
								then pfn.AMOUNT_TYPE
								else pfc.AMOUNT_TYPE end as AMOUNT_TYPE --1/7/13 (JM): modified from pfc.amount_type to new case statement 1/7/13 (JM)
			  from #PF pfc  -- dbo.PERIOD_FINANCIALS pfc
			  left join #PF pfn  -- dbo.PERIOD_FINANCIALS pfn 
						 on pfn.SECURITY_ID = pfc.SECURITY_ID --pfn.ISSUER_ID = pfc.ISSUER_ID     
						 --and pfn.AMOUNT_TYPE = pfc.AMOUNT_TYPE  --1/7/13 (JM): removed join on amount_type as was not catching when data points that had actual (current) and estimate (next period)
						 and pfn.DATA_SOURCE = pfc.DATA_SOURCE and pfn.FISCAL_TYPE = pfc.FISCAL_TYPE
						 and pfn.PERIOD_TYPE = pfc.PERIOD_TYPE and pfn.PERIOD_YEAR = pfc.PERIOD_YEAR+1
						 and pfn.DATA_ID = pfc.DATA_ID         and pfn.CURRENCY = pfc.CURRENCY

			 inner join dbo.DATA_MASTER dm on dm.DATA_ID = pfc.DATA_ID
/*			 inner join (select distinct ReportNumber, PeriodLength, PeriodEndDate 
						   from Reuters.dbo.tblStatementRef
						  where PeriodLength is not NULL
						) sr on sr.ReportNumber = @ReportNumber and sr.PeriodEndDate = pfc.PERIOD_END_DATE
*/			 where pfc.FISCAL_TYPE = 'FISCAL'
--			 order by pfc.ISSUER_ID, pfc.AMOUNT_TYPE, pfc.DATA_SOURCE, pfc.CURRENCY, pfc.PERIOD_YEAR, pfc.PERIOD_TYPE, pfc.DATA_ID


			if @VERBOSE = 'Y'
				BEGIN
					print 'After Calendarize' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
					set @START = GETDATE()
					print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before delete'
				END

 
			begin transaction		-- Lock the table
			
			-- Copy the data from the temp table into the real table
			insert into dbo.PERIOD_FINANCIALS
			select * from #PF

			if @VERBOSE = 'Y'
				BEGIN
					print 'After Insert into PERIOD_FINANCIALS' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
					set @START = GETDATE()
				END
				
			commit					-- Save everything and unlock
			drop table #PF			-- Clean up

			if @VERBOSE = 'Y'
				BEGIN
					print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Done'
				END

		END

		--Clean up
		drop table #LATEST
		drop table #MultipleCheck
		drop table #MultipleLatest
		drop table #Reuters
		drop table #CCE

Go

