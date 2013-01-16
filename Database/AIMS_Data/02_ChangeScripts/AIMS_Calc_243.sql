------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:243 Forward Net Income Growth, 2 Year (Annualized)
--
--(((Sum of NINC for QP1/ Sum of NINC QP0 x Sum of NINC QP2/ Sum of NINC QP1)^1/2)-1)*100
--
-- Author:	Prerna 
-- Date:	July 17, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_243', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_243;
GO

CREATE procedure [dbo].[AIMS_Calc_243](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data for the next 4 quarters for a year  QP1
	select sum(f.amount)as AMOUNT
		,  f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY , DATEPART(YYYY, GETDATE()) as Current_Year
	  into #A
	  from (select * 
			  from dbo.PERIOD_FINANCIALS pf
			 where pf.DATA_ID = 44 -- NINC
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE > GETDATE()						-- next quarter from today
			   and pf.PERIOD_END_DATE < DATEADD( month, 12, getdate())	-- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
			) f
	 group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
		having COUNT(Distinct PERIOD_TYPE) = 4
	

 ---- get the Total amount for the previous 4 fiscal quarters for a year  QP0 --- 

	select sum(f.amount)as AMOUNT
		,  f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY , DATEPART(YYYY, GETDATE()) as Current_Year
	  into #B
	  from (select * 
			  from dbo.PERIOD_FINANCIALS pf
			 where pf.DATA_ID = 44 -- NINC
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE < GETDATE()						-- previous quarter from today
			   and pf.PERIOD_END_DATE > DATEADD( month, -12, getdate())	-- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
			) f
	 group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
		having COUNT(Distinct PERIOD_TYPE) = 4
		
-- Get the data for the next 4 quarters for a year  QP2
	select sum(f.amount)as AMOUNT
		,  f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY , DATEPART(YYYY, GETDATE()) as Current_Year
	  into #C
	  from (select * 
			  from dbo.PERIOD_FINANCIALS pf
			 where pf.DATA_ID = 44 -- NINC
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE > DATEADD( month, 12, getdate())	-- next to next quarter from today
			   and pf.PERIOD_END_DATE < DATEADD( month, 24, getdate())	-- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
			) f
	 group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
		having COUNT(Distinct PERIOD_TYPE) = 4
				

	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	
	insert into dbo.PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		,  ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		,  DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, '', a.COA_TYPE, a.DATA_SOURCE,  a.DATA_SOURCE
		,  '', 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  243 as DATA_ID										-- DATA_ID:243 Forward Net Income Growth, 2 Year (Annualized)
		,  ((sqrt((a.Amount/ b.Amount) * (c.Amount/ a.Amount)))-1)*100 as AMOUNT	-- (((Sum of NINC for QP1/ Sum of NINC QP0 x Sum of NINC QP2/ Sum of NINC QP1)^1/2)-1)*100
		,  '(((NINC Sum of next quarter(' + CAST(a.AMOUNT as varchar(32)) + ') / NINC sum of 4 previous quarters(' + CAST(b.AMOUNT as varchar(32)) + ') x NINC Sum of next to next quarter(' + CAST(c.AMOUNT as varchar(32)) + ') / NINC sum of next 4 quarters(' + CAST(a.AMOUNT as varchar(32)) + '))^1/2)-1)*100' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL'
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.Current_Year = a.Current_Year and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 inner join #C c on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
					and c.Current_Year = a.Current_Year and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY
	 where 1=1 	  
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	   and isnull(a.Amount, 0.0) <> 0.0  --Data validation
	   and (b.AMOUNT <> 0.0 and a.AMOUNT <> 0.0 and ((a.Amount/ b.Amount) * (c.Amount/ a.Amount)) >= 0) --Data validation
--	  order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 243 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '1/1/1900' as PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 243 Forward Net Income Growth, 2 Year (Annualized) . DATA_ID:44 is NULL or ZERO'
			  from #B a
			where --a.DATA_ID = 44 -- NINC
			 isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
				 
			) union (	
			
			select GETDATE() as LOG_DATE, 243 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '1/1/1900' as PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 243 Forward Net Income Growth, 2 Year (Annualized) . Calculated amount is negative'
			  from #A a
				inner join	#B b on b.ISSUER_ID = a.ISSUER_ID
							and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.Current_Year = a.Current_Year and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
				inner join #C c on c.ISSUER_ID = a.ISSUER_ID 
							and c.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
							and c.Current_Year = a.Current_Year and c.FISCAL_TYPE = a.FISCAL_TYPE
							and c.CURRENCY = a.CURRENCY
				
			where (a.Amount/ b.Amount * c.Amount/ a.Amount) < 0 --Data validation
				 
			) union (	
			select GETDATE() as LOG_DATE, 243 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '1/1/1900' as PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 243 Forward Net Income Growth, 2 Year (Annualized) . DATA_ID:44 is NULL or ZERO'
			  from #A a
			where --a.DATA_ID = 44 -- NINC
			 isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
				 
			) union (	
			-- ERROR - No data at all available or one of the next quarter is missing
			select GETDATE() as LOG_DATE, 243 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 243 Forward Net Income Growth, 2 Year (Annualized)  .  DATA_ID:44 no data or one of the next quarter is missing' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	
		-- ERROR - No data at all available or one of the previous quarter is missing
			select GETDATE() as LOG_DATE, 243 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 243 Forward Net Income Growth, 2 Year (Annualized) .  DATA_ID:44 no data or one of the previous quarter is missing' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			  ) union (	
		-- ERROR - No data at all available or one of the previous quarter is missing
			select GETDATE() as LOG_DATE, 243 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 243 Forward Net Income Growth, 2 Year (Annualized) .  DATA_ID:44 no data or one of the previous quarter is missing' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
		



