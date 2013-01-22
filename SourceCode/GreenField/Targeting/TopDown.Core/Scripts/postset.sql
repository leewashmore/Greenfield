--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00155'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())