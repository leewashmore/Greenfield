set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00077'
declare @CurrentScriptVersion as nvarchar(100) = '00078'

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

IF OBJECT_ID ('[dbo].[GetCustomScreeningUserPreferences]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetCustomScreeningUserPreferences]
GO

CREATE PROCEDURE [dbo].[GetCustomScreeningUserPreferences] 
	-- the parameters for the stored procedure here
	@username nvarchar(50)
AS
SET FMTONLY OFF;	   
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
   SELECT l.UserName,
		  l.ListId,
		  l.ListName,
		  l.Accessibilty,
		  d.ScreeningId,
		  d.DataDescription,
		  d.TABLE_COLUMN as TableColumn,
		  d.DataSource,
		  d.PeriodType,
		  d.YearType,
		  d.FromDate,
		  d.ToDate,
		  d.DataPointsOrder
   FROM
	(SELECT * 
	FROM UserCustomisedListInfo 
	WHERE UserName = @username
	or Accessibilty = 'public')l
	
	left join
	
	(SELECT aa.*,bb.TABLE_COLUMN FROM
	(Select * FROM UserListDataPointMappingInfo
	WHERE ListId IN (SELECT ListId FROM UserCustomisedListInfo 
	WHERE UserName = @username
	or Accessibilty = 'public'))aa inner join 
	(Select SCREENING_ID,TABLE_COLUMN from dbo.SCREENING_DISPLAY_REFERENCE 
	UNION Select SCREENING_ID,TABLE_COLUMN from dbo.SCREENING_DISPLAY_FAIRVALUE
	UNION (SELECT Screening_ID,DATA_DESC AS TABLE_COLUMN FROM SCREENING_DISPLAY_PERIOD sdp
	INNER JOIN DATA_MASTER dm ON sdp.DATA_ID = dm.DATA_ID)
	UNION  (SELECT Screening_ID,DATA_DESC AS TABLE_COLUMN FROM SCREENING_DISPLAY_CURRENT sdc
	INNER JOIN DATA_MASTER dm ON sdc.DATA_ID = dm.DATA_ID)) bb
	on aa.SCREENINGID  = bb.SCREENING_ID)d	
	ON l.ListId = d.ListId
	ORDER BY l.ListId,d.DataPointsOrder;
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00078'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


