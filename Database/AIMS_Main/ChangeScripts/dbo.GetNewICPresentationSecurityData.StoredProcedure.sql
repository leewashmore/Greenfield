SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[GetNewICPresentationSecurityData]
	-- Add the parameters for the stored procedure here
	@securityID varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select DISTINCT AMOUNT from PERIOD_FINANCIALS
Where SECURITY_ID = @securityID
AND DATA_ID = 185
AND CURRENCY = 'USD'
AND PERIOD_TYPE = 'C';

END
GO
