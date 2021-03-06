SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[SetMarketSnapshotEntityPreference] 
	-- Add the parameters for the stored procedure here
	  @grouppreferenceid int,
	  @entityName nvarchar(max),
	  @entityReturnType nvarchar(50),
	  @entityType nvarchar(50),
	  @entityOrder int
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
    -- Insert statements for procedure here
	Insert into tblMarketSnapshotEntityPreference(GroupPreferenceId,EntityName,EntityReturnType,EntityType,EntityOrder)
	
	Values (@grouppreferenceid,@entityName,@entityReturnType,@entityType,@entityOrder)
	
	Select SCOPE_IDENTITY()
	
END
GO
