set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00001'
declare @CurrentScriptVersion as nvarchar(100) = '00002'

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

create procedure [dbo].[GetCompanies]
		@Country			varchar(9)	
	,	@Currency			varchar(50)	
AS
BEGIN

SET NOCOUNT OFF;
SET FMTONLY OFF;

		
	-- Get the Company data.
	select	ci.Name
		,	ci.XRef
		,	ci.ReportNumber
		,	ci.Country
		,	ci.Currency
		,	ci.CurrentPeriod
		,	ci.Earnings
		,	ci.Active
		,	ci.CodeIndustry
		,	ci.CodeSector
	  from dbo.tblCompanyInfo ci
	 where 1=1
	   and (@Country is NULL or ci.Country = @Country)
	   and (@Currency is NULL or ci.Currency = @Currency)
	 ;
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






