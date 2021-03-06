USE [AIMS_Main]
GO
/****** Object:  StoredProcedure [dbo].[expVQGData]    Script Date: 06/19/2014 14:45:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

ALTER procedure [dbo].[expVQGData]
as

--Security
select pf.ROOT_SOURCE_DATE,sb.isin,sb.TICKER,pf.AMOUNT, pf.DATA_ID, pf.DATA_SOURCE, pf.SECURITY_ID
into #PE
from dbo.GF_SECURITY_BASEVIEW sb left join dbo.PERIOD_FINANCIALS pf on sb.SECURITY_ID = pf.SECURITY_ID
where pf.DATA_ID = 308
	and pf.PERIOD_TYPE = 'C'
	and pf.CURRENCY = 'USD'
	and pf.DATA_SOURCE = 'PRIMARY'
--	and pf.SECURITY_ID = '178235'

select pf.ROOT_SOURCE_DATE,sb.isin,sb.TICKER,pf.AMOUNT, pf.DATA_ID, pf.DATA_SOURCE, pf.SECURITY_ID
into #PB
from dbo.GF_SECURITY_BASEVIEW sb left join dbo.PERIOD_FINANCIALS pf on sb.SECURITY_ID = pf.SECURITY_ID
where pf.DATA_ID = 306
	and pf.PERIOD_TYPE = 'C'
	and pf.CURRENCY = 'USD'
	and pf.DATA_SOURCE = 'PRIMARY'
--	and pf.SECURITY_ID = '178235'

select pf.ROOT_SOURCE_DATE,sb.isin,sb.TICKER,pf.AMOUNT, pf.DATA_ID, pf.DATA_SOURCE, pf.SECURITY_ID
into #DY
from dbo.GF_SECURITY_BASEVIEW sb left join dbo.PERIOD_FINANCIALS pf on sb.SECURITY_ID = pf.SECURITY_ID
where pf.DATA_ID = 302
	and pf.PERIOD_TYPE = 'C'
	and pf.CURRENCY = 'USD'
	and pf.DATA_SOURCE = 'PRIMARY'
--	and pf.SECURITY_ID = '178235'

select pf.ISSUER_ID,avg(pf.AMOUNT) as AMOUNT
into #ROE
from dbo.GF_SECURITY_BASEVIEW sb left join dbo.PERIOD_FINANCIALS pf on sb.ISSUER_ID = pf.ISSUER_ID
where pf.DATA_ID = 309
	and pf.PERIOD_TYPE = 'C'
	and pf.CURRENCY = 'USD'
	and pf.DATA_SOURCE = 'PRIMARY'
	and pf.FISCAL_TYPE = 'FISCAL'
group by pf.ISSUER_ID

select 
	pe.root_source_date
	, sb.ISSUER_ID
	, sb.SECURITY_ID
	, sb.ISIN
	, sb.TICKER
	, pe.amount as aimsPE
	, pb.amount as aimsPB
	, dy.amount as aimsDY
	, roe.amount as aimsROE
	, fv.UPSIDE
	,sb.ASHMOREEMM_PRIMARY_ANALYST
into #tmp
from dbo.GF_SECURITY_BASEVIEW sb
	left join #PE pe on sb.SECURITY_ID = pe.security_id
	left join #PB pb on sb.SECURITY_ID = pb.security_id
	left join #DY dy on sb.SECURITY_ID = dy.security_id
	left join #ROE roe on sb.ISSUER_ID = roe.issuer_id
	left join FAIR_VALUE fv on sb.SECURITY_ID = fv.SECURITY_ID
order by sb.ISSUER_ID

--select distinct issuer_id from #roe

--select roe.amount, roe.issuer_id from #roe roe group by roe.issuer_id, roe.amount

select tmp.* from #tmp tmp

--select * from #tmp roe order by roe.issuer_id
	
drop table #PE
drop table #DY
drop table #PB
drop table #ROE
drop table #tmp
--Issuer

--select sb.REPORTNUMBER,* from GF_SECURITY_BASEVIEW sb where sb.ISIN = 'KR7000070003'
