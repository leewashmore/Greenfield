set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00089'
declare @CurrentScriptVersion as nvarchar(100) = '00090'

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

IF OBJECT_ID ('[dbo].[SaveUpdatedFairValueMeasures]') IS NOT NULL
	DROP PROCEDURE [dbo].[SaveUpdatedFairValueMeasures]
GO

CREATE PROCEDURE [dbo].[SaveUpdatedFairValueMeasures]
	@securityId VARCHAR(20),
	@xmlScript NVARCHAR(MAX)	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
		
		DECLARE @XML XML
		
		SELECT @XML = @xmlScript
		DECLARE @idoc int
		EXEC sp_xml_preparedocument @idoc OUTPUT, @XML	   
		
		DECLARE @UpdationFairValueMeasureRecordCount INT
		SELECT @UpdationFairValueMeasureRecordCount = COUNT(*) FROM OPENXML(@idoc, '/Root/FairValueCompositionSummaryData', 1)			
			
		IF @UpdationFairValueMeasureRecordCount <> 0
		BEGIN   												
			SELECT _MMVI.[SOURCE],
				   _MMVI.[BUY],
				   _MMVI.[SELL],
				   _MMVI.[UPSIDE],
				   _MMVI.[DATA_ID],
				   _MMVI.[DATE]
			INTO #FairValueMeasureData 
			FROM OPENXML(@idoc, '/Root/FairValueCompositionSummaryData', 1)
				WITH ( 
					[SOURCE]		VARCHAR(20),
					[BUY]			DECIMAL (32,6),
					[SELL]			DECIMAL (32,6),
					[UPSIDE]		DECIMAL (32,6),
					[DATA_ID]    	INT,
					[DATE]          DATETIME ) _MMVI							
					
			MERGE INTO FAIR_VALUE _VI
			USING #FairValueMeasureData _MMVD
			ON _VI.[SECURITY_ID] = @securityId AND _VI.VALUE_TYPE = _MMVD.[SOURCE]
			WHEN MATCHED THEN
			UPDATE SET
				[FV_BUY]				= _MMVD.[BUY],
				[FV_SELL]				= _MMVD.[SELL],
				[UPSIDE]				= _MMVD.[UPSIDE],
				[FV_MEASURE]	        = _MMVD.[DATA_ID],
				[UPDATED]				=  GETUTCDATE()
			WHEN NOT MATCHED THEN
			INSERT (VALUE_TYPE, SECURITY_ID, FV_MEASURE, FV_BUY, FV_SELL, CURRENT_MEASURE_VALUE,UPSIDE, UPDATED)
			VALUES (_MMVD.[SOURCE], @securityId,_MMVD.[DATA_ID], _MMVD.[BUY],_MMVD.[SELL],0,_MMVD.[UPSIDE],GETUTCDATE());		   
			
			DROP TABLE #FairValueMeasureData	
			
		END

COMMIT TRANSACTION		

IF @@ROWCOUNT > 0
BEGIN
		
SELECT b.VALUE_TYPE as SOURCE,a.DATA_DESC as MEASURE,b.FV_BUY as BUY,b.FV_SELL as SELL,b.UPSIDE as UPSIDE
    ,b.UPDATED as DATE,a.DATA_ID as DATA_ID
    from DATA_MASTER a inner join  FAIR_VALUE b ON a.DATA_ID = b.FV_MEASURE 
	WHERE SECURITY_ID=@securityId

END
    
END
GO

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00090'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
