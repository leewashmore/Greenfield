
/****** Object:  StoredProcedure [dbo].[GetMarketSnapshotPreference]    Script Date: 06/05/2012 10:58:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



Alter PROCEDURE [dbo].[GetMarketSnapshotPreference] 
	-- Add the parameters for the stored procedure here
	  @SnapshotPreferenceId INT	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select 
	MSGP.GroupName,
	MSGP.GroupPreferenceID,
    MSEP.EntityPreferenceId,
    MSEP.EntityName,
    MSEP.EntityOrder,
    MSEP.EntityReturnType,
    MSEP.EntityType
    
	FROM 
	tblMarketSnapshotGroupPreference MSGP
	LEFT OUTER JOIN tblMarketSnapshotEntityPreference MSEP
	ON MSGP.GroupPreferenceID = MSEP.GroupPreferenceID
  
    WHERE 
    MSGP.SnapshotPreferenceId = 
		(SELECT SnapshotPreferenceId FROM tblMarketSnapshotPreference
			WHERE SnapshotPreferenceId = @SnapshotPreferenceId)
 
END

GO

