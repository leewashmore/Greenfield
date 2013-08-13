IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expSumAEIMSData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expSumAEIMSData]
GO

/****** Object:  StoredProcedure [dbo].[expSumAEIMSData]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expSumAEIMSData](
@BENCHID varchar(50) = 'MSCI FRONTIER NET',
@ISOCountryCode nvarchar(3) = ''
)
as

declare @RelPeriodM1 char(4) = year(getdate())-1
declare @RelPeriod char(4) = year(getdate())
declare @RelPeriod1 char(4) = year(getdate())+1


select *
into #eM1P 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = @RelPeriodM1
order by pf.ISSUER_ID

select *
into #eM1R 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'REUTERS'
and pf.PERIOD_YEAR = @RelPeriodM1
order by pf.ISSUER_ID

select *
into #eP 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = @RelPeriod
order by pf.ISSUER_ID

select *
into #e1P 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = @RelPeriod1
order by pf.ISSUER_ID

select *
into #eR 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'REUTERS'
and pf.PERIOD_YEAR = @RelPeriod
order by pf.ISSUER_ID

select *
into #e1R 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'REUTERS'
and pf.PERIOD_YEAR = @RelPeriod1
order by pf.ISSUER_ID

select *
into #eqP 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '104'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = @RelPeriod
order by pf.ISSUER_ID

select *
into #eq1P 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '104'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = @RelPeriod1
order by pf.ISSUER_ID

select *
into #dyP 
from PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'CALENDAR'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY = 'USD'
and pf.DATA_ID = '124'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = @RelPeriod
order by pf.ISSUER_ID

select * 
into #mktCap
from PERIOD_FINANCIALS pf
where pf.DATA_ID = 185
and pf.CURRENCY = 'USD'
and pf.PERIOD_TYPE = 'C'
and pf.DATA_SOURCE = 'PRIMARY'


select 
			sb.SECURITY_ID
			,sb.ticker
			,a.ASEC_SEC_SHORT_NAME
			,sb.ISSUER_NAME
			,a.ISO_COUNTRY_CODE
			,a.COUNTRYNAME
			,a.BENCHMARK_WEIGHT
			,sb.MSCI
			,eM1P.AMOUNT/ish.SHARES_OUTSTANDING as epsM1P
			,eM1r.AMOUNT/ish.SHARES_OUTSTANDING as epsM1R
			,eP.AMOUNT/ish.SHARES_OUTSTANDING as epsP
			,e1P.AMOUNT/ish.SHARES_OUTSTANDING as eps1P
			,eR.AMOUNT/ish.SHARES_OUTSTANDING as epsR
			,e1R.AMOUNT/ish.SHARES_OUTSTANDING as eps1R
			,eqP.AMOUNT/ish.SHARES_OUTSTANDING as bvpsP
			,eq1P.AMOUNT/ish.SHARES_OUTSTANDING as bvps1P
			,(-1*dyP.AMOUNT)/ish.SHARES_OUTSTANDING as dyP	
			,mc.amount as mktCap
			,p.PRICE
			,p.ADR_CONV
			,fx.FX_RATE
		into #GBH
		from GF_BENCHMARK_HOLDINGS a 
			left join GF_SECURITY_BASEVIEW sb on a.ASEC_SEC_SHORT_NAME = sb.ASEC_SEC_SHORT_NAME
			left join #eM1P eM1P on sb.ISSUER_ID = eM1p.ISSUER_ID
			left join #eM1R eM1R on sb.ISSUER_ID = eM1r.ISSUER_ID
			left join #eP eP on sb.ISSUER_ID = ep.ISSUER_ID
			left join #e1P e1P on sb.ISSUER_ID = e1p.ISSUER_ID
			left join #eR eR on sb.ISSUER_ID = er.ISSUER_ID
			left join #e1R e1R on sb.ISSUER_ID = e1r.ISSUER_ID
			left join #eqP eqP on sb.ISSUER_ID = eqp.ISSUER_ID
			left join #eq1P eq1P on sb.ISSUER_ID = eq1p.ISSUER_ID
			left join #dyP dyP on sb.ISSUER_ID = dyp.ISSUER_ID
			left join ISSUER_SHARES ish on sb.ISSUER_ID = ish.ISSUER_ID	and ish.SHARES_DATE = (select MAX(PORTFOLIO_DATE) from GF_BENCHMARK_HOLDINGS)
			left join #mktCap mc on sb.SECURITY_ID = mc.security_id
			left join PRICES p on p.SECURITY_ID = sb.SECURITY_ID and a.PORTFOLIO_DATE = p.PRICE_DATE
			left join FX_RATES fx on fx.CURRENCY = sb.TRADING_CURRENCY and fx.FX_DATE = a.PORTFOLIO_DATE
		where BENCHMARK_ID = @BENCHID
		AND PORTFOLIO_DATE = (select MAX(PORTFOLIO_DATE) from GF_BENCHMARK_HOLDINGS)
		and 1 = case
			when @ISOCountryCode = '' then 1
			when @ISOCountryCode = a.iso_country_code then 1
			end
		order by a.COUNTRYNAME,a.MARKET_CAP_IN_USD desc

declare @EPSM1P char(8) =   'EPS' + cast(@RelPeriodM1 as char(4)) +'P' 
declare @EPSM1R char(8) =   'EPS' + cast(@RelPeriodM1 as char(4)) +'R' 
declare @EPSP char(8) =   'EPS' + cast(@RelPeriod as char(4)) +'P' 
declare @EPS1P char(8) =   'EPS' + cast(@RelPeriod1 as char(4)) +'P' 
declare @EPSR char(8) =   'EPS' + cast(@RelPeriod as char(4)) +'R' 
declare @EPS1R char(8) =   'EPS' + cast(@RelPeriod1 as char(4)) +'R' 
declare @BVPSP char(9) =   'BVPS' + cast(@RelPeriod as char(4)) +'P' 
declare @BVPS1P char(9) =   'BVPS' + cast(@RelPeriod1 as char(4)) +'P' 
declare @DYP char(8) =   'DY' + cast(@RelPeriod as char(4)) +'P' 

exec tempdb..sp_rename '#GBH.epsM1P',@EPSM1P,'COLUMN'
exec tempdb..sp_rename '#GBH.epsM1R',@EPSM1R,'COLUMN'
exec tempdb..sp_rename '#GBH.epsP',@EPSP,'COLUMN'
exec tempdb..sp_rename '#GBH.eps1P',@EPS1P,'COLUMN'
exec tempdb..sp_rename '#GBH.epsR',@EPSR,'COLUMN'
exec tempdb..sp_rename '#GBH.eps1R',@EPS1R,'COLUMN'
exec tempdb..sp_rename '#GBH.bvpsP',@BVPSP,'COLUMN'
exec tempdb..sp_rename '#GBH.bvps1P',@BVPS1P,'COLUMN'
exec tempdb..sp_rename '#GBH.dyP',@DYP,'COLUMN'

select *
from #GBH

drop table #eM1p
drop table #eM1r
drop table #ep
drop table #e1p
drop table #er
drop table #e1r
drop table #dyp
drop table #eqp
drop table #eq1p
--drop table #fwdE
drop table #mktCap
drop table #GBH


go