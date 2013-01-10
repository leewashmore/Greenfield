IF OBJECT_ID ( 'Get_Data_Process', 'P' ) IS NOT NULL 
DROP PROCEDURE Get_Data_Process;
GO
-----------------------------------------------------------------------------------
-- Purpose:	To move the data from the Reuters raw tables into the display tables:
--			PERIOD_FINANCIALS.  This procedure works as an interface to load the 
--			Reuters data received in file format into the AIMS database.
--
--			START	- Used to initialize a new Run
--			ADD		- Used to add an ISSUER_ID to the new run
--			RUN		- Used to run the process
--			STOP	- Stop the process cleanly, by teeling it to finish the issuer 
--					  being run, but not start another and end.
--			SUMMARY - Returns a summary report of the Run(s)
--			REPORT	- returns a detailed list of the ISSUERS being Run(s)
--			DASHBOARD - Returns real=time information about the run(s) with and 
--						estimated time of completion.
--
-- Author:	David Muench
-- Date:	July 2, 2012
-----------------------------------------------------------------------------------


create procedure Get_Data_Process(
	@COMMAND		varchar(20)				-- What to do
,	@RUN_ID			integer					-- The Process number
,	@ISSUER_ID		varchar(20) = NULL		-- The company identifier		
,	@CALC_LOG		char		= 'Y'		-- write calculation errors to the log table
,	@VERBOSE		char		= 'Y'		-- show the print messages
,	@RUN_ID_OUT		integer	OUTPUT			-- The return for a new process just started.
)
as

--set statistics profile on

	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()

