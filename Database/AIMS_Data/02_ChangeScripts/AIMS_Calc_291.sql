------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:291 Depreciation/Amortization 
--
--			IFF((SDED + SAMT) = 0, SDPR, (SDED + SAMT))
--
-- Author:	David Muench
-- Date:	October 16, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_291', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_291;
GO

create procedure [dbo].[AIMS_Calc_291](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as
	-- Get the data
	select pf.* 
	  into #A
	   from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 112		-- SDED
	   and pf.ISSUER_ID = @ISSUER_ID
	 order by PERIOD_YEAR

	select pf.* 
	  into #B
	   from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 113		-- SAMT
	   and pf.ISSUER_ID = @ISSUER_ID
	 order by PERIOD_YEAR

	select pf.* 
	  into #C
	   from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 24			-- SDPR
	   and pf.ISSUER_ID = @ISSUER_ID
	 order by PERIOD_YEAR

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  291 as DATA_ID							-- DATA_ID:290 Earnings 
		,  case when (a.AMOUNT + isnull(b.AMOUNT, 0.0)) <> 0 
				then (a.AMOUNT + isnull(b.AMOUNT, 0.0))
				else (isnull(c.AMOUNT, 0.0)) end as AMOUNT
		,  case when (a.AMOUNT + isnull(b.AMOUNT, 0.0)) <> 0 
				then 'SDED('+ cast(a.AMOUNT as varchar(32)) + ') + SAMT('+ CAST( isnull(b.AMOUNT, 0.0) as varchar(32)) + ')'
				else 'SDPR('+ cast(isnull(c.AMOUNT, 0.0) as varchar(32)) + ')' end as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	  left join	#C c on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY
	 where 1=1 
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 291 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 291 Depreciation/Amortization. DATA_ID:112 SDED no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z

			) union (
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 291 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 291 Depreciation/Amortization. DATA_ID:113 SAMT no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z

			) union (
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 291 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 291 Depreciation/Amortization. DATA_ID:24 SDPR no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A
	



