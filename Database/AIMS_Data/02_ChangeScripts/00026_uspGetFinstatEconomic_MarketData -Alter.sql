set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00025'
declare @CurrentScriptVersion as nvarchar(100) = '00026'

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

IF OBJECT_ID ('[dbo].[GetFinstatEconomic_MarketData]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetFinstatEconomic_MarketData]
GO

/****** Object:  StoredProcedure [dbo].[GetFinstatEconomic_MarketData]    Script Date: 08/23/2012 11:56:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Mansi Gupta>
-- Create date: <08/13/2012>
-- =============================================
ALTER PROCEDURE [dbo].[GetFinstatEconomic_MarketData] 
@issuerID varchar(20),
@securityId varchar(20),
@dataSource varchar(10),
@fiscalType char(8),
@currency char(3)
	
AS
BEGIN
DECLARE @currencyCode char(4),
		@countryCode nvarchar(255)

--CREATING GROUPED DATA
	--when FINSTAT_DISPLAY.DATA_ID in PF
SELECT  aa.DATA_SOURCE,
		aa.ROOT_SOURCE,
		aa.ROOT_SOURCE_DATE,
		bb.ESTIMATE_ID,
		aa.PERIOD_YEAR,
		aa.DATA_ID,
		aa.AMOUNT,
		bb.MULTIPLIER,
		bb.DECIMALS,
		bb.PERCENTAGE,
		bb.BOLD_FONT,
		bb.GROUP_NAME,
		bb.SORT_ORDER,
		bb.HARMONIC,
		bb.DATA_DESC
INTO #GroupNameData
FROM
(Select *
From PERIOD_FINANCIALS 
Where( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and PERIOD_TYPE = 'A'
and FISCAL_TYPE = @fiscalType
and CURRENCY = @currency) aa
INNER JOIN
(Select a.*, b.DATA_DESC 
from FINSTAT_DISPLAY a, DATA_MASTER b
where a.COA_TYPE = (Select COA_TYPE from dbo.INTERNAL_ISSUER where ISSUER_ID = @issuerID)
and a.DATA_ID = b.DATA_ID) bb on aa.DATA_ID = bb.DATA_ID
where bb.DATA_ID IS NOT NULL

--CREATING ECONOMIC AND MARKET DATA
SET @countryCode = (Select DISTINCT ISO_COUNTRY_CODE from GF_SECURITY_BASEVIEW 
						where SECURITY_ID = @securityId);
						
SET @currencyCode = (Select CURRENCY_CODE from Country_Master 
						where COUNTRY_CODE = @countryCode);

SELECT FX_RATE,AVG12MonthRATE,DATENAME(yyyy,FX_DATE) AS PERIOD_YEAR
INTO #EconomicMarketData
FROM FX_RATES 
WHERE CURRENCY = @currencyCode AND FX_DATE  IN 
(Select MAX(FX_DATE) From FX_RATES
where DATENAME(yyyy,FX_DATE) IN (Select DISTINCT PERIOD_YEAR from #GroupNameData)
group By DATENAME(yyyy,FX_DATE))

--MACRO_ECONOMIC DATA
SELECT FIELD,YEAR1,VALUE
INTO #Inflation FROM MACROECONOMIC_DATA where COUNTRY_CODE = @countryCode  AND 
						(FIELD = 'INFLATION_PCT' OR FIELD = 'ST_INTEREST_RATE') AND
						YEAR1  IN (Select DISTINCT PERIOD_YEAR from #GroupNameData)
						ORDER BY YEAR1;
						
--Select * from #Inflation;

Select * from #EconomicMarketData e FULL OUTER JOIN #Inflation i
ON e.PERIOD_YEAR = i.YEAR1
WHERE i.YEAR1 IS NOT NULL;

Drop table #GroupNameData,#EconomicMarketData,#Inflation;
	
END



GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00026'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
