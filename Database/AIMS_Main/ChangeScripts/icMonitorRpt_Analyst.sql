use AIMS_Main
go

/****** Object:  StoredProcedure [dbo].[expICMonitorAnalyst]    Script Date: 08/06/2014 15:48:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

alter procedure [dbo].expICMonitorAnalyst(
	@ANALYST nvarchar(225) = ''
	)
	as

declare @portID varchar(10) = 'EQYALL'

select ic.* 
into #ic_most_recent
from IC_TABLE ic ,
	(select ic.ISSUER_ID, MAX(ic.update_date)as update_date from IC_TABLE ic group by ic.ISSUER_ID) a
where
	a.ISSUER_ID = ic.ISSUER_ID
	and a.update_date = ic.update_date

--DROP TABLE #ic_most_recent
declare @totDirtyValue decimal(22,8) 

select @totDirtyValue = sum(ch.DIRTY_VALUE_PC) from GF_COMPOSITE_LTHOLDINGS ch
	where ch.PORTFOLIO_ID = @portID
	and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_COMPOSITE_LTHOLDINGS ch where ch.PORTFOLIO_ID = @portID)
	group by ch.PORTFOLIO_DATE

select ch.ISSUER_ID, sum(ch.DIRTY_VALUE_PC/@totDirtyValue)as PCT_PORTFOLIO, MAX(ch.PORPATH) AS PORPATH
into #porPct
from GF_COMPOSITE_LTHOLDINGS ch
where ch.PORTFOLIO_ID = @portID
and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_COMPOSITE_LTHOLDINGS ch where ch.PORTFOLIO_ID = @portID)
GROUP BY CH.ISSUER_ID

select issuer_id 
	, ISO_COUNTRY_CODE
	, presenter as 'Presenter'
	, ASHMOREEMM_PRIMARY_ANALYST as Analyst
	, TICKER
	, ISSUER_NAME as 'Company Name'
	, date_presented as 'Date Presented'
	, ic_buy as 'IC Buy'
	, ic_sell as 'IC Sell'
	, Original_Action as 'Original Action'
--	, imr.ic_measure
	, currValue as 'BF Value'
	, CURR_ACTION as 'Current Action'
	, UPDATE_DATE 
	, DATA_SOURCE as 'Data Source'
	, PCT_PORTFOLIO as '% Port'
	, PORPATH as 'Portfolio'
	, UPSIDE 
	, FV_BUY as 'FV Buy'
	, FV_SELL as 'FV Sell'
	, tmpGroup
--	, fv.CURRENT_MEASURE_VALUE
--	, fv.FV_MEASURE
from
(
select imr.issuer_id 
	, sb.ISO_COUNTRY_CODE
	, imr.presenter
	, sb.ASHMOREEMM_PRIMARY_ANALYST
	, sb.TICKER
	, sb.ISSUER_NAME
	, imr.date_presented
	, imr.ic_buy
	, imr.ic_sell
	, imr.ic_action as Original_Action
--	, imr.ic_measure
	, im.MEASURE_VALUE as currValue
	, im.CURR_ACTION
	, im.UPDATE_DATE
	, im.DATA_SOURCE
	, isnull(pp.PCT_PORTFOLIO,0) as PCT_PORTFOLIO
	, pp.PORPATH
	, fv.UPSIDE
	, fv.FV_BUY
	, fv.FV_SELL
	, 'A' as tmpGroup
--	, fv.CURRENT_MEASURE_VALUE
--	, fv.FV_MEASURE
from IC_MONITORING im
	left join #ic_most_recent imr on im.ISSUER_ID = imr.issuer_id
	left join GF_SECURITY_BASEVIEW sb on im.SECURITY_ID = sb.SECURITY_ID
	left join #porPct pp on pp.issuer_id = imr.issuer_id
	left join FAIR_VALUE fv on fv.SECURITY_ID = im.security_id 
where im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im)
and imr.presenter = @ANALYST 
and (im.DATA_SOURCE = 'PRIMARY' or im.DATA_SOURCE is null)
and pp.PCT_PORTFOLIO > 0

union

select imr.issuer_id 
	, sb.ISO_COUNTRY_CODE
	, imr.presenter
	, sb.ASHMOREEMM_PRIMARY_ANALYST
	, sb.TICKER
	, sb.ISSUER_NAME
	, imr.date_presented
	, imr.ic_buy
	, imr.ic_sell
	, imr.ic_action as Original_Action
--	, imr.ic_measure
	, im.MEASURE_VALUE as currValue
	, im.CURR_ACTION
	, im.UPDATE_DATE
	, im.DATA_SOURCE
	, isnull(pp.PCT_PORTFOLIO,0) as PCT_PORTFOLIO
	, pp.PORPATH
	, fv.UPSIDE
	, fv.FV_BUY
	, fv.FV_SELL
	, 'B' as tmpGroup
--	, fv.CURRENT_MEASURE_VALUE
--	, fv.FV_MEASURE
from IC_MONITORING im
	left join #ic_most_recent imr on im.ISSUER_ID = imr.issuer_id
	left join GF_SECURITY_BASEVIEW sb on im.SECURITY_ID = sb.SECURITY_ID
	left join #porPct pp on pp.issuer_id = imr.issuer_id
	left join FAIR_VALUE fv on fv.SECURITY_ID = im.security_id 
where im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im)
and imr.presenter = @ANALYST 
and (im.DATA_SOURCE = 'PRIMARY' or im.DATA_SOURCE is null)
and pp.PCT_PORTFOLIO IS NULL
and imr.date_presented > (GETDATE() - 366)
--ORDER BY pp.PCT_PORTFOLIO DESC, imr.presenter,sb.ISSUER_NAME DESC
union
select pp.issuer_id 
	, sb.ISO_COUNTRY_CODE
	, imr.presenter
	, sb.ASHMOREEMM_PRIMARY_ANALYST
	, sb.TICKER
	, sb.ISSUER_NAME
	, imr.date_presented
	, imr.ic_buy
	, imr.ic_sell
	, imr.ic_action as Original_Action
--	, imr.ic_measure
	, im.MEASURE_VALUE as currValue
	, im.CURR_ACTION
	, im.UPDATE_DATE
	, im.DATA_SOURCE
	, isnull(pp.PCT_PORTFOLIO,0) as PCT_PORTFOLIO
	, pp.PORPATH
	, fv.UPSIDE
	, fv.FV_BUY
	, fv.FV_SELL
	, 'C' as tmpGroup
--	, fv.CURRENT_MEASURE_VALUE
--	, fv.FV_MEASURE
from #porPct pp 
	left join #ic_most_recent imr on pp.ISSUER_ID = imr.issuer_id
	left join IC_MONITORING im on (pp.issuer_id = im.issuer_id 
		and im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im)
		and (im.DATA_SOURCE = 'PRIMARY' or im.DATA_SOURCE is null))
	left join GF_SECURITY_BASEVIEW sb on (pp.issuer_id = sb.ISSUER_ID
		and sb.issuer_proxy = sb.SECURITY_ID)
	left join FAIR_VALUE fv on fv.SECURITY_ID = sb.SECURITY_ID 
WHERE sb.ASHMOREEMM_PRIMARY_ANALYST = @ANALYST
	and (imr.presenter <> @ANALYST or imr.presenter is null)
--ORDER BY imr.presenter,pp.PCT_PORTFOLIO desc
) results
ORDER BY tmpGroup, PCT_PORTFOLIO DESC, presenter, ISSUER_NAME DESC

--SELECT * FROM #ic_most_recent imr
--	left join IC_MONITORING im on (imr.issuer_id = im.ISSUER_ID and im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im))
--WHERE presenter = 'MADERC'

	select * from #porPct pp,  #ic_most_recent imr
	where pp.issuer_id = '9868126'
	and pp.issuer_id = imr.issuer_id
	
drop table #ic_most_recent
drop table #porPct