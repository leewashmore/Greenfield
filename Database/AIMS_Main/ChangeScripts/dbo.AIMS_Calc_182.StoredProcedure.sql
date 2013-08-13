
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Calc_182]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Calc_182]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Calc_182]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_Calc_182](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
select pf.ISSUER_ID, 
		pf.SECURITY_ID, 
		pf.COA_TYPE, 
		pf.DATA_SOURCE, 
		max(pf.ROOT_SOURCE) as ROOT_SOURCE, 
		MAX(pf.ROOT_SOURCE_DATE) as ROOT_SOURCE_DATE, 
		PERIOD_TYPE, 
		PERIOD_YEAR, 
		PERIOD_END_DATE, 
		FISCAL_TYPE, 
		CURRENCY, 
		SUM(isnull(amount,0)) as AMOUNT, 
		MAX(SOURCE_CURRENCY) as SOURCE_CURRENCY, 
		MAX(amount_type) as AMOUNT_TYPE
	  into #A
	  from dbo.PERIOD_FINANCIALS pf with (nolock)
	 where DATA_ID in ('63','64','65','66')
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	group by ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY 
	 order by PERIOD_YEAR


select pf.ISSUER_ID, 
		pf.SECURITY_ID, 
		pf.COA_TYPE, 
		pf.DATA_SOURCE, 
		max(pf.ROOT_SOURCE) as ROOT_SOURCE, 
		MAX(pf.ROOT_SOURCE_DATE) as ROOT_SOURCE_DATE, 
		PERIOD_TYPE, 
		PERIOD_YEAR+1 as PERIOD_YEAR, 
		PERIOD_END_DATE, 
		FISCAL_TYPE, 
		CURRENCY, 
		SUM(isnull(amount,0)) as AMOUNT, 
		MAX(SOURCE_CURRENCY) as SOURCE_CURRENCY, 
		MAX(amount_type) as AMOUNT_TYPE
	  into #B
	  from dbo.PERIOD_FINANCIALS pf with (nolock)
	 where DATA_ID in ('63','64','65','66')
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	group by ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY 
	 order by PERIOD_YEAR

/*
	select pf.PERIOD_YEAR + 1 as PRIOR_YEAR, pf.PERIOD_YEAR as PERIOD_YEAR,pf.CURRENCY,pf.COA_TYPE,pf.DATA_SOURCE,pf.FISCAL_TYPE,pf.PERIOD_TYPE,pf.ISSUER_ID,pf.AMOUNT,pf.PERIOD_END_DATE
	  into #B
	  from dbo.PERIOD_FINANCIALS pf with (nolock)
	 where DATA_ID = 104			--QTLE
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	 order by PERIOD_YEAR
*/

	-- Add the data to the table
	BEGIN TRAN T1
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  182 as DATA_ID										-- DATA_ID:182 Investment Asset Growth (YOY)
		,  CASE WHEN a.AMOUNT >= 0 and b.AMOUNT > 0 THEN (a.AMOUNT /b.AMOUNT)-1 
				ELSE NULL 
				END as AMOUNT						--  (Inv assets for Year/ Inv assets for Prior Year)-1
		,  '(Investment Assets for ('+ CAST(a.PERIOD_YEAR as varchar(32)) + '(' + CAST(a.AMOUNT as varchar(32)) + ') / Investment Assets for ('+ CAST(b.PERIOD_YEAR-1 as varchar(32)) +')(' + CAST(b.AMOUNT as varchar(32)) + ')) - 1 ' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and a.PERIOD_TYPE = 'A'
	    and 
	  	 isnull(a.AMOUNT, 0.0)>=0  and isnull(b.AMOUNT, 0.0) > 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	COMMIT TRAN T1
	
	if @CALC_LOG = 'Y'
/*		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 184 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 184  Equity Growth (YOY) .  DATA_ID:104 is NULL or ZERO for prior period'
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 184 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 184  Equity Growth (YOY) .  DATA_ID:104 is missing for the prior period' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PRIOR_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data for prior period
			select GETDATE() as LOG_DATE, 184 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 184  Equity Growth (YOY) .  DATA_ID:104 is missing for the year' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PRIOR_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 184 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 184  Equity Growth (YOY) .  DATA_ID:104 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
	*/
		
	-- Clean up
	drop table #A
	drop table #B

GO


