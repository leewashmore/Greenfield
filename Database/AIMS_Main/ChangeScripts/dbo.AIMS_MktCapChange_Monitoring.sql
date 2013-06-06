IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_MktCapChange_Monitoring]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_MktCapChange_Monitoring]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_MktCapChange_Monitoring]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_MktCapChange_Monitoring]
as

select mdcc.CURR_DATE, 
	gsb.ISSUER_ID, 
	gsb.ASEC_SEC_SHORT_NAME, 
	gsb.SECURITY_ID, 
	gsb.ISSUE_NAME, 
	gsb.TICKER, 
	gsb.ISO_COUNTRY_CODE,
	cast((((mdcc.CURR_CAP/mdcc.PRIOR_CAP)-1)*100) as decimal(6,1)) as CHANGE_PCT,
	mdcc.PRIOR_CAP,
	mdcc.CURR_CAP,
	mdcc.PRIOR_DIAGRAM,
	mdcc.CURR_DIAGRAM
  from dbo.MONITORING_DAILY_CAP_CHANGE mdcc
  join dbo.GF_SECURITY_BASEVIEW gsb on mdcc.SECURITY_ID = gsb.SECURITY_ID
  where mdcc.PRIOR_CAP <>0 and mdcc.CURR_CAP <> 0
  and (mdcc.CURR_CAP/mdcc.PRIOR_CAP > 1.1 or mdcc.CURR_CAP/mdcc.PRIOR_CAP < .9) 