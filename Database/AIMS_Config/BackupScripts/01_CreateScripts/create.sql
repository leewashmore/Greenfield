CREATE USER [WPSuperUser] FOR LOGIN [WPSuperUser] WITH DEFAULT_SCHEMA=[dbo]
GO

exec sp_addrolemember @rolename = 'db_owner' , @membername = 'WPSuperUser' 
GO



IF OBJECT_ID ('dbo.tblDashboardPreference') IS NOT NULL
	DROP TABLE dbo.tblDashboardPreference
GO

CREATE TABLE dbo.tblDashboardPreference
	(
	PreferenceID             INT IDENTITY NOT NULL,
	UserName                 VARCHAR (50) NOT NULL,
	GadgetViewClassName      VARCHAR (100) NOT NULL,
	GadgetViewModelClassName VARCHAR (100) NOT NULL,
	GadgetName               VARCHAR (50) NOT NULL,
	GadgetState              VARCHAR (50) NOT NULL,
	GadgetPosition           INT NOT NULL,
	PreferenceGroupID        VARCHAR (50) NOT NULL,
	CONSTRAINT PK_tblDashboardPreference PRIMARY KEY (PreferenceID)
	)
GO


-- =============================================
-- Author:		Rahul Vig
-- Create date: 22-02-2012
-- Description:	Retrieve User DashBoard Preference
-- =============================================
CREATE PROCEDURE GetDashBoardPreferenceByUserName 
	-- Add the parameters for the stored procedure here
	@UserName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM tblDashboardPreference WHERE UserName = @UserName
END

GO


-- =============================================
-- Author:		Rahul Vig
-- Create date: 22-02-2012
-- Description:	Update DashBoard Preference
-- =============================================
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
		INSERT INTO tblDashboardPreference 
		( UserName, GadgetViewClassName, GadgetViewModelClassName,  GadgetName, GadgetState, GadgetPosition, PreferenceGroupID )
		VALUES 
		( @UserName, @GadgetViewClassName, @GadgetViewModelClassName, @GadgetName, @GadgetState, @GadgetPosition, @PreferenceGroupID )			
	END
END

GO

