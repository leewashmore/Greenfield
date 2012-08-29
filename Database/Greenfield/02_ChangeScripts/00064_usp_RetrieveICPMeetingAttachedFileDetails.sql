--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00063'
declare @CurrentScriptVersion as nvarchar(100) = '00064'
--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO
/****** Object:  StoredProcedure [dbo].[RetrieveICPMeetingAttachedFileDetails]    Script Date: 08/29/2012 14:47:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================
-- Author:		Rahul Vig
-- Create date: 18-08-2012
-- Description:	Retrieve Meeting Attached File Details
-- ===================================================
CREATE PROCEDURE [dbo].[RetrieveICPMeetingAttachedFileDetails] 
	@MeetingId BIGINT = 0
AS
BEGIN
	SET NOCOUNT ON;

    SELECT *
    FROM
    FileMaster _FM
    WHERE _FM.[FileID] IN
    ( SELECT [FileID] FROM MeetingAttachedFileInfo WHERE [MeetingID] = @MeetingId )    
    ORDER BY _FM.[Name]
END

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00064'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
