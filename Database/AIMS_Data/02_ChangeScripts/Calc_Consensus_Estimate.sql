-------------------------------------------------------------------------------
-- This stored procedure selects raw data, calculates and stores display data
-- for the concensus estimates
-------------------------------------------------------------------------------
IF OBJECT_ID ( 'Calc_Consensus_Estimates', 'P' ) IS NOT NULL 
DROP PROCEDURE Calc_Consensus_Estimates;
GO

create procedure Calc_Consensus_Estimates
	@ISSUER_ID	varchar(20)		-- This is a required parameter, NULL is not allowed.
as

	declare @XREF varchar(9)
	select @XREF = cast(cast(XREF as integer) as varchar(9))from GF_SECURITY_BASEVIEW where ISSUER_ID = @ISSUER_ID
	print 'XREF = ' + @XREF

	declare @START		datetime		-- the time the calc starts
	Set @START = getdate()

	-- remove the rows that will be reinserted.
	delete from CURRENT_CONSENSUS_ESTIMATES
	 where (@ISSUER_ID is not null and @ISSUER_ID = ISSUER_ID)
	    or (@ISSUER_ID is NULL and ISSUER_ID in (select ISSUER_ID from dbo.GF_SECURITY_BASEVIEW));

	print 'After first delete' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	delete from CURRENT_CONSENSUS_ESTIMATES
	 where (@ISSUER_ID is not null and SECURITY_ID in (select SECURITY_ID from GF_SECURITY_BASEVIEW where ISSUER_ID = @ISSUER_ID))
	    or (@ISSUER_ID is NULL and SECURITY_ID in (select SECURITY_ID from dbo.GF_SECURITY_BASEVIEW));

	print 'After second delete' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	-- Prepare the Cross reference table
	select distinct ISSUER_ID, ReportNumber 
	  into #XREF
	  from dbo.GF_SECURITY_BASEVIEW;
	create index XREF_idx1 on #XREF(ISSUER_ID);
	create index XREF_idx2 on #XREF(ReportNumber);

	print 'After Prepare the Cross reference table' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()


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


	-- Gather the Annual and quarterly data
	select	rx.ISSUER_ID 
		,	cty. CURRENCY_CODE as Local_Currency 
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
	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = ci.ReportNumber
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
	 inner join #XREF rx on rx.ReportNumber = ci.ReportNumber
	 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
				   from dbo.GF_SECURITY_BASEVIEW 
				  where ISSUER_ID = @ISSUER_ID
				  group by ISSUER_ID) sb on sb.ISSUER_ID = rx.ISSUER_ID
	 inner join dbo.Country_Master cty on cty.COUNTRY_CODE = sb.ISO_COUNTRY_CODE
	 -- this next inner join makes sure there are no duplicates coming from the Consensus Estimates table
	 INNER JOIN (select Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType, MAX(StartDate) as StartDate
				   from Reuters.dbo.tblConsensusEstimate 
				  where XRef = @XREF
				  group by Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType ) a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType and a.StartDate = ce.StartDate
	  left join dbo.FX_RATES fx on fx.CURRENCY = ce.Currency and fx.FX_DATE = ce.PeriodEndDate
	 where sb.ISSUER_ID = @ISSUER_ID
	   and ce.expirationDate is null
	   and (   (sci.COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (sci.COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (sci.COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (sci.COAType = 'BNK' and cm.BANK = 'Y') )
	   and ce.PeriodType in ('A', 'Q1', 'Q2', 'Q3', 'Q4')
	 order by ce.Xref, ce.PeriodEndDate, ce.fYearEnd, ce.EstimateType, ce.PeriodType, ce.StartDate

	print 'After Gather the Annual and quarterly data' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()



	------------------------------------------------------------------------------
	-- This section takes the semi-annual data and creates quarterly data instead
	------------------------------------------------------------------------------

	-- Collect the Semi-annual data	
	select	rx.ISSUER_ID 
		,	cty. CURRENCY_CODE as Local_Currency 
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
		,	fx.FX_RATE as FXRate
		,	fx.Avg90DayRate
		,	fx.Avg12MonthRate
		,	ce.fYearEnd
		,	ce.fPeriodEnd
		,	ci.EARNINGS
	  into #CONSENSUS_EXTRACT_SEMI
	  from Reuters.dbo.tblConsensusEstimate ce
	 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = ci.ReportNumber
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
	 inner join #XREF rx on rx.ReportNumber = ci.ReportNumber 
	 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
				   from dbo.GF_SECURITY_BASEVIEW 
				  where ISSUER_ID = @ISSUER_ID
				  group by ISSUER_ID) sb on sb.ISSUER_ID = rx.ISSUER_ID
	 inner join dbo.Country_Master cty on cty.COUNTRY_CODE = sb.ISO_COUNTRY_CODE
	 -- this next inner join makes sure there are no duplicates coming from the Consensus Estimates table
	 INNER JOIN (select Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType, MAX(StartDate) as StartDate
				   from Reuters.dbo.tblConsensusEstimate 
				  where Xref = @XREF 
				  group by Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType ) a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType and a.StartDate = ce.StartDate
	  left join dbo.FX_RATES fx on fx.CURRENCY = ce.Currency and fx.FX_DATE = ce.PeriodEndDate
	 where sb.ISSUER_ID = @ISSUER_ID
	   and ce.expirationDate is null
	   and (   (sci.COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (sci.COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (sci.COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (sci.COAType = 'BNK' and cm.BANK = 'Y') )
	   and ce.PeriodType = 'S'
	 order by ce.Xref, ce.PeriodEndDate, ce.fYearEnd, ce.EstimateType, ce.PeriodType, ce.StartDate


	print 'After Collect the Semi-annual data	' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()


	-- Modify The Earings type 
	update #CONSENSUS_EXTRACT_SEMI
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID = 8 then 8		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 9 then 8		-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 5 then 8		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (8, 9, 5)
	
	print 'After update 8 9 5 SEMI' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	update #CONSENSUS_EXTRACT
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID = 8 then 8		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 9 then 8		-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 5 then 8		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (8, 9, 5)


	print 'After update 8 9 5' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	-- Modify the Net Income type 
	update #CONSENSUS_EXTRACT_SEMI
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID =11 then 11		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 12 then 11	-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 13 then 11		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (11, 12, 13)
	
	print 'After update 11 12 13 SEMI' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	update #CONSENSUS_EXTRACT
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID =11 then 11		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 12 then 11	-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 13 then 11		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (11, 12, 13)

	print 'After update 11 12 13' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	-- Modify the Income Before Tax type 
	update #CONSENSUS_EXTRACT_SEMI
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID =14 then 14		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 16 then 14	-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 15 then 14		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (14, 16, 15)
	
	print 'After update 14 16 15 SEMI' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	update #CONSENSUS_EXTRACT
	   set EstimateID = case when EARNINGS = 'EPS' and EstimateID =14 then 14		-- correct combo
							 when EARNINGS = 'EPSREP' and EstimateID = 16 then 14	-- correct combo
							 when EARNINGS = 'EBG' and EstimateID = 15 then 14		-- correct combo
							 else 999999 end										-- incorrect combo
	 where EstimateID in (14, 16, 15)

	print 'After Update 14 16 15' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()


	-- remove incorrect rows
	delete #CONSENSUS_EXTRACT
	 where EstimateID = 999999
	print 'remove incorrect rows EXTRACT' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()
	delete #CONSENSUS_EXTRACT_SEMI
	 where EstimateID = 999999
	
	print 'remove incorrect rows SEMI' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()



	-- Create the first quarter results
	print 'Create the first quarter results'
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

	print 'After Create the first quarter results' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()


	-- Create the second quarter results
	print 'Create the second quarter results'
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

	print 'After Create the second quarter results' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()


	-- Create the third quarter results
	print 'Create the third quarter results'
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

	print 'After Create the third quarter results' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()


	-- Create the forth quarter retults
	print 'Create the forth quarter retults'
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

	print 'After Create the forth quarter retults' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()


	-----------------------------------------------
	-- Insert the data into the display table
	-----------------------------------------------
	print 'insert USD'
	
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
  
	print 'After Insert the amounts for the currency USD' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()



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
		,	Local_Currency as Currency
		,	Median AS AMOUNT
		,	NumberOfEstimates
		,	High
		,	Low
		,	isnull(Currency, 'N/A') as SourceCurrency
		,	StandardDeviation
		,	'ESTIMATE' as ValueType
	  from #CONSENSUS_EXTRACT


	print 'After Insert the amounts for the local currency' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()




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
	select	rx.ISSUER_ID 
		,	cty.CURRENCY_CODE as Local_Currency 
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
	 inner join Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
	 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = ci.ReportNumber
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
	 inner join #XREF rx on rx.ReportNumber = ci.ReportNumber 
	 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
				   from dbo.GF_SECURITY_BASEVIEW 
				  where ISSUER_ID = @ISSUER_ID
				  group by ISSUER_ID) sb on sb.ISSUER_ID = rx.ISSUER_ID
	 inner join dbo.Country_Master cty on cty.COUNTRY_CODE = sb.ISO_COUNTRY_CODE
	 -- this next inner join makes sure there are no duplicates coming from the Consensus Estimates table
	 INNER JOIN (select Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType, MAX(UpdateDate) as UpdateDate
				   from Reuters.dbo.tblActual
				  where XRef = @XREF
				  group by Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType ) a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType and a.UpdateDate = ce.UpdateDate
	  left join dbo.FX_RATES fx on fx.CURRENCY = ce.Currency and fx.FX_DATE = ce.PeriodEndDate
	 where (   (@ISSUER_ID is not null and @ISSUER_ID = rx.ISSUER_ID)
			or (@ISSUER_ID is NULL and rx.ISSUER_ID in (select ISSUER_ID from dbo.GF_SECURITY_BASEVIEW)))
	   and (   (sci.COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (sci.COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (sci.COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (sci.COAType = 'BNK' and cm.BANK = 'Y') )
	   and ce.PeriodType in ('A', 'Q1', 'Q2', 'Q3', 'Q4')
	 order by ce.Xref, ce.PeriodEndDate, ce.fYearEnd, ce.EstimateType, ce.PeriodType, ce.UpdateDate


	print 'After gather annual and quarterly' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()


	------------------------------------------------------------------------------
	-- This section takes the semi-annual data and creates quarterly data instead
	------------------------------------------------------------------------------

	-- Collect the Semi-annual data	
	select	rx.ISSUER_ID 
		,	cty.CURRENCY_CODE as Local_Currency 
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
	 inner join #XREF rx on rx.ReportNumber = ci.ReportNumber 
	 inner join (select ISSUER_ID, max(ISO_COUNTRY_CODE) as ISO_COUNTRY_CODE 
				   from dbo.GF_SECURITY_BASEVIEW 
				  group by ISSUER_ID) sb on sb.ISSUER_ID = rx.ISSUER_ID
	 inner join dbo.Country_Master cty on cty.COUNTRY_CODE = sb.ISO_COUNTRY_CODE
	 -- this next inner join makes sure there are no duplicates coming from the Consensus Estimates table
	 INNER JOIN (select Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType, MAX(UpdateDate) as UpdateDate
				   from Reuters.dbo.tblActual
				  group by Xref, PeriodEndDate, fPeriodEnd, EstimateType, PeriodType ) a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType and a.UpdateDate = ce.UpdateDate
	  left join dbo.FX_RATES fx on fx.CURRENCY = ce.Currency and fx.FX_DATE = ce.PeriodEndDate
	 where (   (@ISSUER_ID is not null and @ISSUER_ID = rx.ISSUER_ID)
			or (@ISSUER_ID is NULL and rx.ISSUER_ID in (select ISSUER_ID from dbo.GF_SECURITY_BASEVIEW)))
	   and (   (sci.COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (sci.COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (sci.COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (sci.COAType = 'BNK' and cm.BANK = 'Y') )
	   and ce.PeriodType = 'S'
	 order by ce.Xref, ce.PeriodEndDate, ce.fYearEnd, ce.EstimateType, ce.PeriodType, ce.UpdateDate

	print 'After collect semi- annual' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

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

	print 'After first quarter results ++' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

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

	print 'After after first quarter results, +++' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

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

	print 'After third quarter results' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

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

	print 'After forth quarter results' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	-----------------------------------------------
	-- Insert the data into the display table
	-----------------------------------------------
	print 'Inserting USD 2'
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

	print 'After inserting USD 2' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	print 'Inserting Local'

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
		,	Local_Currency as Currency
		,	ActualValue AS AMOUNT
		,	0 as NumberOfEstimates
		,	0.0 as High
		,	0.0 as Low
		,	isnull(Currency, 'N/A') as SourceCurrency
		,	0.0 as StandardDeviation
		,	'ACTUAL' as ValueType
	  from #ACTUAL_EXTRACT


	print 'After insert local' + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	Set @START = getdate()

	-- Clean up the temp table
	drop table #XREF;
	drop table #CONSENSUS_EXTRACT;
	drop table #CONSENSUS_EXTRACT_SEMI;
	drop table #ACTUAL_EXTRACT;
	drop table #ACTUAL_EXTRACT_SEMI;
	
GO
-- exec Calc_Consensus_Estimates 157902