------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:222 Cash Earnings Growth (YOY) 
--
--			(OTLO for Period / OTLO for Period-1)-1
--
-- Author:	Prerna
-- Date:	July 17, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_222', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_222;
GO

create procedure [dbo].[AIMS_Calc_222](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 117			--OTLO
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	 order by PERIOD_YEAR

	select pf.PERIOD_YEAR + 1 as PRIOR_YEAR, pf.PERIOD_YEAR as PERIOD_YEAR,pf.CURRENCY,pf.COA_TYPE,pf.DATA_SOURCE,pf.FISCAL_TYPE,pf.PERIOD_TYPE,pf.ISSUER_ID,pf.AMOUNT,pf.PERIOD_END_DATE
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 117			--OTLO
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
		,  222 as DATA_ID										-- DATA_ID:222 Cash Earnings Growth (YOY)
		,  (a.AMOUNT /b.AMOUNT)-1 as AMOUNT						--  (OTLO for Period / OTLO for Period-1)-1
		,  '(OTLO for ('+ CAST(a.PERIOD_YEAR as varchar(32)) + '(' + CAST(a.AMOUNT as varchar(32)) + ') / OTLO for (('+ CAST(b.PERIOD_YEAR as varchar(32)) +')(' + CAST(b.AMOUNT as varchar(32)) + '))) - 1 ' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PRIOR_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and a.PERIOD_TYPE = 'A'
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 222 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 222  Cash Earnings Growth (YOY) .  DATA_ID:117 is NULL or ZERO for the period'
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 222 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 222  Cash Earnings Growth (YOY) .  DATA_ID:117 is missing for the period' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PRIOR_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	   
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 222 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 222  Cash Earnings Growth (YOY) .  DATA_ID:117 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A
	drop table #B
	



