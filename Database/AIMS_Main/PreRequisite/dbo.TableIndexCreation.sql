IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CURRENT_CONSENSUS_ESTIMATES]') AND name = N'CURRENT_CONSENSUS_ESTIMATES_IDX1')
DROP INDEX [CURRENT_CONSENSUS_ESTIMATES_IDX1] ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [CURRENT_CONSENSUS_ESTIMATES_IDX1]
ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] ([ISSUER_ID],[PERIOD_TYPE],[AMOUNT_TYPE])
INCLUDE ([PERIOD_YEAR],[FISCAL_TYPE])
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CURRENT_CONSENSUS_ESTIMATES]') AND name = N'CURRENT_CONSENSUS_ESTIMATES_IDX2')
DROP INDEX [CURRENT_CONSENSUS_ESTIMATES_IDX2] ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [CURRENT_CONSENSUS_ESTIMATES_IDX2]
ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] ([ISSUER_ID],[PERIOD_TYPE],[ESTIMATE_ID])
INCLUDE ([SECURITY_ID],[DATA_SOURCE],[DATA_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[CURRENCY],[AMOUNT],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CURRENT_CONSENSUS_ESTIMATES]') AND name = N'CURRENT_CONSENSUS_ESTIMATES_IDX3')
DROP INDEX [CURRENT_CONSENSUS_ESTIMATES_IDX3] ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [CURRENT_CONSENSUS_ESTIMATES_IDX3]
ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] ([SECURITY_ID])
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CURRENT_CONSENSUS_ESTIMATES]') AND name = N'CURRENT_CONSENSUS_ESTIMATES_IDX4')
DROP INDEX [CURRENT_CONSENSUS_ESTIMATES_IDX4] ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [CURRENT_CONSENSUS_ESTIMATES_IDX4]
ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] ([ISSUER_ID],[PERIOD_TYPE])
INCLUDE ([PERIOD_YEAR],[FISCAL_TYPE],[AMOUNT_TYPE])
GO

