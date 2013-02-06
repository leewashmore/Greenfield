set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00120'
declare @CurrentScriptVersion as nvarchar(100) = '00121'

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

IF OBJECT_ID ('[dbo].[ModelUpdateInternalCOAChanges]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelUpdateInternalCOAChanges]
GO

/****** Object:  StoredProcedure [dbo].[ModelUpdateInternalCOAChanges]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelUpdateInternalCOAChanges]
	(
		@ISSUER_ID VARCHAR(20),
		@ROOT_SOURCE VARCHAR(10),
		@CURRENCY VARCHAR(3),
		@COA VARCHAR(8),
		@PERIOD_YEAR INT,
		@TIME_STAMP DATETIME
	)
AS
BEGIN
	
	SET NOCOUNT ON;

    Update INTERNAL_COA_CHANGES 
    Set END_DATE=@TIME_STAMP
    where ISSUER_ID=@ISSUER_ID
    AND ROOT_SOURCE=@ROOT_SOURCE
    AND COA=@COA
    AND PERIOD_YEAR=@PERIOD_YEAR
    AND CURRENCY=@CURRENCY
    AND END_DATE IS NULL
     
    
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00121'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

