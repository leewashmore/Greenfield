set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00054'
declare @CurrentScriptVersion as nvarchar(100) = '00055'

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

CREATE PROCEDURE [dbo].[GetCustomScreeningFINData]
(
@issuerIdsList varchar(max),
@securityIdsList varchar(max),
@dataID int,
@periodType char(2),
@periodYear int,
@fiscalType char(8),
@dataSource varchar(10)
)
AS

SET FMTONLY OFF

BEGIN
DECLARE @tempTable TABLE
(
IssuerId varchar(20),
SecurityId varchar(20),
DataId int,
Amount Decimal(32,6) not null,
PeriodYear int not null
)
DECLARE @sqlquery varchar(max);

if @dataSource = 'REUTERS'
begin
if @issuerIdsList is not null and @securityIdsList is not null
begin
 set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = ''REUTERS''
and (ISSUER_ID IN ('+@issuerIdsList+')or SECURITY_ID IN ('+@securityIdsList+'))
and PERIOD_TYPE = '''+@periodType+'''
and PERIOD_YEAR = '+CAST(@periodYear AS VARCHAR(10))+'
and FISCAL_TYPE = '''+@fiscalType+''''
end

else if @issuerIdsList is not null
begin
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = ''REUTERS''
and ISSUER_ID IN ('+@issuerIdsList+')
and PERIOD_TYPE = '''+@periodType+'''
and PERIOD_YEAR = '+CAST(@periodYear AS VARCHAR(10))+'
and FISCAL_TYPE = '''+@fiscalType+''''
end

else if @securityIdsList is not null
begin 
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = ''REUTERS''
and SECURITY_ID IN ('+@securityIdsList+')
and PERIOD_TYPE = '''+@periodType+'''
and PERIOD_YEAR = '+CAST(@periodYear AS VARCHAR(10))+'
and FISCAL_TYPE = '''+@fiscalType+''''
end

INSERT INTO @tempTable  EXECUTE(@sqlquery)
end

else if @dataSource = 'PRIMARY' OR @dataSource = 'INDUSTRY'
begin 
if @issuerIdsList is not null and @securityIdsList is not null
begin
 set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = '''+@dataSource+'''
and (ISSUER_ID IN ('+@issuerIdsList+')or SECURITY_ID IN ('+@securityIdsList+'))
and PERIOD_TYPE = '''+@periodType+'''
and PERIOD_YEAR = '+CAST(@periodYear AS VARCHAR(10))+'
and FISCAL_TYPE = '''+@fiscalType+''''
end
else if @issuerIdsList is not null
begin
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = '''+@dataSource+'''
and ISSUER_ID IN ('+@issuerIdsList+')
and PERIOD_TYPE = '''+@periodType+'''
and PERIOD_YEAR = '+CAST(@periodYear AS VARCHAR(10))+'
and FISCAL_TYPE = '''+@fiscalType+''''
end

else if @securityIdsList is not null
begin 
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = '''+@dataSource+'''
and SECURITY_ID IN ('+@securityIdsList+')
and PERIOD_TYPE = '''+@periodType+'''
and PERIOD_YEAR = '+CAST(@periodYear AS VARCHAR(10))+'
and FISCAL_TYPE = '''+@fiscalType+''''
end

INSERT INTO @tempTable  EXECUTE(@sqlquery)
end

Select * from @tempTable;
END 
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00055'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



