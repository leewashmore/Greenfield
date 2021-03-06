SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[UpdateMarketSnapshotEntityPreference] 
	-- Add the parameters for the stored procedure here
	  @grouppreferenceid int,
	  @entitypreferenceid int,
	  @entityOrder int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update tblMarketSnapshotEntityPreference
	SET GroupPreferenceId = @grouppreferenceid,
		EntityOrder = @entityOrder		
	where EntityPreferenceId = @entitypreferenceid
	Select @@ROWCOUNT
	
END
GO
