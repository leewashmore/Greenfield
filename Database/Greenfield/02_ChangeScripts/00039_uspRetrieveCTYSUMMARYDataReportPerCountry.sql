set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00038'
declare @CurrentScriptVersion as nvarchar(100) = '00039'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RetrieveCTYSUMMARYDataReportPerCountry]
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
PRINT(@cols)

DECLARE @query NVARCHAR(4000)
SET @query = 'SELECT COUNTRY_NAME, CATEGORY_NAME, DISPLAY_TYPE, DESCRIPTION
	 ,SORT_ORDER,[' + @cols1 + 
' FROM( SELECT cm.COUNTRY_NAME, md.CATEGORY_NAME, md.DISPLAY_TYPE, md.DESCRIPTION
	 ,md.SORT_ORDER, m.YEAR1, m.VALUE
  from dbo.Macroeconomic_Data m
 inner join dbo.Macroeconomic_Display md on md.FIELD = m.FIELD
 inner join dbo.Country_Master cm on cm.COUNTRY_CODE = m.COUNTRY_CODE
 where MD.DISPLAY_TYPE = ''CTYSUMMARY'' AND cm.COUNTRY_CODE='''+@country+''') p 
 PIVOT (SUM(Value) FOR [YEAR1] IN (['+ @cols +'])) AS pvt'


EXECUTE(@query)


END;

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00039'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






