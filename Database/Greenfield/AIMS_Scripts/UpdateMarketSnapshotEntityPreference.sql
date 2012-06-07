
/****** Object:  StoredProcedure [dbo].[UpdateMarketSnapshotEntityPreference]    Script Date: 06/05/2012 11:00:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


Alter PROCEDURE [dbo].[UpdateMarketSnapshotEntityPreference] 
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

