SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:147 Net Debt/EBITDA
--
--			   190 /130 
--
-- Author:	David Muench
-- Date:	July 12, 2012
------------------------------------------------------------------------
alter procedure [dbo].[AIMS_Calc_147](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@STAGE				char		= 'N'
)
as

	-- Get the data
	  select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	  select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	
	IF @STAGE = 'Y'
	BEGIN
	
	  INSERT into #A
	  select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	 where DATA_ID = 190				-- Net Debt
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'			-- Only Annual
		
	INSERT into #B
      select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	 where DATA_ID = 130					-- EBITDA
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'			-- Only Annual
	
	END
	ELSE
	BEGIN
	
	INSERT into #A
      select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	  where DATA_ID = 190				-- Net Debt
	  and pf.ISSUER_ID = @ISSUER_ID
	  and pf.PERIOD_TYPE = 'A'			-- Only Annual

	 INSERT into #B
	 select pf.* 
	 from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	 where DATA_ID = 130					-- EBITDA
	 and pf.ISSUER_ID = @ISSUER_ID
	 and pf.PERIOD_TYPE = 'A'			-- Only Annual
	 
	 
	END
	-- Add the data to the table
	BEGIN TRAN T1
	IF @STAGE= 'Y'
	BEGIN
		
	insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  147 as DATA_ID										-- 
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- Net Debt/EBITDA
		,  'Net Debt(' + CAST(a.AMOUNT as varchar(32)) + ') / EBITDA(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and b.AMOUNT is not null
	   and b.AMOUNT <> 0.0
	END
	ELSE
	BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  147 as DATA_ID										-- 
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- Net Debt/EBITDA
		,  'Net Debt(' + CAST(a.AMOUNT as varchar(32)) + ') / EBITDA(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and b.AMOUNT is not null
	   and b.AMOUNT <> 0.0
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
END
	COMMIT TRAN T1
	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 138 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 147:Net Income Margin.  DATA_ID:130 EBITDA is NULL or ZERO' as TXT
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 147 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 147:Net Income Margin.  DATA_ID:130 EBITDA is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 147 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 147:Net Debt/EBITDA.  DATA_ID:190 Net Debt is missing'
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 147 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 147:Net Debt/EBITDA.  DATA_ID:190 Net Debt no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 147 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 147:Net Debt/EBITDA.  DATA_ID:130 EBITDA no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 147 as DATA_ID, @ISSUER_ID, PERIOD_TYPE
				,  PERIOD_YEAR,  PERIOD_END_DATE,  FISCAL_TYPE,  CURRENCY
				, 'ERROR calculating 147:Net Debt/EBITDA.  DATA_ID:130 EBITDA is null or zero' as TXT
			  from #B
			 where AMOUNT is NULL
				or AMOUNT = 0.0
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
GO
