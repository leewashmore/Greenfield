set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00312'
declare @CurrentScriptVersion as nvarchar(100) = '00313'

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
            ------------------------------------------------------------------------
-- Purpose:	This procedure gets the quarterly data from the Internal
--			tables and creates the quarterly data in the PERIOD_FINANCIALS
--			table for the analysts.
--
-- Author:	David Muench
-- Date:	July 18, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'INTERNAL_QUARTERLY_EXTRACT', 'P' ) IS NOT NULL 
DROP PROCEDURE INTERNAL_QUARTERLY_EXTRACT;
GO

CREATE procedure INTERNAL_QUARTERLY_EXTRACT (
	@ISSUER_ID			varchar(20)	= NULL			-- The company identifier		
,	@VERBOSE			char		= 'Y'
)
as

	-- Gather data that we have


	-- Gather the Actual Quarterly data from the Internal Analysts
	Select i.*
		,  cast(PERIOD_YEAR as varchar(4)) + 'A' as PERIOD
	  into #A
	  from INTERNAL_STATEMENT i
	 inner join dbo.INTERNAL_DATA id on id.ISSUER_ID = i.ISSUER_ID and id.REF_NO = i.REF_NO
	 where i.ISSUER_ID = @ISSUER_ID
	   and i.ROOT_SOURCE = 'PRIMARY'
	   and i.AMOUNT_TYPE = 'ACTUAL'
	   and id.PERIOD_TYPE in ('Q1','Q2','Q3','Q4')
	   and i.ROOT_SOURCE_DATE > DATEADD(day, -120, getdate())	-- Stale date 1/7/13 (JM) Modified from 90 to 120 days per FM
	;

--select 'A' as A, * from #A

	-- Gather any Actual Quarterly data from Reuters
	-- that is not already gathered
	Select *
		,  cast(PERIOD_YEAR as varchar(4)) + 'A' as PERIOD
	  into #B
	  from PERIOD_FINANCIALS 
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_SOURCE = 'REUTERS'
	   and PERIOD_TYPE in ('Q1','Q2','Q3','Q4')
	   and FISCAL_TYPE = 'FISCAL'
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #A)  ;
	;

--select 'B' as B, * from #B

	-- Gather the Estimated Quarterly data from the Internal Analysts
	Select i.*
		,  cast(PERIOD_YEAR as varchar(4)) + 'A' as PERIOD
	  into #C
	  from INTERNAL_STATEMENT i
	 inner join dbo.INTERNAL_DATA id on id.ISSUER_ID = i.ISSUER_ID and id.REF_NO = i.REF_NO
	 where i.ISSUER_ID = @ISSUER_ID
	   and i.ROOT_SOURCE = 'PRIMARY'
	   and i.AMOUNT_TYPE = 'ESTIMATE'
	   and id.PERIOD_TYPE in ('Q1','Q2','Q3','Q4')
	   and i.ROOT_SOURCE_DATE > DATEADD(day, -120, getdate())	-- Stale date 1/7/13 (JM) Modified from 90 to 120 days per FM
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #A)  
	   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #B)  
	;

