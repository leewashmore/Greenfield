------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:171 P/CE 
--
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
--
-- Author:	David Muench
-- Date:	Sept 6, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'CCE_Calc_171', 'P' ) IS NOT NULL 
DROP PROCEDURE CCE_Calc_171;
GO

CREATE procedure [dbo].[CCE_Calc_171](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf  
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and (@ISSUER_ID is null or @ISSUER_ID = sb.ISSUER_ID)
	   and pf.PERIOD_TYPE = 'A'

	select cce.* 
	  into #B
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 3 			-- CFPS
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select a.ISSUER_ID, a.SECURITY_ID, b.DATA_SOURCE
		,  b.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  171 as ESTIMATE_ID										-- DATA_ID:171
		,  a.AMOUNT / b.AMOUNT as AMOUNT			
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B b on b.ISSUER_ID = sb.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 171 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 171 P/CE .  ESTIMATE_ID:3 CFPS is NULL or ZERO'
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 171 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 171 P/CE .  DATA_ID:185 is missing' as TXT
			  from #A a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B b on b.ISSUER_ID = sb.ISSUER_ID  
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 171 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 171 P/CE .  ESTIMATE_ID:3 CFPS is missing' as TXT
			  from #B a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#A b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 171 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 171 P/CE .  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 171 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 171 P/CE .  ESTIMATE_ID:3 CFPS no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	



-- exec CCE_Calc_171 '847547'
-- select * from CURRENT_CONSENSUS_ESTIMATES where ESTIMATE_ID = 171