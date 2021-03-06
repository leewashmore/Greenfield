SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SetMessageInfo] 	
@EmailTo VARCHAR(MAX),
@EmailCc VARCHAR(MAX),
@EmailSubject VARCHAR(255),
@EmailMessageBody VARCHAR(MAX),
@EmailAttachment VARCHAR(MAX),
@UserName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO MessageInfo ( [EmailTo], [EmailCc], [EmailSubject], [EmailMessageBody], [EmailAttachment], [EmailSent]
	, [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn] )
	VALUES ( @EmailTo, @EmailCc, @EmailSubject, @EmailMessageBody, @EmailAttachment, 'False'
	, @UserName, GETUTCDATE(), @UserName, GETUTCDATE())
	
	IF @@ERROR <> 0
	BEGIN
		SELECT -1
		RETURN
	END
	
	SELECT 0
END
GO
