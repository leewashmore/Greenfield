set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00050'
declare @CurrentScriptVersion as nvarchar(100) = '00051'

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

ALTER PROCEDURE [dbo].[GetDCFCashFlow]
(
@ISSUER_ID varchar(50)
)
AS
DECLARE @PERIOD_YEARS TABLE(YEAR INT) 
DECLARE @INC INT=0
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SET @INC=0
WHILE @INC<5
BEGIN
INSERT INTO @PERIOD_YEARS VALUES(YEAR(GETDATE())+@INC)
SET @INC=@INC+1
END



    -- Insert statements for procedure here
	SELECT AMOUNT,
	PERIOD_YEAR,
	0.0 as DISCOUNTING_FACTOR,
	0.0 as FREE_CASH_FLOW 
	FROM PERIOD_FINANCIALS pf
		
	WHERE ISSUER_ID=@ISSUER_ID
	AND DATA_SOURCE='PRIMARY'
	AND PERIOD_TYPE='A'
	AND PERIOD_YEAR IN (SELECT * FROM @PERIOD_YEARS)
	AND CURRENCY='USD'
	AND FISCAL_TYPE='CALENDAR'
	AND DATA_ID=157
	
END

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00051'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

