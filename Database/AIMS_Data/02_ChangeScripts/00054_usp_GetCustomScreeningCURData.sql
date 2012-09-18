set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00053'
declare @CurrentScriptVersion as nvarchar(100) = '00054'

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

/****** Object:  StoredProcedure [dbo].[GetCustomScreeningCURData]    Script Date: 09/18/2012 12:11:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetCustomScreeningCURData]
(
@issuerIdsList varchar(max),
@securityIdsList varchar(max),
@dataID int,
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
and PERIOD_TYPE = ''C'''
Print @sqlquery
end

else if @issuerIdsList is not null
begin
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = ''REUTERS''
and (ISSUER_ID IN ('+@issuerIdsList+')
and PERIOD_TYPE = ''C'''
end

else if @securityIdsList is not null
begin 
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = ''REUTERS''
and SECURITY_ID IN ('+@securityIdsList+')
and PERIOD_TYPE = ''C'''
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
and PERIOD_TYPE = ''C'''
end

else if @issuerIdsList is not null
begin
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = '''+@dataSource+'''
and (ISSUER_ID IN ('+@issuerIdsList+')
and PERIOD_TYPE = ''C'''
end

else if @securityIdsList is not null
begin 
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_ID,AMOUNT,PERIOD_YEAR
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and DATA_ID = '+CAST(@dataID AS VARCHAR(10))+'
and DATA_SOURCE = '''+@dataSource+'''
and SECURITY_ID IN ('+@securityIdsList+')
and PERIOD_TYPE = ''C'''
end

INSERT INTO @tempTable  EXECUTE(@sqlquery)
end

Select * from @tempTable;
END 
GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00054'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



