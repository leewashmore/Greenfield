set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00076'
declare @CurrentScriptVersion as nvarchar(100) = '00077'
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

CREATE PROCEDURE [dbo].[SetUploadFileInfo] 
	@UserName VARCHAR(50),
	@Name VARCHAR(255),	
	@Location VARCHAR(255),
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
		
		INSERT INTO FileMaster ( [Name], [Location], [SecurityName], [SecurityTicker], [Type], [MetaTags], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn] )
		VALUES ( @Name, @Location, @SecurityName, @SecurityTicker, @Type, @MetaTags, @UserName, @UTCDATETIME, @UserName, @UTCDATETIME )
		
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

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00077'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
