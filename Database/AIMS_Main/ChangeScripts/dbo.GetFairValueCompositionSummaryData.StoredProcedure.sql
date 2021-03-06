SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetFairValueCompositionSummaryData]
	@SECURITY_ID varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT b.VALUE_TYPE as SOURCE
    ,a.DATA_DESC as MEASURE
    ,b.FV_BUY as BUY
    ,b.FV_SELL as SELL
    ,b.UPSIDE as UPSIDE
    ,b.UPDATED as DATE
    ,a.DATA_ID as DATA_ID
    from DATA_MASTER a
	inner join  FAIR_VALUE b 
	ON
	a.DATA_ID = b.FV_MEASURE 
	WHERE SECURITY_ID=@SECURITY_ID
END
GO
