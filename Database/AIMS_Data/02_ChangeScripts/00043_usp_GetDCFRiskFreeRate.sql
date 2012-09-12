set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00042'
declare @CurrentScriptVersion as nvarchar(100) = '00043'

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

IF OBJECT_ID ('[dbo].[GetDCFRiskFreeRate]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetDCFRiskFreeRate]
GO

CREATE PROCEDURE [dbo].[GetDCFRiskFreeRate] 
	(
	@COUNTRY_CODE VARCHAR(40) =''  
	 ) 
AS
BEGIN
	
	SET NOCOUNT ON;

    SELECT * 
    FROM MODEL_INPUTS_CTY
    WHERE COUNTRY_CODE=@COUNTRY_CODE
END

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00043'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

