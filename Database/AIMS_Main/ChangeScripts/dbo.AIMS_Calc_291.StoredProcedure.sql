SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[AIMS_Calc_291](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@STAGE				char		= 'N'
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
	   
	   select pf.* 
	  into #C
	   from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	   where 1=0
	-- Get the data
	IF @STAGE = 'Y'
	BEGIN
		insert into #A
		select pf.* 
		  
		   from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		 where DATA_ID = 112		-- SDED
		   and pf.ISSUER_ID = @ISSUER_ID
		 order by PERIOD_YEAR

		insert into #B
		select pf.* 
		  
		   from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		 where DATA_ID = 113		-- SAMT
		   and pf.ISSUER_ID = @ISSUER_ID
		 order by PERIOD_YEAR

		insert into #C
		select pf.* 
		  
		   from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		 where DATA_ID = 24			-- SDPR
		   and pf.ISSUER_ID = @ISSUER_ID
		 order by PERIOD_YEAR
	END
	ELSE
	BEGIN
		insert into #A
		select pf.* 
		  
		   from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
		 where DATA_ID = 112		-- SDED
		   and pf.ISSUER_ID = @ISSUER_ID
		 order by PERIOD_YEAR

		insert into #B
		select pf.* 
		  
		   from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
		 where DATA_ID = 113		-- SAMT
		   and pf.ISSUER_ID = @ISSUER_ID
		 order by PERIOD_YEAR

		insert  into #C
		select pf.* 
		 
		   from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
		 where DATA_ID = 24			-- SDPR
		   and pf.ISSUER_ID = @ISSUER_ID
		 order by PERIOD_YEAR	
	END
	-- Add the data to the table  where cash flow items exist (SDED and SAMT)
	BEGIN TRAN T1
	IF @STAGE = 'Y'
	BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
		 select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  291 as DATA_ID							-- DATA_ID:291 Depreciation 
		, isnull(a.AMOUNT,0.0) + isnull(b.AMOUNT, 0.0) as AMOUNT
		, 'SDED('+ cast(a.AMOUNT as varchar(32)) + ') + SAMT('+ CAST( isnull(b.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
	END
	ELSE
	BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
		 select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  291 as DATA_ID							-- DATA_ID:291 Depreciation 
		, isnull(a.AMOUNT,0.0) + isnull(b.AMOUNT, 0.0) as AMOUNT
		, 'SDED('+ cast(a.AMOUNT as varchar(32)) + ') + SAMT('+ CAST( isnull(b.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation	
	END
	COMMIT TRAN T1

	-- Add the data to the table  where cash flow items do NOT exist (use Income Statement line instead: SDPR)
	BEGIN TRAN T2
	IF @STAGE = 'Y'
	BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	 select c.ISSUER_ID, c.SECURITY_ID, c.COA_TYPE, c.DATA_SOURCE, c.ROOT_SOURCE
		,  c.ROOT_SOURCE_DATE, c.PERIOD_TYPE, c.PERIOD_YEAR, c.PERIOD_END_DATE
		,  c.FISCAL_TYPE, c.CURRENCY
		,  291 as DATA_ID							-- DATA_ID:291 Depreciation 
		,  c.amount
		,  'SDPR('+ cast(isnull(c.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  c.SOURCE_CURRENCY
		,  c.AMOUNT_TYPE
	  from #C c
	  left join	#A a on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY
	 where 1=1 
	   and a.AMOUNT is null	-- Data validation
	   END
	   ELSE
	   BEGIN
	   	insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	 select c.ISSUER_ID, c.SECURITY_ID, c.COA_TYPE, c.DATA_SOURCE, c.ROOT_SOURCE
		,  c.ROOT_SOURCE_DATE, c.PERIOD_TYPE, c.PERIOD_YEAR, c.PERIOD_END_DATE
		,  c.FISCAL_TYPE, c.CURRENCY
		,  291 as DATA_ID							-- DATA_ID:291 Depreciation 
		,  c.amount
		,  'SDPR('+ cast(isnull(c.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  c.SOURCE_CURRENCY
		,  c.AMOUNT_TYPE
	  from #C c
	  left join	#A a on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY
	 where 1=1 
	   and a.AMOUNT is null	-- Data validation
	   END
	COMMIT TRAN T2


	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 291 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 291 Depreciation/Amortization. DATA_ID:112 SDED no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z

			) union (
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 291 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 291 Depreciation/Amortization. DATA_ID:113 SAMT no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z

			) union (
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 291 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 291 Depreciation/Amortization. DATA_ID:24 SDPR no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A
GO