print 'Get_Data-PROCESS @VERBOSE = ' + isnull(@VERBOSE, 'NULL')
	if @VERBOSE = NULL set @VERBOSE = 'N'


	if @COMMAND = 'START'
		BEGIN
			-- Initialize a new run and return the RUN_ID for it.
			if @VERBOSE = 'Y' print 'Start a new process '
			begin transaction
			insert into GET_DATA_RUN (START_TIME, END_TIME, STATUS_TXT, ISSUER_COUNT, STEPS_COUNT, STEPS_DONE, PROCESS_ID)
			values (GETDATE(), NULL, 'START', 0, 0, 0, 0)
			set @RUN_ID_OUT = (select MAX( RUN_ID) from GET_DATA_RUN)
			commit
			if @VERBOSE = 'Y' print 'New Run ID: ' + cast( @RUN_ID_OUT as varchar(10))
		END

	if @COMMAND = 'ADD'
		BEGIN
			-- Add an Issuer ID to the run (requires a RUN_ID and ISSUER_ID)
			if (isnull(@RUN_ID, 0) <= 0)
				if @VERBOSE = 'Y' print 'Invalid RUN_ID for ADD Command:' + cast(@RUN_ID as varchar(10))
			else if (isnull(@ISSUER_ID, 0) <= 0)
				if @VERBOSE = 'Y' print 'Invalid ISSUER_ID for ADD Command:' + cast(@ISSUER_ID as varchar(10))
			else
				BEGIN
					if @VERBOSE = 'Y' print 'Add Issuer ' + @ISSUER_ID + ' to the list for process ' + cast(@RUN_ID as varchar(10))
					insert into GET_DATA_ISSUER_LIST (RUN_ID, ISSUER_ID, START_TIME, END_TIME, STATUS_TXT, PROCESS_ID)
					values (@RUN_ID, @ISSUER_ID, getdate(), null, 'Ready', 0)
				END
		END

	else if @COMMAND = 'RUN'
		BEGIN
			-- Run the process for the required RUN_ID
			if (isnull(@RUN_ID, 0) <= 0)
				print 'Invalid RUN_ID for RUN Command'
			else			
				BEGIN
					print 'Starting RUN for RUN_ID ' + cast(@RUN_ID as varchar(20))
					-- Mark this RUN as Active and determine the number of Issuers being Run.
					update GET_DATA_RUN
					   set STATUS_TXT = 'Active'
					   ,   ISSUER_COUNT = (select COUNT(*) from GET_DATA_ISSUER_LIST where RUN_ID = @RUN_ID)
					 where RUN_ID = @RUN_ID;

					DBCC INDEXDEFRAG (0, FX_RATES, FX_RATES_idx ,0 )
					DBCC INDEXDEFRAG (0, PRICES, PRICES_idx ,0 )
					DBCC INDEXDEFRAG (0, COUNTRY_MASTER, COUNTRY_MASTER_idx2 ,0 )
					DBCC INDEXDEFRAG (0, DATA_MASTER, DATA_MASTER_idx ,0 )
					DBCC INDEXDEFRAG (0, CONSENSUS_MASTER, CONSENSUS_MASTER_idx ,0 )
					DBCC INDEXDEFRAG (0, ISSUER_SHARES, ISSUER_SHARES_inx ,0 )
					DBCC INDEXDEFRAG (0, ISSUER_SHARES, ISSUER_SHARES_indx2 ,0 )
					DBCC INDEXDEFRAG (0, CALC_LIST, CALC_LIST_idx ,0 )
					DBCC INDEXDEFRAG (0, CALC_LIST, CALC_LIST_idx2 ,0 )
					-- Make sure the indexes I need are up to date
					UPDATE STATISTICS GF_SECURITY_BASEVIEW (GF_SECURITY_BASEVIEW_idx, GF_SECURITY_BASEVIEW_idx2
										, GF_SECURITY_BASEVIEW_idx3, GF_SECURITY_BASEVIEW_idx4, GF_SECURITY_BASEVIEW_idx5) WITH FULLSCAN
					UPDATE STATISTICS FX_RATES (FX_RATES_idx) WITH FULLSCAN
					UPDATE STATISTICS PRICES (PRICES_idx) WITH FULLSCAN
					UPDATE STATISTICS COUNTRY_MASTER (COUNTRY_MASTER_idx, COUNTRY_MASTER_idx2) WITH FULLSCAN
					UPDATE STATISTICS DATA_MASTER (DATA_MASTER_idx) WITH FULLSCAN
					UPDATE STATISTICS CONSENSUS_MASTER (CONSENSUS_MASTER_idx) WITH FULLSCAN
					UPDATE STATISTICS CALC_LIST (CALC_LIST_idx) WITH FULLSCAN
					UPDATE STATISTICS CALC_LIST (CALC_LIST_idx2) WITH FULLSCAN
					UPDATE STATISTICS ISSUER_SHARES (ISSUER_SHARES_inx) WITH FULLSCAN
					UPDATE STATISTICS ISSUER_SHARES (ISSUER_SHARES_indx2) WITH FULLSCAN

					if @VERBOSE = 'Y' 
						BEGIN
							print '~~~Before RUN loop - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
							set @START = GETDATE()
						END

					-- Make sure any previously aborted Issuers get rerun.
					update GET_DATA_ISSUER_LIST
					   set STATUS_TXT = 'Ready'
					 where STATUS_TXT = 'Active'
					   and RUN_ID = @RUN_ID


						-- Loop through the data checking each row
						
					select ISSUER_ID
					  into #ISSUER_LIST
					  from GET_DATA_ISSUER_LIST
					 where RUN_ID = @RUN_ID
					   and STATUS_TXT = 'Ready'
					 order by ISSUER_ID
					
					create index ISSUER_LIST_TEMP_idx on #ISSUER_LIST(ISSUER_ID);


					-- Loop through each Issuer in the list and process the Ready IDs one at a time.
					declare @LOOPER integer
					set @LOOPER = 0
					while @LOOPER = 0
						BEGIN
							if @VERBOSE = 'Y' print 'Inside Loop, RUN_ID=' + cast( @RUN_ID as varchar(10))
--							begin transaction
							-- Get the next Issuer to run
							select top 1 @ISSUER_ID =  ISSUER_ID --min(ISSUER_ID)
							  from #ISSUER_LIST	--GET_DATA_ISSUER_LIST
