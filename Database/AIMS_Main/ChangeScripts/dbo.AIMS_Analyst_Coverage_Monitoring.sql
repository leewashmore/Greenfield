IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Analyst_Coverage_Monitoring]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Analyst_Coverage_Monitoring]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Analyst_Coverage_Monitoring]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_Analyst_Coverage_Monitoring]
as

declare @HoldDate datetime

select @HoldDate = MAX(glt.PORTFOLIO_DATE)
from dbo.GF_COMPOSITE_LTHOLDINGS glt
where glt.PORTFOLIO_ID ='EQYALL'

select gsb.issuer_id, gsb.ASEC_SEC_SHORT_NAME, gsb.SECURITY_ID, gsb.ISSUE_NAME, gsb.SECURITY_TYPE, gsb.TICKER, gsb.ISO_COUNTRY_CODE, gsb.ASHMOREEMM_PRIMARY_ANALYST --, gsb.ASHMOREEMM_PORTFOLIO_MANAGER
from .dbo.GF_COMPOSITE_LTHOLDINGS glt
join .dbo.GF_SECURITY_BASEVIEW gsb on glt.ASEC_SEC_SHORT_NAME = gsb.ASEC_SEC_SHORT_NAME
where glt.PORTFOLIO_ID = 'EQYALL'
and glt.PORTFOLIO_DATE = @HoldDate
and (gsb.ASHMOREEMM_PRIMARY_ANALYST is null or gsb.ASHMOREEMM_PRIMARY_ANALYST = '' or gsb.ASHMOREEMM_PRIMARY_ANALYST = 'NULL') -- or gsb.ASHMOREEMM_PORTFOLIO_MANAGER is null or gsb.ASHMOREEMM_PORTFOLIO_MANAGER = '' or gsb.ASHMOREEMM_PORTFOLIO_MANAGER = 'NULL')
and gsb.SECURITY_TYPE not in ('PRV EQUITY','FUND LP','FUND OEIC','BASKET EQ')
and gsb.ISSUER_ID is not null
order by gsb.ISO_COUNTRY_CODE, gsb.ISSUER_NAME, gsb.ISSUE_NAME