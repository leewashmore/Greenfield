use AIMS_Main
go

/****** Object:  StoredProcedure [dbo].[expICWatchList]    Script Date: 08/06/2014 15:48:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

alter procedure [dbo].expICWatchList
	as


select ic.* 
into #ic_most_recent
from IC_TABLE ic ,
	(select ic.ISSUER_ID, MAX(ic.date_presented)as date_presented from IC_TABLE ic group by ic.ISSUER_ID) a
where
	a.ISSUER_ID = ic.ISSUER_ID
	and a.date_presented = ic.DATE_PRESENTED

select * 
from
(select top 10 'SMALL CAP' as 'LGSM'
	, sb.ISSUE_NAME
	, sb.TICKER
	, sb.ASHMOREEMM_PRIMARY_ANALYST as 'Primary Analyst'
	, fv.UPSIDE as '%Upside'
	--, fv.FV_SELL
	--, fv.CURRENT_MEASURE_VALUE
	, pfs.AMOUNT as MktCap 
	--, im.MEASURE_VALUE
	--, imr.ic_sell
	--, case when imr.ic_measure = '236' then im.MEASURE_VALUE/imr.ic_sell  -1  else imr.ic_sell / im.MEASURE_VALUE -1  end as icUpside
	, imr.date_presented as 'Date Presented'
from #ic_most_recent imr
	left join GF_PORTFOLIO_LTHOLDINGS ph on (imr.issuer_id = ph.ISSUER_ID
		and ph.PORTFOLIO_ID = 'PASERSESC'
		and ph.PORTFOLIO_DATE = (select MAX(ph.PORTFOLIO_DATE) from GF_PORTFOLIO_LTHOLDINGS ph where ph.PORTFOLIO_ID = 'PASERSESC'))
	left join GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = imr.issuer_id and sb.issuer_proxy = sb.SECURITY_ID
	left join FAIR_VALUE fv on fv.SECURITY_ID = sb.SECURITY_ID
	left join IC_MONITORING im on (im.ISSUER_ID = imr.issuer_id 
		and im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im) 
		and im.DATA_SOURCE = 'Primary'
		)
	left join PERIOD_FINANCIALS_SECURITY pfs on (pfs.SECURITY_ID = sb.SECURITY_ID
		and pfs.DATA_ID = '185'
		and pfs.DATA_SOURCE = 'PRIMARY'
		and pfs.CURRENCY = 'USD'
		and pfs.PERIOD_TYPE = 'C'
		)
where imr.date_presented > GETDATE()-365
 and ph.GF_ID is null
 and pfs.AMOUNT < 2000
ORDER BY fv.UPSIDE desc
) a
union
select * from
(
select top 10 'LARGE CAP' as 'LGSM'
	,sb.ISSUE_NAME
	, sb.TICKER
	, sb.ASHMOREEMM_PRIMARY_ANALYST as 'Primary Analyst'
	, fv.UPSIDE as '%Upside'
	--, fv.FV_SELL
	--, fv.CURRENT_MEASURE_VALUE
	, pfs.AMOUNT as MktCap
	--, im.MEASURE_VALUE
	--, imr.ic_sell
	--, case when imr.ic_measure = '236' then im.MEASURE_VALUE/imr.ic_sell  -1  else imr.ic_sell / im.MEASURE_VALUE -1  end as icUpside
	, imr.date_presented as 'Date Presented'
from #ic_most_recent imr
	left join GF_PORTFOLIO_LTHOLDINGS ph on (imr.issuer_id = ph.ISSUER_ID
		and ph.PORTFOLIO_ID = 'OSPREYEQ'
		and ph.PORTFOLIO_DATE = (select MAX(ph.PORTFOLIO_DATE) from GF_PORTFOLIO_LTHOLDINGS ph where ph.PORTFOLIO_ID = 'OSPREYEQ'))
	left join GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = imr.issuer_id and sb.issuer_proxy = sb.SECURITY_ID
	left join FAIR_VALUE fv on fv.SECURITY_ID = sb.SECURITY_ID
	left join IC_MONITORING im on (im.ISSUER_ID = imr.issuer_id 
		and im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im) 
		and im.DATA_SOURCE = 'Primary'
		)
	left join PERIOD_FINANCIALS_SECURITY pfs on (pfs.SECURITY_ID = sb.SECURITY_ID
		and pfs.DATA_ID = '185'
		and pfs.DATA_SOURCE = 'PRIMARY'
		and pfs.CURRENCY = 'USD'
		and pfs.PERIOD_TYPE = 'C'
		)
where imr.date_presented > GETDATE()-365
 and ph.GF_ID is null
 and pfs.AMOUNT > 2000
ORDER BY fv.UPSIDE desc
) b
order by  LGSM, '%Upside' DESC

--select * from
--(
--	select top 10 PORPATH,sb.ISSUE_NAME, sb.TICKER, PCT_PORTFOLIO, fv.UPSIDE from #porPct pp
--		left join FAIR_VALUE fv on fv.SECURITY_ID = pp.issuer_proxy
--		left join GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pp.issuer_proxy
--	 order by fv.UPSIDE desc
--) a
--union
--select * from
--(
--	select top 25 PORPATH,sb.ISSUE_NAME, sb.TICKER,PCT_PORTFOLIO, fv.UPSIDE from #porPct pp
--		left join FAIR_VALUE fv on fv.SECURITY_ID = pp.issuer_proxy
--		left join GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pp.issuer_proxy
--	 order by fv.UPSIDE 
--) b
--order by UPSIDE desc


drop table #ic_most_recent