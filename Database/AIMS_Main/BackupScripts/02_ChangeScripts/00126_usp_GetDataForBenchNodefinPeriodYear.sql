set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00125'
declare @CurrentScriptVersion as nvarchar(100) = '00126'

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

IF OBJECT_ID ('[dbo].[usp_GetDataForBenchNodefinPeriodYear]')  IS NOT NULL
	DROP PROCEDURE [dbo].[usp_GetDataForBenchNodefinPeriodYear]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetDataForBenchNodefinPeriodYear] Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetDataForBenchNodefinPeriodYear]
(
@issuerIds varchar(max),
@securityIds varchar(max)
)
AS

SET FMTONLY OFF

BEGIN

DECLARE @tempPorfolio TABLE
(
IssuerId varchar(20),
SecurityId varchar(20),
DataId int,
Amount Decimal(32,6),
PeriodYear int
)

DECLARE 
@previousYear INT,
@aheadYear INT

declare @sqlquery varchar(max)

SET @previousYear = (select datepart(yyyy,getdate())-1)
SET @aheadYear = (select datepart(yyyy,getdate())+3)

--SELECT @previousYear,@aheadYear

if @issuerIds is not null and @securityIds is not null
begin
 set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''A''
and FISCAL_TYPE = ''CALENDAR'' 
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@issuerIds+')or SECURITY_ID IN ('+@securityIds+'))
and PERIOD_YEAR >= '+CAST(@previousYear AS VARCHAR(10))+' and PERIOD_YEAR <= '+CAST(@aheadYear AS VARCHAR(10))+
' and DATA_ID IN (170, 166, 171, 164, 193, 133, 192, 177, 178)'
end

else if @issuerIds is not null
begin
set @sqlquery  = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''A''
and FISCAL_TYPE = ''CALENDAR'' 
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@issuerIds+'))
and PERIOD_YEAR >= '+CAST(@previousYear AS VARCHAR(10))+' and PERIOD_YEAR <= '+CAST(@aheadYear AS VARCHAR(10))+
' and DATA_ID IN (170, 166, 171, 164, 193, 133, 192, 177, 178)'
end

else if @securityIds is not null
begin 
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''A''
and FISCAL_TYPE = ''CALENDAR'' 
and DATA_SOURCE = ''PRIMARY''
and (SECURITY_ID IN ('+@securityIds+'))
and PERIOD_YEAR >= '+CAST(@previousYear AS VARCHAR(10))+' and PERIOD_YEAR <= '+CAST(@aheadYear AS VARCHAR(10))+
' and DATA_ID IN (170, 166, 171, 164, 193, 133, 192, 177, 178)'
end

--and FISCAL_TYPE = ''CALENDAR'' 
--and DATA_SOURCE = ''PRIMARY''
--and PERIOD_YEAR >= '+CAST(@previousYear AS VARCHAR(10))+' and PERIOD_YEAR <= '+CAST(@aheadYear AS VARCHAR(10))+
--' and DATA_ID IN (170, 166, 171, 164, 193, 133, 192, 177, 178)'
--print @sqlquery
insert into @tempPorfolio EXECUTE(@sqlquery)

select * from @tempPorfolio

END 

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00126'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


