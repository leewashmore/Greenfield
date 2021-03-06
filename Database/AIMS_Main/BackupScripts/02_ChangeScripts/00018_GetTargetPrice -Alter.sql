set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00017'
declare @CurrentScriptVersion as nvarchar(100) = '00018'

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

ALTER procedure [dbo].[GetTargetPrice](@XRef nvarchar(15)) as

Select a.Xref, a.Ticker, a.CurrentPrice, a.CurrentPriceDate, a.Currency, b.StartDate, b.Median
	,  b.Currency as TargetCurrency, b.NumOfEsts, b.High, b.Low, b.StdDev, c.MeanLabel
  from  [Reuters].[dbo].tblCompanyInfo a
  left join [Reuters].[dbo].tblCETargetPrice b on b.XREF = a.XREF and b.ExpirationDate is NULL 
  left join [Reuters].[dbo].tblConsensusRecommendation c on c.XREF = a.XREF and c.ExpirationDate is NULL
 where 1=1
   and a.XRef = @XRef






GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00018'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



