SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[AIMS_Calc_230](
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
	insert 	  into #A
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	 where DATA_ID = 290					-- Earnings
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	insert 	  into #B
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	 where DATA_ID = 124					 -- FCDP
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	
	insert 	  into #C
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	 where DATA_ID = 104					 -- QTLE
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	END
	ELSE
	BEGIn
	
	
	insert   into #A
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	 where DATA_ID = 290					-- Earnings
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	insert   into #B
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	 where DATA_ID = 124					 -- FCDP
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   
	insert   into #C   
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	 where DATA_ID = 104					 -- QTLE
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	
	END
	-- Add the data to the table
	BEGIN TRAN T1
	IF @STAGE = 'Y'
	BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  230 as DATA_ID										-- DATA_ID:230 Retained Earnings / Equity
		,  (a.AMOUNT + b.AMOUNT)/c.Amount as AMOUNT						-- (Earnings + FCDP) / QTLE
		,  '(Earnings(' + CAST(a.AMOUNT as varchar(32)) + ') + FCDP(' + CAST(b.AMOUNT as varchar(32)) + ')) / QTLE(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
     inner join	#C c on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY					
	 where 1=1 
	   and a.PERIOD_TYPE = 'A'
	   and isnull(c.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
END
ELSE
BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  230 as DATA_ID										-- DATA_ID:230 Retained Earnings / Equity
		,  (a.AMOUNT + b.AMOUNT)/c.Amount as AMOUNT						-- (Earnings + FCDP) / QTLE
		,  '(Earnings(' + CAST(a.AMOUNT as varchar(32)) + ') + FCDP(' + CAST(b.AMOUNT as varchar(32)) + ')) / QTLE(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
     inner join	#C c on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY					
	 where 1=1 
	   and a.PERIOD_TYPE = 'A'
	   and isnull(c.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
END
	COMMIT TRAN T1
	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 230 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 230 Retained Earnings / Equity.  DATA_ID:104 QTLE is NULL or ZERO'
			  from #C a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 230 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 230 Retained Earnings / Equity.  DATA_ID:290 Earnings is missing'
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 230 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 230 Retained Earnings / Equity.  DATA_ID:124 FCDP is missing' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			
			select GETDATE() as LOG_DATE, 230 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 230 Retained Earnings / Equity.  DATA_ID:104 QTLE is missing' as TXT
			  from #C a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 230 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 230 Retained Earnings / Equity.  DATA_ID:290 Earnings no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 230 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 230 Retained Earnings / Equity.  DATA_ID:124 FCDP no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 230 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 230 Retained Earnings / Equity.  DATA_ID:104 QTLE no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
GO
