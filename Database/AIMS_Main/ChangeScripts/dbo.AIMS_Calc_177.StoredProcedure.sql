SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[AIMS_Calc_177](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@STAGE				char		= 'N'
)
as
	
	select pf.* 
	  into #A
	   from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	   where 1=0;
	   
	   if @stage = 'Y'
	   Begin
			insert into #A
	   		select pf.* 
		   from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		 where DATA_ID = 290			-- Earnings
		   and pf.ISSUER_ID = @ISSUER_ID
		 order by PERIOD_YEAR
	   end
	   else
	   begin
			insert  into #A
			select pf.* 
		   from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
		 where DATA_ID = 290			-- Earnings
		   and pf.ISSUER_ID = @ISSUER_ID
		 order by PERIOD_YEAR
	 end


	-- Add the data to the table
	BEGIN TRAN T1
	IF @STAGE = 'Y'
	BEGIN
			insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
			select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
			,  b.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
			,  a.FISCAL_TYPE, a.CURRENCY
			,  177 as DATA_ID										-- DATA_ID:177 Net Income Growth (YOY) 
			,    CASE WHEN a.AMOUNT >= 0 and b.AMOUNT > 0 THEN (a.AMOUNT /b.AMOUNT)-1 
				ELSE NULL 
				END as AMOUNT 					-- (NINC for Year/ NINC for Prior Year)-1
			,  '(Earnings for ('+ CAST(a.PERIOD_YEAR as varchar(32)) + '(' + CAST(a.AMOUNT as varchar(32)) + ') / Earnings for ('+ CAST(b.PERIOD_YEAR as varchar(32)) +')(' + CAST(b.AMOUNT as varchar(32)) + ')) - 1 ' as CALCULATION_DIAGRAM
			,  a.SOURCE_CURRENCY
			,  a.AMOUNT_TYPE
		  from #A a
		  inner join	#A b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR+1 = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
		where 1=1 and
		isnull(a.AMOUNT, 0.0)>=0  and isnull(b.AMOUNT, 0.0) > 0.0 -- Data validation
		--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	END
	ELSE
	BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  b.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  177 as DATA_ID										-- DATA_ID:177 Net Income Growth (YOY) 
		,    CASE WHEN a.AMOUNT >= 0 and b.AMOUNT > 0 THEN (a.AMOUNT /b.AMOUNT)-1 
				ELSE NULL 
				END as AMOUNT 					-- (NINC for Year/ NINC for Prior Year)-1
		,  '(Earnings for ('+ CAST(a.PERIOD_YEAR as varchar(32)) + '(' + CAST(a.AMOUNT as varchar(32)) + ') / Earnings for ('+ CAST(b.PERIOD_YEAR as varchar(32)) +')(' + CAST(b.AMOUNT as varchar(32)) + ')) - 1 ' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#A b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR+1 = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 and
	  isnull(a.AMOUNT, 0.0)>=0  and isnull(b.AMOUNT, 0.0) > 0.0 -- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
END
	COMMIT TRAN T1
	
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
GO
