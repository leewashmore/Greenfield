--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00003'
declare @CurrentScriptVersion as nvarchar(100) = '00004'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
	return

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	return
end

GO
--PUT YOUR CODE HERE:




ALTER PROCEDURE [dbo].[DeleteMarketSnapshotEntityPreference] 
	-- Add the parameters for the stored procedure here
	  @entitypreferenceid int
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from tblMarketSnapshotEntityPreference
	where EntityPreferenceId = @entitypreferenceid
	
	Select @@ROWCOUNT
    
END

GO






--END OF YOUR CODE.


--indicate thet current script is executed

if @@error = 0
begin
	declare @CurrentScriptVersion as nvarchar(100) = '00004'
	insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
end


