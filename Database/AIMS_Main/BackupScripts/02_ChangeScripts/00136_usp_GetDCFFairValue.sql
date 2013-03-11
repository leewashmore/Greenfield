set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00135'
declare @CurrentScriptVersion as nvarchar(100) = '00136'

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

IF OBJECT_ID ('[dbo].[GetDCFFairValue]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetDCFFairValue]
GO

/****** Object:  StoredProcedure [dbo].[GetDCFFairValue]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDCFFairValue]
(
	@SECURITY_ID VARCHAR(50)
) 	
AS
BEGIN
	SET NOCOUNT ON;

SELECT * FROM PERIOD_FINANCIALS 
WHERE SECURITY_ID=@SECURITY_ID
AND PERIOD_TYPE='C' 
AND DATA_SOURCE='PRIMARY' 
AND CURRENCY='USD'
AND DATA_ID IN (187,188)

END


Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00136'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

