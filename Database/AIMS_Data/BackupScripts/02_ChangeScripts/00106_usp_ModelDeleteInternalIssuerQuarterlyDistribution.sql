set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00105'
declare @CurrentScriptVersion as nvarchar(100) = '00106'

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

IF OBJECT_ID ('[dbo].[ModelDeleteInternalIssuerQuarterlyDistribution]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelDeleteInternalIssuerQuarterlyDistribution]
GO

/****** Object:  StoredProcedure [dbo].[ModelDeleteInternalIssuerQuarterlyDistribution]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelDeleteInternalIssuerQuarterlyDistribution]
	(
		@ISSUER_ID VARCHAR(20),
		@DATA_SOURCE VARCHAR(10)
	)
AS
BEGIN
	
	SET NOCOUNT ON;

	DELETE FROM INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION 
	WHERE ISSUER_ID=@ISSUER_ID AND DATA_SOURCE=@DATA_SOURCE
    
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00106'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

