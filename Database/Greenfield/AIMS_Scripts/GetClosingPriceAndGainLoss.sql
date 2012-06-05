USE [AshmoreEMMPOC]
GO

/****** Object:  StoredProcedure [dbo].[GetClosingPriceAndGainLoss]    Script Date: 06/05/2012 10:33:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetClosingPriceAndGainLoss] 
	-- Add the parameters for the stored procedure here
	@Type nvarchar(50)
  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from tblDemoClosingPandGain where Type=@Type 
END


GO

