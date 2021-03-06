SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[SetMarketSnapshotPreference] 
	-- Add the parameters for the stored procedure here
	  @userId nVARCHAR(100),
	  @snapshotname NVARCHAR(max)
AS
DECLARE @OrderCount int 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Insert into tblMarketSnapshotPreference (UserId,SnapshotName)	
	values (@userId , @snapshotname)
	
	Select SCOPE_IDENTITY()
	
END
GO
