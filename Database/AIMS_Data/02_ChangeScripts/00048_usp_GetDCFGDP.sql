set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00047'
declare @CurrentScriptVersion as nvarchar(100) = '00048'

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

IF OBJECT_ID ('[dbo].[GetDCFGDP]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetDCFGDP]
GO

CREATE PROCEDURE [dbo].[GetDCFGDP]
(
@COUNTRY_CODE VARCHAR(50)
) 
	
AS
BEGIN
	
	SET NOCOUNT ON;
Select LONG_TERM_GDP_GR	
from MODEL_INPUTS_CTY 
where COUNTRY_CODE=@COUNTRY_CODE
    
	
END

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00048'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

