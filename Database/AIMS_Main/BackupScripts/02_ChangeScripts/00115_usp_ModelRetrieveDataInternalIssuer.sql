set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00114'
declare @CurrentScriptVersion as nvarchar(100) = '00115'

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

IF OBJECT_ID ('[dbo].[ModelRetrieveDataInternalIssuer]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelRetrieveDataInternalIssuer]
GO

/****** Object:  StoredProcedure [dbo].[ModelRetrieveDataInternalIssuer]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelRetrieveDataInternalIssuer] 
	(
		@ISSUER_ID VARCHAR(20)
	)
AS
BEGIN
	
	SET NOCOUNT ON;
SELECT COUNT(*) 
FROM INTERNAL_ISSUER 
WHERE ISSUER_ID=@ISSUER_ID
    
	
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00115'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

