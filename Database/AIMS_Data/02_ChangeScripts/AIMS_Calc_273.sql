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
	 where DATA_ID = 36 					-- Income Before Tax (EIBT)
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 18 					-- Loan Loss Provision (ELLP)
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 75					-- Total Assets (ATOT)
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  273 as DATA_ID										-- DATA_ID:273 Pre-Provision Profits as % of  Assets
		,  (a.AMOUNT + b.AMOUNT) / ((c.AMOUNT+d.AMOUNT)/2) as AMOUNT			-- EIBT + ELLP / Avg(ATOT)/2
		,  'Income Before Tax(' + CAST(a.AMOUNT as varchar(32)) + ') + Loan Loss Provision (' + CAST(b.AMOUNT as varchar(32)) +') / (Total Assets (' + CAST(c.AMOUNT as varchar(32)) + ')+ Total Assets prior year(' + CAST(d.AMOUNT as varchar(32)) + '))/2' as CALCULATION_DIAGRAM
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
	 inner join	#C d on d.ISSUER_ID = a.ISSUER_ID 
					and d.DATA_SOURCE = a.DATA_SOURCE and d.PERIOD_TYPE = a.PERIOD_TYPE
					and d.PERIOD_YEAR = a.PERIOD_YEAR-1 and d.FISCAL_TYPE = a.FISCAL_TYPE
					and d.CURRENCY = a.CURRENCY
					
	 where 1=1 
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 273 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 273:Pre-Provision Profits as % of  Assets.  DATA_ID:75 is NULL or ZERO' as TXT
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 273 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 273 Pre-Provision Profits as % of  Assets.  DATA_ID:75 is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 273 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 273: Pre-Provision Profits as % of  Assets.  DATA_ID:36 EIBT is missing'
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (

			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 273 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 273: Pre-Provision Profits as % of  Assets.  DATA_ID:75 no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 273 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 273: Pre-Provision Profits as % of  Assets.  DATA_ID:36 EIBT no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			)
		END
		
	drop table #A
	drop table #B
	drop table #C
	
GO


