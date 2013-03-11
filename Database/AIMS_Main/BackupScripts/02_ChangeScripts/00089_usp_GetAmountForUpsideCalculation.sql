set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00088'
declare @CurrentScriptVersion as nvarchar(100) = '00089'

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

IF OBJECT_ID ('[dbo].[GetAmountForUpsideCalculation]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetAmountForUpsideCalculation]
GO

CREATE PROCEDURE GetAmountForUpsideCalculation
(
	@SECURITY_ID VARCHAR(20),
	@DATA_ID     INT, 									   
	@DATA_SOURCE VARCHAR(10) 
)
AS
BEGIN
	
SET NOCOUNT ON;

DECLARE @amount DECIMAL(32,6)

 SELECT TOP 1 @amount = (PERIOD_FINANCIALS.AMOUNT)
 FROM PERIOD_FINANCIALS
 WHERE SECURITY_ID = '2340' AND DATA_ID = 188 
 AND DATA_SOURCE = 'PRIMARY'
 AND CURRENCY = 'USD' AND PERIOD_TYPE = 'C'

IF @@ROWCOUNT=0
BEGIN
 SET @amount = 0
END

SELECT @amount
   
END
GO

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00089'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
