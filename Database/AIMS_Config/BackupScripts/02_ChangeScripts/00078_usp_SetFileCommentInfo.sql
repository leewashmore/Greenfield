set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00077'
declare @CurrentScriptVersion as nvarchar(100) = '00078'
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

CREATE PROCEDURE [dbo].[SetFileCommentInfo] 
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

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00078'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
