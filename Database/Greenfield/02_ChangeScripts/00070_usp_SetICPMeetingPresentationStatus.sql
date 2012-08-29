--declare  current and required version
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

/****** Object:  StoredProcedure [dbo].[SetICPMeetingPresentationStatus]    Script Date: 08/29/2012 14:42:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================
-- Author:		Rahul Vig
-- Create date: 18-08-2012
-- Description:	Update Meeting Attached File Details
-- ===================================================
CREATE PROCEDURE [dbo].[SetICPMeetingPresentationStatus] 
	@UserName VARCHAR(50),
	@MeetingId BIGINT,	
	@Status VARCHAR(50)	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
		UPDATE PresentationInfo 
		SET StatusType = @Status,
			ModifiedBy = @UserName,
			ModifiedOn = GETUTCDATE()
		WHERE PresentationID IN
		(SELECT PresentationID FROM MeetingPresentationMappingInfo
			WHERE MeetingID = @MeetingId)
		
		
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT -1
			RETURN
		END	
		
	COMMIT TRANSACTION	
	SELECT 0    
END

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00070'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())