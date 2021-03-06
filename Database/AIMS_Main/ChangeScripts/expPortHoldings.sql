USE [AIMS_Main]
GO
/****** Object:  StoredProcedure [dbo].[expPortHoldings]    Script Date: 04/28/2014 13:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


ALTER procedure [dbo].[expPortHoldings]
as


declare @totDirtyValue decimal(22,8) 
declare @tmpDate varchar(22)

select @tmpDate = MAX(ph.PORTFOLIO_DATE) from GF_PORTFOLIO_LTHOLDINGS ph where ph.PORTFOLIO_ID = 'OSPREYEQ'
--select @tmpDate = '1/31/2014'

select PORTFOLIO_ID = case ph.PORTFOLIO_ID when 'SICVESC' then 'GSCF' else ph.PORTFOLIO_ID end
	,ph.TICKER
	,case ISNULL(sb.SEDOL,'TEST') when 'TEST' 
		then (select sbv.SEDOL from GF_SECURITY_BASEVIEW sbv where sbv.ISSUER_ID = ph.ISSUER_ID and sbv.SECURITY_ID = sbv.issuer_proxy) else sb.SEDOL 
		end as SEDOL
	,sb.CUSIP
	,sb.ISIN
	,ph.BALANCE_NOMINAL as Quantity
	,ph.PORTFOLIO_DATE as effective
INTO #TMP
from GF_PORTFOLIO_LTHOLDINGS ph
	left join GF_SECURITY_BASEVIEW sb on sb.ASEC_SEC_SHORT_NAME = ph.ASEC_SEC_SHORT_NAME
	--LEFT JOIN gf_benchmark_holdings bh on (ph.benchmark_id = bh.benchmark_id and bh.PORTFOLIO_DATE = ph.PORTFOLIO_DATE and ph.ASEC_SEC_SHORT_NAME = bh.ASEC_SEC_SHORT_NAME)
	--left join #A a on (a.portfolio_id = ph.PORTFOLIO_ID and a.portfolio_date = ph.PORTFOLIO_DATE)
where --ph.ASHEMM_PROP_REGION_CODE = 'AFRICA' AND
 ph.PORTFOLIO_DATE = @tmpDate
--and ph.PORTFOLIO_ID in ('ABPEQ','APG60','GSCF','CALPERSESC','PRIT','PASERSESC','BIRCH','IOWA','CURIANESC')
and ph.PORTFOLIO_ID in ('ABPEQ','APG60','CALPERSESC','PRIT','PASERSESC','BIRCH','IOWA','CURIANESC','SICVESC','ACTINVER','FJORD1','SAF','SICVFEF','OSPREYEQ','SICVEF','SICVMEF')
and ph.SECURITYTHEMECODE = 'EQUITY'
and ph.DIRTY_VALUE_PC != 0
ORDER BY ph.PORTFOLIO_ID

select PORTFOLIO_ID =  case ph.PORTFOLIO_ID when 'SICVESC' then 'GSCF' else ph.PORTFOLIO_ID end
	, 'USD CURNCY' AS TICKER
	, 'CASH_USD' AS SEDOL
	, 'CASH_USD' AS CUSIP
	, 'CASH_USD' AS ISIN
	,SUM(ph.DIRTY_VALUE_PC) as Quantity
	,ph.PORTFOLIO_DATE as effective
INTO #CASH
from GF_PORTFOLIO_LTHOLDINGS ph
	left join GF_SECURITY_BASEVIEW sb on sb.ASEC_SEC_SHORT_NAME = ph.ASEC_SEC_SHORT_NAME
	--LEFT JOIN gf_benchmark_holdings bh on (ph.benchmark_id = bh.benchmark_id and bh.PORTFOLIO_DATE = ph.PORTFOLIO_DATE and ph.ASEC_SEC_SHORT_NAME = bh.ASEC_SEC_SHORT_NAME)
	--left join #A a on (a.portfolio_id = ph.PORTFOLIO_ID and a.portfolio_date = ph.PORTFOLIO_DATE)
where --ph.ASHEMM_PROP_REGION_CODE = 'AFRICA' AND
 ph.PORTFOLIO_DATE = @tmpDate
--and ph.PORTFOLIO_ID in ('ABPEQ','APG60','GSCF','CALPERSESC','PRIT','PASERSESC','BIRCH','IOWA','CURIANESC')
and ph.PORTFOLIO_ID in ('ABPEQ','APG60','CALPERSESC','PRIT','PASERSESC','BIRCH','IOWA','CURIANESC','SICVESC','ACTINVER','FJORD1','SAF','SICVFEF','OSPREYEQ','SICVEF','SICVMEF')
and ph.SECURITYTHEMECODE = 'CASH'
GROUP BY ph.PORTFOLIO_ID, ph.PORTFOLIO_DATE

--select * from GF_PORTFOLIO_LTHOLDINGS ph
--where ph.PORTFOLIO_ID = 'CALPERSESC'
--and ph.PORTFOLIO_DATE = '3/4/2014'
--and ph.TICKER is null
--and ph.SECURITYTHEMECODE = 'EQUITY'
--AND ph.DIRTY_VALUE_PC != 0

INSERT INTO #TMP
SELECT * FROM #CASH

SELECT * FROM #TMP ORDER BY PORTFOLIO_ID

DROP TABLE #CASH
DROP TABLE #TMP