SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------------------
-- Name:	Get_Consensus
--
-- Purpose:	Get the consensus data to display the statement in the form that the 
--			user needs it.  The Actual data must be combined with the estimated  data
--			so that Actual is always used in case both are present.
--
--------------------------------------------------------------------------------------
alter procedure [dbo].[Get_Consensus](
	@ISSUER_ID			varchar(20)					-- The company identifier		
,	@DATA_SOURCE		varchar(10)  = 'REUTERS'	-- REUTERS, PRIMARY, INDUSTRY
,	@PERIOD_TYPE		char(2) = 'A'				-- A, Q
,	@FISCAL_TYPE		char(8) = 'FISCAL'			-- FISCAL, CALENDAR
,	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
,	@ESTIMATE_ID		integer = NULL				-- A specific ID or NULL for all.
,	@PERIOD_YEAR		integer = NULL				-- A specific year if required	
)
as

	-- Select the Actual data 
	select cce.ISSUER_ID
		,  cce.ESTIMATE_ID
		,  cm.ESTIMATE_DESC
		,  substring(AMOUNT_TYPE,1,1)+cast(cce.PERIOD_YEAR as CHAR(4))+cce.PERIOD_TYPE as Period
		,  cce.AMOUNT_TYPE
		,  cce.PERIOD_YEAR
		,  cce.PERIOD_TYPE
		,  cce.AMOUNT
		,  0.0 as ASHMOREEMM_AMOUNT
		,  cce.NUMBER_OF_ESTIMATES
		,  cce.HIGH
		,  cce.LOW
		,  cce.STANDARD_DEVIATION
		,  cce.SOURCE_CURRENCY
		,  cce.DATA_SOURCE
		,  cce.DATA_SOURCE_DATE
	  into #Actual
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_ID = cce.ESTIMATE_ID
	 where 1=1
	   and cce.ISSUER_ID = @ISSUER_ID
	   and cce.DATA_SOURCE = @DATA_SOURCE
	   and substring(cce.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and cce.FISCAL_TYPE = @FISCAL_TYPE
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ACTUAL'
	   and (@ESTIMATE_ID is NULL or (cce.ESTIMATE_ID = @ESTIMATE_ID))
	   and (@PERIOD_YEAR is NULL or (cce.PERIOD_YEAR = @PERIOD_YEAR))
	 order by cce.PERIOD_YEAR
	 ;

	-- Select the Estimated data
	select cce.ISSUER_ID
		,  cce.ESTIMATE_ID
		,  cm.ESTIMATE_DESC
		,  substring(cce.AMOUNT_TYPE,1,1)+cast(cce.PERIOD_YEAR as CHAR(4))+cce.PERIOD_TYPE as Period
		,  cce.AMOUNT_TYPE
		,  cce.PERIOD_YEAR
		,  cce.PERIOD_TYPE
		,  cce.AMOUNT
		,  0.0 as ASHMOREEMM_AMOUNT
		,  cce.NUMBER_OF_ESTIMATES
		,  cce.HIGH
		,  cce.LOW
		,  cce.STANDARD_DEVIATION
		,  cce.SOURCE_CURRENCY
		,  cce.DATA_SOURCE
		,  cce.DATA_SOURCE_DATE
	  into #Estimate
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
	  left join #Actual a on a.ISSUER_ID = cce.ISSUER_ID and a.ESTIMATE_ID = cce.ESTIMATE_ID
						 and a.PERIOD_YEAR = cce.PERIOD_YEAR and a.PERIOD_TYPE = cce.PERIOD_TYPE
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_ID = cce.ESTIMATE_ID
	 where a.ISSUER_ID is NULL
	   and cce.ISSUER_ID = @ISSUER_ID
	   and cce.DATA_SOURCE = @DATA_SOURCE
	   and substring(cce.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and cce.FISCAL_TYPE = @FISCAL_TYPE
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ESTIMATE'
	   and (@ESTIMATE_ID is NULL or (cce.ESTIMATE_ID = @ESTIMATE_ID))
	   and (@PERIOD_YEAR is NULL or (cce.PERIOD_YEAR = @PERIOD_YEAR))
	 order by cce.PERIOD_YEAR
	 ;

	
	-- Combine Actual with Estimated so that Actual is always used in case both are present.
	select a.ISSUER_ID
		,  a.ESTIMATE_ID
		,  a.ESTIMATE_DESC
		,  a.Period
		,  a.AMOUNT_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_TYPE
		,  a.AMOUNT
		,  b.AMOUNT as ASHMOREEMM_AMOUNT
		,  a.NUMBER_OF_ESTIMATES
		,  a.HIGH
		,  a.LOW
		,  a.STANDARD_DEVIATION
		,  a.SOURCE_CURRENCY
		,  a.DATA_SOURCE
		,  a.DATA_SOURCE_DATE
	  from (select * from #Actual
			union
			select * from #Estimate
		   ) a
	  left join (Select pf.ISSUER_ID, pf.AMOUNT, pf.AMOUNT_TYPE, pf.DATA_ID
					,   pf.FISCAL_TYPE, pf.CURRENCY, pf.PERIOD_YEAR, pf.PERIOD_TYPE
				   from PERIOD_FINANCIALS pf
				  where 1=1
				    and pf.DATA_SOURCE = 'PRIMARY'
				    and substring(pf.PERIOD_TYPE,1,1) = @PERIOD_TYPE
				    and pf.FISCAL_TYPE = @FISCAL_TYPE
				    and pf.CURRENCY = @CURRENCY
				    and pf.ISSUER_ID = @ISSUER_ID
				 ) b on b.ISSUER_ID = a.ISSUER_ID
					and (   (b.DATA_ID =  11 and a.ESTIMATE_ID = 17)		-- Revenue
						 or (b.DATA_ID = 130 and a.ESTIMATE_ID = 7)			-- EBITDA
						 or (b.DATA_ID =  44 and a.ESTIMATE_ID in(11,12,13))-- Net Income
						 or (b.DATA_ID = 131 and a.ESTIMATE_ID in(8,9,5))	-- EPS
						 or (b.DATA_ID = 132 and a.ESTIMATE_ID = 18)		-- ROA
						 or (b.DATA_ID = 133 and a.ESTIMATE_ID = 19)		-- ROE
						)
				    and substring(b.PERIOD_TYPE,1,1) = @PERIOD_TYPE
					and b.FISCAL_TYPE = @FISCAL_TYPE
					and b.CURRENCY = @CURRENCY
					and b.AMOUNT_TYPE = 'ESTIMATE'
					and (@PERIOD_YEAR is NULL or (b.PERIOD_YEAR = @PERIOD_YEAR))
		;



	-- Clean up
	drop table #Actual;
	drop table #Estimate;
GO
