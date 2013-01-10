------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:273  Profit Margins Before Provisions
--
--			(NINC+ELLP)/(SNII+SIIB)
--
-- Author:	Prerna 
-- Date:	July 17, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_273', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_273;
GO

create procedure [dbo].[AIMS_Calc_273](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 44					-- NINC
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 18					-- ELLP
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   
	select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 33					-- SNII
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   
	select pf.* 
	  into #D
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 9					-- SIIB
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	-- Add the data to the table
	
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  273 as DATA_ID										-- DATA_ID:273  Profit Margins Before Provisions
		,  (a.AMOUNT + b.AMOUNT) / (c.AMOUNT + d.AMOUNT) as AMOUNT			-- (NINC+ELLP)/(SNII+SIIB)
		,  '(NINC(' + CAST(a.AMOUNT as varchar(32)) + ') + ELLP('+ CAST(b.AMOUNT as varchar(32)) + ')) / ( SNII(' + CAST(c.AMOUNT as varchar(32)) + ') + SIIB (' + CAST(d.AMOUNT as varchar(32)) + '))' as CALCULATION_DIAGRAM
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
	 inner join	#D d on d.ISSUER_ID = a.ISSUER_ID 
					and d.DATA_SOURCE = a.DATA_SOURCE and d.PERIOD_TYPE = a.PERIOD_TYPE
					and d.PERIOD_YEAR = a.PERIOD_YEAR and d.FISCAL_TYPE = a.FISCAL_TYPE
					and d.CURRENCY = a.CURRENCY
	 where 1=1 
	   and isnull(c.AMOUNT + d.AMOUNT , 0.0) <> 0.0	-- Data validation
	    and a.PERIOD_TYPE = 'A'
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or ZERO data
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 273 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:(33+9) is NULL or ZERO'
			  from #D a
			  inner join	#C b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where isnull(a.AMOUNT + b.amount, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 273 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:44 NINC is missing'
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			 and a.PERIOD_TYPE = 'A'
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 273 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:18 ELLP is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			 and a.PERIOD_TYPE = 'A'
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 273 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:33 SNII is missing' as TXT
			  from #A a
			  left join	#C b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			 and a.PERIOD_TYPE = 'A'
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 273 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:9 SIIB is missing' as TXT
			  from #A a
			  left join	#D b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			 and a.PERIOD_TYPE = 'A'
			
			 ) union (
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 273 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:44 NINC no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 273 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:18 ELLP no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			 ) union (
				select GETDATE() as LOG_DATE, 273 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:33 SNII no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			 ) union (
				select GETDATE() as LOG_DATE, 273 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 273  Profit Margins Before Provisions.  DATA_ID:9 SIIB no data' as TXT
			  from (select COUNT(*) CNT from #D having COUNT(*) = 0) z
			
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
	drop table #D
	