--							 where RUN_ID = @RUN_ID
--							   and STATUS_TXT = 'Ready'

							select @LOOPER = case STATUS_TXT when 'Stopped' then -1 else 0 end
							  from GET_DATA_RUN
							 where RUN_ID = @RUN_ID


							if @LOOPER >= 0
								BEGIN
									if @VERBOSE = 'Y' 
										BEGIN
											print '~~~After Get Next Issuer' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
											set @START = GETDATE()
										END

									delete from #ISSUER_LIST
									 where ISSUER_ID = @ISSUER_ID

									if @VERBOSE = 'Y' 
										BEGIN
											print '~~~After Delete Issuer' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
											set @START = GETDATE()
										END

									if @ISSUER_ID is not NULL
										BEGIN
											-- Let any other process know that it is Active
											begin transaction
											update GET_DATA_ISSUER_LIST
											   set START_TIME = GETDATE()
												,  PROCESS_ID = @@SPID
												,  STATUS_TXT = 'Active'
											 where RUN_ID = @RUN_ID
											   and ISSUER_ID = @ISSUER_ID
											commit

											if @VERBOSE = 'Y' 
												BEGIN
													print '~~~After Active update' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
													set @START = GETDATE()
												END

											-- Run it
											exec Get_Data @ISSUER_ID, @CALC_LOG, @VERBOSE

											if @VERBOSE = 'Y' 
												BEGIN
													print '~~~After Get_Data Run' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
													set @START = GETDATE()
												END
											
											-- Mark it as complete
											begin transaction
											update GET_DATA_ISSUER_LIST
											   set END_TIME = GETDATE()
												,  STATUS_TXT = 'Complete'
											 where RUN_ID = @RUN_ID
											   and ISSUER_ID = @ISSUER_ID
											commit;

											set @ISSUER_ID = NULL;

											if @VERBOSE = 'Y' 
												BEGIN
													print '~~~After Complete update' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
													set @START = GETDATE()
												END

											CHECKPOINT;
										END
									else
										BEGIN
											if @VERBOSE = 'Y' print 'DONE - No more Issuers to process'
		--									commit;
											set @LOOPER = -1
										END				
								END		-- Run Stopped check
						END				-- End While Loop

						if @VERBOSE = 'Y' 
							BEGIN 
								print '~~~After Loop Complete - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
								set @START = GETDATE()
							END
						
					-- Mark the run as complete
					select @LOOPER = COUNT(*) 
					  from GET_DATA_ISSUER_LIST
					 where RUN_ID = @RUN_ID
					   and STATUS_TXT not like 'Complete%'

					if @VERBOSE = 'Y' 
						BEGIN
							print '~~~After Complete Update - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
							set @START = GETDATE()
						END
					 
					if @LOOPER <= 0
						BEGIN
							update GET_DATA_RUN
							   set STATUS_TXT = 'Done'
							   ,   END_TIME = GETDATE()
							   ,   ISSUER_COUNT = (select COUNT(*) from GET_DATA_ISSUER_LIST where RUN_ID = @RUN_ID)
							 where RUN_ID = @RUN_ID;
						END

					if @VERBOSE = 'Y' 	
						BEGIN
							print '~~~After Done Update - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
							set @START = GETDATE()
						END
						
					--Defragment the tables and indexes						
					--DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx ,0 )
					DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx2 ,0 )
					DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx3 ,0 )
					DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx4 ,0 )
					DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx5 ,0 )
					DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx6 ,0 )
