SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
------------------------------------------------------------------------
-- Purpose:	This procedure runs the GET_DATA_PROCESS using a list of
--			ISSUER_IDs in a table.
--
--   RUN_MODE:  F = Full (Recalculates all Values),  I = Incremental (Runs Price-Based Calculations only)
-- Author:	David Muench
-- Date:	Oct 18, 2012
------------------------------------------------------------------------
alter procedure [dbo].[RUN_GET_DATA_PROCESS] (
	@CALC_LOG		char	= 'N'
,	@VERBOSE		char	= 'N'
,   @RUN_MODE		char	= 'F')  
as

	-- Declare the parameter variables for the GET_DATA_PROCESS
	declare @RUN_ID integer
	declare @RUN integer
	
	-- Initialize the process
	exec Get_Data_Process 'START', NULL, NULL, NULL, NULL, @RUN_ID OUTPUT, @RUN_MODE;
	print 'Use this RUN_ID='+isnull(cast( @RUN_ID as varchar(10)), 'NULL');
	set @RUN = @RUN_ID;

	-- Insert the ISSUER_ID values into the list to be run.
	insert into GET_DATA_ISSUER_LIST (RUN_ID, ISSUER_ID, START_TIME, END_TIME, STATUS_TXT, PROCESS_ID)
	select distinct @RUN, issuer_id, GETDATE(), null, 'Ready', 0 
	  from NIGHTLY_ISSUER_LIST;

	-- Start the process running.
	exec Get_Data_Process 'RUN', @RUN, NULL, @CALC_LOG, @VERBOSE, @RUN_ID, @RUN_MODE;
GO
