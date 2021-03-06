SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--drop proc Get_Data
alter procedure [dbo].[Get_Data2](
	@ISSUER_ID			integer					-- The company identifier		
)
as


-- Display the data
/*
select pf.ISSUER_ID, pf.DATA_SOURCE, pf.PERIOD_TYPE, pf.PERIOD_YEAR, pf.PERIOD_END_DATE
	 , pf.FISCAL_TYPE, pf.CURRENCY, pfd.BOLD_FONT, pfd.DATA_DESC, pfd.DECIMALS, pf.AMOUNT
  from dbo.PERIOD_FINANCIALS pf
 inner join dbo.PERIOD_FINANCIALS_DISPLAY pfd on pfd.DATA_ID = pf.DATA_ID
 where ISSUER_ID = @ISSUER_ID;
*/

print 'before #Reuters select'
 
select rx.issuer_id
	,  sr.CurrencyReported as LocalCurrency
	,  sr.ReportNumber
	,  sr.FiscalYear
	,  s.COA
	,  sci.COAType
	,  case when isnull(sr.RepToConvExRate, 0.0) = 0.0 then 0.0
			when isnull(s.Amount, 0.0) = 0.0 then 0.0 
			else (s.Amount/sr.RepToConvExRate) end as ReportedAmount
	,  sr.CurrencyReported
	,  sr.UpdateDate
	,  sr.PeriodEndDate
	,  sr.CurrencyConvertedTo
	,  sr.RepToConvExRate
	,  sr.PeriodLengthCode
	,  sr.PeriodLength
	,  dm.FX_CONV_TYPE
	,  dm.DATA_ID
	,  fx.FX_RATE
	,  fx.AVG90DAYRATE
	,  fx.AVG12MonthRATE
	,  (Amount/RepToConvExRate) * (case when isnull(PeriodLengthCode,'M') = 'M' then 12 else 52 end) / isnull(PeriodLength, 12)
	   as Amount12Month 
	,  sm.ISO_COUNTRY_CODE
	,  cm.CURRENCY_CODE as CountryCurrency
  into #Reuters
  from dbo.tblStd s
 inner join dbo.tblStatementRef sr on sr.RefNo = s.RefNo and sr.ReportNumber = s.ReportNumber
 inner join (select ReportNumber, RefNo, Max(UpdateDate) as UpdateDate	-- This limits the ref data to the latest version
			   from dbo.tblStatementRef
			  where 1=1 -- ReportNumber = 'A508F' --and REfNo in ('2007BAL08', '2007BAL09')
			  group by ReportNumber, RefNo) a on a.ReportNumber = sr.ReportNumber and a.RefNo = sr.RefNo and a.UpdateDate = sr.UpdateDate
 inner join dbo.tblStdCompanyInfo sci on sci.ReportNumber = s.ReportNumber
 inner join dbo.FX_RATES fx on fx.FX_DATE = sr.PeriodEndDate and fx.CURRENCY = sr.CurrencyReported
 inner join dbo.DATA_MASTER dm on dm.COA = s.COA
 inner join (select ReportNumber, max(Xref) as Xref, max(ISSUER_ID) as ISSUER_ID, MAX(security_ID) as Securitiy_ID 
			   from dbo.REUTERS_XREF group by ReportNumber) rx on rx.ReportNumber = sci.ReportNumber	-- limit the number of companies
 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
			   from GF_SECURITY_BASEVIEW group by ISSUER_ID) sm on sm.ISSUER_ID = rx.ISSUER_ID
  left join dbo.Country_Master cm on cm.COUNTRY_CODE = sm.ISO_COUNTRY_CODE
 where 1=1
   and s.COA = 'SDPR'
   and (@ISSUER_ID is null or rx.ISSUER_ID = @ISSUER_ID)	-- If ISSUER_ID is not provided run all issuers
   



print 'after select'


-- Verify that there is work to do
declare @cnt integer
select @cnt = COUNT(*) from #Reuters

