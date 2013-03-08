set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00123'
declare @CurrentScriptVersion as nvarchar(100) = '00124'

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

IF OBJECT_ID ('[dbo].[RetrieveDistinctFXRates]') IS NOT NULL
	DROP PROCEDURE [dbo].[RetrieveDistinctFXRates]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveDistinctFXRates]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[RetrieveDistinctFXRates]
AS
BEGIN
select Distinct(Currency) from FX_RATES
END 
go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00124'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

