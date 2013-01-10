------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:203 Forward Net Debt/Equity  
--
--			:   Forward Net Debt/Equity 
--
-- Author:	Shivani
-- Date:	July 18, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_203', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_203;
GO

CREATE procedure [dbo].[AIMS_Calc_203](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 inner join (select ISSUER_ID, min(period_end_date) as period_end_date
				   from dbo.PERIOD_FINANCIALS		-- to find closest end_date to getdate
				  where DATA_ID = 149		
				    and ISSUER_ID = @ISSUER_ID
				    and period_end_date > getdate()
				    and period_type = 'A'
				  group by ISSUER_ID
				) z on z.ISSUER_ID = pf.ISSUER_ID and z.period_end_date = pf.PERIOD_END_DATE
	 where DATA_ID = 149			-- Net Debt/Equity
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.period_type = 'A'
	
 
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  203 as DATA_ID										-- 203 Forward Net Debt/Equity  
		,  a.AMOUNT as AMOUNT						-- Forward Net Debt/Equity 
		,  'Fwd Annual Net Debt/Equity(' + CAST(a.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
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
			select GETDATE() as LOG_DATE, 203 as DATA_ID, a.ISSUER_ID, 'C'
				,   0, '01/01/1900'	, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 203 Forward Net Debt/Equity . DATA_ID:149 is NULL or ZERO'
			  from #A a
			where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 203 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 203 Forward Net Debt/Equity .  DATA_ID:149 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
	-- Clean up
	drop table #A	
	



