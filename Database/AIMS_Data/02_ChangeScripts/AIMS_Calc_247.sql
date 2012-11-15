
IF OBJECT_ID ( 'AIMS_Calc_247', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_247;
GO

------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:247 Forward EV/EBITDA for Industry of Security
-- 198 / Forward EV/EBITDA for Industry of Security
-- Author:	Shivani
-- Date:	Aug 22, 2012
------------------------------------------------------------------------
CREATE procedure [dbo].[AIMS_Calc_247](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as
print 'In calc_247'
	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 198					
	   and sb.ISSUER_ID = @ISSUER_ID;

select 'Calc_247 #A' as A, * from #A
	select bf.*, sb.SECURITY_ID
	  into #B
	  from dbo.BENCHMARK_NODE_FINANCIALS bf
	  inner join GF_SECURITY_BASEVIEW sb on bf.NODE_ID1 = sb.GICS_INDUSTRY  	  
	  where bf.DATA_ID = 198
	   and sb.ISSUER_ID = @ISSUER_ID
	   and bf.BENCHMARK_ID = 'MSCI EM NET'
	   and bf.NODE_NAME1 = 'INDUSTRY'
	   and bf.NODE_NAME2 is null
	   and bf.PERIOD_TYPE = 'C' ;
select 'Calc_247 #B' as B, * from #B
	    

	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  247 as DATA_ID							-- DATA_ID:247 198 / Forward EV/EBITDA for Industry of Security
		,  a.AMOUNT / b.AMOUNT as AMOUNT			-- 198 / Forward EV/EBITDA for Industry of Security
		,  '198(' + CAST(a.AMOUNT as varchar(32)) + ') / (Forward EV/EBITDA('  + CAST(b.AMOUNT as varchar(32))  + ')'   as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.PERIOD_TYPE = a.PERIOD_TYPE					
					and b.CURRENCY = a.CURRENCY
	 where 1=1 	  
	  and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation	;

	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 247 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 247: Forward EV/EBITDA Relative to Industry.  Forward EV/EBITDA Relative to Industry is NULL or ZERO'
			   from #A a
			 inner join	#B b on b.PERIOD_TYPE = a.PERIOD_TYPE					
							and b.CURRENCY = a.CURRENCY	 
			 where isnull(b.AMOUNT, 0.0) = 0.0	-- Data error	  
			-- Error conditions - missing data 
			) union (	
				select GETDATE() as LOG_DATE, 247 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating  247: Forward EV/EBITDA Relative to Industry is missing' as TXT
			   from #A a
			 left join	#B b on b.PERIOD_TYPE = a.PERIOD_TYPE					
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.AMOUNT is NULL	 
			) union	(
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 247 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 247: Forward EV/EBITDA Relative to Industry.  DATA_ID:198 is missing'
			  from #B b
			 left join	#A a on a.SECURITY_ID = b.SECURITY_ID
							and a.PERIOD_TYPE = b.PERIOD_TYPE					
							and a.CURRENCY = b.CURRENCY
			 where 1=1 and a.ISSUER_ID is NULL	  
			) union	(
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 247 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 247: Forward EV/EBITDA Relative to Industry.  DATA_ID:198 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union	(
			select GETDATE() as LOG_DATE, 247 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 247: Forward EV/EBITDA Relative to Industry.  Forward EV/EBITDA no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B

-- exec aims_calc_247 '13849318'