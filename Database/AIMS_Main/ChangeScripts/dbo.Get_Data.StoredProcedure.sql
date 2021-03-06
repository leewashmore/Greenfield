SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
-- Purpose:	To move the data from the Reuters raw tables into the display tables:
--			PERIOD_FINANCIALS.  This procedure works as an interface to load the 
--			Reuters data received in file format into the AIMS database.
--
-- Author:	David Muench
-- Date:	July 2, 2012
-----------------------------------------------------------------------------------

alter procedure [dbo].[Get_Data](
	@ISSUER_ID			varchar(20) = NULL					-- The company identifier		
,	@CALC_LOG			char		= 'N'			-- write calculation errors to the log table
,	@VERBOSE			char		= 'N'
,	@RUN_MODE			char		= 'F'   -- F = Full (Recalculates all Values),  I = Incremental (Runs Price-Based Calculations only)
,	@STAGE				char		= 'N'	-- N= Records inserted in period_financials_Security and period_financials_issuer.  Y = Period_financials_security_stage and Period_financials_issuer_Stage
)
as

set nocount on
	declare @START		datetime		-- the time the calc starts
	Set @START = getdate()

	--Modified 2/8/13 (JM) to add call to new set_coa_type stored procedure
	exec set_coa_type @ISSUER_ID		
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After Set COA Type for ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			set @START = GETDATE()
		END			

	if @RUN_MODE = 'F' -- For Full runs, delete all of the data and call the stored procedures to re-populate PERIOD_FINANCIALS
	BEGIN
		--Modified 2/7/13 (JM) to move the deletion of records out of CALC_CONSENSUS_ESTIMATES
		
		begin transaction;					-- Lock the table
		
		-- remove the rows that will be reinserted.
		delete from CURRENT_CONSENSUS_ESTIMATES
		 where @ISSUER_ID = ISSUER_ID

		delete CCE from CURRENT_CONSENSUS_ESTIMATES CCE
		INNER JOIN GF_SECURITY_BASEVIEW GSB ON  GSB.SECURITY_ID = CCE.SECURITY_ID
    	where  GSB.ISSUER_ID = @ISSUER_ID
		 
--		 SECURITY_ID in (select SECURITY_ID from GF_SECURITY_BASEVIEW where ISSUER_ID = @ISSUER_ID)

		commit;								-- Unlock the table
		
		
		if @VERBOSE = 'Y' 
			BEGIN
				print '*** After Delete CURRENT_CONSENSUS_ESTIMATES for ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				set @START = GETDATE()
			END			
		--End Modification 2/7/13
		
		
		-- Create the Consensus data first, it will be used later
		if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - Calc_Consensus_Estimates'
		exec Calc_Consensus_Estimates @ISSUER_ID, @VERBOSE
	
		if @VERBOSE = 'Y' 
			BEGIN
				print '*** After Calc_Consensus_Estimates ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END

		--Modified 2/7/13 (JM) to move the deletion of records out of GET_DATA_ANNUAL
		-- Delete the existing data 
		
		begin transaction;					-- Lock the table
		
					--04/01/2014 Delete the data from staging and main table in full mode
					
			delete from PERIOD_FINANCIALS_ISSUER_STAGE		
			where ISSUER_ID = @ISSUER_ID;

			delete PFS from PERIOD_FINANCIALS_SECURITY_STAGE PFS
			INNER JOIN GF_SECURITY_BASEVIEW GSB ON  GSB.SECURITY_ID = PFS.SECURITY_ID
    		where  GSB.ISSUER_ID = @ISSUER_ID
			
		
			delete from PERIOD_FINANCIALS_ISSUER_MAIN
			where ISSUER_ID = @ISSUER_ID;

			delete PFS from PERIOD_FINANCIALS_SECURITY_MAIN PFS
			INNER JOIN GF_SECURITY_BASEVIEW GSB ON  GSB.SECURITY_ID = PFS.SECURITY_ID
    		where  GSB.ISSUER_ID = @ISSUER_ID
    	
		-- where SECURITY_ID in (select distinct SECURITY_ID from GF_SECURITY_BASEVIEW where ISSUER_ID = @ISSUER_ID);

		commit;								-- Unlock the table

		if @VERBOSE = 'Y'
			BEGIN
	--			raiserror( 'After Delete SECURITY_ID', 1, 2) WITH NOWAIT
				print 'After Delete PERIOD_FINANCIALS for ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				set @START = GETDATE()
			END
		--End Modification 2/7/13

		-- Prepare PERIOD_FINANCIALS and write the Annual data into it.
		if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - Get_Data_Annual'
		exec Get_Data_Annual @ISSUER_ID, @VERBOSE,@STAGE
		if @VERBOSE = 'Y' 
			BEGIN
				print '*** After Get_Data_Annual ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END

		-- Write the Quarterly data into PERIOD_FINANCIALS.
		/*if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - Get_Data_Quarterly'
		exec Get_Data_Quarterly @ISSUER_ID, @VERBOSE , @STAGE
		if @VERBOSE = 'Y' 
			BEGIN
				print '*** After Get_Data_Quarterly ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END
		*/
		-- Write the Model data into the PERIOD_FINANCIALS table
		if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - INTERNAL_FISCAL_EXTRACT'
		exec INTERNAL_FISCAL_EXTRACT @ISSUER_ID, @VERBOSE, @STAGE
		if @VERBOSE = 'Y' 
			BEGIN
				print '*** After INTERNAL_FISCAL_EXTRACT ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END
		
		-- Write the Model quarterly data into the PERIOD_FINANCIALS table
		/*if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - INTERNAL_QUARTERLY_EXTRACT'
		exec INTERNAL_QUARTERLY_EXTRACT @ISSUER_ID, @VERBOSE,@STAGE
		if @VERBOSE = 'Y' 
			BEGIN
				print '*** After INTERNAL_QUARTERLY_EXTRACT ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				Set @START = getdate()
			END*/
			
	END
	
	
	
	-- Run all the calculations on the data
	if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - CALCULATIONS'
	exec CALCULATIONS @ISSUER_ID, @CALC_LOG, @VERBOSE, @RUN_MODE, @STAGE
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After CALCULATIONS ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Calendarize the data
/*	print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - Calendarize'
	exec Calendarize @ISSUER_ID
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After Calendarize ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
*/
	-- Update the FAIR_VALUE table
	if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - FAIR_VALUE_UPDATE'
	exec FAIR_VALUE_UPDATE @ISSUER_ID, 'PRIMARY'
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After FAIR_VALUE_UPDATE ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END


	print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - Done'
GO
