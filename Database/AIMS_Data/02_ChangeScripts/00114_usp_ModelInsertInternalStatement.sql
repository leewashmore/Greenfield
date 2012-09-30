set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00113'
declare @CurrentScriptVersion as nvarchar(100) = '00114'

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

IF OBJECT_ID ('[dbo].[ModelInsertInternalStatement]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelInsertInternalStatement]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertInternalStatement]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelInsertInternalStatement] 
	(
		@ISSUER_ID VARCHAR(50),
		@REF_NO VARCHAR(20),
		@PERIOD_YEAR INT,
		@ROOT_SOURCE VARCHAR(10),
		@ROOT_SOURCE_DATE DATETIME,
		@PERIOD_LENGTH INT,
		@PERIOD_END_DATE DATETIME,
		@CURRENCY CHAR(3),
		@AMOUNT_TYPE CHAR(10)
	)
AS
BEGIN	
	
	SET NOCOUNT ON;
	
	INSERT INTO INTERNAL_STATEMENT VALUES(
		@ISSUER_ID ,
		@REF_NO ,
		@PERIOD_YEAR ,
		@ROOT_SOURCE ,
		@ROOT_SOURCE_DATE ,
		@PERIOD_LENGTH ,
		@PERIOD_END_DATE ,
		@CURRENCY ,
		@AMOUNT_TYPE )
    
    RETURN @@ERROR
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00114'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

