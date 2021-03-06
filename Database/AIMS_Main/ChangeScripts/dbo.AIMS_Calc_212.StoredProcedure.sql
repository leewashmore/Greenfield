SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[AIMS_Calc_212](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@STAGE				char		= 'N'
)
as
	
	--Trailing ROE
	  select pf.*          
	  into #A 
      from dbo.PERIOD_FINANCIALS_ISSUER_STAGE  pf with (nolock)
	  where 1=0
	
	  select pf.*          
	  into #B
      from dbo.PERIOD_FINANCIALS_ISSUER_STAGE  pf with (nolock)
	  where 1=0
	
	
	IF @STAGE = 'Y'
	Begin
	--Trailing Earnings
  	  insert into #A 
	  select pf.*          
      from dbo.PERIOD_FINANCIALS_ISSUER_STAGE  pf with (nolock)
	  where pf.DATA_ID = 296   -- Earnings
		and pf.ISSUER_ID = @ISSUER_ID
		and pf.PERIOD_TYPE = 'C'
	
	
	--Trailing Equity 
	  insert into #B
	  select pf.*          
      from dbo.PERIOD_FINANCIALS_ISSUER_STAGE  pf with (nolock)
	  where pf.DATA_ID = 298   -- Equity
		and pf.ISSUER_ID = @ISSUER_ID
		and pf.PERIOD_TYPE = 'C'
	end
	else
	begin
			--Trailing Earnings
	  insert 	  into #A 
	  select pf.*          
      from dbo.PERIOD_FINANCIALS_ISSUER_MAIN  pf with (nolock)
	  where pf.DATA_ID = 296   -- Earnings
		and pf.ISSUER_ID = @ISSUER_ID
		and pf.PERIOD_TYPE = 'C'
	
	
	--Trailing Equity 
	insert into #B
	select pf.*          
      from dbo.PERIOD_FINANCIALS_ISSUER_MAIN  pf with (nolock)
	  where pf.DATA_ID = 298   -- Equity
		and pf.ISSUER_ID = @ISSUER_ID
		and pf.PERIOD_TYPE = 'C'
	end
	
	
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	BEGIN TRAN T1
	IF @stage='Y'
	Begin
		insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
			  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
			  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
		select  distinct b.ISSUER_ID, b.SECURITY_ID, b.COA_TYPE, b.DATA_SOURCE, b.ROOT_SOURCE
			,  b.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
			,  b.FISCAL_TYPE, b.CURRENCY
			,  212 as DATA_ID										-- DATA_ID:212 DATA_ID:212 Trailing ROE
			,  a.AMOUNT /b.AMOUNT as AMOUNT						-- Trailing Earnings**/Previous QTLE*
			,  'Trailing Earnings(' + CAST(a.AMOUNT as varchar(32)) + ') / Previous QTLE(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
			,  b.SOURCE_CURRENCY
			,  b.AMOUNT_TYPE
		  from #A a
		 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
						and b.DATA_SOURCE = a.DATA_SOURCE 
						and b.CURRENCY = a.CURRENCY
		 where 1=1 	  
		   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
		-- order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	end
	else
	begin
		insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
			  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
			  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
		select  distinct b.ISSUER_ID, b.SECURITY_ID, b.COA_TYPE, b.DATA_SOURCE, b.ROOT_SOURCE
			,  b.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
			,  b.FISCAL_TYPE, b.CURRENCY
			,  212 as DATA_ID										-- DATA_ID:212 DATA_ID:212 Trailing ROE
			,  a.AMOUNT /b.AMOUNT as AMOUNT						-- Trailing Earnings**/Previous QTLE*
			,  'Trailing Earnings(' + CAST(a.AMOUNT as varchar(32)) + ') / Previous QTLE(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
			,  b.SOURCE_CURRENCY
			,  b.AMOUNT_TYPE
		  from #A a
		 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
						and b.DATA_SOURCE = a.DATA_SOURCE 
						and b.CURRENCY = a.CURRENCY
		 where 1=1 	  
		   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
		-- order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	
	End
COMMIT TRAN T1

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 212 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 212 Trailing ROE. DATA_ID:290 Earnings is NULL or ZERO'
			  from #A a
			where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 212 as DATA_ID, a.ISSUER_ID, 'C'
				, 0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 212 Trailing ROE.  DATA_ID:290 Earnings is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE 
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	

			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 212 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 212 Trailing ROE .  DATA_ID:290 Earnings no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 212 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 212 Trailing ROE.  DATA_ID:104 QTLE  No data or missing quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
GO
