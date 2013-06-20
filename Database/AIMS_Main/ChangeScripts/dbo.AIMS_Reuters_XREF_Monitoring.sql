IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Reuters_XREF_Monitoring]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Reuters_XREF_Monitoring]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Reuters_XREF_Monitoring]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_Reuters_XREF_Monitoring]
as

select gsb.ISSUER_ID, gsb.ASEC_SEC_SHORT_NAME, gsb.SECURITY_ID, gsb.ISSUE_NAME, gsb.SECURITY_TYPE, gsb.TICKER, gsb.isin, gsb.ISO_COUNTRY_CODE, gsb.XREF, gsb.REPORTNUMBER, ci.Name as 'REUTERS_NAME', ci.Ticker as 'REUTERS_TICKER', ci.ISIN as 'REUTERS_ISIN',ci.XRef as 'REUTERS_XREF', ci.ReportNumber as 'REUTERS_REPORTNUMBER'
from dbo.GF_SECURITY_BASEVIEW gsb
left join AIMS_Reuters.dbo.tblCompanyInfo ci on gsb.ISIN = ci.ISIN
where (gsb.XREF is null or gsb.REPORTNUMBER is null or gsb.XREF = '' or gsb.REPORTNUMBER = '' or gsb.XREF = 'NULL' or gsb.REPORTNUMBER = 'NULL')
and gsb.SECURITY_TYPE not in ('PRV EQUITY','FUND LP','FUND OEIC','BASKET EQ')
and gsb.ISSUER_ID is not null
and gsb.SECURITY_ID not in (select SECURITY_ID from dbo.monitoring_security_suppress)
and ci.XRef is not null
order by gsb.ISSUER_ID