SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[usp_GetDataForBenchNodefinPeriodYear]
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
