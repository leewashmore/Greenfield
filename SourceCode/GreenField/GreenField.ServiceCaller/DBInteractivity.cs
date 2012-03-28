using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.ObjectModel;
using System.Windows;


namespace GreenField.ServiceCaller
{
    [Export(typeof(IDBInteractivity))]
    public class DBInteractivity : IDBInteractivity
    {
        public void GetMessage(Action<String> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrievePrintValueCompleted +=
                new EventHandler<ProxyDataDefinitions.RetrievePrintValueCompletedEventArgs>(client_RetrievePrintValueCompleted);

            client.RetrievePrintValueAsync(callback);
        }

        void client_RetrievePrintValueCompleted(object sender, ProxyDataDefinitions.RetrievePrintValueCompletedEventArgs e)
        {
            Action<String> result = e.UserState as Action<String>;

            if (result != null)
            {
                result(e.Result);
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrievePrintValueCompleted -=
                client_RetrievePrintValueCompleted;
        }

        public void RetrieveDetailedEstimates(String companyName, String periodType, String estimateType, Action<List<DetailedEstimates_Result>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDetailedEstimatesCompleted +=
                new EventHandler<RetrieveDetailedEstimatesCompletedEventArgs>(client_RetrieveDetailedEstimatesCompleted);

            client.RetrieveDetailedEstimatesAsync(companyName, periodType, estimateType, callback);
        }

        void client_RetrieveDetailedEstimatesCompleted(object sender, RetrieveDetailedEstimatesCompletedEventArgs e)
        {
            Action<List<DetailedEstimates_Result>> result = e.UserState as Action<List<DetailedEstimates_Result>>;

            if (result != null)
            {
                result(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDetailedEstimatesCompleted -=
                client_RetrieveDetailedEstimatesCompleted;
        }

        public void RetrieveConsensusEstimates(String companyName, String periodType, Action<List<ConsensusEstimates_Result>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveConsensusEstimatesCompleted +=
                new EventHandler<RetrieveConsensusEstimatesCompletedEventArgs>(client_RetrieveConsensusEstimatesCompleted);

            client.RetrieveConsensusEstimatesAsync(companyName, periodType, callback);
        }

        void client_RetrieveConsensusEstimatesCompleted(object sender, RetrieveConsensusEstimatesCompletedEventArgs e)
        {
            Action<List<ConsensusEstimates_Result>> result = e.UserState as Action<List<ConsensusEstimates_Result>>;

            if (result != null)
            {
                result(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveConsensusEstimatesCompleted -=
                client_RetrieveConsensusEstimatesCompleted;
        }

        public void RetrieveCompaniesList(Action<List<GetCompanies_Result>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveCompaniesListCompleted +=
                new EventHandler<RetrieveCompaniesListCompletedEventArgs>(client_RetrieveCompaniesListCompleted);

            client.RetrieveCompaniesListAsync(callback);
        }

        void client_RetrieveCompaniesListCompleted(object sender, RetrieveCompaniesListCompletedEventArgs e)
        {
            Action<List<GetCompanies_Result>> result = e.UserState as Action<List<GetCompanies_Result>>;

            if (result != null)
            {
                result(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveCompaniesListCompleted -=
                client_RetrieveCompaniesListCompleted;
        }

        public void RetrieveDimensionDataListView(Action<List<String>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDimensionDataListViewCompleted += new EventHandler<RetrieveDimensionDataListViewCompletedEventArgs>(client_RetrieveDimensionDataListViewCompleted);


            client.RetrieveDimensionDataListViewAsync(callback);
        }

        void client_RetrieveDimensionDataListViewCompleted(object sender, RetrieveDimensionDataListViewCompletedEventArgs e)
        {
            Action<List<String>> result = e.UserState as Action<List<String>>;

            if (result != null)
            {
                result(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDimensionDataListViewCompleted -=
                client_RetrieveDimensionDataListViewCompleted;
        }

        public void RetrieveDimensionDataForSelectedView(String viewName, Action<List<HoldingsData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDimensionDataForSelectedViewCompleted +=
                new EventHandler<RetrieveDimensionDataForSelectedViewCompletedEventArgs>(client_RetrieveDimensionDataForSelectedViewCompleted);


            client.RetrieveDimensionDataForSelectedViewAsync(viewName, callback);
        }

        void client_RetrieveDimensionDataForSelectedViewCompleted(object sender, RetrieveDimensionDataForSelectedViewCompletedEventArgs e)
        {
            Action<List<HoldingsData>> result = e.UserState as Action<List<HoldingsData>>;

            if (result != null)
            {
                result(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDimensionDataListViewCompleted -=
                client_RetrieveDimensionDataListViewCompleted;
        }

        public void RetrievePerformanceDataForSelectedView(String viewName, Action<List<PerformanceData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrievePerformanceDataForSelectedViewCompleted +=
                new EventHandler<RetrievePerformanceDataForSelectedViewCompletedEventArgs>(client_RetrievePerformanceDataForSelectedViewCompleted);

            client.RetrievePerformanceDataForSelectedViewAsync(viewName, callback);
        }

        void client_RetrievePerformanceDataForSelectedViewCompleted(object sender, RetrievePerformanceDataForSelectedViewCompletedEventArgs e)
        {
            Action<List<PerformanceData>> result = e.UserState as Action<List<PerformanceData>>;

            if (result != null)
            {
                result(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDimensionDataListViewCompleted -=
                client_RetrieveDimensionDataListViewCompleted;
        }

        public void RetrieveReferenceDataForSelectedView(String viewName, Action<List<ReferenceData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveReferenceDataForSelectedViewCompleted +=
                new EventHandler<RetrieveReferenceDataForSelectedViewCompletedEventArgs>(client_RetrieveReferenceDataForSelectedViewCompleted);

            client.RetrieveReferenceDataForSelectedViewAsync(viewName, callback);
        }

        void client_RetrieveReferenceDataForSelectedViewCompleted(object sender, RetrieveReferenceDataForSelectedViewCompletedEventArgs e)
        {
            Action<List<ReferenceData>> result = e.UserState as Action<List<ReferenceData>>;

            if (result != null)
            {
                result(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDimensionDataListViewCompleted -=
                client_RetrieveDimensionDataListViewCompleted;
        }

        public void RetrievePerformanceDataListView(Action<List<String>> callback)
        {
            RetrieveDimensionDataListView(callback);
        }

        public void RetrieveReferenceDataListView(Action<List<String>> callback)
        {
            RetrieveDimensionDataListView(callback);
        }

        public void RetrieveAggregateDataListView(Action<List<String>> callback)
        {
            RetrieveDimensionDataListView(callback);
        }

        public void RetrieveAggregateDataForSelectedView(String portfolioName, Action<List<AggregatedData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveAggregateDataForSelectedViewCompleted +=
                new EventHandler<RetrieveAggregateDataForSelectedViewCompletedEventArgs>(client_RetrieveAggregateDataForSelectedViewCompleted);

            client.RetrieveAggregateDataForSelectedViewAsync(portfolioName, callback);
        }

        void client_RetrieveAggregateDataForSelectedViewCompleted(object sender, RetrieveAggregateDataForSelectedViewCompletedEventArgs e)
        {
            Action<List<AggregatedData>> result = e.UserState as Action<List<AggregatedData>>;

            if (result != null)
            {
                result(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client =
                new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrieveDimensionDataListViewCompleted -=
                client_RetrieveDimensionDataListViewCompleted;
        }

        public void RetrievePortfolioNames(string viewName, Action<List<string>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrievePortfolioNamesCompleted +=
                new EventHandler<RetrievePortfolioNamesCompletedEventArgs>(client_RetrievePortfolioNamesCompleted);

            client.RetrievePortfolioNamesAsync(viewName, callback);
        }

        void client_RetrievePortfolioNamesCompleted(object sender, RetrievePortfolioNamesCompletedEventArgs e)
        {
            Action<List<string>> callbackMethod = e.UserState as Action<List<string>>;

            if (callbackMethod != null)
            {
                callbackMethod(e.Result.ToList());
            }

            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();

            client.RetrievePortfolioNamesCompleted -= client_RetrievePortfolioNamesCompleted;
        }

        public void RetrieveSecurityReferenceData(Action<List<SecurityOverviewData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveSecurityReferenceDataAsync();
            client.RetrieveSecurityReferenceDataCompleted += (se, e) =>
                {
                    if (callback != null)
                        callback(e.Result.ToList());
                };
        }

        public void RetrieveSecurityReferenceDataByTicker(string ticker, Action<SecurityOverviewData> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveSecurityReferenceDataByTickerAsync(ticker);
            client.RetrieveSecurityReferenceDataByTickerCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void RetrieveEntitySelectionData(Action<List<EntitySelectionData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveEntitySelectionDataAsync();
            client.RetrieveEntitySelectionDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveFundSelectionData(Action<List<FundSelectionData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveFundSelectionDataAsync();
            client.RetrieveFundSelectionDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveBenchmarkSelectionDataAsync();
            client.RetrieveBenchmarkSelectionDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
                }
            };
        }

        public void RetrievePricingReferenceData(ObservableCollection<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyInterval, bool chartEntityTypes, Action<List<PricingReferenceData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrievePricingReferenceDataAsync(entityIdentifiers.ToList(), startDateTime, endDateTime, totalReturnCheck, frequencyInterval, chartEntityTypes);
            client.RetrievePricingReferenceDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveMarketCapitalizationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<MarketCapitalizationData> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveMarketCapitalizationDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveMarketCapitalizationDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void RetrieveAssetAllocationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<AssetAllocationData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveAssetAllocationDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveAssetAllocationDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveSectorBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<SectorBreakdownData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveSectorBreakdownDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveSectorBreakdownDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveRegionBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RegionBreakdownData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveRegionBreakdownDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveRegionBreakdownDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveTopHoldingsData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<TopHoldingsData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveTopHoldingsDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveTopHoldingsDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveIndexConstituentsData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<IndexConstituentsData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveIndexConstituentsDataAsync(benchmarkSelectionData, effectiveDate);
            client.RetrieveIndexConstituentsDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveHoldingsPercentageData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<HoldingsPercentageData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveHoldingsPercentageDataAsync(benchmarkSelectionData, effectiveDate);
            client.RetrieveHoldingsPercentageDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveTopBenchmarkSecuritiesData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<TopBenchmarkSecuritiesData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveTopBenchmarkSecuritiesDataAsync(benchmarkSelectionData, effectiveDate);
            client.RetrieveTopBenchmarkSecuritiesDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrievePortfolioRiskReturnData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrievePortfolioRiskReturnDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrievePortfolioRiskReturnDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveUserPreferenceBenchmarkData(string userName, Action<List<UserBenchmarkPreference>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveUserPreferenceBenchmarkDataAsync(userName);
            client.RetrieveUserPreferenceBenchmarkDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveMorningSnapshotData(List<UserBenchmarkPreference> userBenchmarkPreference, Action<List<MorningSnapshotData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveMorningSnapshotDataAsync(userBenchmarkPreference);
            client.RetrieveMorningSnapshotDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void AddUserPreferenceBenchmarkGroup(string userName, string groupName, Action<bool> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.AddUserPreferenceBenchmarkGroupAsync(userName, groupName);
            client.AddUserPreferenceBenchmarkGroupCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void RemoveUserPreferenceBenchmarkGroup(string userName, string groupName, Action<bool> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RemoveUserPreferenceBenchmarkGroupAsync(userName, groupName);
            client.RemoveUserPreferenceBenchmarkGroupCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void AddUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference, Action<bool> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.AddUserPreferenceBenchmarkAsync(userName, userBenchmarkPreference);
            client.AddUserPreferenceBenchmarkCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void RemoveUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference, Action<bool> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RemoveUserPreferenceBenchmarkAsync(userName, userBenchmarkPreference);
            client.RemoveUserPreferenceBenchmarkCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        #region Build2

        public void RetrieveRelativePerformanceData(string objPortfolioIdentifier, string objEntityIdentifier, Action<List<RelativePerformanceData>> callback)
        {
            ProxyDataOperationsClient client = new ProxyDataOperationsClient();
            client.RetrieveRelativePerformanceDataAsync(objPortfolioIdentifier, objEntityIdentifier);
            client.RetrieveRelativePerformanceDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrievePortfolioDetailsData(string objPortfolioIdentifier, Action<List<PortfolioDetailsData>> callback)
        {
            ProxyDataOperationsClient client = new ProxyDataOperationsClient();
            client.RetrievePortfolioDetailsDataAsync(objPortfolioIdentifier);
            client.RetrievePortfolioDetailsDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        #endregion


        #region Interaction Method for Theoretical Unrealized Gain Loss Gadget

        /// <summary>
        /// Method that calls the Unrealized Gain Loss service method and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="entityIdentifier">Unique Identifier for each security</param>
        /// <param name="startDateTime">Start Date time of the time period selected</param>
        /// <param name="endDateTime">End Date time of the time period selected</param>
        /// <param name="frequencyInterval">frequency Interval selected</param>
        /// <param name="callback"></param>
        public void RetrieveUnrealizedGainLossData(string entityIdentifier, DateTime startDateTime, DateTime endDateTime, string frequencyInterval, Action<List<UnrealizedGainLossData>> callback)
        {

            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveUnrealizedGainLossDataAsync(entityIdentifier, startDateTime, endDateTime, frequencyInterval);
            client.RetrieveUnrealizedGainLossDataCompleted += (se, e) =>
               {
                   if (callback != null)
                       if (e.Result != null)
                       {
                           callback(e.Result.ToList());
                       }
                       else
                       {
                           callback(null);
                       }
               };
        }
        #endregion

        public void RetrieveRelativePerformanceData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveRelativePerformanceDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveRelativePerformanceDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        public void RetrieveRelativePerformanceSectorData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveRelativePerformanceSectorDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveRelativePerformanceSectorDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        public void RetrieveRelativePerformanceSecurityData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveRelativePerformanceSecurityDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate, countryID, sectorID, order, maxRecords);
            client.RetrieveRelativePerformanceSecurityDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }
    }
}
