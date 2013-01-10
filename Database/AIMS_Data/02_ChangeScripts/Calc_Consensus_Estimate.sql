-------------------------------------------------------------------------------
-- This stored procedure selects raw data, calculates and stores display data
-- for the concensus estimates
-------------------------------------------------------------------------------
IF OBJECT_ID ( 'Calc_Consensus_Estimates', 'P' ) IS NOT NULL 
DROP PROCEDURE Calc_Consensus_Estimates;
GO

create procedure Calc_Consensus_Estimates
	@ISSUER_ID	varchar(20)		-- This is a required parameter, NULL is not allowed.
,	@VERBOSE	char		= 'Y'	-- Default to print things
as

	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()


	begin transaction;					-- Lock the table
	
	-- remove the rows that will be reinserted.
	delete from CURRENT_CONSENSUS_ESTIMATES
	 where @ISSUER_ID = ISSUER_ID

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Delete CURRENT_CONSENSUS_ESTIMATES for ISSUER_ID' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END

	delete from CURRENT_CONSENSUS_ESTIMATES
	 where SECURITY_ID in (select SECURITY_ID from GF_SECURITY_BASEVIEW where ISSUER_ID = @ISSUER_ID)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Delete CURRENT_CONSENSUS_ESTIMATES for SECURITY_ID' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END
			
	commit;								-- Unlock the table

	-- Make Country and Currency a variable instead of a join for this single issuer.
	declare @ISO_COUNTRY_CODE	varchar(3)
	declare @CURRENCY_CODE		varchar(3)
	select @ISO_COUNTRY_CODE = max(ISO_COUNTRY_CODE)
	  from dbo.GF_SECURITY_BASEVIEW 
	 where ISSUER_ID = @ISSUER_ID
	select @CURRENCY_CODE = CURRENCY_CODE
	  from dbo.Country_Master 
	 where COUNTRY_CODE = @ISO_COUNTRY_CODE

	-- Make XREF and ReportNumber constants instead of a table for a single Issuer.
	declare @XREF varchar(20)
	declare @ReportNumber varchar(20)
	select @XREF = XRef, @ReportNumber = ReportNumber
	  from dbo.GF_SECURITY_BASEVIEW
	 where ISSUER_ID = @ISSUER_ID;

	-- Make COAType a constant instead of using a join for a single Issuer.
	declare @COAType varchar(4)
	select @COAType = COAType
	  from Reuters.dbo.tblStdCompanyInfo 
	 where ReportNumber = @ReportNumber



