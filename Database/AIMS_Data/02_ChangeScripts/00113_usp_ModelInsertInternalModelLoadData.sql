set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00112'
declare @CurrentScriptVersion as nvarchar(100) = '00113'

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

IF OBJECT_ID ('[dbo].[ModelInsertInternalModelLoadData]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelInsertInternalModelLoadData]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertInternalModelLoadData]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelInsertInternalModelLoadData] 
	(
		@ISSUER_ID VARCHAR(20),
		@ROOT_SOURCE VARCHAR(10),
		@USER_NAME VARCHAR(255),
		@LOAD_TIME DATETIME,
		@DOCUMENT_ID bigint
	)
AS
BEGIN
	
	SET NOCOUNT ON;
	
INSERT INTO INTERNAL_MODEL_LOAD VALUES
	(	@ISSUER_ID ,
		@ROOT_SOURCE ,
		@USER_NAME ,
		@LOAD_TIME ,
		@DOCUMENT_ID 
	)
    
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00113'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

