set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00156'
declare @CurrentScriptVersion as nvarchar(100) = '00157'

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

IF OBJECT_ID ('[dbo].[GetConsensusEstimatesSummaryData]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetConsensusEstimatesSummaryData] 
GO


CREATE PROCEDURE [dbo].[GetConsensusEstimatesSummaryData] @Security varchar(500)
AS

SET FMTONLY OFF

DECLARE @earnings varchar(80),
@currentYear INT,
@currency varchar(10),
@issuerID nvarchar(50),
@xref FLOAT,
@counts INT,
@oneMonthPriorDate DATETIME,
@threeMonthsPriorDate DATETIME,
@sixMonthsPriorDate DATETIME,
@ashYearOne varchar(20),
@ashYearTwo varchar(20),
@ashYearThree varchar(20),
@ashYearFour varchar(20),
@ashYearFive varchar(20),
@conYearOne varchar(20),
@conYearTwo varchar(20),
@conYearThree varchar(20),
@conYearFour varchar(20),
@conYearFive varchar(20),
@diffYearOne varchar(20),
@diffYearTwo varchar(20),
@diffYearThree varchar(20),
@diffYearFour varchar(20),
@diffYearFive varchar(20),
@OneMonthYearOne varchar(20),
@OneMonthYearTwo varchar(20),
@OneMonthYearThree varchar(20),
@OneMonthYearFour varchar(20),
@OneMonthYearFive varchar(20),
@ThreeMonthYearOne varchar(20),
@ThreeMonthYearTwo varchar(20),
@ThreeMonthYearThree varchar(20),
@ThreeMonthYearFour varchar(20),
@ThreeMonthYearFive varchar(20),
@SixMonthYearOne varchar(20),
@SixMonthYearTwo varchar(20),
@SixMonthYearThree varchar(20),
@SixMonthYearFour varchar(20),
@SixMonthYearFive varchar(20)

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
  ISSUER_ID VARCHAR(20) ,
  DATA_SOURCE VARCHAR(10) ,
  PERIOD_YEAR INT ,
  DATA_SOURCE_DATE DATETIME ,
  AMOUNT DECIMAL(32,6)				
)
DECLARE @FINAL_TAB TABLE
(
  iSNo INT IDENTITY(1,1), 
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

SET @xref = (SELECT XREF FROM dbo.GF_SECURITY_BASEVIEW WHERE ASEC_SEC_SHORT_NAME= @Security)

IF @xref IS NOT NULL 
BEGIN

SET @earnings = (SELECT Earnings FROM [Reuters].[dbo].tblCompanyInfo 
WHERE XRef = @xref)

--SELECT @earnings AS EARNINGS

SET @currency = (SELECT Currency FROM [Reuters].[dbo].tblCompanyInfo 
 WHERE XRef  = @xref)

--SELECT @currency AS CURRENCY

SET @issuerID = (SELECT ISSUER_ID FROM dbo.GF_SECURITY_BASEVIEW WHERE ASEC_SEC_SHORT_NAME= @Security)

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

set @ashYearOne =  CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='AshmoreAmount'));
set @ashYearTwo = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='AshmoreAmount'));
set @ashYearThree = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='AshmoreAmount'));
set @ashYearFour = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='AshmoreAmount'));
set @ashYearFive = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='AshmoreAmount'));
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'AshmoreEMM',CASE WHEN CharIndex('.', @ashYearOne) <> 0
THEN SUBSTRING(@ashYearOne, 0, CharIndex('.', @ashYearOne)+2) + '0' ELSE @ashYearOne + '.00' END  ,
CASE WHEN CharIndex('.', @ashYearTwo) <> 0
THEN SUBSTRING(@ashYearTwo, 0, CharIndex('.', @ashYearTwo)+2)+ '0' ELSE @ashYearTwo + '.00' END ,
CASE WHEN CharIndex('.', @ashYearThree) <> 0
THEN SUBSTRING(@ashYearThree, 0, CharIndex('.', @ashYearThree)+2)+ '0' ELSE @ashYearThree + '.00' END ,
CASE WHEN CharIndex('.', @ashYearFour) <> 0
THEN SUBSTRING(@ashYearFour, 0, CharIndex('.', @ashYearFour)+2)+ '0' ELSE @ashYearFour + '.00' END ,
CASE WHEN CharIndex('.', @ashYearFive) <> 0
THEN SUBSTRING(@ashYearFive, 0, CharIndex('.', @ashYearFive)+2)+ '0' ELSE @ashYearFive + '.00' END ,
(SELECT @currency)

