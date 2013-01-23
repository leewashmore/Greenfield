------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:200 Forward ROE
--
--			:   Sum of next 4 quarters NINC**/Next Annual QTLE*
--
-- Author:	Shivani
-- Date:	July 18, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_200', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_200;
GO

CREATE procedure [dbo].[AIMS_Calc_200](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

--print 'InCalc 200'
	-- Calculate the percentage of the amount to use.
	declare @PERCENTAGE decimal(32,6)
	declare @PERIOD_END_DATE datetime
	declare @PERIOD_YEAR integer
	select @PERCENTAGE = cast(datediff(day, getdate(), MIN(period_end_date)) as decimal(32,6)) / 365.0
	   ,   @PERIOD_END_DATE = MIN(period_end_date)
	  from PERIOD_FINANCIALS
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_ID = 104
	   and PERIOD_END_DATE > GETDATE()
	   and PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
--print 'Percentage = ' + cast(@PERCENTAGE as varchar(20))
--print 'PERIOD_END_DATE = ' + cast(@PERIOD_END_DATE as varchar(32))
	select @PERIOD_YEAR = PERIOD_YEAR
	  from PERIOD_FINANCIALS
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_ID = 104
	   and PERIOD_END_DATE = @PERIOD_END_DATE
	   and PERIOD_TYPE = 'A'
--print 'PERIOD_YEAR = ' + cast(@PERIOD_YEAR as varchar(32))
	-- Get the data
	
	select distinct ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE
		  , (AMOUNT * @PERCENTAGE) as AMOUNT, AMOUNT as AMOUNT100
	  into #A
	  from dbo.PERIOD_FINANCIALS pf  
	 where DATA_ID = 290		-- Earnings
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and pf.PERIOD_END_DATE = @PERIOD_END_DATE
	
	select distinct ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE
		  , (AMOUNT * (1-@PERCENTAGE)) as AMOUNT
	  into #B
	  from dbo.PERIOD_FINANCIALS pf  
	 where DATA_ID = 290		-- Earnings
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and pf.PERIOD_YEAR = @PERIOD_YEAR + 1
	
 ---- Total amount for all the fiscal quarters within an year --- 

	select distinct ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE
		  , (AMOUNT * @PERCENTAGE) as AMOUNT, AMOUNT as AMOUNT100
	  into #C
	  from dbo.PERIOD_FINANCIALS pf  
	 where DATA_ID = 104		-- Equity
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and pf.PERIOD_END_DATE = @PERIOD_END_DATE
	
	select distinct ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE
		  , (AMOUNT * (1-@PERCENTAGE)) as AMOUNT
	  into #D
	  from dbo.PERIOD_FINANCIALS pf  
	 where DATA_ID = 104		-- Equity
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and pf.PERIOD_YEAR = @PERIOD_YEAR + 1
		

	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  200 as DATA_ID										-- DATA_ID:200 DATA_ID:200 Forward ROE
		,  case when b.AMOUNT IS NOT NULL and d.AMOUNT IS NOT NULL then ((a.AMOUNT + b.AMOUNT) / (c.AMOUNT + d.AMOUNT))
				when b.AMOUNT IS     NULL and d.AMOUNT IS NOT NULL then ((a.AMOUNT100) / (c.AMOUNT + d.AMOUNT))
				when b.AMOUNT IS NOT NULL and d.AMOUNT IS     NULL then ((a.AMOUNT + b.AMOUNT) / (c.AMOUNT100))
																   else ((a.AMOUNT100) / (c.AMOUNT100)) end	as AMOUNT						-- Sum of next 4 quarters NINC**/Next Annual QTLE*

		,  case when b.AMOUNT IS NOT NULL and d.AMOUNT IS NOT NULL then 'Earning(' + cast(a.AMOUNT as varchar(20)) + ' + ' + cast(b.AMOUNT as varchar(20)) + ') / Equity(' + cast(c.AMOUNT as varchar(20)) + ' + '  + cast(d.AMOUNT as varchar(20)) + ')'
				when b.AMOUNT IS     NULL and d.AMOUNT IS NOT NULL then 'Earning(' + cast(a.AMOUNT100 as varchar(20)) + ') / Equity(' + cast(c.AMOUNT as varchar(20)) + ' + '  + cast(d.AMOUNT as varchar(20)) + ')'
				when b.AMOUNT IS NOT NULL and d.AMOUNT IS     NULL then 'Earning(' + cast(a.AMOUNT as varchar(20)) + ' + ' + cast(b.AMOUNT as varchar(20)) + ') / Equity(' + cast(c.AMOUNT100 as varchar(20)) + ')'
																   else 'Earning(' + cast(a.AMOUNT100 as varchar(20)) + ') / Equity(' + cast(c.AMOUNT100 as varchar(20)) + ')' end as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE
					and b.CURRENCY = a.CURRENCY
	 inner join	#C c on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE
					and c.CURRENCY = a.CURRENCY
	  left join	#D d on d.ISSUER_ID = a.ISSUER_ID 
					and d.DATA_SOURCE = a.DATA_SOURCE
					and d.CURRENCY = a.CURRENCY
	 where 1=1 	  
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 200 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 200 Forward ROE. DATA_ID:104 QTLE is NULL or ZERO'
			  from #A a
			where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 200 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 200 Forward ROE.  DATA_ID:104 QTLE is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	

			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 200 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 200 Forward ROE .  DATA_ID:104 QTLE no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 200 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 200 Forward ROE.  DATA_ID:44 NINC  no data or missing quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
	drop table #D
	


