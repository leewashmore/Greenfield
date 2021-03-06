SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SetUploadFileInfo] 
	@UserName VARCHAR(50),
	@Name VARCHAR(255),	
	@Location VARCHAR(255),
	@CompanyName VARCHAR(50),
	@SecurityName VARCHAR(255),
	@SecurityTicker VARCHAR(50),
	@Type VARCHAR(50),
	@MetaTags VARCHAR(50),
	@Comments VARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
		DECLARE @FILE_ID BIGINT
		DECLARE @UTCDATETIME DATETIME
		SET @UTCDATETIME = GETUTCDATE()
		
		INSERT INTO FileMaster ( [Name], [Location], [IssuerName], [SecurityName], [SecurityTicker], [Type], [MetaTags], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn] )
		VALUES ( @Name, @Location, @CompanyName, @SecurityName, @SecurityTicker, @Type, @MetaTags, @UserName, @UTCDATETIME, @UserName, @UTCDATETIME )
		
		SET @FILE_ID = @@IDENTITY
		
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT -1
			RETURN
		END	
		
		INSERT INTO CommentInfo ( [Comment], [FileID], [CommentBy], [CommentOn] )
		VALUES ( @Comments, @FILE_ID, @UserName, @UTCDATETIME )
		
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT -2
			RETURN
		END		
		
	COMMIT TRANSACTION	
	SELECT 0
    
END
GO
