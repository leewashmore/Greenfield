SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
------------------------------------------------------------------------
-- Purpose:	This procedure is to run all the calculations for Issuers
--			and securities.  There is a specific order for the 
--			calculations to be run in which is coded sequencially into 
--			this stored procedure.
--
-- Author:	David Muench
-- Date:	July 12, 2012
------------------------------------------------------------------------
alter procedure [dbo].[CALCULATIONS] (
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@VERBOSE			char		= 'Y'			-- Print messages
,	@RUN_MODE			char		= 'F'
,	@STAGE				char		= 'N'
)
as

	
	-- create smoe variables for looping through the calculations
	declare @CALC_NUM	varchar(3)		-- Calculation number
	declare @CALC_SEQ	integer			-- The sequence to execute the calculation in
	declare @CALC_NAME	varchar(100)	-- The name of the calculation
	declare @LAST_SEQ	integer			-- the last sequence number run
	declare @SQL		varchar(200)	-- The SQL to be run
	declare @START		datetime		-- the time the calc starts
	
	set @START = GETDATE()
	set @LAST_SEQ = 0
	
	--For Incremental runs, need to delete price-based calculation and price based preliminary calculations (price and market cap) 
	if @RUN_MODE = 'I'
	Begin
		
		delete from PERIOD_FINANCIALS_SECURITY_MAIN
		where SECURITY_ID in (select distinct SECURITY_ID from GF_SECURITY_BASEVIEW where ISSUER_ID = @ISSUER_ID)
			and (data_id in( select CALC_NUM from CALC_LIST where ACTIVE = 'Y' and PRICEBASED = 'Y')
			or DATA_ID in (191,185,218));  -- 191, 185 and 218 are price, market cap and shares outstanding respectively but part of calculation 0 so must be deleted until that is de-coupled
			
		delete from PERIOD_FINANCIALS_ISSUER_MAIN
		where ISSUER_ID = @ISSUER_ID
			and data_id in( select CALC_NUM from CALC_LIST where ACTIVE = 'Y' and PRICEBASED = 'Y'); 
		
		
		if @VERBOSE = 'Y' 
			BEGIN
				print '*** After Delete Calculation Records on Incremental Run for ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
				set @START = GETDATE();
			END
	End
	
	
	-- Create a cursor to control the calculations loop
/*	declare calc1 cursor 
		for select CALC_SEQ, CALC_NUM, CALC_NAME
			  from dbo.CALC_LIST with(NOLOCK)
			 where 1=1 
			 order by CALC_SEQ, CALC_NUM;
*/

	--Compile calc_list based on run mode and COA Type of the Issuer
	
	select ii.ISSUER_ID, ii.COA_TYPE
	  into #COA_TYPE
	  from dbo.INTERNAL_ISSUER ii
	 where ii.ISSUER_ID = @ISSUER_ID

	select CALC_SEQ, CALC_NUM, CALC_NAME
	into #CALC_LIST
		from dbo.CALC_LIST cl with(NOLOCK)
		inner join dbo.DATA_MASTER dm on cl.CALC_NUM = dm.DATA_ID
		inner join #COA_TYPE ct on (ct.COA_TYPE = 'BNK' and dm.BANK = 'Y')
							 or (ct.COA_TYPE = 'IND' and dm.INDUSTRIAL = 'Y')
							 or (ct.COA_TYPE = 'FIN' and dm.INSURANCE = 'Y')
							 or (ct.COA_TYPE = 'UTL' and dm.UTILITY = 'Y')
	where cl.ACTIVE = 'Y' and cl.PRICEBASED = 'Y' 
	union
	select CALC_SEQ, CALC_NUM, CALC_NAME
		from dbo.CALC_LIST cl with(NOLOCK)
		inner join dbo.DATA_MASTER dm on cl.CALC_NUM = dm.DATA_ID
		inner join #COA_TYPE ct on (ct.COA_TYPE = 'BNK' and dm.BANK = 'Y')
							 or (ct.COA_TYPE = 'IND' and dm.INDUSTRIAL = 'Y')
							 or (ct.COA_TYPE = 'FIN' and dm.INSURANCE = 'Y')
							 or (ct.COA_TYPE = 'UTL' and dm.UTILITY = 'Y')
	where ACTIVE = 'Y' and PRICEBASED = 'N' 
	and @RUN_MODE ='F'
	order by CALC_SEQ, CALC_NUM;	

	create index CALC_LIST_TEMP_idx on #CALC_LIST(CALC_SEQ, CALC_NUM)		
	
	drop table #COA_TYPE
	
	-- Get the first set of data from the cursor
