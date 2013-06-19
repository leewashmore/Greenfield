IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Issuer_Proxy_Monitoring]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Issuer_Proxy_Monitoring]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Issuer_Proxy_Monitoring]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_Issuer_Proxy_Monitoring]
as

--First select securities missing an issuer proxy
select distinct ISSUER_ID 
into #IP
from .dbo.GF_SECURITY_BASEVIEW gsb
where gsb.SECURITY_TYPE not in ('PRV EQUITY','FUND LP','FUND OEIC','BASKET EQ') 
and (gsb.issuer_proxy is null or gsb.issuer_proxy = '' or gsb.issuer_proxy = 'NULL')
and gsb.ISSUER_ID is not null

--then select all securities for that issuer so the user can choose one as the proxy
select 	gsb.issuer_id,
	gsb.SECURITY_ID, 
	gsb.ASEC_SEC_SHORT_NAME,
	gsb.ISSUER_NAME,
	gsb.TICKER,
	gsb.isin,
	gsb.SECURITY_TYPE,
	gsb.ISO_COUNTRY_CODE,
	gsb.issuer_proxy
from dbo.GF_SECURITY_BASEVIEW gsb
where gsb.issuer_id in (select issuer_id from #IP)
order by gsb.issuer_id

drop table #IP

--Securities with a proxy security that is not of that issuer or not in GF_SECURITY_BASEVIEW
select *
from dbo.GF_SECURITY_BASEVIEW gsb 
where gsb.issuer_proxy is not null and gsb.issuer_proxy <> ''
and gsb.issuer_proxy not in (select security_id from dbo.GF_SECURITY_BASEVIEW where ISSUER_ID = gsb.ISSUER_ID)
