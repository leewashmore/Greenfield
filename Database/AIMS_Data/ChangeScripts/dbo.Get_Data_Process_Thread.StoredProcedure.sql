USE [AIMS_Main]
GO

/****** Object:  StoredProcedure [dbo].[Get_Data_Process_Thread]    Script Date: 02/22/2013 18:52:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

-----------------------------------------------------------------------------------
-- Purpose:	Called from the c# console app to set the issuer list to be processed
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
-- Author:	Akhtar Nazirali 
-- Date:	February 12, 2013
-----------------------------------------------------------------------------------


CREATE procedure [dbo].[Get_Data_Process_Thread](
	@COMMAND	varchar(20)='START'				-- What to do
,	@RUN_MODE	char(1)='F'
,	@RUN_ID_OUT		integer	OUTPUT	

		
)
as
	
	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()

	-- Initialize a new run and return the RUN_ID for it.
	


				if @RUN_MODE = 'F'
					begin
						truncate table period_financials
						--select 'test'
					end

				--if @RUN_MODE = 'I'
				--	begin
				--		DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx2 ,0 )
				--		DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx3 ,0 )
				--		DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx4 ,0 )
				--		DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx5 ,0 )
				--		DBCC INDEXDEFRAG (0, PERIOD_FINANCIALS, PERIOD_FINANCIALS_idx6 ,0 )
				--	end

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
					
			if @COMMAND = 'START'
			begin
				begin transaction t1
					insert into GET_DATA_RUN (START_TIME, END_TIME, STATUS_TXT, ISSUER_COUNT, STEPS_COUNT, STEPS_DONE, PROCESS_ID)
					values (GETDATE(), NULL, 'START', 0, 0, 0, 0)
					set @RUN_ID_OUT = (select MAX( RUN_ID) from GET_DATA_RUN)
		
					insert into GET_DATA_ISSUER_LIST (RUN_ID, ISSUER_ID, START_TIME, END_TIME, STATUS_TXT, PROCESS_ID)
					 select distinct @RUN_ID_OUT, a.ISSUER_ID, GETDATE(), null, 'Ready', 0  from 
						(select gsb.issuer_id 
							from GF_SECURITY_BASEVIEW gsb
							where gsb.XREF is not null and gsb.ISSUER_ID is not null and gsb.ISSUER_ID <> '__ISSUER'
							group by gsb.ISSUER_ID
							union 
						select ints.ISSUER_ID  
							from INTERNAL_STATEMENT ints
							where ints.ROOT_SOURCE_DATE > DATEADD(day, -120, getdate())
							group by ints.ISSUER_ID) a
					
					--select distinct @RUN_ID_OUT, issuer_id, GETDATE(), null, 'Ready', 0 
					--from TEST_issuer_list;
					
					

					
				commit transaction t1
			end
		 		
		 	update GET_DATA_RUN
				   set STATUS_TXT = 'Active'
				   ,   ISSUER_COUNT = (select COUNT(*) from GET_DATA_ISSUER_LIST where RUN_ID = @RUN_ID_OUT)
					 where RUN_ID = @RUN_ID_OUT;

					
		return @RUN_ID_OUT

GO


