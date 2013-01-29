set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00305'
declare @CurrentScriptVersion as nvarchar(100) = '00306'

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
-- Purpose:	This procedure calculates the value for DATA_ID:177 Net Income Growth (YOY) 
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
--
-- Author:	David Muench
-- Date:	Sept 6, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'CCE_Calc_177', 'P' ) IS NOT NULL 
DROP PROCEDURE CCE_Calc_177;
GO

create procedure [dbo].[CCE_Calc_177](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	select cce.* 
	  into #A
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 11			-- Net Income
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	select cce.PERIOD_YEAR+1 as PERIOD_YEAR, cce.CURRENCY, cce.DATA_SOURCE, cce.AMOUNT_TYPE
		,  cce.FISCAL_TYPE, cce.PERIOD_TYPE, cce.ISSUER_ID, cce.AMOUNT, cce.PERIOD_END_DATE
	  into #B
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 11			-- Net Income
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select a.ISSUER_ID, a.SECURITY_ID, b.DATA_SOURCE
		,  a.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  177 as ESTIMATE_ID										-- DATA_ID:166 P/E 
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- 185/NINC  
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	   and b.AMOUNT > 0					-- must not be negative
	   and a.AMOUNT >= 0				-- must not be negative

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  ESTIMATE_ID:11 is NULL or ZERO for prior period'
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error
	 			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  ESTIMATE_ID:11 is missing for the year' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
	  			) union (	

			-- Error conditions - missing data for prior period
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  ESTIMATE_ID:11 is missing for the prior period' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
	  			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 177 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  ESTIMATE_ID:11 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END

	-- Clean up
	drop table #A
	drop table #B
	



-- exec CCE_Calc_177 '847547'
-- select * from CURRENT_CONSENSUS_ESTIMATES where ESTIMATE_ID = 177


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00306'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())