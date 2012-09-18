set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00060'
declare @CurrentScriptVersion as nvarchar(100) = '00061'

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

CREATE PROCEDURE [dbo].[GetCustomScreeningMarketCap]
(
@issuerIdsList varchar(max),
@securityIdsList varchar(max)
)
AS

SET FMTONLY OFF

BEGIN
DECLARE @tempTable TABLE
(
SecurityId varchar(20),
IssuerId varchar(20),
Amount decimal(32,6) not null
)
DECLARE @sqlquery varchar(max);

if @issuerIdsList is not null and @securityIdsList is not null
begin
set @sqlquery = 'Select SECURITY_ID,ISSUER_ID,AMOUNT
from PERIOD_FINANCIALS
where (ISSUER_ID IN ('+@issuerIdsList+')or SECURITY_ID IN ('+@securityIdsList+'))
AND	DATA_ID = 185
AND DATA_SOURCE = ''PRIMARY''
AND CURRENCY = ''USD''
AND PERIOD_TYPE = ''C'''
end

ELSE if @issuerIdsList is not null
begin 
set @sqlquery = 'Select SECURITY_ID,ISSUER_ID,AMOUNT
from PERIOD_FINANCIALS
where ISSUER_ID IN ('+@issuerIdsList+')
AND	DATA_ID = 185
AND DATA_SOURCE = ''PRIMARY''
AND CURRENCY = ''USD''
AND PERIOD_TYPE = ''C'''
end

else if @securityIdsList is not null
begin
Set @sqlquery = 'Select SECURITY_ID,ISSUER_ID,AMOUNT
from PERIOD_FINANCIALS
where SECURITY_ID IN ('+@securityIdsList+')
AND	DATA_ID = 185
AND DATA_SOURCE = ''PRIMARY''
AND CURRENCY = ''USD''
AND PERIOD_TYPE = ''C'''
end

INSERT INTO @tempTable  EXECUTE(@sqlquery)

Select * from @tempTable;
END 

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00061'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