/****** Object:  Index [PERIOD_FINANCIALS_ISSUER_MAIN_IDX1]    Script Date: 04/15/2014 14:05:10 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_ISSUER_MAIN]') AND name = N'PERIOD_FINANCIALS_ISSUER_MAIN_IDX1')
DROP INDEX [PERIOD_FINANCIALS_ISSUER_MAIN_IDX1] ON [dbo].[PERIOD_FINANCIALS_ISSUER_MAIN] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_ISSUER_MAIN_IDX1]
ON [dbo].[PERIOD_FINANCIALS_ISSUER_MAIN] ([ISSUER_ID],[DATA_SOURCE],[PERIOD_TYPE],[FISCAL_TYPE],[AMOUNT_TYPE])
INCLUDE ([SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[CURRENCY],[DATA_ID],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY])
GO

/****** Object:  Index [PERIOD_FINANCIALS_ISSUER_MAIN_IDX2]    Script Date: 04/15/2014 14:05:43 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_ISSUER_MAIN]') AND name = N'PERIOD_FINANCIALS_ISSUER_MAIN_IDX2')
DROP INDEX [PERIOD_FINANCIALS_ISSUER_MAIN_IDX2] ON [dbo].[PERIOD_FINANCIALS_ISSUER_MAIN] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_ISSUER_MAIN_IDX2]
ON [dbo].[PERIOD_FINANCIALS_ISSUER_MAIN] ([DATA_SOURCE],[PERIOD_TYPE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO

/****** Object:  Index [PERIOD_FINANCIALS_ISSUER_MAIN_IDX3]    Script Date: 04/15/2014 14:06:27 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_ISSUER_MAIN]') AND name = N'PERIOD_FINANCIALS_ISSUER_MAIN_IDX3')
DROP INDEX [PERIOD_FINANCIALS_ISSUER_MAIN_IDX3] ON [dbo].[PERIOD_FINANCIALS_ISSUER_MAIN] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_ISSUER_MAIN_IDX3]
ON [dbo].[PERIOD_FINANCIALS_ISSUER_MAIN] ([DATA_SOURCE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_TYPE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_ISSUER_STAGE]') AND name = N'PERIOD_FINANCIALS_ISSUER_STAGE_IDX1')
DROP INDEX [PERIOD_FINANCIALS_ISSUER_STAGE_IDX1] ON [dbo].[PERIOD_FINANCIALS_ISSUER_STAGE] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_ISSUER_STAGE_IDX1]
ON [dbo].[PERIOD_FINANCIALS_ISSUER_STAGE] ([ISSUER_ID],[DATA_SOURCE],[PERIOD_TYPE],[FISCAL_TYPE],[AMOUNT_TYPE])
INCLUDE ([SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[CURRENCY],[DATA_ID],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY])
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_ISSUER_STAGE]') AND name = N'PERIOD_FINANCIALS_ISSUER_STAGE_IDX2')
DROP INDEX [PERIOD_FINANCIALS_ISSUER_STAGE_IDX2] ON [dbo].[PERIOD_FINANCIALS_ISSUER_STAGE] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_ISSUER_STAGE_IDX2]
ON [dbo].[PERIOD_FINANCIALS_ISSUER_STAGE] ([DATA_SOURCE],[PERIOD_TYPE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_ISSUER_STAGE]') AND name = N'PERIOD_FINANCIALS_ISSUER_STAGE_IDX3')
DROP INDEX [PERIOD_FINANCIALS_ISSUER_STAGE_IDX3] ON [dbo].[PERIOD_FINANCIALS_ISSUER_STAGE] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_ISSUER_STAGE_IDX3]
ON [dbo].[PERIOD_FINANCIALS_ISSUER_STAGE] ([DATA_SOURCE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_TYPE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_MAIN]') AND name = N'PERIOD_FINANCIALS_SECURITY_MAIN_IDX1')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX1] ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX1]
ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] ([SECURITY_ID])

GO

/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_MAIN_IDX2]    Script Date: 04/15/2014 14:12:32 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_MAIN]') AND name = N'PERIOD_FINANCIALS_SECURITY_MAIN_IDX2')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX2] ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] WITH ( ONLINE = OFF )
GO



CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX2]
ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] ([SECURITY_ID],[DATA_SOURCE],[PERIOD_TYPE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])

GO



/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_MAIN_IDX3]    Script Date: 04/15/2014 14:13:07 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_MAIN]') AND name = N'PERIOD_FINANCIALS_SECURITY_MAIN_IDX3')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX3] ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX3] ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] 
(
	[DATA_SOURCE] ASC,
	[PERIOD_TYPE] ASC,
	[PERIOD_YEAR] ASC,
	[FISCAL_TYPE] ASC,
	[CURRENCY] ASC,
	[DATA_ID] ASC
)
INCLUDE ( [ISSUER_ID],
[SECURITY_ID],
[COA_TYPE],
[ROOT_SOURCE],
[ROOT_SOURCE_DATE],
[PERIOD_END_DATE],
[AMOUNT],
[CALCULATION_DIAGRAM],
[SOURCE_CURRENCY],
[AMOUNT_TYPE]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO


/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_MAIN_IDX4]    Script Date: 04/15/2014 14:13:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_MAIN]') AND name = N'PERIOD_FINANCIALS_SECURITY_MAIN_IDX4')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX4] ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX4]
ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] ([DATA_SOURCE],[PERIOD_TYPE],[FISCAL_TYPE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO


/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_MAIN_IDX5]    Script Date: 04/15/2014 14:14:00 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_MAIN]') AND name = N'PERIOD_FINANCIALS_SECURITY_MAIN_IDX5')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX5] ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_MAIN_IDX5]
ON [dbo].[PERIOD_FINANCIALS_SECURITY_MAIN] ([DATA_SOURCE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_TYPE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO


/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_STAGE_IDX1]    Script Date: 04/15/2014 14:11:40 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_STAGE]') AND name = N'PERIOD_FINANCIALS_SECURITY_STAGE_IDX1')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX1] ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX1]
ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] ([SECURITY_ID])
GO

/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_STAGE_IDX2]    Script Date: 04/15/2014 14:12:32 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_STAGE]') AND name = N'PERIOD_FINANCIALS_SECURITY_STAGE_IDX2')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX2] ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] WITH ( ONLINE = OFF )
GO



CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX2]
ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] ([SECURITY_ID],[DATA_SOURCE],[PERIOD_TYPE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO



/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_STAGE_IDX3]    Script Date: 04/15/2014 14:13:07 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_STAGE]') AND name = N'PERIOD_FINANCIALS_SECURITY_STAGE_IDX3')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX3] ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX3] ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] 
(
	[DATA_SOURCE] ASC,
	[PERIOD_TYPE] ASC,
	[PERIOD_YEAR] ASC,
	[FISCAL_TYPE] ASC,
	[CURRENCY] ASC,
	[DATA_ID] ASC
)
INCLUDE ( [ISSUER_ID],
[SECURITY_ID],
[COA_TYPE],
[ROOT_SOURCE],
[ROOT_SOURCE_DATE],
[PERIOD_END_DATE],
[AMOUNT],
[CALCULATION_DIAGRAM],
[SOURCE_CURRENCY],
[AMOUNT_TYPE]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO


/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_STAGE_IDX4]    Script Date: 04/15/2014 14:13:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_STAGE]') AND name = N'PERIOD_FINANCIALS_SECURITY_STAGE_IDX4')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX4] ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX4]
ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] ([DATA_SOURCE],[PERIOD_TYPE],[FISCAL_TYPE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_YEAR],[PERIOD_END_DATE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO


/****** Object:  Index [PERIOD_FINANCIALS_SECURITY_STAGE_IDX5]    Script Date: 04/15/2014 14:14:00 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_SECURITY_STAGE]') AND name = N'PERIOD_FINANCIALS_SECURITY_STAGE_IDX5')
DROP INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX5] ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [PERIOD_FINANCIALS_SECURITY_STAGE_IDX5]
ON [dbo].[PERIOD_FINANCIALS_SECURITY_STAGE] ([DATA_SOURCE],[CURRENCY],[DATA_ID])
INCLUDE ([ISSUER_ID],[SECURITY_ID],[COA_TYPE],[ROOT_SOURCE],[ROOT_SOURCE_DATE],[PERIOD_TYPE],[PERIOD_YEAR],[PERIOD_END_DATE],[FISCAL_TYPE],[AMOUNT],[CALCULATION_DIAGRAM],[SOURCE_CURRENCY],[AMOUNT_TYPE])
GO




/****** Object:  Index [GF_PORTFOLIO_HOLDINGS_IDX2]    Script Date: 04/15/2014 14:01:24 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GF_PORTFOLIO_HOLDINGS]') AND name = N'GF_PORTFOLIO_HOLDINGS_IDX2')
DROP INDEX [GF_PORTFOLIO_HOLDINGS_IDX2] ON [dbo].[GF_PORTFOLIO_HOLDINGS] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [GF_PORTFOLIO_HOLDINGS_IDX2]
ON [dbo].[GF_PORTFOLIO_HOLDINGS] ([PORTFOLIO_DATE],[PORTFOLIO_ID])
INCLUDE ([GF_ID],[PFCH_POR_CALC_SHORT],[PORTFOLIO_THEME_SUBGROUP_CODE],[PORTFOLIO_CURRENCY],[BENCHMARK_ID],[ISSUER_ID],[ASEC_SEC_SHORT_NAME],[ISSUE_NAME],[TICKER],[SECURITYTHEMECODE],[A_SEC_INSTR_TYPE],[SECURITY_TYPE],[BALANCE_NOMINAL],[DIRTY_PRICE],[TRADING_CURRENCY],[DIRTY_VALUE_PC],[BENCHMARK_WEIGHT],[ASH_EMM_MODEL_WEIGHT],[MARKET_CAP_IN_USD],[ASHEMM_PROP_REGION_CODE],[ASHEMM_PROP_REGION_NAME],[ISO_COUNTRY_CODE],[COUNTRYNAME],[GICS_SECTOR],[GICS_SECTOR_NAME],[GICS_INDUSTRY],[GICS_INDUSTRY_NAME],[GICS_SUB_INDUSTRY],[GICS_SUB_INDUSTRY_NAME],[LOOK_THRU_FUND],[BARRA_RISK_FACTOR_MOMENTUM],[BARRA_RISK_FACTOR_VOLATILITY],[BARRA_RISK_FACTOR_VALUE],[BARRA_RISK_FACTOR_SIZE],[BARRA_RISK_FACTOR_SIZE_NONLIN],[BARRA_RISK_FACTOR_GROWTH],[BARRA_RISK_FACTOR_LIQUIDITY],[BARRA_RISK_FACTOR_LEVERAGE],[BARRA_RISK_FACTOR_PBETEWLD])

GO

/****** Object:  Index [GF_BENCHMARK_HOLDINGS_IDX2]    Script Date: 04/15/2014 13:47:58 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GF_BENCHMARK_HOLDINGS]') AND name = N'GF_BENCHMARK_HOLDINGS_IDX2')
DROP INDEX [GF_BENCHMARK_HOLDINGS_IDX2] ON [dbo].[GF_BENCHMARK_HOLDINGS] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [GF_BENCHMARK_HOLDINGS_IDX2]
ON [dbo].[GF_BENCHMARK_HOLDINGS] ([PORTFOLIO_DATE],[BENCHMARK_ID])
INCLUDE ([GF_ID],[PORTFOLIO_ID],[PORTFOLIO_THEME_SUBGROUP_CODE],[PORTFOLIO_CURRENCY],[ISSUER_ID],[ASEC_SEC_SHORT_NAME],[ISSUE_NAME],[TICKER],[SECURITYTHEMECODE],[A_SEC_INSTR_TYPE],[SECURITY_TYPE],[BALANCE_NOMINAL],[DIRTY_PRICE],[TRADING_CURRENCY],[DIRTY_VALUE_PC],[BENCHMARK_WEIGHT],[ASH_EMM_MODEL_WEIGHT],[MARKET_CAP_IN_USD],[ASHEMM_PROP_REGION_CODE],[ASHEMM_PROP_REGION_NAME],[ISO_COUNTRY_CODE],[COUNTRYNAME],[GICS_SECTOR],[GICS_SECTOR_NAME],[GICS_INDUSTRY],[GICS_INDUSTRY_NAME],[GICS_SUB_INDUSTRY],[GICS_SUB_INDUSTRY_NAME],[LOOK_THRU_FUND],[BARRA_RISK_FACTOR_MOMENTUM],[BARRA_RISK_FACTOR_VOLATILITY],[BARRA_RISK_FACTOR_VALUE],[BARRA_RISK_FACTOR_SIZE],[BARRA_RISK_FACTOR_SIZE_NONLIN],[BARRA_RISK_FACTOR_GROWTH],[BARRA_RISK_FACTOR_LIQUIDITY],[BARRA_RISK_FACTOR_LEVERAGE],[BARRA_RISK_FACTOR_PBETEWLD])
GO


/****** Object:  Index [GF_PERF_DAILY_ATTRIBUTION_IDX2]    Script Date: 04/15/2014 13:55:04 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GF_PERF_DAILY_ATTRIBUTION]') AND name = N'GF_PERF_DAILY_ATTRIBUTION_IDX2')
DROP INDEX [GF_PERF_DAILY_ATTRIBUTION_IDX2] ON [dbo].[GF_PERF_DAILY_ATTRIBUTION] WITH ( ONLINE = OFF )
GO



CREATE NONCLUSTERED INDEX [GF_PERF_DAILY_ATTRIBUTION_IDX2] ON [dbo].[GF_PERF_DAILY_ATTRIBUTION] 
(
	[NODE_NAME] ASC,
	[PORTFOLIO] ASC
)
INCLUDE ( [TO_DATE]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
