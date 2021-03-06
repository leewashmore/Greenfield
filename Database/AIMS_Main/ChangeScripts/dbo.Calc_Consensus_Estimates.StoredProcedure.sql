/****** Object:  StoredProcedure [dbo].[Calc_Consensus_Estimates]    Script Date: 03/28/2013 11:30:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
ALTER procedure [dbo].[Calc_Consensus_Estimates]
	@ISSUER_ID	varchar(20)		-- This is a required parameter, NULL is not allowed.
,	@VERBOSE	char		= 'Y'	-- Default to print things
as

	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()

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
/*	select @COAType = COAType
	  from AIMS_Reuters.dbo.tblStdCompanyInfo 
	 where ReportNumber = @ReportNumber
*/
	--Modified 7/12/13 (JM) to pull from internal issuer for companies where no Reuters reported but consensus exists (no COA Type exists in Reuters forecast tables)
	 select @COAType = COA_TYPE
	  from dbo.INTERNAL_ISSUER 
	 where ISSUER_ID = @ISSUER_ID
	 
	 
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


	--1/11/13 (JM) added logic to validate the correct periodicity to use for this company
	select XRef, left(fYearEnd,4) as fiscalyear, 1 as qtr, 0 as semi, max(ce.NumOfEsts) as NumofEst
	into #QtrSemi1
	from AIMS_Reuters.dbo.tblConsensusEstimate ce
	where ExpirationDate is null
	and PeriodType in ('Q1','Q2','Q3','Q4')
	and XRef = @XREF
	group by XRef, fYearEnd
	union
	select XRef, left(fYearEnd,4) as fiscalyear, 0 as qtr, 1 as semi, max(ce.NumOfEsts) as NumofEst
	from AIMS_Reuters.dbo.tblConsensusEstimate ce
	where ExpirationDate is null
	and PeriodType in ('S')
	and XRef = @XREF
	group by XRef, fYearEnd


	select qs1.XRef, fiscalyear, max(ci.periodicity) as periodicity, SUM(qtr) as qtr, SUM(semi) as semi, MAX(NumofEst) as NumofEst, cast(right(ci.fYearEnd,2) as int) as FYEMonth
	into #QtrSemi
	from #QtrSemi1 qs1
	inner join AIMS_Reuters.dbo.tblCompanyInfo ci on ci.XRef = qs1.Xref	
	group by qs1.XRef, fiscalyear, ci.fYearEnd

	drop table #QtrSemi1

	-- collect a list of the most recent entries, because we cannot count on expirationDate being null
	-- Making this a temp table instead of a subselect in the next query saves huge time.

	--Modified 1/9/13 (JM) to account for duplicates in consensus estimates due to Fiscal Year End Changes
	--Check for duplicates arising from two period end dates, i.e. fiscal year end change
	--ANNUAL 
	select xref, periodtype, fiscalyear, MAX(a.FYEMonth) as FYEMonth, Count(*) as tally
	into #DuplicateCheck
	from
		(select ce.Xref, PeriodEndDate, PeriodType, LEFT(ce.fPeriodEnd,4) as FiscalYear, cast(right(ci.fYearEnd,2) as int) as FYEMonth
		  from AIMS_Reuters.dbo.tblConsensusEstimate ce
		  inner join AIMS_Reuters.dbo.tblCompanyInfo ci on ce.XRef = ci.XRef
		 where ce.XRef = @XRef
		   and ce.ExpirationDate is null
		 group by ce.XRef, PeriodEndDate, PeriodType, ce.fPeriodEnd, ci.fYearEnd) a
	where PeriodType = ('A')  
	group by xref, PeriodType, FiscalYear
	 	 
	 --add data for periods where there are no duplicates
	 select ce.XRef, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType
	  into #CE
	  from AIMS_Reuters.dbo.tblConsensusEstimate ce 
	  inner join #DuplicateCheck dc on ce.XRef = dc.XRef and ce.PeriodType = dc.PeriodType and left(ce.fPeriodEnd,4) = dc.FiscalYear   
	 where ce.XRef = @XREF
	   and ExpirationDate is null
	   and dc.tally = 1
	 group by ce.Xref, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType 
	  
	 --when there are duplicates, use the data set with the period end equal to the last annual period end date as defined by Reuters
	 insert	into #CE
	 select ce.XRef, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType
	  from AIMS_Reuters.dbo.tblConsensusEstimate ce 
	  inner join #DuplicateCheck dc on ce.XRef = dc.XRef and ce.PeriodType = dc.PeriodType and left(ce.fPeriodEnd,4) = dc.FiscalYear   
	 where ce.XRef = @XREF
	   and ExpirationDate is null
	   and dc.tally = 2
	   and ce.fYearEnd = ltrim(rtrim(dc.FiscalYear)) + right('00' + cast(dc.FYEMonth as varchar(2)),2)
	 group by ce.Xref, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType 
	 
	 drop table #DuplicateCheck


	--Quarters
	
	 --add data for periods where there are no duplicates
	 insert into #CE
	 select ce.XRef, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType
	  from AIMS_Reuters.dbo.tblConsensusEstimate ce 
	  inner join #QtrSemi qs on ce.XRef = qs.XRef and left(ce.fYearEnd,4) = qs.FiscalYear   
	 where ce.XRef = @XREF
	   and ce.ExpirationDate is null
	   and ce.PeriodType in ('Q1','Q2','Q3','Q4') and (qs.semi = 0 or qs.periodicity = 'Q') 
	   and qs.qtr = 1
	 group by ce.Xref, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType 
	  
	 --when there are duplicates, use the data set with the period end equal to the last annual period end date as defined by Reuters
	 insert	into #CE
	 select ce.XRef, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType
	  from AIMS_Reuters.dbo.tblConsensusEstimate ce 
	  inner join #QtrSemi qs on ce.XRef = qs.XRef and left(ce.fYearEnd,4) = qs.FiscalYear   
	 where ce.XRef = @XREF
	   and ce.ExpirationDate is null
	   and ce.PeriodType in ('Q1','Q2','Q3','Q4') and (qs.semi = 0 or qs.periodicity = 'Q') 	   
	   and qs.qtr = 2
	   and ce.fYearEnd = ltrim(rtrim(qs.FiscalYear))  + right('00' + cast(qs.FYEMonth as varchar(2)),2)
	 group by ce.Xref, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType 
	 
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
				 when ce.Unit = 'MC' then ce.Median /  100
				 when ce.Unit = 'P' then ce.Median  /  100
				 else ce.Median end as Median
		,	ce.Currency
		,	ce.StartDate as SourceDate
		,	ce.PeriodEndDate
		,	ce.PeriodType
		,	ce.NumofEsts as NumberOfEstimates
