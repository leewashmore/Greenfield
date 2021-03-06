SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[GetCustomScreeningCURData]
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
