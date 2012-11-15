------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:290 Earnings 
--
--			If including Preferred Shares, then NINC, otherwise CIAC.
--
-- Author:	David Muench
-- Date:	October 16, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_290', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_290;
GO

create procedure [dbo].[AIMS_Calc_290](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as
	declare @DATA_ID integer
	select @DATA_ID = case when PREFERRED = 'Y' then 44 else 47 end	-- NINC or CIAC
	  from ISSUER_SHARES_COMPOSITION
	 where ISSUER_ID = @ISSUER_ID
	

	-- Get the data
	select pf.* 
	  into #A
	   from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = @DATA_ID			-- Earnings
	   and pf.ISSUER_ID = @ISSUER_ID
	 order by PERIOD_YEAR


	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  290 as DATA_ID							-- DATA_ID:290 Earnings 
		,  a.AMOUNT as AMOUNT						-- NINC or CIAC
		,  'Earnings('+ CAST(a.PERIOD_YEAR as varchar(32)) + ') ' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#A b on b.ISSUER_ID = a.ISSUER_ID and b.AMOUNT_TYPE = a.AMOUNT_TYPE
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR+1 = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  DATA_ID:290 Earnings is NULL or ZERO'
			  from #A a
			 where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error

	 		) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  DATA_ID:290 Earnings is missing for the prior year' as TXT
			  from #A a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID and b.AMOUNT_TYPE = a.AMOUNT_TYPE
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR+1 = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL

  			) union (	

			-- Error conditions - missing data for prior period
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  DATA_ID:290 Earnings is missing for the year' as TXT
			  from #A a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID and b.AMOUNT_TYPE = a.AMOUNT_TYPE
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR+1 and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL

  			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 177 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  DATA_ID:290 Earnings no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A
	



