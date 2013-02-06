set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00149'
declare @CurrentScriptVersion as nvarchar(100) = '00150'

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

IF OBJECT_ID ('[dbo].[usp_GetMacroDataEMSummary]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_GetMacroDataEMSummary]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[usp_GetMacroDataEMSummary]
(
@countryCodes varchar(max)
)
AS

SET FMTONLY OFF

BEGIN
DECLARE @tempMacroData TABLE
(
CountryCode varchar(8),
Field varchar(30),
Year1 int,
Value Decimal(32,6)
)
declare @sqlquery varchar(max),
@previousYear INT,
@currentYear INT,
@nextYear INT
if @countryCodes is not null
begin
SET @currentYear = (select datepart(yyyy,getdate()))
set @previousYear = @currentYear - 1
set @nextYear = @currentYear + 1
 set @sqlquery = 'Select COUNTRY_CODE,FIELD,YEAR1,VALUE from dbo.Macroeconomic_Data 
where ( COUNTRY_CODE in  ('+@countryCodes+')) and FIELD in (''REAL_GDP_GROWTH_RATE'',''INFLATION_PCT'',''ST_INTEREST_RATE'',''CURRENT_ACCOUNT_PCT_GDP'')
and YEAR1 in ('+CAST(@previousYear AS VARCHAR(10))+','+CAST(@currentYear AS VARCHAR(10))+','+CAST(@nextYear AS VARCHAR(10))+')'
end
INSERT INTO @tempMacroData  EXECUTE(@sqlquery)
select * from @tempMacroData
END 



GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00150'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


