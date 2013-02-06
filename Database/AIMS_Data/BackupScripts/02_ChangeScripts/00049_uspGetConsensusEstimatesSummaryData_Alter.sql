set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00048'
declare @CurrentScriptVersion as nvarchar(100) = '00049'

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

IF OBJECT_ID ('[dbo].[usp_GetConsensusEstimatesSummaryData]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_GetConsensusEstimatesSummaryData]
GO

ALTER PROCEDURE [dbo].[GetConsensusEstimatesSummaryData] @Security varchar(500)
AS

SET FMTONLY OFF

DECLARE @earnings varchar(80),
@currentYear INT,
@currency varchar(80),
@issuerID DECIMAL,
@xref FLOAT,
@counts INT,
@oneMonthPriorDate DATETIME,
@threeMonthsPriorDate DATETIME,
@sixMonthsPriorDate DATETIME

DECLARE  @TEMP_CON_CHANGE_TAB TABLE 
(
 
 OriginalDate DateTime,
 ExpirationDate DateTime,
 FiscalYear VARCHAR(80),
 Median REAL
 )
DECLARE @TEMP_ESTIMATE_TAB TABLE 
(  
  ISSUER_ID VARCHAR(80),
  DATA_SOURCE VARCHAR(80),
  PERIOD_YEAR INT ,
  DATA_SOURCE_DATE DATETIME,
  AMOUNT DECIMAL(32,6)				
)
DECLARE @TEMP_ASHMORE_TAB TABLE 
( 
  ISSUER_ID VARCHAR(80),
  DATA_SOURCE VARCHAR(80),
  PERIOD_YEAR INT ,
  DATA_SOURCE_DATE DATETIME,
  AMOUNT DECIMAL(32,6)				
)
DECLARE @FINAL_TAB TABLE
(
  FISCAL_YEAR VARCHAR(80) ,  
  MEDIAN REAL,
  MTYPE VARCHAR(80),
  DATE_SOURCE_DATE DATETIME	
)
DECLARE @ASHMORE_FINAL_TAB TABLE
(
NetIncome VARCHAR(100),
YEAR1 VARCHAR(100),
YEAR2 VARCHAR(100),
YEAR3 VARCHAR(100),
YEAR4 VARCHAR(100),
YEAR5 VARCHAR(100),
currency varchar(80)
)

BEGIN

SET @xref = (SELECT XREF FROM dbo.GF_SECURITY_BASEVIEW WHERE ISSUE_NAME = @Security)

IF @xref IS NOT NULL 
BEGIN

SET @earnings = (SELECT Earnings FROM [Reuters].[dbo].tblCompanyInfo 
WHERE XRef = @xref)

--SELECT @earnings AS EARNINGS

SET @currency = (SELECT Currency FROM [Reuters].[dbo].tblCompanyInfo 
 WHERE XRef  = @xref)

--SELECT @currency AS CURRENCY

SET @issuerID = (SELECT ISSUER_ID FROM dbo.GF_SECURITY_BASEVIEW WHERE ISSUE_NAME = @Security)

--SELECT @issuerID AS ISSUERID

SELECT @counts =  COUNT(*)  
FROM dbo.CURRENT_CONSENSUS_ESTIMATES
WHERE ISSUER_ID = @issuerID AND DATA_SOURCE = 'REUTERS' AND PERIOD_TYPE = 'A' AND FISCAL_TYPE = 'FISCAL' 
AND CURRENCY = @currency AND CURRENCY = SOURCE_CURRENCY AND ESTIMATE_ID =  11 AND AMOUNT_TYPE = 'ESTIMATE'

IF @counts = 0
SET @currency = 'USD'

IF @earnings ='EPS'
BEGIN
INSERT INTO @TEMP_ESTIMATE_TAB(ISSUER_ID,DATA_SOURCE,PERIOD_YEAR,DATA_SOURCE_DATE,AMOUNT)
SELECT ISSUER_ID,DATA_SOURCE,PERIOD_YEAR,DATA_SOURCE_DATE,AMOUNT
FROM dbo.CURRENT_CONSENSUS_ESTIMATES
WHERE ISSUER_ID = @issuerID AND DATA_SOURCE = 'REUTERS' AND PERIOD_TYPE = 'A' AND FISCAL_TYPE = 'FISCAL' 
AND CURRENCY = @currency AND CURRENCY = SOURCE_CURRENCY AND ESTIMATE_ID =  11 AND AMOUNT_TYPE = 'ESTIMATE'
END

ELSE IF @earnings = 'EPSREP'
BEGIN
INSERT INTO @TEMP_ESTIMATE_TAB(ISSUER_ID,DATA_SOURCE,PERIOD_YEAR,DATA_SOURCE_DATE,AMOUNT)
SELECT ISSUER_ID,DATA_SOURCE,PERIOD_YEAR,DATA_SOURCE_DATE,AMOUNT
FROM dbo.CURRENT_CONSENSUS_ESTIMATES
WHERE ISSUER_ID = @issuerID AND DATA_SOURCE = 'REUTERS' AND PERIOD_TYPE = 'A' AND FISCAL_TYPE = 'FISCAL' 
AND CURRENCY = @currency AND CURRENCY = SOURCE_CURRENCY AND ESTIMATE_ID =  13 AND AMOUNT_TYPE = 'ESTIMATE'
END 

ELSE IF @earnings = 'EBG'
BEGIN
INSERT INTO @TEMP_ESTIMATE_TAB(ISSUER_ID,DATA_SOURCE,PERIOD_YEAR,DATA_SOURCE_DATE,AMOUNT)
SELECT ISSUER_ID,DATA_SOURCE,PERIOD_YEAR,DATA_SOURCE_DATE,AMOUNT
FROM dbo.CURRENT_CONSENSUS_ESTIMATES
WHERE ISSUER_ID = @issuerID AND DATA_SOURCE = 'REUTERS' AND PERIOD_TYPE = 'A' AND FISCAL_TYPE = 'FISCAL' 
AND CURRENCY = @currency AND CURRENCY = SOURCE_CURRENCY AND ESTIMATE_ID =  12 AND AMOUNT_TYPE = 'ESTIMATE'
END

--select * from @TEMP_ESTIMATE_TAB

INSERT INTO @TEMP_ASHMORE_TAB(ISSUER_ID,DATA_SOURCE,PERIOD_YEAR,DATA_SOURCE_DATE,AMOUNT)
SELECT ISSUER_ID,DATA_SOURCE,PERIOD_YEAR,ROOT_SOURCE_DATE,AMOUNT
FROM dbo.PERIOD_FINANCIALS
where ISSUER_ID = @issuerID  AND DATA_SOURCE = 'PRIMARY' AND  PERIOD_TYPE = 'A' AND FISCAL_TYPE = 'FISCAL' and CURRENCY = @currency  and DATA_ID = 44

--SELECT * FROM @TEMP_ASHMORE_TAB

IF @earnings ='EPS'
BEGIN
INSERT INTO @TEMP_CON_CHANGE_TAB(OriginalDate,ExpirationDate,Median,FiscalYear)
(select ce.OriginalDate,ce.ExpirationDate,ce.Median,LEFT(ce.fYearEnd,4) 
From [Reuters].[dbo].tblConsensusEstimate ce
-- use this join to limit the record set to only the most recent rows.
inner join (select XRef, PeriodEndDate, fYearEnd, EstimateType, MAX(StartDate) as StartDate
from [Reuters].[dbo].tblConsensusEstimate
group by XRef, PeriodEndDate, fYearEnd, EstimateType) a
on a.XRef = ce.XRef and a.PeriodEndDate = ce.PeriodEndDate and a.fYearEnd = ce.fYearEnd
and a.EstimateType = ce.EstimateType and a.StartDate = ce.StartDate
and ce.XRef = @xref 
and ce.EstimateType = 'NTP' 
and ce.PeriodType = 'A')
END

IF @earnings ='EPSREP'
BEGIN
INSERT INTO @TEMP_CON_CHANGE_TAB(OriginalDate,ExpirationDate,Median,FiscalYear)
(select ce.OriginalDate,ce.ExpirationDate,ce.Median,LEFT(ce.fYearEnd,4) 
From [Reuters].[dbo].tblConsensusEstimate ce
-- use this join to limit the record set to only the most recent rows.
inner join (select XRef, PeriodEndDate, fYearEnd, EstimateType, MAX(StartDate) as StartDate
from [Reuters].[dbo].tblConsensusEstimate
group by XRef, PeriodEndDate, fYearEnd, EstimateType) a
on a.XRef = ce.XRef and a.PeriodEndDate = ce.PeriodEndDate and a.fYearEnd = ce.fYearEnd
and a.EstimateType = ce.EstimateType and a.StartDate = ce.StartDate
and ce.XRef = @xref 
and ce.EstimateType = 'NTPREP' 
and ce.PeriodType = 'A') 
END

IF @earnings ='EBG'
BEGIN
INSERT INTO @TEMP_CON_CHANGE_TAB(OriginalDate,ExpirationDate,Median,FiscalYear)
(select ce.OriginalDate,ce.ExpirationDate,ce.Median,LEFT(ce.fYearEnd,4) 
From [Reuters].[dbo].tblConsensusEstimate ce
-- use this join to limit the record set to only the most recent rows.
inner join (select XRef, PeriodEndDate, fYearEnd, EstimateType, MAX(StartDate) as StartDate
from [Reuters].[dbo].tblConsensusEstimate
group by XRef, PeriodEndDate, fYearEnd, EstimateType) a
on a.XRef = ce.XRef and a.PeriodEndDate = ce.PeriodEndDate and a.fYearEnd = ce.fYearEnd
and a.EstimateType = ce.EstimateType and a.StartDate = ce.StartDate
and ce.XRef = @xref 
and ce.EstimateType = 'NTPBG' 
and ce.PeriodType = 'A')
END

--SELECT * FROM @TEMP_CON_CHANGE_TAB
SET @currentYear = (select datepart(yyyy,getdate()))
SET @oneMonthPriorDate = (select dateadd(mm,-1,getdate()))
SET @threeMonthsPriorDate = (select dateadd(mm,-3,getdate()))
SET @sixMonthsPriorDate = (select dateadd(mm,-6,getdate()))

--SELECT @oneMonthPriorDate,@threeMonthsPriorDate,@sixMonthsPriorDate

INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT FiscalYear,Median,'nCurrent' FROM @TEMP_CON_CHANGE_TAB WHERE
ExpirationDate is NULL AND (FiscalYear >= @currentYear-1 AND FiscalYear <= @currentYear + 4)

INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT FiscalYear,Median,'n1Month' FROM @TEMP_CON_CHANGE_TAB WHERE
 (@oneMonthPriorDate BETWEEN OriginalDate AND ExpirationDate) AND (FiscalYear >= @currentYear-1 AND FiscalYear <= @currentYear + 4)
 
 INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT FiscalYear,Median,'n3Month' FROM @TEMP_CON_CHANGE_TAB WHERE
 (@threeMonthsPriorDate BETWEEN OriginalDate AND ExpirationDate) AND (FiscalYear >= @currentYear-1 AND FiscalYear <= @currentYear + 4)
 
  INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT FiscalYear,Median,'n6Month' FROM @TEMP_CON_CHANGE_TAB WHERE
 (@sixMonthsPriorDate BETWEEN OriginalDate AND ExpirationDate) AND (FiscalYear >= @currentYear-1 AND FiscalYear <= @currentYear + 4)
 
INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT PERIOD_YEAR,AMOUNT,'AshmoreAmount' FROM @TEMP_ASHMORE_TAB 
WHERE  PERIOD_YEAR >= @currentYear-1 AND PERIOD_YEAR <= @currentYear + 4

INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT PERIOD_YEAR,AMOUNT,'EstimateAmount' FROM @TEMP_ESTIMATE_TAB 
WHERE  PERIOD_YEAR >= @currentYear-1 AND PERIOD_YEAR <= @currentYear + 4

INSERT INTO @FINAL_TAB(FISCAL_YEAR,DATE_SOURCE_DATE,MTYPE) 
SELECT PERIOD_YEAR,DATA_SOURCE_DATE,'SourceDate' FROM @TEMP_ESTIMATE_TAB 
WHERE  PERIOD_YEAR >= @currentYear-1 AND PERIOD_YEAR <= @currentYear + 4


END

--SELECT * FROM @FINAL_TAB

INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'AshmoreEMM',(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='AshmoreAmount'),
(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='AshmoreAmount'),
(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='AshmoreAmount'),
(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='AshmoreAmount'),
(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='AshmoreAmount'),
(SELECT @currency)

INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Median',(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='EstimateAmount'),
(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='EstimateAmount'),
(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='EstimateAmount'),
(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='EstimateAmount'),
(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='EstimateAmount'),
(SELECT @currency)

INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Difference',((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='EstimateAmount') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='EstimateAmount') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='EstimateAmount') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='EstimateAmount') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='AshmoreAmount')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='EstimateAmount') - 1),
(SELECT @currency)

INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 1 Month',((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n1Month') - 1),
((SELECT  TOP 1 MEDIAN  FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n1Month') - 1),
((SELECT  TOP 1 MEDIAN  FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n1Month') - 1),
((SELECT  TOP 1 MEDIAN  FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n1Month') - 1),
((SELECT TOP 1  MEDIAN  FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n1Month') - 1),
(SELECT @currency)

INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 3 Month',((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN  FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n3Month') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n3Month') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n3Month') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n3Month') - 1),
((SELECT   TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT TOP 1  MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n3Month') - 1),
(SELECT @currency)

INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 6 Month',((SELECT  TOP 1  MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n6Month') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n6Month') - 1),
((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n6Month') - 1),
((SELECT   TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1  MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n6Month') - 1),
((SELECT  TOP 1  MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT  TOP 1  MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n6Month') - 1),
(SELECT @currency)

INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Last Update (Consensus)',(SELECT  TOP 1 CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='SourceDate'),
(SELECT  TOP 1 CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='SourceDate'),
(SELECT  TOP 1 CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='SourceDate'),
(SELECT  TOP 1 CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='SourceDate'),
(SELECT  TOP 1 CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='SourceDate'),
(SELECT @currency)

SELECT * FROM @ASHMORE_FINAL_TAB
END

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00049'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

