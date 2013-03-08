set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00109'
declare @CurrentScriptVersion as nvarchar(100) = '00110'

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

IF OBJECT_ID ('[dbo].[ModelInsertInternalData]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelInsertInternalData]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertInternalData]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelInsertInternalData] 
	(
		@ISSUER_ID VARCHAR(20),
		@REF_NO VARCHAR(20),
		@PERIOD_TYPE CHAR(2),
		@COA CHAR(8),
		@AMOUNT DECIMAL(32,6)
	)
AS
BEGIN
	
	INSERT INTO INTERNAL_DATA VALUES
	(	@ISSUER_ID ,
		@REF_NO ,
		@PERIOD_TYPE ,
		@COA ,
		@AMOUNT 
	)
	
	RETURN @@ERROR
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00110'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

