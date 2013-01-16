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
	 
	 

	--figure out which statements have duplicates due to FYE changes
	select distinct fiscalyear 
	into #MultipleCheck
		from(select ReportNumber, FiscalYear, interimnumber,StatementType, COUNT(*) as tally, MAX(PeriodEndDate) as MaxPeriod, MIN(PeriodEndDate) as MinPeriod
			from Reuters..tblStdInterimRef 
			where ReportNumber = @ReportNumber
			group by ReportNumber, FiscalYear, interimnumber, StatementType) a 
	where a.tally > 1	


	-- Get COAs for each ISSUER where there are no duplicates due to FYE changes
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Create COAs: No Dupes'

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
			,  si.Amount/sir.RepToConvExRate as Amount 
			,  sir.StatementType
			,  sir.FiscalYear as PERIOD_YEAR
			,  sir.interimnumber
			, fx.FX_RATE
			, fx.AVG90DAYRATE
		
		  into #COAs
		  from Reuters..tblStdInterim si														-- Get the listed COAs
		 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = si.ReportNumber	-- Get the COAtype
		 inner join dbo.DATA_MASTER dm on dm.COA = si.COA							-- Allow only selected COAs
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
		 inner join Reuters.dbo.tblStdInterimRef sir on sir.ReportNumber = si.ReportNumber and sir.RefNo = si.RefNo

		 inner join dbo.FX_RATES fx on fx.FX_DATE = sir.PeriodEndDate and fx.CURRENCY = sir.CurrencyReported

		 left join #MultipleCheck mc on sir.FiscalYear = mc.FiscalYear 
		where 1=1
		   and si.ReportNumber = @ReportNumber
		   and (   sir.StatementType = 'BAL'
				or ((sir.PeriodLengthCode = 'M') and (sir.PeriodLength IN (3,6,9,12)))
				or ((sir.PeriodLengthCode = 'W') and (sir.PeriodLength IN (12,13,14,25,26,27,38,39,40,51,52,53)))
			   )
		   and @CURRENCY_CODE is not NULL
		   and mc.FiscalYear is NULL  --ensure only statements without duplicates are taken	   
		   		   
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Create COAs w/no Duplicates' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


	select ROW_NUMBER() OVER(PARTITION BY sir.FiscalYear, sir.StatementType ORDER BY sir.PeriodEndDate DESC) AS "RowNumber", sir.*	
	into #Partition
	from Reuters..tblStdInterimRef sir
	inner join #MultipleCheck mc on sir.FiscalYear = mc.FiscalYear
	where sir.ReportNumber = @ReportNumber 

	drop table #MultipleCheck

	-- Get COAs for each ISSUER where there are duplicates due to FYE changes and taking the most recent 4 statements, adjusting the interim number as necessary
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Create COAs: Dupes'

		insert into #COAs 
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
			,  si.Amount/sir.RepToConvExRate as Amount 
			,  sir.StatementType
			,  sir.FiscalYear as PERIOD_YEAR
			,  case when p.RowNumber = 1 then 4
					when p.RowNumber = 2 then 3
					when p.RowNumber = 3 then 2
					when p.RowNumber = 4 then 1 
					else sir.InterimNumber end as InterimNumber
			--, sir.interimnumber	
			, fx.FX_RATE
			, fx.AVG90DAYRATE
		from Reuters..tblStdInterim si														-- Get the listed COAs
		 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = si.ReportNumber	-- Get the COAtype
		 inner join dbo.DATA_MASTER dm on dm.COA = si.COA							-- Allow only selected COAs
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
		 inner join Reuters.dbo.tblStdInterimRef sir on sir.ReportNumber = si.ReportNumber and sir.RefNo = si.RefNo

		 inner join dbo.FX_RATES fx on fx.FX_DATE = sir.PeriodEndDate and fx.CURRENCY = sir.CurrencyReported

		 inner join #Partition p on  si.ReportNumber = p.ReportNumber and si.RefNo = p.RefNo
		where 1=1
		   and si.ReportNumber = @ReportNumber
		   and (   sir.StatementType = 'BAL'
				or ((sir.PeriodLengthCode = 'M') and (sir.PeriodLength IN (3,6,9,12)))
				or ((sir.PeriodLengthCode = 'W') and (sir.PeriodLength IN (12,13,14,25,26,27,38,39,40,51,52,53)))
			   )
		   and @CURRENCY_CODE is not NULL
		   and p.RowNumber < 5

		   		   
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Create COAs w/ Duplicates' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	drop table #Partition


