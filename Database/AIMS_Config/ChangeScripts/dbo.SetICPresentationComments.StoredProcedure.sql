SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===================================================
-- Author:		Rahul Vig
-- Create date: 18-08-2012
-- Description:	Update Meeting Attached File Details
-- ===================================================
alter procedure [dbo].[SetICPresentationComments] 
	@UserName VARCHAR(50),
	@PresentationID BIGINT,
	@Comment VARCHAR(255)		
AS
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO CommentInfo ( PresentationID, Comment, CommentBy, CommentOn)
	VALUES ( @PresentationID, @Comment, @UserName, GETUTCDATE())
		
	SELECT * FROM CommentInfo WHERE PresentationID = @PresentationID				
	ORDER BY CommentOn DESC
    
END
GO
