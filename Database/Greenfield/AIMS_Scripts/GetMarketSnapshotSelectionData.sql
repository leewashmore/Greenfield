USE [AshmoreEMMPOC]
GO

/****** Object:  StoredProcedure [dbo].[GetMarketSnapshotSelectionData]    Script Date: 06/05/2012 10:58:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[GetMarketSnapshotSelectionData] 
	-- Add the parameters for the stored procedure here
	  @userId nVARCHAR(100)	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select SnapshotPreferenceId,SnapshotName from tblMarketSnapshotPreference
	WHERE UserId = @userId	
END

GO

