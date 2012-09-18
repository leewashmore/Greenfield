set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00062'
declare @CurrentScriptVersion as nvarchar(100) = '00063'

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
	
	(SELECT * 
	FROM UserListDataPointMappingInfo
	WHERE ListId IN (SELECT ListId FROM UserCustomisedListInfo 
	WHERE UserName = @username
	or Accessibilty = 'public'))d	
	ON l.ListId = d.ListId
	ORDER BY l.ListId,d.DataPointsOrder;
END

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00063'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




