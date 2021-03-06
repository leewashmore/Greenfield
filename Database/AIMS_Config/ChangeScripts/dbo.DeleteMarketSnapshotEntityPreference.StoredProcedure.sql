SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[DeleteMarketSnapshotEntityPreference] 
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
