--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00074'
declare @CurrentScriptVersion as nvarchar(100) = '00075'
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

/****** Object:  StoredProcedure [dbo].[SetPresentationInfo]    Script Date: 08/29/2012 14:48:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sneha Sharma
-- Create date: 06-08-2012
-- Description:	Inserts a new presentation
-- =============================================
CREATE PROCEDURE [dbo].[SetPresentationInfo] 
	-- Add the parameters for the stored procedure here
	  @userName VARCHAR(50),
	  @xmlScript NVARCHAR(MAX)
AS
BEGIN
	
	SET NOCOUNT ON;
	BEGIN TRANSACTION
		
		--DECLARE @UserName VARCHAR(50) = 'sneha'
		--DECLARE @xmlScript NVARCHAR(MAX) = N'<Root>
		--<ICPresentationOverviewData AcceptWithoutDiscussionFlag="true" PresentationID="0" Presenter="sneha1" StatusType="In Progress" SecurityTicker="St1" SecurityName="Security1" SecurityCountry="India" SecurityCountryCode="IND" SecurityIndustry="SEC" SecurityCashPosition="0" SecurityPosition="0" SecurityMSCIStdWeight="0" SecurityMSCIIMIWeight="0" SecurityGlobalActiveWeight="0" SecurityLastClosingPrice="0" SecurityMarketCapitalization="0" SecurityPFVMeasure="PFV" SecurityBuyRange="0" SecuritySellRange="0" SecurityRecommendation="BUY" CommitteePFVMeasure="PFV" CommitteeBuyRange="0" CommitteeSellRange="0" CommitteeRecommendation="BUY" CommitteeRangeEffectiveThrough="2012-08-20T15:13:00" AdminNotes="ADMINNOTES" MeetingDateTime="2012-08-30T08:30:00" MeetingClosedDateTime="2012-08-27T17:00" MeetingVotingClosedDateTime="2012-08-30T08:00"/><VotingUser>User1</VotingUser><VotingUser>User2</VotingUser><VotingUser>User3</VotingUser><VotingUser>user4</VotingUser><VotingUser>user5</VotingUser><VotingUser>user6</VotingUser>
		--</Root>'	
		
		
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
				
		--SELECT * FROM OPENXML(@idoc, N'/Root/ICPresentationOverviewData') 
  --WHERE [text] IS NOT NULL

				
		DECLARE @InsertPresentationRecordCount INT
		SELECT @InsertPresentationRecordCount = COUNT(*) FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)			
		
		DECLARE @SecurityPFVMeasure VARCHAR(255)
		DECLARE @SecurityBuyRange REAL
		DECLARE	@SecuritySellRange REAL
		DECLARE @SecurityRecommendation VARCHAR(50)

		SELECT Top(1) @SecurityPFVMeasure = [SecurityPFVMeasure] FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
				WITH ( [SecurityPFVMeasure] VARCHAR(50))
				
		SELECT Top(1) @SecurityBuyRange = [SecurityBuyRange] FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
				WITH ( [SecurityBuyRange] REAL)

		SELECT Top(1) @SecuritySellRange = [SecuritySellRange] FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
				WITH ( [SecuritySellRange] REAL)

		SELECT Top(1) @SecurityRecommendation = [SecurityRecommendation] FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
				WITH ( [SecurityRecommendation] VARCHAR(50))
				
		
		--#############################################################################################################
		-- PresentationInfo - Inserted Record Into PresentationInfo table
		--#############################################################################################################		
		
		IF @InsertPresentationRecordCount <>0
		BEGIN
			DECLARE @PresentationID BIGINT
			DECLARE @MeetingID BIGINT
			
			INSERT INTO PresentationInfo (
			[Presenter],
			[StatusType],
			[SecurityTicker],
			[SecurityName],
			[SecurityCountry],
			[SecurityCountryCode],
			[SecurityIndustry],
			[SecurityCashPosition],
			[SecurityPosition],
			[SecurityMSCIStdWeight],
			[SecurityMSCIIMIWeight],
			[SecurityGlobalActiveWeight],
			[SecurityLastClosingPrice],
			[SecurityMarketCapitalization],
			[SecurityPFVMeasure],
			[SecurityBuyRange],
			[SecuritySellRange],
			[SecurityRecommendation],
			[CommitteePFVMeasure],
			[CommitteeBuyRange],
			[CommitteeSellRange],
			[CommitteeRecommendation],
			[CommitteeRangeEffectiveThrough],
			[AcceptWithoutDiscussionFlag],
			[AdminNotes],
			[CreatedBy],
			[CreatedOn],
			[ModifiedBy],
			[ModifiedOn],
			[Analyst],
			[Price],
			[FVCalc],
			[SecurityBuySellvsCrnt],
			[CurrentHoldings],
			[PercentEMIF],
			[SecurityBMWeight],
			[SecurityActiveWeight],
			[YTDRet_Absolute],
			[YTDRet_RELtoLOC],
			[YTDRet_RELtoEM])
			
			SELECT TOP(1) 
			[Presenter],
			[StatusType],
			[SecurityTicker],
			[SecurityName],
			[SecurityCountry],
			[SecurityCountryCode],
			[SecurityIndustry],
			[SecurityCashPosition],
			[SecurityPosition],
			[SecurityMSCIStdWeight],
			[SecurityMSCIIMIWeight],
			[SecurityGlobalActiveWeight],
			[SecurityLastClosingPrice],
			[SecurityMarketCapitalization],
			[SecurityPFVMeasure],
			[SecurityBuyRange],
			[SecuritySellRange],
			[SecurityRecommendation],
			[CommitteePFVMeasure],
			[CommitteeBuyRange],
			[CommitteeSellRange],
			[CommitteeRecommendation],
			[CommitteeRangeEffectiveThrough],
			[AcceptWithoutDiscussionFlag],
			[AdminNotes],
			@userName,
			GETUTCDATE(),
			@userName,
			GETUTCDATE(),
			[Analyst],
			[Price],
			[FVCalc],
			[SecurityBuySellvsCrnt],
			[CurrentHoldings],
			[PercentEMIF],
			[SecurityBMWeight],
			[SecurityActiveWeight],
			[YTDRet_Absolute],
			[YTDRet_RELtoLOC],
			[YTDRet_RELtoEM]
			
			FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
			WITH (
			[Presenter] VARCHAR(50),
			[StatusType] VARCHAR(50),
			[SecurityTicker] VARCHAR(50),
			[SecurityName] VARCHAR(50),
			[SecurityCountry] VARCHAR(50),
			[SecurityCountryCode] VARCHAR(50),
			[SecurityIndustry] VARCHAR(50),
			[SecurityCashPosition] REAL,
			[SecurityPosition] BIGINT,
			[SecurityMSCIStdWeight] REAL,
			[SecurityMSCIIMIWeight] REAL,
			[SecurityGlobalActiveWeight] REAL,
			[SecurityLastClosingPrice] REAL,
			[SecurityMarketCapitalization] REAL,
			[SecurityPFVMeasure] VARCHAR(50),
			[SecurityBuyRange] REAL,
			[SecuritySellRange] REAL,
			[SecurityRecommendation] VARCHAR(50),
			[InvestmentThesis] NVARCHAR(MAX),
			[Background] NVARCHAR(MAX),
			[Valuations] NVARCHAR(MAX),
			[EarningsOutlook] NVARCHAR(MAX),
			[CompetitiveAdvantage] NVARCHAR(MAX),
			[CompetitiveDisadvantage] NVARCHAR(MAX),
			[CommitteePFVMeasure] VARCHAR(255),
			[CommitteeBuyRange] REAL,
			[CommitteeSellRange] REAL,
			[CommitteeRecommendation] VARCHAR(50),
			[CommitteeRangeEffectiveThrough] DATE,
			[AcceptWithoutDiscussionFlag] BIT,
			[AdminNotes] VARCHAR(255),			
			[Analyst] VARCHAR(50),
			[Price] VARCHAR(50),
			[FVCalc] VARCHAR(50),
			[SecurityBuySellvsCrnt] VARCHAR(50),
			[CurrentHoldings] VARCHAR(50),
			[PercentEMIF] VARCHAR(50),
			[SecurityBMWeight] VARCHAR(50),
			[SecurityActiveWeight] VARCHAR(50),
			[YTDRet_Absolute] VARCHAR(50),
			[YTDRet_RELtoLOC] VARCHAR(50),
			[YTDRet_RELtoEM] VARCHAR(50))		
			
			
			SET @PresentationID = @@IDENTITY
			
			--INSERT INTO tempTable1 ([PresentationID], [Description]) values (@PresentationID, 'Inserted Presentation ID After INSERT into PresentationInfo')			
						
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -2
				RETURN
			END				
			
		--#############################################################################################################
		-- MeetingInfo - Inserted Record Into MeetingInfo table
		--#############################################################################################################		
			
			DECLARE @MeetingDateTime DATETIME
			SELECT Top(1) @MeetingDateTime = [MeetingDateTime] FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
				WITH ( [MeetingDateTime] DATETIME)
				
			DECLARE @MeetingClosedDateTime DATETIME
			SELECT Top(1) @MeetingClosedDateTime = [MeetingClosedDateTime] FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
				WITH ( [MeetingClosedDateTime] DATETIME)
				
			DECLARE @MeetingVotingClosedDateTime DATETIME
			SELECT Top(1) @MeetingVotingClosedDateTime = [MeetingVotingClosedDateTime] FROM OPENXML(@idoc, '/Root/ICPresentationOverviewData', 1)
				WITH ( [MeetingVotingClosedDateTime] DATETIME)		
			
			SELECT Top(1) @MeetingID = MeetingInfo.MeetingID FROM MeetingInfo
			WHERE MeetingDateTime = @MeetingDateTime 
			AND MeetingClosedDateTime = @MeetingClosedDateTime
			AND MeetingVotingClosedDateTime = @MeetingVotingClosedDateTime
			
			IF @MeetingID IS NULL
			BEGIN
				INSERT INTO MeetingInfo (MeetingDateTime, MeetingClosedDateTime, MeetingVotingClosedDateTime, [MeetingDescription], CreatedBy,
				CreatedOn, ModifiedBy, ModifiedOn)
				VALUES (@MeetingDateTime, @MeetingClosedDateTime, @MeetingVotingClosedDateTime, NULL, @userName,
				GETUTCDATE(), @userName, GETUTCDATE())
				
				SET @MeetingID = @@IDENTITY
			END
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -3
				RETURN
			END	
			
		--#############################################################################################################
		-- MeetingPresentationMappingInfo - Inserted Record Into MeetingPresentationMappingInfo table
		--#############################################################################################################		
			
			INSERT INTO MeetingPresentationMappingInfo (PresentationID, MeetingID, CreatedBy, CreatedOn, ModifedBy, ModifiedOn)
			VALUES ( @PresentationID, @MeetingID, @userName, GETUTCDATE(), @userName, GETUTCDATE())
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -4
				RETURN
			END	
			

			--INSERT INTO tempTable1 ([PresentationID], [Description]) values (@PresentationID, 'Inserted Presentation ID After INSERT into MeetingPresentationMappingInfo')			
			
	
	END			
		
		--#############################################################################################################
		-- VoterInfo - Updated VoterInfo table
		--#############################################################################################################
		
		DECLARE @InsertVoterInfoRecordCount INT
		SELECT @InsertVoterInfoRecordCount = COUNT(*) FROM OPENXML(@idoc, '/Root/VotingUser', 1)			
		
		
		IF @InsertVoterInfoRecordCount <> 0
		BEGIN
						
			INSERT INTO VoterInfo 
			(			[PresentationID],
						[Name],
						[VoteType],
						[PostMeetingFlag],
						[DiscussionFlag],
						[VoterPFVMeasure],
						[VoterBuyRange],
						[VoterSellRange],
						[VoterRecommendation],
						[CreatedBy],
						[CreatedOn],
						[ModifiedBy],
						[ModifiedOn])
			SELECT		@PresentationID,						
						[text],						
						'Agree',
						'False',
						'False',
						@SecurityPFVMeasure,
						@SecurityBuyRange,
						@SecuritySellRange,
						@SecurityRecommendation,
						@userName,
						GETUTCDATE(),
						@userName,
						GETUTCDATE()			
			FROM OPENXML(@idoc, '/Root/VotingUser') 
			WHERE [text] IS NOT NULL				
					

			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -5
				RETURN
			END	
	END
	SELECT 0
	COMMIT TRANSACTION
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00075'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
