IF OBJECT_ID ( 'Get_Data_Annual', 'P' ) IS NOT NULL 
DROP PROCEDURE Get_Data_Annual;
GO
-----------------------------------------------------------------------------------
-- Purpose:	To move the data from the Reuters raw tables into the display tables:
--			PERIOD_FINANCIALS.  This procedure works as an interface to load the 
--			Reuters data received in file format into the AIMS database.
--
-- Author:	David Muench
-- Date:	July 2, 2012
-----------------------------------------------------------------------------------

create procedure Get_Data_Annual(
	@ISSUER_ID			varchar(20) = NULL					-- The company identifier		
)
as

	print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before #Reuters select'
	 
	select rx.issuer_id
		,  sr.CurrencyReported as LocalCurrency
		,  sr.ReportNumber
		,  sr.FiscalYear
		,  s.COA
		,  sci.COAType
		,  case when isnull(sr.RepToConvExRate, 0.0) = 0.0 then 0.0
				when isnull(s.Amount, 0.0) = 0.0 then 0.0 
				else (s.Amount/(case when sr.RepToConvExRate <> 0.0 then sr.RepToConvExRate else 1.0 end)) end as ReportedAmount
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
		,  (Amount/case when RepToConvExRate <> 0.0 then RepToConvExRate else 1.0 end) * (case when isnull(sr.PeriodLengthCode,'M') = 'M' then 12 else 52 end) / isnull(sr.PeriodLength, 12)
		   as Amount12Month 
		,  sm.ISO_COUNTRY_CODE
		,  cm.CURRENCY_CODE as CountryCurrency
	  into #Reuters
	  from Reuters.dbo.tblStd s
	 inner join Reuters.dbo.tblStatementRef sr on sr.RefNo = s.RefNo and sr.ReportNumber = s.ReportNumber
	 inner join (select ReportNumber, RefNo, Max(UpdateDate) as UpdateDate	-- This limits the ref data to the latest version
				   from Reuters.dbo.tblStatementRef
				  where 1=1 -- ReportNumber = 'A508F' --and REfNo in ('2007BAL08', '2007BAL09')
				  group by ReportNumber, RefNo) a on a.ReportNumber = sr.ReportNumber and a.RefNo = sr.RefNo and a.UpdateDate = sr.UpdateDate
	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = s.ReportNumber
	  left join dbo.FX_RATES fx on fx.FX_DATE = sr.PeriodEndDate and fx.CURRENCY = sr.CurrencyReported
	 inner join dbo.DATA_MASTER dm on dm.COA = s.COA
	 inner join (select ReportNumber, max(Xref) as Xref, max(ISSUER_ID) as ISSUER_ID, MAX(security_ID) as Securitiy_ID 
				   from dbo.GF_SECURITY_BASEVIEW group by ReportNumber) rx on rx.ReportNumber = sci.ReportNumber	-- limit the number of companies
	 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
				   from GF_SECURITY_BASEVIEW group by ISSUER_ID) sm on sm.ISSUER_ID = rx.ISSUER_ID
	  left join dbo.Country_Master cm on cm.COUNTRY_CODE = sm.ISO_COUNTRY_CODE
	-- Limit to only the latest data
	 inner join (select sr.ReportNumber, sr.RefNo, sr.FiscalYear, sr.StatementType, sr.PeriodLengthCode, sr.PeriodLength, sr.CurrencyReported
					 ,  MAX(sr.UpdateDate) as UpdateDate
				   from Reuters.dbo.tblStatementRef sr
				  inner join (select ReportNumber, max(Xref) as Xref, max(ISSUER_ID) as ISSUER_ID, MAX(security_ID) as Securitiy_ID 
			 					from dbo.GF_SECURITY_BASEVIEW group by ReportNumber) rx on rx.ReportNumber = sr.ReportNumber	-- limit the number of companies
				  where 1=1
					and (@ISSUER_ID is null or rx.ISSUER_ID = @ISSUER_ID)	-- If ISSUER_ID is not provided run all issuers
				  group by sr.ReportNumber, sr.RefNo, sr.FiscalYear, sr.StatementType, sr.PeriodLengthCode, sr.PeriodLength, sr.CurrencyReported
				) l on l.ReportNumber = sr.ReportNumber and l.RefNo = sr.RefNo and l.FiscalYear = sr.FiscalYear 
				   and l.StatementType = sr.StatementType and l.CurrencyReported  = sr.CurrencyReported  
-- don't need	   and l.PeriodLengthCode = sr.PeriodLengthCode and l.PeriodLength = sr.PeriodLength 
	 where 1=1
	   and (@ISSUER_ID is null or rx.ISSUER_ID = @ISSUER_ID)	-- If ISSUER_ID is not provided run all issuers

