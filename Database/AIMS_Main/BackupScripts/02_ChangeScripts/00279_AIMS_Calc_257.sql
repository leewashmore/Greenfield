set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00278'
declare @CurrentScriptVersion as nvarchar(100) = '00279'

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
-- Purpose:	This procedure calculates the value for DATA_ID:257 Trailing Minority Interest  
--
--			:   Previous Annual LMIN*
--
-- Author:	Anupriya
-- Date:	July 27, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_257', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_257;
GO

Create procedure [dbo].[AIMS_Calc_257](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	
	declare @PERIOD_TYPE varchar(2)
	declare @PERIOD_END_DATE datetime

	select @PERIOD_TYPE = MIN(period_type), 
		@PERIOD_END_DATE = max(period_end_date) 
	from dbo.PERIOD_FINANCIALS pf  -- to find closest end_date to getdate
	where DATA_ID = 92  --LMIN		
	and pf.ISSUER_ID = @ISSUER_ID
	and period_end_date < getdate() 
	and pf.FISCAL_TYPE = 'FISCAL'
	and pf.AMOUNT_TYPE = 'ACTUAL'
	group by PERIOD_END_DATE


	select pf.* 
		 into #A
		from dbo.PERIOD_FINANCIALS pf 
		where DATA_ID = 92			-- LMIN
		   and pf.ISSUER_ID = @ISSUER_ID
		   and pf.PERIOD_TYPE = @PERIOD_TYPE
		   and pf.FISCAL_TYPE = 'FISCAL'
		   and period_end_date = @PERIOD_END_DATE	
	
 
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  '' as FISCAL_TYPE, a.CURRENCY
		,  257 as DATA_ID										-- 257 Trailing Minority Interest 
		,  a.AMOUNT as AMOUNT						
		,  'Previous Minority Interest(' + CAST(a.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a	
	 where 1=1 	  
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 257 as DATA_ID, a.ISSUER_ID, 'C'
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 257 Trailing Minority Interest. DATA_ID:92 is NULL or ZERO'
			  from #A a
			where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 257 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 257 Trailing Minority Interest.  DATA_ID:92 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A	
	
GO



--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00279'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())