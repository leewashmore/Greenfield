SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetDCFRiskFreeRate] 
	(
	@COUNTRY_CODE VARCHAR(40) =''  
	 ) 
AS
BEGIN
	
	SET NOCOUNT ON;

    SELECT * 
    FROM MODEL_INPUTS_CTY
    WHERE COUNTRY_CODE=@COUNTRY_CODE
END
GO
