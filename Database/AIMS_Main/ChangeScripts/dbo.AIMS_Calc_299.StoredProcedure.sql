IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Calc_299]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Calc_299]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Calc_299]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_Calc_299](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@STAGE				char		= 'N'
)
as

	  select  pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	  select   pf.* 
	   into #A
	  from dbo.PERIOD_FINANCIALS_SECURITY_STAGE pf with (nolock)
	  where 1=0
	--Trailing Dividend Yield

	if @stage = 'Y'
	BEGIN
		-- Get the data
		insert  into #B
		select  pf.* 
		  
		  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		 where DATA_ID = 297			--Trailing Dividends
		   and pf.ISSUER_ID = @ISSUER_ID
		   and period_type = 'C'
		
		insert   into #A
		select  distinct pf.* 
		 
		  from dbo.PERIOD_FINANCIALS_SECURITY_STAGE pf with (nolock)
		  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
		 where DATA_ID = 185			--Market Capitalization
		   and sb.ISSUER_ID = @ISSUER_ID
		   and period_type = 'C'
	END
	ELSE
	BEGIN
	insert  into #B
		select  pf.* 
		  
		  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
		 where DATA_ID = 297			--Trailing Dividends
		   and pf.ISSUER_ID = @ISSUER_ID
		   and period_type = 'C'
	insert    into #A
		select  distinct pf.* 
		
		  from dbo.PERIOD_FINANCIALS_SECURITY_MAIN pf with (nolock)
		  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
		 where DATA_ID = 185			--Market Capitalization
		   and sb.ISSUER_ID = @ISSUER_ID
		   and period_type = 'C'
	
	END
	
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	BEGIN TRAN T1
	IF @STAGE = 'Y'
	BEGIN
	insert into PERIOD_FINANCIALS_SECURITY_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  299 as DATA_ID										-- DATA_ID:299 Trailing Dividend Yield
		,  (-1*b.AMOUNT) /a.AMOUNT as AMOUNT						-- Trailing Dividends*/Current Market Cap** 
		,  'Trailing Dividends(' + CAST(b.AMOUNT as varchar(32)) + ') /Current Mkt Cap(' + CAST(a.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B b on b.ISSUER_ID = sb.ISSUER_ID
					and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.CURRENCY = a.CURRENCY
	 where 1=1 	  
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
	 --order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	 END
	 ELSE
	 BEGIN
	insert into PERIOD_FINANCIALS_SECURITY_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  299 as DATA_ID										-- DATA_ID:299 Trailing Dividend Yield
		,  (-1*b.AMOUNT) /a.AMOUNT as AMOUNT						-- Trailing Dividends*/Current Market Cap** 
		,  'Trailing Dividends(' + CAST(b.AMOUNT as varchar(32)) + ') /Current Mkt Cap(' + CAST(a.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B b on b.ISSUER_ID = sb.ISSUER_ID
					and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.CURRENCY = a.CURRENCY
	 where 1=1 	  
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
	 --order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY	 
	 END
		COMMIT TRAN T1

	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 299 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 299 Trailing Div Yield. DATA_ID:185 Market Cap is NULL or ZERO'
			   from #B a
			  where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error
			) union (	
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 299 as DATA_ID, a.ISSUER_ID, 'C'
				,0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 299 Trailing Div Yield .  DATA_ID:185 Market Cap is missing' as TXT
			  from #A a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE 
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 299 as DATA_ID, a.ISSUER_ID, 'C'
				,0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 299 Trailing Div Yield .  DATA_ID:296 Trailing Dividends is missing' as TXT
			  from #B a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = a.ISSUER_ID 
			  left join	#A b on b.SECURITY_ID = sb.SECURITY_ID
							and b.DATA_SOURCE = a.DATA_SOURCE 
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	

			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 299 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 299 Trailing Div Yield .  DATA_ID:296 Trailing Dividends no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 299 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 299 Trailing Div Yield.  DATA_ID:185 Market Cap No data or missing quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
GO
