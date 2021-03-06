set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00258'
declare @CurrentScriptVersion as nvarchar(100) = '00259'

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
-- Purpose:	This procedure calculates the value for DATA_ID:234 Net Income Growth, 3 Year (Annualized) 
--
--			(((177 for Period * 177 for Period -1 * 177 for Period -2)^1/3)-1)*100
--
-- Author:	Shivani 
-- Date:	July 17, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_234', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_234;
GO

CREATE procedure [dbo].[AIMS_Calc_234](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 177			--
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	 order by PERIOD_YEAR

-- data for period - 1
	 
	select pf.PERIOD_YEAR + 1 as PRIOR_YEAR, pf.PERIOD_YEAR as PERIOD_YEAR,pf.CURRENCY,pf.COA_TYPE,pf.DATA_SOURCE,pf.FISCAL_TYPE,pf.PERIOD_TYPE,pf.ISSUER_ID,pf.AMOUNT,pf.PERIOD_END_DATE
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 177
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	 order by PERIOD_YEAR
	 
-- Data for period - 2	 

	select pf.PERIOD_YEAR + 2 as PRIOR_YEAR, pf.PERIOD_YEAR as PERIOD_YEAR,pf.CURRENCY,pf.COA_TYPE,pf.DATA_SOURCE,pf.FISCAL_TYPE,pf.PERIOD_TYPE,pf.ISSUER_ID,pf.AMOUNT,pf.PERIOD_END_DATE
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 177
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	 order by PERIOD_YEAR

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  234 as DATA_ID										-- DATA_ID:234 Net Income Growth, 3 Year (Annualized)  
		,  (power(cast((a.AMOUNT * b.AMOUNT * c.amount) as float),(CAST (1 as float ) / CAST (3 as float))) -1) * 100 as AMOUNT
		,  '(((177 for('+ CAST(a.PERIOD_YEAR as varchar(32)) + '(' + CAST(a.AMOUNT as varchar(32)) + ') * 177 for ('+ CAST(b.PERIOD_YEAR as varchar(32)) +')(' + CAST(b.AMOUNT as varchar(32)) + ')) * 177 for ('+ CAST(c.PERIOD_YEAR as varchar(32)) +')(' + CAST(c.AMOUNT as varchar(32)) + ')) ^ 1/3) -1) * 100)' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PRIOR_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
     inner join	#C c on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PRIOR_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY					
	 where 1=1 	   
	   and (a.AMOUNT * b.AMOUNT * c.amount) > 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 235 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '1/1/1900' as PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 234 Net Income Growth, 3 Year (Annualized) . Calculated amount is negative'
			  from #A a
			 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PRIOR_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 inner join	#C c on c.ISSUER_ID = a.ISSUER_ID 
							and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
							and c.PRIOR_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
							and c.CURRENCY = a.CURRENCY		
				
			where 1=1	
			 and (a.AMOUNT * b.AMOUNT * c.amount) < 0.0	-- Data validation 
			 ) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 234 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 234 Net Income Growth, 3 Year (Annualized)  .  DATA_ID:177 is missing for the year' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PRIOR_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data for prior period
			select GETDATE() as LOG_DATE, 234 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 234 Net Income Growth, 3 Year (Annualized)  .  DATA_ID:177 is missing for the period -1' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PRIOR_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data for prior period
			select GETDATE() as LOG_DATE, 234 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 234 Net Income Growth, 3 Year (Annualized)  .  DATA_ID:177 is missing for the period -2' as TXT
			  from #C a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PRIOR_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 234 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 234 Net Income Growth, 3 Year (Annualized)  .  DATA_ID:177 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
	




--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00259'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())