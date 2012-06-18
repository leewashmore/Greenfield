set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00001'
declare @CurrentScriptVersion as nvarchar(100) = '00002'

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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SetDashBoardPreference] 
	-- Add the parameters for the stored procedure here
	@UserName VARCHAR(50), 
	@GadgetViewClassName VARCHAR(100),
	@GadgetViewModelClassName VARCHAR(100),
	@GadgetName VARCHAR(50),
	@GadgetState VARCHAR(50),
	@PreferenceGroupID VARCHAR(50),
	@GadgetPosition INT
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM tblDashboardPreference WHERE UserName LIKE @UserName AND PreferenceGroupID NOT LIKE @PreferenceGroupID)
		BEGIN
			DELETE FROM tblDashboardPreference WHERE UserName LIKE @UserName
		END
	
	BEGIN
	IF((@GadgetName!='null')and (@GadgetState!='null'))
	
	Begin
		INSERT INTO tblDashboardPreference 
		( UserName, GadgetViewClassName, GadgetViewModelClassName,  GadgetName, GadgetState, GadgetPosition, PreferenceGroupID )
		VALUES 
		( @UserName, @GadgetViewClassName, @GadgetViewModelClassName, @GadgetName, @GadgetState, @GadgetPosition, @PreferenceGroupID )			
	end
		
	END
END

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






