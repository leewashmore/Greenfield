set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00075'
declare @CurrentScriptVersion as nvarchar(100) = '00076'
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

/****** Object:  StoredProcedure [dbo].[SetPresentationInfo]    Script Date: 08/29/2012 14:48:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Mansi Gupta>
-- Create date: <08/30/2012>
-- =============================================
CREATE PROCEDURE [dbo].[GetDocumentsData]
	-- Add the parameters for the stored procedure here
	@searchString nvarchar(MAX)
AS
DECLARE @fileID bigint;
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;

    -- Insert statements for procedure here
	Select DISTINCT * 
	INTO #fileMaster
	from FileMaster 
	Where MetaTags like '%'+@searchString+'%' or
	SecurityName like '%'+@searchString+'%' or
	SecurityTicker like '%'+@searchString+'%';
	
	Select FileID,Comment,CommentBy,CommentOn 
	INTO #commentInfo
	from CommentInfo 
	Where FileID IN (Select FileID from #fileMaster);
	
	Select f.*,c.Comment,c.CommentBy,c.CommentOn 
	from #fileMaster f
	left join 
	#commentInfo c 
	ON f.FileID = c.FileID;
	
	Drop table #commentInfo,#fileMaster;
END

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00076'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

