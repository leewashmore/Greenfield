set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00009'
declare @CurrentScriptVersion as nvarchar(100) = '00010'

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
/****** Object:  StoredProcedure [dbo].[GetConsensusEstimateDetailedData]    Script Date: 06/27/2012 12:02:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetQuarterlyResults]
(
@DataId INT = 44,
@PeriodYear INT
)
AS

SET FMTONLY OFF

BEGIN


DECLARE @TEMP_QUARTERLY_RESULT TABLE 
(
 ISSUER_ID VARCHAR(20),
 IssuerName NVARCHAR(255),
 Region NVARCHAR(255),
 Country NVARCHAR(255),
 Sector NVARCHAR(255),
 Industry NVARCHAR(255), 
 Currency VARCHAR(20),
 LastUpdate DATETIME,
 Q1 DECIMAL(38,6),
 Q2 DECIMAL(38,6),
 Q3 DECIMAL(38,6),
 Q4 DECIMAL(38,6),
 Annual DECIMAL(38,6),
 QuarterlySum DECIMAL(38,6),
 QuarterlySumPercentage DECIMAL(38,6),
 Consensus DECIMAL(38,6),
 ConsensusPercentage DECIMAL(38,6),
 High DECIMAL(38,6),
 Low DECIMAL(38,6),
 Brokers INT,
 ConsensusUpdate DATETIME,
 XREF FLOAT,
 EstimateId INT
 )

INSERT INTO @TEMP_QUARTERLY_RESULT(ISSUER_ID,IssuerName,Region,Country,Sector,Industry,
Currency,Q1,Q2,Q3,Q4,Annual,QuarterlySum,QuarterlySumPercentage,XREF)
SELECT aa.ISSUER_ID,bb.ISSUE_NAME,bb.ASEC_SEC_COUNTRY_ZONE_NAME,bb.ASEC_SEC_COUNTRY_NAME,bb.GICS_SECTOR_NAME,
bb.GICS_INDUSTRY_NAME,aa.CURRENCY,aa.Q1,aa.Q2,aa.Q3,aa.Q4,aa.A,
(aa.Q1 + aa.Q2+aa.Q3+aa.Q4)AS QuarterlySum,((aa.Q1 + aa.Q2+aa.Q3+aa.Q4)/aa.A)*100 AS QuarterlySumPercentage,bb.XREF
FROM GF_SECURITY_BASEVIEW bb
JOIN
(
SELECT ISSUER_ID,CURRENCY,Q1,Q2,Q3,Q4,A FROM PERIOD_FINANCIALS
PIVOT 
(
 SUM(AMOUNT)
 FOR PERIOD_TYPE
 IN (Q1, Q2, Q3, Q4,A)
) AS P
WHERE PERIOD_YEAR = @PeriodYear AND Data_Id = @DataId) aa ON bb.ISSUER_ID = aa.ISSUER_ID

UPDATE @TEMP_QUARTERLY_RESULT
SET LastUpdate = bb.LastPrimaryModelLoad
FROM @TEMP_QUARTERLY_RESULT aa JOIN INTERNAL_ISSUER bb
ON aa.ISSUER_ID = bb.ISSUER_ID

IF @DataId = 11
BEGIN
UPDATE @TEMP_QUARTERLY_RESULT
SET EstimateId = 17
FROM @TEMP_QUARTERLY_RESULT
END
ELSE
BEGIN
UPDATE @TEMP_QUARTERLY_RESULT
SET EstimateId = CASE bb.Earnings
WHEN 'EPS' THEN 11
WHEN 'EPSREP' THEN 13
WHEN 'EBG' THEN 12
END
FROM @TEMP_QUARTERLY_RESULT aa JOIN [Reuters].[dbo].tblCompanyInfo  bb ON aa.XREF = bb.XRef
END

UPDATE @TEMP_QUARTERLY_RESULT
SET Consensus  = bb.AMOUNT,
 High = bb.HIGH,
 Low =bb.LOW,
 Brokers =bb.NUMBER_OF_ESTIMATES,
 ConsensusUpdate = bb.DATA_SOURCE_DATE
FROM @TEMP_QUARTERLY_RESULT aa JOIN CURRENT_CONSENSUS_ESTIMATES bb
ON aa.ISSUER_ID = bb.ISSUER_ID AND aa.EstimateId = bb.ESTIMATE_ID AND aa.Currency = bb.CURRENCY

UPDATE @TEMP_QUARTERLY_RESULT
SET ConsensusPercentage  = (Consensus/Annual)*100
FROM @TEMP_QUARTERLY_RESULT


SELECT * FROM @TEMP_QUARTERLY_RESULT
END
GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00010'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


