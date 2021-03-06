SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[RetrieveDataPointsModelUpload](
@ISSUER_ID VARCHAR(50)
	)
AS

DECLARE @COA_TYPE VARCHAR(50)
BEGIN
	SET @COA_TYPE= (Select TOP 1 [COA_TYPE] from INTERNAL_ISSUER where ISSUER_ID=@ISSUER_ID);
	SET NOCOUNT ON;

   SELECT dm.COA as COA
   ,pfd.SORT_ORDER as SORT_ORDER
   ,pfd.DATA_DESC as DATA_DESCRIPTION 
   ,dm.CONVERT_FLAG
   FROM PERIOD_FINANCIALS_DISPLAY pfd
   Inner Join DATA_MASTER dm on pfd.DATA_ID=dm.DATA_ID
   Where pfd.STATEMENT_TYPE in ('BAL','INC','CAS') and pfd.COA_TYPE=@COA_TYPE 
   ORDER BY SORT_ORDER 
END
GO