if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Pivot the data'	
-- ???? Test for FYE change...what happens? Can we do something for FYE changes?  i.e., take the last 4 periods and re-label as 1,2,3,4 ????
	-- Pivot the data so that it is easier to use.

	select ISSUER_ID
			, COA
			, FX_CONV_TYPE
			, DATA_ID
			, COUNTRY_CODE
			, CURRENCY_CODE
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
			, SUM(FXRate1) as FXRate1
			, SUM(FXRate2) as FXRate2
			, SUM(FXRate3) as FXRate3
			, SUM(FXRate4) as FXRate4
			, SUM(FXAvgRate1) as FXAvgRate1
			, SUM(FXAvgRate2) as FXAvgRate2
			, SUM(FXAvgRate3) as FXAvgRate3
			, SUM(FXAvgRate4) as FXAvgRate4
			, MAX(CurrencyReported1) as CurrencyReported1
			, MAX(CurrencyReported2) as CurrencyReported2
			, MAX(CurrencyReported3) as CurrencyReported3
			, MAX(CurrencyReported4) as CurrencyReported4
		  into #OUT
		  from (select ISSUER_ID
					, COA
					, FX_CONV_TYPE
					, DATA_ID
					, COUNTRY_CODE
					, CURRENCY_CODE
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
					, case interimnumber when 1 then sum(FX_RATE) else 0.0 end as FXRate1
					, case interimnumber when 2 then sum(FX_RATE) else 0.0 end as FXRate2
					, case interimnumber when 3 then sum(FX_RATE) else 0.0 end as FXRate3
					, case interimnumber when 4 then sum(FX_RATE) else 0.0 end as FXRate4
					, case interimnumber when 1 then sum(AVG90DAYRATE) else 0.0 end as FXAvgRate1
					, case interimnumber when 2 then sum(AVG90DAYRATE) else 0.0 end as FXAvgRate2
					, case interimnumber when 3 then sum(AVG90DAYRATE) else 0.0 end as FXAvgRate3
					, case interimnumber when 4 then sum(AVG90DAYRATE) else 0.0 end as FXAvgRate4
					, case interimnumber when 1 then CurrencyReported else null end as CurrencyReported1
					, case interimnumber when 2 then CurrencyReported else null end as CurrencyReported2
					, case interimnumber when 3 then CurrencyReported else null end as CurrencyReported3
					, case interimnumber when 4 then CurrencyReported else null end as CurrencyReported4
						
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
					, StatementType
					, PERIOD_YEAR
					, interimnumber
					, CurrencyReported
				) a
		 group by ISSUER_ID
			, COA
			, FX_CONV_TYPE
			, DATA_ID
			, COUNTRY_CODE
			, CURRENCY_CODE
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
	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 1: Scenario 1 (USD)'

		--Scenario 1: BAL item or Period Length = 3
		--in USD
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate1, 0.0) <> 0.0 then o.AMOUNT1 / o.FXAvgRate1
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate1, 0.0) <> 0.0 then o.AMOUNT1 / o.FXRate1
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported1 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where (StatementType = 'BAL' or PeriodLength1 = 3) and o.PeriodEndDate1 is not null	
		   
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 1: Scenario 1 (USD)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 1: Scenario 1 (Local)'

		   --in Local Currency
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate1, 0.0) <> 0.0 then o.AMOUNT1 / o.FXAvgRate1*fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate1, 0.0) <> 0.0 then o.AMOUNT1 / o.FXRate1 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported1 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate1 and fx.CURRENCY = o.CURRENCY_CODE
		where (StatementType = 'BAL' or PeriodLength1 = 3) and o.PeriodEndDate1 is not null		
		   
		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 1: Scenario 1 (Local)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	
    	-------------------
		-- Interim number 2
		-------------------

	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2: Scenario 1 (USD)'
	--Scenario 1: BAL item or Period Length = 3
		--in USD
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate2, 0.0) <> 0.0 then o.AMOUNT2 / o.FXAvgRate2
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate2, 0.0) <> 0.0 then o.AMOUNT2 / o.FXRate2
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported2 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where (StatementType = 'BAL' or PeriodLength2 = 3) and o.PeriodEndDate2 is not null		
		   	
		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2: Scenario 1 (USD)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		   
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2: Scenario 1 (local)'
		--in Local Currency
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate2, 0.0) <> 0.0 then o.AMOUNT2 / o.FXAvgRate2 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate2, 0.0) <> 0.0 then o.AMOUNT2 / o.FXRate2 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported2 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = o.CURRENCY_CODE
			where (StatementType = 'BAL' or PeriodLength2 = 3) and o.PeriodEndDate2 is not null	
		   
		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2: Scenario 1 (local)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END
		
		
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2: Scenario 2 (USD & Q2)'
	--Scenario 2: Non-BAL item and Period Length = 6 with no prior period (semi-annual)
		--in USD for Q2
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate2, 0.0) <> 0.0 then (o.AMOUNT2 / o.FXAvgRate2)/2
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate2, 0.0) <> 0.0 then (o.AMOUNT2 / o.FXRate2)/2
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported2 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where StatementType <> 'BAL' and PeriodLength2 = 6 and (PeriodLength1 is null or PeriodLength1 = 0) and o.PeriodEndDate2 is not null

		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2: Scenario 2 (USD & q2)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2: Scenario 2 (Local & Q2)'
		--in local currency for Q2
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate2, 0.0) <> 0.0 then (o.AMOUNT2 / o.FXAvgRate2)/2 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate2, 0.0) <> 0.0 then (o.AMOUNT2 / o.FXRate2)/2 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported2 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate2 and fx.CURRENCY = o.CURRENCY_CODE
			where StatementType <> 'BAL' and PeriodLength2 = 6 and (PeriodLength1 is null or PeriodLength1 = 0) and o.PeriodEndDate2 is not null
	 	
	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2: Scenario 2 (Local & q2)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		   
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2: Scenario 2 (USD & Q1)'
		--in USD for Q1 (create this Q1 quarter value)
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate2 as Root_Source_Date
			, 'Q1' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, DATEADD(s,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate2)-2,0))as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then (o.AMOUNT2 / fx.AVG90DAYRATE)/2
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then (o.AMOUNT2 / fx.FX_RATE)/2
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported2 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate2)-2,0)) and fx.CURRENCY = o.CurrencyReported2
			 where StatementType <> 'BAL' and PeriodLength2 = 6 and (PeriodLength1 is null or PeriodLength1 = 0) and o.PeriodEndDate2 is not null

	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2: Scenario 2 (USD & q1)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2: Scenario 2 (Local & Q1)'
		--in Local Currency for Q1 (create this Q1 quarter value)
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate2 as Root_Source_Date
			, 'Q1' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, DATEADD(s,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate2)-2,0))as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, o.CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull( fx1.AVG90DAYRATE, 0.0) <> 0.0 then (o.AMOUNT2 / fx1.AVG90DAYRATE)/2*fx2.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx1.FX_RATE, 0.0) <> 0.0 then (o.AMOUNT2 / fx1.FX_RATE)/2*fx2.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported2 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx1 on fx1.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate2)-2,0)) and fx1.CURRENCY = o.CurrencyReported2
			inner join dbo.FX_RATES fx2 on fx2.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate2)-2,0)) and fx2.CURRENCY = o.CURRENCY_CODE
		where StatementType <> 'BAL' and PeriodLength2 = 6 and (PeriodLength1 is null or PeriodLength1 = 0) and o.PeriodEndDate2 is not null

	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2: Scenario 2 (Local & q1)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2: Scenario 3 (USD)'
	--Scenario 3: Non-BAL item, prior period reported and this period nets out to length of 3
		--in USD for Q2
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2 - o.AMOUNT1
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate2, 0.0) <> 0.0  and isnull(o.FXAvgRate1, 0.0) <> 0.0 then (o.AMOUNT2 / o.FXAvgRate2) - (o.AMOUNT1 / o.FXAvgRate1)
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate2, 0.0) <> 0.0 and isnull(o.FXRate1, 0.0) <> 0.0 then (o.AMOUNT2 / o.FXRate2) - (o.AMOUNT1 / o.FXRate1)
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported2 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where StatementType <> 'BAL' and (PeriodLength2 - PeriodLength1) = 3 and (PeriodLength1 is not null or PeriodLength1 <> 0) and o.PeriodEndDate2 is not null


		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2: Scenario 3 (USD)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 2: Scenario 3 (Local)'
	--Scenario 3: Non-BAL item, prior period reported and this period nets out to length of 3
		--in Local for Q2
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, o.CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT2 - o.AMOUNT1
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate2, 0.0) <> 0.0  and isnull(o.FXAvgRate1, 0.0) <> 0.0 then ((o.AMOUNT2 / o.FXAvgRate2) *fx2.AVG90DAYRATE) - ((o.AMOUNT1 / o.FXAvgRate1)*fx1.AVG90DAYRATE)
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate2, 0.0) <> 0.0 and isnull(o.FXRate1, 0.0) <> 0.0 then ((o.AMOUNT2 / o.FXRate2)*fx2.FX_RATE) - ((o.AMOUNT1 / o.FXRate1)*fx1.FX_RATE)
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported2 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		  inner join dbo.FX_RATES fx1 on fx1.FX_DATE = o.PeriodEndDate1 and fx1.CURRENCY =  o.CURRENCY_CODE 
		  inner join dbo.FX_RATES fx2 on fx2.FX_DATE = o.PeriodEndDate2 and fx2.CURRENCY =  o.CURRENCY_CODE 
		where StatementType <> 'BAL' and (PeriodLength2 - PeriodLength1) = 3 and (PeriodLength1 is not null or PeriodLength1 <> 0) and o.PeriodEndDate2 is not null


		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 2: Scenario 3 (Local)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



    	-------------------
		-- Interim number 3
		-------------------

	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3: Scenario 1 (USD)'
	--Scenario 1: BAL item or Period Length = 3
		--in USD
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate3, 0.0) <> 0.0 then o.AMOUNT3 / o.FXAvgRate3
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate3, 0.0) <> 0.0 then o.AMOUNT3 / o.FXRate3
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported3 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where (StatementType = 'BAL' or PeriodLength3 = 3) and o.PeriodEndDate3 is not null		
		   	
		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3: Scenario 1 (USD)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		   
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3: Scenario 1 (local)'
		--in Local Currency
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate3, 0.0) <> 0.0 then o.AMOUNT3 / o.FXAvgRate3 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate3, 0.0) <> 0.0 then o.AMOUNT3 / o.FXRate3 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported3 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate3 and fx.CURRENCY = o.CURRENCY_CODE
			where (StatementType = 'BAL' or PeriodLength3 = 3) and o.PeriodEndDate3 is not null	
		   
		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3: Scenario 1 (local)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END
		
		
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3: Scenario 2 (USD & Q3)'
	--Scenario 2: Non-BAL item and Period Length = 6 with no prior period (semi-annual)
		--in USD for Q3
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT3/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate3, 0.0) <> 0.0 then (o.AMOUNT3 / o.FXAvgRate3)/2
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate3, 0.0) <> 0.0 then (o.AMOUNT3 / o.FXRate3)/2
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported3 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where StatementType <> 'BAL' and PeriodLength3 = 6 and (PeriodLength2 is null or PeriodLength2 = 0) and o.PeriodEndDate3 is not null

		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3: Scenario 2 (USD & Q3)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3: Scenario 2 (Local & Q3)'
		--in local currency for Q3
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT3/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate3, 0.0) <> 0.0 then (o.AMOUNT3 / o.FXAvgRate3)/2 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate3, 0.0) <> 0.0 then (o.AMOUNT3 / o.FXRate3)/2 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported3 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate3 and fx.CURRENCY = o.CURRENCY_CODE
			where StatementType <> 'BAL' and PeriodLength3 = 6 and (PeriodLength2 is null or PeriodLength2 = 0) and o.PeriodEndDate3 is not null
	 	
	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3: Scenario 2 (Local & q3)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		   
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3: Scenario 2 (USD & Q2)'
		--in USD for Q2 (create this Q2 quarter value)
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate3 as Root_Source_Date
			, 'Q2' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, DATEADD(s,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate3)-2,0))as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT3/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then (o.AMOUNT3 / fx.AVG90DAYRATE)/2
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then (o.AMOUNT3 / fx.FX_RATE)/2
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported3 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate3)-2,0)) and fx.CURRENCY = o.CurrencyReported3
			 where StatementType <> 'BAL' and PeriodLength3 = 6 and (PeriodLength2 is null or PeriodLength2 = 0) and o.PeriodEndDate3 is not null

	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3: Scenario 2 (USD & q2)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3: Scenario 2 (Local & Q2)'
		--in Local Currency for Q2 (create this Q2 quarter value)
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate3 as Root_Source_Date
			, 'Q2' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, DATEADD(s,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate3)-2,0))as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, o.CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT3/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull( fx1.AVG90DAYRATE, 0.0) <> 0.0 then (o.AMOUNT3 / fx1.AVG90DAYRATE)/2*fx2.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx1.FX_RATE, 0.0) <> 0.0 then (o.AMOUNT3 / fx1.FX_RATE)/2*fx2.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported3 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx1 on fx1.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate3)-2,0)) and fx1.CURRENCY = o.CurrencyReported3
			inner join dbo.FX_RATES fx2 on fx2.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate3)-2,0)) and fx2.CURRENCY = o.CURRENCY_CODE
		where StatementType <> 'BAL' and PeriodLength3 = 6 and (PeriodLength2 is null or PeriodLength2 = 0) and o.PeriodEndDate3 is not null

	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3: Scenario 2 (Local & q2)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3: Scenario 3 (USD)'
	--Scenario 3: Non-BAL item, prior period reported and this period nets out to length of 3
		--in USD for Q3
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT3 - o.AMOUNT2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate3, 0.0) <> 0.0  and isnull(o.FXAvgRate2, 0.0) <> 0.0 then (o.AMOUNT3 / o.FXAvgRate3) - (o.AMOUNT2 / o.FXAvgRate2)
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate3, 0.0) <> 0.0 and isnull(o.FXRate2, 0.0) <> 0.0 then (o.Amount3 / o.FXRate3) - (o.AMOUNT2 / o.FXRate2)
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported3 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where StatementType <> 'BAL' and (PeriodLength3 - PeriodLength2) = 3 and (PeriodLength2 is not null or PeriodLength2 <> 0) and o.PeriodEndDate3 is not null


		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3: Scenario 3 (USD)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 3: Scenario 3 (Local)'
	--Scenario 3: Non-BAL item, prior period reported and this period nets out to length of 3
		--in Local for Q3
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, o.CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT3 - o.AMOUNT2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate3, 0.0) <> 0.0  and isnull(o.FXAvgRate2, 0.0) <> 0.0 then ((o.AMOUNT3 / o.FXAvgRate3) *fx3.AVG90DAYRATE) - ((o.AMOUNT2 / o.FXAvgRate2)*fx2.AVG90DAYRATE)
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate3, 0.0) <> 0.0 and isnull(o.FXRate2, 0.0) <> 0.0 then ((o.AMOUNT3 / o.FXRate3)*fx3.FX_RATE) - ((o.AMOUNT2 / o.FXRate2)*fx2.FX_RATE)
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported3 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		  inner join dbo.FX_RATES fx2 on fx2.FX_DATE = o.PeriodEndDate2 and fx2.CURRENCY =  o.CURRENCY_CODE 
		  inner join dbo.FX_RATES fx3 on fx3.FX_DATE = o.PeriodEndDate3 and fx3.CURRENCY =  o.CURRENCY_CODE 
		where StatementType <> 'BAL' and (PeriodLength3 - PeriodLength2) = 3 and (PeriodLength2 is not null or PeriodLength2 <> 0) and o.PeriodEndDate3 is not null


		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 3: Scenario 3 (Local)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


    	-------------------
		-- Interim number 4
		-------------------

	if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4: Scenario 1 (USD)'
	--Scenario 1: BAL item or Period Length = 3
		--in USD
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate4, 0.0) <> 0.0 then o.AMOUNT4 / o.FXAvgRate4
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate4, 0.0) <> 0.0 then o.AMOUNT4 / o.FXRate4
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported4 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where (StatementType = 'BAL' or PeriodLength4 = 3) and o.PeriodEndDate4 is not null		
		   	
		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4: Scenario 1 (USD)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		   
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4: Scenario 1 (local)'
		--in Local Currency
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate4, 0.0) <> 0.0 then o.AMOUNT4 / o.FXAvgRate4 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate4, 0.0) <> 0.0 then o.AMOUNT4 / o.FXRate4 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported4 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate4 and fx.CURRENCY = o.CURRENCY_CODE
			where (StatementType = 'BAL' or PeriodLength4 = 3) and o.PeriodEndDate4 is not null	
		   
		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4: Scenario 1 (local)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END
		
		
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4: Scenario 2 (USD & Q4)'
	--Scenario 2: Non-BAL item and Period Length = 6 with no prior period (semi-annual)
		--in USD for Q4
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT4/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate4, 0.0) <> 0.0 then (o.AMOUNT4 / o.FXAvgRate4)/2
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate4, 0.0) <> 0.0 then (o.AMOUNT4 / o.FXRate4)/2
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported4 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where StatementType <> 'BAL' and PeriodLength4 = 6 and (PeriodLength3 is null or PeriodLength3 = 0) and o.PeriodEndDate4 is not null

		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4: Scenario 2 (USD & q4)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4: Scenario 2 (Local & Q4)'
		--in local currency for Q4
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT4/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate4, 0.0) <> 0.0 then (o.AMOUNT4 / o.FXAvgRate4)/2 * fx.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate4, 0.0) <> 0.0 then (o.AMOUNT4 / o.FXRate4)/2 * fx.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported4 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = o.PeriodEndDate4 and fx.CURRENCY = o.CURRENCY_CODE
			where StatementType <> 'BAL' and PeriodLength4 = 6 and (PeriodLength3 is null or PeriodLength3 = 0) and o.PeriodEndDate4 is not null
	 	
	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4: Scenario 2 (Local & q4)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		   
		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4: Scenario 2 (USD & Q3)'
		--in USD for Q3 (create this Q3 quarter value)
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate4 as Root_Source_Date
			, 'Q3' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, DATEADD(s,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate4)-2,0))as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, 'USD' as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT4/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(fx.AVG90DAYRATE, 0.0) <> 0.0 then (o.AMOUNT4 / fx.AVG90DAYRATE)/2
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx.FX_RATE, 0.0) <> 0.0 then (o.AMOUNT4 / fx.FX_RATE)/2
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported4 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx on fx.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate4)-2,0)) and fx.CURRENCY = o.CurrencyReported4
			 where StatementType <> 'BAL' and PeriodLength4 = 6 and (PeriodLength3 is null or PeriodLength3 = 0) and o.PeriodEndDate4 is not null

	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4: Scenario 2 (USD & q3)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4: Scenario 2 (Local & Q3)'
		--in Local Currency for Q3 (create this Q3 quarter value)
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
			, ' ' as COA_TYPE
			, 'REUTERS' as DATA_SOURCE
			, 'REUTERS' as ROOT_SOURCE
			, o.UpdateDate4 as Root_Source_Date
			, 'Q3' as PERIOD_TYPE
			, o.PERIOD_YEAR
			, DATEADD(s,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate4)-2,0))as PeriodEndDate
		--	, PeriodLength
		--	, PeriodLengthCode
			, 'FISCAL' as Fiscal_Type
			, o.CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT4/2
					when  o.FX_CONV_TYPE  = 'AVG' and isnull( fx1.AVG90DAYRATE, 0.0) <> 0.0 then (o.AMOUNT4 / fx1.AVG90DAYRATE)/2*fx2.AVG90DAYRATE
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(fx1.FX_RATE, 0.0) <> 0.0 then (o.AMOUNT4 / fx1.FX_RATE)/2*fx2.FX_RATE
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported4 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
			inner join dbo.FX_RATES fx1 on fx1.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate4)-2,0)) and fx1.CURRENCY = o.CurrencyReported4
			inner join dbo.FX_RATES fx2 on fx2.FX_DATE = DATEADD(d,-1,DATEADD(mm,DATEDIFF(m,0,o.PeriodEndDate4)-2,0)) and fx2.CURRENCY = o.CURRENCY_CODE
		where StatementType <> 'BAL' and PeriodLength4 = 6 and (PeriodLength3 is null or PeriodLength3 = 0) and o.PeriodEndDate4 is not null

	 	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4: Scenario 2 (Local & q3)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END



		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4: Scenario 3 (USD)'
	--Scenario 3: Non-BAL item, prior period reported and this period nets out to length of 3
		--in USD for Q4
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT4 - o.AMOUNT3
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate4, 0.0) <> 0.0  and isnull(o.FXAvgRate3, 0.0) <> 0.0 then (o.AMOUNT4 / o.FXAvgRate4) - (o.AMOUNT3 / o.FXAvgRate3)
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate4, 0.0) <> 0.0 and isnull(o.FXRate3, 0.0) <> 0.0 then (o.AMOUNT4 / o.FXRate4) - (o.AMOUNT3 / o.FXRate3)
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported4 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		 where StatementType <> 'BAL' and (PeriodLength4 - PeriodLength3) = 3 and (PeriodLength3 is not null or PeriodLength3 <> 0) and o.PeriodEndDate4 is not null


		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4: Scenario 3 (USD)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END


		if @VERBOSE = 'Y' print '>>> ' + CONVERT(varchar(40), getdate(), 121) + ' - Interim number 4: Scenario 3 (Local)'
	--Scenario 3: Non-BAL item, prior period reported and this period nets out to length of 3
		--in Local for Q4
		insert into PERIOD_FINANCIALS
		select o.ISSUER_ID 
			, ' ' as SECURITY_ID
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
			, o.CURRENCY_CODE as CURRENCY
			, o.Data_ID
			, CASE when o.FX_CONV_TYPE  = 'NONE' then o.AMOUNT4 - o.AMOUNT3
					when  o.FX_CONV_TYPE  = 'AVG' and isnull(o.FXAvgRate4, 0.0) <> 0.0  and isnull(o.FXAvgRate3, 0.0) <> 0.0 then ((o.AMOUNT4 / o.FXAvgRate4) *fx4.AVG90DAYRATE) - ((o.AMOUNT3 / o.FXAvgRate3)*fx3.AVG90DAYRATE)
					when  o.FX_CONV_TYPE  = 'PIT' and isnull(o.FXRate4, 0.0) <> 0.0 and isnull(o.FXRate3, 0.0) <> 0.0 then ((o.AMOUNT4 / o.FXRate4)*fx4.FX_RATE) - ((o.AMOUNT3 / o.FXRate3)*fx3.FX_RATE)
					else 0.0 end AS AMOUNT
			, ' ' as CALCULATION_DIAGRAM
			, o.CurrencyReported4 as SOURCE_CURRENCY
			, 'ACTUAL' as AMOUNT_TYPE
		  from #OUT o
		  inner join dbo.FX_RATES fx3 on fx3.FX_DATE = o.PeriodEndDate3 and fx3.CURRENCY =  o.CURRENCY_CODE 
		  inner join dbo.FX_RATES fx4 on fx4.FX_DATE = o.PeriodEndDate4 and fx4.CURRENCY =  o.CURRENCY_CODE 
		where StatementType <> 'BAL' and (PeriodLength4 - PeriodLength3) = 3 and (PeriodLength3 is not null or PeriodLength3 <> 0) and o.PeriodEndDate4 is not null


		if @VERBOSE = 'Y' 
		BEGIN
			print 'After Interim number 4: Scenario 3 (Local)' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
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
	--also inserting estimate 11 into data id 47
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
  
