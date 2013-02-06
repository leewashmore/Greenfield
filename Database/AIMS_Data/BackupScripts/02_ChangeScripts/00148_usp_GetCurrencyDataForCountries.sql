set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00147'
declare @CurrentScriptVersion as nvarchar(100) = '00148'

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

IF OBJECT_ID ('[dbo].[usp_GetCurrencyDataForCountries]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_GetCurrencyDataForCountries]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetCurrencyDataForCountries]
(
@countryCodes varchar(max)
)
AS

SET FMTONLY OFF

BEGIN
DECLARE @tempCurrencyCodes TABLE
(
CurrencyCode varchar(8)
)
DECLARE @fxRatesData TABLE
(
CurrencyCode varchar(8)
)
declare @sqlquery varchar(max)

if @countryCodes is not null
begin
 set @sqlquery = 'Select CURRENCY_CODE from dbo.Country_Master 
where ( COUNTRY_CODE in  ('+@countryCodes+'))'
end
INSERT INTO @tempCurrencyCodes  EXECUTE(@sqlquery)
select cm.COUNTRY_CODE,fr.CURRENCY,fr.FX_DATE,fr.FX_RATE from FX_RATES fr
left outer join Country_Master cm
on fr.CURRENCY = cm.CURRENCY_CODE
where CURRENCY in (select CurrencyCode from @tempCurrencyCodes)
END 

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00148'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