/*
select 'Get_Data_Annual #Reuters' as GDA, * 
  from #Reuters
 order by ISSUER_ID, FiscalYear, DATA_ID, COA
*/
	print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - after select'


	-- Verify that there is work to do
	declare @cnt integer
	select @cnt = COUNT(*) from #Reuters

	if @cnt > 0 
		BEGIN
			-- Remove any existing values.  They will be replaced below
/*			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before delete'
			delete from PERIOD_FINANCIALS
			 where ISSUER_ID in (select distinct ISSUER_ID from #Reuters);
			delete from PERIOD_FINANCIALS
			 where SECURITY_ID in (select distinct SECURITY_ID from GF_SECURITY_BASEVIEW where ISSUER_ID in (select distinct ISSUER_ID from #Reuters));
*/
			-- Create a temp table that mirrors PERIOD_FINAINCIALS
			select * 
			  into #PF
			  from PERIOD_FINANCIALS
			 where 1=0
	 
			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert USD1'
			-- Insert the Reuters data for those that have a USD currency
			insert into #PF --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
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

			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert USD2'
			-- Insert the Reuters data for those that have a USD currency, converted to Local currency
			insert into #PF  --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
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
			   and isnull(r.FX_RATE, 0.0) <> 0.0
			   and isnull(r.AVG12MonthRATE, 0.0) <> 0.0


			-- Insert the Reuters data for those that have a non-USD currency
			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert non-USD1'
			insert into #PF  --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
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

			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert non-USD2'
			-- Insert the Reuters data for those that have a non-USD currency, converted to USD
			insert into #PF  --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
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
			   and isnull(r.FX_RATE, 0.0) <> 0.0
			   and isnull(r.AVG12MonthRATE, 0.0) <> 0.0




		-- Otherwise
			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before insert Otherwise1'
			-- Insert the Reuters data for those that have a USD currency, converted to Local currency
			insert into #PF  --PERIOD_FINANCIALS
			select r.ISSUER_ID
				,  ' ' as SECURITY_ID
				,  ' ' as COA_TYPE
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
			   and isnull(r.FX_RATE, 0.0) <> 0.0
			   and isnull(r.AVG12MonthRATE, 0.0) <> 0.0

			-- Copy Consensus data in where the year is missing from PERIOD_FINANCIALS
			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Copy Consensus data '
			insert into #PF  --dbo.PERIOD_FINANCIALS
						 (ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE,
						  ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, 
						  FISCAL_TYPE, CURRENCY, DATA_ID, AMOUNT, CALCULATION_DIAGRAM, 
						  SOURCE_CURRENCY, AMOUNT_TYPE)
			select cce.ISSUER_ID
				,  cce.SECURITY_ID
				,  ' ' as COA_TYPE
				,  cce.DATA_SOURCE
				,  'CONSENSUS' as ROOT_SOURCE
				,  cce.DATA_SOURCE_DATE as ROOT_SOURCE_DATE
				,  cce.PERIOD_TYPE
				,  cce.PERIOD_YEAR
				,  cce.PERIOD_END_DATE
				,  cce.FISCAL_TYPE
				,  cce.CURRENCY
				,  case ESTIMATE_ID when  2 then 118
									when  6 then 29
									when 11 then 44
									when 14 then 36
									when 17 then 11
									when  1 then 104
									when  3 then 117
									when  4 then 124
											end as DATA_ID
				,  case ESTIMATE_ID when  1 then cce.AMOUNT * ci.OutstandingShares
									when  3 then cce.AMOUNT * ci.OutstandingShares
									when  4 then cce.AMOUNT * ci.OutstandingShares
											else cce.AMOUNT end as AMOUNT
				,  ' ' asCALCULATION_DIAGRAM
				,  cce.SOURCE_CURRENCY
				,  cce.AMOUNT_TYPE
			  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
			 inner join (select cce.ISSUER_ID, cce.PERIOD_YEAR, min(cce.AMOUNT_TYPE) as AMOUNT_TYPE
						   from dbo.CURRENT_CONSENSUS_ESTIMATES cce
						   left join #PF pf --dbo.PERIOD_FINANCIALS pf 
										on pf.ISSUER_ID = cce.ISSUER_ID and pf.FISCAL_TYPE = cce.FISCAL_TYPE
										and pf.PERIOD_YEAR = cce.PERIOD_YEAR and pf.PERIOD_TYPE = cce.PERIOD_TYPE
						  where cce.PERIOD_TYPE = 'A'
							and pf.DATA_ID is null
							and (@ISSUER_ID is null or cce.ISSUER_ID = @ISSUER_ID)	-- If ISSUER_ID is not provided run all issuers
						  group by cce.ISSUER_ID, cce.PERIOD_YEAR
						) a on a.ISSUER_ID = cce.ISSUER_ID and a.PERIOD_YEAR = cce.PERIOD_YEAR and a.AMOUNT_TYPE = cce.AMOUNT_TYPE
			 inner join (select ISSUER_ID, XREF from dbo.GF_SECURITY_BASEVIEW) sb on sb.ISSUER_ID = cce.ISSUER_ID
			 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = sb.XREF
			 where cce.ESTIMATE_ID in (2, 6, 11, 14, 17, 1, 3, 4)
			   and (@ISSUER_ID is null or cce.ISSUER_ID = @ISSUER_ID)	-- If ISSUER_ID is not provided run all issuers
			   and cce.PERIOD_TYPE = 'A'

