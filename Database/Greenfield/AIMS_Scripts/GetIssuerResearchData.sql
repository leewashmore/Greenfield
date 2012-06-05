USE [AshmoreEMMPOC]
GO

/****** Object:  StoredProcedure [dbo].[GetIssuerResearchData]    Script Date: 06/05/2012 10:34:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--drop proc Get_Data
Create procedure [dbo].[GetIssuerResearchData](
	@ISSUER_ID			integer					-- The company identifier		
)
as


-- Display the data


 
select rx.issuer_id
	,  sr.CurrencyReported as LocalCurrency
	,  sr.ReportNumber
	,  sr.FiscalYear
	,  s.COA
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
	, (Amount/RepToConvExRate) * (case when isnull(PeriodLengthCode,'M') = 'M' then 12 else 52 end) / isnull(PeriodLength, 12)
	  as Amount12Month 
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
 inner join dbo.REUTERS_XREF rx on rx.ReportNumber = sci.ReportNumber	-- limit the number of companies
 where 1=1
   and sr.UpdateType = 'RES'			-- Temp fix.  Need to find most recent change.
   and rx.PROVIDER = 'Y'									-- Only select data from one Reporter for the company
   and (@ISSUER_ID is null or rx.ISSUER_ID = @ISSUER_ID)	-- If ISSUER_ID is not provided run all issuers
   


-- Verify that there is work to do
declare @cnt integer
select @cnt = COUNT(*) from #Reuters

if @cnt > 0 
	BEGIN
		-- Remove any existing values.  They will be replaced below
		delete from PERIOD_FINANCIALS
		 where ISSUER_ID in (select distinct ISSUER_ID from #Reuters);
 
		-- Insert the Reuters data
		insert into PERIOD_FINANCIALS
		select r.ISSUER_ID
			,  ' ' as SECURITY_ID
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
			,  'A' as AMOUNT_TYPE
		  from #Reuters r
	END

GO

