IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expScreening]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expScreening]
GO

/****** Object:  StoredProcedure [dbo].[expScreening]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expScreening]
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


--extract benchmark weights
declare @BenchDate datetime

select @BenchDate = MAX(gbh.PORTFOLIO_DATE)
from dbo.GF_BENCHMARK_HOLDINGS gbh
where gbh.BENCHMARK_ID = 'MSCI EM IMI NET'
group by gbh.BENCHMARK_ID

--select IMI and STD weights
select gbh1.ASEC_SEC_SHORT_NAME as SHORT_NAME, gbh1.BENCHMARK_WEIGHT/100 as IMI_WEIGHT, std.STD_WEIGHT
into #S2
from dbo.GF_BENCHMARK_HOLDINGS gbh1
left join (
	select gbh2.ASEC_SEC_SHORT_NAME, gbh2.BENCHMARK_WEIGHT/100 as STD_WEIGHT
	from dbo.GF_BENCHMARK_HOLDINGS gbh2
	where gbh2.BENCHMARK_ID = 'MSCI EM NET' and gbh2.PORTFOLIO_DATE=@BenchDate
	) std on gbh1.ASEC_SEC_SHORT_NAME = std.ASEC_SEC_SHORT_NAME
where gbh1.BENCHMARK_ID = 'MSCI EM IMI NET' and gbh1.PORTFOLIO_DATE=@BenchDate

--Select BGA weight
declare @PortValue decimal(22,8)

select @PortValue = SUM(gcl.DIRTY_VALUE_PC)
from dbo.GF_COMPOSITE_LTHOLDINGS gcl
where gcl.PORTFOLIO_DATE = @BenchDate and gcl.PORTFOLIO_ID = 'EQYBGA'

select gcl.ASEC_SEC_SHORT_NAME as SHORT_NAME, gcl.DIRTY_VALUE_PC/@PortValue as BGA_WEIGHT 
into #S3
from dbo.GF_COMPOSITE_LTHOLDINGS gcl
where gcl.PORTFOLIO_ID = 'EQYBGA' and gcl.PORTFOLIO_DATE = @BenchDate

--Last Analyst Upload
select ints.ISSUER_ID, MAX(ints.root_source_date) as LAST_UPDATE
into #S4 
from dbo.INTERNAL_STATEMENT ints
where ints.ROOT_SOURCE = 'PRIMARY'
group by ISSUER_ID


--Extract Security level data
/*
declare @YearM1 int = year(getdate())-1
declare @Year int = year(getdate())
declare @Year1 int = year(getdate())+1
declare @Year2 int = year(getdate())+2
declare @Year3 int = year(getdate())+3
*/

