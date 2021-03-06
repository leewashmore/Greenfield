SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[usp_GetCurrencyDataForCountries]
(
@countryCodes varchar(max)
)
AS

SET FMTONLY OFF

BEGIN
DECLARE @tempCurrencyCodes TABLE
(
CurrencyCode varchar(8)
)
DECLARE @fxRatesData TABLE
(
CurrencyCode varchar(8)
)
declare @sqlquery varchar(max)

if @countryCodes is not null
begin
 set @sqlquery = 'Select CURRENCY_CODE from dbo.Country_Master 
where ( COUNTRY_CODE in  ('+@countryCodes+'))'
end
INSERT INTO @tempCurrencyCodes  EXECUTE(@sqlquery)
select cm.COUNTRY_CODE,fr.CURRENCY,fr.FX_DATE,fr.FX_RATE from FX_RATES fr
left outer join Country_Master cm
on fr.CURRENCY = cm.CURRENCY_CODE
where CURRENCY in (select CurrencyCode from @tempCurrencyCodes)
END
GO
