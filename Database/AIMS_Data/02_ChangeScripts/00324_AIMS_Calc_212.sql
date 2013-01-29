set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00323'
declare @CurrentScriptVersion as nvarchar(100) = '00324'

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
-- Purpose:	This procedure calculates the value for DATA_ID:212 Trailing ROE
--
--			:  Trailing Earnings**/Previous  QTLE*
--
-- Author:	Justin Machata
-- Date:	January 25, 2013
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_212', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_212;
GO

CREATE procedure [dbo].[AIMS_Calc_212](
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
	where DATA_ID = 290  --Earnings		
	and pf.ISSUER_ID = @ISSUER_ID
	and period_end_date < getdate() 
	and pf.FISCAL_TYPE = 'FISCAL'
	and pf.AMOUNT_TYPE = 'ACTUAL'
	group by PERIOD_END_DATE
	
	--Trailing Earnings
	select pf.AMOUNT, pf.ISSUER_ID, pf.FISCAL_TYPE, pf.COA_TYPE, pf.DATA_SOURCE, pf.CURRENCY, datepart(year,getdate())as Current_Year
	into #A
	from PERIOD_FINANCIALS pf
	where 1=0
	
	if @PERIOD_TYPE = 'A'
		insert into #A
		select f.AMOUNT, f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY,datepart(year,getdate())as Current_Year          
			 from (select * 
						   from dbo.PERIOD_FINANCIALS  pf
						  where pf.DATA_ID = 290   -- Earnings
							and pf.FISCAL_TYPE = 'FISCAL'
							and pf.PERIOD_TYPE = 'A'
							and pf.PERIOD_END_DATE = @PERIOD_END_DATE        
							and pf.ISSUER_ID = @ISSUER_ID
						 ) f
	else
		insert into #A
		select sum(f.amount)as AMOUNT, f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY,datepart(year,getdate())as Current_Year          
			 from (select * 
						   from dbo.PERIOD_FINANCIALS  pf
						  where pf.DATA_ID = 290   -- Earnings
							and pf.FISCAL_TYPE = 'FISCAL'
							and pf.PERIOD_TYPE like 'Q%'
							and pf.PERIOD_END_DATE <= @PERIOD_END_DATE          
							and pf.PERIOD_END_DATE > DATEADD( month, -12, @PERIOD_END_DATE)   -- only 4 quarters
							and pf.ISSUER_ID = @ISSUER_ID
						 ) f
			group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
		having count(distinct PERIOD_TYPE) = 4 
	
	
	--Previous QTLE, regardless of period type 
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
	select  distinct b.ISSUER_ID, b.SECURITY_ID, b.COA_TYPE, b.DATA_SOURCE, b.ROOT_SOURCE
		,  b.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  b.FISCAL_TYPE, b.CURRENCY
		,  212 as DATA_ID										-- DATA_ID:212 DATA_ID:212 Trailing ROE
		,  a.AMOUNT /b.AMOUNT as AMOUNT						-- Trailing Earnings**/Previous QTLE*
		,  'Trailing Earnings(' + CAST(a.AMOUNT as varchar(32)) + ') / Previous QTLE(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.CURRENCY = a.CURRENCY
	 where 1=1 	  
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	-- order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 212 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 212 Trailing ROE. DATA_ID:290 Earnings is NULL or ZERO'
			  from #A a
			where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 212 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 212 Trailing ROE.  DATA_ID:290 Earnings is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE 
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	

			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 212 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 212 Trailing ROE .  DATA_ID:290 Earnings no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 212 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 212 Trailing ROE.  DATA_ID:104 QTLE  No data or missing quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	




--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00324'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())