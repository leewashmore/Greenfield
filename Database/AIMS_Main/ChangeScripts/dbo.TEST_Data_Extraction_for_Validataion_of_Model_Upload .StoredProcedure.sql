SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Lane Summers
-- Create date: 10-15-2012
-- Description:	This procedure can be used to export records that would be effected during a Model Upload into AIMS
-- =============================================
alter procedure [dbo].[TEST_Data_Extraction_for_Validataion_of_Model_Upload ]
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

--Declare Variables
declare @sql varchar(8000)
declare @Issuer_id varchar(100)
declare @FileLocation varchar(100) 
declare @Query varchar(max) 

--Set Variables
--set @Issuer_id = '174083' --Ashaka Cement PLC (DONT USE...For some reason this shows as Blue Financial Services in Africa Model)
--set @Issuer_id = '7266678' --Zenith Bank Ltd
--set @Issuer_id = '187559' --CIB smithd\password
set @Issuer_id = '162677' --Illovo Sugar SHUMELDAT\password

set @FileLocation = 'c:\temp\test\After-'+@Issuer_id+'-' -- This is on LONWEB1T:  \\lonweb1t\c$\Temp\TEST
------------------------------------------
--INTERNAL_STATEMENT table:
set @Query = 'SELECT * FROM [AIMS_Main].[dbo].[INTERNAL_STATEMENT] where [ISSUER_ID] = '
select @sql = 'bcp "' + @Query + @Issuer_id + '" QUERYOUT ' + @FileLocation + 'INTERNAL_STATEMENT-' + @Issuer_id + '.txt -c -t\t -T -S ' + @@servername
exec master..xp_cmdshell @sql


--INTERNAL_DATA table for each COA row in the column.  COA rows begin in row 7 of the Worksheet:
set @Query = 'SELECT * FROM [AIMS_Main].[dbo].[INTERNAL_DATA] where [PERIOD_TYPE] = ''A'' AND [ISSUER_ID] = '
select @sql = 'bcp "' + @Query + @Issuer_id + 'order by [COA]' +'" QUERYOUT ' + @FileLocation + 'INTERNAL_DATA-' + @Issuer_id + '.txt -c -t\t -T -S' + @@servername
exec master..xp_cmdshell @sql  


--INTERNAL_COMMODITY_ASSUMPTIONS table:
set @Query = 'SELECT * FROM [AIMS_Main].[dbo].[INTERNAL_COMMODITY_ASSUMPTIONS] where [ISSUER_ID] = '
select @sql = 'bcp "' + @Query + @Issuer_id + '" QUERYOUT ' + @FileLocation + 'INTERNAL_COMMODITY_ASSUMPTIONS-' + @Issuer_id + '.txt -c -t\t -T -S' + @@servername
exec master..xp_cmdshell @sql  


--INTERNAL_ISSUER table:
set @Query = 'SELECT * FROM [AIMS_Main].[dbo].[INTERNAL_ISSUER] where [ISSUER_ID] = '
select @sql = 'bcp "' + @Query + @Issuer_id + '" QUERYOUT ' + @FileLocation + 'INTERNAL_ISSUER-' + @Issuer_id + '.txt -c -t\t -T -S' + @@servername
exec master..xp_cmdshell @sql  
    
  
--INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION table
set @Query = 'SELECT * FROM [AIMS_Main].[dbo].[INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION] where [ISSUER_ID] = '
select @sql = 'bcp "' + @Query + @Issuer_id + '" QUERYOUT ' + @FileLocation + 'INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION-' + @Issuer_id + '.txt -c -t\t -T -S' + @@servername
exec master..xp_cmdshell @sql  


--INTERNAL_MODEL_LOAD table (The Excel document was added to the documentation store):
set @Query = 'SELECT * FROM [AIMS_Main].[dbo].[INTERNAL_MODEL_LOAD] where [ISSUER_ID] = '
select @sql = 'bcp "' + @Query + @Issuer_id + '" QUERYOUT ' + @FileLocation + 'INTERNAL_MODEL_LOAD-' + @Issuer_id + '.txt -c -t\t -T -S' + @@servername
exec master..xp_cmdshell @sql  
  

--INTERNAL_COA_CHANGES table
set @Query = 'SELECT * FROM [AIMS_Main].[dbo].[INTERNAL_COA_CHANGES] where [ISSUER_ID] = '
select @sql = 'bcp "' + @Query + @Issuer_id + '" QUERYOUT ' + @FileLocation + 'INTERNAL_COA_CHANGES-' + @Issuer_id + '.txt -c -t\t -T -S' + @@servername
exec master..xp_cmdshell @sql  

-------------------------------------------------------------------------

END
GO
