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
declare @3M0 as datetime = getdate()-90
declare @1M0 as datetime = getdate()-30


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
	, CURR_ACTION as 'Analyst Tracking Parameter'
	, UPDATE_DATE as 'Update Date'
	, DATA_SOURCE as 'Data Source'
	, PCT_PORTFOLIO as '% Port'
	, PORPATH as 'Portfolio'
	, VQG_UPSIDE as 'VQG Upside'
	, Upside AS 'IC Upside'
--	, test
	, FV_BUY as 'FV Buy'
	, FV_SELL as 'FV Sell'
	, tmpGroup
--	, fv.CURRENT_MEASURE_VALUE
--	, fv.FV_MEASURE
INTO #analystTmp
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
	, isnull(vqg.HIST_UPSIDE, vqg.VQG_UPSIDE) as VQG_UPSIDE
	, case when imr.ic_measure = '236' then im.MEASURE_VALUE/imr.ic_sell - 1 else
		imr.ic_sell/im.MEASURE_VALUE - 1 end as 'Upside'
--	, fv.UPSIDE as 'Test'
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
	left join VQG vqg on vqg.SECURITY_ID = im.SECURITY_ID and vqg.DATE = (select max(date) from vqg)
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
	, isnull(vqg.HIST_UPSIDE, vqg.VQG_UPSIDE) as VQG_UPSIDE
	, case when imr.ic_measure = '236' then im.MEASURE_VALUE/imr.ic_sell - 1 else
		imr.ic_sell/im.MEASURE_VALUE - 1 end as 'Upside'
--	, fv.UPSIDE as 'Test'
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
	left join VQG vqg on vqg.SECURITY_ID = im.SECURITY_ID and vqg.DATE = (select max(date) from vqg)
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
	, isnull(vqg.HIST_UPSIDE,vqg.VQG_UPSIDE) as VQG_UPSIDE
	, case when imr.ic_measure = '236' then im.MEASURE_VALUE/imr.ic_sell - 1 else
		imr.ic_sell/im.MEASURE_VALUE - 1 end as 'Upside'
--	, fv.UPSIDE as 'Test'
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
	left join VQG vqg on vqg.SECURITY_ID = sb.SECURITY_ID and vqg.DATE = (select max(date) from vqg)
WHERE sb.ASHMOREEMM_PRIMARY_ANALYST = @ANALYST
	and (imr.presenter <> @ANALYST or imr.presenter is null)
--ORDER BY imr.presenter,pp.PCT_PORTFOLIO desc
) results
ORDER BY tmpGroup, 'IC Upside' DESC, presenter, ISSUER_NAME DESC

	select sb.XREF 
	into #xref
	from #analystTmp at
		left join GF_SECURITY_BASEVIEW sb on sb.TICKER = at.TICKER

	select pfm.ISSUER_ID, sb.XREF,MIN(pfm.PERIOD_END_DATE) as PED
	into #PED
	from AIMS_Main..PERIOD_FINANCIALS_ISSUER pfm
		left join AIMS_Main..GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = pfm.ISSUER_ID
	where pfm.PERIOD_END_DATE > GETDATE()
	and pfm.DATA_ID = '290'
	and pfm.CURRENCY = 'USD'
	and pfm.FISCAL_TYPE = 'FISCAL'
	and pfm.DATA_SOURCE = 'PRIMARY'
	and pfm.PERIOD_TYPE = 'A'
	GROUP BY pfm.ISSUER_ID, sb.XREF
	ORDER BY pfm.ISSUER_ID

	select CE.ORIGINALDATE as originalDate, ped.PED,a.XRef 
	into #Curr
	from #xref a
		left join #ped ped on ped.xref = a.XREF
		left join AIMS_Reuters..tblConsensusEstimate ce on 
			(
			ce.XRef = a.XREF
			and ce.ExpirationDate is null
			and ce.PeriodEndDate = ped.PED
			and ce.PeriodType = 'A'
			and ce.EstimateType = 'NTP'
			)
	group by a.XRef, ped.PED, ce.OriginalDate
	
	select MAX(CE.ORIGINALDATE) as originalDate, ped.PED,a.XRef 
	into #3mo
	from #xref a
		left join #ped ped on ped.xref = a.XREF
		left join AIMS_Reuters..tblConsensusEstimate ce on 
			(
			ce.XRef = a.XREF
			and ce.OriginalDate <= @3M0
			and ce.PeriodEndDate = ped.PED
			and ce.PeriodType = 'A'
			and ce.EstimateType = 'NTP'
			)
	group by a.XRef, ped.PED

	select MAX(CE.ORIGINALDATE) as originalDate, ped.PED,a.XRef 
	into #1mo
	from #xref a
		left join #ped ped on ped.xref = a.XREF
		left join AIMS_Reuters..tblConsensusEstimate ce on 
			(
			ce.XRef = a.XREF
			and ce.OriginalDate <= @1M0
			and ce.PeriodEndDate = ped.PED
			and ce.PeriodType = 'A'
			and ce.EstimateType = 'NTP'
			)
	group by a.XRef, ped.PED

