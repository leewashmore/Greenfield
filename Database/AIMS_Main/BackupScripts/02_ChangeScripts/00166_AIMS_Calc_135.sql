set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00165'
declare @CurrentScriptVersion as nvarchar(100) = '00166'

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
            IF OBJECT_ID ( 'AIMS_Calc_135', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_135;
GO
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:135 Cost of Funds
--
--			(STIE for Year/ AVERAGE(LIBD+LODP+SOBL+STLD for Year, LIBD+LODP+SOBL + STLD) for Prior Year))-1
--
-- Author:	David Muench
-- Date:	July 2, 2012
------------------------------------------------------------------------
create procedure AIMS_Calc_135(
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as

	-- Get the data
	select ISSUER_ID, SECURITY_ID, ' ' as COA_TYPE, DATA_SOURCE, max(ROOT_SOURCE) as ROOT_SOURCE, max(ROOT_SOURCE_DATE) as ROOT_SOURCE_DATE, PERIOD_TYPE
		,  PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE,CURRENCY, 0 as DATA_ID, sum(AMOUNT) as AMOUNT
		,  ' ' as CALCULATION_DIAGRAM, max(SOURCE_CURRENCY) as SOURCE_CURRENCY, min(AMOUNT_TYPE) as AMOUNT_TYPE
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 16					-- STIE
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	 group by ISSUER_ID, SECURITY_ID, DATA_SOURCE
		,  PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE,CURRENCY


	select ISSUER_ID, SECURITY_ID, ' ' as COA_TYPE, DATA_SOURCE, max(ROOT_SOURCE) as ROOT_SOURCE, max(ROOT_SOURCE_DATE) as ROOT_SOURCE_DATE, PERIOD_TYPE
		,  PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE,CURRENCY, 0 as DATA_ID, sum(AMOUNT) as AMOUNT
		,  ' ' as CALCULATION_DIAGRAM, max(SOURCE_CURRENCY) as SOURCE_CURRENCY, min(AMOUNT_TYPE) as AMOUNT_TYPE
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID in (262, 263, 81, 90)		-- LIBD+LODP+SOBL + STLD
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	 group by ISSUER_ID, SECURITY_ID, DATA_SOURCE
		,  PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE,CURRENCY



/*	select ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE, ROOT_SOURCE_DATE, PERIOD_TYPE
		,  PERIOD_YEAR+1 as PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE,CURRENCY, DATA_ID, sum(AMOUNT) as AMOUNT
		,  CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID in (262, 263, 81, 90)		-- LIBD+LODP+SOBL + STLD
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	 group by ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE, ROOT_SOURCE_DATE
		,  PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE,CURRENCY, DATA_ID
		,  CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE
*/

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select b.ISSUER_ID, b.SECURITY_ID, b.COA_TYPE, b.DATA_SOURCE, b.ROOT_SOURCE
		,  b.ROOT_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  135 as DATA_ID										-- DATA_ID:135 Yield on Interest Earning Assets
		,  (isnull(a.AMOUNT, 0.0) / ((b.AMOUNT + isnull(c.AMOUNT, 0.0)) / 2))  AMOUNT			-- - 1
		,  '(STIE(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') / (LIBD+LODP+SOBL+STLD this year(' + CAST(b.AMOUNT as varchar(32)) + ' + LIBD+LODP+SOBL+STLD prior year(' + CAST(isnull(c.AMOUNT, b.AMOUNT) as varchar(32)) + ') / 2) - 1' as CALCULATION_DIAGRAM
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
	  from #B b
	  left join	#A a on a.ISSUER_ID = b.ISSUER_ID 
					and a.DATA_SOURCE = b.DATA_SOURCE and a.PERIOD_TYPE = b.PERIOD_TYPE
					and a.PERIOD_YEAR = b.PERIOD_YEAR and a.FISCAL_TYPE = b.FISCAL_TYPE
					and a.CURRENCY = b.CURRENCY
	 inner join	#B c on c.ISSUER_ID = b.ISSUER_ID 
					and c.DATA_SOURCE = b.DATA_SOURCE and c.PERIOD_TYPE = b.PERIOD_TYPE
					and c.PERIOD_YEAR+1 = b.PERIOD_YEAR and c.FISCAL_TYPE = b.FISCAL_TYPE
					and c.CURRENCY = b.CURRENCY
	 where 1=1 
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 135 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 135:Cost of Funds.  This Year LIBD+LODP+SOBL+STLD is NULL or ZERO' as TXT
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			) union (

/*			select GETDATE() as LOG_DATE, 135 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 135:Cost of Funds.  Prior Year LIBD+LODP+SOBL+STLD is NULL or ZERO' as TXT
			  from #C a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			) union (
*/			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 135 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 135:Cost of Funds.  DATA_ID:58 SOEA is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 135 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 135:Cost of Funds.  DATA_ID:9 SIIB is missing'
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 135 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARN calculating 135:Cost of Funds.  DATA_ID:9 SIIB no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 135 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 135:Cost of Funds.  This Year LIBD+LODP+SOBL+STLD no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
/*			) union (
			select GETDATE() as LOG_DATE, 135 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARN calculating 135:Cost of Funds.  Prior Year LIBD+LODP+SOBL+STLD no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
*/			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
--	drop table #C
go

-- delete from PERIOD_FINANCIALS where DATA_ID = 135 and ISSUER_ID = 187318
-- exec AIMS_Calc_135 187318
-- select * from PERIOD_FINANCIALS where DATA_ID = 135 and ISSUER_ID = 187318


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00166'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())