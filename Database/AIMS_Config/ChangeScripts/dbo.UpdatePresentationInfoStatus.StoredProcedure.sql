USE [AIMS_Config]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePresentationInfoStatus]    Script Date: 11/18/2014 16:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[UpdatePresentationInfoStatus]
(@FromStatus varchar(20),
@ToStatus varchar(20))
	
AS
BEGIN
	SET NOCOUNT ON;

/*    SELECT FM.FileID, FM.Name, P.SecurityName,P.SecurityTicker, FM.Location
    FROM
    FileMaster FM
    INNER JOIN  dbo.PresentationAttachedFileInfo Pa ON Pa.FileId = Fm.FileId
    Inner Join dbo.Presentationinfo p on p.presentationid = pa.presentationid
    where p.statustype = 'Ready for Voting'
    and FM.Category='Investment Committee Packet'
    and FM.Type = 'IC Presentations'*/
    -- update the time the status was changed
    update meetinginfo set meetingdatetime = getdate(), meetingcloseddatetime = getdate(),
    meetingvotingclosedDateTime = getdate() where meetingid in (
    select meetingid from meetingpresentationmappinginfo where presentationid in (
    select presentationid from dbo.presentationInfo where statustype = @FromStatus)
    )
    
    update dbo.Presentationinfo set statustype = @ToStatus
    where presentationId in (select presentationid from dbo.presentationInfo where statustype = @FromStatus)
    
   
END
