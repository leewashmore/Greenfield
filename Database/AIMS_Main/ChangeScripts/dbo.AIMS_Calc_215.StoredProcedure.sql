SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[AIMS_Calc_215](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@STAGE				char		='N'
)
as
	  insert into #A
	  select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	-- Get the data
	
	if @stage = 'Y'
	Begin
		insert  into #A
		select pf.* 
		  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		 where DATA_ID = 149			-- Net Debt/Equity
		   and pf.ISSUER_ID = @ISSUER_ID
		   and period_end_date = (select max(period_end_date) from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf  -- to find closest end_date to getdate
								   where DATA_ID = 149			
									 and pf.ISSUER_ID = @ISSUER_ID
									 and period_end_date < getdate() )
		 and period_type = 'A'
	end
	else
	begin
		insert into #A
		select pf.* 
	    
	    from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	    where DATA_ID = 149			-- Net Debt/Equity
	    and pf.ISSUER_ID = @ISSUER_ID
	    and period_end_date = (select max(period_end_date) from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf  -- to find closest end_date to getdate
							   where DATA_ID = 149			
							     and pf.ISSUER_ID = @ISSUER_ID
						         and period_end_date < getdate() )
		and period_type = 'A'
	End
 
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	BEGIN TRAN T1
	IF @STAGE = 'Y'
	BEGIN
		insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
			  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
			  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
		select a.ISSUER_ID, a.SECURITY_ID, ' ', a.DATA_SOURCE, a.ROOT_SOURCE
			,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
			,  a.FISCAL_TYPE, a.CURRENCY
			,  215 as DATA_ID										-- 215 Trailing Net Debt/Equity 
			,  a.AMOUNT as AMOUNT						-- Previous Annual Net Debt/Equity (149)*
			,  'Previous Annual Net Debt/Equity(' + CAST(a.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
			,  a.SOURCE_CURRENCY
			,  a.AMOUNT_TYPE
		  from #A a	
		 where 1=1 	  
		   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
	--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	END
	ELSE
	BEGIN
		insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, ' ', a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  215 as DATA_ID										-- 215 Trailing Net Debt/Equity 
		,  a.AMOUNT as AMOUNT						-- Previous Annual Net Debt/Equity (149)*
		,  'Previous Annual Net Debt/Equity(' + CAST(a.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a	
	 where 1=1 	  
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	END
	COMMIT TRAN T1

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 215 as DATA_ID, a.ISSUER_ID, 'C'
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 215 Trailing Net Debt/Equity. DATA_ID:149 is NULL or ZERO'
			  from #A a
			where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 215 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 215 Trailing Net Debt/Equity.  DATA_ID:149 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A
GO
