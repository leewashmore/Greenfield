SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[DeleteFileMaster] 
	-- Add the parameters for the stored procedure here
	   @fileId BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from FileMaster where FileID = @fileId	
	
END
GO
