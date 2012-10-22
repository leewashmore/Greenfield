IF OBJECT_ID ( 'AIMS_Calc_137', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_137;
GO
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:137 Net Interest Margin
--
--			 (ENII /AVERAGE(SOEA+ANTL for Year, SOEA+ANTL for Prior Year))-1
--
-- Author:	David Muench
-- Date:	July 2, 2012
------------------------------------------------------------------------
create procedure AIMS_Calc_137(
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 17					-- ENII
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'			-- Only Annual

	-- SOEA+ANTL for current year
	select pf.ISSUER_ID, pf.SECURITY_ID, max(pf.COA_TYPE) as COA_TYPE, pf.DATA_SOURCE, max(pf.ROOT_SOURCE) as ROOT_SOURCE
		  , pf.ROOT_SOURCE_DATE, pf.PERIOD_TYPE, pf.PERIOD_YEAR, pf.PERIOD_END_DATE, pf.FISCAL_TYPE, pf.CURRENCY
		  , 0 as DATA_ID, sum(AMOUNT) as AMOUNT, ' ' as CALCULATION_DIAGRAM, pf.SOURCE_CURRENCY, pf.AMOUNT_TYPE
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID in (58,59)			-- SOEA, ANTL
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'			-- Only Annual
	 group by ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , SOURCE_CURRENCY, AMOUNT_TYPE

	-- SOEA+ANTL for prior year
	select pf.ISSUER_ID, pf.SECURITY_ID, max(pf.COA_TYPE) as COA_TYPE, pf.DATA_SOURCE, max(pf.ROOT_SOURCE) as ROOT_SOURCE
		  , pf.ROOT_SOURCE_DATE, pf.PERIOD_TYPE, pf.PERIOD_YEAR+1 as PERIOD_YEAR, pf.PERIOD_END_DATE, pf.FISCAL_TYPE, pf.CURRENCY
		  , 0 as DATA_ID, sum(AMOUNT) as AMOUNT, ' ' as CALCULATION_DIAGRAM, pf.SOURCE_CURRENCY, pf.AMOUNT_TYPE
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID in (58,59)			-- SOEA, ANTL
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'			-- Only Annual
	 group by ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , SOURCE_CURRENCY, AMOUNT_TYPE


	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select b.ISSUER_ID, b.SECURITY_ID, b.COA_TYPE, b.DATA_SOURCE, b.ROOT_SOURCE
		,  b.ROOT_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  137 as DATA_ID										-- DATA_ID:137 Net Interest Margin
		,  (isnull(a.AMOUNT, 0.0) / ((b.AMOUNT + isnull(c.AMOUNT, b.AMOUNT))/2)) -1 as AMOUNT						-- 136/SOEA
		,  '(ENII(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') / AVERAGE(SOEA+ANTL current year(' + CAST(b.AMOUNT as varchar(32)) + '), SOEA+ANTL prior year(' + CAST(isnull(c.AMOUNT,b.AMOUNT) as varchar(32)) + ')))-1' as CALCULATION_DIAGRAM
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
	  from #B b
	  left join	#A a on a.ISSUER_ID = b.ISSUER_ID 
					and a.DATA_SOURCE = b.DATA_SOURCE and a.PERIOD_TYPE = b.PERIOD_TYPE
					and a.PERIOD_YEAR = b.PERIOD_YEAR and a.FISCAL_TYPE = b.FISCAL_TYPE
					and a.CURRENCY = b.CURRENCY
	  left join	#C c on c.ISSUER_ID = a.ISSUER_ID
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY
	 where 1=1 
	   and b.AMOUNT <> 0.0
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 138 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 137:Net Interest Margin.  DATA_ID:58 SOEA+59 ANTL current year is NULL or ZERO' as TXT
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 137 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 137:Net Interest Margin.  DATA_ID:17 ENII is missing'
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (
			select GETDATE() as LOG_DATE, 137 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 137:Net Interest Margin.  DATA_ID:58 SOEA+59 ANTL prior year is missing'
			  from #B a
			  left join	#C b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 137 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 137:Net Interest Margin.  DATA_ID:17 ENII no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 137 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 137:Net Interest Margin.  DATA_ID:58 SOEA+59 ANTL current year no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 137 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 137:Net Interest Margin.  DATA_ID:58 SOEA+59 ANTL prior year no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
go

-- exec AIMS_Calc_137 330892
-- select * from PERIOD_FINANCIALS where DATA_ID = 137
-- select * from calc_log where issuer_Id = 330892 and data_id = 137
