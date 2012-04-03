--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00000'
declare @CurrentScriptVersion as nvarchar(100) = '00001'

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


--PUT YOUR CODE HERE:


--END OF YOUR CODE.


--indicate thet current script is executed
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



