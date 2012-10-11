set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00140'
declare @CurrentScriptVersion as nvarchar(100) = '00141'

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

IF OBJECT_ID ('[dbo].[GetPortfolioDetailsExternalData]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetPortfolioDetailsExternalData]
GO

/****** Object:  StoredProcedure [dbo].[GetPortfolioDetailsExternalData]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPortfolioDetailsExternalData] 
	(
		@issuerIds varchar(max),
		@securityIds varchar(max)
	)
AS
BEGIN
		
	SET NOCOUNT ON;

DECLARE @tempData TABLE
(
IssuerId varchar(20),
SecurityId varchar(20),
DataSource varchar(10),
PeriodType varchar(5),
Currency varchar(5),
DataId int,
Amount Decimal(32,6),
PeriodYear int
)

declare @sqlquery varchar(max)
declare @sqlqueryIssuer varchar(max)

declare @currentYear int =Year(getdate())
declare @nextYear int =Year(getdate())+1

if @securityIds is not null
begin
set @sqlquery = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,PERIOD_TYPE,CURRENCY,
 DATA_ID,AMOUNT,PERIOD_YEAR 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''C''
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@securityIds+'))
and DATA_ID IN (185,187,198,188)'
end

if @issuerIds is not null
    begin
    set @sqlqueryIssuer = 'Select ISSUER_ID,SECURITY_ID,DATA_SOURCE,PERIOD_TYPE,CURRENCY,
 DATA_ID,AMOUNT,PERIOD_YEAR 
from PERIOD_FINANCIALS 
where CURRENCY = ''USD''
and PERIOD_TYPE = ''A''
and FISCAL_TYPE=''CALENDER''
and DATA_SOURCE = ''PRIMARY''
and (ISSUER_ID IN ('+@securityIds+'))
and DATA_ID IN (178,177,133,149,146)'
end
    
    INSERT INTO @tempData  EXECUTE(@sqlquery)
    INSERT INTO @tempData  EXECUTE(@sqlqueryIssuer)   
    
    select * from @tempData
    
    
    
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00141'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

