set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00119'
declare @CurrentScriptVersion as nvarchar(100) = '00120'

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

IF OBJECT_ID ('[dbo].[ModelUpdateDataInternalIssuer]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelUpdateDataInternalIssuer]
GO

/****** Object:  StoredProcedure [dbo].[ModelUpdateDataInternalIssuer]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelUpdateDataInternalIssuer] 
	(
		@ISSUER_ID VARCHAR(20),
		@LASTPRIMARYMODELLOAD DATETIME,
		@LASTINDUSTRYMODELLOAD DATETIME
	)
AS
BEGIN
	
	SET NOCOUNT ON;
UPDATE INTERNAL_ISSUER
SET ISSUER_ID=@ISSUER_ID,LastIndustryModelLoad=@LASTINDUSTRYMODELLOAD,LastPrimaryModelLoad=@LASTPRIMARYMODELLOAD
WHERE ISSUER_ID=@ISSUER_ID
    
	
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00120'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