set @conYearOne =  CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='EstimateAmount'));
set @conYearTwo = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='EstimateAmount'));
set @conYearThree = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='EstimateAmount'));
set @conYearFour = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='EstimateAmount'));
set @conYearFive = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='EstimateAmount'));
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Median',CASE WHEN CharIndex('.', @conYearOne) <> 0
THEN SUBSTRING(@conYearOne, 0, CharIndex('.', @conYearOne)+2) + '0' ELSE @conYearOne + '.00' END  ,
CASE WHEN CharIndex('.', @conYearTwo) <> 0
THEN SUBSTRING(@conYearTwo, 0, CharIndex('.', @conYearTwo)+2)+ '0' ELSE @conYearTwo + '.00' END ,
CASE WHEN CharIndex('.', @conYearThree) <> 0
THEN SUBSTRING(@conYearThree, 0, CharIndex('.', @conYearThree)+2) + '0'ELSE @conYearThree + '.00' END ,
CASE WHEN CharIndex('.', @conYearFour) <> 0
THEN SUBSTRING(@conYearFour, 0, CharIndex('.', @conYearFour)+2) + '0' ELSE @conYearFour + '.00' END ,
CASE WHEN CharIndex('.', @conYearFive) <> 0
THEN SUBSTRING(@conYearFive, 0, CharIndex('.', @conYearFive)+2)+ '0' ELSE @conYearFive + '.00' END ,
(SELECT @currency)