-------------------------------------------------------------------------------------
---------------------------------  E S T I M A T E  ---------------------------------
-------------------------------------------------------------------------------------
-- This section calculates the ESTIMATE data.  The process is nearly identical to the
-- process below for ACTUAL data, except that the initial data comes from a 
-- different Reuters table.
--
-- The two sections were not combined for simplicity of coding.
-------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------

	-- collect a list of the most recent entries, because we cannot count on expirationDate being null
	-- Making this a temp table instead of a subselect in the next query saves huge time.
	select Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType, MAX(StartDate) as StartDate
	  into #CE
	  from Reuters.dbo.tblConsensusEstimate 
	 where XRef = @XREF
	   and ExpirationDate is null
	 group by Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType 

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After collect most recent entries' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
		
	-- Gather the Annual and quarterly data
	select	@ISSUER_ID as ISSUER_ID
		,	@CURRENCY_CODE as Local_Currency 
		,	ce.XREF
		,	Left(ce.fYearEnd,4) as PeriodYear
		,	ce.EstimateType
		,	Case when ce.Unit = 'T' then ce.Median  / 1000
				 when ce.Unit = 'B' then ce.Median  * 1000
				 when ce.Unit = 'MC' then ce.Median *  100
				 when ce.Unit = 'P' then ce.Median  /  100
				 else ce.Median end as Median
		,	ce.Currency
		,	ce.StartDate as SourceDate
		,	ce.PeriodEndDate
		,	ce.PeriodType
		,	ce.NumofEsts as NumberOfEstimates
		,	ce.High 
		,	ce.Low
		,	ce.StdDev as StandardDeviation
		,	cm.FX_CONV_TYPE as FXConversionType
		,	cm.ESTIMATE_ID as EstimateID
		,	fx.FX_RATE as FXRate
		,	fx.Avg90DayRate
		,	fx.Avg12MonthRate
		,	ci.EARNINGS
		,   'ORIGINAL' as TXT
	  into #CONSENSUS_EXTRACT
	  from Reuters.dbo.tblConsensusEstimate ce
	 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
	 inner join #CE a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType and a.StartDate = ce.StartDate
				and  ce.ExpirationDate is null
	  left join dbo.FX_RATES fx on fx.CURRENCY = @CURRENCY_CODE/*ce.Currency*/ and fx.FX_DATE = ce.PeriodEndDate
	 where 1=1 --sb.ISSUER_ID = @ISSUER_ID
	   and ce.XRef = @XREF
	   and ce.expirationDate is null
	   and (   (@COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (@COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (@COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (@COAType = 'BNK' and cm.BANK = 'Y') )
	   and ce.PeriodType in ('A', 'Q1', 'Q2', 'Q3', 'Q4')
	   and @CURRENCY_CODE is not NULL
--	 order by ce.Xref, ce.PeriodEndDate, ce.fYearEnd, ce.EstimateType, ce.PeriodType, ce.StartDate

-- select 'consensus #CONSENSUS_EXTRACT 1' as A, * from #CONSENSUS_EXTRACT order by periodyear, EstimateID
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Gather the Annual and quarterly data' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END


	-- Make the values for CAPEX and DEV negative, per the request by AshmoreEMM (Justin and Gerred)
	update #CONSENSUS_EXTRACT
	   set Median = Median * -1
	   ,   High = High * -1
	   ,   Low = Low * -1
	 where EstimateID in (2, 4)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After negate CAPEX and DIV for Annual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END


	------------------------------------------------------------------------------
	-- This section takes the semi-annual data and creates quarterly data instead
	------------------------------------------------------------------------------

	-- Collect the Semi-annual data	
	select	@ISSUER_ID as ISSUER_ID
		,	@CURRENCY_CODE as Local_Currency 
		,	ce.XREF
		,	Left(ce.fYearEnd,4) as PeriodYear
		,	ce.EstimateType
		,	Case when ce.Unit = 'T' then (ce.Median/2)  / 1000
				 when ce.Unit = 'B' then (ce.Median/2)  * 1000
				 when ce.Unit = 'MC' then (ce.Median/2) *  100
				 when ce.Unit = 'P' then (ce.Median/2)  /  100
				 else ce.Median/2 end as Median
		,	ce.Currency
		,	ce.StartDate as SourceDate
		,	ce.PeriodEndDate
		,	ce.PeriodType
		,	ce.NumofEsts as NumberOfEstimates
		,	ce.High 
		,	ce.Low
		,	ce.StdDev as StandardDeviation
		,	cm.FX_CONV_TYPE as FXConversionType
		,	cm.ESTIMATE_ID as EstimateID
			-- If the reported currency is USD then we need the FX rate for the Country currency
		,	case ce.Currency when 'USD' then fx2.FX_RATE else fx.FX_RATE end as FXRate
		,	case ce.Currency when 'USD' then fx2.Avg90DayRate else fx.Avg90DayRate end as Avg90DayRate
		,	case ce.Currency when 'USD' then fx2.Avg12MonthRate else fx.Avg12MonthRate end as Avg12MonthRate
		,	ce.fYearEnd
		,	ce.fPeriodEnd
		,	ci.EARNINGS
	  into #CONSENSUS_EXTRACT_SEMI
	  from Reuters.dbo.tblConsensusEstimate ce
	 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
--	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = ci.ReportNumber
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
--	 inner join #XREF rx on rx.ReportNumber = ci.ReportNumber 
/*	 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
				   from dbo.GF_SECURITY_BASEVIEW 
				  where ISSUER_ID = @ISSUER_ID
				  group by ISSUER_ID) sb on sb.ISSUER_ID = rx.ISSUER_ID
	 inner join dbo.Country_Master cty on cty.COUNTRY_CODE = sb.ISO_COUNTRY_CODE
*/	 -- this next inner join makes sure there are no duplicates coming from the Consensus Estimates table
	 INNER JOIN (select Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType, MAX(StartDate) as StartDate
				   from Reuters.dbo.tblConsensusEstimate 
				  where Xref = @XREF 
				  group by Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType ) a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType and a.StartDate = ce.StartDate
	  left join dbo.FX_RATES fx on fx.CURRENCY = ce.Currency and fx.FX_DATE = ce.PeriodEndDate
	  left join dbo.FX_RATES fx2 on fx2.CURRENCY = @CURRENCY_CODE and fx2.FX_DATE = ce.PeriodEndDate

	 where 1=1 --sb.ISSUER_ID = @ISSUER_ID
	   and ce.XRef = @XREF
	   and ce.expirationDate is null
	   and (   (@COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (@COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (@COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (@COAType = 'BNK' and cm.BANK = 'Y') )
	   and ce.PeriodType = 'S'
	   and @CURRENCY_CODE is not NULL
--	 order by ce.Xref, ce.PeriodEndDate, ce.fYearEnd, ce.EstimateType, ce.PeriodType, ce.StartDate


	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Collect the Semi-annual data	' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Make the values for CAPEX and DEV negative, per the request by AshmoreEMM (Justin and Gerred)
	update #CONSENSUS_EXTRACT_SEMI
	   set Median = Median * -1
	   ,   High = High * -1
	   ,   Low = Low * -1
	 where EstimateID in (2, 4)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After negate CAPEX and DIV for Semi-Annual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Modify The Earings type 
	update #CONSENSUS_EXTRACT_SEMI
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID = 8 then 8		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 9 then 8		-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 5 then 8		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (8, 9, 5)
	
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After update 8 9 5 SEMI' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
		
	update #CONSENSUS_EXTRACT
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID = 8 then 8		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 9 then 8		-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 5 then 8		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (8, 9, 5)


	if @VERBOSE = 'Y' 
		BEGIN
			print 'After update 8 9 5' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
		
	-- Modify the Net Income type 
	update #CONSENSUS_EXTRACT_SEMI
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID =11 then 11		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 12 then 11	-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 13 then 11		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (11, 12, 13)
	
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After update 11 12 13 SEMI' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
		
	update #CONSENSUS_EXTRACT
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID =11 then 11		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 12 then 11	-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 13 then 11		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (11, 12, 13)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After update 11 12 13' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
		
	-- Modify the Income Before Tax type 
	update #CONSENSUS_EXTRACT_SEMI
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID =14 then 14		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 16 then 14	-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 15 then 14		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (14, 16, 15)
	
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After update 14 16 15 SEMI' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
		
	update #CONSENSUS_EXTRACT
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID =14 then 14		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 16 then 14	-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 15 then 14		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (14, 16, 15)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Update 14 16 15' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- remove incorrect rows
	delete #CONSENSUS_EXTRACT
	 where EstimateID = 999999

	if @VERBOSE = 'Y' 
		BEGIN
			print 'remove incorrect rows EXTRACT' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
		
	delete #CONSENSUS_EXTRACT_SEMI
	 where EstimateID = 999999
	
	if @VERBOSE = 'Y' 
		BEGIN
			print 'remove incorrect rows SEMI' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END


	-- Create the first quarter results
	if @VERBOSE = 'Y' print 'Create the first quarter results'
	insert into #CONSENSUS_EXTRACT
	select	ces.ISSUER_ID 
		,	ces.Local_Currency 
		,	ces.XREF
		,	ces.PeriodYear
		,	ces.EstimateType
		,	ces.Median
		,	ces.Currency
		,	ces.SourceDate
		,	ces.PeriodEndDate
		,	'Q1' as PeriodType
		,	ces.NumberOfEstimates
		,	ces.High 
		,	ces.Low
		,	ces.StandardDeviation
		,	ces.FXConversionType
		,	ces.EstimateID
		,	ces.FXRate
		,	ces.Avg90DayRate
		,	ces.Avg12MonthRate
		,	ces.EARNINGS
		,   'Q1' as TXT
	  from #CONSENSUS_EXTRACT_SEMI ces
	  left join #CONSENSUS_EXTRACT ce on rtrim(ce.ISSUER_ID) = rtrim(ces.ISSUER_ID) and rtrim(isnull(ce.Currency,'Z')) = rtrim(isnull(ces.Currency,'Z'))
				and rtrim(ce.EstimateType) = rtrim(ces.EstimateType) and rtrim(ce.PeriodType) = 'Q1'
				and rtrim(ce.PeriodYear) = rtrim(ces.PeriodYear)
	 where	fYearEnd <> fPeriodEnd
	   and ce.ISSUER_ID is null

-- select 'consensus #CONSENSUS_EXTRACT 2' as A, * from #CONSENSUS_EXTRACT order by periodyear, EstimateID
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Create the first quarter results' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Create the second quarter results
	insert into #CONSENSUS_EXTRACT
	select	ces.ISSUER_ID 
		,	ces.Local_Currency 
		,	ces.XREF
		,	ces.PeriodYear
		,	ces.EstimateType
		,	ces.Median
		,	ces.Currency
		,	ces.SourceDate
		,	ces.PeriodEndDate
		,	'Q2' as PeriodType
		,	ces.NumberOfEstimates
		,	ces.High 
		,	ces.Low
		,	ces.StandardDeviation
		,	ces.FXConversionType
		,	ces.EstimateID
		,	ces.FXRate
		,	ces.Avg90DayRate
		,	ces.Avg12MonthRate
		,	ces.EARNINGS
		,   'Q2' as TXT
	  from #CONSENSUS_EXTRACT_SEMI ces
	  left join #CONSENSUS_EXTRACT ce on rtrim(ce.ISSUER_ID) = rtrim(ces.ISSUER_ID) and rtrim(isnull(ce.Currency,'Z')) = rtrim(isnull(ces.Currency,'Z'))
				and rtrim(ce.EstimateType) = rtrim(ces.EstimateType) and rtrim(ce.PeriodType) = 'Q2'
				and rtrim(ce.PeriodYear) = rtrim(ces.PeriodYear)
	 where	fYearEnd <> fPeriodEnd
	   and ce.ISSUER_ID is null

-- select 'consensus #CONSENSUS_EXTRACT 3' as A, * from #CONSENSUS_EXTRACT order by periodyear, EstimateID
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Create the second quarter results' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Create the third quarter results
	insert into #CONSENSUS_EXTRACT
	select	ces.ISSUER_ID 
		,	ces.Local_Currency 
		,	ces.XREF
		,	ces.PeriodYear
		,	ces.EstimateType
		,	ces.Median
		,	ces.Currency
		,	ces.SourceDate
		,	ces.PeriodEndDate
		,	'Q3' as PeriodType
		,	ces.NumberOfEstimates
		,	ces.High 
		,	ces.Low
		,	ces.StandardDeviation
		,	ces.FXConversionType
		,	ces.EstimateID
		,	ces.FXRate
		,	ces.Avg90DayRate
		,	ces.Avg12MonthRate
		,	ces.EARNINGS
		,   'Q3' as TXT
	  from #CONSENSUS_EXTRACT_SEMI ces
	  left join #CONSENSUS_EXTRACT ce on rtrim(ce.ISSUER_ID) = rtrim(ces.ISSUER_ID) and rtrim(isnull(ce.Currency,'Z')) = rtrim(isnull(ces.Currency,'Z'))
				and rtrim(ce.EstimateType) = rtrim(ces.EstimateType) and rtrim(ce.PeriodType) = 'Q3'
				and rtrim(ce.PeriodYear) = rtrim(ces.PeriodYear)
	 where	fYearEnd = fPeriodEnd
	   and ce.ISSUER_ID is null

-- select 'consensus #CONSENSUS_EXTRACT 4' as A, * from #CONSENSUS_EXTRACT order by periodyear, EstimateID
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Create the third quarter results' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Create the forth quarter retults
	insert into #CONSENSUS_EXTRACT
	select	ces.ISSUER_ID 
		,	ces.Local_Currency 
		,	ces.XREF
		,	ces.PeriodYear
		,	ces.EstimateType
		,	ces.Median
		,	ces.Currency
		,	ces.SourceDate
		,	ces.PeriodEndDate
		,	'Q4' as PeriodType
		,	ces.NumberOfEstimates
		,	ces.High 
		,	ces.Low
		,	ces.StandardDeviation
		,	ces.FXConversionType
		,	ces.EstimateID
		,	ces.FXRate
		,	ces.Avg90DayRate
		,	ces.Avg12MonthRate
		,	ces.EARNINGS
		,   'Q4' as TXT
	  from #CONSENSUS_EXTRACT_SEMI ces
	  left join #CONSENSUS_EXTRACT ce on rtrim(ce.ISSUER_ID) = rtrim(ces.ISSUER_ID) and rtrim(isnull(ce.Currency,'Z')) = rtrim(isnull(ces.Currency,'Z'))
				and rtrim(ce.EstimateType) = rtrim(ces.EstimateType) and rtrim(ce.PeriodType) = 'Q4'
				and rtrim(ce.PeriodYear) = rtrim(ces.PeriodYear)
	 where	fYearEnd = fPeriodEnd
	   and ce.ISSUER_ID is null

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Create the forth quarter retults' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

--  select 'consensus #CONSENSUS_EXTRACT 5' as A, * from #CONSENSUS_EXTRACT order by periodyear, EstimateID

	-----------------------------------------------
	-- Insert the data into the display table
	-----------------------------------------------
	if @VERBOSE = 'Y'	print 'insert USD'
	
	-- Insert the amounts for the currency USD
	insert into dbo.CURRENT_CONSENSUS_ESTIMATES
	Select	Issuer_ID 
		,	' ' as SECURITY_ID
		,	'REUTERS' as Source
		,	SourceDate
		,	PeriodType
		,	PeriodYear
		,	PeriodEndDate
		,	'FISCAL' as FiscalType
		,	EstimateID
		,	'USD' as Currency
		,	CASE FXConversionType when 'NONE' then Median
								  when 'AVG'  then Median / isnull(AVG12MonthRate, 1.0)
								  when 'PIT'  then Median / isnull(FXRate, 1.0)
								  else Median end AS AMOUNT
		,	NumberOfEstimates
		,	CASE FXConversionType when 'NONE' then High
								  when 'AVG'  then High / isnull(AVG12MonthRate, 1.0)
								  when 'PIT'  then High / isnull(FXRate, 1.0)
								  else Median end AS High
		,	CASE FXConversionType when 'NONE' then Low
								  when 'AVG'  then Low / isnull(AVG12MonthRate, 1.0)
								  when 'PIT'  then Low / isnull(FXRate, 1.0)
								  else Median end AS Low
		,	isnull(Currency, 'N/A') as SourceCurrency
		,	StandardDeviation
		,	'ESTIMATE' as ValueType
	  from #CONSENSUS_EXTRACT
	 where not (FXConversionType = 'AVG' and isnull(AVG12MonthRate, 0.0) = 0.0)
	   and not (FXConversionType = 'PIT' and isnull(FXRate, 0.0) = 0.0)
	   and Currency <> 'USD'
  
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Insert the amounts for the currency USD' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Insert the amounts for the country currency when the source is USD
	insert into dbo.CURRENT_CONSENSUS_ESTIMATES
	Select	ce.Issuer_ID 
		,	' ' as SECURITY_ID
		,	'REUTERS' as Source
		,	ce.SourceDate
		,	ce.PeriodType
		,	ce.PeriodYear
		,	ce.PeriodEndDate
		,	'FISCAL' as FiscalType
		,	ce.EstimateID
		,	@CURRENCY_CODE as Currency
		,	CASE FXConversionType when 'NONE' then ce.Median
								  when 'AVG'  then ce.Median * isnull(ce.AVG12MonthRate, 1.0)
								  when 'PIT'  then ce.Median * isnull(ce.FXRate, 1.0)
								  else ce.Median end AS AMOUNT
		,	NumberOfEstimates
		,	CASE FXConversionType when 'NONE' then ce.High
								  when 'AVG'  then ce.High * isnull(ce.AVG12MonthRate, 1.0)
								  when 'PIT'  then ce.High * isnull(ce.FXRate, 1.0)
								  else ce.High end AS High
		,	CASE FXConversionType when 'NONE' then ce.Low
								  when 'AVG'  then ce.Low * isnull(ce.AVG12MonthRate, 1.0)
								  when 'PIT'  then ce.Low * isnull(ce.FXRate, 1.0)
								  else ce.Low end AS Low
		,	isnull(ce.Currency, 'N/A') as SourceCurrency
		,	ce.StandardDeviation
		,	'ESTIMATE' as ValueType
	  from #CONSENSUS_EXTRACT ce
	 where not (ce.FXConversionType = 'AVG' and isnull(ce.AVG12MonthRate, 0.0) = 0.0)
	   and not (ce.FXConversionType = 'PIT' and isnull(ce.FXRate, 0.0) = 0.0)
	   and ce.Currency = 'USD'
  
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Insert the amounts for the currency USD' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END



	-- Insert the amounts for the local currency
	insert into dbo.CURRENT_CONSENSUS_ESTIMATES
	Select	Issuer_ID 
		,	' ' as SECURITY_ID
		,	'REUTERS' as Source
		,	SourceDate
		,	PeriodType
		,	PeriodYear
		,	PeriodEndDate
		,	'FISCAL' as FiscalType
		,	EstimateID
		,	isnull(Currency, @CURRENCY_CODE) as Currency		-- For ROE and ROA where there is no currency
		,	Median AS AMOUNT
		,	NumberOfEstimates
		,	High
		,	Low
		,	isnull(Currency, 'N/A') as SourceCurrency
		,	StandardDeviation
		,	'ESTIMATE' as ValueType
	  from #CONSENSUS_EXTRACT
--	 where Currency <> 'USD'			-- DLM

	-- Insert the amounts for the currency USD for values without currency like ROA and ROE
	insert into dbo.CURRENT_CONSENSUS_ESTIMATES
	Select	Issuer_ID 
		,	' ' as SECURITY_ID
		,	'REUTERS' as Source
		,	SourceDate
		,	PeriodType
		,	PeriodYear
		,	PeriodEndDate
		,	'FISCAL' as FiscalType
		,	EstimateID
		,	isnull(Currency, 'USD') as Currency		-- For ROE and ROA where there is no currency
		,	Median AS AMOUNT
		,	NumberOfEstimates
		,	High
		,	Low
		,	isnull(Currency, 'N/A') as SourceCurrency
		,	StandardDeviation
		,	'ESTIMATE' as ValueType
	  from #CONSENSUS_EXTRACT
	 where Currency is NULL		-- ROA and ROE have null currency and need this extra Insert

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Insert the amounts for the local currency' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END



-------------------------------------------------------------------------------------
---------------------------------  A C T U A L  -------------------------------------
-------------------------------------------------------------------------------------
-- This section calculates the ACTUAL data.  THe process is nearly identical to the
-- process above for ESTIMATE data, except that the initial data comes from a 
-- different Reuters table.
--
-- The two sections were not combined for simplicity of coding.
-------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------




	-- Gather the Annual and quarterly data
	select	@ISSUER_ID as ISSUER_ID 
		,	@CURRENCY_CODE as Local_Currency 
		,	ce.XREF
		,	Left(ce.fYearEnd,4) as PeriodYear
		,	ce.EstimateType
		,	Case when ce.Unit = 'T' then ce.ActualValue / 1000
				 when ce.Unit = 'B' then ce.ActualValue * 1000
				 else ce.ActualValue end as ActualValue
		,	ce.Currency
		,	ce.UpdateDate as SourceDate
		,	ce.PeriodEndDate
		,	ce.PeriodType
		,	cm.FX_CONV_TYPE as FXConversionType
		,	cm.ESTIMATE_ID as EstimateID
		,	fx.FX_RATE as FXRate
		,	fx.Avg90DayRate
		,	fx.Avg12MonthRate
		,	'ORIG' as TXT1
	  into #ACTUAL_EXTRACT
	  from Reuters.dbo.tblActual ce
--	 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
--	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = ci.ReportNumber
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
--	 inner join #XREF rx on rx.ReportNumber = ci.ReportNumber 
/*	 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
				   from dbo.GF_SECURITY_BASEVIEW 
				  where ISSUER_ID = @ISSUER_ID
				  group by ISSUER_ID) sb on sb.ISSUER_ID = rx.ISSUER_ID
	 inner join dbo.Country_Master cty on cty.COUNTRY_CODE = sb.ISO_COUNTRY_CODE
*/	 -- this next inner join makes sure there are no duplicates coming from the Consensus Estimates table
	 INNER JOIN (select Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType, MAX(UpdateDate) as UpdateDate
				   from Reuters.dbo.tblActual
				  where XRef = @XREF
				  group by Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType ) a
					on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType and a.UpdateDate = ce.UpdateDate
	  left join dbo.FX_RATES fx on fx.CURRENCY = @CURRENCY_CODE/*ce.Currency*/ and fx.FX_DATE = ce.PeriodEndDate
	 where 1=1 --@ISSUER_ID = ISSUER_ID
	   and ce.XRef = @XREF
	   and (   (@COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (@COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (@COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (@COAType = 'BNK' and cm.BANK = 'Y') )
	   and ce.PeriodType in ('A', 'Q1', 'Q2', 'Q3', 'Q4')
--	 order by ce.Xref, ce.PeriodEndDate, ce.fYearEnd, ce.EstimateType, ce.PeriodType, ce.UpdateDate


	if @VERBOSE = 'Y' 
		BEGIN
			print 'After gather annual and quarterly' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Make the values for CAPEX and DEV negative, per the request by AshmoreEMM (Justin and Gerred)
	update #ACTUAL_EXTRACT
	   set ActualValue = ActualValue * -1
	 where EstimateID in (2, 4)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After negate CAPEX and DIV for Actual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	------------------------------------------------------------------------------
	-- This section takes the semi-annual data and creates quarterly data instead
	------------------------------------------------------------------------------
	select Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType, MAX(UpdateDate) as UpdateDate
	  into #REUTERS_LATEST
	  from Reuters.dbo.tblActual
	 where XRef = @XREF			-- Added 20121106
	 group by Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType
				  
	-- Collect the Semi-annual data	
	select	@ISSUER_ID  as ISSUER_ID 
		,	@CURRENCY_CODE as Local_Currency 
		,	ce.XREF
		,	Left(ce.fYearEnd,4) as PeriodYear
		,	ce.EstimateType
		,	Case when ce.Unit = 'T' then (ce.ActualValue/2) / 1000
				 when ce.Unit = 'B' then (ce.ActualValue/2) *  1000
				 else ce.ActualValue/2 end as ActualValue
		,	ce.Currency
		,	ce.UpdateDate as SourceDate
		,	ce.PeriodEndDate
		,	ce.PeriodType
		,	cm.FX_CONV_TYPE as FXConversionType
		,	cm.ESTIMATE_ID as EstimateID
		,	fx.FX_RATE as FXRate
		,	fx.Avg90DayRate
		,	fx.Avg12MonthRate
		,	ce.fYearEnd
		,	ce.fPeriodEnd
	  into #ACTUAL_EXTRACT_SEMI
	  from Reuters.dbo.tblActual ce
	 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = ci.ReportNumber
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
--	 inner join #XREF rx on rx.ReportNumber = ci.ReportNumber 
/*	 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
				   from dbo.GF_SECURITY_BASEVIEW 
				  where ISSUER_ID = @ISSUER_ID	-- Added 20121106
				  group by ISSUER_ID) sb on sb.ISSUER_ID = rx.ISSUER_ID
	 inner join dbo.Country_Master cty on cty.COUNTRY_CODE = sb.ISO_COUNTRY_CODE
*/	 -- this next inner join makes sure there are no duplicates coming from the Consensus Estimates table
	 INNER JOIN #REUTERS_LATEST a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType and a.UpdateDate = ce.UpdateDate
	  left join dbo.FX_RATES fx on fx.CURRENCY = ce.Currency and fx.FX_DATE = ce.PeriodEndDate
	 where 1=1 --@ISSUER_ID = rx.ISSUER_ID
	   and ce.XRef = @XREF
	   and (   (@COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (@COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (@COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (@COAType = 'BNK' and cm.BANK = 'Y') )
	   and ce.PeriodType = 'S'
--	 order by ce.Xref, ce.PeriodEndDate, ce.fYearEnd, ce.EstimateType, ce.PeriodType, ce.UpdateDate

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After collect semi- annual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Create the first quarter retults
	insert into #ACTUAL_EXTRACT
	select	@ISSUER_ID  as ISSUER_ID 
		,	ces.Local_Currency 
		,	ces.XREF
		,	ces.PeriodYear
		,	ces.EstimateType
		,	ces.ActualValue
		,	ces.Currency
		,	ces.SourceDate
		,	ces.PeriodEndDate
		,	'Q1' as PeriodType
		,	ces.FXConversionType
		,	ces.EstimateID
		,	ces.FXRate
		,	ces.Avg90DayRate
		,	ces.Avg12MonthRate
		,	'A-Q1' as TXT1
	  from #ACTUAL_EXTRACT_SEMI ces
	  left join #ACTUAL_EXTRACT ce on rtrim(ce.ISSUER_ID) = rtrim(ces.ISSUER_ID) and rtrim(isnull(ce.Currency,'Z')) = rtrim(isnull(ces.Currency,'Z'))
				and rtrim(ce.EstimateType) = rtrim(ces.EstimateType) and rtrim(ce.PeriodType) = 'Q1'
				and rtrim(ce.PeriodYear) = rtrim(ces.PeriodYear)
	 where	fYearEnd <> fPeriodEnd
	   and ce.ISSUER_ID is null

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After first quarter results ++' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Create the first quarter retults
	insert into #ACTUAL_EXTRACT
	select	ces.ISSUER_ID 
		,	ces.Local_Currency 
		,	ces.XREF
		,	ces.PeriodYear
		,	ces.EstimateType
		,	ces.ActualValue
		,	ces.Currency
		,	ces.SourceDate
		,	ces.PeriodEndDate
		,	'Q2' as PeriodType
		,	ces.FXConversionType
		,	ces.EstimateID
		,	ces.FXRate
		,	ces.Avg90DayRate
		,	ces.Avg12MonthRate
		,	'A-Q2' as TXT1
	  from #ACTUAL_EXTRACT_SEMI ces
	  left join #ACTUAL_EXTRACT ce on rtrim(ce.ISSUER_ID) = rtrim(ces.ISSUER_ID) and rtrim(isnull(ce.Currency,'Z')) = rtrim(isnull(ces.Currency,'Z'))
				and rtrim(ce.EstimateType) = rtrim(ces.EstimateType) and rtrim(ce.PeriodType) = 'Q2'
				and rtrim(ce.PeriodYear) = rtrim(ces.PeriodYear)
	 where	fYearEnd <> fPeriodEnd
	   and ce.ISSUER_ID is null

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After after first quarter results, +++' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Create the third quarter results
	insert into #ACTUAL_EXTRACT
	select	ces.ISSUER_ID 
		,	ces.Local_Currency 
		,	ces.XREF
		,	ces.PeriodYear
		,	ces.EstimateType
		,	ces.ActualValue
		,	ces.Currency
		,	ces.SourceDate
		,	ces.PeriodEndDate
		,	'Q3' as PeriodType
		,	ces.FXConversionType
		,	ces.EstimateID
		,	ces.FXRate
		,	ces.Avg90DayRate
		,	ces.Avg12MonthRate
		,	'A-Q3' as TXT1
	  from #ACTUAL_EXTRACT_SEMI ces
	  left join #ACTUAL_EXTRACT ce on rtrim(ce.ISSUER_ID) = rtrim(ces.ISSUER_ID) and rtrim(isnull(ce.Currency,'Z')) = rtrim(isnull(ces.Currency,'Z'))
				and rtrim(ce.EstimateType) = rtrim(ces.EstimateType) and rtrim(ce.PeriodType) = 'Q3'
				and rtrim(ce.PeriodYear) = rtrim(ces.PeriodYear)
	 where	fYearEnd = fPeriodEnd
	   and ce.ISSUER_ID is null

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After third quarter results' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Create the forth quarter retults
	insert into #ACTUAL_EXTRACT
	select	ces.ISSUER_ID 
		,	ces.Local_Currency 
		,	ces.XREF
		,	ces.PeriodYear
		,	ces.EstimateType
		,	ces.ActualValue
		,	ces.Currency
		,	ces.SourceDate
		,	ces.PeriodEndDate
		,	'Q4' as PeriodType
		,	ces.FXConversionType
		,	ces.EstimateID
		,	ces.FXRate
		,	ces.Avg90DayRate
		,	ces.Avg12MonthRate
		,	'A-Q4' as TXT1
	  from #ACTUAL_EXTRACT_SEMI ces
	  left join #ACTUAL_EXTRACT ce on rtrim(ce.ISSUER_ID) = rtrim(ces.ISSUER_ID) and rtrim(isnull(ce.Currency,'Z')) = rtrim(isnull(ces.Currency,'Z'))
				and rtrim(ce.EstimateType) = rtrim(ces.EstimateType) and rtrim(ce.PeriodType) = 'Q4'
				and rtrim(ce.PeriodYear) = rtrim(ces.PeriodYear)
	 where	fYearEnd = fPeriodEnd
	   and ce.ISSUER_ID is null

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After forth quarter results' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-----------------------------------------------
	-- Insert the data into the display table
	-----------------------------------------------
	if @VERBOSE = 'Y' 	print 'Inserting USD 2'
	-- Insert the amounts for the currency USD
	insert into dbo.CURRENT_CONSENSUS_ESTIMATES
	Select	Issuer_ID 
		,	' ' as SECURITY_ID
		,	'REUTERS' as Source
		,	SourceDate
		,	PeriodType
		,	PeriodYear
		,	PeriodEndDate
		,	'FISCAL' as FiscalType
		,	EstimateID
		,	'USD' as Currency
		,	CASE FXConversionType when 'NONE' then ActualValue
								  when 'AVG'  then ActualValue / isnull(AVG12MonthRate, 1.0)
								  when 'PIT'  then ActualValue / isnull(FXRate, 1.0)
								  else ActualValue end AS AMOUNT
		,	0 as NumberOfEstimates
		,	0.0 as High
		,	0.0 as Low
		,	isnull(Currency, 'N/A') as SourceCurrency
		,	0.0 as StandardDeviation
		,	'ACTUAL' as ValueType
	  from #ACTUAL_EXTRACT
	 where not (FXConversionType = 'AVG' and isnull(AVG12MonthRate, 0.0) = 0.0)
	   and not (FXConversionType = 'PIT' and isnull(FXRate, 0.0) = 0.0)
	   and Currency <> 'USD'

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After inserting USD 2' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	if @VERBOSE = 'Y' 	print 'Inserting Local FX'
	-- Insert the amounts for the country currency because the numbers were reported in USD
	insert into dbo.CURRENT_CONSENSUS_ESTIMATES
	Select	Issuer_ID 
		,	' ' as SECURITY_ID
		,	'REUTERS' as Source
		,	SourceDate
		,	PeriodType
		,	PeriodYear
		,	PeriodEndDate
		,	'FISCAL' as FiscalType
		,	EstimateID
		,	'USD' as Currency
		,	CASE FXConversionType when 'NONE' then ActualValue
								  when 'AVG'  then ActualValue * isnull(AVG12MonthRate, 1.0)
								  when 'PIT'  then ActualValue * isnull(FXRate, 1.0)
								  else ActualValue end AS AMOUNT
		,	0 as NumberOfEstimates
		,	0.0 as High
		,	0.0 as Low
		,	isnull(Currency, 'N/A') as SourceCurrency
		,	0.0 as StandardDeviation
		,	'ACTUAL' as ValueType
	  from #ACTUAL_EXTRACT
	 where (not (FXConversionType = 'AVG' and isnull(AVG12MonthRate, 0.0) = 0.0))
	   and (not (FXConversionType = 'PIT' and isnull(FXRate, 0.0) = 0.0))
	   and Currency = 'USD'

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After inserting Local FX' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
		
	if @VERBOSE = 'Y'	print 'Inserting Local'

	-- Insert the amounts for the local currency
	insert into dbo.CURRENT_CONSENSUS_ESTIMATES
	Select	Issuer_ID 
		,	' ' as SECURITY_ID
		,	'REUTERS' as Source
		,	SourceDate
		,	PeriodType
		,	PeriodYear
		,	PeriodEndDate
		,	'FISCAL' as FiscalType
		,	EstimateID
		,	isnull(Local_Currency, 'N/A') as Currency
		,	ActualValue AS AMOUNT
		,	0 as NumberOfEstimates
		,	0.0 as High
		,	0.0 as Low
		,	isnull(Currency, 'N/A') as SourceCurrency
		,	0.0 as StandardDeviation
		,	'ACTUAL' as ValueType
	  from #ACTUAL_EXTRACT


	if @VERBOSE = 'Y' 
		BEGIN
			print 'After insert local' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Clean up the temp table
--	drop table #XREF;
	drop table #CONSENSUS_EXTRACT;
	drop table #CONSENSUS_EXTRACT_SEMI;
	drop table #ACTUAL_EXTRACT;
	drop table #ACTUAL_EXTRACT_SEMI;
	drop table #REUTERS_LATEST
	
GO
-- exec Calc_Consensus_Estimates 157902