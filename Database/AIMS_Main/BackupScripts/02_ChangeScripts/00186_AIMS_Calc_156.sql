set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00185'
declare @CurrentScriptVersion as nvarchar(100) = '00186'

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
-- Purpose:	This procedure calculates the value for DATA_ID:156 Non-Provisioned NPL/Equity
--
--			(VRUQ-ALLA)/QTLE
--
-- Author:	Anupriya
-- Date:	August 16, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_156', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_156;
GO

CREATE procedure [dbo].[AIMS_Calc_156](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS  pf 
	 where DATA_ID = 259			--VRUQ		
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   
	   -- Get the data
	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS  pf 
	 where DATA_ID = 281			--ALLA		
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	
       -- Get the data
	select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS  pf 
	 where DATA_ID = 104			--QTLE	   
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select c.ISSUER_ID, c.SECURITY_ID, c.COA_TYPE, c.DATA_SOURCE, c.ROOT_SOURCE
		,  c.ROOT_SOURCE_DATE, c.PERIOD_TYPE, c.PERIOD_YEAR, c.PERIOD_END_DATE
		,  c.FISCAL_TYPE, c.CURRENCY
		,  156 as DATA_ID										-- DATA_ID:156 Non-Provisioned NPL/Equity
		,  (isnull(a.AMOUNT, 0.0) - isnull(b.AMOUNT, 0.0)) /c.AMOUNT as AMOUNT						-- (259 - 281)/104
		,    '(VRUQ(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') -  ALLA(' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ')) / QTLE('  + CAST(c.AMOUNT as varchar(32)) + ') ' as CALCULATION_DIAGRAM
		,  c.SOURCE_CURRENCY
		,  c.AMOUNT_TYPE
	  from #C c
	 inner join	#B b on b.ISSUER_ID = c.ISSUER_ID 
					and b.DATA_SOURCE = c.DATA_SOURCE and b.PERIOD_TYPE = c.PERIOD_TYPE
					and b.PERIOD_YEAR = c.PERIOD_YEAR and b.FISCAL_TYPE = c.FISCAL_TYPE
					and b.CURRENCY = c.CURRENCY
	inner join	#A a on a.ISSUER_ID = c.ISSUER_ID and a.DATA_SOURCE = c.DATA_SOURCE 
	                and a.PERIOD_TYPE = c.PERIOD_TYPE and a.PERIOD_YEAR = c.PERIOD_YEAR 
					and a.FISCAL_TYPE = c.FISCAL_TYPE and a.CURRENCY = c.CURRENCY	
	where 1=1 
	   and c.PERIOD_TYPE = 'A'
	   and isnull(c.AMOUNT, 0.0) <> 0.0	-- Data validation
	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 156 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 156 Non-Provisioned NPL/Equity.  DATA_ID:104 QTLE is NULL or ZERO'
			  from #C a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (
				
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 156 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 156 Non-Provisioned NPL/Equity.  DATA_ID:259 VRUQ is missing' as TXT
			  from #C a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
		    
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 156 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 156 Non-Provisioned NPL/Equity.  DATA_ID:281 ALLA is missing' as TXT
			  from #C a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 156 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 156 Non-Provisioned NPL/Equity.  DATA_ID:104 QTLE is missing' as TXT
			  from #C a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 156 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 156 Non-Provisioned NPL/Equity.  DATA_ID:259 VRUQ no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	
			
			select GETDATE() as LOG_DATE, 156 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 156 Non-Provisioned NPL/Equity.  DATA_ID:281 ALLA no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			) union (	
			
			select GETDATE() as LOG_DATE, 156 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 156 Non-Provisioned NPL/Equity.  DATA_ID:104 QTLE no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C




--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00186'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())