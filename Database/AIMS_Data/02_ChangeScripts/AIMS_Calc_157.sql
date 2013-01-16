------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:157 Free Cash Flow
--
--			130 - TTAX + SOCF + SCEX
--
-- Author:	Shivani
-- Date:	July 13, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_157', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_157;
GO

CREATE procedure [dbo].[AIMS_Calc_157](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write out log values to CALC_LOG table
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 130		-- SOPI*SDPR
	   and pf.ISSUER_ID = @ISSUER_ID
	  

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 37			--TTAX   
	   and pf.ISSUER_ID = @ISSUER_ID
	   	   	   
	   
	select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 116			--SOCF   
	   and pf.ISSUER_ID = @ISSUER_ID
	   
	   
	select pf.* 
	  into #D
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 118			--SCEX   
	   and pf.ISSUER_ID = @ISSUER_ID
	   
	   
	   

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  157 as DATA_ID										-- DATA_ID:157 Free Cash Flow
		,  isnull(a.AMOUNT, 0.0) - isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0) + isnull(d.AMOUNT, 0.0) as AMOUNT		-- 130 - TTAX + SOCF - SCEX
		,  '130(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') - TTAX(' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + SOCF(' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ') + SCEX (' + CAST(isnull(d.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID and b.CURRENCY = a.CURRENCY
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 inner join	#C c on c.ISSUER_ID = a.ISSUER_ID and c.CURRENCY = a.CURRENCY
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.AMOUNT_TYPE = a.AMOUNT_TYPE
	 inner join	#D d on d.ISSUER_ID = a.ISSUER_ID and d.CURRENCY = a.CURRENCY	
					and d.DATA_SOURCE = a.DATA_SOURCE and d.PERIOD_TYPE = a.PERIOD_TYPE
					and d.PERIOD_YEAR = a.PERIOD_YEAR and d.FISCAL_TYPE = a.FISCAL_TYPE
					and d.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	   
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 157 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 157 Free Cash Flow .  DATA_ID:37 TTAX is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.AMOUNT_TYPE = a.AMOUNT_TYPE
			 where 1=1 and b.ISSUER_ID is NULL	   

			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 157 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 157 Free Cash Flow.  DATA_ID:130 is missing' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.AMOUNT_TYPE = a.AMOUNT_TYPE
			 where 1=1 and b.ISSUER_ID is NULL	  

			) union (
			 
			 -- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 157 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'WARNING calculating 157 Free Cash Flow.  DATA_ID:116 SOCF is missing' as TXT
			  from #A a
			  left join	#C b on b.ISSUER_ID = a.ISSUER_ID and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.AMOUNT_TYPE = a.AMOUNT_TYPE
			 where 1=1 and b.ISSUER_ID is NULL
			 
			 ) union (
			 
			 -- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 157 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'WARNING calculating 157 Free Cash Flow.  DATA_ID:118 SCEX is missing' as TXT
			  from #A a
			  left join	#D b on b.ISSUER_ID = a.ISSUER_ID and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.AMOUNT_TYPE = a.AMOUNT_TYPE
			 where 1=1 and b.ISSUER_ID is NULL
			 
			  	 
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 157 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 157 Free Cash Flow .  DATA_ID:130 SOPI+SDPR no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z

			) union (	

			select GETDATE() as LOG_DATE, 157 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 157 Free Cash Flow.  DATA_ID:37 TTAX no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z

			)union (	

			select GETDATE() as LOG_DATE, 157 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 157 Free Cash Flow.  DATA_ID:116 SOCF no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z

			)union (	

			select GETDATE() as LOG_DATE, 157 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 157 Free Cash Flow.  DATA_ID:118 SCEX no data' as TXT
			  from (select COUNT(*) CNT from #D having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
	drop table #D
	



-- exec AIMS_CALC_157 '330892'