set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00102'
declare @CurrentScriptVersion as nvarchar(100) = '00103'
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

ALTER TABLE [PresentationInfo]
DROP COLUMN SecurityMSCIStdWeight, SecurityMSCIIMIWeight, SecurityGlobalActiveWeight
GO	

ALTER TABLE [PresentationInfo]
ADD SecurityPFVMeasureValue DECIMAL NULL, CommitteePFVMeasureValue DECIMAL NULL

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00103'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
