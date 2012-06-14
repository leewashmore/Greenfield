
set noexec off


--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00006'
declare @CurrentScriptVersion as nvarchar(100) = '00007'

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
--PUT YOUR CODE HERE:


Alter PROCEDURE [dbo].[GetMarketSnapshotPreference] 
	-- Add the parameters for the stored procedure here
	  @SnapshotPreferenceId INT	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select 
	MSGP.GroupName,
	MSGP.GroupPreferenceID,
    MSEP.EntityPreferenceId,
    MSEP.EntityName,
    MSEP.EntityOrder,
    MSEP.EntityReturnType,
    MSEP.EntityType
    
	FROM 
	tblMarketSnapshotGroupPreference MSGP
	LEFT OUTER JOIN tblMarketSnapshotEntityPreference MSEP
	ON MSGP.GroupPreferenceID = MSEP.GroupPreferenceID
  
    WHERE 
    MSGP.SnapshotPreferenceId = 
		(SELECT SnapshotPreferenceId FROM tblMarketSnapshotPreference
			WHERE SnapshotPreferenceId = @SnapshotPreferenceId)
 
END

GO





--END OF YOUR CODE.


--indicate thet current script is executed

if @@error = 0
begin
	declare @CurrentScriptVersion as nvarchar(100) = '00007'
	insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
end


