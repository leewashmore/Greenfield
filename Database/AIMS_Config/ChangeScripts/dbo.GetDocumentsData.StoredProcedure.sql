SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetDocumentsData]
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
	IssuerName like '%'+@searchString+'%' or
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
