use AIMS_Main
go

/****** Object:  StoredProcedure [dbo].[expUpsideMonitorPortfolio]    Script Date: 08/06/2014 15:48:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

alter procedure [dbo].expUpsideMonitorPortfolio(
	@portID nvarchar(225) = ''
	)
	as

declare @totDirtyValue decimal(22,8) 
declare @3M0 as datetime = getdate()-90
declare @1M0 as datetime = getdate()-30


select ic.* 
into #ic_most_recent
from IC_TABLE ic ,
	(select ic.ISSUER_ID, MAX(ic.update_date)as update_date from IC_TABLE ic group by ic.ISSUER_ID) a
where
	a.ISSUER_ID = ic.ISSUER_ID
	and a.update_date = ic.update_date

select @totDirtyValue = sum(ch.DIRTY_VALUE_PC) from GF_PORTFOLIO_LTHOLDINGS ch
	where ch.PORTFOLIO_ID = @portID
	and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_PORTFOLIO_LTHOLDINGS ch where ch.PORTFOLIO_ID = @portID)
	group by ch.PORTFOLIO_DATE

select ch.ISSUER_ID
	, sum(ch.DIRTY_VALUE_PC/@totDirtyValue)as PCT_PORTFOLIO
	, MAX(ch.PORPATH) AS PORPATH
	, MAX(sb.issuer_proxy) as issuer_proxy
	, ch.ISO_COUNTRY_CODE
into #porPct
from GF_PORTFOLIO_LTHOLDINGS ch
	left join GF_SECURITY_BASEVIEW sb on sb.ASEC_SEC_SHORT_NAME = ch.ASEC_SEC_SHORT_NAME
where ch.PORTFOLIO_ID = @portID
and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_PORTFOLIO_LTHOLDINGS ch where ch.PORTFOLIO_ID = @portID)
and ch.SECURITYTHEMECODE = 'EQUITY'
GROUP BY CH.ISSUER_ID, ch.ISO_COUNTRY_CODE

	select sb.XREF 
	into #xref
	from GF_PORTFOLIO_LTHOLDINGS ph
		left join GF_SECURITY_BASEVIEW sb on sb.ASEC_SEC_SHORT_NAME = ph.ASEC_SEC_SHORT_NAME
	where ph.PORTFOLIO_ID = @portID
	and ph.PORTFOLIO_DATE = (SELECT max(ph.PORTFOLIO_DATE) FROM GF_PORTFOLIO_LTHOLDINGS ph WHERE ph.PORTFOLIO_ID = @portID)
	and ph.SECURITYTHEMECODE = 'EQUITY'

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
	, (cf.median - a.median)/abs(a.median) as 'Chg1Mo'
	, (cf.median - b.median)/abs(b.median) as 'Chg3Mo'
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
--	, ped.ped
into #internalEst
from INTERNAL_COA_CHANGES icc
	left join #ped ped on ped.issuer_id = icc.ISSUER_ID
	left join FX_RATES fr on icc.CURRENCY = fr.CURRENCY and icc.PERIOD_END_DATE = fr.FX_DATE
	left join #intStartDate isd on isd.issuer_id = icc.ISSUER_ID 
where  icc.COA = 'NINC'
	AND icc.END_DATE IS NULL
	and ped.ped = icc.PERIOD_END_DATE
	and isd.startdate = icc.START_DATE
order by  startDate, icc.PERIOD_END_DATE

select sb.ISSUER_ID
--	,  sb.XREF
	, PORPATH
	, sb.ASHMOREEMM_PRIMARY_ANALYST as Analyst
	, sb.ISSUE_NAME as CoName
	, pp.ISO_COUNTRY_CODE as 'Cty Code'
	, sb.TICKER
	, pp.PCT_PORTFOLIO as '% Port'
	, imr.date_presented as 'Date Presented'
	--, fv.FV_BUY/iserr(fv.CURRENT_MEASURE_VALUE,-9) - 1 as Downside
	, fv.CURRENT_MEASURE_VALUE as BFValue
--	, fv.FV_BUY as Buy
--	, fv.FV_SELL as Sell
	, imr.ic_buy as Buy
	, imr.ic_sell as Sell
	, case when imr.ic_measure = '236' then im.MEASURE_VALUE/imr.ic_sell - 1 else
		imr.ic_sell/im.MEASURE_VALUE - 1 end as 'IC Upside'
--	, fv.UPSIDE as 'Analyst Upside'
	, isnull(vqg.HIST_UPSIDE, vqg.VQG_UPSIDE) as 'VQG Upside'
	, ce.Chg1Mo
	, ce.Chg3Mo
--	, ce.currest
	, ie.startdate as 'Last Model Upload'
	, (ie.amount - ce.currest)/ABS(ce.currest) as '%Diff Internal v Cons'
from #porPct pp
	left join FAIR_VALUE fv on fv.SECURITY_ID = pp.issuer_proxy
	left join GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pp.issuer_proxy
	left join  #ic_most_recent imr on imr.issuer_id = pp.issuer_id
	left join #consEst ce on ce.xref = sb.XREF
	left join #internalEst ie on ie.issuer_id = pp.issuer_id
	left join VQG vqg on vqg.SECURITY_ID = sb.SECURITY_ID and vqg.DATE = (select max(date) from vqg)
	left join IC_MONITORING im on (sb.ISSUER_ID = im.ISSUER_ID and im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im) and im.DATA_SOURCE = 'PRIMARY')
order by 'IC Upside' desc

--select a.*, b.*, sb.XREF from #porPct a
--	left join GF_SECURITY_BASEVIEW sb on sb.issuer_proxy = a.issuer_proxy and sb.issuer_proxy = sb.SECURITY_ID
--	left join #1mo b on b.xref = sb.XREF
--ORDER BY a.issuer_id	

--WHERE a.xref = '300184712'


drop table #ic_most_recent
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

