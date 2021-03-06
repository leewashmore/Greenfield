SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[GetCOMMODITY_FORECASTS] (@CommodityID varchar(50) )
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF( @CommodityID = 'ALL')
    SELECT * from dbo.COMMODITY_FORECASTS
    ELSE
	SELECT * from dbo.COMMODITY_FORECASTS where COMMODITY_ID = @CommodityID
END
GO
