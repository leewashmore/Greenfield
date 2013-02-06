set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00104'
declare @CurrentScriptVersion as nvarchar(100) = '00105'

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

IF OBJECT_ID ('[dbo].[ModelDeleteInternalIssuer]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelDeleteInternalIssuer]
GO

/****** Object:  StoredProcedure [dbo].[ModelDeleteInternalIssuer]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelDeleteInternalIssuer] 
	(
		@ISSUER_ID VARCHAR(20)
	)
AS
BEGIN
	
	DELETE FROM INTERNAL_ISSUER 
	WHERE ISSUER_ID=@ISSUER_ID
	
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00105'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

