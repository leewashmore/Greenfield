SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[SaveUpdatedFairValueMeasures]
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
			SELECT _MMVI.[Source],
				   _MMVI.[Buy],
				   _MMVI.[Sell],
				   _MMVI.[Upside],
				   _MMVI.[DataId],
				   _MMVI.[Date]
			INTO #FairValueMeasureData 
			FROM OPENXML(@idoc, '/Root/FairValueCompositionSummaryData', 1)
				WITH ( 
					[Source]		VARCHAR(20),
					[Buy]			DECIMAL (32,6),
					[Sell]			DECIMAL (32,6),
					[Upside]		DECIMAL (32,6),
					[DataId]    	INT,
					[Date]          DATETIME ) _MMVI							
			
			
			--(MOD) 7/23/13 (JM) Pull the current measure value and store on save
			declare @CurrentValue decimal(32,6)
			
			select @CurrentValue = pf.amount 
			from period_financials pf
			join #FairValueMeasureData fvmd on pf.data_id = fvmd.dataid and pf.DATA_SOURCE = fvmd.source
			where pf.SECURITY_ID = @securityId
			and pf.CURRENCY = 'USD'
			and pf.PERIOD_TYPE = 'C'
			and pf.PERIOD_YEAR = 0
			
			--(MOD) 2014-12-09 LMS Needed for when there are no current measures
			if (@CurrentValue is null)
			begin
			   SET @CurrentValue = 0
			end
					
			MERGE INTO FAIR_VALUE _VI
			USING #FairValueMeasureData _MMVD
			ON _VI.[SECURITY_ID] = @securityId AND _VI.VALUE_TYPE = _MMVD.[Source]
			WHEN MATCHED THEN
			UPDATE SET
				[FV_BUY]				= _MMVD.[Buy],
				[FV_SELL]				= _MMVD.[Sell],
				[UPSIDE]				= _MMVD.[Upside],
				[FV_MEASURE]	        = _MMVD.[DataId],
				[UPDATED]				=  GETUTCDATE(),
				[PERIOD_TYPE]           = 'C',
				[PERIOD_YEAR]           =  0,
				[CURRENT_MEASURE_VALUE] = @CurrentValue
			WHEN NOT MATCHED THEN
			INSERT (VALUE_TYPE, SECURITY_ID, FV_MEASURE, FV_BUY, FV_SELL, CURRENT_MEASURE_VALUE,UPSIDE, UPDATED, PERIOD_TYPE, PERIOD_YEAR)
			VALUES (_MMVD.[Source], @securityId,_MMVD.[DataId], _MMVD.[Buy],_MMVD.[Sell],@CurrentValue,_MMVD.[Upside],GETUTCDATE(),'C',0);		   
			
			DROP TABLE #FairValueMeasureData	
			
		END

COMMIT TRANSACTION		

		
SELECT b.VALUE_TYPE as SOURCE,a.DATA_DESC as MEASURE,b.FV_BUY as BUY,b.FV_SELL as SELL,b.UPSIDE as UPSIDE
    ,b.UPDATED as DATE,a.DATA_ID as DATA_ID
    from DATA_MASTER a inner join  FAIR_VALUE b ON a.DATA_ID = b.FV_MEASURE 
	WHERE SECURITY_ID=@securityId

    
END
