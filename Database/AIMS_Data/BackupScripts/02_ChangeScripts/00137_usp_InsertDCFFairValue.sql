set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00136'
declare @CurrentScriptVersion as nvarchar(100) = '00137'

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

IF OBJECT_ID ('[dbo].[InsertDCFFairValue]') IS NOT NULL
	DROP PROCEDURE [dbo].[InsertDCFFairValue]
GO

/****** Object:  StoredProcedure [dbo].[InsertDCFFairValue]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertDCFFairValue]
	(
	@SECURITY_ID VARCHAR(20),
	@VALUE_TYPE VARCHAR(20),
	@FV_MEASURE INT,
	@FV_BUY DECIMAL(32,6),
	@FV_SELL DECIMAL(32,6),
	@CURRENT_MEASURE_VALUE	DECIMAL(32,6),
	@UPSIDE DECIMAL(32,6),
	@UPDATED DATETIME
	)
AS
BEGIN
	
	INSERT INTO FAIR_VALUE VALUES(@VALUE_TYPE,@SECURITY_ID,@FV_MEASURE,@FV_BUY,@FV_SELL,@CURRENT_MEASURE_VALUE,@UPSIDE,@UPDATED)
	
	RETURN @@ERROR

END


Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00137'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

