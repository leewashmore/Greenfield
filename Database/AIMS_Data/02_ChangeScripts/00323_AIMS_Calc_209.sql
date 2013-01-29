set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00322'
declare @CurrentScriptVersion as nvarchar(100) = '00323'

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
-- Purpose:	This procedure calculates the value for DATA_ID:209 Trailing P/BV
--
--			Current Market Cap* /Trailing QTLE**
--
-- Author:	Justin
-- Date:	January 25, 2013
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_209', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_209;
GO

CREATE procedure [dbo].[AIMS_Calc_209](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as

	-- Get the data
	declare @PERIOD_TYPE varchar(2)
	declare @PERIOD_END_DATE datetime

	select @PERIOD_TYPE = MIN(period_type), 
		@PERIOD_END_DATE = max(period_end_date) 
	from dbo.PERIOD_FINANCIALS pf  -- to find closest end_date to getdate
	where DATA_ID = 104  --Equity	
	and pf.ISSUER_ID = @ISSUER_ID
	and period_end_date < getdate() 
	and pf.FISCAL_TYPE = 'FISCAL'
	and pf.AMOUNT_TYPE = 'ACTUAL'
	group by PERIOD_END_DATE

	
	select distinct pf.* 
	   into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and sb.ISSUER_ID = @ISSUER_ID
	   and period_type = 'C'

 ---- Total amount for all the fiscal quarters within an year --- 

	select *          
	  into #B 
      from dbo.PERIOD_FINANCIALS  pf
	  where pf.DATA_ID = 104   -- QTLE
		and pf.FISCAL_TYPE = 'FISCAL'
		and pf.PERIOD_TYPE = @PERIOD_TYPE
		and pf.PERIOD_END_DATE = @PERIOD_END_DATE
		and pf.ISSUER_ID = @ISSUER_ID

	
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  209 as DATA_ID										-- DATA_ID:209 Trailing P/BV
		,  a.AMOUNT /b.AMOUNT as AMOUNT						-- Current Market Cap* / Trailing QTLE**
		,  'Mktcap(' + CAST(a.AMOUNT as varchar(32)) + ') / Trailing Equity(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B b on b.ISSUER_ID = sb.ISSUER_ID
					and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.CURRENCY = a.CURRENCY
	 where 1=1 	  
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	 --order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 209 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 209 Trailing P/BV. DATA_ID:104 QTLE is NULL or ZERO'
	  			  from #B a
			  where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error
			) union (	
			
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 209 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 209 Trailing P/BV .  DATA_ID:104 QTLE  is missing' as TXT
			  from #A a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B b on b.ISSUER_ID = sb.ISSUER_ID
							and b.DATA_SOURCE = a.DATA_SOURCE 
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	

			select GETDATE() as LOG_DATE, 209 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 209 Trailing P/BV .  DATA_ID:185 Market Cap is missing' as TXT
			  from #B a
			   inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = a.ISSUER_ID
			  left join	#A b on b.SECURITY_ID = sb.SECURITY_ID
							and b.DATA_SOURCE = a.DATA_SOURCE 
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 209 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 209 Trailing P/BV .  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 209 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 209 Trailing P/BV.  DATA_ID:104 QTLE No data or missing quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	




--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00323'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())