-----------------------------------------------------------------------------------
-- Purpose:	To take the raw interim data provided by Reuters and make quarterly 
--			data out of it.  This procedure implements an interface from the data
--			provided by Reuters into the AIMS database.
--
-- Author:	David Muench
-- Date:	July 23, 2012
-----------------------------------------------------------------------------------

IF OBJECT_ID ( 'Get_Data_Quarterly', 'P' ) IS NOT NULL 
DROP PROCEDURE Get_Data_Quarterly;
GO

CREATE procedure Get_Data_Quarterly(
	@ISSUER_ID			varchar(20) = NULL
,	@VERBOSE			char		= 'Y'
)
as


	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()

	-- Get the constant information about this Issuer.
	declare @XREF	varchar(20)
	declare @ReportNumber	varchar(20)
	declare @ISO_COUNTRY_CODE	varchar(3)
	select @XREF = max(XREF)
		,  @ReportNumber = max(REPORTNUMBER)
		,  @ISO_COUNTRY_CODE = max(ISO_COUNTRY_CODE)
	  from dbo.GF_SECURITY_BASEVIEW
	 where ISSUER_ID = @ISSUER_ID
	 group by ISSUER_ID

	declare @CURRENCY_CODE	varchar(3)
	select @CURRENCY_CODE = max(CURRENCY_CODE)
	  from Country_Master
	 where COUNTRY_CODE = @ISO_COUNTRY_CODE


		-- Get COAs for each ISSUER
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Create COAs'

		select @ISSUER_ID as ISSUER_ID
			,  dm.COA
			,  dm.FX_CONV_TYPE
			,  dm.DATA_ID
			,  @ISO_COUNTRY_CODE as COUNTRY_CODE
			,  @CURRENCY_CODE as CURRENCY_CODE
			,  sir.UpdateDate
			,  sir.PeriodEndDate
			,  sir.CurrencyConvertedTo
			,  sir.CurrencyReported
			,  sir.RepToConvExRate
			,  'M' as PeriodLengthCode
			,  case when sir.PeriodLengthCode = 'W' and sir.PeriodLength in (12,13,14) then 3
					when sir.PeriodLengthCode = 'W' and sir.PeriodLength in (25,26,27) then 6
					when sir.PeriodLengthCode = 'W' and sir.PeriodLength in (38,39,40) then 9
					when sir.PeriodLengthCode = 'W' and sir.PeriodLength in (51,52,53) then 12
					else sir.PeriodLength end as PeriodLength
			,  si.Amount
			,  sir.StatementType
			,  sir.FiscalYear as PERIOD_YEAR
			,  sir.interimnumber

		  into #COAs
		  from Reuters.dbo.tblStd s															-- Get the listed COAs
		 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = s.ReportNumber	-- Get the COAtype
		 inner join dbo.DATA_MASTER dm on dm.COA = s.COA							-- Allow only selected COAs
									  and (  (sci.COAType = 'IND' and dm.INDUSTRIAL = 'Y')
										  or (sci.COAType = 'FIN' and dm.INSURANCE = 'Y')
										  or (sci.COAType = 'UTL' and dm.UTILITY = 'Y')
										  or (sci.COAType = 'BNK' and dm.BANK = 'Y')
										  )
--									  and 'Y' = case sci.COAType when 'IND' then dm.INDUSTRIAL
--																 when 'FIN' then dm.INSURANCE
--																 when 'UTL' then dm.UTILITY
--																 when 'BNK' then dm.BANK
--																 else 'N' end
		 inner join Reuters.dbo.tblStdInterim si on si.ReportNumber = s.ReportNumber and si.COA = s.COA
		 inner join Reuters.dbo.tblStdInterimRef sir on sir.ReportNumber = si.ReportNumber and sir.RefNo = si.RefNo

		 inner join dbo.FX_RATES fx on fx.FX_DATE = sir.PeriodEndDate and fx.CURRENCY = sir.CurrencyReported

/*		 inner join (select ReportNumber, max(Xref) as Xref, max(ISSUER_ID) as ISSUER_ID, MAX(security_ID) as Securitiy_ID 
					   from dbo.GF_SECURITY_BASEVIEW group by ReportNumber) rx on rx.ReportNumber = sci.ReportNumber	-- limit the number of companies
		 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
					   from GF_SECURITY_BASEVIEW group by ISSUER_ID) sm on sm.ISSUER_ID = rx.ISSUER_ID
		  left join dbo.Country_Master cm on cm.COUNTRY_CODE = sm.ISO_COUNTRY_CODE
*/		 where 1=1
		   and s.ReportNumber = @ReportNumber
		   and (   sir.StatementType = 'BAL'
				or sir.PeriodLengthCode = 'M'
				or ((sir.PeriodLengthCode = 'W') and (sir.PeriodLength IN (12,13,14,25,26,27,38,39,40,51,52,53)))
			   )
		   and @CURRENCY_CODE is not NULL


	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Create COAs' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


