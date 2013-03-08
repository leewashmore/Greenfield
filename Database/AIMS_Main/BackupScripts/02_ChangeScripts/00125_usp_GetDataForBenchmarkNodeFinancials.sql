set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00124'
declare @CurrentScriptVersion as nvarchar(100) = '00125'

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

IF OBJECT_ID ('[dbo].[usp_GetDataForBenchmarkNodefinancials]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_GetDataForBenchmarkNodefinancials]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetDataForBenchmarkNodefinancials] Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetDataForBenchmarkNodefinancials]
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
Amount Decimal(32,6)
)

declare @sqlquery varchar(max)

if @issuerIds is not null and @securityIds is not null
begin
 set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@issuerIds+')or SECURITY_ID IN ('+@securityIds+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

else if @issuerIds is not null
begin
set @sqlquery  = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@issuerIds+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

else if @securityIds is not null
begin 
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (SECURITY_ID IN ('+@securityIds+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

INSERT INTO @tempPorfolio  EXECUTE(@sqlquery)
SELECT * FROM @tempPorfolio
END 

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00125'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


