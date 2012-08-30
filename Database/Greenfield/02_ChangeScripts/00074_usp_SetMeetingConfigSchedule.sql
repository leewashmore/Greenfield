--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00073'
declare @CurrentScriptVersion as nvarchar(100) = '00074'
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


/****** Object:  StoredProcedure [dbo].[SetMeetingConfigSchedule]    Script Date: 08/29/2012 14:55:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sneha Sharma
-- Create date: 14-08-2012
-- Description:	Updates the System Configuration
-- =============================================
CREATE PROCEDURE [dbo].[SetMeetingConfigSchedule]
	-- Add the parameters for the stored procedure here
	
	@presentationDateTime datetime,
    @presentationTimeZone varchar(50),
    @presentationDeadline datetime,
    @preMeetingVotingDeadline datetime,
    @userName varchar(50)
    
AS
BEGIN		
	
	UPDATE MeetingConfigurationSchedule   
	SET [PresentationDateTime] = @presentationDateTime,
       [PresentationTimeZone] = @presentationTimeZone,
       [PresentationDeadline] = @presentationDeadline,
       [PreMeetingVotingDeadline] = @preMeetingVotingDeadline,
       [ModifiedBy] = @userName,
       [ModifiedOn] = GETUTCDATE()
 	
 	if @@ERROR <> 0
 	BEGIN 
 	 SELECT -1
 	END
 	
 	SELECT 0
END

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00074'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())