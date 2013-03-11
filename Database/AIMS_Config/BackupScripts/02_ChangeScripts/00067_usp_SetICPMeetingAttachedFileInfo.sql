set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00065'
declare @CurrentScriptVersion as nvarchar(100) = '00067'
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
/****** Object:  StoredProcedure [dbo].[SetICPMeetingAttachedFileInfo]    Script Date: 08/29/2012 14:39:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================
-- Author:		Rahul Vig
-- Create date: 18-08-2012
-- Description:	Update Meeting Attached File Details
-- ===================================================
CREATE PROCEDURE [dbo].[SetICPMeetingAttachedFileInfo] 
	@UserName VARCHAR(50),
	@MeetingId BIGINT,	
	@xmlScript NVARCHAR(MAX)	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
		DECLARE @XML XML
		DECLARE @UpdationICPMeetingAttachedFileRecordCount INT
		
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
		-- ICPMeetingAttachedFileInfo
		--#############################################################################################################
		SELECT @UpdationICPMeetingAttachedFileRecordCount = COUNT(*) FROM OPENXML(@idoc, '/Root/FileMaster', 1)		
		
		IF @UpdationICPMeetingAttachedFileRecordCount <> 0
		BEGIN
		
			DECLARE @FileId BIGINT
		
			SELECT	[FileID],
					[Name], 
					[SecurityName], 
					[SecurityTicker],
					[Location],  
					[MetaTags],  
					[Type]
			INTO #ICPMeetingAttachedFileData
			FROM OPENXML(@idoc, '/Root/FileMaster', 1) 
				WITH (
					[FileID] BIGINT,
					[Name] VARCHAR(255), 
					[SecurityName] VARCHAR(255), 
					[SecurityTicker] VARCHAR(50),
					[Location] VARCHAR(255),  
					[MetaTags] VARCHAR(255),  
					[Type] VARCHAR(50)) _MAFD		
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -2
				RETURN
			END	
		
			DECLARE @InsertCount INT
			SELECT @InsertCount = COUNT(*) FROM #ICPMeetingAttachedFileData WHERE [FileID] = 0
					
			IF @InsertCount <> 0
			BEGIN	
				INSERT INTO FileMaster ([Name], [SecurityName], [SecurityTicker], [Location], [MetaTags], [Type]
				, [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn])
				SELECT [Name], [SecurityName], [SecurityTicker], [Location], [MetaTags], [Type]
				, @UserName, GETUTCDATE(), @UserName, GETUTCDATE() FROM #ICPMeetingAttachedFileData
				WHERE [FileID] = 0
				
				SET @FileId = @@IDENTITY
				
				IF @@ERROR <> 0
				BEGIN
					ROLLBACK TRANSACTION
					SELECT -3
					RETURN
				END	
			
				INSERT INTO MeetingAttachedFileInfo ( [FileID], [MeetingID], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn] )
				VALUES ( @FileId, @MeetingId, @UserName, GETUTCDATE(), @UserName, GETUTCDATE() )
				
				IF @@ERROR <> 0
				BEGIN
					ROLLBACK TRANSACTION
					SELECT -4
					RETURN
				END	
			END
			
			DECLARE @DeletionCount INT
			SELECT @DeletionCount = COUNT(*) FROM #ICPMeetingAttachedFileData WHERE [FileID] <> 0
			
			IF @DeletionCount <> 0
			BEGIN
				DELETE FROM FileMaster WHERE [FileID] IN 
				(SELECT [FileID] FROM #ICPMeetingAttachedFileData WHERE [FileID] <> 0)
			END
			
			IF @@ERROR <> 0
				BEGIN
					ROLLBACK TRANSACTION
					SELECT -5
					RETURN
				END	
				
			DROP TABLE #ICPMeetingAttachedFileData
													
			IF @@ERROR <> 0
				BEGIN
					SELECT -6
					ROLLBACK TRANSACTION
					RETURN
				END							
		END
	COMMIT TRANSACTION	
	SELECT 0
    
END

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00067'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
