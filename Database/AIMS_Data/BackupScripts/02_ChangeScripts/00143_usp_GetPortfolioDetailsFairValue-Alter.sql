set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00142'
declare @CurrentScriptVersion as nvarchar(100) = '00143'

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

IF OBJECT_ID ('[dbo].[GetPortfolioDetailsFairValue]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetPortfolioDetailsFairValue]
GO

/****** Object:  StoredProcedure [dbo].[GetPortfolioDetailsFairValue]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPortfolioDetailsFairValue] 
	(
	@SECURITY_IDS varchar(max)
	)
AS
BEGIN
	
	declare @TempData Table
	(
	[VALUE_TYPE] [varchar](20) NOT NULL,
	[SECURITY_ID] [varchar](20) NOT NULL,
	[FV_MEASURE] [int] NOT NULL,
	[FV_BUY] [decimal](32, 6) NOT NULL,
	[FV_SELL] [decimal](32, 6) NOT NULL,
	[CURRENT_MEASURE_VALUE] [decimal](32, 6) NOT NULL,
	[UPSIDE] [decimal](32, 6) NOT NULL,
	[UPDATED] [datetime] NULL
	)
	declare @sqlquery varchar(max)
	SET NOCOUNT ON;

if @SECURITY_IDS is not null
begin
set @sqlquery ='SELECT * 
    FROM FAIR_VALUE 
    WHERE VALUE_TYPE=''PRIMARY'' 
    AND SECURITY_ID IN ('+ @SECURITY_IDS+')'
END
INSERT INTO @TempData  EXECUTE(@sqlquery)
    
    Select * from @TempData
    
END

Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00143'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

