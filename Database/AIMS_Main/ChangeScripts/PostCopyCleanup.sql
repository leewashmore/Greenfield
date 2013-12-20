IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PostCopyCleanup]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[PostCopyCleanup]
GO

/****** Object:  StoredProcedure [dbo].[PostCopyCleanup]    Script Date: 12/10/2013 09:27:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[PostCopyCleanup]
as


----------------------------------------------------------------------------------------------------------------
-- This procedure is designed to be run at the end or after the SSIS copy has completed and performs some post  
-- copy cleanup that used to occur during hte materialization of the WCF.
----------------------------------------------------------------------------------------------------------------


-------AIMS_MARKET_DATA-------

--Nothing to Do



-------AIMS_PORTFOLIO_PERFORMANCE-------

--Nothing to Do



-------AIMS_PORTFOLIO-------
--GF_BENCHMARK_HOLDINGS
Truncate TABLE GF_BENCHMARK_HOLDINGS;
INSERT INTO GF_BENCHMARK_HOLDINGS SELECT * FROM GFq_BENCHMARK_HOLDINGS;
delete from GF_BENCHMARK_HOLDINGS where benchmark_id in
	(select bhco.benchmark_id from GFq_BENCHMARK_HOLDING_CARVEOUT bhco group by bhco.benchmark_id);
INSERT INTO GF_BENCHMARK_HOLDINGS
   (GF_ID
   ,PORTFOLIO_DATE
   ,PORTFOLIO_ID
   ,PORTFOLIO_THEME_SUBGROUP_CODE
   ,PORTFOLIO_CURRENCY
   ,BENCHMARK_ID
   ,ISSUER_ID
   ,ASEC_SEC_SHORT_NAME
   ,ISSUE_NAME
   ,TICKER
   ,SECURITYTHEMECODE
   ,A_SEC_INSTR_TYPE
   ,SECURITY_TYPE
   ,BALANCE_NOMINAL
   ,DIRTY_PRICE
   ,TRADING_CURRENCY
   ,DIRTY_VALUE_PC
   ,BENCHMARK_WEIGHT --reuse old field
   ,ASH_EMM_MODEL_WEIGHT
   ,MARKET_CAP_IN_USD
   ,ASHEMM_PROP_REGION_CODE
   ,ASHEMM_PROP_REGION_NAME
   ,ISO_COUNTRY_CODE
   ,COUNTRYNAME
   ,GICS_SECTOR
   ,GICS_SECTOR_NAME
   ,GICS_INDUSTRY
   ,GICS_INDUSTRY_NAME
   ,GICS_SUB_INDUSTRY
   ,GICS_SUB_INDUSTRY_NAME
   ,LOOK_THRU_FUND
   ,BARRA_RISK_FACTOR_MOMENTUM
   ,BARRA_RISK_FACTOR_VOLATILITY
   ,BARRA_RISK_FACTOR_VALUE
   ,BARRA_RISK_FACTOR_SIZE
   ,BARRA_RISK_FACTOR_SIZE_NONLIN
   ,BARRA_RISK_FACTOR_GROWTH
   ,BARRA_RISK_FACTOR_LIQUIDITY
   ,BARRA_RISK_FACTOR_LEVERAGE
   ,BARRA_RISK_FACTOR_PBETEWLD)
   SELECT (GF_ID + 99000000) AS GF_ID
         ,PORTFOLIO_DATE
         ,PORTFOLIO_ID
         ,PORTFOLIO_THEME_SUBGROUP_CODE
         ,PORTFOLIO_CURRENCY
         ,BENCHMARK_ID
         ,ISSUER_ID
         ,ASEC_SEC_SHORT_NAME
         ,ISSUE_NAME
         ,TICKER
         ,SECURITYTHEMECODE
         ,A_SEC_INSTR_TYPE
         ,SECURITY_TYPE
         ,BALANCE_NOMINAL
         ,DIRTY_PRICE
         ,TRADING_CURRENCY
         ,DIRTY_VALUE_PC
         ,BENCHMARK_WEIGHT_REBASED --for new rebased figure
         ,ASH_EMM_MODEL_WEIGHT
         ,MARKET_CAP_IN_USD
         ,ASHEMM_PROP_REGION_CODE
         ,ASHEMM_PROP_REGION_NAME
         ,ISO_COUNTRY_CODE
         ,COUNTRYNAME
         ,GICS_SECTOR
         ,GICS_SECTOR_NAME
         ,GICS_INDUSTRY
         ,GICS_INDUSTRY_NAME
         ,GICS_SUB_INDUSTRY
         ,GICS_SUB_INDUSTRY_NAME
         ,LOOK_THRU_FUND
         ,BARRA_RISK_FACTOR_MOMENTUM
         ,BARRA_RISK_FACTOR_VOLATILITY
         ,BARRA_RISK_FACTOR_VALUE
         ,BARRA_RISK_FACTOR_SIZE
         ,BARRA_RISK_FACTOR_SIZE_NONLIN
         ,BARRA_RISK_FACTOR_GROWTH
         ,BARRA_RISK_FACTOR_LIQUIDITY
         ,BARRA_RISK_FACTOR_LEVERAGE
         ,BARRA_RISK_FACTOR_PBETEWLD
   FROM   GFq_BENCHMARK_HOLDING_CARVEOUT;

--GF_PORTFOLIO_HOLDINGS
Truncate table GF_PORTFOLIO_HOLDINGS;
INSERT INTO GF_PORTFOLIO_HOLDINGS SELECT * FROM GFq_PORTFOLIO_HOLDINGS;
--  Deletes rows where daily date = monthly date 
Delete from GF_PORTFOLIO_HOLDINGS where portfolio_date in (
	SELECT max(portfolio_date) FROM GF_PORTFOLIO_HOLDINGS  where PFCH_POR_CALC_SHORT <>  'DAILY' )
	and PFCH_POR_CALC_SHORT = 'DAILY';

--GF_PORTFOLIO_LTHOLDINGS
TRUNCATE TABLE GF_PORTFOLIO_LTHOLDINGS;
INSERT INTO GF_PORTFOLIO_LTHOLDINGS SELECT * FROM GFQ_PORTFOLIO_LTHOLDINGS;
--  Deletes rows where daily date = monthly date 
Delete from GF_PORTFOLIO_LTHOLDINGS where portfolio_date in (
	SELECT max(portfolio_date) FROM GF_PORTFOLIO_LTHOLDINGS  where PFCH_POR_CALC_SHORT <> 'DAILY' )
	and PFCH_POR_CALC_SHORT = 'DAILY';
