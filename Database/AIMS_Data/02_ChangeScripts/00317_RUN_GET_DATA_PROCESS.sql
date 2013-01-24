set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00316'
declare @CurrentScriptVersion as nvarchar(100) = '00317'

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
            IF OBJECT_ID ( 'RUN_GET_DATA_PROCESS', 'P' ) IS NOT NULL 
DROP PROCEDURE RUN_GET_DATA_PROCESS;
GO
------------------------------------------------------------------------
-- Purpose:	This procedure runs the GET_DATA_PROCESS using a list of
--			ISSUER_IDs in a table.
--
-- Author:	David Muench
-- Date:	Oct 18, 2012
------------------------------------------------------------------------
create procedure RUN_GET_DATA_PROCESS (
	@CALC_LOG		char	= 'N'
,	@VERBOSE		char	= 'N'
)
as

	-- Declare the parameter variables for the GET_DATA_PROCESS
	declare @RUN_ID integer
	declare @RUN integer
	
	-- Initialize the process
	exec Get_Data_Process 'START', NULL, NULL, NULL, NULL, @RUN_ID OUTPUT;
	print 'Use this RUN_ID='+isnull(cast( @RUN_ID as varchar(10)), 'NULL');
	set @RUN = @RUN_ID;

	-- Insert the ISSUER_ID values into the list to be run.
	insert into GET_DATA_ISSUER_LIST (RUN_ID, ISSUER_ID, START_TIME, END_TIME, STATUS_TXT, PROCESS_ID)
	select distinct @RUN, issuer_id, GETDATE(), null, 'Ready', 0 
	  from NIGHTLY_ISSUER_LIST;

	-- Start the process running.
	exec Get_Data_Process 'RUN', @RUN, NULL, @CALC_LOG, @VERBOSE, @RUN_ID;

go

-- exec RUN_GET_DATA_PROCESS
-- insert into NIGHTLY_ISSUER_LIST values('223340')
-- select * from NIGHTLY_ISSUER_LIST 
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00317'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())