set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00314'
declare @CurrentScriptVersion as nvarchar(100) = '00315'

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
            IF OBJECT_ID ( 'Get_Data', 'P' ) IS NOT NULL 
DROP PROCEDURE Get_Data;
GO-----------------------------------------------------------------------------------
-- Purpose:	To move the data from the Reuters raw tables into the display tables:
--			PERIOD_FINANCIALS.  This procedure works as an interface to load the 
--			Reuters data received in file format into the AIMS database.
--
-- Author:	David Muench
-- Date:	July 2, 2012
-----------------------------------------------------------------------------------

create procedure Get_Data(
	@ISSUER_ID			varchar(20) = NULL					-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
,	@VERBOSE			char		= 'Y'
)
as
	declare @START		datetime		-- the time the calc starts
	Set @START = getdate()

	-- Create the Consensus data first, it will be used later
	if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - Calc_Consensus_Estimates'
	exec Calc_Consensus_Estimates @ISSUER_ID, @VERBOSE
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After Calc_Consensus_Estimates ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Prepare PERIOD_FINANCIALS and write the Annual data into it.
	if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - Get_Data_Annual'
	exec Get_Data_Annual @ISSUER_ID, @VERBOSE
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After Get_Data_Annual ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END

	-- Write the Quarterly data into PERIOD_FINANCIALS.
	if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - Get_Data_Quarterly'
	exec Get_Data_Quarterly @ISSUER_ID, @VERBOSE
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After Get_Data_Quarterly ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
	
	-- Write the Model data into the PERIOD_FINANCIALS table
	if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - INTERNAL_FISCAL_EXTRACT'
	exec INTERNAL_FISCAL_EXTRACT @ISSUER_ID, @VERBOSE
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After INTERNAL_FISCAL_EXTRACT ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
	
	-- Write the Model quarterly data into the PERIOD_FINANCIALS table
	if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - INTERNAL_QUARTERLY_EXTRACT'
	exec INTERNAL_QUARTERLY_EXTRACT @ISSUER_ID, @VERBOSE
	if @VERBOSE = 'Y' 
		BEGIN
			print '*** After INTERNAL_QUARTERLY_EXTRACT ' + @ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
			Set @START = getdate()
		END
	
	-- Run all the calculations on the data
	if @VERBOSE = 'Y' print '** ' + CONVERT(varchar(40), getdate(), 121) + ' - CALCULATIONS'
	exec CALCULATIONS @ISSUER_ID, @CALC_LOG, @VERBOSE
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
go

-- exec Get_Data 187318
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00315'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())