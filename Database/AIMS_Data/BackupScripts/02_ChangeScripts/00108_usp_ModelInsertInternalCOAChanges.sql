set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00107'
declare @CurrentScriptVersion as nvarchar(100) = '00108'

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

IF OBJECT_ID ('[dbo].[ModelInsertInternalCOAChanges]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelInsertInternalCOAChanges]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertInternalCOAChanges]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelInsertInternalCOAChanges]
	(
		@ISSUER_ID VARCHAR(20),
		@ROOT_SOURCE VARCHAR(10),
		@LOAD_ID BIGINT,
		@CURRENCY VARCHAR(3),
		@COA VARCHAR(8),
		@PERIOD_YEAR INT,
		@PERIOD_END_DATE DATETIME,
		@START_DATE DATETIME,
		@END_DATE DATETIME,
		@AMOUNT DECIMAL(32,6),
		@UNITS CHAR(1)
	)
AS
BEGIN
	
	SET NOCOUNT ON;

    INSERT INTO INTERNAL_COA_CHANGES VALUES
    (
		@ISSUER_ID ,
		@ROOT_SOURCE ,
		@LOAD_ID ,
		@CURRENCY ,
		@COA ,
		@PERIOD_YEAR ,
		@PERIOD_END_DATE ,
		@START_DATE ,
		@END_DATE ,
		@AMOUNT ,
		@UNITS 
    )
     
    
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00108'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

