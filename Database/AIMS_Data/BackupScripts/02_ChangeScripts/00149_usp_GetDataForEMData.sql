set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00148'
declare @CurrentScriptVersion as nvarchar(100) = '00149'

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

IF OBJECT_ID ('[dbo].[usp_GetDataForEMData]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_GetDataForEMData]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_GetDataForEMData]
(
@securityIdsList varchar(max),
@yearList varchar(40)
)
AS

SET FMTONLY OFF

BEGIN

DECLARE 
@currentYear INT


DECLARE @tempCurrentYear TABLE
(
IssuerId varchar(20),
SecurityId varchar(20),
DataSource varchar(10),
DataId int,
Amount Decimal(32,6),
DataYear int,
DataType varchar(10) 
)

DECLARE @tempConsensusYear TABLE
(
IssuerId varchar(20),
SecurityId varchar(20),
DataSource varchar(10),
DataId int,
Amount Decimal(32,6),
DataYear int,
DataType varchar(10)
)

declare @sqlquery varchar(max)
declare @sqlqueryforConsensus varchar(max)

SET @currentYear = (select datepart(yyyy,getdate()))
if @securityIdsList is not null 
begin
 set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,DATA_ID,AMOUNT,PERIOD_YEAR,''W''
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''A''
AND (PERIOD_YEAR IN ('+@yearList+'))
and FISCAL_TYPE = ''CALENDAR''
and DATA_SOURCE = ''PRIMARY''
and (SECURITY_ID IN ('+@securityIdsList+'))
and DATA_ID IN (166, 164, 133, 192, 177)'

set @sqlqueryforConsensus  = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,DATA_ID,AMOUNT,PERIOD_YEAR,''C'' 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''A''
and (PERIOD_YEAR IN ('+@yearList+'))
and FISCAL_TYPE = ''CALENDAR''
and DATA_SOURCE = ''REUTERS''
and (SECURITY_ID IN ('+@securityIdsList+'))
and DATA_ID IN (166, 164, 133, 192, 177)'
end

print @sqlquery
print @sqlqueryforConsensus

INSERT INTO @tempCurrentYear  EXECUTE(@sqlquery)
INSERT INTO @tempConsensusYear  EXECUTE(@sqlqueryforConsensus)
--SELECT * FROM @tempBenchmark

select * from @tempCurrentYear
union
select * from @tempConsensusYear
END 

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00149'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


