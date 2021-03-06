SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SetFileCommentInfo] 
	@UserName VARCHAR(50),
	@FileId BIGINT,	
	@Comment VARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
		INSERT INTO CommentInfo ( [Comment], [FileID], [CommentBy], [CommentOn] )
		VALUES ( @Comment, @FileId, @UserName, GETUTCDATE() )
		
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
