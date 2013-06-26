IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Price_Refresh_Monitoring]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Price_Refresh_Monitoring]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Price_Refresh_Monitoring]    Script Date: 06/17/2013 08:49:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


CREATE procedure [dbo].[AIMS_Price_Refresh_Monitoring]
as

--First select all the minimum dates
select SECURITY_ID, MIN(PRICE_DATE) as MIN_DATE
into #minDates
from dbo.PRICES
group by SECURITY_ID

select gsb.ASEC_SEC_SHORT_NAME, gsb.SECURITY_ID, md.MIN_DATE
from .dbo.GF_SECURITY_BASEVIEW gsb
left join #minDates md on md.SECURITY_ID = gsb.SECURITY_ID
where gsb.SECURITY_TYPE not in ('PRV EQUITY','FUND LP','FUND OEIC','BASKET EQ')
order by gsb.ASEC_SEC_SHORT_NAME

drop table #minDates