/* Moved to a seperate stored procedure
			-- Calendarize
			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before Calendarize'
			insert into #PF  --PERIOD_FINANCIALS
			select pfc.ISSUER_ID
				,  pfc.SECURITY_ID
				,  ' ' as COA_TYPE
				,  pfc.DATA_SOURCE
				,  pfc.ROOT_SOURCE
				,  pfc.ROOT_SOURCE_DATE
				,  pfc.PERIOD_TYPE
				,  pfc.PERIOD_YEAR
				,  pfc.PERIOD_END_DATE
				,  'CALENDAR' as FISCAL_TYPE
				,  pfc.CURRENCY
				,  pfc.DATA_ID
				,  (pfc.AMOUNT  * datepart(month, sr.PeriodEndDate)/12)
					+  case when pfn.DATA_ID is not null 
								then (pfn.AMOUNT * (12-datepart(month, sr.PeriodEndDate)/12))
							else (pfc.AMOUNT * (12-datepart(month, sr.PeriodEndDate)/12)) end as AMOUNT
				,  pfc.CALCULATION_DIAGRAM
				,  pfc.SOURCE_CURRENCY
				,  pfc.AMOUNT_TYPE
			  from #PF pfc  -- dbo.PERIOD_FINANCIALS pfc
			  left join #PF pfn  -- dbo.PERIOD_FINANCIALS pfn 
						 on pfn.ISSUER_ID = pfc.ISSUER_ID     and pfn.AMOUNT_TYPE = pfc.AMOUNT_TYPE
						 and pfn.DATA_ID = pfc.DATA_ID         and pfn.CURRENCY = pfc.CURRENCY
						 and pfn.DATA_SOURCE = pfc.DATA_SOURCE and pfn.FISCAL_TYPE = pfc.FISCAL_TYPE
						 and pfn.PERIOD_TYPE = pfc.PERIOD_TYPE and pfn.PERIOD_YEAR = pfc.PERIOD_YEAR
						 and pfn.SECURITY_ID = pfc.SECURITY_ID 
			 inner join dbo.DATA_MASTER dm on dm.DATA_ID = pfc.DATA_ID
			 inner join (select distinct ISSUER_ID, ReportNumber 
						   from dbo.GF_SECURITY_BASEVIEW
						) sb on sb.ISSUER_ID = pfc.ISSUER_ID
			 inner join (select distinct ReportNumber, PeriodLength, PeriodEndDate 
						   from Reuters.dbo.tblStatementRef
						  where PeriodLength is not NULL
						) sr on sr.ReportNumber = sb.ReportNumber and sr.PeriodEndDate = pfc.PERIOD_END_DATE
			 where dm.CALENDARIZE = 'Y'
			   and (@ISSUER_ID is null or sb.ISSUER_ID = @ISSUER_ID)	-- If ISSUER_ID is not provided run all issuers
			 order by pfc.ISSUER_ID, pfc.AMOUNT_TYPE, pfc.DATA_SOURCE, pfc.CURRENCY, pfc.PERIOD_YEAR, pfc.PERIOD_TYPE, pfc.DATA_ID
*/

			begin transaction		-- Lock the table
			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - before delete'
			
			-- Delete the existing data
			delete from PERIOD_FINANCIALS
			 where ISSUER_ID in (select distinct ISSUER_ID from #Reuters);
			delete from PERIOD_FINANCIALS
			 where SECURITY_ID in (select distinct SECURITY_ID from GF_SECURITY_BASEVIEW where ISSUER_ID in (select distinct ISSUER_ID from #Reuters));

			-- Copy the data from the temp table into the real table
			insert into dbo.PERIOD_FINANCIALS
			select * from #PF

			commit					-- Save everything and unlock
			drop table #PF			-- Clean up

			print '>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Done'
			
			
		END

go
--  exec Get_Data 125631 -- this issuer has a period end date of Sept. 30
/*
select *
  from PERIOD_FINANCIALS
 where ISSUER_ID = 125631
 order by DATA_ID, PERIOD_YEAR, FISCAL_TYPE
*/