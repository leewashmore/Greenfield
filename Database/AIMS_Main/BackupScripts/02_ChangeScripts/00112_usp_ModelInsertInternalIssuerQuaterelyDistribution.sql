set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00111'
declare @CurrentScriptVersion as nvarchar(100) = '00112'

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

IF OBJECT_ID ('[dbo].[ModelInsertInternalIssuerQuaterelyDistribution]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelInsertInternalIssuerQuaterelyDistribution]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertInternalIssuerQuaterelyDistribution]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelInsertInternalIssuerQuaterelyDistribution]
(
@ISSUER_ID VARCHAR(20),
@DATA_SOURCE VARCHAR(20),
@PERIOD_TYPE CHAR(2),
@PERCENTAGE DECIMAL(32,6),
@LAST_UPDATED DATETIME
) 

AS
BEGIN
	
	INSERT INTO INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION VALUES
	(
		@ISSUER_ID ,
		@DATA_SOURCE ,
		@PERIOD_TYPE ,
		@PERCENTAGE ,
		@LAST_UPDATED 
	)
	RETURN @@ERROR
	
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00112'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