select tmp.XRef
	,ce.OriginalDate
	,CE.PeriodEndDate
	, ce.Mean/fr.AVG12MonthRATE *(
		case when ce.Unit = 'B' then 1000
			when ce.unit = 'M' then 1
			when ce.unit = 'T' then .001
			when ce.unit = 'U' then 1
			end
			) as Mean
	, ce.Median/fr.AVG12MonthRATE *(
		case when ce.Unit = 'B' then 1000
			when ce.unit = 'M' then 1
			when ce.unit = 'T' then .001
			when ce.unit = 'U' then 1
			end
			)as Median
INTO #currFinal
from #curr tmp
	left join AIMS_Reuters..tblConsensusEstimate ce on 
		(
		ce.XRef = tmp.xref
		and ce.OriginalDate = tmp.originalDate
		and ce.PeriodEndDate = tmp.PED
		and ce.PeriodType = 'A'
		and ce.EstimateType = 'NTP'
		)
	left join FX_RATES fr on ce.Currency = fr.CURRENCY and ce.PeriodEndDate = fr.FX_DATE


select tmp.XRef
	, ce.OriginalDate
	, CE.PeriodEndDate
	, ce.Mean/fr.AVG12MonthRATE * (
		case when ce.Unit = 'B' then 1000
			when ce.unit = 'M' then 1
			when ce.unit = 'T' then .001
			when ce.unit = 'U' then 1
			end
			)as Mean
	, ce.Median/fr.AVG12MonthRATE *(
		case when ce.Unit = 'B' then 1000
			when ce.unit = 'M' then 1
			when ce.unit = 'T' then .001
			when ce.unit = 'U' then 1
			end
			)as Median
INTO #3moFinal
from #3mo tmp
	left join AIMS_Reuters..tblConsensusEstimate ce on 
		(
		ce.XRef = tmp.xref
		and ce.OriginalDate = tmp.originalDate
		and ce.PeriodEndDate = tmp.PED
		and ce.PeriodType = 'A'
		and ce.EstimateType = 'NTP'
		)
	left join FX_RATES fr on ce.Currency = fr.CURRENCY and ce.PeriodEndDate = fr.FX_DATE

select tmp.XRef
	, ce.OriginalDate
	, CE.PeriodEndDate
	, ce.Mean/fr.AVG12MonthRATE *(
		case when ce.Unit = 'B' then 1000
			when ce.unit = 'M' then 1
			when ce.unit = 'T' then .001
			when ce.unit = 'U' then 1
			end
			) as Mean
	, ce.Median/fr.AVG12MonthRATE *(
		case when ce.Unit = 'B' then 1000
			when ce.unit = 'M' then 1
			when ce.unit = 'T' then .001
			when ce.unit = 'U' then 1
			end
			) as Median 