set @diffYearOne =  CONVERT(VARCHAR(100),((SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
set @diffYearTwo = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
set @diffYearThree = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
set @diffYearFour = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
set @diffYearFive = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='AshmoreAmount')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Difference',CASE WHEN CharIndex('.', @diffYearOne) <> 0
THEN SUBSTRING(@diffYearOne, 0, CharIndex('.', @diffYearOne)+3) + '%' ELSE @diffYearOne + '.00%' END  ,
CASE WHEN CharIndex('.', @diffYearTwo) <> 0
THEN SUBSTRING(@diffYearTwo, 0, CharIndex('.', @diffYearTwo)+3) + '%' ELSE @diffYearTwo + '.00%' END ,
CASE WHEN CharIndex('.', @diffYearThree) <> 0
THEN SUBSTRING(@diffYearThree, 0, CharIndex('.', @diffYearThree)+3) + '%' ELSE @diffYearThree + '.00%' END ,
CASE WHEN CharIndex('.', @diffYearFour) <> 0
THEN SUBSTRING(@diffYearFour, 0, CharIndex('.', @diffYearFour)+3) + '%' ELSE @diffYearFour + '.00%' END ,
CASE WHEN CharIndex('.', @diffYearFive) <> 0
THEN SUBSTRING(@diffYearFive, 0, CharIndex('.', @diffYearFive)+3) + '%' ELSE @diffYearFive + '.00%' END ,
(SELECT @currency)

set @OneMonthYearOne =  CONVERT(VARCHAR(100),((SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
set @OneMonthYearTwo = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
set @OneMonthYearThree = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
set @OneMonthYearFour = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
set @OneMonthYearFive = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 1 Month',CASE WHEN CharIndex('.', @OneMonthYearOne) <> 0
THEN SUBSTRING(@OneMonthYearOne, 0, CharIndex('.', @OneMonthYearOne)+3) + '%' ELSE @OneMonthYearOne + '.00%' END ,CASE WHEN CharIndex('.', @OneMonthYearTwo) <> 0
THEN SUBSTRING(@OneMonthYearTwo, 0, CharIndex('.', @OneMonthYearTwo)+3) + '%' ELSE @OneMonthYearTwo + '.00%' END  ,
CASE WHEN CharIndex('.', @OneMonthYearThree) <> 0
THEN SUBSTRING(@OneMonthYearThree, 0, CharIndex('.', @OneMonthYearThree)+3) + '%' ELSE @OneMonthYearThree + '.00%' END ,
CASE WHEN CharIndex('.', @OneMonthYearFour) <> 0
THEN SUBSTRING(@OneMonthYearFour, 0, CharIndex('.', @OneMonthYearFour)+3) + '%' ELSE @OneMonthYearFour + '.00%' END ,
CASE WHEN CharIndex('.', @OneMonthYearFive) <> 0
THEN SUBSTRING(@OneMonthYearFive, 0, CharIndex('.', @OneMonthYearFive)+3) + '%' ELSE @OneMonthYearFive + '.00%' END ,
(SELECT @currency)

set @ThreeMonthYearOne =  CONVERT(VARCHAR(100),((SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
set @ThreeMonthYearTwo = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
set @ThreeMonthYearThree = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
set @ThreeMonthYearFour = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
set @ThreeMonthYearFive = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 3 Month',CASE WHEN CharIndex('.', @ThreeMonthYearOne) <> 0
THEN SUBSTRING(@ThreeMonthYearOne, 0, CharIndex('.', @ThreeMonthYearOne)+3) + '%' ELSE @ThreeMonthYearOne + '.00%' END ,CASE WHEN CharIndex('.', @ThreeMonthYearTwo) <> 0
THEN SUBSTRING(@ThreeMonthYearTwo, 0, CharIndex('.', @ThreeMonthYearTwo)+3) + '%' ELSE @ThreeMonthYearTwo + '.00%' END  ,
CASE WHEN CharIndex('.', @ThreeMonthYearThree) <> 0
THEN SUBSTRING(@ThreeMonthYearThree, 0, CharIndex('.', @ThreeMonthYearThree)+3) + '%' ELSE @ThreeMonthYearThree + '.00%' END ,
CASE WHEN CharIndex('.', @ThreeMonthYearFour) <> 0
THEN SUBSTRING(@ThreeMonthYearFour, 0, CharIndex('.', @ThreeMonthYearFour)+3) + '%' ELSE @ThreeMonthYearFour + '.00%' END ,
CASE WHEN CharIndex('.', @ThreeMonthYearFive) <> 0
THEN SUBSTRING(@ThreeMonthYearFive, 0, CharIndex('.', @ThreeMonthYearFive)+3) + '%' ELSE @ThreeMonthYearFive + '.00%' END ,
(SELECT @currency)

set @SixMonthYearOne =  CONVERT(VARCHAR(100),((SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
set @SixMonthYearTwo = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
set @SixMonthYearThree = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
set @SixMonthYearFour = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
set @SixMonthYearFive = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 6 Month',CASE WHEN CharIndex('.', @SixMonthYearOne) <> 0
THEN SUBSTRING(@SixMonthYearOne, 0, CharIndex('.', @SixMonthYearOne)+3) + '%' ELSE @SixMonthYearOne + '.00%' END ,CASE WHEN CharIndex('.', @SixMonthYearTwo) <> 0
THEN SUBSTRING(@SixMonthYearTwo, 0, CharIndex('.', @SixMonthYearTwo)+3) + '%' ELSE @SixMonthYearTwo + '.00%' END  ,
CASE WHEN CharIndex('.', @SixMonthYearThree) <> 0
THEN SUBSTRING(@SixMonthYearThree, 0, CharIndex('.', @SixMonthYearThree)+3) + '%' ELSE @SixMonthYearThree + '.00%' END ,
CASE WHEN CharIndex('.', @SixMonthYearFour) <> 0
THEN SUBSTRING(@SixMonthYearFour, 0, CharIndex('.', @SixMonthYearFour)+3) + '%' ELSE @SixMonthYearFour + '.00%' END ,
CASE WHEN CharIndex('.', @SixMonthYearFive) <> 0
THEN SUBSTRING(@SixMonthYearFive, 0, CharIndex('.', @SixMonthYearFive)+3) + '%' ELSE @SixMonthYearFive + '.00%' END ,
(SELECT @currency)


INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Last Update (Consensus)',(SELECT TOP 1  CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='SourceDate'),
(SELECT  TOP 1 CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='SourceDate'),
(SELECT TOP 1  CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='SourceDate'),
(SELECT  TOP 1 CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='SourceDate'),
(SELECT TOP 1  CONVERT(VARCHAR(100),DATE_SOURCE_DATE,23) FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='SourceDate'),
(SELECT @currency)

SELECT * FROM @ASHMORE_FINAL_TAB
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00157'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


