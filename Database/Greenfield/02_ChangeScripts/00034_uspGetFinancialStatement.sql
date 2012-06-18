set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00001'
declare @CurrentScriptVersion as nvarchar(100) = '00002'

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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetFinancialStatement] 
	-- Add the parameters for the stored procedure here
	@ISSUER_ID			varchar(20),				-- The company identifier		
	@DATA_SOURCE		varchar(10)  = 'REUTERS',	-- REUTERS, PRIMARY, INDUSTRY
	@PERIOD_TYPE		char(2) = 'A',				-- A, Q
	@FISCAL_TYPE		char(8) = 'FISCAL',			-- FISCAL, CALENDAR
	@STATEMENT_TYPE		char(3) = 'BAL',			-- Type of statement to get: BAL, CAS, INC
	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT
	   pfd.GROUP_NAME
	,  pfd.BOLD_FONT
	,  pfd.SORT_ORDER
	,  pfd.DATA_DESC
	,  pf.ROOT_SOURCE
	,  pf.ROOT_SOURCE_DATE
	,  pf.AMOUNT * CASE WHEN (pfd.MULTIPLIER = 0.0) THEN 1.0 ELSE pfd.MULTIPLIER END AS AMOUNT
	,  AMOUNT_TYPE
	,  pf.PERIOD_YEAR
	,  pf.PERIOD_TYPE	
	,  pf.CALCULATION_DIAGRAM
FROM dbo.PERIOD_FINANCIALS pf
INNER JOIN dbo.PERIOD_FINANCIALS_DISPLAY pfd ON pfd.DATA_ID = pf.DATA_ID
INNER JOIN dbo.DATA_MASTER dm ON dm.DATA_ID = pf.DATA_ID
END

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






