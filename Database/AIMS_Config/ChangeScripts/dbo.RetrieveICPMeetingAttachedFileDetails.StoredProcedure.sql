SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================
-- Author:		Rahul Vig
-- Create date: 18-08-2012
-- Description:	Retrieve Meeting Attached File Details
-- ===================================================
alter procedure [dbo].[RetrieveICPMeetingAttachedFileDetails] 
	@MeetingId BIGINT = 0
AS
BEGIN
	SET NOCOUNT ON;

    SELECT *
    FROM
    FileMaster _FM
    WHERE _FM.[FileID] IN
    ( SELECT [FileID] FROM MeetingAttachedFileInfo WHERE [MeetingID] = @MeetingId )    
    ORDER BY _FM.[Name]
END
GO
