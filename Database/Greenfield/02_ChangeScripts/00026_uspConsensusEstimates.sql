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

create procedure [dbo].[ConsensusEstimates]
		@Xref				varchar(9)	
	,	@Name				varchar(50)	
	,	@PeriodType			varchar(2)	
AS
BEGIN
Declare @FirstPeriodEndDate	datetime

SET NOCOUNT OFF;
SET FMTONLY OFF;

	-- Set the parameter values to defaults if not provided

	if( (@Xref is NULL) and (@Name is NULL) )
		select @Name = 'Petroleo Brasileiro SA - Petrobras';

	if( (@Xref is NULL) and (@Name is not NULL) )
		select @Xref = Xref from dbo.tblCompanyInfo where Name = @Name;

	if( @PeriodType is NULL )
		set @PeriodType = 'A';

	if( @FirstPeriodEndDate is NULL )
		set @FirstPeriodEndDate = '2009-12-31 00:00:00.000';



	-- Create a list of all the years needed.
	select 'A200912' as  sPeriodEnd, 'ADec2009' as Period into #years
	insert #years values ('A201012', 'ADec2010')
	insert #years values ('E200912', 'EDec2010')
	insert #years values ('E201012', 'EDec2010')
	insert #years values ('E201112', 'EDec2011')
	insert #years values ('E201212', 'EDec2012')
	insert #years values ('E201312', 'EDec2013')
	insert #years values ('E201412', 'EDec2014')
	insert #years values ('E201512', 'EDec2015')

	-- Get a list of all the Estimate types needed
	select distinct EstimateType 
	  into #esttype
	  from  (
				select distinct EstimateType
				  from dbo.tblActual 
				 where 1=1
				   and XRef = @Xref
				   and PeriodEndDate >= @FirstPeriodEndDate
				   and PeriodType = @PeriodType
			union
				select distinct ce.EstimateType
				  from dbo.tblConsensusEstimate ce
				 where 1=1
				   and XRef = @Xref
				   and PeriodType = @PeriodType
				   and fPeriodEnd >= '200912'
			) a


	-- Get the data
	select @XRef as Xref, y.period, y.EstimateType, a.Amount as Amount, a.NumOfEsts, a.High, a.Low, a.StdDev, a.AnnouncementDate
	  into #est
	  from  (
			select y.Period, et.EstimateType, y.sPeriodEnd
			  from #years y inner join #esttype et on 1=1
			) y
	  left join ( 
				-- Get the actual data for 2009 & 2010
				select XRef, 'A'+fPeriodEnd as sPeriodEnd, EstimateType, ActualValue as Amount, 0 as NumOfEsts, '' as High, '' as Low, '' as StdDev, AnnouncementDate
				  from dbo.tblActual 
				 where XRef = @Xref
				   and PeriodEndDate >= @FirstPeriodEndDate
				   and PeriodType = @PeriodType
			union
				-- Get the Estimated data
				select ce.XRef, 'E'+ce.fPeriodEnd as sPeriodEnd, ce.EstimateType, ce.Mean as Amount, NumOfEsts, ce.High, ce.Low, ce.StdDev, '' as AnnouncementDate
				  from dbo.tblConsensusEstimate ce
				 inner join (
							select XRef, fPeriodEnd, max(OriginalDate) as OriginalDate
							  from dbo.tblConsensusEstimate
							 where 1=1
							   and XRef = 100013798
							   and PeriodType = @PeriodType
							   and fPeriodEnd >= '200912'
							 group by XRef, fPeriodEnd
							 ) ceX on ceX.XRef = ce.XRef and ceX.fPeriodEnd = ce.fPeriodEnd and ceX.OriginalDate = ce.OriginalDate
				 where 1=1
				   and ce.XRef = 100013798
	--			   and ce.EstimateType = 'ROE'
				   and ce.PeriodType = @PeriodType
				   and ce.PeriodEndDate >= @FirstPeriodEndDate
				) a on a.sPeriodEnd = y.sPeriodEnd and a.EstimateType = y.EstimateType
			
		 
		 
	-- PIVOT the data and put it into correct rows.
	select	max(Xref) as Xref
		,	EstimateType
		,	srt
		,	AmtType
		,	max([ADec2009]) as Actual_Dec_2009
		,	max([ADec2010]) as Actual_Dec_2010
		,	max([EDec2009]) as Estimated_Dec_2009
		,	max([EDec2010]) as Estimated_Dec_2010
		,	max([EDec2011]) as Estimated_Dec_2011
		,	max([EDec2012]) as Estimated_Dec_2012
		,	max([EDec2013]) as Estimated_Dec_2013
		,	max([EDec2014]) as Estimated_Dec_2014
		,	max([EDec2015]) as Estimated_Dec_2015
	 from (
			select pvt.Xref, EstimateType, '0' as Srt, 'Amount' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(Amount) 
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '1' as Srt, 'NumOfEsts' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(NumOfEsts)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '2' as Srt, 'High' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(High)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '3' as Srt, 'Low' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(Low)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '4' as Srt, 'StdDev' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(StdDev)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '5' as Srt, 'AnnouncementDate' as AmtType
				,	case when pvt.[ADec2009] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[ADec2009], 101) else '' end as ADec2009
				,	case when pvt.[ADec2010] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[ADec2010], 101) else '' end as ADec2010 
				,	case when pvt.[EDec2009] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2009], 101) else '' end as ADec2009
				,	case when pvt.[EDec2010] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2010], 101) else '' end as ADec2010
				,	case when pvt.[EDec2011] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2011], 101) else '' end as ADec2011
				,	case when pvt.[EDec2012] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2012], 101) else '' end as ADec2012
				,	case when pvt.[EDec2013] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2013], 101) else '' end as ADec2013 
				,	case when pvt.[EDec2014] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2014], 101) else '' end as ADec2014
				,	case when pvt.[EDec2015] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2015], 101) else '' end as ADec2015 
			  from #est e 
			 pivot 
			 ( max(AnnouncementDate)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
			) a
	 group by EstimateType, Srt, AmtType
	 order by EstimateType, Srt, AmtType
	;

	-- Clean up
	drop table #est;
	drop table #years;
	drop table #esttype;

END;
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