--		,	ce.High 
		,	Case when ce.Unit = 'T' then ce.High  / 1000
				 when ce.Unit = 'B' then ce.High  * 1000
				 when ce.Unit = 'MC' then ce.High /  100
				 when ce.Unit = 'P' then ce.High  /  100
				 else ce.High end as High
--		,	ce.Low
		,	Case when ce.Unit = 'T' then ce.Low  / 1000
				 when ce.Unit = 'B' then ce.Low  * 1000
				 when ce.Unit = 'MC' then ce.Low /  100
				 when ce.Unit = 'P' then ce.Low  /  100
				 else ce.Low end as Low
--		,	ce.StdDev as StandardDeviation
		,	Case when ce.Unit = 'T' then ce.StdDev  / 1000
				 when ce.Unit = 'B' then ce.StdDev  * 1000
				 when ce.Unit = 'MC' then ce.StdDev /  100
				 when ce.Unit = 'P' then ce.StdDev  /  100
				 else ce.StdDev end as StandardDeviation	
		,	cm.FX_CONV_TYPE as FXConversionType
		,	cm.ESTIMATE_ID as EstimateID
		,	fx.FX_RATE as FXRate
		,	fx.Avg90DayRate
		,	fx.Avg12MonthRate
		,   fxrep.FX_RATE as FXRateRep
		,	fxrep.Avg90DayRate as Avg90DayRateRep
		,	fxrep.Avg12MonthRate as Avg12MonthRateRep
		,	ci.EARNINGS
		,   'ORIGINAL' as TXT
	  into #CONSENSUS_EXTRACT
	  from AIMS_Reuters.dbo.tblConsensusEstimate ce
	 inner join AIMS_Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
	 inner join #CE a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType --and a.StartDate = ce.StartDate
				and  ce.ExpirationDate is null
	  left join dbo.FX_RATES fx on fx.CURRENCY = @CURRENCY_CODE/*ce.Currency*/ and fx.FX_DATE = ce.PeriodEndDate
	  left join dbo.FX_RATES fxrep on fxrep.CURRENCY = ce.Currency and fxrep.FX_DATE = ce.PeriodEndDate
	 where 1=1 --sb.ISSUER_ID = @ISSUER_ID
	   and ce.XRef = @XREF
	   and ce.expirationDate is null
	   and (   (@COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (@COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (@COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (@COAType = 'BNK' and cm.BANK = 'Y') )
	   and @CURRENCY_CODE is not NULL

	drop table #CE
	
-- select 'consensus #CONSENSUS_EXTRACT 1' as A, * from #CONSENSUS_EXTRACT order by periodyear, EstimateID
	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Gather the Annual and quarterly data' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END


	------------------------------------------------------------------------------
	-- This section takes the semi-annual data and creates quarterly data instead
	------------------------------------------------------------------------------
	
		--1/11/13 (JM) to add a check on duplicates within the semi-annual data set
		select ce.XRef, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType
		into #CE2
		from AIMS_Reuters.dbo.tblConsensusEstimate ce 
			inner join #QtrSemi qs on ce.XRef = qs.XRef and left(ce.fYearEnd,4) = qs.FiscalYear      
		where ce.XRef = @XREF
			and ce.ExpirationDate is null
			and ce.PeriodType = 'S' and (qs.qtr = 0 or qs.periodicity = 'S') 
	  		and qs.semi < 3
		group by ce.Xref, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType 
		
	  
	 --when there are duplicates, use the data set with the period end equal to the last annual period end date as defined by Reuters
	 insert	into #CE2
	 select ce.XRef, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType
	  from AIMS_Reuters.dbo.tblConsensusEstimate ce 
	  inner join #QtrSemi qs on ce.XRef = qs.XRef and left(ce.fYearEnd,4) = qs.FiscalYear   
	 where ce.XRef = @XREF
	   and ce.ExpirationDate is null
	   and ce.PeriodType ='S' and (qs.qtr = 0 or qs.periodicity = 'S') 	   
	   and qs.semi > 2
	   and ce.fYearEnd = ltrim(rtrim(qs.FiscalYear))  + right('00' + cast(qs.FYEMonth as varchar(2)),2)
	 group by ce.Xref, ce.PeriodEndDate, ce.fPeriodEnd, ce.EstimateType, ce.PeriodType 

	drop table #QtrSemi
		
		-- Collect the Semi-annual data	
		select	@ISSUER_ID as ISSUER_ID
			,	@CURRENCY_CODE as Local_Currency 
			,	ce.XREF
			,	Left(ce.fYearEnd,4) as PeriodYear
			,	ce.EstimateType
			,	Case when ce.EstimateType = 'ROE' then ce.Median 
					 when ce.EstimateType = 'ROA' then ce.Median 
					 when ce.Unit = 'T' then (ce.Median/2)  / 1000
					 when ce.Unit = 'B' then (ce.Median/2)  * 1000
					 when ce.Unit = 'MC' then (ce.Median/2) /  100
					 when ce.Unit = 'P' then (ce.Median/2)  /  100
					 else ce.Median/2 end as Median
			,	ce.Currency
			,	ce.StartDate as SourceDate
			,	ce.PeriodEndDate
			,	ce.PeriodType
			,	ce.NumofEsts as NumberOfEstimates
--			,	ce.High 
			,	Case when ce.EstimateType = 'ROE' then ce.High 
					 when ce.EstimateType = 'ROA' then ce.High 
					 when ce.Unit = 'T' then (ce.High/2)  / 1000
					 when ce.Unit = 'B' then (ce.High/2)  * 1000
					 when ce.Unit = 'MC' then (ce.High/2) /  100
					 when ce.Unit = 'P' then (ce.High/2)  /  100
					 else ce.High/2 end as High
--			,	ce.Low
			,	Case when ce.EstimateType = 'ROE' then ce.Low 
					 when ce.EstimateType = 'ROA' then ce.Low 
					 when ce.Unit = 'T' then (ce.Low/2)  / 1000
					 when ce.Unit = 'B' then (ce.Low/2)  * 1000
					 when ce.Unit = 'MC' then (ce.Low/2) /  100
					 when ce.Unit = 'P' then (ce.Low/2)  /  100
					 else ce.Low/2 end as Low
--			,	ce.StdDev as StandardDeviation
			,	Case when ce.EstimateType = 'ROE' then ce.StdDev
					 when ce.EstimateType = 'ROA' then ce.StdDev
					 when ce.Unit = 'T' then (ce.StdDev/2)  / 1000
					 when ce.Unit = 'B' then (ce.StdDev/2)  * 1000
					 when ce.Unit = 'MC' then (ce.StdDev/2) /  100
					 when ce.Unit = 'P' then (ce.StdDev/2)  /  100
					 else ce.StdDev/2 end as StandardDeviation
			,	cm.FX_CONV_TYPE as FXConversionType
			,	cm.ESTIMATE_ID as EstimateID
			,	fx.FX_RATE as FXRate
			,	fx.Avg90DayRate
			,	fx.Avg12MonthRate
			,   fxrep.FX_RATE as FXRateRep
			,	fxrep.Avg90DayRate as Avg90DayRateRep
			,	fxrep.Avg12MonthRate as Avg12MonthRateRep
			--	-- If the reported currency is USD then we need the FX rate for the Country currency
			--,	case ce.Currency when 'USD' then fx2.FX_RATE else fx.FX_RATE end as FXRate
			--,	case ce.Currency when 'USD' then fx2.Avg90DayRate else fx.Avg90DayRate end as Avg90DayRate
			--,	case ce.Currency when 'USD' then fx2.Avg12MonthRate else fx.Avg12MonthRate end as Avg12MonthRate
			,	ce.fYearEnd
			,	ce.fPeriodEnd
			,	ci.EARNINGS
		  into #CONSENSUS_EXTRACT_SEMI
		  from AIMS_Reuters.dbo.tblConsensusEstimate ce
		 inner join AIMS_Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
		 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
		 inner join #CE2 a
				on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType --and a.StartDate = ce.StartDate
				and  ce.ExpirationDate is null
		  left join dbo.FX_RATES fxrep on fxrep.CURRENCY = ce.Currency and fxrep.FX_DATE = ce.PeriodEndDate
		  left join dbo.FX_RATES fx on fx.CURRENCY = @CURRENCY_CODE and fx.FX_DATE = ce.PeriodEndDate

		 where 1=1 --sb.ISSUER_ID = @ISSUER_ID
		   and ce.XRef = @XREF
		   and ce.expirationDate is null
		   and (   (@COAType = 'IND' and cm.INDUSTRIAL = 'Y')
				or (@COAType = 'FIN' and cm.INSURANCE = 'Y')
				or (@COAType = 'UTL' and cm.UTILITY = 'Y') 
				or (@COAType = 'BNK' and cm.BANK = 'Y') )
		   and @CURRENCY_CODE is not NULL

		drop table #CE2
		
		if @VERBOSE = 'Y' 
			BEGIN
				print 'After Collect the Semi-annual data	' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
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
			,	ces.FXRateRep
			,	ces.Avg90DayRateRep
			,	ces.Avg12MonthRateRep
			,	ces.EARNINGS
			,   'Q1' as TXT
		  from #CONSENSUS_EXTRACT_SEMI ces
		 where	fYearEnd <> fPeriodEnd

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
			,	ces.FXRateRep
			,	ces.Avg90DayRateRep
			,	ces.Avg12MonthRateRep
			,	ces.EARNINGS
			,   'Q2' as TXT
		  from #CONSENSUS_EXTRACT_SEMI ces
		 where	fYearEnd <> fPeriodEnd

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
			,	ces.FXRateRep
			,	ces.Avg90DayRateRep
			,	ces.Avg12MonthRateRep
			,	ces.EARNINGS
			,   'Q3' as TXT
		  from #CONSENSUS_EXTRACT_SEMI ces
		 where	fYearEnd = fPeriodEnd

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
			,	ces.FXRateRep
			,	ces.Avg90DayRateRep
			,	ces.Avg12MonthRateRep
			,	ces.EARNINGS
			,   'Q4' as TXT
		  from #CONSENSUS_EXTRACT_SEMI ces
		 where	fYearEnd = fPeriodEnd

		if @VERBOSE = 'Y' 
			BEGIN
				print 'After Create the forth quarter retults' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END
			
		drop table #CONSENSUS_EXTRACT_SEMI

	
	
	-- Make the values for CAPEX and DEV negative, per the request by AshmoreEMM (Justin and Gerred)
	update #CONSENSUS_EXTRACT
	   set Median = Median * -1
	   ,   High = High * -1
	   ,   Low = Low * -1
	   ,   StandardDeviation = StandardDeviation * -1
	 where EstimateID in (2, 4)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After negate CAPEX and DIV for Annual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
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
	
	--  select 'consensus #CONSENSUS_EXTRACT 5' as A, * from #CONSENSUS_EXTRACT order by periodyear, EstimateID

-----------------------------------------------
-- Insert the data into the display table
-----------------------------------------------

		if @VERBOSE = 'Y'	print 'insert USD'
		BEGIN tran t1		
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
									  when 'AVG'  then Median / isnull(AVG12MonthRateRep, 1.0)
									  when 'PIT'  then Median / isnull(FXRateRep, 1.0)
									  else Median end AS AMOUNT
			,	NumberOfEstimates
			,	CASE FXConversionType when 'NONE' then High
									  when 'AVG'  then High / isnull(AVG12MonthRateRep, 1.0)
									  when 'PIT'  then High / isnull(FXRateRep, 1.0)
									  else High end AS High
			,	CASE FXConversionType when 'NONE' then Low
									  when 'AVG'  then Low / isnull(AVG12MonthRateRep, 1.0)
									  when 'PIT'  then Low / isnull(FXRateRep, 1.0)
									  else Low end AS Low
			,	isnull(Currency, 'N/A') as SourceCurrency
--			,	StandardDeviation
			,	CASE FXConversionType when 'NONE' then StandardDeviation
									  when 'AVG'  then StandardDeviation / isnull(AVG12MonthRateRep, 1.0)
									  when 'PIT'  then StandardDeviation / isnull(FXRateRep, 1.0)
									  else StandardDeviation end AS StandardDeviation			
			,	'ESTIMATE' as ValueType
		  from #CONSENSUS_EXTRACT
		 where not (FXConversionType = 'AVG' and isnull(AVG12MonthRateRep, 0.0) = 0.0)
		   and not (FXConversionType = 'PIT' and isnull(FXRateRep, 0.0) = 0.0)
--		   and Currency <> 'USD'
		commit tran t1
		
		if @VERBOSE = 'Y' 
			BEGIN
				print 'After Insert the amounts for the currency USD' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END

		-- Insert the amounts for the country currency 
		begin tran t1
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
			,	CASE FXConversionType when 'NONE' then Median
									  when 'AVG'  then (Median / isnull(AVG12MonthRateRep, 1.0)) * isnull(AVG12MonthRate, 1.0)
									  when 'PIT'  then (Median / isnull(FXRateRep, 1.0)) * isnull(FXRate, 1.0)
									  else Median end AS AMOUNT
			,	NumberOfEstimates
			,	CASE FXConversionType when 'NONE' then High
									  when 'AVG'  then (High / isnull(AVG12MonthRateRep, 1.0))* isnull(AVG12MonthRate, 1.0)
									  when 'PIT'  then High / isnull(FXRateRep, 1.0)* isnull(FXRate, 1.0)
									  else High end AS High
			,	CASE FXConversionType when 'NONE' then Low
									  when 'AVG'  then Low / isnull(AVG12MonthRateRep, 1.0)* isnull(AVG12MonthRate, 1.0)
									  when 'PIT'  then Low / isnull(FXRateRep, 1.0)* isnull(FXRate, 1.0)
									  else Low end AS Low			
			,	isnull(ce.Currency, 'N/A') as SourceCurrency
--			,	ce.StandardDeviation
			,	CASE FXConversionType when 'NONE' then StandardDeviation
									  when 'AVG'  then StandardDeviation / isnull(AVG12MonthRateRep, 1.0)* isnull(AVG12MonthRate, 1.0)
									  when 'PIT'  then StandardDeviation / isnull(FXRateRep, 1.0)* isnull(FXRate, 1.0)
									  else StandardDeviation end AS StandardDeviation			
			,	'ESTIMATE' as ValueType
		  from #CONSENSUS_EXTRACT ce
		 where not (ce.FXConversionType = 'AVG' and isnull(ce.AVG12MonthRate, 0.0) = 0.0)
		   and not (ce.FXConversionType = 'PIT' and isnull(ce.FXRate, 0.0) = 0.0)
			and not (ce.FXConversionType = 'AVG' and isnull(ce.AVG12MonthRateRep, 0.0) = 0.0)
		   and not (ce.FXConversionType = 'PIT' and isnull(ce.FXRateRep, 0.0) = 0.0)
--		   and ce.Currency = 'USD'
		commit tran t1
		 
		if @VERBOSE = 'Y' 
			BEGIN
				print 'After Insert the local amounts' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END



	--	-- Insert the amounts for the local currency
	--	begin tran t1
	--	insert into dbo.CURRENT_CONSENSUS_ESTIMATES
	--	Select	Issuer_ID 
	--		,	' ' as SECURITY_ID
	--		,	'REUTERS' as Source
	--		,	SourceDate
	--		,	PeriodType
	--		,	PeriodYear
	--		,	PeriodEndDate
	--		,	'FISCAL' as FiscalType
	--		,	EstimateID
	--		,	isnull(Currency, @CURRENCY_CODE) as Currency		-- For ROE and ROA where there is no currency
	--		,	Median AS AMOUNT
	--		,	NumberOfEstimates
	--		,	High
	--		,	Low
	--		,	isnull(Currency, 'N/A') as SourceCurrency
	--		,	StandardDeviation
	--		,	'ESTIMATE' as ValueType
	--	  from #CONSENSUS_EXTRACT
	----	 where Currency <> 'USD'			-- DLM
	--	commit tran t1
		
		---- Insert the amounts for the currency USD for values without currency like ROA and ROE
		--begin tran t1
		--insert into dbo.CURRENT_CONSENSUS_ESTIMATES
		--Select	Issuer_ID 
		--	,	' ' as SECURITY_ID
		--	,	'REUTERS' as Source
		--	,	SourceDate
		--	,	PeriodType
		--	,	PeriodYear
		--	,	PeriodEndDate
		--	,	'FISCAL' as FiscalType
		--	,	EstimateID
		--	,	isnull(Currency, 'USD') as Currency		-- For ROE and ROA where there is no currency
		--	,	Median AS AMOUNT
		--	,	NumberOfEstimates
		--	,	High
		--	,	Low
		--	,	isnull(Currency, 'N/A') as SourceCurrency
		--	,	StandardDeviation
		--	,	'ESTIMATE' as ValueType
		--  from #CONSENSUS_EXTRACT
		-- where Currency is NULL		-- ROA and ROE have null currency and need this extra Insert
		--commit tran t1
		
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


	--1/10/13 (JM) Modified below to add #Actual temp table to account for dupes
	--Check for duplicates arising from two period end dates, i.e. fiscal year end change
	
	--Annual
	select xref, periodtype, fiscalyear, MAX(a.FYEMonth) as FYEMonth, Count(*) as tally
	into #DuplicateCheck4
	from
		(select act.Xref, PeriodEndDate, PeriodType, LEFT(act.fPeriodEnd,4) as FiscalYear, cast(right(ci.fYearEnd,2) as int) as FYEMonth
		  from AIMS_Reuters.dbo.tblActual act
		   inner join AIMS_Reuters.dbo.tblCompanyInfo ci on act.XRef = ci.XRef
		 where act.XRef = @XRef
		 group by act.XRef, PeriodEndDate, PeriodType, act.fPeriodEnd, ci.fYearEnd) a
	 where PeriodType = ('A')  
	 group by xref, PeriodType, FiscalYear

	 --add data for periods where there are no duplicates
	 select act.XRef, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType
	  into #Actual
	  from AIMS_Reuters.dbo.tblActual act 
	  inner join #DuplicateCheck4 dc on act.XRef = dc.XRef and act.PeriodType = dc.PeriodType and left(act.fPeriodEnd,4) = dc.FiscalYear   
	 where act.XRef = @XREF
	   and dc.tally = 1
	 group by act.Xref, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType 

	 --when there are duplicates, use the data set with the period end equal to the last annual period end date as defined by Reuters
	 insert	into #Actual
	 select act.XRef, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType
	  from AIMS_Reuters.dbo.tblActual act
	  inner join #DuplicateCheck4 dc on act.XRef = dc.XRef and act.PeriodType = dc.PeriodType and left(act.fPeriodEnd,4) = dc.FiscalYear   
	 where act.XRef = @XREF
	   and dc.tally = 2
	   and act.fYearEnd = ltrim(rtrim(dc.FiscalYear))  + right('00' + cast(dc.FYEMonth as varchar(2)),2)
	 group by act.Xref, act.PeriodEndDate,act.fPeriodEnd, act.EstimateType, act.PeriodType 
	 
	 drop table #DuplicateCheck4


	--Quarterly
	select XRef, left(fYearEnd,4) as fiscalyear, 1 as qtr, 0 as semi
	into #QtrSemi1A
	from AIMS_Reuters.dbo.tblActual ac
	where PeriodType in ('Q1','Q2','Q3','Q4')
	and XRef = @XREF
	group by XRef, fYearEnd
	union
	select XRef, left(fYearEnd,4) as fiscalyear, 0 as qtr, 1 as semi
	from AIMS_Reuters.dbo.tblActual ac
	where PeriodType in ('S')
	and XRef = @XREF
	group by XRef, fYearEnd

	select qs1.XRef, fiscalyear, max(ci.periodicity) as periodicity, SUM(qtr) as qtr, SUM(semi) as semi, cast(right(ci.fYearEnd,2) as int) as FYEMonth
	into #QtrSemiA
	from #QtrSemi1A qs1
	inner join AIMS_Reuters.dbo.tblCompanyInfo ci on ci.XRef = qs1.Xref	
	group by qs1.XRef, fiscalyear, ci.fYearEnd
	
	drop table #QtrSemi1A


	 --add data for periods where there are no duplicates
	 insert into #Actual
	 select act.XRef, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType
	  from AIMS_Reuters.dbo.tblActual act 
	  inner join #QtrSemiA qs on act.XRef = qs.XRef and left(act.fYearEnd,4) = qs.FiscalYear   
	 where act.XRef = @XREF
	   and act.PeriodType in ('Q1','Q2','Q3','Q4') and (qs.semi = 0 or qs.periodicity = 'Q') 
	   and qs.qtr = 1
	 group by act.Xref, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType 

	 --when there are duplicates, use the data set with the period end equal to the last annual period end date as defined by Reuters
	 insert	into #Actual
	 select act.XRef, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType
	  from AIMS_Reuters.dbo.tblActual act
	  inner join #QtrSemiA qs on act.XRef = qs.XRef and left(act.fYearEnd,4) = qs.FiscalYear   
	 where act.XRef = @XREF
	   and act.PeriodType in ('Q1','Q2','Q3','Q4') and (qs.semi = 0 or qs.periodicity = 'Q') 	   
	   and qs.qtr = 2
	   and act.fYearEnd = ltrim(rtrim(qs.FiscalYear))  + right('00' + cast(qs.FYEMonth as varchar(2)),2)
	 group by act.Xref, act.PeriodEndDate,act.fPeriodEnd, act.EstimateType, act.PeriodType 
	 
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
		,   fxrep.FX_RATE as FXRateRep
		,	fxrep.Avg90DayRate as Avg90DayRateRep
		,	fxrep.Avg12MonthRate as Avg12MonthRateRep
		,	'ORIG' as TXT1
		,   ci.Earnings
	  into #ACTUAL_EXTRACT
	  from AIMS_Reuters.dbo.tblActual ce
	  inner join AIMS_Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
	 INNER JOIN #Actual a on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
				and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
				and  a.PeriodType = ce.PeriodType 	 
	  left join dbo.FX_RATES fx on fx.CURRENCY = @CURRENCY_CODE/*ce.Currency*/ and fx.FX_DATE = ce.PeriodEndDate
	  left join dbo.FX_RATES fxrep on fxrep.CURRENCY = ce.Currency and fxrep.FX_DATE = ce.PeriodEndDate  
	 where 1=1 --@ISSUER_ID = ISSUER_ID
	   and ce.XRef = @XREF
	   and (   (@COAType = 'IND' and cm.INDUSTRIAL = 'Y')
			or (@COAType = 'FIN' and cm.INSURANCE = 'Y')
			or (@COAType = 'UTL' and cm.UTILITY = 'Y') 
			or (@COAType = 'BNK' and cm.BANK = 'Y') )


	if @VERBOSE = 'Y' 
		BEGIN
			print 'After gather annual and quarterly' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Make the values for CAPEX and DEV negative, per the request by AshmoreEMM (Justin and Gerred)
/*	update #ACTUAL_EXTRACT
	   set ActualValue = ActualValue * -1
	 where EstimateID in (2, 4)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After negate CAPEX and DIV for Actual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
*/



	------------------------------------------------------------------------------
	-- This section takes the semi-annual data and creates quarterly data instead
	------------------------------------------------------------------------------
		
		--1/11/13 (JM) to add a check on duplicates within the semi-annual data set
		select act.XRef, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType
		into #REUTERS_LATEST
		from AIMS_Reuters.dbo.tblActual act
			inner join #QtrSemiA qs on act.XRef = qs.XRef and left(act.fYearEnd,4) = qs.FiscalYear      
		where act.XRef = @XREF
			and act.PeriodType = 'S' and (qs.qtr = 0 or qs.periodicity = 'S') 
	  		and qs.semi < 3
		group by act.Xref, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType 		
	  
	 --when there are duplicates, use the data set with the period end equal to the last annual period end date as defined by Reuters
	 insert	into #REUTERS_LATEST
	select act.XRef, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType
	from AIMS_Reuters.dbo.tblActual act
		inner join #QtrSemiA qs on act.XRef = qs.XRef and left(act.fYearEnd,4) = qs.FiscalYear      
	where act.XRef = @XREF
		and act.PeriodType = 'S' and (qs.qtr = 0 or qs.periodicity = 'S') 
	  	and qs.semi > 2
	   and act.fYearEnd = ltrim(rtrim(qs.FiscalYear))  + right('00' + cast(qs.FYEMonth as varchar(2)),2)
	group by act.Xref, act.PeriodEndDate, act.fPeriodEnd, act.EstimateType, act.PeriodType 		
	 		
		drop table #QtrSemiA
					  
		-- Collect the Semi-annual data	
		select	@ISSUER_ID  as ISSUER_ID 
			,	@CURRENCY_CODE as Local_Currency 
			,	ce.XREF
			,	Left(ce.fYearEnd,4) as PeriodYear
			,	ce.EstimateType
			,	Case when ce.EstimateType = 'ROE' then ce.ActualValue
			         when ce.EstimateType = 'ROA' then ce.ActualValue
					 when ce.Unit = 'T' then (ce.ActualValue/2) / 1000
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
			,   fxrep.FX_RATE as FXRateRep
			,	fxrep.Avg90DayRate as Avg90DayRateRep
			,	fxrep.Avg12MonthRate as Avg12MonthRateRep
			,	ce.fYearEnd
			,	ce.fPeriodEnd
			,   ci.Earnings
		  into #ACTUAL_EXTRACT_SEMI
		  from AIMS_Reuters.dbo.tblActual ce
		 inner join AIMS_Reuters.dbo.tblCompanyInfo ci on ci.XRef = ce.XRef
		 inner join AIMS_Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = ci.ReportNumber
		 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_TYPE = ce.EstimateType
		 INNER JOIN #REUTERS_LATEST a
					on a.XRef = ce.XRef and  a.PeriodEndDate = ce.PeriodEndDate 
					and  a.fPeriodEnd = ce.fPeriodEnd and  a.EstimateType = ce.EstimateType 
					and  a.PeriodType = ce.PeriodType --and a.UpdateDate = ce.UpdateDate
		  left join dbo.FX_RATES fx on fx.CURRENCY = ce.Currency and fx.FX_DATE = ce.PeriodEndDate
		  left join dbo.FX_RATES fxrep on fxrep.CURRENCY = ce.Currency and fxrep.FX_DATE = ce.PeriodEndDate  
		 where 1=1 --@ISSUER_ID = rx.ISSUER_ID
		   and ce.XRef = @XREF
		   and (   (@COAType = 'IND' and cm.INDUSTRIAL = 'Y')
				or (@COAType = 'FIN' and cm.INSURANCE = 'Y')
				or (@COAType = 'UTL' and cm.UTILITY = 'Y') 
				or (@COAType = 'BNK' and cm.BANK = 'Y') )

		drop table #REUTERS_LATEST


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
			,	ces.FXRateRep
			,	ces.Avg90DayRateRep
			,	ces.Avg12MonthRateRep			
			,	'A-Q1' as TXT1
			,   ces.Earnings
		  from #ACTUAL_EXTRACT_SEMI ces
		 where	fYearEnd <> fPeriodEnd

		if @VERBOSE = 'Y' 
			BEGIN
				print 'After first quarter results ++' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END

		-- Create the second quarter retults
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
			,	ces.FXRateRep
			,	ces.Avg90DayRateRep
			,	ces.Avg12MonthRateRep			
			,	'A-Q2' as TXT1
			,   ces.Earnings
		  from #ACTUAL_EXTRACT_SEMI ces
		 where	fYearEnd <> fPeriodEnd

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
			,	ces.FXRateRep
			,	ces.Avg90DayRateRep
			,	ces.Avg12MonthRateRep			
			,	'A-Q3' as TXT1
			,   ces.Earnings
		  from #ACTUAL_EXTRACT_SEMI ces
		 where	fYearEnd = fPeriodEnd

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
			,	ces.FXRateRep
			,	ces.Avg90DayRateRep
			,	ces.Avg12MonthRateRep			
			,	'A-Q4' as TXT1
			,   ces.Earnings
		  from #ACTUAL_EXTRACT_SEMI ces
		 where	fYearEnd = fPeriodEnd

		if @VERBOSE = 'Y' 
			BEGIN
				print 'After forth quarter results' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END
			
		drop table #ACTUAL_EXTRACT_SEMI
		
	
	-- Make the values for CAPEX and DEV negative, per the request by AshmoreEMM (Justin and Gerred)
	update #ACTUAL_EXTRACT
	   set ActualValue = ActualValue * -1
	 where EstimateID in (2, 4)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After negate CAPEX and DIV for Actual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	update #ACTUAL_EXTRACT
	set EstimateID = case when EARNINGS = 'EPS' and EstimateID = 8 then 8		-- correct combo
						when EARNINGS = 'EPSREP' and EstimateID = 9 then 8		-- correct combo
						when EARNINGS = 'EBG' and EstimateID = 5 then 8		-- correct combo
						else 999999 end										-- incorrect combo
	where EstimateID in (8, 9, 5)


	if @VERBOSE = 'Y' 
		BEGIN
			print 'After update 8 9 5 for Actual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	update #ACTUAL_EXTRACT
	set EstimateID = case when EARNINGS = 'EPS' and EstimateID =11 then 11		-- correct combo
						when EARNINGS = 'EPSREP' and EstimateID = 12 then 11	-- correct combo
						when EARNINGS = 'EBG' and EstimateID = 13 then 11		-- correct combo
						else 999999 end										-- incorrect combo
	where EstimateID in (11, 12, 13)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After update 11 12 13 for Actual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	update #ACTUAL_EXTRACT
	set EstimateID = case when EARNINGS = 'EPS' and EstimateID =14 then 14		-- correct combo
						when EARNINGS = 'EPSREP' and EstimateID = 16 then 14	-- correct combo
						when EARNINGS = 'EBG' and EstimateID = 15 then 14		-- correct combo
						else 999999 end										-- incorrect combo
	where EstimateID in (14, 16, 15)

	if @VERBOSE = 'Y' 
		BEGIN
			print 'After Update 14 16 15 for Actual' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- remove incorrect rows
	delete #ACTUAL_EXTRACT
	where EstimateID = 999999

	if @VERBOSE = 'Y' 
		BEGIN
			print 'remove incorrect rows Actual EXTRACT' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	
	
	
		-----------------------------------------------
		-- Insert the data into the display table
		-----------------------------------------------
		if @VERBOSE = 'Y' 	print 'Inserting USD 2'
		-- Insert the amounts for the currency USD
			begin tran t1
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
									  when 'AVG'  then ActualValue / isnull(AVG12MonthRateRep, 1.0)
									  when 'PIT'  then ActualValue / isnull(FXRateRep, 1.0)
									  else ActualValue end AS AMOUNT
			,	0 as NumberOfEstimates
			,	0.0 as High
			,	0.0 as Low
			,	isnull(Currency, 'N/A') as SourceCurrency
			,	0.0 as StandardDeviation
			,	'ACTUAL' as ValueType
		  from #ACTUAL_EXTRACT
		 where not (FXConversionType = 'AVG' and isnull(AVG12MonthRateRep, 0.0) = 0.0)
		   and not (FXConversionType = 'PIT' and isnull(FXRateRep, 0.0) = 0.0)
--		   and Currency <> 'USD'
		commit tran t1


		if @VERBOSE = 'Y' 
			BEGIN
				print 'After inserting USD 2' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END

		if @VERBOSE = 'Y' 	print 'Inserting Local FX'
		begin tran t1
		-- Insert the amounts for the country currency 
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
			,	@CURRENCY_CODE as Currency
			,	CASE FXConversionType when 'NONE' then ActualValue
									  when 'AVG'  then (ActualValue / isnull(AVG12MonthRateRep, 1.0)) * isnull(AVG12MonthRate, 1.0)
									  when 'PIT'  then (ActualValue / isnull(FXRateRep, 1.0)) * isnull(FXRate, 1.0)
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
			and (not (FXConversionType = 'AVG' and isnull(AVG12MonthRateRep, 0.0) = 0.0))
		   and (not (FXConversionType = 'PIT' and isnull(FXRateRep, 0.0) = 0.0))
--		   and Currency = 'USD'
		commit tran t1

		if @VERBOSE = 'Y' 
			BEGIN
				print 'After inserting Local FX' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END
			
		if @VERBOSE = 'Y'	print 'Inserting Local'

		---- Insert the amounts for the local currency
		--begin tran t1
		--insert into dbo.CURRENT_CONSENSUS_ESTIMATES
		--Select	Issuer_ID 
		--	,	' ' as SECURITY_ID
		--	,	'REUTERS' as Source
		--	,	SourceDate
		--	,	PeriodType
		--	,	PeriodYear
		--	,	PeriodEndDate
		--	,	'FISCAL' as FiscalType
		--	,	EstimateID
		--	,   isnull(Currency, @CURRENCY_CODE) as Currency
		--	,	ActualValue AS AMOUNT
		--	,	0 as NumberOfEstimates
		--	,	0.0 as High
		--	,	0.0 as Low
		--	,	isnull(Currency, 'N/A') as SourceCurrency
		--	,	0.0 as StandardDeviation
		--	,	'ACTUAL' as ValueType
		--  from #ACTUAL_EXTRACT
		--commit tran t1


		--if @VERBOSE = 'Y' 
		--	BEGIN
		--		print 'After insert local' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
		--		Set @START = getdate()
		--	END
	
	
	-- Clean up the temp table
--	drop table #XREF;
	drop table #CONSENSUS_EXTRACT;
	drop table #ACTUAL_EXTRACT
	drop table #Actual


GO