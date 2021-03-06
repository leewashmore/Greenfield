SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetDCFSummaryData]
(
@ISSUER_ID varchar(50)
)
AS
DECLARE @CURRENT_YEAR INT

BEGIN

SET @CURRENT_YEAR= YEAR(GETDATE()); 
	SET NOCOUNT ON;
SELECT AMOUNT, DATA_ID FROM PERIOD_FINANCIALS
WHERE ISSUER_ID=@ISSUER_ID
AND DATA_SOURCE='PRIMARY'
AND PERIOD_TYPE='C'
AND CURRENCY='USD'
AND DATA_ID IN (255,258,256,257)
    
END
GO
