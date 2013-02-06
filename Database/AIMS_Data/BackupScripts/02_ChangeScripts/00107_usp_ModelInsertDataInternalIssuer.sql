set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00106'
declare @CurrentScriptVersion as nvarchar(100) = '00107'

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

IF OBJECT_ID ('[dbo].[ModelInsertDataInternalIssuer]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelInsertDataInternalIssuer]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertDataInternalIssuer]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelInsertDataInternalIssuer] 
	(
		@ISSUER_ID VARCHAR(20),
		@COA VARCHAR(3),
		@LASTPRIMARYMODELLOAD DATETIME,
		@LASTINDUSTRYMODELLOAD DATETIME
	)
AS
BEGIN
	
	SET NOCOUNT ON;
INSERT INTO INTERNAL_ISSUER VALUES
	(		@ISSUER_ID ,
		@COA ,
		@LASTPRIMARYMODELLOAD ,
		@LASTINDUSTRYMODELLOAD 
	)
    
	
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00107'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

