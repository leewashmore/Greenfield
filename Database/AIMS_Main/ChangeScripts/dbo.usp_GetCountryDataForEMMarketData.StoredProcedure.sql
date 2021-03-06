SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[usp_GetCountryDataForEMMarketData]
AS
BEGIN
(Select se.COUNTRY_CODE as CountryCode,cm.COUNTRY_NAME as CountryName,cm.ASHEMM_PROPRIETARY_REGION_NAME as RegionName,
'C' AS Type 
from dbo.SUMMARY_EM_DISPLAY se
inner join dbo.Country_Master cm 
on se.COUNTRY_CODE = cm.COUNTRY_CODE
union
Select cg.COUNTRY_CODE CountryCode,cgm.GROUP_NAME as CountryName,rm.ASHEMM_PROPRIETARY_REGION_NAME as RegionName,
'G' AS Type 
from dbo.SUMMARY_EM_DISPLAY se
inner join dbo.COUNTRY_GROUP cg
on se.GROUP_CODE = cg.GROUP_CODE
inner join dbo.COUNTRY_GROUP_MASTER cgm
on se.GROUP_CODE = cgm.GROUP_CODE
inner join dbo.REGION_MASTER rm
on cgm.ASHEMM_PROPRIETARY_REGION_CODE = rm.ASHEMM_PROPRIETARY_REGION_CODE)
order by CountryName
END
GO
