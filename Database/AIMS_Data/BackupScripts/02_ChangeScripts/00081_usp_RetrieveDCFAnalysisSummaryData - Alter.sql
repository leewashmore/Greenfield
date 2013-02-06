set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00080'
declare @CurrentScriptVersion as nvarchar(100) = '00081'

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

IF OBJECT_ID ('[dbo].[usp_RetrieveDCFAnalysisSummaryData]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_RetrieveDCFAnalysisSummaryData]
GO

CREATE PROCEDURE [dbo].[usp_RetrieveDCFAnalysisSummaryData](
                @ISSUER_ID                                      varchar(20)                    -- The company identifier
,               @DATA_SOURCE                             varchar(10)  = 'PRIMARY'              -- REUTERS, PRIMARY, INDUSTRY
,               @PERIOD_TYPE                                char(2) = 'C'                      -- A, Q
,               @FISCAL_TYPE                                char(8) = 'FISCAL'                 -- FISCAL, CALENDAR
,               @CURRENCY                                      char(3)  = 'USD'                -- USD or the currency of the country (local)
)
AS
BEGIN
                -- SET NOCOUNT ON added to prevent extra result sets from
                -- interfering with SELECT statements.
                SET NOCOUNT ON;

    -- Insert statements for procedure here
                SELECT *
                FROM PERIOD_FINANCIALS pf
                WHERE
                pf.ISSUER_ID=@ISSUER_ID AND
                pf.DATA_SOURCE=@DATA_SOURCE AND
                pf.PERIOD_TYPE=@PERIOD_TYPE AND
                pf.DATA_ID in (232,289) AND
                pf.FISCAL_TYPE=@FISCAL_TYPE AND
                PF.PERIOD_YEAR=YEAR(gETDATE()) AND
                pf.CURRENCY=@CURRENCY
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00081'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




