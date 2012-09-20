set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00079'
declare @CurrentScriptVersion as nvarchar(100) = '00080'

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

IF OBJECT_ID ('[dbo].[GetDCFSummaryData]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetDCFSummaryData]
GO

CREATE PROCEDURE [dbo].[GetDCFSummaryData]
(
@ISSUER_ID varchar(50)
)
AS
DECLARE @CURRENT_YEAR INT

BEGIN

SET @CURRENT_YEAR= YEAR(GETDATE()); 
	SET NOCOUNT ON;
SELECT AMOUNT, DATA_ID FROM PERIOD_FINANCIALS
WHERE ISSUER_ID=@ISSUER_ID
AND DATA_SOURCE='PRIMARY'
AND PERIOD_TYPE='C'
AND CURRENCY='USD'
AND DATA_ID IN (255,258,256,257)
    
END




GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00080'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())