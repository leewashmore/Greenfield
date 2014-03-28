USE [AIMS_Main]
GO
/****** Object:  StoredProcedure [dbo].[expIdxHoldings]    Script Date: 03/28/2014 10:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO



ALTER procedure [dbo].[expIdxHoldings]
as

declare @tmpDate varchar(22)
select @tmpDate = MAX(ph.PORTFOLIO_DATE) from GF_PORTFOLIO_LTHOLDINGS ph where ph.PORTFOLIO_ID = 'ABPEQ'

select PORT_ID = 
		CASE bh.BENCHMARK_ID
			WHEN 'MSCI EM NET' THEN 'MSCI_NET' 
			WHEN 'MSCI EM SC NET' THEN 'MSCI_EMSML'
			WHEN 'MSCI EM IMI NET' THEN 'MSCI_EMIMI'
			WHEN 'MSCI FRONTIER NET' THEN 'FRONTIER'
			WHEN 'FTSE EM SC Custom' THEN 'FTSEEMSMCUST'
			ELSE bh.BENCHMARK_ID
		END
	, bh.TICKER
	, sb.SEDOL
	, sb.CUSIP
	, sb.ISIN
	, bh.BENCHMARK_WEIGHT
	, bh.PORTFOLIO_DATE as Effective
from GF_BENCHMARK_HOLDINGS bh
	LEFT JOIN GF_SECURITY_BASEVIEW sb on bh.ASEC_SEC_SHORT_NAME = sb.ASEC_SEC_SHORT_NAME
where bh.BENCHMARK_ID in ('MSCI EM NET','MSCI EM SC NET','MSCI EM IMI NET','MSCI FRONTIER NET','FTSE EM SC Custom')
and bh.PORTFOLIO_DATE = @tmpDate
order by bh.BENCHMARK_ID
