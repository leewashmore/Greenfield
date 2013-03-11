set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00074'
declare @CurrentScriptVersion as nvarchar(100) = '00075'

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

IF OBJECT_ID ('[dbo].[GetDCF_ROIC]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetDCF_ROIC]
GO

CREATE PROCEDURE [dbo].[GetDCF_ROIC] 
(
	@ISSUER_ID			varchar(20)					-- The company identifier
,	@YEAR int 										-- The value of Year for which data is to be fetched
,	@DATA_SOURCE		varchar(10)  = 'PRIMARY'	-- REUTERS, PRIMARY, INDUSTRY
,	@PERIOD_TYPE		char(2) = 'A'				-- A, Q
,	@FISCAL_TYPE		char(8) = 'FISCAL'			-- FISCAL, CALENDAR
,	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
 
)
AS
BEGIN
	
	SET NOCOUNT ON;

Select 
		Amount as AMOUNT,
		DATA_ID as DATA_ID,
		FISCAL_TYPE as FISCAL
from PERIOD_FINANCIALS pf
where pf.ISSUER_ID = @ISSUER_ID
	AND pf.DATA_SOURCE = @DATA_SOURCE
	AND LEFT(pf.PERIOD_TYPE,1) = @PERIOD_TYPE
	AND (pf.FISCAL_TYPE = @FISCAL_TYPE OR PF.FISCAL_TYPE='CALENDAR')
	AND pf.CURRENCY = @CURRENCY
	AND pf.DATA_ID in (162,141)
	AND pf.PERIOD_YEAR=@YEAR
   
END


GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00075'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())