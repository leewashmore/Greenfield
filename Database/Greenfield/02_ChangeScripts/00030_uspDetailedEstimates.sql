set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00001'
declare @CurrentScriptVersion as nvarchar(100) = '00002'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[DetailedEstimates]
		@Xref				varchar(9)	
	,	@Name				varchar(50)	
	,	@PeriodType			varchar(2)		
	,	@EstimateType		varchar(20)	
AS
BEGIN

SET NOCOUNT OFF;
SET FMTONLY OFF;

Declare @FirstPeriodEndDate	datetime

	-- Set the parameter values to defaults if not provided

	if( (@Xref is NULL) and (@Name is NULL) )
		select @Name = 'Petroleo Brasileiro SA - Petrobras';

	if( (@Xref is NULL) and (@Name is not NULL) )
		select @Xref = Xref from dbo.tblCompanyInfo where Name = @Name;

	if( @PeriodType is NULL )
		set @PeriodType = 'A';

	if( @EstimateType is NULL )
		set @EstimateType = 'EPS';

	if( @FirstPeriodEndDate is NULL )
		set @FirstPeriodEndDate = '2009-12-31 00:00:00.000';

		
	-- Get the data needed for the first part.
	select	ci.Name
		,	ce.EstimateType
		,	'FY'+left(ce.fPeriodEnd, 4) as Period
		,	ce.PeriodEndDate
		,	ce.OriginalDate
		,	ce.ExpirationDate
		,	ce.PeriodType
		,	ce.Currency
		,	ce.Mean 
		,	ce.Median
		,	ce.NumOfEsts
		,	ce.High
		,	ce.Low
		,	ce.StdDev
		,	act.Actual
	  into	#est
	  from dbo.tblCompanyInfo ci
	 inner Join dbo.tblConsensusEstimate ce on ce.Xref = ci.Xref
	 inner join	(select Xref, PeriodEndDate, MAX(OriginalDate) as OriginalDate 
				  from dbo.tblConsensusEstimate 
				 where PeriodType = @PeriodType
				   and EstimateType = @EstimateType
				   and XRef = 100033558
				 group by Xref, PeriodEndDate
				) latest on latest.XRef = ce.XRef and latest.PeriodEndDate = ce.PeriodEndDate and latest.OriginalDate = ce.OriginalDate
				
	  left join	(select	a.XRef
					,	a.EstimateType
					,	a.PeriodEndDate
					,	a.PeriodType
					,	a.Currency
					,	a.ActualValue as Actual
				  from dbo.tblCompanyInfo ci
				 inner Join dbo.tblActual a on a.Xref = ci.Xref
				 where a.EstimateType = @EstimateType
				   and a.PeriodType = @PeriodType
				   and a.XRef = @Xref
				) act on act.XRef = ci.Xref and act.PeriodEndDate = ce.PeriodEndDate and act.Currency = ce.Currency
	 where 1=1
	   and ce.PeriodType = @PeriodType
	   and ce.EstimateType = @EstimateType
	   and (@FirstPeriodEndDate is NULL or ce.PeriodEndDate >= @FirstPeriodEndDate)
	   and ce.XRef = 100033558
	 ;



	-- PIVOT the data and put it into correct rows.
	select	Name
		,	EstimateType
		,	srt
		,	AmtType
		,	max([FY2009]) as FY2009
		,	max([FY2010]) as FY2010
		,	max([FY2011]) as FY2011
		,	max([FY2012]) as FY2012
		,	max([FY2013]) as FY2013
		,	max([FY2014]) as FY2014
		,	max([FY2015]) as FY2015
	 from (
			select pvt.Name, EstimateType, 1 as Srt, 'Mean' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(Mean)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 2 as Srt, 'Median' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(Median)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 3 as Srt, 'NumOfEsts' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(NumOfEsts)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 4 as Srt, 'High' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(High)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 5 as Srt, 'Low' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(Low)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 6 as Srt, 'StdDev' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(StdDev)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 7 as Srt, 'Actual' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(Actual)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
			) a
	 group by Name, EstimateType, Srt, AmtType
	 order by Srt
	;

	-- Clean up the temp table
--	drop table #est;
END;
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






