set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00110'
declare @CurrentScriptVersion as nvarchar(100) = '00111'

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

IF OBJECT_ID ('[dbo].[ModelInsertInternalIssuer]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelInsertInternalIssuer]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertInternalIssuer]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelInsertInternalIssuer] 
	(
		@ISSUER_ID VARCHAR(20),
		@COA_TYPE VARCHAR(3),
		@LASTPRIMARYMODELLOAD DATETIME,
		@LASTINDUSTRYMODELLOAD DATETIME
	)
AS
BEGIN
	
	SET NOCOUNT ON;

INSERT INTO INTERNAL_ISSUER VALUES
(
		@ISSUER_ID ,
		@COA_TYPE ,
		@LASTPRIMARYMODELLOAD ,
		@LASTINDUSTRYMODELLOAD 
)
   
   RETURN @@ERROR 
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00111'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

