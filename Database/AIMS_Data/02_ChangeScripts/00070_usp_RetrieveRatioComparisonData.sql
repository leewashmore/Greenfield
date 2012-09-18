set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00069'
declare @CurrentScriptVersion as nvarchar(100) = '00070'

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

IF OBJECT_ID ('[dbo].[RetrieveRatioComparisonData]') IS NOT NULL
	DROP PROCEDURE [dbo].[RetrieveRatioComparisonData]
GO

CREATE PROCEDURE [dbo].[RetrieveRatioComparisonData] 
	-- Add the parameters for the stored procedure here
	@securityBucketXML VARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRANSACTION
	
	DECLARE @XML XML
	SELECT @XML = @securityBucketXML
	DECLARE @idoc int
	EXEC sp_xml_preparedocument @idoc OUTPUT, @XML
    
    --Check for xml root node 'RatioData'
    DECLARE @RatioDataCount INT
	SELECT @RatioDataCount = COUNT(*) FROM OPENXML(@idoc, '/RatioData', 1)
		WITH ( GroupName VARCHAR(50))
	
	IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT 1
			RETURN
		END		
		
	IF @RatioDataCount <> 0
		BEGIN
			DECLARE @PeriodType VARCHAR(50)						
			SELECT @PeriodType = PeriodType FROM OPENXML(@idoc, '/RatioData', 2)
						WITH (PeriodType VARCHAR(50) '@PeriodType')
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 2
				RETURN
			END
		
			DECLARE @PeriodYear INT			
			SELECT @PeriodYear = PeriodYear FROM OPENXML(@idoc, '/RatioData', 2)
						WITH (PeriodYear VARCHAR(50) '@PeriodYear')
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 3
				RETURN
			END
			
			DECLARE @FinancialDataId INT			
			SELECT @FinancialDataId = DataId FROM OPENXML(@idoc, '/RatioData', 2)
						WITH (DataId VARCHAR(50) '@FinancialDataId')
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 4
				RETURN
			END
			
			DECLARE @FinancialEstimationId INT			
			SELECT @FinancialEstimationId = EstimationId FROM OPENXML(@idoc, '/RatioData', 2)
						WITH (EstimationId VARCHAR(50) '@FinancialEstimationId')
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 5
				RETURN
			END
			
			DECLARE @ValuationDataId INT			
			SELECT @ValuationDataId = DataId FROM OPENXML(@idoc, '/RatioData', 2)
						WITH (DataId VARCHAR(50) '@ValuationDataId')
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 6
				RETURN
			END
			
			DECLARE @ValuationEstimationId INT			
			SELECT @ValuationEstimationId = EstimationId FROM OPENXML(@idoc, '/RatioData', 2)
						WITH (EstimationId VARCHAR(50) '@ValuationEstimationId')
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 7
				RETURN
			END
			
			SELECT * INTO #SecurityInfo 
			FROM OPENXML(@idoc, '/RatioData/Issue', 2)
				WITH (
					SECURITY_ID VARCHAR(50) '@SecurityId', 
					ISSUER_ID VARCHAR(50) '@IssuerId', 
					ISSUE_NAME VARCHAR(50) '@IssueName')
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 8
				RETURN
			END
			
			SELECT SI.*, PF.AMOUNT, PF.DATA_ID INTO #PrimaryFinancialsSecurityInfo
			FROM #SecurityInfo SI, dbo.PERIOD_FINANCIALS PF			
			WHERE (PF.SECURITY_ID = SI.SECURITY_ID OR PF.ISSUER_ID = SI.ISSUER_ID)
				AND PF.CURRENCY = 'USD'
				AND PF.DATA_SOURCE = 'PRIMARY'
				AND PF.PERIOD_TYPE = @PeriodType
				AND PF.FISCAL_TYPE = 
					CASE WHEN @PeriodYear = 0 THEN PF.FISCAL_TYPE 
					ELSE 'CALENDAR' END
				AND PF.PERIOD_YEAR = 
				    CASE WHEN @PeriodYear = 0 THEN PF.PERIOD_YEAR 
					ELSE @PeriodYear END
				AND (PF.DATA_ID = @FinancialDataId or pf.DATA_ID = @ValuationDataId)							
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 9
				RETURN
			END
			
			SELECT SI.*, CCE.AMOUNT, CCE.ESTIMATE_ID INTO #CurrentConsensusEstimatesSecurityInfo
			FROM #SecurityInfo SI, dbo.CURRENT_CONSENSUS_ESTIMATES CCE			
			WHERE (CCE.SECURITY_ID = SI.SECURITY_ID OR CCE.ISSUER_ID = SI.ISSUER_ID)
				AND CCE.CURRENCY = 'USD'
				AND CCE.DATA_SOURCE = 'REUTERS'
				AND CCE.PERIOD_TYPE = @PeriodType
				AND CCE.FISCAL_TYPE = 
					CASE WHEN @PeriodYear = 0 THEN CCE.FISCAL_TYPE 
					ELSE 'CALENDAR' END
				AND CCE.PERIOD_YEAR = 
				    CASE WHEN @PeriodYear = 0 THEN CCE.PERIOD_YEAR 
					ELSE @PeriodYear END
				AND (CCE.ESTIMATE_ID = @FinancialEstimationId or CCE.ESTIMATE_ID = @ValuationEstimationId)
				AND CCE.AMOUNT_TYPE = 'ESTIMATE'
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 10
				RETURN
			END
			
			SELECT * INTO #CrudeRatioData 
			FROM #PrimaryFinancialsSecurityInfo
				UNION
			SELECT * FROM #CurrentConsensusEstimatesSecurityInfo CCESI
				WHERE SECURITY_ID NOT IN
				( SELECT SECURITY_ID FROM #PrimaryFinancialsSecurityInfo)
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 11
				RETURN
			END
			
			SELECT SECURITY_ID, ISSUER_ID, ISSUE_NAME, AMOUNT, CASE WHEN (DATA_ID = @FinancialDataId OR DATA_ID = @FinancialEstimationId)
				THEN 'FINANCIAL' ELSE 'VALUATION' END AS RATIO_TYPE
			INTO #UpdatedRatioData FROM #CrudeRatioData
				
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 12
				RETURN
			END
			
			SELECT DISTINCT SECURITY_ID, 
			(CASE WHEN (SELECT COUNT(*) FROM #UpdatedRatioData WHERE SECURITY_ID = URD.SECURITY_ID) <> 2 
			THEN 'FALSE' ELSE 
			(CASE WHEN 
				(
				(SELECT COUNT(*) FROM #UpdatedRatioData WHERE SECURITY_ID = URD.SECURITY_ID 
				AND RATIO_TYPE = 'FINANCIAL') = 1				
				) AND 
				(
				(SELECT COUNT(*) FROM #UpdatedRatioData WHERE SECURITY_ID = URD.SECURITY_ID 
				AND RATIO_TYPE = 'VALUATION') = 1
				)
			THEN 
				(
					CASE WHEN 
					(URD.AMOUNT IS NOT NULL AND URD.AMOUNT <> 0)
					THEN 'TRUE' ELSE 'FALSE' END
				)				
			ELSE 'FALSE' END
			)
			 END) AS VALID_DATA
			INTO #DistictSecurityId
			FROM #UpdatedRatioData URD
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 13
				RETURN
			END
			
			SELECT * INTO #UnpivotedRatioData FROM #UpdatedRatioData 
			WHERE SECURITY_ID IN
				(SELECT SECURITY_ID FROM #DistictSecurityId WHERE VALID_DATA = 'TRUE')
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 14
				RETURN
			END
			
			SELECT SECURITY_ID, ISSUER_ID, ISSUE_NAME, FINANCIAL, VALUATION FROM #UnpivotedRatioData
			PIVOT (MAX(AMOUNT) FOR RATIO_TYPE IN ([FINANCIAL], [VALUATION])) AS AMOUNT
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 15
				RETURN
			END
			
			DROP TABLE #UnpivotedRatioData
			DROP TABLE #DistictSecurityId
			DROP TABLE #UpdatedRatioData
			DROP TABLE #CrudeRatioData
			DROP TABLE #CurrentConsensusEstimatesSecurityInfo
			DROP TABLE #PrimaryFinancialsSecurityInfo
			DROP TABLE #SecurityInfo
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 16
				RETURN
			END
			
			COMMIT TRANSACTION
		END
END


GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00070'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())