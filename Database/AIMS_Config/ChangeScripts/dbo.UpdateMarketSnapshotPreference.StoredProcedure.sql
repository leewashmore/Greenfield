SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[UpdateMarketSnapshotPreference] 
	-- Add the parameters for the stored procedure here
	  @userId NVARCHAR(100),
	  @snapshotname NVARCHAR(max),
	  @snapshotpreferenceid INT	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update tblMarketSnapshotPreference
	SET SnapshotName = @snapshotname		
	where UserId = @userId AND SnapshotPreferenceId = @snapshotpreferenceid
	Select @@ROWCOUNT
	
END
GO
