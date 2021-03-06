SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[AIMS_Calc_227](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
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
	  into #c
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	  select pf.* 
	  into #D
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	  
	-- Get the data
	IF @STAGE = 'Y'
	BEGIN
		insert   into #A
		select pf.* 
	    from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	   where DATA_ID = 56					-- ATCA
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	insert   into #B
	select pf.* 
	 from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	 where DATA_ID = 86					-- LTCL
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	insert 	  into #c
	select pf.* 
	from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	where DATA_ID = 83					-- Cash and Short Term Investments (SCSI    )
	and pf.ISSUER_ID = @ISSUER_ID
	and pf.PERIOD_TYPE = 'A'
	   
	insert 	into #D
	select pf.* 
	from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	where DATA_ID = 51					-- Notes Payable/Short Term Debt(LSTD)
	and pf.ISSUER_ID = @ISSUER_ID
	and pf.PERIOD_TYPE = 'A'
END
ELSE
BEGIN
	insert   into #A
		select pf.* 
	    from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	   where DATA_ID = 56					-- ATCA
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	insert   into #B
	select pf.* 
	 from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	 where DATA_ID = 86					-- LTCL
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	insert 	  into #c
	select pf.* 
	from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	where DATA_ID = 83					-- Cash and Short Term Investments (SCSI    )
	and pf.ISSUER_ID = @ISSUER_ID
	and pf.PERIOD_TYPE = 'A'
	   
	insert 	into #D
	select pf.* 
	from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	where DATA_ID = 51					-- Notes Payable/Short Term Debt(LSTD)
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
		,  227 as DATA_ID										-- DATA_ID:227 Net Working Capital
		,  ( a.AMOUNT -c.amount)  - (b.AMOUNT-d.amount) as AMOUNT			-- (ATCA-SCSI) - (LTCL-LSTD)
		,  '(ATCA(' + CAST(a.AMOUNT as varchar(32)) + ') - SCSI(' + CAST(c.AMOUNT as varchar(32)) + '))-( LTCL(' + CAST(b.AMOUNT as varchar(32)) + ')- LSTD(' + CAST(d.AMOUNT as varchar(32)) + ')  )' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	inner join #C c on c.ISSUER_ID = b.ISSUER_ID 
					and c.DATA_SOURCE = b.DATA_SOURCE and c.PERIOD_TYPE = b.PERIOD_TYPE
					and c.PERIOD_YEAR = b.PERIOD_YEAR and c.FISCAL_TYPE = b.FISCAL_TYPE
					and c.CURRENCY = b.CURRENCY
	inner join #D d on d.ISSUER_ID = c.ISSUER_ID 
					and d.DATA_SOURCE = c.DATA_SOURCE and d.PERIOD_TYPE = c.PERIOD_TYPE
					and d.PERIOD_YEAR = c.PERIOD_YEAR and d.FISCAL_TYPE = c.FISCAL_TYPE
					and d.CURRENCY = c.CURRENCY
					
					
	 	 where 1=1 
	   --and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	    and a.PERIOD_TYPE = 'A'
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
		,  227 as DATA_ID										-- DATA_ID:227 Net Working Capital
		,  ( a.AMOUNT -c.amount)  - (b.AMOUNT-d.amount) as AMOUNT			-- (ATCA-SCSI) - (LTCL-LSTD)
		,  '(ATCA(' + CAST(a.AMOUNT as varchar(32)) + ') - SCSI(' + CAST(c.AMOUNT as varchar(32)) + '))-( LTCL(' + CAST(b.AMOUNT as varchar(32)) + ')- LSTD(' + CAST(d.AMOUNT as varchar(32)) + ')  )' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	inner join #C c on c.ISSUER_ID = b.ISSUER_ID 
					and c.DATA_SOURCE = b.DATA_SOURCE and c.PERIOD_TYPE = b.PERIOD_TYPE
					and c.PERIOD_YEAR = b.PERIOD_YEAR and c.FISCAL_TYPE = b.FISCAL_TYPE
					and c.CURRENCY = b.CURRENCY
	inner join #D d on d.ISSUER_ID = c.ISSUER_ID 
					and d.DATA_SOURCE = c.DATA_SOURCE and d.PERIOD_TYPE = c.PERIOD_TYPE
					and d.PERIOD_YEAR = c.PERIOD_YEAR and d.FISCAL_TYPE = c.FISCAL_TYPE
					and d.CURRENCY = c.CURRENCY
					
					
	 	 where 1=1 
	   --and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	    and a.PERIOD_TYPE = 'A'
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY	
	END
	COMMIT TRAN T1
	/*if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or ZERO data
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 227 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 227:Net Working Capital.  DATA_ID:56 ATCA is missing'
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			 and a.PERIOD_TYPE = 'A'
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 227 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 227:Net Working Capital.  DATA_ID:86 LTCL is missing' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			 and a.PERIOD_TYPE = 'A'
				 ) union (
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 227 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 227:Net Working Capital.  DATA_ID:56 ATCA no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 227 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 227:Net Working Capital.  DATA_ID:86 LTCL no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z	)
		END
		*/
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
	drop table #D
GO
