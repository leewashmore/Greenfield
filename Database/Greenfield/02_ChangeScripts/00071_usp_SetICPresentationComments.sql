--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00070'
declare @CurrentScriptVersion as nvarchar(100) = '00071'
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

/****** Object:  StoredProcedure [dbo].[SetICPresentationComments]    Script Date: 08/29/2012 14:43:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================
-- Author:		Rahul Vig
-- Create date: 18-08-2012
-- Description:	Update Meeting Attached File Details
-- ===================================================
CREATE PROCEDURE [dbo].[SetICPresentationComments] 
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

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00071'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
