using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.ServiceCaller.ModelFXDefinitions;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Collections.ObjectModel;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.ServiceCaller.MeetingDefinitions;
using GreenField.ServiceCaller.AlertDefinitions;
using GreenField.ServiceCaller.DocumentWorkSpaceDefinitions;
using GreenField.ServiceCaller.DCFDefinitions;
using GreenField.ServiceCaller.CustomScreeningDefinitions;

namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Service Caller Interface for Security Reference Data and Holdings, Benchmark & Performance Data.
    /// </summary>
    public interface IDBInteractivity
    {
        #region Build1

        void RetrieveSecurityReferenceData(Action<List<SecurityOverviewData>> callback);

        /// <summary>
        /// service call method for security overview gadget
        /// </summary>
        /// <param name="callback"></param>
        void RetrieveSecurityOverviewData(EntitySelectionData entitySelectionData, Action<SecurityOverviewData> callback);

        void RetrieveEntitySelectionData(Action<List<EntitySelectionData>> callback);

        void RetrieveEntitySelectionWithBenchmarkData(Action<List<EntitySelectionData>> callback);

        void RetrieveUnrealizedGainLossData(EntitySelectionData entityIdentifier, DateTime startDateTime, DateTime endDateTime, String frequencyInterval, Action<List<UnrealizedGainLossData>> callback);

        void RetrievePricingReferenceData(ObservableCollection<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyInterval, Action<List<PricingReferenceData>> callback);

        #endregion

        #region Build2

        void RetrievePortfolioSelectionData(Action<List<PortfolioSelectionData>> callback);

        void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback);

        void RetrieveMarketCapitalizationData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool isExCashSecurity, bool lookThruEnabled, Action<List<MarketCapitalizationData>> callback);

        /// <summary>
        /// Service Caller method for AssetAllocation gadget
        /// </summary>
        /// <param name="fundSelectionData">Selected Portfolio</param>
        /// <param name="effectiveDate">selected Date</param>
        /// <param name="callback">List of AssetAllocationData</param>
        void RetrieveAssetAllocationData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, bool lookThru, bool excludeCash, 
            Action<List<AssetAllocationData>> callback);

        /// <summary>
        /// service call method for sector breakdown gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="isExCashSecurity"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="callback"></param>
        void RetrieveSectorBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, bool lookThruEnabled,
            Action<List<SectorBreakdownData>> callback);

        /// <summary>
        /// service call method for region breakdown gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="isExCashSecurity"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="callback"></param>
        void RetrieveRegionBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, bool lookThruEnabled,
            Action<List<RegionBreakdownData>> callback);

        /// <summary>
        /// service call method for top holdings gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="isExCashSecurity"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="callback"></param>
        void RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, bool lookThruEnabled,
            Action<List<TopHoldingsData>> callback);

        /// <summary>
        /// service call method for index constituent gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="callback"></param>
        void RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool lookThruEnabled,
            Action<List<IndexConstituentsData>> callback);

        /// <summary>
        /// service call method for RiskIndexExposures gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="isExCashSecurity"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="filterType"></param>
        /// <param name="filterValue"></param>
        /// <param name="callback"></param>
        void RetrieveRiskIndexExposuresData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, bool lookThruEnabled, 
            string filterType, string filterValue, Action<List<RiskIndexExposuresData>> callback);

        void RetrieveHoldingsPercentageData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled, Action<List<HoldingsPercentageData>> callback);

        void RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData fundmarkSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled, Action<List<HoldingsPercentageData>> callback);

        void RetrieveTopBenchmarkSecuritiesData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<TopBenchmarkSecuritiesData>> callback);

        void RetrievePortfolioRiskReturnData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback);

        void RetrieveBenchmarkFilterSelectionData(String benchmarkCode, String BenchmarkName, String filterType, Action<List<BenchmarkFilterSelectionData>> callback);

        void RetrieveMarketSnapshotSelectionData(string userName, Action<List<MarketSnapshotSelectionData>> callback);

        void RetrieveMarketSnapshotPreference(int snapshotPreferenceId, Action<List<MarketSnapshotPreference>> callback);

        void RetrieveMarketSnapshotPerformanceData(List<MarketSnapshotPreference> marketSnapshotPreference, Action<List<MarketSnapshotPerformanceData>> callback);

        /// <summary>
        /// Save changes in a specific user snapshot
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="marketSnapshotSelectionData">Snapshot details</param>
        /// <param name="createEntityPreferenceInfo">Snapshot preference for entities that are to be created</param>
        /// <param name="updateEntityPreferenceInfo">Snapshot preference for entities that are to be updated</param>
        /// <param name="deleteEntityPreferenceInfo">Snapshot preference for entities that are to be removed</param>
        /// <param name="deleteGroupPreferenceInfo">Group preference Ids for entity groups that are to be removed</param>
        /// <param name="createGroupPreferenceInfo">Group names for entity groups that are to be created</param>
        /// <param name="callback">Callback method that takes List of MarketSnapshotPreference as its argument</param>
        void SaveMarketSnapshotPreference(string updateXML, Action<List<MarketSnapshotPreference>> callback);

        /// <summary>
        /// Save changes to a new snapshot specified by user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="snapshotName">Snapshot name</param>
        /// <param name="snapshotPreference">Snapshot preference details</param>
        /// <param name="callback">Callback Method that takes List of MarketSnapshotSelectionData as it's argument</param>
        void SaveAsMarketSnapshotPreference(string updateXML, Action<PopulatedMarketSnapshotPerformanceData> callback);

        void RemoveMarketSnapshotPreference(string userName, string snapshotName, Action<bool?> callback);

        void RetrieveFilterSelectionData(PortfolioSelectionData selectedPortfolio, DateTime? effectiveDate, Action<List<FilterSelectionData>> callback);

        void RetrieveRelativePerformanceData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String period, Action<List<RelativePerformanceData>> callback);

        void RetrieveRelativePerformanceSectorData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback);

        void RetrieveRelativePerformanceSecurityData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, string sectorID = null);

        void RetrieveRelativePerformanceCountryActivePositionData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null);

        void RetrieveRelativePerformanceSectorActivePositionData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null);

        void RetrieveRelativePerformanceSecurityActivePositionData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null);

        void RetrievePerformanceGraphData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String period, String country, Action<List<PerformanceGraphData>> callback);

        void RetrievePerformanceGridData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String country, Action<List<PerformanceGridData>> callback);

        void RetrieveAttributionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String nodeName, Action<List<AttributionData>> callback);

        void RetrieveHeatMapData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String period, Action<List<HeatMapData>> callback);

        /// <summary>
        /// Service caller method to retrieve PortfolioDetails Data
        /// </summary>
        /// <param name="objPortfolioIdentifier">Portfolio Identifier</param>
        /// <param name="objSelectedDate">Selected Date</param>
        /// <param name="callback">collection of Portfolio Details Data</param>
        void RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioIdentifier, DateTime objSelectedDate, bool lookThruEnabled, bool excludeCash, bool objGetBenchmark, Action<List<PortfolioDetailsData>> callback);

        /// <summary>
        /// Service caller method to retrieve Benchmark Return Data for MultiLineBenchmarkUI- Chart
        /// </summary>
        /// <param name="objSelectedEntities">Details of Selected Portfolio & Security</param>
        /// <param name="callback">List of BenchmarkChartReturnData</param>
        void RetrieveBenchmarkChartReturnData(Dictionary<string, string> objSelectedEntites, Action<List<BenchmarkChartReturnData>> callback);

        /// <summary>
        /// Service caller method to retrieve Benchmark Return Data for MultiLineBenchmarkUI- Chart 
        /// </summary>
        /// <param name="objSelectedEntites">Details of Selected Portfolio & Security</param>
        /// <param name="callback">List of BenchmarkGridReturnData</param>
        void RetrieveBenchmarkGridReturnData(Dictionary<string, string> objSelectedEntites, Action<List<BenchmarkGridReturnData>> callback);


        #endregion

        #region Slice-3

        void RetrieveRelativePerformanceUIData(Dictionary<string, string> objSelectedEntity, DateTime objEffectiveDate, Action<List<RelativePerformanceUIData>> callback);

        void RetrieveCountrySelectionData(Action<List<CountrySelectionData>> callback);

        void RetrieveRegionSelectionData(Action<List<RegionSelectionData>> callback);

        void RetrieveMacroDatabaseKeyAnnualReportData(String countryName, Action<List<MacroDatabaseKeyAnnualReportData>> callback);

        void RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(String countryName, List<String> countryValues, Action<List<MacroDatabaseKeyAnnualReportData>> callback);

        void RetrieveChartExtensionData(Dictionary<string, string> objSelectedEntities, DateTime objEffectiveDate, Action<List<ChartExtensionData>> callback);


        #endregion

        #region Slice 4 - FX

        void RetrieveCommoditySelectionData(Action<List<FXCommodityData>> callback);

        void RetrieveCommodityData(string commodityID, Action<List<FXCommodityData>> callback);

        #endregion

        #region Slice 5 - External Research

        void RetrieveIssuerReferenceData(EntitySelectionData entitySelectionData, Action<IssuerReferenceData> callback);

        void RetrieveFinancialStatementData(string issuerID, FinancialStatementDataSource dataSource, FinancialStatementPeriodType periodType
            , FinancialStatementFiscalType fiscalType, FinancialStatementType statementType, String currency, Action<List<FinancialStatementData>> callback);

        void RetrieveQuarterlyResultsData(String fieldValue, int yearValue, Action<List<QuarterlyResultsData>> callback);
        void RetrievePRevenueData(EntitySelectionData entitySelectionData, string chartTitle, Action<List<PRevenueData>> callback);

        #region ConsensusEstimatesGadgets

        void RetrieveBasicData(EntitySelectionData entitySelectionData, Action<List<BasicData>> callback);

        void RetrieveTargetPriceData(EntitySelectionData entitySelectionData, Action<List<TargetPriceCEData>> callback);

        void RetrieveConsensusEstimatesMedianData(string issuerId, FinancialStatementPeriodType periodType, String currency, Action<List<ConsensusEstimateMedian>> callback);

        void RetrieveConsensusEstimatesValuationsData(string issuerId,string longName, FinancialStatementPeriodType periodType, String currency, Action<List<ConsensusEstimatesValuations>> callback);


        #endregion

        void RetrieveFinstatDetailData(string issuerId, string securityId, FinancialStatementDataSource dataSource, FinancialStatementFiscalType fiscalType, String currency, Int32 yearRange, Action<List<FinstatDetailData>> callback);

        void RetrieveConsensusEstimateDetailedData(string issuerId, FinancialStatementPeriodType periodType, String currency, Action<List<ConsensusEstimateDetail>> callback);

        void RetrieveRatioComparisonData(String contextSecurityXML, Action<List<RatioComparisonData>> callback);

        void RetrieveRatioSecurityReferenceData(ScatterGraphContext context, IssuerReferenceData issuerDetails, Action<List<GF_SECURITY_BASEVIEW>> callback);

        void RetrieveCOASpecificData(String issuerId, int? securityId, FinancialStatementDataSource cSource, FinancialStatementFiscalType cFiscalType, String cCurrency, Action<List<COASpecificData>> callback);

        void RetrieveValuationGrowthData(PortfolioSelectionData selectedPortfolio, DateTime? effectiveDate, String filterType, String filterValue, bool lookThruEnabled, Action<List<ValuationQualityGrowthData>> callback);
        void RetrieveConsensusEstimateDetailedBrokerData(string issuerId, FinancialStatementPeriodType periodType, String currency, Action<List<ConsensusEstimateDetail>> callback);
        #endregion

        #region Internal Research
        void RetrieveConsensusEstimatesSummaryData(EntitySelectionData entitySelectionData, Action<List<ConsensusEstimatesSummaryData>> callback);

        void RetrieveCompositeFundData(EntitySelectionData entityIdentifiers, PortfolioSelectionData portfolio, Action<List<CompositeFundData>> callback);
        #endregion

        #region PortalEnhancements
        void RetrieveDocumentsData(String searchString, Action<List<DocumentCategoricalData>> callback);

        void UploadDocument(String fileName, Byte[] fileByteStream, String deleteFileUrl, Action<String> callback);

        void DeleteDocument(String fileName, Action<bool?> callback);

        void RetrieveDocument(String fileName, Action<Byte[]> callback);

        void SetUploadFileInfo(String userName, String Name, String Location, String CompanyName, String SecurityName
            , String SecurityTicker, String Type, String MetaTags, String Comments, Action<Boolean?> callback);

        void GetDocumentsMetaTags(Action<List<string>> callback, Boolean OnlyTags = false);

        void RetrieveDocumentsDataForUser(String userName, Action<List<DocumentCategoricalData>> callback);

        void SetDocumentComment(String userName, Int64 fileId, String comment, Action<Boolean?> callback);

        void DeleteFileMasterRecord(Int64 fileId, Action<Boolean?> callback);

        void UpdateDocumentsDataForUser(Int64 fileId, String fileName, String userName, String metaTags, String companyInfo
            , String categoryType, String comment, Byte[] overwriteStream, Action<Boolean?> callback);
        #endregion

        #region Investment Committee

        void RetrieveMeetingInfoByPresentationStatus(String presentationStatus, Action<List<MeetingInfo>> callback);

        void RetrieveMeetingMinuteDetails(Int64? meetingID, Action<List<MeetingMinuteData>> callback);

        void RetrieveMeetingAttachedFileDetails(Int64? meetingID, Action<List<FileMaster>> callback);

        void UpdateMeetingMinuteDetails(String userName, MeetingInfo meetingInfo, List<MeetingMinuteData> meetingMinuteData, Action<Boolean?> callback);

        void UpdateMeetingAttachedFileStreamData(String userName, Int64 meetingId, FileMaster fileMasterInfo, Boolean deletionFlag, Action<Boolean?> callback);

        void RetrievePresentationOverviewData(Action<List<ICPresentationOverviewData>> callback);

        void RetrievePresentationVoterData(Int64 presentationId, Action<List<VoterInfo>> callback, Boolean includeICAdminInfo = false);

        void RetrieveSecurityPFVMeasureCurrentPrices(String securityId, List<String> pfvTypeInfo, Action<Dictionary<String, Decimal?>> callback);

        void UpdateDecisionEntryDetails(String userName, ICPresentationOverviewData presentationOverViewData, List<VoterInfo> voterInfo, Action<Boolean?> callback);

        void CreatePresentation(String userName, ICPresentationOverviewData presentationOverviewData, Action<Boolean?> callback);

        void RetrieveSecurityDetails(EntitySelectionData entitySelectionData, ICPresentationOverviewData presentationOverviewData, PortfolioSelectionData portfolioData, Action<ICPresentationOverviewData> callback);

        void GetAvailablePresentationDates(Action<List<MeetingInfo>> callback);

        void UpdateMeetingConfigSchedule(String userName, MeetingConfigurationSchedule meetingConfigurationSchedule, Action<Boolean?> callback);

        void RetrievePresentationComments(Int64 presentationId, Action<List<CommentInfo>> callback);

        void SetPresentationComments(string userName, Int64 presentationId, String comment, Action<List<CommentInfo>> callback);

        void UpdatePreMeetingVoteDetails(String userName, List<VoterInfo> voterInfo, Action<Boolean?> callback);

        void SetMeetingPresentationStatus(String userName, Int64 meetingId, String status, Action<Boolean?> callback);

        void UpdateMeetingPresentationDate(String userName, Int64 presentationId, MeetingInfo meetingInfo, Action<Boolean?> callback);

        void GetMeetingConfigSchedule(Action<MeetingConfigurationSchedule> callback);

        void RetrievePresentationAttachedFileDetails(Int64? presentationID, Action<List<FileMaster>> callback);

        //  void UpdatePresentationAttachedFileStreamData(String userName, Int64 presentationId, string url, FileMaster presentationAttachedFileData, Action<Boolean?> callback);

        void UpdatePresentationAttachedFileStreamData(String userName, Int64 presentationId, FileMaster fileMasterInfo, Boolean deletionFlag, Action<Boolean?> callback);

        void SetICPPresentationStatus(String userName, Int64 presentationId, String status, Action<Boolean?> callback);

        void RetrieveCurrentPFVMeasures(List<String> PFVTypeInfo, String securityTicker, Action<Dictionary<String, Decimal?>> callback);

        void GetAllUsers(Action<List<MembershipUserInfo>> callback);

        void GetUsersByNames(List<String> userNames, Action<List<MembershipUserInfo>> callback);

        void SetMessageInfo(String emailTo, String emailCc, String emailSubject, String emailMessageBody, String emailAttachment
            , String userName, Action<Boolean?> callback);

        void GenerateMeetingMinutesReport(Int64 meetingId, Action<Byte[]> callback);

        void GeneratePreMeetingVotingReport(Int64 presentationId, Action<Byte[]> callback);

        void RetrieveSummaryReportData(DateTime startDate, DateTime endDate, Action<List<SummaryReportData>> callback);

        void GenerateICPacketReport(Int64 presentationId, Action<Byte[]> callback);

        void ReSubmitPresentation(String userName, ICPresentationOverviewData presentationOverviewData, Boolean sendAlert, Action<Boolean?> callback);
        #endregion

        #region DCF

        void RetrieveDCFAnalysisData(EntitySelectionData entitySelectionData, Action<List<DCFAnalysisSummaryData>> callback);

        void RetrieveDCFTerminalValueCalculationsData(EntitySelectionData entitySelectionData, Action<List<DCFTerminalValueCalculationsData>> callback);

        void RetrieveCashFlows(EntitySelectionData entitySelectionData, Action<List<DCFCashFlowData>> callback);
        void RetrieveDCFFreeCashFlowsData(EntitySelectionData entitySelectionData, Action<List<FreeCashFlowsData>> callback);

        void RetrieveDCFSummaryData(EntitySelectionData entitySelectionData, Action<List<DCFSummaryData>> callback);

        void RetrieveDCFCurrentPrice(EntitySelectionData entitySelectionData, Action<decimal?> callback);

        void RetrieveDCFFairValueData(EntitySelectionData entitySelectionData, Action<List<PERIOD_FINANCIALS>> callback);

        void InsertDCFFairValueData(EntitySelectionData entitySelectionData, string valueType, int? fvMeasure, decimal? fvbuy, decimal? fvSell, decimal? currentMeasureValue, decimal? upside, DateTime? updated, Action<bool> callback);
        #endregion

        void RetrieveCompanyData(Action<List<String>> callback);

        #region Custom Screening Tool

        void RetrieveCustomControlsList(string parameter, Action<List<string>> callback);

        void RetrieveSecurityReferenceTabDataPoints(Action<List<CustomSelectionData>> callback);

        void RetrievePeriodFinancialsTabDataPoints(Action<List<CustomSelectionData>> callback);

        void RetrieveCurrentFinancialsTabDataPoints(Action<List<CustomSelectionData>> callback);

        void RetrieveFairValueTabDataPoints(Action<List<CustomSelectionData>> callback);        

        void RetrieveSecurityData(PortfolioSelectionData portfolio, EntitySelectionData benchmark, String region, String country, String sector, String industry,
                                        List<CSTUserPreferenceInfo> userPreference,Action<List<CustomScreeningSecurityData>> callback);

        void SaveUserDataPointsPreference(string userPreference, string username, Action<Boolean?> callback);

        void GetCustomScreeningUserPreferences(string username, Action<List<CSTUserPreferenceInfo>> callback);

        void UpdateUserDataPointsPreference(string userPreference, string username, string existingListname, string newListname, string accessibility, Action<Boolean?> callback);

        void RetrieveFairValueTabSource(Action<List<string>> callback);
        #endregion

        #region Documents

        void RetrieveDocumentsData(EntitySelectionData selectedSecurity, Action<byte[]> callback);        
        #endregion

        #region FAIR VALUE 
        void RetrieveFairValueCompostionSummary(EntitySelectionData entitySelectionData, Action<List<FairValueCompositionSummaryData>> callback);
        void RetrieveFairValueCompostionSummaryData(EntitySelectionData entitySelectionData, Action<List<FairValueCompositionSummaryData>> callback);

        void RetrieveFairValueDataWithNewUpside(EntitySelectionData entitySelectionData, FairValueCompositionSummaryData editedFairValueData
            ,Action<FairValueCompositionSummaryData> callback);

        void SaveUpdatedFairValueData(EntitySelectionData entitySelectionData, List<FairValueCompositionSummaryData> editedFairValueDataList
            , Action<List<FairValueCompositionSummaryData>> callback);

        #endregion

        void UploadModelExcelSheet(byte[] fileStream, string userName, Action<string> callback);
    }
}
