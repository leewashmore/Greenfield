--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00065'
declare @CurrentScriptVersion as nvarchar(100) = '00066'
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

/****** Object:  StoredProcedure [dbo].[RetrieveICPresentationOverviewData]    Script Date: 08/29/2012 14:48:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[RetrieveICPresentationOverviewData]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	
	SELECT 
	_PI.AcceptWithoutDiscussionFlag,
	_PI.AdminNotes,
	_PI.Background,
	_PI.CommitteeBuyRange,
	_PI.CommitteePFVMeasure,
	_PI.CommitteeRangeEffectiveThrough,
	_PI.CommitteeRecommendation,
	_PI.CommitteeSellRange,
	_PI.CompetitiveAdvantage,
	_PI.CompetitiveDisadvantage,
	_PI.CreatedBy,
	_PI.CreatedOn,
	_PI.EarningsOutlook,
	_PI.InvestmentThesis,
	_PI.ModifiedBy,
	_PI.ModifiedOn,
	_PI.PresentationID,
	_PI.Presenter,
	_PI.SecurityBuyRange,
	_PI.SecurityCashPosition,
	_PI.SecurityCountry,
	_PI.SecurityCountryCode,
	_PI.SecurityGlobalActiveWeight,
	_PI.SecurityIndustry,
	_PI.SecurityLastClosingPrice,
	_PI.SecurityMSCIIMIWeight,
	_PI.SecurityMSCIStdWeight,
	_PI.SecurityMarketCapitalization,
	_PI.SecurityName,
	_PI.SecurityPFVMeasure,
	_PI.SecurityPosition,
	_PI.SecurityRecommendation,
	_PI.SecuritySellRange,
	_PI.SecurityTicker,
	_PI.Valuations,
	_PI.StatusType,
	_PI.Analyst,
	_PI.Price,
	_PI.FVCalc,
	_PI.SecurityBuySellvsCrnt,
	_PI.CurrentHoldings,
	_PI.PercentEMIF,
	_PI.SecurityBMWeight,
	_PI.SecurityActiveWeight,
	_PI.YTDRet_Absolute,
	_PI.YTDRet_RELtoLOC,
	_PI.YTDRet_RELtoEM,
	_MI.MeetingDateTime,
	_MI.MeetingClosedDateTime,
	_MI.MeetingVotingClosedDateTime
	
	FROM 
	PresentationInfo  _PI  
	LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.PresentationID = _PI.PresentationID
	LEFT JOIN MeetingInfo _MI ON _MPMI.MeetingID = _MI.MeetingID
END


GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00066'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())