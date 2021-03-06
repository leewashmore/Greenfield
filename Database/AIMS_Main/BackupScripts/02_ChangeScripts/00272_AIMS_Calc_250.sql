set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00271'
declare @CurrentScriptVersion as nvarchar(100) = '00272'

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
            
IF OBJECT_ID ( 'AIMS_Calc_250', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_250;
GO
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:250 Forward P/BV Relative to Industry
-- 188 / Forward P/BV for Industry of Security*
-- Author:	Prerna
-- Date:	Aug 22, 2012
------------------------------------------------------------------------
Create procedure [dbo].[AIMS_Calc_250](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 188					
	   and sb.ISSUER_ID = @ISSUER_ID

	select bf.*, sb.SECURITY_ID
	  into #B
	  from dbo.BENCHMARK_NODE_FINANCIALS bf
	 inner join GF_SECURITY_BASEVIEW sb on sb.GICS_INDUSTRY = bf.NODE_ID1
	 where bf.DATA_ID = 188
	   and sb.ISSUER_ID = @ISSUER_ID
	   and bf.BENCHMARK_ID = 'MSCI EM NET'
	   and bf.NODE_NAME1 = 'INDUSTRY'
	   and bf.NODE_NAME2 is null
	   and bf.PERIOD_TYPE = 'C' 
	    

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  250 as DATA_ID							-- DATA_ID:250 Forward P/BV Relative to Industry
		,  a.AMOUNT / b.AMOUNT as AMOUNT			-- 188 / Forward P/BV for Industry of Security*
		,  '188(' + CAST(a.AMOUNT as varchar(32)) + ') / (Forward P/BV('  + CAST(b.AMOUNT as varchar(32))  + ')'   as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.SECURITY_ID = a.SECURITY_ID
					and b.PERIOD_TYPE = a.PERIOD_TYPE					
					and b.CURRENCY = a.CURRENCY
	 where 1=1 	  
	  and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation	

	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 250 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 250: Forward P/BV Relative to Industry.  Forward P/BV Relative to Industry is NULL or ZERO'
			  from #A a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error	  
			-- Error conditions - missing data 
			) union (	
				select GETDATE() as LOG_DATE, 250 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating  250: Forward P/BV Relative to Industry.   DATA_ID:188 Forward P/BV for Industry of Security is missing' as TXT
			  from #A a
			  left join	#B b on b.SECURITY_ID = a.SECURITY_ID
							and b.PERIOD_TYPE = a.PERIOD_TYPE					
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.SECURITY_ID is NULL	 
			) union	(
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 250 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 250: Forward P/BV Relative to Industry.  DATA_ID:188 is missing' as TXT
			  from #B b
			  left join	#A a on a.SECURITY_ID = b.SECURITY_ID
							and a.PERIOD_TYPE = b.PERIOD_TYPE					
							and a.CURRENCY = b.CURRENCY
			 where 1=1 and a.ISSUER_ID is NULL	  
			) union	(
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 250 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 250: Forward P/BV Relative to Industry.  DATA_ID:188 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union	(
			select GETDATE() as LOG_DATE, 250 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 250: Forward P/BV Relative to Industry.  DATA_ID:188 Forward P/BV no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00272'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())