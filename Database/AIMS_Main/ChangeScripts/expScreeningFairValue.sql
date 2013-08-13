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
	gsb.GICS_SUB_INDUSTRY_NAME as SUB_INDUSTRY
into #S1	
from dbo.GF_SECURITY_BASEVIEW gsb
where gsb.SECURITY_TYPE not in ('BASKET EQ','FUND LP','FUND OEIC','PRV EQUITY')
and gsb.ISSUER_ID is not null
order by gsb.SECURITY_TYPE

select issuer_id
into #Issuers
from #S1
group by issuer_id


--Extract Security level data

select * 
into #S5
from dbo.PERIOD_FINANCIALS pf
where pf.SECURITY_ID in (select SECURITY_ID from #S1)
and pf.DATA_SOURCE = 'PRIMARY'
and pf.CURRENCY = 'USD'
and pf.FISCAL_TYPE in ('CALENDAR','')
and pf.PERIOD_TYPE in ('C')
and pf.DATA_ID in (185,186,236,198,238,188,187,189)

select s.security_id, 

	s2.amount as PBV_BF24,
	s3.amount as PE_BF24,
	s4.amount as PCE_BF24,
	s5.amount as EV_EBITDA_BF24,
	s6.amount as DY_BF24,
	s7.amount as PNAV_BF24,
	s.amount as MKT_CAP,
	s1.amount as EV
into #SecLevel	
from #S5 s
left join #S5 s1 on s.security_id = s1.security_id and s1.data_id = 186
left join #S5 s2 on s.security_id = s2.security_id and s2.data_id = 188
left join #S5 s3 on s.security_id = s3.security_id and s3.data_id = 187
left join #S5 s4 on s.security_id = s4.security_id and s4.data_id = 189
left join #S5 s5 on s.security_id = s5.security_id and s5.data_id = 198
left join #S5 s6 on s.security_id = s6.security_id and s6.data_id = 236
left join #S5 s7 on s.security_id = s7.security_id and s7.data_id = 238
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
select fv.SECURITY_ID, dm.DATA_DESC, fv.CURRENT_MEASURE_VALUE, fv.FV_BUY, fv.FV_SELL, fv.UPSIDE, fv.UPDATED
into #FV
from dbo.FAIR_VALUE fv
join dbo.DATA_MASTER dm on fv.FV_MEASURE = dm.DATA_ID
where fv.VALUE_TYPE = 'PRIMARY'

--Join it all together
select a.*, 
	fv.updated AS FV_UPDATED, 
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
	E.EV, 
	F.EQUITY_BF24, 
	F.EARNINGS_BF24, 
	case when PCE_BF24 = NULL then NULL else E.MKT_CAP/e.PCE_BF24 end as CASHEARN_BF24,
	case when EV_EBITDA_BF24 = NULL then NULL else E.EV/e.EV_EBITDA_BF24 end as EBITDA_BF24,
	case when PNAV_BF24 = NULL then NULL else E.MKT_CAP/e.PNAV_BF24 end as NAV_BF24,
 	F.DIVIDENDS_BF24
--into #Screening	
from #S1 a
left join #SecLevel e on a.security_id = E.security_id
left join #IssLevel f on a.issuer_id = f.issuer_id
left join #FV fv on a.security_id = fv.security_id
order by a.COUNTRY, a.ISSUE_NAME

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
drop table #S5
drop table #S6
drop table #SecLevel
drop table #IssLevel
drop table #FV
--drop table #Screening

go