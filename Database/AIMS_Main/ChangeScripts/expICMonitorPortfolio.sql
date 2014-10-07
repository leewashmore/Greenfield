use AIMS_Main
go

/****** Object:  StoredProcedure [dbo].[expICMonitorPortfolio]    Script Date: 08/06/2014 15:48:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

alter procedure [dbo].expICMonitorPortfolio(
	@portID nvarchar(225) = ''
	)
	as

select ic.* 
into #ic_most_recent
from IC_TABLE ic ,
	(select ic.ISSUER_ID, MAX(ic.update_date)as update_date from IC_TABLE ic group by ic.ISSUER_ID) a
where
	a.ISSUER_ID = ic.ISSUER_ID
	and a.update_date = ic.update_date

--DROP TABLE #ic_most_recent
declare @totDirtyValue decimal(22,8) 

select @totDirtyValue = sum(ch.DIRTY_VALUE_PC) from GF_PORTFOLIO_HOLDINGS ch
	where ch.PORTFOLIO_ID = @portID
	and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_PORTFOLIO_HOLDINGS ch where ch.PORTFOLIO_ID = @portID)
	group by ch.PORTFOLIO_DATE

select ch.ISSUER_ID, sum(ch.DIRTY_VALUE_PC/@totDirtyValue)as PCT_PORTFOLIO, max(ch.PORTFOLIO_ID)as PORTFOLIO_ID, MAX(sb.issuer_proxy) as issuer_proxy
into #porPct
from GF_PORTFOLIO_HOLDINGS ch
	left join GF_SECURITY_BASEVIEW sb on sb.ASEC_SEC_SHORT_NAME = ch.ASEC_SEC_SHORT_NAME
where ch.PORTFOLIO_ID = @portID
and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_PORTFOLIO_HOLDINGS ch where ch.PORTFOLIO_ID = @portID)
and ch.SECURITYTHEMECODE = 'EQUITY'
GROUP BY CH.ISSUER_ID

select pp.issuer_id 
	, pp.PORTFOLIO_ID
	, sb.ASHMOREEMM_PRIMARY_ANALYST as 'Analyst'
	, sb.TICKER
	, sb.ISSUER_NAME as 'Co. Name'
	, imr.date_presented as 'Date Presented'
	, imr.ic_buy as 'IC Buy'
	, imr.ic_sell as 'IC Sell'
	, imr.ic_action as 'Original Action'
--	, imr.ic_measure
	, im.MEASURE_VALUE as 'BF Value'
	, im.CURR_ACTION as 'Current Action'
	, im.UPDATE_DATE 
	, im.DATA_SOURCE as 'Data Source'
	, isnull(pp.PCT_PORTFOLIO,0) as 'Port%'
	, fv.UPSIDE
	, fv.FV_BUY as 'FV Buy'
	, fv.FV_SELL as 'FV Sell'
--	, fv.CURRENT_MEASURE_VALUE
--	, fv.FV_MEASURE
from #porPct pp 
	left join #ic_most_recent imr on pp.ISSUER_ID = imr.issuer_id
	left join GF_SECURITY_BASEVIEW sb on pp.issuer_proxy = sb.SECURITY_ID
	left join IC_MONITORING im on pp.issuer_id = im.issuer_id 
		and im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im)
		and (im.DATA_SOURCE = 'PRIMARY' or im.DATA_SOURCE is null)
	left join FAIR_VALUE fv on fv.SECURITY_ID = pp.issuer_proxy 
ORDER BY isnull(imr.presenter,'znull'),fv.UPSIDE desc

--select * from #porPct pp order by pp.issuer_id
--select * from #ic_most_recent
drop table #ic_most_recent
drop table #porPct