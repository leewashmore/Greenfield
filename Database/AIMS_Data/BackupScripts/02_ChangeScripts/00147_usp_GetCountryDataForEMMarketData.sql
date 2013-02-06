set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00146'
declare @CurrentScriptVersion as nvarchar(100) = '00147'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO

IF OBJECT_ID ('[dbo].[usp_GetCountryDataForEMMarketData]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_GetCountryDataForEMMarketData]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetCountryDataForEMMarketData]
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

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00147'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


