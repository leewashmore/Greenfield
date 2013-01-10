------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:274 Debt Instruments/Assets
--
--			(LTTD+LSTB)/ATOT
--
-- Author:	Aniket 
-- Date:	July 16, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_274', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_274;
GO

CREATE procedure [dbo].[AIMS_Calc_274](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	select pf.* 
	into #A
	from dbo.PERIOD_FINANCIALS pf 
	   where DATA_ID = 89				-- LTTD
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 82					-- LSTB
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 75					-- ATOT
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select c.ISSUER_ID, c.SECURITY_ID, c.COA_TYPE, c.DATA_SOURCE, c.ROOT_SOURCE
		,  c.ROOT_SOURCE_DATE, c.PERIOD_TYPE, c.PERIOD_YEAR, c.PERIOD_END_DATE
		,  c.FISCAL_TYPE, c.CURRENCY
		,  274 as DATA_ID										-- DATA_ID:274 Debt Instruments/Assets
		,  (isnull(a.AMOUNT, 0.0) +  isnull(b.AMOUNT, 0.0)) / c.AMOUNT as AMOUNT			-- (LTTD+LSTB)/ATOT
		,  '(LTTD(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') +  LSTB(' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ')) / ATOT('  + CAST(c.AMOUNT as varchar(32)) + ') ' as CALCULATION_DIAGRAM
		,  c.SOURCE_CURRENCY
		,  c.AMOUNT_TYPE
	  from #C c
	  left join	#B b on b.ISSUER_ID = c.ISSUER_ID 
					and b.DATA_SOURCE = c.DATA_SOURCE and b.PERIOD_TYPE = c.PERIOD_TYPE
					and b.PERIOD_YEAR = c.PERIOD_YEAR and b.FISCAL_TYPE = c.FISCAL_TYPE
					and b.CURRENCY = c.CURRENCY
	  left join	#A a on a.ISSUER_ID = c.ISSUER_ID 
					and a.DATA_SOURCE = c.DATA_SOURCE and a.PERIOD_TYPE = c.PERIOD_TYPE
					and a.PERIOD_YEAR = c.PERIOD_YEAR and a.FISCAL_TYPE = c.FISCAL_TYPE
					and a.CURRENCY = c.CURRENCY
	 where 1=1 
	   and isnull(c.AMOUNT, 0.0) <> 0.0	-- Data validation
	   and (a.AMOUNT is not null or b.AMOUNT is not null)	-- Both Numerator values cannot be null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or ZERO data
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 274 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 274:Debt Instruments/Assets.  DATA_ID:75 ATOT is NULL or ZERO'
			  from #C a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 274 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 274:Debt Instruments/Assets.  DATA_ID:89 LTTD is missing'
			  from #C a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 274 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 274:Debt Instruments/Assets.  DATA_ID:82 LSTB is missing' as TXT
			  from #C a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (
			 
			 -- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 274 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 274:Debt Instruments/Assets.  DATA_ID:75 ATOT is missing' as TXT
			  from #A a
			  left join	#C b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 274 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 274:Debt Instruments/Assets.  DATA_ID:89 LTTD no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 274 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 274:Debt Instruments/Assets.  DATA_ID:82 LSTB no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 274 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 274:Debt Instruments/Assets.  DATA_ID:75 ATOT no data' as TXT
			  from (select COUNT(*) CNT  from #C having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C



