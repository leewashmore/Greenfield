set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00031'
declare @CurrentScriptVersion as nvarchar(100) = '00032'

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

IF OBJECT_ID ('[dbo].[usp_GetDataForValuationQualityGrowth]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_GetDataForValuationQualityGrowth]
GO

CREATE PROCEDURE [dbo].[usp_GetDataForValuationQualityGrowth]
(
@issuerIdsForPortfolio varchar(max),
@securityIdsForPortfolio varchar(max),
@issuerIdsForBenchmark varchar(max),
@securityIdsForBenchmark varchar(max)
)
AS

SET FMTONLY OFF

BEGIN

DECLARE @tempPorfolio TABLE
(
IssuerId varchar(20),
SecurityId varchar(20),
DataSource varchar(10),
PeriodType varchar(5),
Currency varchar(5),
DataId int,
Amount Decimal(32,6),
AmountType varchar(15)
)

DECLARE @tempBenchmark TABLE
(
IssuerId varchar(20),
SecurityId varchar(20),
DataSource varchar(10),
PeriodType varchar(5),
Currency varchar(5),
DataId int,
Amount Decimal(32,6),
AmountType varchar(15)
)

declare @sqlquery varchar(max)
declare @sqlqueryforBenchmark varchar(max)
--declare @IssuerIdPortfolio Varchar(MAX) = '''223340'',''99'''
--select @IssuerIdPortfolio = (SUBSTRING(@IssuerIdPortfolio,1,9))
--select @IssuerIdPortfolio
if @issuerIdsForPortfolio is not null and @securityIdsForPortfolio is not null
begin
 set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,PERIOD_TYPE,CURRENCY,
 DATA_ID,AMOUNT,''Portfolio'' 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@issuerIdsForPortfolio+')or SECURITY_ID IN ('+@securityIdsForPortfolio+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

else if @issuerIdsForPortfolio is not null
begin
set @sqlquery  = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,PERIOD_TYPE,CURRENCY,
 DATA_ID,AMOUNT,''Portfolio'' 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@issuerIdsForPortfolio+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

else if @securityIdsForPortfolio is not null
begin 
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,PERIOD_TYPE,CURRENCY,
 DATA_ID,AMOUNT,''Portfolio'' 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (SECURITY_ID IN ('+@securityIdsForPortfolio+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

INSERT INTO @tempPorfolio  EXECUTE(@sqlquery)

--SELECT * FROM @tempPorfolio

if @issuerIdsForBenchmark is not null and @securityIdsForBenchmark is not null
begin
set @sqlqueryforBenchmark = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,PERIOD_TYPE,CURRENCY,
DATA_ID,AMOUNT,''Benchmark'' 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@issuerIdsForBenchmark+') or SECURITY_ID IN ('+@securityIdsForBenchmark+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

else if @issuerIdsForBenchmark is not null
begin
set @sqlqueryforBenchmark = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,PERIOD_TYPE,CURRENCY,
DATA_ID,AMOUNT,''Benchmark'' 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@issuerIdsForBenchmark+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

else if @securityIdsForBenchmark is not null
begin 
set @sqlqueryforBenchmark = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,PERIOD_TYPE,CURRENCY,
DATA_ID,AMOUNT,''Benchmark'' 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (SECURITY_ID IN ('+@securityIdsForBenchmark+'))
and DATA_ID IN (197, 198, 187, 189, 188, 200, 236, 201, 202)'
end

INSERT INTO @tempBenchmark  EXECUTE(@sqlqueryforBenchmark)
--SELECT * FROM @tempBenchmark

select * from @tempBenchmark
union
select * from @tempPorfolio
END 

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00032'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

