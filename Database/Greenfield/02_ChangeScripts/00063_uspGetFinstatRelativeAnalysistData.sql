--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00062'
declare @CurrentScriptVersion as nvarchar(100) = '00063'
--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 return
--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
 RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
 return
end
GO

/****** Object:  StoredProcedure [dbo].[GetFinstatRelativeAnalysisData]    Script Date: 08/23/2012 11:58:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





-- =============================================
-- Author:		<Mansi Gupta>
-- Create date: <08/13/2012>
-- =============================================
CREATE PROCEDURE [dbo].[GetFinstatRelativeAnalysisData] 
@issuerID varchar(20),
@securityId varchar(20),
@dataSource varchar(10),
@fiscalType char(8)
	
AS
BEGIN
DECLARE @countryCode nvarchar(255),
	    @benchmarkId nvarchar(255),
	    @gicsIndustry nvarchar(255)

---------------------------------STEP 1
Select p.DATA_ID,p.PERIOD_YEAR,p.AMOUNT,f.DECIMALS,f.MULTIPLIER,f.PERCENTAGE,'step1' as VALUE
INTO #A
From (Select * from PERIOD_FINANCIALS 
Where( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and PERIOD_TYPE = 'A'
and FISCAL_TYPE = @fiscalType
and CURRENCY = 'USD'
AND DATA_ID IN(44,166,164,133))p

INNER JOIN
(Select *
from FINSTAT_DISPLAY
where COA_TYPE = (Select COA_TYPE from dbo.INTERNAL_ISSUER where ISSUER_ID = @issuerID)
AND DATA_ID IN (44,166,164,133)) f 
ON p.DATA_ID = f.DATA_ID


----------------------------------STEP 2
--ACTUAL DATA
Select ESTIMATE_ID,PERIOD_YEAR,AMOUNT into #CCE_Actual 
From CURRENT_CONSENSUS_ESTIMATES 
Where( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId) 
and DATA_SOURCE = @dataSource
and PERIOD_TYPE = 'A'
and FISCAL_TYPE = @fiscalType
and CURRENCY = 'USD'
and AMOUNT_TYPE = 'ACTUAL'
and ESTIMATE_ID IN (11,166,164,19)
ORDER BY PERIOD_YEAR;

--ESTIMATE DATA
Select ESTIMATE_ID,PERIOD_YEAR,AMOUNT into #CCE_Estimate
From CURRENT_CONSENSUS_ESTIMATES 
Where( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId) 
and DATA_SOURCE = @dataSource
and PERIOD_TYPE = 'A'
and FISCAL_TYPE = @fiscalType
and CURRENCY = 'USD'
and ESTIMATE_ID IN (11,166,164,19)
and PERIOD_YEAR not in (select PERIOD_YEAR from #CCE_Actual) 
order by PERIOD_YEAR;

 --ACTUAL DATA AND ESTIMATE DATA COMBINED
 Select * into #CCEDATA 
 from #CCE_Estimate cce union (select * from #CCE_Actual cca)
 order by PERIOD_YEAR;
 
 --JOIN CCEDATA WITH COA SPECIFIC DATA
 Select cce.*,f.DECIMALS,f.MULTIPLIER,f.PERCENTAGE,'step2' as VALUE 
 INTO #B
 from #CCEDATA cce
 INNER JOIN
(Select * from FINSTAT_DISPLAY
where COA_TYPE = (Select COA_TYPE from dbo.INTERNAL_ISSUER where ISSUER_ID = @issuerID)
AND ESTIMATE_ID IN (11,166,164,19)) f 
ON cce.ESTIMATE_ID = f.ESTIMATE_ID;
 
 -------------------------------------STEP 3
 SET @countryCode = (Select DISTINCT ISO_COUNTRY_CODE from GF_SECURITY_BASEVIEW 
						where SECURITY_ID = @securityId);
			
Select b.*,f.DECIMALS,f.MULTIPLIER,f.PERCENTAGE,'step3' as VALUE
INTO #C
from 
(Select DATA_ID,PERIOD_YEAR,AMOUNT from BENCHMARK_NODE_FINANCIALS
where BENCHMARK_ID = 'MSCI EM NET'
AND NODE_NAME1 = 'COUNTRY'
and NODE_ID1 = @countryCode
and NODE_NAME2 IS NULL 
and DATA_ID IN (166,164,133)
and PERIOD_TYPE = 'A'
and CURRENCY = 'USD') b

INNER JOIN
(Select *
from FINSTAT_DISPLAY
where COA_TYPE = (Select COA_TYPE from dbo.INTERNAL_ISSUER where ISSUER_ID = @issuerID)
AND DATA_ID IN (44,166,164,133)) f 
ON b.DATA_ID = f.DATA_ID

---------------------------------------STEP 5
 SET @gicsIndustry = (Select DISTINCT GICS_INDUSTRY from GF_SECURITY_BASEVIEW 
						where SECURITY_ID = @securityId);

Select b.*,f.DECIMALS,f.MULTIPLIER,f.PERCENTAGE,'step5' as VALUE 
INTO #D
from 
(Select DATA_ID,PERIOD_YEAR,AMOUNT from BENCHMARK_NODE_FINANCIALS
where BENCHMARK_ID = 'MSCI EM NET'
AND NODE_NAME1 = 'INDUSTRY'
and NODE_ID1 = @gicsIndustry
and NODE_NAME2 IS NULL 
and DATA_ID IN (166,164,133)
and PERIOD_TYPE = 'A'
and CURRENCY = 'USD') b

INNER JOIN
(Select *
from FINSTAT_DISPLAY
where COA_TYPE = (Select COA_TYPE from dbo.INTERNAL_ISSUER where ISSUER_ID = @issuerID)
AND DATA_ID IN (44,166,164,133)) f 
ON b.DATA_ID = f.DATA_ID

--------------------------------UNION ALL DATA SETS
Select * from #A Union 
(Select * from #B) Union 
(Select * from #C) Union 
(Select * from #D)
order by VALUE
  
  -----------------------------DROP ALL TEMPORARY TABLES
 drop table #CCEDATA,#CCE_Actual,#CCE_Estimate,#A,#B,#C,#D; 

END

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00063'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