if @cnt > 0 
	BEGIN
		-- Remove any existing values.  They will be replaced below
		print 'before delete'
		delete from PERIOD_FINANCIALS
		 where ISSUER_ID in (select distinct ISSUER_ID from #Reuters);
 
		print 'before insert USD1'
		-- Insert the Reuters data for those that have a USD currency
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  COAType as COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.UpdateDate as ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.FiscalYear as PERIOD_YEAR
			,  r.PeriodEndDate as PERIOD_END_DATE
			,  'FISCAL' as FISCAL_TYPE
			,  r.CurrencyReported as CURRENCY
			,  r.DATA_ID as DATA_ID
			,  r.ReportedAmount	as AMOUNT
			,  ' ' as CALCULATION_DIAGRAM
			,  r.CurrencyReported as SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #Reuters r
		 where CurrencyReported = 'USD'

		print 'before insert USD2'
		-- Insert the Reuters data for those that have a USD currency, converted to Local currency
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COAType as COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.UpdateDate as ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.FiscalYear as PERIOD_YEAR
			,  r.PeriodEndDate as PERIOD_END_DATE
			,  'FISCAL' as FISCAL_TYPE
			,  r.CountryCurrency as CURRENCY
			,  r.DATA_ID as DATA_ID
			,  case when FX_CONV_TYPE = 'PIT' then r.ReportedAmount * r.FX_RATE
					when FX_CONV_TYPE = 'AVG' then r.ReportedAmount * r.AVG12MonthRATE
					else r.ReportedAmount end
			,  ' ' as CALCULATION_DIAGRAM
			,  r.CurrencyReported as SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #Reuters r
		 where CurrencyReported = 'USD'


		-- Insert the Reuters data for those that have a non-USD currency
		print 'before insert non-USD1'
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  COAType as COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.UpdateDate as ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.FiscalYear as PERIOD_YEAR
			,  r.PeriodEndDate as PERIOD_END_DATE
			,  'FISCAL' as FISCAL_TYPE
			,  r.CurrencyReported as CURRENCY
			,  r.DATA_ID as DATA_ID
			,  r.Amount12Month	as AMOUNT
			,  ' ' as CALCULATION_DIAGRAM
			,  r.CurrencyReported as SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #Reuters r
		 where r.CurrencyReported = r.CountryCurrency

		print 'before insert non-USD2'
		-- Insert the Reuters data for those that have a non-USD currency, converted to USD
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COAType as COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.UpdateDate as ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.FiscalYear as PERIOD_YEAR
			,  r.PeriodEndDate as PERIOD_END_DATE
			,  'FISCAL' as FISCAL_TYPE
			,  'USD' as CURRENCY
			,  r.DATA_ID as DATA_ID
			,  case when FX_CONV_TYPE = 'PIT' then r.ReportedAmount / r.FX_RATE
					when FX_CONV_TYPE = 'AVG' then r.ReportedAmount / r.AVG12MonthRATE
					else r.ReportedAmount end
			,  ' ' as CALCULATION_DIAGRAM
			,  r.CurrencyReported as SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #Reuters r
		 where r.CurrencyReported = r.CountryCurrency




	-- Otherwise
		print 'before insert Otherwise1'
		-- Insert the Reuters data for those that have a USD currency, converted to Local currency
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COAType as COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.UpdateDate as ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.FiscalYear as PERIOD_YEAR
			,  r.PeriodEndDate as PERIOD_END_DATE
			,  'FISCAL' as FISCAL_TYPE
			,  'USD' as CURRENCY
			,  r.DATA_ID as DATA_ID
			,  case when FX_CONV_TYPE = 'PIT' then r.ReportedAmount / r.FX_RATE
					when FX_CONV_TYPE = 'AVG' then r.ReportedAmount / r.AVG12MonthRATE
					else r.ReportedAmount end
			,  ' ' as CALCULATION_DIAGRAM
			,  r.CurrencyReported as SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #Reuters r
		 where r.CurrencyReported <> r.CountryCurrency
		   and r.CurrencyReported <> 'USD'


	-- Calendarize
		print 'Calendarize the data'

		-- Any fiscal data where the period ends in December will be defacto Calendarized 
		-- Therefore copy that data and call it CALENDAR
		insert into PERIOD_FINANCIALS
		select pf.ISSUER_ID
			,  pf.SECURITY_ID
			,  pf.COA_TYPE
			,  pf.DATA_SOURCE
			,  pf.ROOT_SOURCE
			,  pf.ROOT_SOURCE_DATE
			,  pf.PERIOD_TYPE
			,  pf.PERIOD_YEAR
			,  pf.PERIOD_END_DATE
			,  'CALENDAR' as FISCAL_TYPE
			,  pf.CURRENCY
			,  pf.DATA_ID
			,  pf.AMOUNT
			,  pf.CALCULATION_DIAGRAM
			,  pf.SOURCE_CURRENCY
			,  pf.AMOUNT_TYPE
		  from PERIOD_FINANCIALS pf
		 where month(pf.PERIOD_END_DATE) = 12

		-- Gather the non-December end date data
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COAType as COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.UpdateDate as ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.FiscalYear as PERIOD_YEAR
			,  r.PeriodEndDate as PERIOD_END_DATE
			,  'CALENDAR' as FISCAL_TYPE
			,  'USD' as CURRENCY
			,  r.DATA_ID as DATA_ID
			,  r.Amount12Month
			,  ' ' as CALCULATION_DIAGRAM
			,  r.CurrencyReported as SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
			,  r.FX_CONV_TYPE
		  into #CAL
		  from #Reuters r
		 inner join dbo.DATA_MASTER dm on dm.DATA_ID = r.DATA_ID
		 where 1=1
		   and r.CurrencyReported <> 'USD'
		   and dm.CALENDARIZE = 'Y'
		   and month(r.PeriodEndDate) <> 12





		select c.ISSUER_ID
			,  c.SECURITY_ID
			,  c.COA_TYPE
			,  c.DATA_SOURCE
			,  c.ROOT_SOURCE
			,  c.ROOT_SOURCE_DATE
			,  c.PERIOD_TYPE
			,  c.PERIOD_YEAR
			,  c.PERIOD_END_DATE
			,  c.FISCAL_TYPE
			,  c.CURRENCY
			,  c.DATA_ID
			,  c.Amount12Month
			,  ' ' as CALCULATION_DIAGRAM
			,  c.SOURCE_CURRENCY
			,  c.AMOUNT_TYPE
			,  cp.Amount12Month as AMOUNT_NEXT
			,  case when cp.Amount12Month IS NULL then c.Amount12Month
					else (c.Amount12Month * (12-MONTH(c.PERIOD_END_DATE)) / 12)	-- last months of this row
					  +  (cp.Amount12Month *    MONTH(c.PERIOD_END_DATE) / 12)	-- first months of next ros
					end as AMOUNT
		  into #CALENDAR
		  from #CAL c
		  left join #CAL cp on cp.PERIOD_YEAR = c.PERIOD_YEAR+1
							and cp.ISSUER_ID = c.ISSUER_ID and cp.DATA_ID = c.DATA_ID
							and cp.AMOUNT_TYPE = c.AMOUNT_TYPE and cp.PERIOD_TYPE = c.PERIOD_TYPE







------------------------------------------
------------------------------------------
------------------------------------------
-- Copied from above to insert with FX  --
------------------------------------------
------------------------------------------
------------------------------------------


		print 'before insert USD1'
		-- Insert the Reuters data for those that have a USD currency
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.PERIOD_YEAR
			,  r.PERIOD_END_DATE
			,  'CALENDAR' as FISCAL_TYPE
			,  r.CURRENCY
			,  r.DATA_ID as DATA_ID
			,  r.AMOUNT
			,  ' ' as CALCULATION_DIAGRAM
			,  r.SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #CALENDAR r
		 where r.SOURCE_CURRENCY = 'USD'

		print 'before insert USD2'
		-- Insert the Reuters data for those that have a USD currency, converted to Local currency
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.PERIOD_YEAR
			,  r.PERIOD_END_DATE
			,  'CALENDAR' as FISCAL_TYPE
			,  r.CURRENCY
			,  r.DATA_ID as DATA_ID
			,  case when dm.FX_CONV_TYPE = 'PIT' then r.AMOUNT * fx.FX_RATE			-- get the value for the right date
					when dm.FX_CONV_TYPE = 'AVG' then r.AMOUNT * fx.AVG12MonthRATE	-- get the value for the right date
					else r.AMOUNT end
			,  ' ' as CALCULATION_DIAGRAM
			,  r.SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #CALENDAR r
		 inner join dbo.FX_RATES fx on fx.CURRENCY = r.SOURCE_CURRENCY and fx.FX_DATE = r.PERIOD_END_DATE
		 inner join dbo.DATA_MASTER dm on dm.DATA_ID = r.DATA_ID
		 where r.SOURCE_CURRENCY = 'USD'


		-- Insert the Reuters data for those that have a non-USD currency
		print 'before insert non-USD1'
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.PERIOD_YEAR
			,  r.PERIOD_END_DATE
			,  'CALENDAR' as FISCAL_TYPE
			,  r.CURRENCY
			,  r.DATA_ID
			,  r.AMOUNT
			,  ' ' as CALCULATION_DIAGRAM
			,  r.SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #CALENDAR r
		 where r.SOURCE_CURRENCY = r.CURRENCY

		print 'before insert non-USD2'
		-- Insert the Reuters data for those that have a non-USD currency, converted to USD
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.PERIOD_YEAR
			,  r.PERIOD_END_DATE
			,  'CALENDAR' as FISCAL_TYPE
			,  'USD' as CURRENCY
			,  r.DATA_ID as DATA_ID
			,  case when dm.FX_CONV_TYPE = 'PIT' then r.AMOUNT / fx.FX_RATE
					when dm.FX_CONV_TYPE = 'AVG' then r.AMOUNT / fx.AVG12MonthRATE
					else r.AMOUNT end
			,  ' ' as CALCULATION_DIAGRAM
			,  r.SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #CALENDAR r
		 inner join dbo.FX_RATES fx on fx.CURRENCY = r.SOURCE_CURRENCY and fx.FX_DATE = r.PERIOD_END_DATE
		 inner join dbo.DATA_MASTER dm on dm.DATA_ID = r.DATA_ID
		 where r.SOURCE_CURRENCY = r.CURRENCY




	-- Otherwise
		print 'before insert Otherwise1'
		-- Insert the Reuters data for those that have a USD currency, converted to Local currency
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
			,  r.COA_TYPE
			,  'REUTERS' as DATA_SOURCE
			,  'REUTERS' as ROOT_SOURCE
			,  r.ROOT_SOURCE_DATE
			,  'A' as PERIOD_TYPE
			,  r.PERIOD_YEAR
			,  r.PERIOD_END_DATE
			,  'CALENDAR' as FISCAL_TYPE
			,  'USD' as CURRENCY
			,  r.DATA_ID as DATA_ID
			,  case when dm.FX_CONV_TYPE = 'PIT' then r.AMOUNT / fx.FX_RATE
					when dm.FX_CONV_TYPE = 'AVG' then r.AMOUNT / fx.AVG12MonthRATE
					else r.AMOUNT end
			,  ' ' as CALCULATION_DIAGRAM
			,  r.SOURCE_CURRENCY
			,  'ACTUAL' as AMOUNT_TYPE
		  from #CALENDAR r
		 inner join dbo.FX_RATES fx on fx.CURRENCY = r.SOURCE_CURRENCY and fx.FX_DATE = r.PERIOD_END_DATE
		 inner join dbo.DATA_MASTER dm on dm.DATA_ID = r.DATA_ID
		 where r.SOURCE_CURRENCY <> r.CURRENCY
		   and r.SOURCE_CURRENCY <> 'USD'




	END
GO
