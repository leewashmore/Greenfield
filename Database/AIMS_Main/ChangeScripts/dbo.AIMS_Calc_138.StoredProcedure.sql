SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:138 Non-Interest Income as % Revenue
--
--			SNII/RTLR
--
-- Author:	David Muench
-- Date:	July 2, 2012
------------------------------------------------------------------------
alter procedure [dbo].[AIMS_Calc_138](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
,	@stage				char		= 'N'
)
as

		
	  select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	  select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0

	-- Get the data
	if @stage = 'Y'
	begin
		insert into  #A
		select pf.* 
		from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
		where DATA_ID = 33					-- SNII 
		and pf.ISSUER_ID = @ISSUER_ID
		and pf.PERIOD_TYPE = 'A'			-- Only Annual

		insert into #B
		select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	 where DATA_ID = 17					-- Net Interest Income (ENII)
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'			-- Only Annual
	
	end
	else
	begin
	
	insert into  #A
	select pf.* 
	  
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	 where DATA_ID = 33					-- SNII 
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'			-- Only Annual

	insert into  #B
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	 where DATA_ID = 17					-- Net Interest Income (ENII)
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'			-- Only Annual

	end

	-- Add the data to the table
	BEGIN TRAN T1
	if @stage = 'Y'
	begin
			insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
			select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
			,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
			,  a.FISCAL_TYPE, a.CURRENCY
			,  138 as DATA_ID										-- DATA_ID:138 Non-Interest Income as % Revenue
			,  CASE WHEN  (a.AMOUNT+ b.AMOUNT)> 0  THEN a.AMOUNT /(a.AMOUNT+ b.AMOUNT)
				ELSE NULL 
				END as AMOUNT
						-- SNII/(SNII+ENII)
			,  'SNII(' + CAST(a.AMOUNT as varchar(32)) + ') / RTLR(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
			,  a.SOURCE_CURRENCY
			,  a.AMOUNT_TYPE
			from #A a
			inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
			where 1=1 
			and a.amount is not null and (isnull(b.AMOUNT,0.0)+isnull(a.AMOUNT, 0.0)) > 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	end
	else
	begin
	insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE) -- Splitting into 2 tables Change to insert the data into period_Financials_issuer
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  138 as DATA_ID										-- DATA_ID:138 Non-Interest Income as % Revenue
		,  CASE WHEN  (a.AMOUNT+ b.AMOUNT)> 0  THEN a.AMOUNT /(a.AMOUNT+ b.AMOUNT)
				ELSE NULL 
				END as AMOUNT
						-- SNII/(SNII+ENII)
		,  'SNII(' + CAST(a.AMOUNT as varchar(32)) + ') / RTLR(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and a.amount is not null and (isnull(b.AMOUNT,0.0)+isnull(a.AMOUNT, 0.0)) > 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	 
	end
	COMMIT TRAN T1
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 138 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 138:Non-Interest Income as % Revenue.  DATA_ID:11 RTLR is NULL or ZERO'
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 138 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 138:Non-Interest Income as % Revenue.  DATA_ID:11 RTLR is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 138 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 138:Non-Interest Income as % Revenue.  DATA_ID:33 SNII is missing'
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 138 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 138:Non-Interest Income as % Revenue.  DATA_ID:33 SNII no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 138 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 138:Non-Interest Income as % Revenue.  DATA_ID:11 RTLR no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B

GO


