USE [AshmoreEMMPOC]
GO

/****** Object:  StoredProcedure [dbo].[DeleteMarketSnapshotGroupPreference]    Script Date: 06/05/2012 10:32:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE PROCEDURE [dbo].[DeleteMarketSnapshotGroupPreference] 
	-- Add the parameters for the stored procedure here
	  @grouppreferenceid int
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from tblMarketSnapshotGroupPreference
	where GroupPreferenceId = @grouppreferenceid
	
	Select @@ROWCOUNT
    
END


GO

