set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00096'
declare @CurrentScriptVersion as nvarchar(100) = '00097'

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

IF OBJECT_ID ('[dbo].[GetModelConsensusEstimates]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetModelConsensusEstimates]
GO

CREATE procedure [dbo].[GetModelConsensusEstimates](
	@ISSUER_ID			varchar(20)					-- The company identifier		
,	@DATA_SOURCE		varchar(10)  = 'REUTERS'	-- REUTERS, PRIMARY, INDUSTRY
,	@PERIOD_TYPE		char(2) = 'A'				-- A, Q
,	@FISCAL_TYPE		char(8) = 'FISCAL'			-- FISCAL, CALENDAR
,	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
)
as
Begin	
	-- Select the Estimated data
	select cce.ISSUER_ID
		,  cce.ESTIMATE_ID
		,  cm.ESTIMATE_DESC
		,  substring(cce.AMOUNT_TYPE,1,1)+cast(cce.PERIOD_YEAR as CHAR(4))+cce.PERIOD_TYPE as Period
		,  cce.AMOUNT_TYPE
		,  cce.PERIOD_YEAR
		,  cce.PERIOD_TYPE
		,  cce.AMOUNT
		,  cce.NUMBER_OF_ESTIMATES
		,  cce.HIGH
		,  cce.LOW
		,  cce.STANDARD_DEVIATION
		,  cce.SOURCE_CURRENCY
		,  cce.DATA_SOURCE
		,  cce.DATA_SOURCE_DATE
		,  0 as SortOrder
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_ID = cce.ESTIMATE_ID
	 where cce.ISSUER_ID = @ISSUER_ID
	   and cce.DATA_SOURCE = @DATA_SOURCE
	   and cce.ESTIMATE_ID in (17,7,6,14,11,8,10,1,18,19,2,3,4)
	   and substring(cce.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and cce.FISCAL_TYPE = @FISCAL_TYPE
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ESTIMATE'
	 order by cm.ESTIMATE_DESC, cce.PERIOD_YEAR,cce.PERIOD_TYPE
	END 

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00097'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