--					DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx7 ,0 )
--					DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, NDEX8 ,0 )
					DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS_DISPLAY, PERIOD_FINANCIALS_DISPLAY_idx ,0 )
					--DBCC INDEXDEFRAG (0, CURRENT_CONSENSUS_ESTIMATES, CURRENT_CONSENSUS_ESTIMATES_idx ,0 )
					DBCC INDEXDEFRAG (0, CURRENT_CONSENSUS_ESTIMATES, CURRENT_CONSENSUS_ESTIMATES_idx2 ,0 )
					DBCC INDEXDEFRAG (0, INTERNAL_STATEMENT, INTERNAL_STATEMENT_idx ,0 )
					DBCC INDEXDEFRAG (0, INTERNAL_STATEMENT, INTERNAL_STATEMENT_idx2 ,0 )
					DBCC INDEXDEFRAG (0, INTERNAL_DATA, INTERNAL_DATA_idx ,0 )
					DBCC INDEXDEFRAG (0, INTERNAL_ISSUER, INTERNAL_ISSUER_idx ,0 )
					DBCC INDEXDEFRAG (0, INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION, INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION_idx ,0 )
					DBCC INDEXDEFRAG (0, ISSUER_SHARES_COMPOSITION, ISSUER_SHARES_COMPOSITION_idx ,0 )
					DBCC INDEXDEFRAG (0, ISSUER_SHARES_COMPOSITION, ISSUER_SHARES_COMPOSITION_idx2 ,0 )
					DBCC INDEXDEFRAG (0, CALC_LOG, CALC_LOG_idx ,0 )
					DBCC INDEXDEFRAG (0, CALC_LOG, CALC_LOG_idx2 ,0 )
					DBCC INDEXDEFRAG (0, GRID_TYPES, GRID_TYPES_idx ,0 )
					DBCC INDEXDEFRAG (0, FINANCIAL_GRIDS_DISPLAY, FINANCIAL_GRIDS_DISPLAY_idx ,0 )
					DBCC INDEXDEFRAG (0, FINANCIAL_GRIDS_DISPLAY, FINANCIAL_GRIDS_DISPLAY_idx2 ,0 )
					DBCC INDEXDEFRAG (0, FINANCIAL_GRIDS_DISPLAY, FINANCIAL_GRIDS_DISPLAY_idx3 ,0 )
					DBCC INDEXDEFRAG (0, FAIR_VALUE, FAIR_VALUE_idx ,0 )
					DBCC INDEXDEFRAG (0, FINSTAT_DISPLAY, FINSTAT_DISPLAY_idx ,0 )
					DBCC INDEXDEFRAG (0, BENCHMARK_FINANCIALS, BENCHMARK_FINANCIALS_idx ,0 )
					DBCC INDEXDEFRAG (0, PORTFOLIO_FINANCIALS, PORTFOLIO_FINANCIALS_idx ,0 )
					--DBCC INDEXDEFRAG (0, BENCHMARK_NODE_FINANCIALS, BENCHMARK_NODE_FINANCIALS_idx ,0 )
					DBCC INDEXDEFRAG (0, MODEL_INPUTS_CTY, MODEL_INPUTS_CTY_idx ,0 )

					if @VERBOSE = 'Y' 
						BEGIN
							print '~~~After DBCC Index Defrag - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
							set @START = GETDATE()
						END

					-- Update the statistics on all indexes
					--UPDATE STATISTICS PERIOD_FINANCIALS (PERIOD_FINANCIALS_idx) WITH FULLSCAN
					UPDATE STATISTICS PERIOD_FINANCIALS (PERIOD_FINANCIALS_idx2) WITH FULLSCAN
					UPDATE STATISTICS PERIOD_FINANCIALS (PERIOD_FINANCIALS_idx3) WITH FULLSCAN
					UPDATE STATISTICS PERIOD_FINANCIALS (PERIOD_FINANCIALS_idx4) WITH FULLSCAN
					UPDATE STATISTICS PERIOD_FINANCIALS (PERIOD_FINANCIALS_idx5) WITH FULLSCAN
					UPDATE STATISTICS PERIOD_FINANCIALS (PERIOD_FINANCIALS_idx6) WITH FULLSCAN