--select 'C' as C, * from #C

		-- Gather any Estimated Annual data from Reuters
		-- that is not already gathered
		Select *
			,  cast(PERIOD_YEAR as varchar(4)) + 'A' as PERIOD
		  into #D
		  from PERIOD_FINANCIALS 
		 where ISSUER_ID = @ISSUER_ID
		   and DATA_SOURCE = 'REUTERS'
		   and AMOUNT_TYPE = 'ESTIMATE'
		   and PERIOD_TYPE in ('Q1','Q2','Q3','Q4')
		   and FISCAL_TYPE = 'FISCAL'
		   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #A)
		   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #B)
		   and cast(PERIOD_YEAR as varchar(4)) + PERIOD_TYPE not in (select distinct PERIOD from #C)
		;

--select 'D' as D, * from #D


	-- Gather the COA_TYPES, use INTERNAL_ISSUER first
	select ii.ISSUER_ID, ii.COA_TYPE
	  into #COA_TYPE
	  from dbo.INTERNAL_ISSUER ii
	 where ii.ISSUER_ID = @ISSUER_ID
	;

	insert into #COA_TYPE
	select distinct sb.ISSUER_ID, max(sci.COAType)
	  from dbo.GF_SECURITY_BASEVIEW sb 
	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = sb.REPORTNUMBER
	 where sb.ISSUER_ID = @ISSUER_ID
	   and sb.ISSUER_ID not in (select ISSUER_ID from #COA_TYPE)
	 group by sb.ISSUER_ID
	;

--select 'COA' as COA_TYPE, * from #COA_TYPE

	-- Gather the required COA codes by COA_TYPE
	select distinct dm.COA
	  into #DM
	  from dbo.DATA_MASTER dm
	 inner join #COA_TYPE ct on (ct.COA_TYPE = 'BNK' and dm.BANK = 'Y')
							 or (ct.COA_TYPE = 'IND' and dm.INDUSTRIAL = 'Y')
							 or (ct.COA_TYPE = 'FIN' and dm.INSURANCE = 'Y')
							 or (ct.COA_TYPE = 'UTL' and dm.UTILITY = 'Y')
	 where (dm.COA is not NULL and dm.COA > ' ')
	   and dm.QUARTERLY = 'Y'


--select 'DM' as DM, * from #DM

	-- Gather it all together
	SELECT ISSUER_ID, ROOT_SOURCE, ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR
		,  PERIOD_END_DATE, AMOUNT, CURRENCY, AMOUNT_TYPE, COA
	  into #R
	  from (
			select i.ISSUER_ID, i.ROOT_SOURCE, i.ROOT_SOURCE_DATE, i.PERIOD_YEAR, id.COA
				,  i.PERIOD_END_DATE, i.CURRENCY, i.AMOUNT_TYPE, id.PERIOD_TYPE, id.AMOUNT
			  from dbo.INTERNAL_DATA id
			 inner join dbo.INTERNAL_STATEMENT i on i.ISSUER_ID = id.ISSUER_ID and i.REF_NO = id.REF_NO
			 inner join (select * from #A union select * from #C) a 
					on a.ISSUER_ID = id.ISSUER_ID and a.REF_NO = id.REF_NO
			 inner join #DM dm on dm.COA = id.COA
			 where 1=1
			   and id.PERIOD_TYPE in ('Q1','Q2','Q3','Q4')
		union
			select b.ISSUER_ID, b.ROOT_SOURCE, b.ROOT_SOURCE_DATE, b.PERIOD_YEAR, dm.COA
				,  b.PERIOD_END_DATE, b.CURRENCY, b.AMOUNT_TYPE, b.PERIOD_TYPE, b.AMOUNT
			  from #B b
			 inner join dbo.DATA_MASTER dm on dm.DATA_ID = b.DATA_ID
		union
			select d.ISSUER_ID, d.ROOT_SOURCE, d.ROOT_SOURCE_DATE, d.PERIOD_YEAR, dm.COA
				,  d.PERIOD_END_DATE, d.CURRENCY, d.AMOUNT_TYPE, d.PERIOD_TYPE, d.AMOUNT
			  from #D d
			 inner join dbo.DATA_MASTER dm on dm.DATA_ID = d.DATA_ID
			) z
	;

--select 'R' as R, * from #R

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
		,  ' ' as CALCULATION_DIAGRAM
		,  r.CURRENCY as SOURCE_CURRENCY
		,  r.AMOUNT_TYPE
		,  fx.FX_RATE
		,  fx.AVG90DAYRATE
		,  fx.AVG12MonthRATE
		,  dm.FX_CONV_TYPE
	  into #RESULT
	  from #R r
	 inner join #COA_TYPE ct on ct.ISSUER_ID = r.ISSUER_ID
	 inner join (select distinct ISSUER_ID, ISO_COUNTRY_CODE from dbo.GF_SECURITY_BASEVIEW) i
				on i.ISSUER_ID = r.ISSUER_ID
	 inner join dbo.Country_Master cm on cm.COUNTRY_CODE = i.ISO_COUNTRY_CODE
	 inner join dbo.FX_RATES fx on fx.CURRENCY = cm.CURRENCY_CODE
	 inner join dbo.DATA_MASTER dm on dm.COA = r.COA and r.PERIOD_END_DATE = fx.FX_DATE
	;
	 
--select 'RESULT' as RESULT, * from #RESULT

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
		,  'Quarterly Insert for Local' as CALCULATION_DIAGRAM
		,  SOURCE_CURRENCY
		,  AMOUNT_TYPE
	  from #RESULT


	-- Finally insert it all into the PERIOD_FINANCIALS table for USD Currency
/*
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
		,  'USD' as CURRENCY
		,  DATA_ID
		,  case when FX_CONV_TYPE = 'NONE' then AMOUNT
				when FX_CONV_TYPE = 'AVG' and isnull(AVG90DAYRATE, 0.0) <> 0.0 then AMOUNT / AVG90DAYRATE
				when FX_CONV_TYPE = 'PIT' and isnull(FX_RATE, 0.0) <> 0.0 then AMOUNT / FX_RATE
				else 0.0 end as AMOUNT
		,  case when FX_CONV_TYPE = 'NONE' then 'Quarterly Insert for USD - NONE ' --CALCULATION_DIAGRAM
				when FX_CONV_TYPE = 'AVG' and isnull(AVG90DAYRATE, 0.0) = 0.0 then 'Quarterly Insert for USD - Average 90 day FX Rate is bad'
				when FX_CONV_TYPE = 'PIT' and isnull(FX_RATE, 0.0) = 0.0 then 'Quarterly Insert for USD - Daily FX Rate is bad'
				else 'Quarterly Insert for USD - no FX ' end as CALCULATION_DIAGRAM
		,  SOURCE_CURRENCY
		,  AMOUNT_TYPE
	  from #RESULT
*/
	-- Clean up temp tables
	drop table #A
	drop table #B
	drop table #C
	drop table #COA_TYPE
	drop table #DM
	drop table #R
	drop table #RESULT


go

-- exec INTERNAL_QUARTERLY_EXTRACT 8131602
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00313'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())