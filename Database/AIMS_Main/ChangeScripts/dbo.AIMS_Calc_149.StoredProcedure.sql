SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:149 Net Debt/Equity
--
--			   190 /QTLE 
--
-- Author:	David Muench
-- Date:	July 12, 2012
------------------------------------------------------------------------
alter procedure [dbo].[AIMS_Calc_149](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)	
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS_ISSUER pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	 where DATA_ID = 190					-- Net Debt
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'				-- Only Annual

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS_ISSUER pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	 where DATA_ID = 104					-- QTLE
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'				-- Only Annual

	-- Add the data to the table
	BEGIN TRAN T1
	insert into PERIOD_FINANCIALS_ISSUER(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  149 as DATA_ID										-- 
		,  CASE WHEN b.AMOUNT > 0  THEN isnull(a.AMOUNT,0.0) / b.AMOUNT
				ELSE NULL 
				END as AMOUNT						-- Net Debt/Equity
		,  'Net Debt(' + CAST(a.AMOUNT as varchar(32)) + ') / QTLE(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID and b.AMOUNT_TYPE = a.AMOUNT_TYPE
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	 and ISNULL(b.AMOUNT, 0.0) > 0.0
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
COMMIT TRAN T1
	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 149 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 149:Net Income Margin.  DATA_ID:104 QTLE is NULL or ZERO' as TXT
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error

			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 149 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 149:Net Income Margin.  DATA_ID:104 QTLE is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID and b.AMOUNT_TYPE = a.AMOUNT_TYPE
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL

			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 149 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 149:Net Income Margin.  DATA_ID:190 Net Debt is missing'
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID and b.AMOUNT_TYPE = a.AMOUNT_TYPE
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL

			) union (

			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 149 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 149:Net Income Margin.  DATA_ID:190 Net Debt no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z

			) union (

			select GETDATE() as LOG_DATE, 149 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 149:Net Income Margin.  DATA_ID:104 QTLE no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B

GO


