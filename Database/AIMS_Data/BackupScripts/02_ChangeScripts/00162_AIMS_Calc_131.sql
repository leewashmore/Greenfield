set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00161'
declare @CurrentScriptVersion as nvarchar(100) = '00162'

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
            IF OBJECT_ID ( 'AIMS_Calc_131', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_131;
GO
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:131 EPS
--
--			NINC / (QTCO + QTPO)
--
-- Author:	David Muench
-- Date:	July 2, 2012
------------------------------------------------------------------------
create procedure AIMS_Calc_131(
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 290					-- Earnings
	   and pf.ISSUER_ID = @ISSUER_ID

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 293					-- Reuters Shares Outstanding 
	   and pf.ISSUER_ID = @ISSUER_ID
/*
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 44					-- NINC
	   and pf.ISSUER_ID = @ISSUER_ID

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 106					-- QTCO
	   and pf.ISSUER_ID = @ISSUER_ID

	select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 107					-- QTPO
	   and pf.ISSUER_ID = @ISSUER_ID
*/
	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select b.ISSUER_ID, b.SECURITY_ID, b.COA_TYPE, b.DATA_SOURCE, b.ROOT_SOURCE
		,  b.ROOT_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  131 as DATA_ID										-- DATA_ID:131 EPS
		,  isnull(a.AMOUNT, 0.0) / b.AMOUNT  as AMOUNT			-- NINC / (QTCO + QTPO)
		,  'Earnings(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') / Reuters Shares Outstanding(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
--		,  isnull(b.AMOUNT, 0.0) / ( b.AMOUNT + isnull(c.AMOUNT, 0.0)) as AMOUNT			-- NINC / (QTCO + QTPO)
--		,  'NINC(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') / + ( QTCO(' + CAST(b.AMOUNT as varchar(32)) + ') + QTPO('  + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') )' as CALCULATION_DIAGRAM
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
	  from #B b
	  left join	#A a on a.ISSUER_ID = b.ISSUER_ID 
					and a.DATA_SOURCE = b.DATA_SOURCE and a.PERIOD_TYPE = b.PERIOD_TYPE
					and a.PERIOD_YEAR = b.PERIOD_YEAR and a.FISCAL_TYPE = b.FISCAL_TYPE
					and a.CURRENCY = b.CURRENCY
--	  left join	#C c on c.ISSUER_ID = b.ISSUER_ID 
--					and c.DATA_SOURCE = b.DATA_SOURCE and c.PERIOD_TYPE = b.PERIOD_TYPE
--					and c.PERIOD_YEAR = b.PERIOD_YEAR and c.FISCAL_TYPE = b.FISCAL_TYPE
--					and c.CURRENCY = b.CURRENCY
	 where 1=1 
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or ZERO data
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 131 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 131:EPS.  DATA_ID:293 Reuters Shares Outstanding is NULL or ZERO'
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 131 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 131:EPS.  DATA_ID:290 Earnings is missing' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (
			 
			 -- Error conditions - missing data 
/*			select GETDATE() as LOG_DATE, 131 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'WARNING calculating 131:EPS.  DATA_ID:290 Earnings is missing' as TXT
			  from #B a
			  left join	#C b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (
*/			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 131 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 131:EPS.  DATA_ID:44 NINC no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 131 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 131:EPS.  DATA_ID:106 QTCO no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
/*			) union (
			select GETDATE() as LOG_DATE, 131 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 131:EPS.  DATA_ID:107 QTPO no data' as TXT
			  from (select COUNT(*) CNT  from #C having COUNT(*) = 0) z
*/
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
--	drop table #C
go

-- exec AIMS_Calc_131 128251
-- select * from PERIOD_FINANCIALS where DATA_ID = 131


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00162'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())