--	open calc1;
--	fetch next from calc1
--	 into @CALC_SEQ, @CALC_NUM, @CALC_NAME

			-- Get the first ISSUER_ID from the list
			select top 1 @CALC_SEQ = CALC_SEQ, @CALC_NUM = CALC_NUM, @CALC_NAME = CALC_NAME
			  from #CALC_LIST
			 order by CALC_SEQ, CALC_NUM
			if @VERBOSE = 'Y' 
				BEGIN
					print CONVERT(varchar(40), getdate(), 121) + ' - First select from #CALC_LIST ' + @CALC_NUM + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000);
					set @START = GETDATE();
				END

			-- Delete the selected ISSUER_ID so that it does not get run again
			delete from #CALC_LIST 
			 where CALC_SEQ = @CALC_SEQ
			   and CALC_NUM = @CALC_NUM
			if @VERBOSE = 'Y' 
				BEGIN
					print CONVERT(varchar(40), getdate(), 121) + ' - First delete from #CALC_LIST ' + @CALC_NUM + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000);
					set @START = GETDATE();
				END

	-- Loop through the data checking each row
	while isnull(@CALC_NUM, -1) >= 0	--@@FETCH_STATUS = 0
		BEGIN
			set @START = GETDATE();
			if( @CALC_SEQ <> @LAST_SEQ)
				BEGIN
					if @VERBOSE = 'Y' print CONVERT(varchar(40), getdate(), 121) + ' - Sequence # ' + cast(@CALC_SEQ as varchar(3))
					set @LAST_SEQ = @CALC_SEQ
				END
			
			-- Execute the calculation
--			print CONVERT(varchar(40), getdate(), 121) + ' - ' + @CALC_NUM + ' - ' + @CALC_NAME
			select @SQL = 'AIMS_CALC_' + @CALC_NUM		-- Construct the correct procedure name
--			print 'EXEC ' + @SQL + ' ''' + @ISSUER_ID + ''''
			
			exec @SQL @ISSUER_ID, @CALC_LOG, @STAGE


			if @VERBOSE = 'Y' 
				BEGIN
					print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + @CALC_NUM + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000);
					set @START = GETDATE();
				END
			-- Get the next row of data
--			fetch next from @CURSOR_NAME
--			 into @CALC_SEQ, @CALC_NUM, @CALC_NAME
			set @CALC_NUM = -1				-- set to stop in case nothing selected
			select top 1 @CALC_SEQ = CALC_SEQ, @CALC_NUM = CALC_NUM, @CALC_NAME = CALC_NAME
			  from #CALC_LIST
			 order by CALC_SEQ, CALC_NUM

			if @VERBOSE = 'Y' 
				BEGIN
					print CONVERT(varchar(40), getdate(), 121) + ' - Loop select from #CALC_LIST ' + @CALC_NUM + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000);
					set @START = GETDATE();
				END
			-- Delete the selected ISSUER_ID so that it does not get run again
			delete from #CALC_LIST 
			 where CALC_SEQ = @CALC_SEQ
			   and CALC_NUM = @CALC_NUM
			if @VERBOSE = 'Y' 
				BEGIN
					print CONVERT(varchar(40), getdate(), 121) + ' - Loop delete from #CALC_LIST ' + @CALC_NUM + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000);
					set @START = GETDATE();
				END

		END;


	-- Clean up the cursor
--	close calc1;
--	deallocate calc1;

	drop table #CALC_LIST
	/*  Removed 03/5/2013 (JM) to no longer calculate consensus calculations, using the valuations in PERIOD_FINANCIALS where DATA_SOURCE = 'REUTERS' and ROOT_SOURCE = 'CONSENSUS'

	-- Run the seven calculations for the Consensus data
	if @VERBOSE = 'Y' print 'Run the calculations for the Consensus data'
	/*
	exec CCE_Calc_164 @ISSUER_ID, @CALC_LOG
	print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + '164' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	exec CCE_Calc_166 @ISSUER_ID, @CALC_LOG
	print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + '166' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	exec CCE_Calc_170 @ISSUER_ID, @CALC_LOG
	print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + '170' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	exec CCE_Calc_171 @ISSUER_ID, @CALC_LOG
	print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + '171' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	exec CCE_Calc_177 @ISSUER_ID, @CALC_LOG
	print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + '177' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	exec CCE_Calc_172 @ISSUER_ID, @CALC_LOG		-- 172 must come after 177
	print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + '172' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	exec CCE_Calc_192 @ISSUER_ID, @CALC_LOG
	print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + '192' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	*/	
	-- The seven CCE calculations have been pulled together into one stored procedure
	-- This single procedure executes more quickly then the individual procedures.
	exec CCE_Calc_ALL @ISSUER_ID, @CALC_LOG
	if @VERBOSE = 'Y' print CONVERT(varchar(40), getdate(), 121) + ' - DONE with calculation ' + 'CCE_ALL' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	*/
GO