select * 
into #S5
from dbo.PERIOD_FINANCIALS pf
where pf.SECURITY_ID in (select SECURITY_ID from #S1)
and pf.DATA_SOURCE = 'PRIMARY'
and pf.CURRENCY = 'USD'
and pf.FISCAL_TYPE in ('CALENDAR','')
and pf.PERIOD_TYPE in ('A','C')
and pf.DATA_ID in (185,166,171,193,164)

select s.security_id, 
	s.amount as MKT_CAP,
	s1.amount as PE_2012,
	s2.amount as PE_2013,
	s3.amount as PE_2014,
	s4.amount as PE_2015,
	s5.amount as PCE_2013,
	s6.amount as PCE_2014,
	s7.amount as PCE_2015,
	s8.amount as EV_EBITDA_2012,
	s9.amount as EV_EBITDA_2013,
	s10.amount as EV_EBITDA_2014,
	s11.amount as EV_EBITDA_2015,	
	s12.amount as PBV_2012,
	s13.amount as PBV_2013,
	s14.amount as PBV_2014,
	s15.amount as PBV_2015	
into #SecLevel	
from #S5 s
left join #S5 s1 on s.security_id = s1.security_id and s1.data_id = 166 and s1.period_year = 2012
left join #S5 s2 on s.security_id = s2.security_id and s2.data_id = 166 and s2.period_year = 2013
left join #S5 s3 on s.security_id = s3.security_id and s3.data_id = 166 and s3.period_year = 2014
left join #S5 s4 on s.security_id = s4.security_id and s4.data_id = 166 and s4.period_year = 2015
left join #S5 s5 on s.security_id = s5.security_id and s5.data_id = 171 and s5.period_year = 2013
left join #S5 s6 on s.security_id = s6.security_id and s6.data_id = 171 and s6.period_year = 2014
left join #S5 s7 on s.security_id = s7.security_id and s7.data_id = 171 and s7.period_year = 2015
left join #S5 s8 on s.security_id = s8.security_id and s8.data_id = 193 and s8.period_year = 2012
left join #S5 s9 on s.security_id = s9.security_id and s9.data_id = 193 and s9.period_year = 2013
left join #S5 s10 on s.security_id = s10.security_id and s10.data_id = 193 and s10.period_year = 2014
left join #S5 s11 on s.security_id = s11.security_id and s11.data_id = 193 and s11.period_year = 2015
left join #S5 s12 on s.security_id = s12.security_id and s12.data_id = 164 and s12.period_year = 2012
left join #S5 s13 on s.security_id = s13.security_id and s13.data_id = 164 and s13.period_year = 2013
left join #S5 s14 on s.security_id = s14.security_id and s14.data_id = 164 and s14.period_year = 2014
left join #S5 s15 on s.security_id = s15.security_id and s15.data_id = 164 and s15.period_year = 2015
where s.data_id = 185 and s.period_type = 'C'


--Extract Issuer level data
select * 
into #S6
from dbo.PERIOD_FINANCIALS pf
where pf.ISSUER_ID in (select ISSUER_ID from #S1)
and pf.DATA_SOURCE = 'PRIMARY'
and pf.CURRENCY = 'USD'
and pf.FISCAL_TYPE in ('CALENDAR','')
and pf.PERIOD_TYPE in ('A','C')
and pf.DATA_ID in (177,222,133)

select s.issuer_id, 
	s1.amount as EARN_GROWTH_2013,
	s2.amount as EARN_GROWTH_2014,
	s3.amount as EARN_GROWTH_2015,
	s4.amount as CASHEARN_GROWTH_2013,
	s5.amount as CASHEARN_GROWTH_2014,
	s6.amount as CASHEARN_GROWTH_2015,
	s7.amount as ROE_2013,
	s8.amount as ROE_2014,
	s9.amount as ROE_2015
into #IssLevel	
from #Issuers s
left join #S6 s1 on s.issuer_id = s1.issuer_id and s1.data_id = 177 and s1.period_year = 2013
left join #S6 s2 on s.issuer_id = s2.issuer_id and s2.data_id = 177 and s2.period_year = 2014
left join #S6 s3 on s.issuer_id = s3.issuer_id and s3.data_id = 177 and s3.period_year = 2015
left join #S6 s4 on s.issuer_id = s4.issuer_id and s4.data_id = 222 and s4.period_year = 2013
left join #S6 s5 on s.issuer_id = s5.issuer_id and s5.data_id = 222 and s5.period_year = 2014
left join #S6 s6 on s.issuer_id = s6.issuer_id and s6.data_id = 222 and s6.period_year = 2015
left join #S6 s7 on s.issuer_id = s7.issuer_id and s7.data_id = 133 and s7.period_year = 2013
left join #S6 s8 on s.issuer_id = s8.issuer_id and s8.data_id = 133 and s8.period_year = 2014
left join #S6 s9 on s.issuer_id = s9.issuer_id and s9.data_id = 133 and s9.period_year = 2015


--Fair Value data
select fv.SECURITY_ID, dm.DATA_DESC, fv.CURRENT_MEASURE_VALUE, fv.FV_BUY, fv.FV_SELL, fv.UPSIDE
into #FV
from dbo.FAIR_VALUE fv
join dbo.DATA_MASTER dm on fv.FV_MEASURE = dm.DATA_ID
where fv.VALUE_TYPE = 'PRIMARY'

--Join it all together
select a.*, 
	d.LAST_UPDATE, 
	C.BGA_WEIGHT, 
	B.IMI_WEIGHT, 
	B.STD_WEIGHT, 
	E.MKT_CAP, 
	E.PE_2012, 
	E.PE_2013, 
	E.PE_2014, 
	E.PE_2015,
	f.EARN_GROWTH_2013, 
	f.EARN_GROWTH_2014, 
	f.EARN_GROWTH_2015, 
	E.PCE_2013,
	E.PCE_2014,
	E.PCE_2015,		
	f.CASHEARN_GROWTH_2013, 
	f.CASHEARN_GROWTH_2014, 
	f.CASHEARN_GROWTH_2015,
	E.EV_EBITDA_2012,
	E.EV_EBITDA_2013,
	E.EV_EBITDA_2014,
	E.EV_EBITDA_2015,
	E.PBV_2012,
	E.PBV_2013,
	E.PBV_2014,
	E.PBV_2015,
	f.ROE_2013, 	
	f.ROE_2014, 	
	f.ROE_2015,
	fv.data_desc as FV_MEASURE,
	fv.CURRENT_MEASURE_VALUE,
	fv.fv_buy as BUY,
	fv.fv_sell as SELL,
	fv.UPSIDE 	
--into #Screening	
from #S1 a
left join #S2 b on a.short_name = B.SHORT_NAME
left join #S3 c on a.short_name = c.SHORT_NAME
left join #S4 d on a.issuer_id = d.issuer_id
left join #SecLevel e on a.security_id = E.security_id
left join #IssLevel f on a.issuer_id = f.issuer_id
left join #FV fv on a.security_id = fv.security_id
order by BGA_WEIGHT desc

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
drop table #S2
drop table #S3
drop table #S4
drop table #S5
drop table #S6
drop table #SecLevel
drop table #IssLevel
drop table #FV
--drop table #Screening

go