SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[RetrieveICPresentationAttachedFileDetails]
	@PresentationId BIGINT = 0
AS
BEGIN
	SET NOCOUNT ON;

    SELECT *
    FROM
    FileMaster _FM
    WHERE _FM.[FileID] IN
    ( SELECT [FileID] FROM PresentationAttachedFileInfo WHERE [PresentationID] = @PresentationId )    
    ORDER BY _FM.[Name]
END
GO