INTO #1moFinal
from #1mo tmp
	left join AIMS_Reuters..tblConsensusEstimate ce on 
		(
		ce.XRef = tmp.xref
		and ce.OriginalDate = tmp.originalDate
		and ce.PeriodEndDate = tmp.PED
		and ce.PeriodType = 'A'
		and ce.EstimateType = 'NTP'
		)
	left join FX_RATES fr on ce.Currency = fr.CURRENCY and ce.PeriodEndDate = fr.FX_DATE

select cf.xref
	, cf.PeriodEndDate
	, cf.median as 'CurrEst'
	, a.median as '1moEst'
	, b.median as '3moEst'
	, (cf.median - a.median)/ABS(a.median) as 'Chg1Mo'
	, (cf.median - b.median)/ABS(b.median) as 'Chg3Mo'
into #consEst
from #Currfinal cf, #1moFinal a, #3moFinal b
where cf.xref = a.xref
and cf.xref = b.xref

select icc.ISSUER_ID, MAX(icc.START_DATE) AS startDate
into #intStartDate
from INTERNAL_COA_CHANGES icc
	left join #ped ped on ped.issuer_id = icc.ISSUER_ID
where icc.COA = 'NINC'
AND ped.ped = icc.PERIOD_END_DATE
and icc.END_DATE is null
and icc.ROOT_SOURCE = 'PRIMARY'
group by icc.ISSUER_ID, icc.ROOT_SOURCE

select icc.ISSUER_ID
	, icc.COA
	, icc.START_DATE AS startDate
--	,case when icc.END_DATE > (icc.START_DATE + 120) then (icc.START_DATE + 120) else case when icc.END_DATE is null then icc.START_DATE + 120 else icc.END_DATE end end as END_DATE
	, icc.END_DATE
	, icc.PERIOD_END_DATE
	, icc.CURRENCY
	, icc.UNITS 
	, icc.AMOUNT/fr.AVG12MonthRATE as Amount
	, intSt.ROOT_SOURCE_DATE
--	, ped.ped
into #internalEst
from INTERNAL_COA_CHANGES icc
	left join #ped ped on ped.issuer_id = icc.ISSUER_ID
	left join FX_RATES fr on icc.CURRENCY = fr.CURRENCY and icc.PERIOD_END_DATE = fr.FX_DATE
	left join #intStartDate isd on isd.issuer_id = icc.ISSUER_ID 
	left join INTERNAL_STATEMENT intSt on (intSt.ISSUER_ID = icc.ISSUER_ID
			and intSt.PERIOD_END_DATE = ped.ped
			and intSt.ROOT_SOURCE = 'PRIMARY'
			)
where  icc.COA = 'NINC'
	AND icc.END_DATE IS NULL
	and ped.ped = icc.PERIOD_END_DATE
	and isd.startdate = icc.START_DATE
order by  startDate, icc.PERIOD_END_DATE

select at.*
	, ce.Chg1Mo
	, ce.Chg3Mo
	, (ie.amount - ce.currest)/ABS(ce.currest) as '%Diff Internal v Cons'
--	, ie.startdate as 'Last Model Upload'
	, ie.root_source_date as 'Last Model Upload'
from #analystTmp at
	left join GF_SECURITY_BASEVIEW sb on sb.TICKER = at.ticker and sb.issuer_proxy = sb.SECURITY_ID
	left join #consEst ce on ce.xref = sb.XREF and sb.issuer_proxy = sb.SECURITY_ID
	left join #internalEst ie on ie.issuer_id = at.issuer_id
order by at.tmpGroup, 'IC Upside' DESC

drop table #ic_most_recent
drop table #analystTmp
drop table #porPct
drop table #3mo
drop table #1mo
drop table #3moFinal
drop table #1moFinal
drop table #PED
drop table #xref
drop table #curr
drop table #currfinal
drop table #consEst
drop table #internalEst
drop table #intStartDate