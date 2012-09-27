set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00095'
declare @CurrentScriptVersion as nvarchar(100) = '00096'

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

IF OBJECT_ID ('[dbo].[RetrieveDataPointsModelUpload]') IS NOT NULL
	DROP PROCEDURE [dbo].[RetrieveDataPointsModelUpload]
GO

CREATE PROCEDURE [dbo].[RetrieveDataPointsModelUpload](
@ISSUER_ID VARCHAR(50)
	)
AS

DECLARE @COA_TYPE VARCHAR(50)
BEGIN
	SET @COA_TYPE= (Select TOP 1 [COA_TYPE] from INTERNAL_ISSUER where ISSUER_ID=@ISSUER_ID);
	SET NOCOUNT ON;

   SELECT dm.COA as COA
   ,pfd.SORT_ORDER as SORT_ORDER
   ,pfd.DATA_DESC as DATA_DESCRIPTION 
   FROM PERIOD_FINANCIALS_DISPLAY pfd
   Inner Join DATA_MASTER dm on pfd.DATA_ID=dm.DATA_ID
   Where pfd.STATEMENT_TYPE in ('BAL','INC','CAS') and pfd.COA_TYPE=@COA_TYPE 
   ORDER BY SORT_ORDER 
END

Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00096'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




