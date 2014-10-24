USE [AIMS_Config]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveICPack]    Script Date: 10/20/2014 15:41:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RetrieveICPack]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RetrieveICPack]
GO

USE [AIMS_Config]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveICPack]    Script Date: 10/20/2014 15:41:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO
Create procedure [dbo].[RetrieveICPack]
	
AS
BEGIN
	SET NOCOUNT ON;

    SELECT FM.FileID, FM.Name, P.SecurityName,P.SecurityTicker, FM.Location
    FROM
    FileMaster FM
    INNER JOIN  dbo.PresentationAttachedFileInfo Pa ON Pa.FileId = Fm.FileId
    Inner Join dbo.Presentationinfo p on p.presentationid = pa.presentationid
    where p.statustype = 'Ready for Voting'
    and FM.Category='Investment Committee Packet'
    and FM.Type = 'IC Presentations'
   
END
GO


-- exec [dbo].[RetrieveICPack]