IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Inactive_Monitoring]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Inactive_Monitoring]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Inactive_Monitoring]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_Inactive_Monitoring]
as

select gsb.ISSUER_ID,
	gsb.SECURITY_ID,
	gsb.ASEC_SEC_SHORT_NAME,
	gsb.ISSUE_NAME,
	gsb.TICKER,
	gsb.isin,
	gsb.SECURITY_TYPE,
	gsb.ISO_COUNTRY_CODE,
	gsb.update_bb_status
from dbo.GF_SECURITY_BASEVIEW gsb
where gsb.SECURITY_ID in (select SECURITY_ID from dbo.ISSUER_SHARES_COMPOSITION)
and gsb.UPDATE_BB_STATUS in ('INACTIVE')
and gsb.SECURITY_ID not in (select SECURITY_ID from dbo.monitoring_security_suppress)
order by gsb.ISSUER_ID
