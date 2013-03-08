set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00298'
declare @CurrentScriptVersion as nvarchar(100) = '00299'

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
            ------------------------------------------------------------------------
-- Purpose:	This procedure is to run all the calculations for Issuers
--			and securities.  There is a specific order for the 
--			calculations to be run in which is coded sequencially into 
--			this stored procedure.
--
-- Author:	David Muench
-- Date:	July 12, 2012
------------------------------------------------------------------------
alter procedure CALCULATIONS (
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@VERBOSE			char		= 'Y'			-- Print messages
)
as

	-- create smoe variables for looping through the calculations
	declare @CALC_NUM	varchar(3)		-- Calculation number
	declare @CALC_SEQ	integer			-- The sequence to execute the calculation in
	declare @CALC_NAME	varchar(100)	-- The name of the calculation
	declare @LAST_SEQ	integer			-- the last sequence number run
	declare @SQL		varchar(200)	-- The SQL to be run
	declare @START		datetime		-- the time the calc starts
	
	set @LAST_SEQ = 0
	
	-- Create a cursor to control the calculations loop
/*	declare calc1 cursor 
		for select CALC_SEQ, CALC_NUM, CALC_NAME
			  from dbo.CALC_LIST with(NOLOCK)
			 where 1=1 
			 order by CALC_SEQ, CALC_NUM;
*/

		-- Create a temporary table to replace the cursor previously used.
		select CALC_SEQ, CALC_NUM, CALC_NAME
		  into #CALC_LIST
		  from dbo.CALC_LIST with(NOLOCK)
		 where 1=1 
		 order by CALC_SEQ, CALC_NUM;	
		 create index CALC_LIST_TEMP_idx on #CALC_LIST(CALC_SEQ, CALC_NUM)		 
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
			
			exec @SQL @ISSUER_ID, @CALC_LOG


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



GO



-- exec CALCULATIONS 128251
-- truncate table CALC_LOG
-- select * from CALC_LOG where LOG_DATE > '07/13/2012'

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00299'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())