------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:201 Forward Revenue Growth 
--
--(Sum of next 4 quarters RTLR**/ Sum of previous 4 quarters RTLR**) -1
--
-- Author:  Prerna
-- Date:	July 17, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_201', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_201;
GO

CREATE procedure [dbo].[AIMS_Calc_201](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data for the next 4 quarters for a year
	select sum(f.amount)as AMOUNT
		,  f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY , DATEPART(YYYY, GETDATE()) as Current_Year
	  into #A
	  from (select * 
			  from dbo.PERIOD_FINANCIALS pf
			 where pf.DATA_ID = 11 -- RTLR
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE > GETDATE()						-- next quarter from today
			   and pf.PERIOD_END_DATE < DATEADD( month, 12, getdate())	-- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
			) f
	 group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having COUNT(Distinct PERIOD_TYPE) = 4
	

 ---- get the Total amount for the previous 4 fiscal quarters for a year  --- 

	select sum(f.amount)as AMOUNT
		,  f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY ,DATEPART(YYYY, GETDATE()) as Current_Year
	  into #B
	  from (select * 
			  from dbo.PERIOD_FINANCIALS pf
			 where pf.DATA_ID = 11 -- RTLR
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE < GETDATE()						-- previous quarter from today
			   and pf.PERIOD_END_DATE > DATEADD( month, -12, getdate())	-- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
			) f
	 group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having COUNT(Distinct PERIOD_TYPE) = 4
	
				

	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	
	insert into dbo.PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
		select a.ISSUER_ID, '', a.COA_TYPE, a.DATA_SOURCE,  a.DATA_SOURCE
		,  '', 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  201 as DATA_ID										-- DATA_ID:201 Forward Revenue Growth 
		,  a.AMOUNT /b.AMOUNT as AMOUNT						-- (Sum of next 4 quarters RTLR**/ Sum of previous 4 quarters RTLR**) -1
		,  'RTLR Sum of next quarter(' + CAST(a.AMOUNT as varchar(32)) + ') / RTLR sum of 4 previous quarters(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL'
		from #A a
		inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.Current_Year = a.Current_Year and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY		
		where 1=1 	  
	    and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--		order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 201 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '1/1/1900' as PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 201 Forward Revenue Growth . DATA_ID:11 is NULL or ZERO'
			  from #B a
			where --a.DATA_ID = 11 -- RTLR
			(@ISSUER_ID is null or @ISSUER_ID = a.ISSUER_ID)
			and isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
				 
			) union (	
			-- ERROR - No data at all available or one of the next quarter is missing
			select GETDATE() as LOG_DATE, 201 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 201 Forward Revenue Growth .  DATA_ID:11 no data or one of the next quarter is missing' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	
		-- ERROR - No data at all available or one of the previous quarter is missing
			select GETDATE() as LOG_DATE, 201 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 201 Forward Revenue Growth.  DATA_ID:11 no data or one of the previous quarter is missing' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	