/*
	print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Update COAs'

		-- Make any weekly values into monthly.
		update #COAs
		   set PeriodLengthCode = 'M'
			,  PeriodLength = case when PeriodLength in (12,13,14) then 3
								   when PeriodLength in (25,26,27) then 6
								   when PeriodLength in (38,39,40) then 9
								   when PeriodLength in (51,52,53) then 12 end
		 where (PeriodLengthCode = 'W' and PeriodLength IN (12,13,14,25,26,27,38,39,40,51,52,53))
*/

	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Pivot the data'

		-- Pivot the data so that it is easier to use.
		select ISSUER_ID
			, COA
			, FX_CONV_TYPE
			, DATA_ID
			, COUNTRY_CODE
			, CURRENCY_CODE
			, CurrencyConvertedTo
			, CurrencyReported
			, RepToConvExRate
			, StatementType
			, PERIOD_YEAR
			, max(UpdateDate1) as UpdateDate1
			, max(UpdateDate2) as UpdateDate2
			, max(UpdateDate3) as UpdateDate3
			, max(UpdateDate4) as UpdateDate4
			, max(PeriodEndDate1) as PeriodEndDate1
			, max(PeriodEndDate2) as PeriodEndDate2
			, max(PeriodEndDate3) as PeriodEndDate3
			, max(PeriodEndDate4) as PeriodEndDate4
			, sum(PeriodLength1) as PeriodLength1
			, sum(PeriodLength2) as PeriodLength2
			, sum(PeriodLength3) as PeriodLength3
			, sum(PeriodLength4) as PeriodLength4
			, SUM(Amount1) as Amount1
			, SUM(Amount2) as Amount2
			, SUM(Amount3) as Amount3
			, SUM(Amount4) as Amount4
		  into #OUT
		  from (select ISSUER_ID
					, COA
					, FX_CONV_TYPE
					, DATA_ID
					, COUNTRY_CODE
					, CURRENCY_CODE
					, CurrencyConvertedTo
					, CurrencyReported
					, RepToConvExRate
					, StatementType
					, PERIOD_YEAR
					, case interimnumber when 1 then UpdateDate else null end as UpdateDate1
					, case interimnumber when 2 then UpdateDate else null end as UpdateDate2
					, case interimnumber when 3 then UpdateDate else null end as UpdateDate3
					, case interimnumber when 4 then UpdateDate else null end as UpdateDate4
					, case interimnumber when 1 then PeriodEndDate else null end as PeriodEndDate1
					, case interimnumber when 2 then PeriodEndDate else null end as PeriodEndDate2
					, case interimnumber when 3 then PeriodEndDate else null end as PeriodEndDate3
					, case interimnumber when 4 then PeriodEndDate else null end as PeriodEndDate4
					, case interimnumber when 1 then max(PeriodLength) else null end as PeriodLength1
					, case interimnumber when 2 then max(PeriodLength) else null end as PeriodLength2
					, case interimnumber when 3 then max(PeriodLength) else null end as PeriodLength3
					, case interimnumber when 4 then max(PeriodLength) else null end as PeriodLength4
					, case interimnumber when 1 then sum(Amount) else 0.0 end as Amount1
					, case interimnumber when 2 then sum(Amount) else 0.0 end as Amount2
					, case interimnumber when 3 then sum(Amount) else 0.0 end as Amount3
					, case interimnumber when 4 then sum(Amount) else 0.0 end as Amount4
				  from #COAs
				 where ISSUER_ID = @ISSUER_ID
				 group by ISSUER_ID
					, COA
					, FX_CONV_TYPE
					, DATA_ID
					, COUNTRY_CODE
					, CURRENCY_CODE
					, UpdateDate
					, PeriodEndDate
					, CurrencyConvertedTo
					, CurrencyReported
					, RepToConvExRate
					, StatementType
					, PERIOD_YEAR
					, interimnumber
				) a
		 group by ISSUER_ID
			, COA
			, FX_CONV_TYPE
			, DATA_ID
			, COUNTRY_CODE
			, CURRENCY_CODE
			, CurrencyConvertedTo
			, CurrencyReported
			, RepToConvExRate
			, StatementType
			, PERIOD_YEAR

		-- Done collecting, now output.
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Pivot the data' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


		-------------------
		-- Interim number 1
		-------------------
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 1 a'
		
		-- Insert the USD currency
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate1 as Root_Source_Date
			, 'Q1' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate1 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT1
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT1 / fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT1 / fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate1 and fx.CURRENCY = 'USD'
		 where PeriodLength1 is null or PeriodLength1 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 1 a' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		-- Insert the local currency
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 1 b'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate1 as Root_Source_Date
			, 'Q1' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate1 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT1
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT1 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT1 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate1 and fx.CURRENCY = o.CURRENCY_CODE
		 where PeriodLength1 is null or PeriodLength1 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 1 b' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		 
		-- Need to log data from interim 1 that is not periodlength - 3

		-------------------
		-- Interim number 2
		-------------------
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2 a'
		-- Insert the USD currency for the ones that have 3 months in the period
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate2 as Root_Source_Date
			, 'Q2' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate2 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT2 / fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT2 / fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = 'USD'
		 where PeriodLength2 is null or PeriodLength2 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2 a' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		-- Insert the local currency
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2 b'
	
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate2 as Root_Source_Date
			, 'Q2' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate2 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT2 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT2 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = o.CURRENCY_CODE
		 where PeriodLength2 is null or PeriodLength2 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2 b' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


		-- Insert the USD currency for the ones with 6 month periods
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2 c'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate1 as Root_Source_Date
			, 'Q2' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate2 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT2 / fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT2 / fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = 'USD'
		 where PeriodLength2 = 6
		   and PeriodLength1 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2 c' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		-- Insert the local currency
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2 d'
	
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate1 as Root_Source_Date
			, 'Q2' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate2 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT2 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT2 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = o.CURRENCY_CODE
		 where PeriodLength2 = 6
		   and PeriodLength1 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2 d' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



		-- Insert the USD currency for the ones with 6 month periods
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2 e'
	
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate2 as Root_Source_Date
			, 'Q2' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate2 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT2 / 2 / fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT2 / 2 / fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = 'USD'
		 where PeriodLength2 = 6
		   and (PeriodLength1 is null or PeriodLength1 <> 3)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2 e' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		-- Insert the local currency
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2 f'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate2 as Root_Source_Date
			, 'Q2' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate2 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2 / 2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT2 / 2 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT2 / 2 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = o.CURRENCY_CODE
		 where PeriodLength2 = 6
		   and (PeriodLength1 is null or PeriodLength1 <> 3)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2 f' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



		-- Insert the USD currency for the ones with 6 month periods - Q1
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2 g'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate2 as Root_Source_Date
			, 'Q1' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate2 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT2 / 2 / fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT2 / 2 / fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = 'USD'
		 where PeriodLength2 = 6
		   and (PeriodLength1 is null or PeriodLength1 <> 3)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2 g' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		-- Insert the local currency - Q1
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2 h'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate2 as Root_Source_Date
			, 'Q1' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate2 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2 / 2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT2 / 2 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT2 / 2 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = o.CURRENCY_CODE
		 where PeriodLength2 = 6
		   and (PeriodLength1 is null or PeriodLength1 <> 3)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2 h' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



		--------------------
		-- Interim number 3
		--------------------
		-- Insert the USD currency for the ones that have 3 months in the period
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3 a'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate3 as Root_Source_Date
			, 'Q3' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate3 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT3
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT3 / fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT3 / fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate3 and fx.CURRENCY = 'USD'
		 where PeriodLength3 is null or PeriodLength3 = 3


	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3 a' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		-- Insert the local currency
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3 b'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate3 as Root_Source_Date
			, 'Q3' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate3 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT3
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT3 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT3 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate3 and fx.CURRENCY = o.CURRENCY_CODE
		 where PeriodLength3 is null or PeriodLength3 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3 b' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



		--------------------
		-- Interim number 3
		--------------------
		-- Insert the USD currency for the ones that have 3 months in the period
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4 a'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate4 as Root_Source_Date
			, 'Q4' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate4 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT4
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT4 / fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT4 / fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate4 and fx.CURRENCY = 'USD'
		 where PeriodLength4 is null or PeriodLength4 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4 a' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		-- Insert the local currency
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4 b'
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECUIRTY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate4 as Root_Source_Date
			, 'Q4' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, o.PeriodEndDate4 as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT4
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then o.AMOUNT4 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then o.AMOUNT4 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate4 and fx.CURRENCY = o.CURRENCY_CODE
		 where PeriodLength4 is null or PeriodLength4 = 3

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4 b' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

