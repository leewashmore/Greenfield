SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetDCF_ROIC] 
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
