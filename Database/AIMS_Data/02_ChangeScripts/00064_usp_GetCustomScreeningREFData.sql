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
CREATE PROCEDURE [dbo].[GetCustomScreeningREFData]
(
@securityIdsList varchar(max)
)
AS

SET FMTONLY OFF

BEGIN
DECLARE @tempTable TABLE
(
SECURITY_ID varchar(50),
ASEC_SEC_SHORT_NAME nvarchar(255),
ISSUE_NAME nvarchar(255),
ISIN nvarchar(255),
SEDOL nvarchar(255),
SECS_INSTYPE nvarchar(255),
ASEC_INSTR_TYPE nvarchar(255),
SECURITY_TYPE nvarchar(255),
ASEC_FC_SEC_REF float,
LOOK_THRU_FUND nvarchar(255),
FIFTYTWO_WEEK_LOW float,
FIFTYTWO_WEEK_HIGH float,
SECURITY_VOLUME_AVG_90D nvarchar(255),
SECURITY_VOLUME_AVG_6M float,
SECURITY_VOLUME_AVG_30D float,
GREENFIELD_FLAG nvarchar(255),
FLOAT_AMOUNT float,
CUSIP nvarchar(255),
STOCK_EXCHANGE_ID nvarchar(255),
ASEC_ISSUED_VOLUME float,
ISSUER_ID nvarchar(50),
TRADING_CURRENCY nvarchar(255),
SHARES_OUTSTANDING float,
BETA float,
BARRA_BETA float,
TICKER nvarchar(255),
MSCI float,
BARRA nvarchar(255),
ISO_COUNTRY_CODE nvarchar(255),
ASEC_SEC_COUNTRY_NAME nvarchar(255),
ASHEMM_PROPRIETARY_REGION_CODE nvarchar(255),
ASEC_SEC_COUNTRY_ZONE_NAME nvarchar(255),
ISSUER_NAME nvarchar(255),
ASHEMM_ONE_LINER_DESCRIPTION nvarchar(255),
BLOOMBERG_DESCRIPTION ntext,
ASHMOREEMM_INDUSTRY_ANALYST nvarchar(255),
ASHMOREEMM_PRIMARY_ANALYST nvarchar(255),
ASHMOREEMM_PORTFOLIO_MANAGER nvarchar(255),
WEBSITE nvarchar(255),
FISCAL_YEAR_END nvarchar(255),
XREF float,
REPORTNUMBER nvarchar(255),
GICS_SECTOR float,
GICS_SECTOR_NAME nvarchar(255),
GICS_INDUSTRY float,
GICS_INDUSTRY_NAME nvarchar(255),
GICS_SUB_INDUSTRY float,
GICS_SUB_INDUSTRY_NAME nvarchar(255),
SHARES_PER_ADR float,
ADR_UNDERLYING_TICKER nvarchar(255),
MARKET_CAP_IN_TRADING_CURRENCY float,
CLOSING_PRICE float,
LAST_CLOSE_FX_QUO_CURR_TO_USD float,
LAST_CLOSE_DATE datetime,
TOT_CURR_SHRS_OUTST_ALL_CLASS float
)
DECLARE @sqlquery varchar(max);

if @securityIdsList is not null
begin 

set @sqlquery = 'Select SECURITY_ID,
ASEC_SEC_SHORT_NAME,
ISSUE_NAME,
ISIN,
SEDOL,
SECS_INSTYPE,
ASEC_INSTR_TYPE,
SECURITY_TYPE,
ASEC_FC_SEC_REF,
LOOK_THRU_FUND,
FIFTYTWO_WEEK_LOW,
FIFTYTWO_WEEK_HIGH,
SECURITY_VOLUME_AVG_90D,
SECURITY_VOLUME_AVG_6M,
SECURITY_VOLUME_AVG_30D,
GREENFIELD_FLAG,
FLOAT_AMOUNT,
CUSIP,
STOCK_EXCHANGE_ID,
ASEC_ISSUED_VOLUME,
ISSUER_ID,
TRADING_CURRENCY,
SHARES_OUTSTANDING,
BETA,
BARRA_BETA,
TICKER,
MSCI,
BARRA,
ISO_COUNTRY_CODE,
ASEC_SEC_COUNTRY_NAME,
ASHEMM_PROPRIETARY_REGION_CODE,
ASEC_SEC_COUNTRY_ZONE_NAME,
ISSUER_NAME,
ASHEMM_ONE_LINER_DESCRIPTION,
BLOOMBERG_DESCRIPTION ntext,
ASHMOREEMM_INDUSTRY_ANALYST,
ASHMOREEMM_PRIMARY_ANALYST,
ASHMOREEMM_PORTFOLIO_MANAGER,
WEBSITE,
FISCAL_YEAR_END,
XREF,
REPORTNUMBER,
GICS_SECTOR,
GICS_SECTOR_NAME,
GICS_INDUSTRY,
GICS_INDUSTRY_NAME,
GICS_SUB_INDUSTRY,
GICS_SUB_INDUSTRY_NAME,
SHARES_PER_ADR,
ADR_UNDERLYING_TICKER,
MARKET_CAP_IN_TRADING_CURRENCY,
CLOSING_PRICE,
LAST_CLOSE_FX_QUO_CURR_TO_USD,
LAST_CLOSE_DATE datetime,
TOT_CURR_SHRS_OUTST_ALL_CLASS
from GF_SECURITY_BASEVIEW
where SECURITY_ID IN ('+@securityIdsList+')'

end
Print @sqlquery;

INSERT INTO @tempTable  EXECUTE(@sqlquery)

Select * from @tempTable;
END 

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00064'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

