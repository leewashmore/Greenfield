set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00064'
declare @CurrentScriptVersion as nvarchar(100) = '00065'

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
CREATE PROCEDURE [dbo].[GetFinancialTabDataDescriptions]
	@tabName nvarchar(10)
AS
SET FMTONLY OFF
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

 if @tabName = 'Period'
 begin
	Select a.SCREENING_ID, b.DATA_DESC, b.QUARTERLY, b.ANNUAL,a.DATA_ID,a.ESTIMATE_ID
	INTO #periodTempTable
	from SCREENING_DISPLAY_PERIOD a 
	left join 
	DATA_MASTER b  
	on a.DATA_ID = b.DATA_ID
	order by data_desc;
	
	Select * from #periodTempTable;
	Drop table #periodTempTable;
end

else if @tabName = 'Current'
begin
	Select a.SCREENING_ID, b.DATA_DESC, null AS QUARTERLY,null AS ANNUAL,a.DATA_ID, a.ESTIMATE_ID
	INTO #currentTempTable
	from SCREENING_DISPLAY_CURRENT a 
	left join 
	DATA_MASTER b  
	on a.DATA_ID = b.DATA_ID
	order by data_desc;
	
	Select * from #currentTempTable;
	Drop table #currentTempTable;
end

END

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00065'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




