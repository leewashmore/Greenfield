set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00121'
declare @CurrentScriptVersion as nvarchar(100) = '00122'

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

IF OBJECT_ID ('[dbo].[ModelUpdateInternalModelLoadData]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelUpdateInternalModelLoadData]
GO

/****** Object:  StoredProcedure [dbo].[ModelUpdateInternalModelLoadData]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelUpdateInternalModelLoadData] 
	(
		@LOAD_ID int,
		@ISSUER_ID VARCHAR(20),
		@ROOT_SOURCE VARCHAR(10),
		@USER_NAME VARCHAR(255),
		@LOAD_TIME DATETIME,
		@DOCUMENT_ID bigint
	)
AS
BEGIN
	
	SET NOCOUNT ON;
	
UPDATE INTERNAL_MODEL_LOAD 
SET ISSUER_ID=@ISSUER_ID,
ROOT_SOURCE=@ROOT_SOURCE,
UserName=@USER_NAME,
LOAD_TIME=@LOAD_TIME,
DOCUMENT_ID=@DOCUMENT_ID
WHERE LOAD_ID=@LOAD_ID
    
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00122'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

