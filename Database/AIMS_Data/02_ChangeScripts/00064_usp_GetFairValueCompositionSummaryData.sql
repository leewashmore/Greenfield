set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00063'
declare @CurrentScriptVersion as nvarchar(100) = '00064'

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

IF OBJECT_ID ('[dbo].[[GetFairValueCompositionSummaryData]]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetFairValueCompositionSummaryData]
GO
CREATE PROCEDURE [dbo].[GetFairValueCompositionSummaryData]
	@SECURITY_ID varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT b.VALUE_TYPE as SOURCE
    ,a.DATA_DESC as MEASURE
    ,b.FV_BUY as BUY
    ,b.FV_SELL as SELL
    ,b.UPSIDE as UPSIDE
    ,b.UPDATED as DATE
    ,a.DATA_ID as DATA_ID
    from DATA_MASTER a
	inner join  FAIR_VALUE b 
	ON
	a.DATA_ID = b.FV_MEASURE 
	WHERE SECURITY_ID=@SECURITY_ID
END
GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00064'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
