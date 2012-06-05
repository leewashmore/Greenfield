USE [AshmoreEMMPOC]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveEMSummaryDataReportPerCountry]    Script Date: 06/05/2012 10:59:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



 CREATE PROCEDURE [dbo].[RetrieveEMSummaryDataReportPerCountry]
		@country		varchar(9)	
AS
BEGIN

SET NOCOUNT OFF;
SET FMTONLY OFF;

DECLARE @cols NVARCHAR(3000)
SELECT @cols = COALESCE(@cols + '],[', '') + 
   CAST( YEAR1 AS varchar(4))
FROM (SELECT DISTINCT YEAR1 FROM Macroeconomic_Data WHERE COUNTRY_CODE= @country) a ORDER BY YEAR1 ASC

DECLARE @cols1 NVARCHAR(3000)
SELECT @cols1 = COALESCE(@cols1 + ',[', '') + 
+CAST( YEAR1 AS varchar(4))+'] AS YEAR_' + CAST( YEAR1 AS varchar(4))
FROM (SELECT DISTINCT YEAR1 FROM Macroeconomic_Data WHERE COUNTRY_CODE= @country) a ORDER BY YEAR1 ASC
PRINT(@cols1)

DECLARE @query NVARCHAR(4000)
SET @query = 'SELECT COUNTRY_NAME, CATEGORY_NAME, DISPLAY_TYPE, DESCRIPTION
	 ,SORT_ORDER,[' + @cols1 + 
' FROM( SELECT cm.COUNTRY_NAME, md.CATEGORY_NAME, md.DISPLAY_TYPE, md.DESCRIPTION
	 ,md.SORT_ORDER, m.YEAR1, m.VALUE
  from dbo.Macroeconomic_Data m
 inner join dbo.Macroeconomic_Display md on md.FIELD = m.FIELD
 inner join dbo.Country_Master cm on cm.COUNTRY_CODE = m.COUNTRY_CODE
 where MD.DISPLAY_TYPE = ''EMSUMMARY'' AND cm.COUNTRY_CODE='''+@country+''') p 
 PIVOT (SUM(Value) FOR [YEAR1] IN (['+ @cols +'])) AS pvt'

 EXECUTE(@query)


END;


GO