--------------------------
-- Copy Consensus Data
--------------------------

	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - copy consensus data'
	-- Get a temp list of Period Financial data that relates to this Issuer	
	select * 
	  into #PF
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.ISSUER_ID = @ISSUER_ID
	   and pf.DATA_SOURCE = 'REUTERS'

	if @VERBOSE = 'Y' print 'After #PF' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()
	 -- Index it for an easier join
	 create index PF_idx on #PF(ISSUER_ID ,FISCAL_TYPE, PERIOD_YEAR, PERIOD_TYPE)

	if @VERBOSE = 'Y' print 'After #PF Index' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Collect only the Quarterly data from the Consensus table the is NOT in Period Financials.	 	
	select cce.ISSUER_ID, cce.PERIOD_YEAR, cce.PERIOD_TYPE, min(cce.AMOUNT_TYPE) as AMOUNT_TYPE
	  into #CCE
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
	  left join #PF pf on pf.ISSUER_ID = cce.ISSUER_ID and pf.FISCAL_TYPE = cce.FISCAL_TYPE
					  and pf.PERIOD_YEAR = cce.PERIOD_YEAR and pf.PERIOD_TYPE = cce.PERIOD_TYPE
	 where 1=1
--	   and substring(cce.PERIOD_TYPE,1,1) = 'Q'
	   and cce.PERIOD_TYPE like 'Q%'
	   and pf.DATA_ID is null
	   and cce.ISSUER_ID = @ISSUER_ID
	 group by cce.ISSUER_ID, cce.PERIOD_YEAR, cce.PERIOD_TYPE	

				 
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After collect consensus data' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END
	
	
	-- Copy Consensus data in where the year&quarter is missing from PERIOD_FINANCIALS
	insert into dbo.PERIOD_FINANCIALS (ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE,
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
		,  case ESTIMATE_ID when  1 then cce.AMOUNT * ci.OutstandingShares / 1000000
							when  3 then cce.AMOUNT * ci.OutstandingShares / 1000000
							when  4 then cce.AMOUNT * ci.OutstandingShares / 1000000
									else cce.AMOUNT end as AMOUNT
		,  ' ' asCALCULATION_DIAGRAM
		,  cce.SOURCE_CURRENCY
		,  cce.AMOUNT_TYPE
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
/*	 inner join (select cce.ISSUER_ID, cce.PERIOD_YEAR, cce.PERIOD_TYPE, min(cce.AMOUNT_TYPE) as AMOUNT_TYPE
				   from dbo.CURRENT_CONSENSUS_ESTIMATES cce
				   left join dbo.PERIOD_FINANCIALS pf on pf.ISSUER_ID = cce.ISSUER_ID and pf.FISCAL_TYPE = cce.FISCAL_TYPE
													 and pf.PERIOD_YEAR = cce.PERIOD_YEAR and pf.PERIOD_TYPE = cce.PERIOD_TYPE
				  where substring(cce.PERIOD_TYPE,1,1) = 'Q'
				    and pf.DATA_ID is null
				  group by cce.ISSUER_ID, cce.PERIOD_YEAR, cce.PERIOD_TYPE
				) a 
*/	inner join #CCE a
				on a.ISSUER_ID = cce.ISSUER_ID and a.PERIOD_YEAR = cce.PERIOD_YEAR 
					and a.PERIOD_TYPE = cce.PERIOD_TYPE and a.AMOUNT_TYPE = cce.AMOUNT_TYPE
	 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = @XREF
	 where cce.ESTIMATE_ID in (2, 6, 11, 14, 17, 1, 3, 4)
	   and substring(cce.PERIOD_TYPE,1,1) = 'Q'



	-- Copy Consensus data in where the year&quarter is missing from PERIOD_FINANCIALS
	insert into dbo.PERIOD_FINANCIALS (ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE,
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
		,  47 as DATA_ID
		,  cce.AMOUNT 
		,  ' ' asCALCULATION_DIAGRAM
		,  cce.SOURCE_CURRENCY
		,  cce.AMOUNT_TYPE
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
	 inner join #CCE a
				on a.ISSUER_ID = cce.ISSUER_ID and a.PERIOD_YEAR = cce.PERIOD_YEAR 
					and a.PERIOD_TYPE = cce.PERIOD_TYPE and a.AMOUNT_TYPE = cce.AMOUNT_TYPE
	 where cce.ESTIMATE_ID = 11
	   and substring(cce.PERIOD_TYPE,1,1) = 'Q'




	if @VERBOSE = 'Y' 
		BEGIN
			print 'After copy consensus data' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		-- clean up temp tables
		drop table #COAs;
		drop table #OUT;
	 
go
--  exec Get_Data_Quarterly 223340
  
