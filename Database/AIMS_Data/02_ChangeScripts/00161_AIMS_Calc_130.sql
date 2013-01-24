set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00160'
declare @CurrentScriptVersion as nvarchar(100) = '00161'

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
            IF OBJECT_ID ( 'AIMS_Calc_130', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_130;
GO
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:130 EBITDA
--
--			SOPI + SDPR
--
-- Author:	David Muench
-- Date:	July 2, 2012
------------------------------------------------------------------------
create procedure AIMS_Calc_130(
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the log table
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 29					-- SOPI
	   and pf.ISSUER_ID = @ISSUER_ID

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.ISSUER_ID = @ISSUER_ID
	   and (  (pf.PERIOD_TYPE like 'Q%' and pf.DATA_ID = 24)		-- SDPR
	       or (pf.PERIOD_TYPE = 'A' and pf.DATA_ID = 291) )			-- Depreciation

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  130 as DATA_ID							-- DATA_ID:130 EBITDA
		,  a.AMOUNT + isnull(b.AMOUNT, 0.0) as AMOUNT			-- SOPI + SDPR
		,  'SOPI(' + CAST(a.AMOUNT as varchar(32)) + ') + SDPR('  + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - missing data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 130 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 130:EBITDA.  DATA_ID:24 SDPR or 291 Depreciation is missing'
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL

			) union	(

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 130 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 130:EBITDA.  DATA_ID:29 SOPI is missing' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union	(
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 130 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 130:EBITDA.  DATA_ID:24 SDPR or 291 Depreciation no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			) union	(
			select GETDATE() as LOG_DATE, 130 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 130:EBITDA.  DATA_ID:29 SOPI no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
go

-- exec AIMS_Calc_130 '330892'

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00161'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())