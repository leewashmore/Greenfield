set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00007'
declare @CurrentScriptVersion as nvarchar(100) = '00008'

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

IF OBJECT_ID ('[dbo].[Get_Statement]') IS NOT NULL
	DROP PROCEDURE [dbo].[Get_Statement]
GO

Create PROCEDURE [dbo].[Get_Statement](
	@ISSUER_ID			varchar(20),				-- The company identifier		
	@DATA_SOURCE		varchar(10)  = 'REUTERS',	-- REUTERS, PRIMARY, INDUSTRY
	@PERIOD_TYPE		char(2)      = 'A',			-- A, Q
	@FISCAL_TYPE		char(8)      = 'FISCAL',	-- FISCAL, CALENDAR
	@STATEMENT_TYPE		char(3)      = 'BAL',		-- Type of statement to get: BAL, CAS, INC
	@CURRENCY			char(3)	     = 'USD'		-- USD or the currency of the country (local)
)
AS
--------------------------------------------------------------------------------------
-- Select the data for the statement
--------------------------------------------------------------------------------------
SELECT pfd.GROUP_NAME						AS [GroupName]
	,  pf.DATA_ID							AS [DataId]
	,  pfd.BOLD_FONT						AS [BoldFont]
	,  pfd.DECIMALS							AS [Decimals]
	,  pf.ROOT_SOURCE						AS [RootSource]
	,  pf.ROOT_SOURCE_DATE					AS [RootSourceDate]
	,  pf.CALCULATION_DIAGRAM				AS [CalculationDiagram]
	,  pfd.DATA_DESC						AS [Description]
	,  pfd.SORT_ORDER						AS [SortOrder]
	,  pf.PERIOD_YEAR						AS [PeriodYear]
	,  pf.PERIOD_TYPE						AS [PeriodType]
	,  pf.AMOUNT * CASE 
	   WHEN (pfd.MULTIPLIER = 0.0) THEN 1.0 
	   ELSE pfd.MULTIPLIER END				AS [Amount]
	,  pf.AMOUNT_TYPE						AS [AmountType]
	,  'N'									AS [IsConsensus]

INTO #STATEMENTS_DATA

FROM dbo.PERIOD_FINANCIALS pf
INNER JOIN dbo.PERIOD_FINANCIALS_DISPLAY pfd 
	ON pfd.DATA_ID = pf.DATA_ID AND pfd.COA_TYPE = pf.COA_TYPE
INNER JOIN dbo.DATA_MASTER dm 
	ON dm.DATA_ID = pf.DATA_ID 

WHERE 1=1
	AND pf.ISSUER_ID = @ISSUER_ID
	AND pf.DATA_SOURCE = @DATA_SOURCE
	AND LEFT(pf.PERIOD_TYPE,1) = @PERIOD_TYPE
	AND pf.FISCAL_TYPE = @FISCAL_TYPE
	AND pf.CURRENCY = @CURRENCY
	AND pfd.STATEMENT_TYPE = @STATEMENT_TYPE

ORDER BY pfd.SORT_ORDER, PERIOD_YEAR

--------------------------------------------------------------------------------------
-- Select the data for the external research grid
--------------------------------------------------------------------------------------
DECLARE @EARNINGS VARCHAR(10)

SELECT @EARNINGS = CI.Earnings
FROM [Reuters].[dbo].[tblCompanyInfo] CI
INNER JOIN [dbo].[GF_SECURITY_BASEVIEW] GSB
	ON GSB.XREF = CI.XRef
WHERE GSB.ISSUER_ID = @ISSUER_ID

SELECT ''					AS [GroupName]
	, CCE.ESTIMATE_ID		AS [DataId]
	, 'N'					AS [BoldFont]
	, 0						AS [Decimals]
	, CCE.DATA_SOURCE   	AS [RootSource]
	, CCE.DATA_SOURCE_DATE	AS [RootSourceDate]
	, ''					AS [CalculationDiagram]
	, SCM.[DESCRIPTION]		AS [Description]
	, SCM.SORT_ORDER		AS [SortOrder]
	, CCE.PERIOD_YEAR		AS [PeriodYear]
	, CCE.PERIOD_TYPE		AS [PeriodType]
	, CCE.AMOUNT			AS [Amount]
	, CCE.AMOUNT_TYPE		AS [AmountType]
	, 'Y'					AS [IsConsensus]
INTO #CONSENSUS_ACTUAL_DATA
FROM dbo.CURRENT_CONSENSUS_ESTIMATES CCE
INNER JOIN dbo.STATEMENT_CONSENSUS_MAPPING SCM 
	ON SCM.ESTIMATE_ID = CCE.ESTIMATE_ID

WHERE SCM.STATEMENT_TYPE = @STATEMENT_TYPE
	AND CCE.ISSUER_ID = @ISSUER_ID
	AND CCE.DATA_SOURCE = @DATA_SOURCE
	AND LEFT(CCE.PERIOD_TYPE, 1) = @PERIOD_TYPE
	AND CCE.FISCAL_TYPE = @FISCAL_TYPE
	AND CCE.CURRENCY = @CURRENCY
	AND CCE.AMOUNT_TYPE = 'ACTUAL'
	AND (SCM.EARNINGS = @EARNINGS OR SCM.EARNINGS IS NULL)

SELECT ''					AS [GroupName]
	, CCE.ESTIMATE_ID		AS [DataId]
	, 'N'					AS [BoldFont]
	, 0						AS [Decimals]
	, CCE.DATA_SOURCE   	AS [RootSource]
	, CCE.DATA_SOURCE_DATE	AS [RootSourceDate]
	, ''					AS [CalculationDiagram]
	, SCM.[DESCRIPTION]		AS [Description]
	, SCM.SORT_ORDER		AS [SortOrder]
	, CCE.PERIOD_YEAR		AS [PeriodYear]
	, CCE.PERIOD_TYPE		AS [PeriodType]
	, CCE.AMOUNT			AS [Amount]
	, CCE.AMOUNT_TYPE		AS [AmountType]
	, 'Y'					AS [IsConsensus]
INTO #CONSENSUS_ESTIMATE_DATA
FROM dbo.CURRENT_CONSENSUS_ESTIMATES CCE
INNER JOIN dbo.STATEMENT_CONSENSUS_MAPPING SCM 
	ON SCM.ESTIMATE_ID = CCE.ESTIMATE_ID

WHERE SCM.STATEMENT_TYPE = @STATEMENT_TYPE
	AND (SCM.EARNINGS = @EARNINGS OR SCM.EARNINGS IS NULL
	AND CCE.ISSUER_ID = @ISSUER_ID
	AND CCE.DATA_SOURCE = @DATA_SOURCE
	AND LEFT(CCE.PERIOD_TYPE, 1) = @PERIOD_TYPE
	AND CCE.FISCAL_TYPE = @FISCAL_TYPE
	AND CCE.CURRENCY = @CURRENCY
	AND CCE.AMOUNT_TYPE = 'ESTIMATE'
	AND CCE.PERIOD_YEAR NOT IN (SELECT DISTINCT [PeriodYear] FROM #CONSENSUS_ACTUAL_DATA)
	)

SELECT * FROM #STATEMENTS_DATA
UNION
SELECT * FROM #CONSENSUS_ACTUAL_DATA
UNION
SELECT * FROM #CONSENSUS_ESTIMATE_DATA
ORDER BY SortOrder, PeriodYear

DROP TABLE #STATEMENTS_DATA
DROP TABLE #CONSENSUS_ACTUAL_DATA
DROP TABLE #CONSENSUS_ESTIMATE_DATA


GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00008'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



