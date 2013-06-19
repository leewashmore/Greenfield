IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Share_Composition_Monitoring]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Share_Composition_Monitoring]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Share_Composition_Monitoring]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_Share_Composition_Monitoring]
as

declare @PortDate datetime

select @PortDate = MAX(portfolio_date)
from .dbo.GF_COMPOSITE_LTHOLDINGS gcl
where gcl.PORTFOLIO_ID = 'EQYALL'

select max('HOLDING') as Type, 
max(gcl.issuer_id) as ISSUER_ID, 
gcl.ASEC_SEC_SHORT_NAME, 
max(gsb.SECURITY_ID) as SECURITY_ID, 
max(gcl.ISSUE_NAME) as ISSUE_NAME, 
max(gcl.SECURITY_TYPE) as SECURITY_TYPE, 
max(gcl.TICKER) as TICKER,
MAX(gcl.ISO_COUNTRY_CODE) as COUNTRY,
MAX(gsb.UPDATE_BB_STATUS) as BB_STATUS
into #Hold
from .dbo.GF_COMPOSITE_LTHOLDINGS gcl
left join .dbo.GF_SECURITY_BASEVIEW gsb on gcl.ASEC_SEC_SHORT_NAME = gsb.ASEC_SEC_SHORT_NAME
left join dbo.ISSUER_SHARES_COMPOSITION isc on gcl.ISSUER_ID = isc.ISSUER_ID
where isc.SECURITY_ID is null
and gsb.SECURITY_TYPE not in ('PRV EQUITY','FUND LP','FUND OEIC','BASKET EQ')
and gcl.ISSUER_ID is not null
AND gcl.PORTFOLIO_DATE = @PortDate
and gsb.SECURITY_ID not in (select SECURITY_ID from dbo.monitoring_security_suppress)
group by gcl.ASEC_SEC_SHORT_NAME


select max('BENCHMARK') as Type, 
max(bm.issuer_id) as ISSUER_ID, 
bm.ASEC_SEC_SHORT_NAME, 
max(gsb.SECURITY_ID) as SECURITY_ID, 
max(bm.ISSUE_NAME) as ISSUE_NAME, 
max(bm.SECURITY_TYPE) as SECURITY_TYPE, 
max(bm.TICKER) as TICKER,
MAX(bm.ISO_COUNTRY_CODE) as COUNTRY,
MAX(gsb.UPDATE_BB_STATUS) as BB_STATUS
into #Benchmark
from .dbo.GF_BENCHMARK_HOLDINGS bm 
left join .dbo.GF_SECURITY_BASEVIEW gsb on bm.ASEC_SEC_SHORT_NAME = gsb.ASEC_SEC_SHORT_NAME
left join dbo.ISSUER_SHARES_COMPOSITION isc on bm.ISSUER_ID = isc.ISSUER_ID
where isc.SECURITY_ID is null
and gsb.SECURITY_TYPE not in ('PRV EQUITY','FUND LP','FUND OEIC','BASKET EQ')
and bm.ISSUER_ID is not null
AND bm.PORTFOLIO_DATE = @PortDate
and bm.ISSUER_ID not in (select ISSUER_ID from #Hold)
and gsb.SECURITY_ID not in (select SECURITY_ID from dbo.monitoring_security_suppress)
group by bm.ASEC_SEC_SHORT_NAME


select 'WATCHLIST' as Type,
gsb.ISSUER_ID, 
gsb.ASEC_SEC_SHORT_NAME, 
gsb.SECURITY_ID, 
gsb.ISSUE_NAME, 
gsb.SECURITY_TYPE, 
gsb.TICKER, 
gsb.ISO_COUNTRY_CODE, 
gsb.UPDATE_BB_STATUS
into #Watch
from dbo.GF_SECURITY_BASEVIEW gsb
left join dbo.ISSUER_SHARES_COMPOSITION isc on gsb.ISSUER_ID = isc.ISSUER_ID
where isc.SECURITY_ID is null
and gsb.SECURITY_TYPE not in ('PRV EQUITY','FUND LP','FUND OEIC','BASKET EQ')
and gsb.ISSUER_ID is not null
and gsb.ISSUER_ID not in (select ISSUER_ID from #Hold)
and gsb.ISSUER_ID not in (select ISSUER_ID from #Benchmark)
and gsb.SECURITY_ID not in (select SECURITY_ID from dbo.monitoring_security_suppress)
order by gsb.ISSUER_ID

select *
from #Hold
union
select *
from #Benchmark 
union
select *
from #Watch


drop table #Hold
drop table #Benchmark
drop table #Watch