--					UPDATE STATISTICS PERIOD_FINANCIALS (PERIOD_FINANCIALS_idx7) WITH FULLSCAN
--					UPDATE STATISTICS PERIOD_FINANCIALS (NDEX8) WITH FULLSCAN
					UPDATE STATISTICS PERIOD_FINANCIALS_DISPLAY (PERIOD_FINANCIALS_DISPLAY_idx) WITH FULLSCAN
					--UPDATE STATISTICS CURRENT_CONSENSUS_ESTIMATES (CURRENT_CONSENSUS_ESTIMATES_idx) WITH FULLSCAN
					UPDATE STATISTICS CURRENT_CONSENSUS_ESTIMATES (CURRENT_CONSENSUS_ESTIMATES_idx2) WITH FULLSCAN
					UPDATE STATISTICS INTERNAL_STATEMENT (INTERNAL_STATEMENT_idx) WITH FULLSCAN
					UPDATE STATISTICS INTERNAL_STATEMENT (INTERNAL_STATEMENT_idx2) WITH FULLSCAN
					UPDATE STATISTICS INTERNAL_DATA (INTERNAL_DATA_idx) WITH FULLSCAN
					UPDATE STATISTICS INTERNAL_ISSUER (INTERNAL_ISSUER_idx) WITH FULLSCAN
					UPDATE STATISTICS INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION (INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION_idx) WITH FULLSCAN
					UPDATE STATISTICS ISSUER_SHARES_COMPOSITION (ISSUER_SHARES_COMPOSITION_idx) WITH FULLSCAN
					UPDATE STATISTICS ISSUER_SHARES_COMPOSITION (ISSUER_SHARES_COMPOSITION_idx2) WITH FULLSCAN
					UPDATE STATISTICS CALC_LOG (CALC_LOG_idx) WITH FULLSCAN
					UPDATE STATISTICS CALC_LOG (CALC_LOG_idx2) WITH FULLSCAN
					UPDATE STATISTICS GRID_TYPES (GRID_TYPES_idx) WITH FULLSCAN
					UPDATE STATISTICS FINANCIAL_GRIDS_DISPLAY (FINANCIAL_GRIDS_DISPLAY_idx) WITH FULLSCAN
					UPDATE STATISTICS FINANCIAL_GRIDS_DISPLAY (FINANCIAL_GRIDS_DISPLAY_idx2) WITH FULLSCAN
					UPDATE STATISTICS FINANCIAL_GRIDS_DISPLAY (FINANCIAL_GRIDS_DISPLAY_idx3) WITH FULLSCAN
					UPDATE STATISTICS FAIR_VALUE (FAIR_VALUE_idx) WITH FULLSCAN
					UPDATE STATISTICS FINSTAT_DISPLAY (FINSTAT_DISPLAY_idx) WITH FULLSCAN
					UPDATE STATISTICS BENCHMARK_FINANCIALS (BENCHMARK_FINANCIALS_idx) WITH FULLSCAN
					UPDATE STATISTICS PORTFOLIO_FINANCIALS (PORTFOLIO_FINANCIALS_idx) WITH FULLSCAN
					--UPDATE STATISTICS BENCHMARK_NODE_FINANCIALS (BENCHMARK_NODE_FINANCIALS_idx) WITH FULLSCAN
					UPDATE STATISTICS CALC_LIST (CALC_LIST_idx) WITH FULLSCAN
					UPDATE STATISTICS CALC_LIST (CALC_LIST_idx2) WITH FULLSCAN
					UPDATE STATISTICS MODEL_INPUTS_CTY (MODEL_INPUTS_CTY_idx) WITH FULLSCAN

					if @VERBOSE = 'Y' 
						BEGIN
							print '~~~After Update Statistics - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
							set @START = GETDATE()
						END
						
					-- This message is for JAMS to recognise that the process is complete
					print 'Get_Data Completed Successfully.'

				END						-- Else
		END								-- End Command RUN
		
	if @COMMAND = 'STOP'
		BEGIN
			-- Initialize a new run and return the RUN_ID for it.
			if @VERBOSE = 'Y' print 'Stop a Active process '
			begin transaction
			update GET_DATA_RUN
			   set STATUS_TXT = 'Stopped'
			   ,   END_TIME = GETDATE()
			   ,   ISSUER_COUNT = (select COUNT(*) from GET_DATA_ISSUER_LIST where RUN_ID = @RUN_ID)
			 where (@RUN_ID is null or RUN_ID = @RUN_ID)
			   and STATUS_TXT = 'Active'
			commit
			if @VERBOSE = 'Y' print 'New Run ID: ' + cast( @RUN_ID_OUT as varchar(10))
		END


	if @COMMAND = 'SUMMARY'
		BEGIN
			-- Provide a summary report about the run(s).  If RUN_ID is NULL show all
			select RUN_ID, START_TIME, END_TIME, STATUS_TXT, ISSUER_COUNT
			  from dbo.GET_DATA_RUN
			 where (@RUN_ID is NULL or RUN_ID = @RUN_ID)
			 order by RUN_ID;
		END

	if @COMMAND = 'REPORT'
		BEGIN
			-- Provide a detailed report about the run(s).  If RUN_ID is NULL show all
			select *, cast(DATEDIFF(MS, START_TIME, END_TIME) as decimal(32,3)) / 1000.0 as Seconds
			  from GET_DATA_ISSUER_LIST
			 where (@RUN_ID is NULL or RUN_ID = @RUN_ID)
			 order by RUN_ID, STATUS_TXT, ISSUER_ID;
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
				,  Total_Seconds
				,  Total_Seconds / c.COMPLETE as Avg_Secs_per_Issuer
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
						
			  left join (select RUN_ID, SUM(Elapsed_Seconds) as Total_Seconds
						   from (select RUN_ID, (cast(DATEDIFF(MS, START_TIME, END_TIME) as decimal(32,3)) / 1000.0) as Elapsed_Seconds
								   from GET_DATA_ISSUER_LIST  
								  where STATUS_TXT = 'Complete') z
						  group by RUN_ID
						) e on e.RUN_ID = l.RUN_ID
			 where (@RUN_ID is NULL or l.RUN_ID = @RUN_ID)
			 order by RUN_ID desc;
		END
 

GO
