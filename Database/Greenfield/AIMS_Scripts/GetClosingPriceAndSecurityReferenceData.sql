USE [AshmoreEMMPOC]
GO

/****** Object:  StoredProcedure [dbo].[GetClosingPriceSecurityReferenceData]    Script Date: 06/05/2012 10:34:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Akshay Mathur
-- Create date: 
-- Description:	To get the value of Closing Price
-- =============================================
CREATE PROCEDURE [dbo].[GetClosingPriceSecurityReferenceData] 
	-- Add the parameters for the stored procedure here
	@XRef int = 0, 
	@PeriodType varchar(100),
	@EstimateType varchar(100)  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from tblActual where XRef=@XRef and PeriodType=@PeriodType and EstimateType=@EstimateType
END

GO

