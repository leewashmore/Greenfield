SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetBasicData] (
@SecurityID varchar(20)
 )	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
declare @MarketCapitalization decimal 
declare @EnterpriseValue decimal


    -- Insert statements for procedure here
	SELECT @MarketCapitalization = AMOUNT 
	from dbo.PERIOD_FINANCIALS_SECURITY
	where
	SECURITY_ID = @SecurityID and 
	DATA_ID = 185  and
	CURRENCY = 'USD' and
	PERIOD_TYPE = 'C'
	
	SELECT  @EnterpriseValue = AMOUNT 
	from dbo.PERIOD_FINANCIALS_SECURITY 
	where
	SECURITY_ID = @SecurityID and 
	DATA_ID = 186  and
	CURRENCY = 'USD' and
	PERIOD_TYPE = 'C'
	
	SELECT  @MarketCapitalization as MARKET_CAPITALIZATION ,@EnterpriseValue as ENTERPRISE_VALUE
END
GO
