set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00041'
declare @CurrentScriptVersion as nvarchar(100) = '00042'

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

IF OBJECT_ID ('[dbo].[GetDCFMarketCap]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetDCFMarketCap]
GO

CREATE PROCEDURE [dbo].[GetDCFMarketCap]
(
@SECURITY_ID varchar(50)
) 
AS
BEGIN
	SELECT AMOUNT FROM PERIOD_FINANCIALS WHERE
	SECURITY_ID=@SECURITY_ID 
	AND PERIOD_TYPE='C'
	AND CURRENCY='USD'
	AND DATA_ID=185
	
END


GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00042'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

