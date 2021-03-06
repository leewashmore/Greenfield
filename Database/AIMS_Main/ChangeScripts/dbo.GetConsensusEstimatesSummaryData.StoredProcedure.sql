/****** Object:  StoredProcedure [dbo].[GetConsensusEstimatesSummaryData]    Script Date: 07/18/2013 10:54:13 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetConsensusEstimatesSummaryData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetConsensusEstimatesSummaryData]
GO

/****** Object:  StoredProcedure [dbo].[GetConsensusEstimatesSummaryData]    Script Date: 07/18/2013 10:54:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE procedure [dbo].[GetConsensusEstimatesSummaryData] @Security varchar(500)
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

SET @earnings = (SELECT Earnings FROM [AIMS_Reuters].[dbo].tblCompanyInfo 
WHERE XRef = @xref)

--SELECT @earnings AS EARNINGS

SET @currency = (SELECT Currency FROM [AIMS_Reuters].[dbo].tblCompanyInfo 
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

SET @currentYear = (select datepart(yyyy,getdate()))
SET @oneMonthPriorDate = (select dateadd(mm,-1,getdate()))
SET @threeMonthsPriorDate = (select dateadd(mm,-3,getdate()))
SET @sixMonthsPriorDate = (select dateadd(mm,-6,getdate()))

--SELECT @oneMonthPriorDate as one,@threeMonthsPriorDate as three,@sixMonthsPriorDate as six

IF @earnings ='EPS'
BEGIN
INSERT INTO @TEMP_CON_CHANGE_TAB(OriginalDate,ExpirationDate,Median,FiscalYear)
(select ce.OriginalDate,ce.ExpirationDate,ce.Median,LEFT(ce.fYearEnd,4)
From [AIMS_Reuters].[dbo].tblConsensusEstimate ce
where ce.XRef = @xref 
and ce.EstimateType = 'NTP' 
and ce.PeriodType = 'A'
and (@oneMonthPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')
or @threeMonthsPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')
or @sixMonthsPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')))
END

IF @earnings ='EPSREP'
BEGIN
INSERT INTO @TEMP_CON_CHANGE_TAB(OriginalDate,ExpirationDate,Median,FiscalYear)
(select ce.OriginalDate,ce.ExpirationDate,ce.Median,LEFT(ce.fYearEnd,4)
From [AIMS_Reuters].[dbo].tblConsensusEstimate ce
where ce.XRef = @xref 
and ce.EstimateType = 'NTPREP' 
and ce.PeriodType = 'A'
and (@oneMonthPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')
or @threeMonthsPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')
or @sixMonthsPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')))
END

IF @earnings ='EBG'
BEGIN
INSERT INTO @TEMP_CON_CHANGE_TAB(OriginalDate,ExpirationDate,Median,FiscalYear)
(select ce.OriginalDate,ce.ExpirationDate,ce.Median,LEFT(ce.fYearEnd,4)
From [AIMS_Reuters].[dbo].tblConsensusEstimate ce
where ce.XRef = @xref 
and ce.EstimateType = 'NTPBG' 
and ce.PeriodType = 'A'
and (@oneMonthPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')
or @threeMonthsPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')
or @sixMonthsPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')))
END

--SELECT * FROM @TEMP_CON_CHANGE_TAB

INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT FiscalYear,Median,'nCurrent' FROM @TEMP_CON_CHANGE_TAB WHERE
ExpirationDate is NULL AND (FiscalYear >= @currentYear-1 AND FiscalYear <= @currentYear + 4)

INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT FiscalYear,Median,'n1Month' FROM @TEMP_CON_CHANGE_TAB WHERE
 (@oneMonthPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')) AND (FiscalYear >= @currentYear-1 AND FiscalYear <= @currentYear + 4)

INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT FiscalYear,Median,'n3Month' FROM @TEMP_CON_CHANGE_TAB WHERE
(@threeMonthsPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')) AND (FiscalYear >= @currentYear-1 AND FiscalYear <= @currentYear + 4)

INSERT INTO @FINAL_TAB(FISCAL_YEAR,MEDIAN,MTYPE) 
SELECT FiscalYear,Median,'n6Month' FROM @TEMP_CON_CHANGE_TAB WHERE
 (@sixMonthsPriorDate BETWEEN OriginalDate AND ISNULL(ExpirationDate, '9999-12-31')) AND (FiscalYear >= @currentYear-1 AND FiscalYear <= @currentYear + 4)
 
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

-------------------------------------------------
--Below here is compiling data into display ready
--------------------------------------------------
set @ashYearOne =  CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='AshmoreAmount'));
set @ashYearTwo = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='AshmoreAmount'));
set @ashYearThree = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='AshmoreAmount'));
set @ashYearFour = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='AshmoreAmount'));
set @ashYearFive = CONVERT(VARCHAR(100),(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='AshmoreAmount'));
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'AEIM',CASE WHEN CharIndex('.', @ashYearOne) <> 0
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
THEN SUBSTRING(@conYearOne, 0, CharIndex('.', @conYearOne)+2) + '0' ELSE @conYearOne + '.0' END  ,
CASE WHEN CharIndex('.', @conYearTwo) <> 0
THEN SUBSTRING(@conYearTwo, 0, CharIndex('.', @conYearTwo)+2)+ '0' ELSE @conYearTwo + '.0' END ,
CASE WHEN CharIndex('.', @conYearThree) <> 0
THEN SUBSTRING(@conYearThree, 0, CharIndex('.', @conYearThree)+2) + '0'ELSE @conYearThree + '.0' END ,
CASE WHEN CharIndex('.', @conYearFour) <> 0
THEN SUBSTRING(@conYearFour, 0, CharIndex('.', @conYearFour)+2) + '0' ELSE @conYearFour + '.0' END ,
CASE WHEN CharIndex('.', @conYearFive) <> 0
THEN SUBSTRING(@conYearFive, 0, CharIndex('.', @conYearFive)+2)+ '0' ELSE @conYearFive + '.0' END ,
(SELECT @currency)

set @diffYearOne =  CONVERT(VARCHAR(100),((SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
set @diffYearTwo = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
set @diffYearThree = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
set @diffYearFour = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='AshmoreAmount')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
set @diffYearFive = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='AshmoreAmount')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='EstimateAmount' AND MEDIAN <> 0) - 1)*100);
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Difference',CASE WHEN CharIndex('.', @diffYearOne) <> 0
THEN SUBSTRING(@diffYearOne, 0, CharIndex('.', @diffYearOne)+2) + '%' ELSE @diffYearOne + '.0%' END  ,
CASE WHEN CharIndex('.', @diffYearTwo) <> 0
THEN SUBSTRING(@diffYearTwo, 0, CharIndex('.', @diffYearTwo)+2) + '%' ELSE @diffYearTwo + '.0%' END ,
CASE WHEN CharIndex('.', @diffYearThree) <> 0
THEN SUBSTRING(@diffYearThree, 0, CharIndex('.', @diffYearThree)+2) + '%' ELSE @diffYearThree + '.0%' END ,
CASE WHEN CharIndex('.', @diffYearFour) <> 0
THEN SUBSTRING(@diffYearFour, 0, CharIndex('.', @diffYearFour)+2) + '%' ELSE @diffYearFour + '.0%' END ,
CASE WHEN CharIndex('.', @diffYearFive) <> 0
THEN SUBSTRING(@diffYearFive, 0, CharIndex('.', @diffYearFive)+2) + '%' ELSE @diffYearFive + '.0%' END ,
(SELECT @currency)

set @OneMonthYearOne =  CONVERT(VARCHAR(100),((SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
set @OneMonthYearTwo = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
set @OneMonthYearThree = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
set @OneMonthYearFour = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
set @OneMonthYearFive = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n1Month' AND MEDIAN <> 0) - 1)*100);
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 1 Month',CASE WHEN CharIndex('.', @OneMonthYearOne) <> 0
THEN SUBSTRING(@OneMonthYearOne, 0, CharIndex('.', @OneMonthYearOne)+2) + '%' ELSE @OneMonthYearOne + '.0%' END ,CASE WHEN CharIndex('.', @OneMonthYearTwo) <> 0
THEN SUBSTRING(@OneMonthYearTwo, 0, CharIndex('.', @OneMonthYearTwo)+2) + '%' ELSE @OneMonthYearTwo + '.0%' END  ,
CASE WHEN CharIndex('.', @OneMonthYearThree) <> 0
THEN SUBSTRING(@OneMonthYearThree, 0, CharIndex('.', @OneMonthYearThree)+2) + '%' ELSE @OneMonthYearThree + '.0%' END ,
CASE WHEN CharIndex('.', @OneMonthYearFour) <> 0
THEN SUBSTRING(@OneMonthYearFour, 0, CharIndex('.', @OneMonthYearFour)+2) + '%' ELSE @OneMonthYearFour + '.0%' END ,
CASE WHEN CharIndex('.', @OneMonthYearFive) <> 0
THEN SUBSTRING(@OneMonthYearFive, 0, CharIndex('.', @OneMonthYearFive)+2) + '%' ELSE @OneMonthYearFive + '.0%' END ,
(SELECT @currency)

set @ThreeMonthYearOne =  CONVERT(VARCHAR(100),((SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
set @ThreeMonthYearTwo = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
set @ThreeMonthYearThree = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
set @ThreeMonthYearFour = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
set @ThreeMonthYearFive = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n3Month' AND MEDIAN <> 0) - 1)*100);
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 3 Month',CASE WHEN CharIndex('.', @ThreeMonthYearOne) <> 0
THEN SUBSTRING(@ThreeMonthYearOne, 0, CharIndex('.', @ThreeMonthYearOne)+2) + '%' ELSE @ThreeMonthYearOne + '.0%' END ,CASE WHEN CharIndex('.', @ThreeMonthYearTwo) <> 0
THEN SUBSTRING(@ThreeMonthYearTwo, 0, CharIndex('.', @ThreeMonthYearTwo)+2) + '%' ELSE @ThreeMonthYearTwo + '.0%' END  ,
CASE WHEN CharIndex('.', @ThreeMonthYearThree) <> 0
THEN SUBSTRING(@ThreeMonthYearThree, 0, CharIndex('.', @ThreeMonthYearThree)+2) + '%' ELSE @ThreeMonthYearThree + '.0%' END ,
CASE WHEN CharIndex('.', @ThreeMonthYearFour) <> 0
THEN SUBSTRING(@ThreeMonthYearFour, 0, CharIndex('.', @ThreeMonthYearFour)+2) + '%' ELSE @ThreeMonthYearFour + '.0%' END ,
CASE WHEN CharIndex('.', @ThreeMonthYearFive) <> 0
THEN SUBSTRING(@ThreeMonthYearFive, 0, CharIndex('.', @ThreeMonthYearFive)+2) + '%' ELSE @ThreeMonthYearFive + '.0%' END ,
(SELECT @currency)

set @SixMonthYearOne =  CONVERT(VARCHAR(100),((SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear-1 AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
set @SixMonthYearTwo = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
set @SixMonthYearThree = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+1 AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
set @SixMonthYearFour = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='nCurrent')/(SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+2 AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
set @SixMonthYearFive = CONVERT(VARCHAR(100),((SELECT  TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='nCurrent')/(SELECT TOP 1 MEDIAN FROM @FINAL_TAB WHERE FISCAL_YEAR = @currentYear+3 AND MTYPE ='n6Month' AND MEDIAN <> 0) - 1)*100);
INSERT INTO @ASHMORE_FINAL_TAB(NetIncome,YEAR1,YEAR2,YEAR3,YEAR4,YEAR5,currency)
SELECT 'Consensus Change – 6 Month',CASE WHEN CharIndex('.', @SixMonthYearOne) <> 0
THEN SUBSTRING(@SixMonthYearOne, 0, CharIndex('.', @SixMonthYearOne)+2) + '%' ELSE @SixMonthYearOne + '.0%' END ,CASE WHEN CharIndex('.', @SixMonthYearTwo) <> 0
THEN SUBSTRING(@SixMonthYearTwo, 0, CharIndex('.', @SixMonthYearTwo)+2) + '%' ELSE @SixMonthYearTwo + '.0%' END  ,
CASE WHEN CharIndex('.', @SixMonthYearThree) <> 0
THEN SUBSTRING(@SixMonthYearThree, 0, CharIndex('.', @SixMonthYearThree)+2) + '%' ELSE @SixMonthYearThree + '.0%' END ,
CASE WHEN CharIndex('.', @SixMonthYearFour) <> 0
THEN SUBSTRING(@SixMonthYearFour, 0, CharIndex('.', @SixMonthYearFour)+2) + '%' ELSE @SixMonthYearFour + '.0%' END ,
CASE WHEN CharIndex('.', @SixMonthYearFive) <> 0
THEN SUBSTRING(@SixMonthYearFive, 0, CharIndex('.', @SixMonthYearFive)+2) + '%' ELSE @SixMonthYearFive + '.0%' END ,
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


