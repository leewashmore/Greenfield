SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SetICPresentationDecisionEntryDetails] 
	@UserName VARCHAR(50),
	@xmlScript NVARCHAR(MAX)	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
		
		DECLARE @XML XML
		
		SELECT @XML = @xmlScript
		DECLARE @idoc int
		EXEC sp_xml_preparedocument @idoc OUTPUT, @XML		
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN
			END	
		
		--#############################################################################################################
		-- ICPresentationOverviewData - Updated PresentationInfo table
		--#############################################################################################################
		
		DECLARE @UpdationICPresentationOverviewRecordCount INT
		SELECT @UpdationICPresentationOverviewRecordCount = COUNT(*) FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)			
			
		IF @UpdationICPresentationOverviewRecordCount <> 0
		BEGIN
			SELECT * INTO #ICPresentationOverviewData FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
				WITH ( 
					[PresentationID]			BIGINT,
					[AdminNotes]				VARCHAR(255),
					[CommitteePFVMeasure]		VARCHAR(255),
					[CommitteePFVMeasureValue]	DECIMAL,
					[CommitteeBuyRange]			REAL,
					[CommitteeSellRange]		REAL,
					[CommitteeRecommendation]	VARCHAR(50))			
			
			UPDATE PresentationInfo 
			SET [AdminNotes]				= _IMMD.[AdminNotes],
				[CommitteePFVMeasure]		= _IMMD.[CommitteePFVMeasure],
				[CommitteePFVMeasureValue]	= _IMMD.[CommitteePFVMeasureValue],
				[CommitteeBuyRange]			= _IMMD.[CommitteeBuyRange],
				[CommitteeSellRange]		= _IMMD.[CommitteeSellRange],
				[CommitteeRecommendation]	= _IMMD.[CommitteeRecommendation],
				[StatusType]				= 'Decision Entered',
				[ModifiedBy]				= @UserName,
				[ModifiedOn]				= GETUTCDATE()
			FROM #ICPresentationOverviewData _IMMD
			WHERE PresentationInfo.[PresentationID] = _IMMD.[PresentationID]
						
			DROP TABLE #ICPresentationOverviewData
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -2
				RETURN
			END		
		END
			
		--#############################################################################################################
		-- VoterInfo - Updated VoterInfo table
		--#############################################################################################################
		DECLARE @UpdationVoterInfoRecordCount INT
		SELECT @UpdationVoterInfoRecordCount = COUNT(*) FROM OPENXML(@idoc, '/Root/VoterInfo', 1)		
			
		IF @UpdationVoterInfoRecordCount <> 0
		BEGIN
			SELECT _MMVI.[VoterID],
				   _MMVI.[VoterPFVMeasure],
				   _MMVI.[VoterBuyRange],
				   _MMVI.[VoterSellRange],
				   _MMVI.[VoteType]				   
			INTO #ICVoterInfo
			FROM OPENXML(@idoc, '/Root/VoterInfo', 1)
				WITH ( 
					[VoterID]			BIGINT, 
					[VoterPFVMeasure]	VARCHAR(255), 
					[VoterBuyRange]		REAL,
					[VoterSellRange]	REAL,
					[VoteType]			VARCHAR(50)) _MMVI

			UPDATE VoterInfo
			SET
				VoterInfo.[VoterPFVMeasure] = _ICVI.[VoterPFVMeasure],
				VoterInfo.[VoterBuyRange]	= _ICVI.[VoterBuyRange],
				VoterInfo.[VoterSellRange]	= _ICVI.[VoterSellRange],
				VoterInfo.[VoteType]		= _ICVI.[VoteType],
				VoterInfo.[ModifiedBy]		= @UserName,
				VoterInfo.[ModifiedOn]		= GETUTCDATE()
			FROM #ICVoterInfo _ICVI
			WHERE VoterInfo.[VoterID] = _ICVI.[VoterID]	
											
			DROP TABLE #ICVoterInfo
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -3
				RETURN
			END	
		END
		
	SELECT 0
	COMMIT TRANSACTION	
    
END
GO
