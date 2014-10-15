SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[DeletePresentationInfo] 
		@PresentationId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
		
	--Deleting voter information
	delete from voterinfo where presentationid = @PresentationId
	
	--Deleting Meeting information
	delete  meetinginfo where meetingid in (Select meetingid from meetingpresentationmappinginfo where presentationid = @PresentationId)
	
	select fileid  into #A from PresentationAttachedFileInfo where presentationid = @PresentationId
	--Deleting from PresentationAttachedFileInfo 
	delete from dbo.PresentationAttachedFileInfo where presentationid = @PresentationId
	
	
	--Deleting from file master
	delete from filemaster where fileid in (select fileid from #A)
	
	--Deleting from meetingpresentationmappinginfo 
	delete from meetingpresentationmappinginfo where presentationid = @PresentationId

    --Deleting from presentationinfo 
	delete from presentationinfo where presentationid  = @PresentationId
	
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


--exec [DeletePresentationInfo] 175