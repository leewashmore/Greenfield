SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[SetMarketSnapshotGroupPreference] 
	-- Add the parameters for the stored procedure here
	  @snapshotpreferenceId int,
	  @groupname NVARCHAR(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    -- Insert statements for procedure here
	Insert into tblMarketSnapshotGroupPreference(GroupName,SnapshotPreferenceId)	
	values (@groupname,@snapshotpreferenceId)
	
	Select SCOPE_IDENTITY()
	
END
GO
