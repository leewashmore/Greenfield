SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[SetDashBoardPreference] 
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
