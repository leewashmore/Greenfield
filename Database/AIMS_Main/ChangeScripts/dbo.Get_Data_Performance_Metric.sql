
/****** Object:  StoredProcedure [dbo].[Get_Data_Performance_Metric]    Script Date: 03/19/2014 09:40:36 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Get_Data_Performance_Metric]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Get_Data_Performance_Metric]
GO


/****** Object:  StoredProcedure [dbo].[Get_Data_Performance_Metric]    Script Date: 03/19/2014 09:40:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[Get_Data_Performance_Metric](
	@COMMAND		varchar(20)				-- What to do
,	@RUN_ID			integer		
)
AS
if @COMMAND = 'SUMMARY'
		BEGIN
			-- Provide a summary report about the run(s).  If RUN_ID is NULL show all
			select RUN_ID, START_TIME, END_TIME, STATUS_TXT, ISSUER_COUNT
			  from dbo.GET_DATA_RUN
			 where (@RUN_ID is NULL or RUN_ID = @RUN_ID)
			 order by RUN_ID;
		END
declare @ThreadCount int = 20;
	if @COMMAND = 'REPORT'
		BEGIN
			-- Provide a detailed report about the run(s).  If RUN_ID is NULL show all
			select *
				 , cast(DATEDIFF(MS, START_TIME, END_TIME) as decimal(32,3)) / (1000.0*@ThreadCount) as Seconds --uncomment during multithread
			--	, cast(DATEDIFF(MS, START_TIME, END_TIME) as decimal(32,3)) / 1000.0 as seconds -- Comment this during multi thread
			  from GET_DATA_ISSUER_LIST
			 where (@RUN_ID is NULL or RUN_ID = @RUN_ID)
			 order by end_time desc
		END

	if @COMMAND = 'DASHBOARD'
		BEGIN
			-- Provide a detailed report about the run(s) with an extimate completion time.
			select l.RUN_ID
				,  l.START_TIME as Started_At
				,  l.END_TIME as Complete_At
				,  ISNULL(cnt.CNT, 0) as CNT
				,  isnull(c.COMPLETE, 0) as COMPLETE
				,  ((cast(isnull(c.COMPLETE, 0) as decimal(32,6))) / cast(ISNULL(cnt.CNT, 1) as decimal(32,6))*100) as Percent_Complete
			--	,  cast(DATEDIFF(MS, l.START_TIME, l.END_TIME) as decimal(32,3)) / 1000.0 as  Total_Seconds --uncomment during multithread
				,  Total_Seconds -- Comment this during multi thread
			--	,  cast(DATEDIFF(MS, l.START_TIME, l.END_TIME) as decimal(32,3)) /(1000.0 * c.COMPLETE) as average_per_issuer --uncomment during multithread
				,  Total_Seconds / c.COMPLETE as Avg_Secs_per_Issuer --- Comment this during multi thread
				,  (Total_Seconds / c.COMPLETE * (cnt.cnt - c.COMPLETE)) as Seconds_Remaining
				,  DATEADD(second, (Total_Seconds / c.COMPLETE * (cnt.cnt - c.COMPLETE)), c.MAX_TIME) as ETA
			  from GET_DATA_RUN l with(NOLOCK)
			  left join (select RUN_ID, COUNT(*) as CNT 
						   from GET_DATA_ISSUER_LIST 
						  group by RUN_ID
						) cnt on cnt.RUN_ID = l.RUN_ID

			  left join (select RUN_ID, COUNT(*) as COMPLETE, MAX(END_TIME) as MAX_TIME
						   from GET_DATA_ISSUER_LIST 
						  where STATUS_TXT = 'Complete'
						  group by RUN_ID
						) c on c.RUN_ID = l.RUN_ID
						
			  left join (select RUN_ID
		--						,SUM(Elapsed_Seconds) as Total_Seconds	--- Comment this during multi thread
							,	 SUM(Elapsed_Seconds)/@ThreadCount as Total_Seconds --uncomment during multithread
						   from (select RUN_ID, (cast(DATEDIFF(MS, START_TIME, END_TIME) as decimal(32,3)) / 1000.0) as Elapsed_Seconds
								   from GET_DATA_ISSUER_LIST  
								  where STATUS_TXT = 'Complete') z
						  group by RUN_ID
						) e on e.RUN_ID = l.RUN_ID
			 where (@RUN_ID is NULL or l.RUN_ID = @RUN_ID)
			 order by RUN_ID desc;
		END
		
		
		if @COMMAND = 'FAILED'
		BEGIN
			-- Provide a detailed report about the run(s).  If RUN_ID is NULL show all
			select *
				 , cast(DATEDIFF(MS, START_TIME, END_TIME) as decimal(32,3)) / (1000.0*@ThreadCount) as Seconds --uncomment during multithread
			--	, cast(DATEDIFF(MS, START_TIME, END_TIME) as decimal(32,3)) / 1000.0 as seconds -- Comment this during multi thread
			  from GET_DATA_ISSUER_LIST
			 where (@RUN_ID is NULL or RUN_ID = @RUN_ID) and status_txt <> 'Complete'
			 order by end_time desc
		END

GO


