SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[UpdateMessageInfo] 	
@EmailId BIGINT,
@EmailSent BIT,
@UserName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE MessageInfo SET
	[EmailSent] = @EmailSent,
	[ModifiedBy] = @UserName,
	[ModifiedOn] = GETUTCDATE()
	WHERE [EmailId] = @EmailId
	
	IF @@ERROR <> 0
	BEGIN
		SELECT -1
		RETURN
	END
	
	SELECT 0
END
GO
