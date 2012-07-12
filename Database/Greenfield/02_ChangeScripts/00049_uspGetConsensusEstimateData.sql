set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00048'
declare @CurrentScriptVersion as nvarchar(100) = '00049'

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

CREATE PROCEDURE [dbo].[GetConsensusEstimateData] @Issuer_Id nvarchar(20),
@periodType nvarchar(10), @currency nvarchar(10)
	
AS
DECLARE 
@earnings nvarchar(20),
@netIncomeType int,
@epsktype int
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SET @earnings = (Select Earnings from tblCompanyInfo 
				where XRef IN (Select XRef from GF_SECURITY_BASEVIEW 
								where ISSUER_ID = @Issuer_Id));
								
SET @netIncomeType =   CASE @earnings  
  WHEN 'EPS' THEN 11  
  WHEN 'EPSREP' THEN 13   
  WHEN 'EBG' THEN 12  
  ELSE null 
  END
  
  SET @epsktype =   CASE @earnings  
  WHEN 'EPS' THEN 8  
  WHEN 'EPSREP' THEN 9   
  WHEN 'EBG' THEN 5  
  ELSE null
  END 	
  							
SELECT cce.ISSUER_ID
	,  cce.SECURITY_ID
	,  cce.DATA_SOURCE
	,  cce.DATA_SOURCE_DATE
	,  cce.PERIOD_TYPE
	,  cce.PERIOD_YEAR
	,  cce.PERIOD_END_DATE
	,  cce.FISCAL_TYPE
	,  cm.ESTIMATE_TYPE
	,  cm.ESTIMATE_DESC
	,  cce.CURRENCY
	,  cce.AMOUNT
	,  cce.NUMBER_OF_ESTIMATES
	,  cce.HIGH
	,  cce.LOW
	,  cce.SOURCE_CURRENCY
	,  cce.STANDARD_DEVIATION
	,  cce.AMOUNT_TYPE
  FROM dbo.CURRENT_CONSENSUS_ESTIMATES cce
 INNER JOIN dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_ID = cce.ESTIMATE_ID
 WHERE ISSUER_ID = @Issuer_Id		-- parameter from user - ISSUER_ID of selected security
   and CM.ESTIMATE_ID IN (17,7,@netIncomeType,@epsktype,18,19)	-- parameter from user - Data point selected
   and left(cce.PERIOD_TYPE,1) = left(@periodType,1)	-- Parameter from user - Annual or Quarterly
   and cce.CURRENCY = @currency           	    -- Parameter from user - Currency
   and (   (PERIOD_YEAR < DATEPART(year, getdate()) and AMOUNT_TYPE = 'ACTUAL')
		or (PERIOD_YEAR >= DATEPART(year, getdate()) and AMOUNT_TYPE = 'ESTIMATE') ) 
 order by PERIOD_YEAR, PERIOD_TYPE
  
END



GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00049'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




