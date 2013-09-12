IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expScreeningFairValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expScreeningFairValue]
GO

/****** Object:  StoredProcedure [dbo].[expScreeningFairValue]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expScreeningFairValue]
as

--Figure out latest price date
declare @PRICEDATE datetime

select @PRICEDATE = MAX(p.PRICE_DATE)
from dbo.PRICES p


--extract full universe
select gsb.ISSUER_ID, 
	gsb.SECURITY_ID, 
	gsb.ASEC_SEC_SHORT_NAME as SHORT_NAME,
	gsb.ISSUE_NAME,
	gsb.SECURITY_TYPE,
	gsb.ISIN,
	gsb.TICKER,
	gsb.ASHMOREEMM_PRIMARY_ANALYST as PRIMARY_ANALYST,
	gsb.ASHEMM_PROPRIETARY_REGION_CODE as REGION,
	gsb.asec_sec_country_name as COUNTRY,
	gsb.GICS_SECTOR_NAME as SECTOR,
	gsb.GICS_INDUSTRY_NAME as INDUSTRY,
	gsb.GICS_SUB_INDUSTRY_NAME as SUB_INDUSTRY,
	gsb.TRADING_CURRENCY,
	gsb.SHARES_PER_ADR
into #S1	
from dbo.GF_SECURITY_BASEVIEW gsb
where gsb.SECURITY_TYPE not in ('BASKET EQ','FUND LP','FUND OEIC','PRV EQUITY')
and gsb.ISSUER_ID is not null
order by gsb.SECURITY_TYPE

select issuer_id
into #Issuers
from #S1
group by issuer_id


--Select BGA weight

declare @PortDate datetime
declare @PortValue decimal(22,8)

select @PortDate = MAX(gcl.PORTFOLIO_DATE)
from dbo.GF_COMPOSITE_LTHOLDINGS gcl
where gcl.PORTFOLIO_ID = 'EQYBGA'

select @PortValue = SUM(gcl.DIRTY_VALUE_PC)
from dbo.GF_COMPOSITE_LTHOLDINGS gcl
where gcl.PORTFOLIO_DATE = @PortDate and gcl.PORTFOLIO_ID = 'EQYBGA'

select gcl.ASEC_SEC_SHORT_NAME as SHORT_NAME, gcl.DIRTY_VALUE_PC as MKT_VALUE, gcl.DIRTY_VALUE_PC/@PortValue as BGA_WEIGHT 
into #S3
from dbo.GF_COMPOSITE_LTHOLDINGS gcl
where gcl.PORTFOLIO_ID = 'EQYBGA' and gcl.PORTFOLIO_DATE = @PortDate

--Last Analyst Upload
select ints.ISSUER_ID, MAX(ints.root_source_date) as ANALYST_UPDATE
into #S4 
from dbo.INTERNAL_STATEMENT ints
where ints.ROOT_SOURCE = 'PRIMARY'
group by ISSUER_ID


--Extract Security level data

select * 
into #S5
from dbo.PERIOD_FINANCIALS pf
where pf.SECURITY_ID in (select SECURITY_ID from #S1)
and pf.DATA_SOURCE = 'PRIMARY'
and pf.CURRENCY = 'USD'
and pf.FISCAL_TYPE in ('CALENDAR','')
and pf.PERIOD_TYPE in ('C')
and pf.DATA_ID in (185,186,236,198,238,188,187,189,218)

select s.security_id, 
	s2.amount as PBV_BF24,
	s3.amount as PE_BF24,
	s4.amount as PCE_BF24,
	s5.amount as EV_EBITDA_BF24,
	s6.amount as DY_BF24,
	s7.amount as PNAV_BF24,
	s.amount as MKT_CAP,
	s1.amount as EV,
	s8.amount as SHARES
into #SecLevel	
from #S5 s
left join #S5 s1 on s.security_id = s1.security_id and s1.data_id = 186
left join #S5 s2 on s.security_id = s2.security_id and s2.data_id = 188
left join #S5 s3 on s.security_id = s3.security_id and s3.data_id = 187
left join #S5 s4 on s.security_id = s4.security_id and s4.data_id = 189
left join #S5 s5 on s.security_id = s5.security_id and s5.data_id = 198
left join #S5 s6 on s.security_id = s6.security_id and s6.data_id = 236
left join #S5 s7 on s.security_id = s7.security_id and s7.data_id = 238
left join #S5 s8 on s.security_id = s8.security_id and s8.data_id = 218
where s.data_id = 185 and s.period_type = 'C'


--Extract Issuer level data
select * 
into #S6
from dbo.PERIOD_FINANCIALS pf
where pf.ISSUER_ID in (select ISSUER_ID from #S1)
and pf.DATA_SOURCE = 'PRIMARY'
and pf.CURRENCY = 'USD'
and pf.PERIOD_TYPE in ('C')
and pf.DATA_ID in (279,280,300)

select s.issuer_id, 
	s1.amount as EQUITY_BF24,
	s2.amount as EARNINGS_BF24,
	s3.amount as DIVIDENDS_BF24
into #IssLevel	
from #Issuers s
left join #S6 s1 on s.issuer_id = s1.issuer_id and s1.data_id = 280 
left join #S6 s2 on s.issuer_id = s2.issuer_id and s2.data_id = 279 
left join #S6 s3 on s.issuer_id = s3.issuer_id and s3.data_id = 300 


--Fair Value data
select fv.SECURITY_ID, dm.DATA_DESC, fv.CURRENT_MEASURE_VALUE, fv.FV_BUY, fv.FV_SELL, fv.UPSIDE, fv.UPDATED, fv.FV_MEASURE
into #FV
from dbo.FAIR_VALUE fv
join dbo.DATA_MASTER dm on fv.FV_MEASURE = dm.DATA_ID
where fv.VALUE_TYPE = 'PRIMARY'

--Join it all together
select a.*, 
	B.MKT_VALUE,
	B.BGA_WEIGHT,
	C.ANALYST_UPDATE,
	fv.updated AS FV_UPDATE,
	fv.FV_MEASURE as FV_ID, 
	fv.data_desc as FV_MEASURE,
	fv.fv_buy as BUY,
	fv.fv_sell as SELL,
	fv.CURRENT_MEASURE_VALUE,
	fv.UPSIDE, 	
	e.PBV_BF24,
	e.PE_BF24,
	e.PCE_BF24,
	e.EV_EBITDA_BF24,
	e.PNAV_BF24,
	e.DY_BF24,
	E.MKT_CAP, 
	E.SHARES,
	E.EV, 
	F.EQUITY_BF24, 
	F.EARNINGS_BF24, 
	case when PCE_BF24 = NULL then NULL else E.MKT_CAP/e.PCE_BF24 end as CASHEARN_BF24,
	case when EV_EBITDA_BF24 = NULL then NULL else E.EV/e.EV_EBITDA_BF24 end as EBITDA_BF24,
	case when PNAV_BF24 = NULL then NULL else E.MKT_CAP/e.PNAV_BF24 end as NAV_BF24,
 	F.DIVIDENDS_BF24,
 	p.PRICE,
 	fx.FX_RATE
into #Screening	
from #S1 a
left join #S3 b on a.short_name = b.SHORT_NAME
left join #S4 c on a.issuer_id = c.issuer_id
left join #SecLevel e on a.security_id = E.security_id
left join #IssLevel f on a.issuer_id = f.issuer_id
left join #FV fv on a.security_id = fv.security_id
left join dbo.PRICES p on a.security_id = p.SECURITY_ID and p.PRICE_DATE = @PRICEDATE
left join dbo.FX_RATES fx on a.trading_currency = fx.CURRENCY and fx.FX_DATE = @PRICEDATE
order by a.COUNTRY, a.ISSUE_NAME

update #Screening
set SHARES_PER_ADR = 1 where SHARES_PER_ADR = 0

select *,
case when FV_ID = 187 then ((BUY*EARNINGS_BF24*ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	when FV_ID = 188 then ((BUY*EQUITY_BF24*ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	when FV_ID = 189 then ((BUY*CASHEARN_BF24*ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	when FV_ID = 238 then ((BUY*NAV_BF24*ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	when FV_ID = 236 then (((DIVIDENDS_BF24/BUY)/ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE	
	else 0 end as BUY_PRICE,
case when FV_ID = 187 then ((SELL*EARNINGS_BF24*ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	when FV_ID = 188 then ((SELL*EQUITY_BF24*ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	when FV_ID = 189 then ((SELL*CASHEARN_BF24*ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	when FV_ID = 238 then ((SELL*NAV_BF24*ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	when FV_ID = 236 then (((DIVIDENDS_BF24/SELL)/ISNULL(SHARES_PER_ADR,1))/SHARES)*FX_RATE
	else 0 end as SELL_PRICE
from #Screening s




/*
DECLARE @StartDT DATETIME = getdate()
DECLARE @FileLocation VARCHAR(50)

SET @FileLocation = '\\sqlprod\dataimport\AIMS Screening-' 
   + CONVERT(CHAR(10), @StartDT, 120) + '.csv';


declare @sql varchar(8000)

select @sql = 'bcp "Select * from #Screening" queryout ' + @FileLocation + ' -c -t\t -T -S ' + @@SERVERNAME 
exec master..xp_cmdshell @sql
*/


drop table #Issuers
drop table #S1
drop table #S3
drop table #S4
drop table #S5
drop table #S6
drop table #SecLevel
drop table #IssLevel
drop table #FV
drop table #Screening

go