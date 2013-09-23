IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expScreeningReutersComp]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].expScreeningReutersComp
GO

/****** Object:  StoredProcedure [dbo].[expScreeningReutersComp]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expScreeningReutersComp]
as

--extract full universe
select gsb.ISSUER_ID, 
	gsb.ISSUER_NAME,
	gsb.TICKER,
	gsb.GICS_SECTOR_NAME as SECTOR,
	gsb.asec_sec_country_name as COUNTRY,
	gsb.ASHMOREEMM_PRIMARY_ANALYST as PRIMARY_ANALYST,
	gsb.issuer_proxy
into #S1	
from dbo.GF_SECURITY_BASEVIEW gsb
where gsb.SECURITY_TYPE not in ('BASKET EQ','FUND LP','FUND OEIC','PRV EQUITY')
and gsb.ISSUER_ID is not null
and gsb.SECURITY_ID in (select issuer_proxy from dbo.GF_SECURITY_BASEVIEW)

select issuer_id
into #Issuers
from #S1
group by issuer_id

--Last Analyst Upload
select ints.ISSUER_ID, MAX(ints.root_source_date) as LAST_UPDATE
into #S2 
from dbo.INTERNAL_STATEMENT ints
where ints.ROOT_SOURCE = 'PRIMARY'
group by ISSUER_ID

--Extract Issuer level data
select * 
into #S3
from dbo.PERIOD_FINANCIALS pf
where pf.ISSUER_ID in (select ISSUER_ID from #S1)
and pf.CURRENCY = 'USD'
and pf.FISCAL_TYPE in ('FISCAL')
and pf.PERIOD_TYPE in ('A')
and pf.PERIOD_YEAR in (2013,2014,2015)
and pf.DATA_ID in (11,29,290)

--Extract ROE BF24 data
select * 
into #S5
from dbo.PERIOD_FINANCIALS pf
where pf.ISSUER_ID in (select ISSUER_ID from #S1)
and pf.CURRENCY = 'USD'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_TYPE in ('C')
and pf.DATA_ID = 200

--Extract Security level/BF data
select * 
into #S4
from dbo.PERIOD_FINANCIALS pf
where pf.SECURITY_ID in (select issuer_proxy from #S1) 
and pf.CURRENCY = 'USD'
and pf.PERIOD_TYPE in ('C')
and pf.DATA_SOURCE = 'PRIMARY'
and pf.DATA_ID in (185,187,188)

--Extract Upsides
select * 
into #S6
from dbo.FAIR_VALUE fv
where fv.security_id in (select issuer_proxy from #S1)
and fv.value_type = 'PRIMARY'

select s.issuer_id, 
	s1.amount as REVENUE_2013_PRIMARY,
	s2.amount as REVENUE_2014_PRIMARY,
	s3.amount as REVENUE_2015_PRIMARY,
	s4.amount as REVENUE_2013_REUTERS,
	s5.amount as REVENUE_2014_REUTERS,
	s6.amount as REVENUE_2015_REUTERS,
	s7.amount as EARNINGS_2013_PRIMARY,
	s8.amount as EARNINGS_2014_PRIMARY,
	s9.amount as EARNINGS_2015_PRIMARY,
	s10.amount as EARNINGS_2013_REUTERS,
	s11.amount as EARNINGS_2014_REUTERS,
	s12.amount as EARNINGS_2015_REUTERS,
	s13.amount as OPINCOME_2013_PRIMARY,
	s14.amount as OPINCOME_2014_PRIMARY,
	s15.amount as OPINCOME_2015_PRIMARY,
	s16.amount as OPINCOME_2013_REUTERS,
	s17.amount as OPINCOME_2014_REUTERS,
	s18.amount as OPINCOME_2015_REUTERS,
	s1.root_source as ROOT_SOURCE_2013,
	s2.root_source as ROOT_SOURCE_2014,
	s3.root_source as ROOT_SOURCE_2015,
	s19.amount as ROE_BF24
into #IssLevel	
from #Issuers s
left join #S3 s1 on s.issuer_id = s1.issuer_id and s1.data_id = 11 and s1.period_year = 2013 and s1.data_source = 'PRIMARY'
left join #S3 s2 on s.issuer_id = s2.issuer_id and s2.data_id = 11 and s2.period_year = 2014 and s2.data_source = 'PRIMARY'
left join #S3 s3 on s.issuer_id = s3.issuer_id and s3.data_id = 11 and s3.period_year = 2015 and s3.data_source = 'PRIMARY'
left join #S3 s4 on s.issuer_id = s4.issuer_id and s4.data_id = 11 and s4.period_year = 2013 and s4.data_source = 'REUTERS'
left join #S3 s5 on s.issuer_id = s5.issuer_id and s5.data_id = 11 and s5.period_year = 2014 and s5.data_source = 'REUTERS'
left join #S3 s6 on s.issuer_id = s6.issuer_id and s6.data_id = 11 and s6.period_year = 2015 and s6.data_source = 'REUTERS'
left join #S3 s7 on s.issuer_id = s7.issuer_id and s7.data_id = 290 and s7.period_year = 2013 and s7.data_source = 'PRIMARY'
left join #S3 s8 on s.issuer_id = s8.issuer_id and s8.data_id = 290 and s8.period_year = 2014 and s8.data_source = 'PRIMARY'
left join #S3 s9 on s.issuer_id = s9.issuer_id and s9.data_id = 290 and s9.period_year = 2015 and s9.data_source = 'PRIMARY'
left join #S3 s10 on s.issuer_id = s10.issuer_id and s10.data_id = 290 and s10.period_year = 2013 and s10.data_source = 'REUTERS'
left join #S3 s11 on s.issuer_id = s11.issuer_id and s11.data_id = 290 and s11.period_year = 2014 and s11.data_source = 'REUTERS'
left join #S3 s12 on s.issuer_id = s12.issuer_id and s12.data_id = 290 and s12.period_year = 2015 and s12.data_source = 'REUTERS'
left join #S3 s13 on s.issuer_id = s13.issuer_id and s13.data_id = 29 and s13.period_year = 2013 and s13.data_source = 'PRIMARY'
left join #S3 s14 on s.issuer_id = s14.issuer_id and s14.data_id = 29 and s14.period_year = 2014 and s14.data_source = 'PRIMARY'
left join #S3 s15 on s.issuer_id = s15.issuer_id and s15.data_id = 29 and s15.period_year = 2015 and s15.data_source = 'PRIMARY'
left join #S3 s16 on s.issuer_id = s16.issuer_id and s16.data_id = 29 and s16.period_year = 2013 and s16.data_source = 'REUTERS'
left join #S3 s17 on s.issuer_id = s17.issuer_id and s17.data_id = 29 and s17.period_year = 2014 and s17.data_source = 'REUTERS'
left join #S3 s18 on s.issuer_id = s18.issuer_id and s18.data_id = 29 and s18.period_year = 2015 and s18.data_source = 'REUTERS'
left join #S5 s19 on s.issuer_id = s19.issuer_id and s19.data_id = 200 and s19.data_source = 'PRIMARY'



--Join it all together
select a.*, 
	b.LAST_UPDATE,
	c.ROOT_SOURCE_2013,
	c.ROOT_SOURCE_2014,
	c.ROOT_SOURCE_2015, 
	c.REVENUE_2013_PRIMARY,
	c.REVENUE_2014_PRIMARY,
	c.REVENUE_2015_PRIMARY,
	c.REVENUE_2013_REUTERS,
	c.REVENUE_2014_REUTERS,
	c.REVENUE_2015_REUTERS,
	c.EARNINGS_2013_PRIMARY,
	c.EARNINGS_2014_PRIMARY,
	c.EARNINGS_2015_PRIMARY,
	c.EARNINGS_2013_REUTERS,
	c.EARNINGS_2014_REUTERS,
	c.EARNINGS_2015_REUTERS,
	c.OPINCOME_2013_PRIMARY,
	c.OPINCOME_2014_PRIMARY,
	c.OPINCOME_2015_PRIMARY,
	c.OPINCOME_2013_REUTERS,
	c.OPINCOME_2014_REUTERS,
	c.OPINCOME_2015_REUTERS,
	d.amount as MKT_CAP,
	e.amount as PE_BF24,
	f.amount as PBV_BF24,
	c.ROE_BF24,
	g.upside
from #S1 a
left join #S2 b on a.issuer_id = b.issuer_id
left join #IssLevel c on a.issuer_id = c.issuer_id
left join #S4 d on a.issuer_proxy = d.security_id and d.data_id = 185 
left join #S4 e on a.issuer_proxy = e.security_id and e.data_id = 187 
left join #S4 f on a.issuer_proxy = f.security_id and f.data_id = 188 
left join #S6 g on a.issuer_proxy = g.security_id  
order by a.COUNTRY, a.ISSUER_NAME


drop table #Issuers
drop table #S1
drop table #S2
drop table #S3
drop table #S4
drop table #S5
drop table #S6
drop table #IssLevel

go