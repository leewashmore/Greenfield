using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.DataContracts;
using GreenField.ServiceCaller.PerformanceDefinitions;

namespace Greenfield.ServiceCaller.UnitTest
{
    [TestClass]
    public class DbInteractivityTestClass : SilverlightTest
    {
        #region Security Overview
        /// <summary>
        /// RetrieveSecurityOverviewData Test Method - Null Result Set for specifit Ticker
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveSecurityOverviewDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entityIdentifier = new EntitySelectionData()
                    {
                        InstrumentID = "BRPETROBRE",
                        LongName = "PETROBRAS - PETROLEO BRAS",
                        ShortName = "PETR3 BZ",
                        SecurityType = "EQUITY"
                    };

            instance.RetrieveSecurityOverviewData(entityIdentifier, (SecurityOverviewData resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Security data for Selected Ticker not returned");
                EnqueueTestComplete();
            });
        }
        #endregion

        #region ToolBox Selectors
        /// <summary>
        /// RetrieveEntitySelectionData Test Method - Null Result Set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveEntitySelectionDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.RetrieveEntitySelectionData((List<EntitySelectionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Entities not available");
                EnqueueTestComplete();
            });

        }
        #endregion

        #region Build1

        #region Closing/Gross Price Chart

        /// <summary>
        /// RetrievePricingReferenceData Test Method - Sample Data
        /// entityIdentifiers - Instrument ID - BRPETROBRE
        /// startDateTime - DateTime.Now.AddYears(-1)
        /// endDateTime - DateTime.Now
        /// totalReturnCheck - false
        /// frequencyInterval - Daily
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Pricing")]
        public void RetrievePricingReferenceDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            ObservableCollection<EntitySelectionData> entityIdentifiers =
                new ObservableCollection<EntitySelectionData> { new EntitySelectionData() { InstrumentID = "BRPETROBRE" } };
            DateTime startDateTime = DateTime.Now.AddYears(-1);
            DateTime endDateTime = DateTime.Now;
            bool totalReturnCheck = false;
            string frequencyInterval = "Daily";

            instance.RetrievePricingReferenceData(entityIdentifiers, startDateTime, endDateTime, totalReturnCheck,
                frequencyInterval, (List<PricingReferenceData> resultSet) =>
                {
                    Assert.IsNotNull(resultSet, "Pricing Reference Data Not Available");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrievePricingReferenceData Test Method - entityIdentifiers as null - should return an empty result set
        /// entityIdentifiers - null
        /// startDateTime - DateTime.Now.AddYears(-1)
        /// endDateTime - DateTime.Now
        /// totalReturnCheck - false
        /// frequencyInterval - Daily
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Pricing")]
        public void RetrievePricingReferenceDataEntityIdentifiersNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            ObservableCollection<EntitySelectionData> entityIdentifiers = null;
            DateTime startDateTime = DateTime.Now.AddYears(-1);
            DateTime endDateTime = DateTime.Now;
            bool totalReturnCheck = false;
            string frequencyInterval = "Daily";

            instance.RetrievePricingReferenceData(entityIdentifiers, startDateTime, endDateTime, totalReturnCheck,
                frequencyInterval, (List<PricingReferenceData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "Pricing Reference Data Should Be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrievePricingReferenceData Test Method - entityIdentifiers Empty - should return an empty result set
        /// entityIdentifiers - Empty
        /// startDateTime - DateTime.Now.AddYears(-1)
        /// endDateTime - DateTime.Now
        /// totalReturnCheck - false
        /// frequencyInterval - Daily
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Pricing")]
        public void RetrievePricingReferenceDataEntityIdentifiersEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            ObservableCollection<EntitySelectionData> entityIdentifiers = new ObservableCollection<EntitySelectionData>();
            DateTime startDateTime = DateTime.Now.AddYears(-1);
            DateTime endDateTime = DateTime.Now;
            bool totalReturnCheck = false;
            string frequencyInterval = "Daily";

            instance.RetrievePricingReferenceData(entityIdentifiers, startDateTime, endDateTime, totalReturnCheck,
                frequencyInterval, (List<PricingReferenceData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "Pricing Reference Data Should Be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrievePricingReferenceData Test Method - endDateTime exceeds startDateTime - should return an empty result set
        /// entityIdentifiers - null
        /// startDateTime - DateTime.Now
        /// endDateTime - DateTime.Now.AddYears(-1)
        /// totalReturnCheck - false
        /// frequencyInterval - Daily
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Pricing")]
        public void RetrievePricingReferenceDataSelectionDatesOrderedTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            ObservableCollection<EntitySelectionData> entityIdentifiers =
                new ObservableCollection<EntitySelectionData> { new EntitySelectionData() { InstrumentID = "BRPETROBRE" } };
            DateTime startDateTime = DateTime.Now;
            DateTime endDateTime = DateTime.Now.AddYears(-1);
            bool totalReturnCheck = false;
            string frequencyInterval = "Daily";

            instance.RetrievePricingReferenceData(entityIdentifiers, startDateTime, endDateTime, totalReturnCheck, frequencyInterval, (List<PricingReferenceData> result) =>
            {
                Assert.AreEqual<int>(0, result.Count, "Pricing Reference Data Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Unrealized Gain Loss Chart

        /// <summary>
        /// RetrieveUnrealizedGainLossData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveUnrealizedGainLossDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entity = new EntitySelectionData() { ShortName = "PETR3 BZ" };
            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;
            string frequencyInterval = "Daily";

            instance.RetrieveUnrealizedGainLossData(entity, startDate, endDate,
                frequencyInterval, (List<UnrealizedGainLossData> resultSet) =>
                {
                    Assert.IsNotNull(resultSet, "Unrealized Gain-Loss Data Not Available");
                    EnqueueTestComplete();
                });

        }

        /// <summary>
        /// RetrieveUnrealizedGainLossData Test Method - entityIdentifiers as null - should return an empty result set
        /// entityIdentifiers - null
        /// startDateTime - DateTime.Now.AddYears(-1)
        /// endDateTime - DateTime.Now
        /// frequencyInterval - Daily
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveUnrealizedGainLossDataEntityIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entity = null;
            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;
            string frequencyInterval = "Daily";

            instance.RetrieveUnrealizedGainLossData(entity, startDate, endDate,
                frequencyInterval, (List<UnrealizedGainLossData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "Unrealized Gain-Loss Data Should Be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveUnrealizedGainLossData Test Method - entityIdentifiers Empty - should return an empty result set
        /// entityIdentifiers - Empty
        /// startDateTime - DateTime.Now.AddYears(-1)
        /// endDateTime - DateTime.Now
        /// frequencyInterval - Daily
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveUnrealizedGainLossDataEntityIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entity = new EntitySelectionData();
            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;
            string frequencyInterval = "Daily";

            instance.RetrieveUnrealizedGainLossData(entity, startDate, endDate,
                frequencyInterval, (List<UnrealizedGainLossData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "Unrealized Gain-Loss Should Be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveUnrealizedGainLossData Test Method - endDateTime exceeds startDateTime - should return an empty result set
        /// entityIdentifiers - null
        /// startDateTime - DateTime.Now
        /// endDateTime - DateTime.Now.AddYears(-1)
        /// frequencyInterval - Daily
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveUnrealizedGainLossDataSelectionDatesOrderedTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entity = new EntitySelectionData() { ShortName = @"PETR3 BZ" };
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddYears(-1);
            string frequencyInterval = "Daily";

            instance.RetrieveUnrealizedGainLossData(entity, startDate, endDate,
                frequencyInterval, (List<UnrealizedGainLossData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "Unrealized Gain-Loss Data Should Be Empty");
                    EnqueueTestComplete();
                });
        }

        #endregion

        #endregion

        #region Build 2

        #region Top 10 Holdings Gadget

        /// <summary>
        /// RetrieveTopHoldingsData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopHoldingsDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            bool isCashExclude = false;
            bool islookthruenabled = false;
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, isCashExclude,islookthruenabled, (List<TopHoldingsData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Top 10 Holdings Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTopHoldingsData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopHoldingsDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, isCashExclude,islookthruenabled, (List<TopHoldingsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Top 10 Holdings Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTopHoldingsData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// isCashExclude = false
        /// islookthruenabled = false;
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopHoldingsDataSelectionDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, isCashExclude,islookthruenabled, (List<TopHoldingsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Top 10 Holdings Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTopHoldingsData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// isCashExclude = false
        /// islookthruenabled = false;
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopHoldingsDataSelectionDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, isCashExclude,islookthruenabled, (List<TopHoldingsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Top 10 Holdings Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Index Constituent Export Gadget

        /// <summary>
        /// RetrieveIndexConstituentData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveIndexConstituentDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool islookthruenabled = false;
            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate,islookthruenabled, (List<IndexConstituentsData> resultSet) =>
                {
                    Assert.IsNotNull(resultSet, "Index constituent Data Not Available");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveIndexConstituentData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveIndexConstituentDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool islookthruenabled = false;
            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate, islookthruenabled, (List<IndexConstituentsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Index constituent Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveIndexConstituentData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// islookthruenabled = false;
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveIndexConstituentDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool islookthruenabled = false;
            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate,islookthruenabled, (List<IndexConstituentsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Index constituent Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveIndexConstituentData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// islookthruenabled = false;
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveIndexConstituentDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool islookthruenabled = false;
            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate,islookthruenabled, (List<IndexConstituentsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Index constituent Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region MarketCapitalization Gadget

        /// <summary>
        /// RetrieveMarketCapitalizationData test method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMarketCapitalizationDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            string filterType = null;
            string filterValue = null;
            bool isExCash = false;
            bool lookThru = false;
            instance.RetrieveMarketCapitalizationData(portfolio, effectiveDate, filterType, filterValue, isExCash, lookThru, (List<MarketCapitalizationData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Market Capitalization Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketCapitalizationData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMarketCapitalizationDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            string filterType = null;
            string filterValue = null;
            bool isExCash = false;
            bool lookThru = false;
            instance.RetrieveMarketCapitalizationData(portfolio, effectiveDate, filterType, filterValue, isExCash, lookThru, (List<MarketCapitalizationData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Market Capitalization Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketCapitalizationData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMarketCapitalizationDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = Convert.ToDateTime("01/31/2012");
            string filterType = null;
            string filterValue = null;
            bool isExCash = false;
            bool lookThru = false;

            instance.RetrieveMarketCapitalizationData(portfolio, effectiveDate, filterType, filterValue, isExCash, lookThru, (List<MarketCapitalizationData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Market Capitalization Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketCapitalizationData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMarketCapitalizationDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = Convert.ToDateTime("01/31/2012");
            string filterType = null;
            string filterValue = null;
            bool isExCash = false;
            bool lookThru = false;

            instance.RetrieveMarketCapitalizationData(portfolio, effectiveDate, filterType, filterValue, isExCash, lookThru, (List<MarketCapitalizationData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Market Capitalization Should Be Empty");
                EnqueueTestComplete();
            });
        }
        #endregion

        #region Region Breakdown

        /// <summary>
        /// RetrieveRegionBreakdownData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveRegionBreakdownDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveRegionBreakdownData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<RegionBreakdownData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Region Breakdown Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRegionBreakdownData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveRegionBreakdownDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveRegionBreakdownData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<RegionBreakdownData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Region Breakdown Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRegionBreakdownData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// isCashExclude = false
        /// islookthruenabled = false;
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveRegionBreakdownDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveRegionBreakdownData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<RegionBreakdownData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Region Breakdown Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRegionBreakdownData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// isCashExclude = false
        /// islookthruenabled = false;
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveRegionBreakdownDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveRegionBreakdownData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<RegionBreakdownData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Region Breakdown Should Be Empty");
                EnqueueTestComplete();
            });
        }
        #endregion       

        #region Sector Breakdown Gadget
        /// <summary>
        /// RetrieveSectorBreakdownData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveSectorBreakdownDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveSectorBreakdownData(portfolio, effectiveDate, isCashExclude,islookthruenabled, (List<SectorBreakdownData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Sector Breakdown Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveSectorBreakdownData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveSectorBreakdownDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveSectorBreakdownData(portfolio, effectiveDate, isCashExclude,islookthruenabled, (List<SectorBreakdownData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Sector Breakdown Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveSectorBreakdownData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// isCashExclude = false
        /// islookthruenabled = false;
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveSectorBreakdownDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveSectorBreakdownData(portfolio, effectiveDate, isCashExclude,islookthruenabled, (List<SectorBreakdownData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Sector Breakdown Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveSectorBreakdownData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// isCashExclude = false
        /// islookthruenabled = false;
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveSectorBreakdownDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            instance.RetrieveSectorBreakdownData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<SectorBreakdownData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Sector Breakdown Should Be Empty");
                EnqueueTestComplete();
            });
        }
        #endregion

        #region AssetAllocationGadget

        /// <summary>
        /// RetrieveAssetAllocationData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Asset")]
        public void RetrieveAssetAllocationDataPortfolioIdentifierNull()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            bool excludeCash = false;
            bool lookThru = false;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveAssetAllocationData(portfolio, effectiveDate, lookThru, excludeCash, (List<AssetAllocationData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Asset Allocation Data should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrievePortfolioDetails Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Asset")]
        public void RetrieveAssetAllocationDataPortfolioIdentifierEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool excludeCash = false;
            bool lookThru = false;
            instance.RetrieveAssetAllocationData(portfolio, effectiveDate, lookThru, excludeCash, (List<AssetAllocationData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Asset Allocation Data should be Empty");
                EnqueueTestComplete();
            });
        }


        /// <summary>
        /// RetrieveAssetAllocationData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Asset")]
        public void RetrieveAssetAllocationDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool excludeCash = false;
            bool lookThru = false;
            instance.RetrieveAssetAllocationData(portfolio, effectiveDate, lookThru, excludeCash, (List<AssetAllocationData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Asset Allocation Data should not be empty");
                EnqueueTestComplete();
            });
        }


        /// <summary>
        /// RetrieveAssetAllocationData Test Method  - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Asset")]
        public void RetrieveAssetAllocationDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool lookThru = false;
            bool excludeCash = false;
            instance.RetrieveAssetAllocationData(portfolio, effectiveDate, lookThru, excludeCash, (List<AssetAllocationData> resultSet) =>
            {
                Assert.AreEqual(0, resultSet.Count, "Asset Allocation Data Should Be Empty");
                EnqueueTestComplete();
            });
        }


        #endregion

        #region PortfolioDetailsUI Gadget

        /// <summary>
        /// RetrievePortfolioDetails Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// bool getBenchmark=false
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Asset")]
        public void RetrievePortfolioDetailsDataPortfolioIdentifierNull()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            bool getBenchmark = false;
            bool excludeCash = false;
            bool lookThru = false;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, lookThru, excludeCash, getBenchmark, (List<PortfolioDetailsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Portfolio Details Data should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrievePortfolioDetails Test Method - portfolioIdentifiers as empty - should return an empty result set
        /// portfolioIdentifiers - empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// bool getBenchmark=false
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Asset")]
        public void RetrievePortfolioDetailsDataPortfolioIdentifierEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            bool lookThru = false;
            bool getBenchmark = false;
            bool excludeCash = false;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, lookThru, excludeCash, getBenchmark, (List<PortfolioDetailsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Portfolio Details Data should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrievePortfolioDetailsData Test Method  - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Asset")]
        public void RetrievePortfolioDetailsDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool lookThru = false;
            bool getBenchamrk = false;
            bool excludeCash = false;
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, lookThru, excludeCash, getBenchamrk, (List<PortfolioDetailsData> resultSet) =>
            {
                Assert.AreEqual(0, resultSet.Count, "Portfolio Details Data Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Slice3

        #region MultiLineBenchmarkUI

        #region Chart

        /// <summary>
        /// RetrieveBenchmarkChartReturnData Test Method - selectedentities as null - should  return an  empty set
        /// selectedentities - null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MultiLineBenchmarkUI")]
        public void RetrieveBenchmarkChartReturnDataSelectedEntitiesNull()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = null;
            instance.RetrieveBenchmarkChartReturnData(selectedEntites, (List<BenchmarkChartReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "BenchmarkChartReturn Data should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveBenchmarkChartReturnData Test Method - selectedentities as empty - should return an empty result set
        /// selectedentities - Empty
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MultiLineBenchmarkUI")]
        public void RetrieveBenchmarkChartReturnDataSelectedEntitiesEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            instance.RetrieveBenchmarkChartReturnData(selectedEntites, (List<BenchmarkChartReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "BenchmarkChartReturn Data should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveBenchmarkChartReturnData Test Method - selectedEntities as DummyValues - should return an empty result set
        /// selectedentities - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MultiLineBenchmarkUI")]
        public void RetrieveBenchmarkChartReturnDataSelectedEntitiesSampleValues()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            selectedEntites.Add("SECURITY", "ABC");
            selectedEntites.Add("PORTFOLIO", "XYZ");
            instance.RetrieveBenchmarkChartReturnData(selectedEntites, (List<BenchmarkChartReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "BenchmarkChartReturn Data should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveBenchmarkChartReturnData Test Method - selectedEntities as Sample Values - should not return an null
        /// selectedentities - SampleData
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MultiLineBenchmarkUI")]
        public void RetrieveBenchmarkChartReturnData()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            selectedEntites.Add("SECURITY", "AFGRI LTD");
            selectedEntites.Add("PORTFOLIO", "AFRICA");
            instance.RetrieveBenchmarkChartReturnData(selectedEntites, (List<BenchmarkChartReturnData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "BenchmarkChartReturn Data should not be null");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Grid

        /// <summary>
        /// RetrieveBenchmarkGridReturnData Test Method - selectedentities as null - should  return an  empty set
        /// selectedentities - null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MultiLineBenchmarkUI")]
        public void RetrieveBenchmarkGridReturnDataSelectedEntitiesNull()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = null;
            instance.RetrieveBenchmarkGridReturnData(selectedEntites, (List<BenchmarkGridReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "BenchmarkGridReturnData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveBenchmarkGridReturnData Test Method - selectedentities as empty - should return an empty result set
        /// selectedentities - Empty
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MultiLineBenchmarkUI")]
        public void RetrieveBenchmarkGridReturnDataSelectedEntitiesEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            instance.RetrieveBenchmarkGridReturnData(selectedEntites, (List<BenchmarkGridReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "BenchmarkGridReturnData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveBenchmarkGridReturnData Test Method - selectedEntities as DummyValues - should return an empty result set
        /// selectedentities - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MultiLineBenchmarkUI")]
        public void RetrieveBenchmarkGridReturnDataDummyValues()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            selectedEntites.Add("SECURITY", "ABC");
            selectedEntites.Add("PORTFOLIO", "XYZ");
            instance.RetrieveBenchmarkGridReturnData(selectedEntites, (List<BenchmarkGridReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "BenchmarkGridReturnData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveBenchmarkGridReturnData Test Method - selectedEntities as Sample Values - should not return an null
        /// selectedentities - SampleData
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MultiLineBenchmarkUI")]
        public void RetrieveBenchmarkGridReturnData()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            selectedEntites.Add("SECURITY", "AFGRI LTD");
            selectedEntites.Add("PORTFOLIO", "AFRICA");
            instance.RetrieveBenchmarkGridReturnData(selectedEntites, (List<BenchmarkGridReturnData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "BenchmarkGridReturnData should not be null");
                EnqueueTestComplete();
            });
        }


        #endregion

        #endregion

        #region ChartExtension

        /// <summary>
        /// RetrieveChartExtensionData Test Method - selectedEntities as null - should  return an  empty set
        /// selectedEntities - null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("ChartExtension")]
        public void RetrieveChartExtensionDataSelectedEntitiesNull()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = null;
            DateTime objEffectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveChartExtensionData(selectedEntites, objEffectiveDate, (List<ChartExtensionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ChartExtensionData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveChartExtensionData Test Method - selectedEntities as empty - should  return an  empty set
        /// selectedEntities - Empty
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("ChartExtension")]
        public void RetrieveChartExtensionDataSelectedEntitiesEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            DateTime objEffectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveChartExtensionData(selectedEntites, objEffectiveDate, (List<ChartExtensionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ChartExtensionData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveChartExtensionData Test Method - selectedEntities as Dummy - should  return an  empty set
        /// selectedEntities - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("ChartExtension")]
        public void RetrieveChartExtensionDataSelectedEntitiesDummyValues()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            selectedEntites.Add("SECURITY", "ABC");
            selectedEntites.Add("PORTFOLIO", "XYZ");
            DateTime objEffectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveChartExtensionData(selectedEntites, objEffectiveDate, (List<ChartExtensionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ChartExtensionData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveChartExtensionData Test Method - selectedEntities as Sample Values - should not return null values
        /// selectedEntities - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("ChartExtension")]
        public void RetrieveChartExtensionDataSelectedEntitiesSampleData()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            DateTime objEffectiveDate = new DateTime(2012, 1, 31);
            selectedEntites.Add("SECURITY", "AAF");
            selectedEntites.Add("PORTFOLIO", "AFRICA");
            instance.RetrieveChartExtensionData(selectedEntites, objEffectiveDate, (List<ChartExtensionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet.Count, "ChartExtensionData should not be null");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Excess Contribution Gadgets

        #region Relative Performance Gadget

        /// <summary>
        /// RetrieveRelativePerformanceData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceData(portfolio, effectiveDate, period, (List<RelativePerformanceData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Relative Performance Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceData(portfolio, effectiveDate, period, (List<RelativePerformanceData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// period - 1M
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceDataSelectionDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceData(portfolio, effectiveDate, period, (List<RelativePerformanceData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// period - 1M
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceDataSelectionDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceData(portfolio, effectiveDate, period, (List<RelativePerformanceData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Contributor/Detractor Gadget

        /// <summary>
        /// RetrieveRelativePerformanceSecurityData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSecurityDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSecurityData(portfolio, effectiveDate, period, (List<RelativePerformanceSecurityData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Relative Performance Security Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSecurityData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSecurityDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSecurityData(portfolio, effectiveDate, period, (List<RelativePerformanceSecurityData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Security Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSecurityData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// period - 1M
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSecurityDataSelectionDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSecurityData(portfolio, effectiveDate, period, (List<RelativePerformanceSecurityData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Security Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSecurityData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// period - 1M
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSecurityDataSelectionDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSecurityData(portfolio, effectiveDate, period, (List<RelativePerformanceSecurityData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Security Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Relative Performance Country Active Position Gadget

        /// <summary>
        /// RetrieveRelativePerformanceCountryActivePositionData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceCountryActivePositionDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceCountryActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Relative Performance Active Position Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceCountryActivePositionData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceCountryActivePositionDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceCountryActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceCountryActivePositionData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// period - 1M
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceCountryActivePositionDataSelectionDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceCountryActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceCountryActivePositionData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// period - 1M
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceCountryActivePositionData()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceCountryActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Relative Performance Sector Active Position Gadget

        /// <summary>
        /// RetrieveRelativePerformanceSectorActivePositionData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSectorActivePositionDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSectorActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Relative Performance Active Position Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSectorActivePositionData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSectorActivePositionDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSectorActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSectorActivePositionData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// period - 1M
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSectorActivePositionDataSelectionDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSectorActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSectorActivePositionData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// period - 1M
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSectorActivePositionData()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSectorActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }


        #endregion

        #region Relative Performance Security Active Position Gadget

        /// <summary>
        /// RetrieveRelativePerformanceSecurityActivePositionData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSecurityActivePositionDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSecurityActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Relative Performance Active Position Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSecurityActivePositionData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSecurityActivePositionDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSecurityActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSecurityActivePositionData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// period - 1M
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSecurityActivePositionDataSelectionDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSecurityActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceSecurityActivePositionData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("02 / 29 / 2012")
        /// period - 1M
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Relative Performance")]
        public void RetrieveRelativePerformanceSecurityActivePositionData()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String period = "1M";

            instance.RetrieveRelativePerformanceSecurityActivePositionData(portfolio, effectiveDate, period, (List<RelativePerformanceActivePositionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Relative Performance Active Position Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion
        #endregion

        #region Market Performance Snapshot

        #region RetrieveMarketSnapshotSelectionData
        /// <summary>
        /// RetrieveMarketSnapshotSelectionData Test Method - preference for userName exists  - should  return Empty set
        /// userName - rvig
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketSnapshotSelectionDataUserNamePreferenceExists()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.RetrieveMarketSnapshotSelectionData("rvig", (List<MarketSnapshotSelectionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketSnapshotSelectionData should not be null");
                Assert.AreNotEqual<int>(0, resultSet.Count, "MarketSnapshotSelectionData should not be empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketSnapshotSelectionData Test Method - no preference for the userName exista - should  return Empty set
        /// userName - sverma
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketSnapshotSelectionDataUserNamePreferenceNotExists()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.RetrieveMarketSnapshotSelectionData("sverma", (List<MarketSnapshotSelectionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketSnapshotSelectionData should not be null");
                Assert.AreEqual<int>(0, resultSet.Count, "MarketSnapshotSelectionData should be empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketSnapshotSelectionData Test Method - userName as null or Empty - should  return Empty set
        /// userName - null and empty
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketSnapshotSelectionDataUserNameNullOrEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.RetrieveMarketSnapshotSelectionData(null, (List<MarketSnapshotSelectionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketSnapshotSelectionData should not be null");
                Assert.AreEqual<int>(0, resultSet.Count, "MarketSnapshotSelectionData should be empty");
                instance.RetrieveMarketSnapshotSelectionData(String.Empty, (List<MarketSnapshotSelectionData> resultSet2) =>
                {
                    Assert.IsNotNull(resultSet, "MarketSnapshotSelectionData should not be null");
                    Assert.AreEqual<int>(0, resultSet2.Count, "MarketSnapshotSelectionData should be empty");
                    EnqueueTestComplete();
                });
            });
        } 
        #endregion

        #region RetrieveMarketSnapshotPreference
        /// <summary>
        /// RetrieveMarketSnapshotPreference Test Method - SnapshotId does not exists  - should  return Empty set
        /// SnapshotId - 1000
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketSnapshotPreferenceSnapshotIdNotExists()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.RetrieveMarketSnapshotPreference(1000, (List<MarketSnapshotPreference> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketSnapshotSelectionData should not be null");
                Assert.AreEqual<int>(0, resultSet.Count, "MarketSnapshotPreference should be empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketSnapshotPreference Test Method - SnapshotId exists without data - should  return Empty set
        /// SnapshotId - 71
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketSnapshotPreferenceSnapshotIdExistsNoPreference()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.RetrieveMarketSnapshotPreference(71, (List<MarketSnapshotPreference> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketSnapshotSelectionData should not be null");
                Assert.AreEqual<int>(0, resultSet.Count, "MarketSnapshotPreference should be empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketSnapshotPreference Test Method - SnapshotId exists without data - should  return Empty set
        /// SnapshotId - 71
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketSnapshotPreferenceSnapshotIdExistsWithPreference()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.RetrieveMarketSnapshotPreference(74, (List<MarketSnapshotPreference> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketSnapshotSelectionData should not be null");
                Assert.AreNotEqual<int>(0, resultSet.Count, "MarketSnapshotSelectionData should not be empty");
                EnqueueTestComplete();
            });
        } 
        #endregion

        #region RetrieveMarketPerformanceSnapshotData
        /// <summary>
        /// RetrieveMarketPerformanceSnapshotData Test Method - marketSnapshotPreference is null or empty  - should  return Empty set
        /// marketSnapshotPreference - Null and Empty
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketPerformanceSnapshotDataPreferenceIsNullOrEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.RetrieveMarketPerformanceSnapshotData(null, (List<MarketPerformanceSnapshotData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketPerformanceSnapshotData should not be null");
                Assert.AreEqual<int>(0, resultSet.Count, "MarketPerformanceSnapshotData should be empty");
                instance.RetrieveMarketPerformanceSnapshotData(new List<MarketSnapshotPreference>(), (List<MarketPerformanceSnapshotData> resultSet2) =>
                {
                    Assert.IsNotNull(resultSet2, "MarketPerformanceSnapshotData should not be null");
                    Assert.AreEqual<int>(0, resultSet2.Count, "MarketPerformanceSnapshotData should be empty");
                    EnqueueTestComplete();
                });
            });
        }

        /// <summary>
        /// RetrieveMarketPerformanceSnapshotData Test Method - marketSnapshotPreference Incorrect EntityName  - should  return null values
        /// marketSnapshotPreference - InvalidName
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketPerformanceSnapshotDataPreferenceInvalidName()
        {
            DBInteractivity instance = new DBInteractivity();
            List<MarketSnapshotPreference> marketPerformanceSnapshotData = new List<MarketSnapshotPreference>();
            marketPerformanceSnapshotData.Add(new MarketSnapshotPreference()
            {
                EntityName = "InvalidName",
                EntityOrder = 1,
                EntityPreferenceId = 1,
                EntityReturnType = "Price",
                EntityType = "SECURITY",
                GroupName = "Group1",
                GroupPreferenceID = 1
            });

            instance.RetrieveMarketPerformanceSnapshotData(marketPerformanceSnapshotData, (List<MarketPerformanceSnapshotData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketPerformanceSnapshotData should not be null");
                Assert.AreEqual(null, resultSet[0].DateToDateReturn, "DateToDateReturn should be null");
                Assert.AreEqual(null, resultSet[0].LastYearReturn, "LastYearReturn should be null");
                Assert.AreEqual(null, resultSet[0].MonthToDateReturn, "MonthToDateReturn should be null");
                Assert.AreEqual(null, resultSet[0].QuarterToDateReturn, "QuarterToDateReturn should be null");
                Assert.AreEqual(null, resultSet[0].SecondLastYearReturn, "SecondLastYearReturn should be null");
                Assert.AreEqual(null, resultSet[0].ThirdLastYearReturn, "ThirdLastYearReturn should be null");
                Assert.AreEqual(null, resultSet[0].WeekToDateReturn, "WeekToDateReturn should be null");
                Assert.AreEqual(null, resultSet[0].YearToDateReturn, "YearToDateReturn should be null");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketPerformanceSnapshotData Test Method - marketSnapshotPreference Incorrect ReturnType  - should  return null values
        /// marketSnapshotPreference - InvalidReturnType
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketPerformanceSnapshotDataPreferenceInvalidReturnType()
        {
            DBInteractivity instance = new DBInteractivity();
            List<MarketSnapshotPreference> marketPerformanceSnapshotData = new List<MarketSnapshotPreference>();
            marketPerformanceSnapshotData.Add(new MarketSnapshotPreference()
            {
                EntityName = "BOSHIWA INTERNATIONAL HOLDIN",
                EntityOrder = 1,
                EntityPreferenceId = 1,
                EntityReturnType = "InvalidReturnType",
                EntityType = "SECURITY",
                GroupName = "Group1",
                GroupPreferenceID = 1
            });

            instance.RetrieveMarketPerformanceSnapshotData(marketPerformanceSnapshotData, (List<MarketPerformanceSnapshotData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketPerformanceSnapshotData should not be null");
                Assert.AreEqual(null, resultSet[0].DateToDateReturn, "DateToDateReturn should be null");
                Assert.AreEqual(null, resultSet[0].LastYearReturn, "LastYearReturn should be null");
                Assert.AreEqual(null, resultSet[0].MonthToDateReturn, "MonthToDateReturn should be null");
                Assert.AreEqual(null, resultSet[0].QuarterToDateReturn, "QuarterToDateReturn should be null");
                Assert.AreEqual(null, resultSet[0].SecondLastYearReturn, "SecondLastYearReturn should be null");
                Assert.AreEqual(null, resultSet[0].ThirdLastYearReturn, "ThirdLastYearReturn should be null");
                Assert.AreEqual(null, resultSet[0].WeekToDateReturn, "WeekToDateReturn should be null");
                Assert.AreEqual(null, resultSet[0].YearToDateReturn, "YearToDateReturn should be null");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketPerformanceSnapshotData Test Method - marketSnapshotPreference Incorrect EntityType  - should  return Empty set
        /// marketSnapshotPreference - InvalidType
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketPerformanceSnapshotDataPreferenceInvalidType()
        {
            DBInteractivity instance = new DBInteractivity();
            List<MarketSnapshotPreference> marketPerformanceSnapshotData = new List<MarketSnapshotPreference>();
            marketPerformanceSnapshotData.Add(new MarketSnapshotPreference()
            {
                EntityName = "BOSHIWA INTERNATIONAL HOLDIN",
                EntityOrder = 1,
                EntityPreferenceId = 1,
                EntityReturnType = "Price",
                EntityType = "InvalidType",
                GroupName = "Group1",
                GroupPreferenceID = 1
            });

            instance.RetrieveMarketPerformanceSnapshotData(marketPerformanceSnapshotData, (List<MarketPerformanceSnapshotData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketPerformanceSnapshotData should not be null");
                Assert.AreEqual<int>(0, resultSet.Count, "MarketPerformanceSnapshotData should be empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMarketPerformanceSnapshotData Test Method - marketSnapshotPreference valid data  - should not return Empty set
        /// marketSnapshotPreference - InvalidType
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void RetrieveMarketPerformanceSnapshotDataPreferenceValidInput()
        {
            DBInteractivity instance = new DBInteractivity();
            List<MarketSnapshotPreference> marketPerformanceSnapshotData = new List<MarketSnapshotPreference>();
            marketPerformanceSnapshotData.Add(new MarketSnapshotPreference()
            {
                EntityName = "BOSHIWA INTERNATIONAL HOLDIN",
                EntityOrder = 1,
                EntityPreferenceId = 1,
                EntityReturnType = "Price",
                EntityType = "SECURITY",
                GroupName = "Group1",
                GroupPreferenceID = 1
            });

            instance.RetrieveMarketPerformanceSnapshotData(marketPerformanceSnapshotData, (List<MarketPerformanceSnapshotData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketPerformanceSnapshotData should not be null");
                Assert.AreNotEqual<int>(0, resultSet.Count, "MarketPerformanceSnapshotData should not be empty");
                EnqueueTestComplete();
            });
        } 
        #endregion

        #region SaveMarketSnapshotPreference
        /// <summary>
        /// SaveMarketSnapshotPreference Test Method - SnapshotPreferenceId does not exists  - should return null
        /// SnapshotPreferenceId - Null and Empty
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void SaveMarketSnapshotPreferenceSnapshotPreferenceIdNotExists()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.SaveMarketSnapshotPreference("<Root></Root>", (List<MarketSnapshotPreference> resultSet) =>
            {
                Assert.IsNull(resultSet, "MarketSnapshotPreference should be null");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// SaveMarketSnapshotPreference Test Method - updateXML xml format errors  - should return null
        /// updateXML - #Root##/Root#
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void SaveMarketSnapshotPreferenceUpdateXMLFormatErrors()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.SaveMarketSnapshotPreference("#Root##/Root#", (List<MarketSnapshotPreference> resultSet) =>
            {
                Assert.IsNull(resultSet, "MarketSnapshotPreference should be null");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// SaveMarketSnapshotPreference Test Method - updateXML is null or empty  - should return null
        /// updateXML - Null and Empty
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void SaveMarketSnapshotPreferenceUpdateXMLNullOrEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.SaveMarketSnapshotPreference(null, (List<MarketSnapshotPreference> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketSnapshotPreference should not be null");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// SaveMarketSnapshotPreference Test Method - valid data  - should not return null
        /// updateXML - #Root##/Root#
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void SaveMarketSnapshotPreferenceValidData()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.SaveMarketSnapshotPreference("<Root GroupPreferenceId=\"74\"></Root>", (List<MarketSnapshotPreference> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketSnapshotPreference should not be null");
                EnqueueTestComplete();
            });
        } 
        #endregion

        #region SaveAsMarketSnapshotPreference
        /// <summary>
        /// SaveAsMarketSnapshotPreference Test Method - updateXML is corrupt  - should return null
        /// updateXML - #root##/root#
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void SaveAsMarketSnapshotPreferenceupdateXMLFormatErrors()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.SaveAsMarketSnapshotPreference("#root##/root#", (PopulatedMarketPerformanceSnapshotData resultSet) =>
            {
                Assert.IsNull(resultSet, "PopulatedMarketPerformanceSnapshotData should be null");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// SaveAsMarketSnapshotPreference Test Method - updateXML is null or empty  - should create empty snapshot
        /// updateXML - Null and Empty
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void SaveAsMarketSnapshotPreferenceUpdateXMLNullOrEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.SaveAsMarketSnapshotPreference(null, (PopulatedMarketPerformanceSnapshotData resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.MarketPerformanceSnapshotInfo.Count, "MarketPerformanceSnapshotInfo should be empty");
                Assert.IsNotNull(resultSet.MarketSnapshotSelectionInfo, "MarketSnapshotSelectionInfo should not be null");
                instance.SaveAsMarketSnapshotPreference(String.Empty, (PopulatedMarketPerformanceSnapshotData resultSet2) =>
                {
                    Assert.AreEqual<int>(0, resultSet.MarketPerformanceSnapshotInfo.Count, "MarketPerformanceSnapshotInfo should be empty");
                    Assert.IsNotNull(resultSet.MarketSnapshotSelectionInfo, "MarketSnapshotSelectionInfo should not be null");
                    EnqueueTestComplete();
                });
            });
        }

        /// <summary>
        /// SaveAsMarketSnapshotPreference Test Method - valid data  - should return null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("MarketPerformanceSnapshot")]
        public void SaveAsMarketSnapshotPreferenceValidData()
        {
            DBInteractivity instance = new DBInteractivity();
            instance.SaveAsMarketSnapshotPreference("<root UserName=\"rvig\" SnapshotName=\"snapshot\"></root>", (PopulatedMarketPerformanceSnapshotData resultSet) =>
            {
                Assert.IsNotNull(resultSet, "PopulatedMarketPerformanceSnapshotData should not be null");
                EnqueueTestComplete();
            });
        }
        #endregion
        #endregion

        #region RelativePerformanceUI

        /// <summary>
        /// RetrieveRelativePerformanceUIData Test Method - selectedEntity as null - should  return an  empty set
        /// selectedEntity - null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RelativePerformanceUI")]
        public void RetrieveRelativePerformanceUIDataSelectedEntitiesNull()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = null;
            DateTime objEffectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveRelativePerformanceUIData(selectedEntites, objEffectiveDate, (List<RelativePerformanceUIData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "RelativePerformanceUIData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRelativePerformanceUIData Test Method - selectedEntity as empty - should  return an  empty set
        /// selectedEntity - null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RelativePerformanceUI")]
        public void RetrieveRelativePerformanceUIDataSelectedEntitiesEmpty()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            DateTime objEffectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveRelativePerformanceUIData(selectedEntites, objEffectiveDate, (List<RelativePerformanceUIData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "RelativePerformanceUIData should be Empty");
                EnqueueTestComplete();
            });
        }



        /// <summary>
        /// RetrieveRelativePerformanceUIData Test Method - selectedEntity as Dummy Values - should  return an  empty set
        /// selectedEntity - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RelativePerformanceUI")]
        public void RetrieveRelativePerformanceUIDataDummyValues()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            selectedEntites.Add("SECURITY", "ABC");
            selectedEntites.Add("PORTFOLIO", "XYZ");
            DateTime objEffectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveRelativePerformanceUIData(selectedEntites, objEffectiveDate, (List<RelativePerformanceUIData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "RelativePerformanceUIData should be Empty");
                EnqueueTestComplete();
            });
        }


        /// <summary>
        /// RetrieveRelativePerformanceUIData Test Method - selectedEntity as Sample Values - should not return null
        /// selectedEntity - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RelativePerformanceUI")]
        public void RetrieveRelativePerformanceUIDataSelectedEntitiesSampleData()
        {
            DBInteractivity instance = new DBInteractivity();
            Dictionary<string, string> selectedEntites = new Dictionary<string, string>();
            selectedEntites.Add("SECURITY", "AAF");
            selectedEntites.Add("PORTFOLIO", "AFRICA");
            DateTime objEffectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveRelativePerformanceUIData(selectedEntites, objEffectiveDate, (List<RelativePerformanceUIData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "RelativePerformanceUIData should not be null");
                EnqueueTestComplete();
            });
        }

        #endregion

        #endregion

        #region HoldingsPieChartforSector Gadget


        /// <summary>
        /// RetrieveHoldingsPercentageData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHoldingsPercentageDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrieveHoldingsPercentageData(portfolio, effectiveDate, filterType, filterValue,false, (List<HoldingsPercentageData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Holdings Pie chart for sector Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveHoldingsPercentageData Test Method - Sample Data Which does not retrieve any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHoldingsPercentageDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "UBEF" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrieveHoldingsPercentageData(portfolio, effectiveDate, filterType, filterValue,false, (List<HoldingsPercentageData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Holdings Pie chart for Sector Should Be Empty");
                EnqueueTestComplete();
            });
        }


        /// <summary>
        /// RetrieveHoldingsPercentageData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHoldingsPercentageDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrieveHoldingsPercentageData(portfolio, effectiveDate, filterType, filterValue,false, (List<HoldingsPercentageData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Holdings Pie chart for Sector Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveHoldingsPercentageData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHoldingsPercentageDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrieveHoldingsPercentageData(portfolio, effectiveDate, filterType, filterValue,false, (List<HoldingsPercentageData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Holdings Pie chart for Sector Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region HoldingsPieChartforRegion Gadget

        /// <summary>
        /// RetrieveHoldingsPercentageDataForRegion Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHoldingsPercentageDataForRegionTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrieveHoldingsPercentageDataForRegion(portfolio, effectiveDate, filterType, filterValue,false, (List<HoldingsPercentageData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Holdings Pie chart for Region Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveHoldingsPercentageDataForRegion Test Method - Sample Data Which does not retrieve any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHoldingsPercentageDataForRegionNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "UBEF" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrieveHoldingsPercentageDataForRegion(portfolio, effectiveDate, filterType, filterValue,false, (List<HoldingsPercentageData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Holdings Pie chart for Region Should Be Empty");
                EnqueueTestComplete();
            });
        }


        /// <summary>
        /// RetrieveHoldingsPercentageDataForRegion Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHoldingsPercentageDataForRegionPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrieveHoldingsPercentageDataForRegion(portfolio, effectiveDate, filterType, filterValue,false, (List<HoldingsPercentageData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Holdings Pie chart for Region Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveHoldingsPercentageDataForRegion Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHoldingsPercentageDataForRegionPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrieveHoldingsPercentageDataForRegion(portfolio, effectiveDate, filterType, filterValue,false, (List<HoldingsPercentageData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Holdings Pie chart for Region Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Attribution Gadget

        /// <summary>
        /// RetrieveAttributionData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveAttributionDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            String nodeName = "Country";
            DateTime effectiveDate = new DateTime(2012, 2, 29);

            instance.RetrieveAttributionData(portfolio, effectiveDate,nodeName, (List<AttributionData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "Attribution Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveAttributionData Test Method - Sample Data Which does not retrieve any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveAttributionDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "UBEF" };
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String nodeName = "Country";
            instance.RetrieveAttributionData(portfolio, effectiveDate,nodeName, (List<AttributionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Attribution Data Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveAttributionData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveAttributionDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String nodeName = "Country";
            instance.RetrieveAttributionData(portfolio, effectiveDate,nodeName, (List<AttributionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Attribution Data Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveAttributionData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveAttributionDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 2, 29);
            String nodeName = "Country";
            instance.RetrieveAttributionData(portfolio, effectiveDate,nodeName, (List<AttributionData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Attribution Data Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Performance Grid Gadget
        /// <summary>
        /// RetrievePerformanceGridData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrievePerformanceGridDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrievePerformanceGridData(portfolio, effectiveDate,"Colombia", (List<PerformanceGridData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "PerformanceGrid Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrievePerformanceGridData Test Method - Sample Data Which does not retrieve any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrievePerformanceGridDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "UBEF" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePerformanceGridData(portfolio, effectiveDate,"Colombia", (List<PerformanceGridData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PerformanceGrid Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrievePerformanceGridData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrievePerformanceGridDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrievePerformanceGridData(portfolio, effectiveDate,"Colombia", (List<PerformanceGridData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PerformanceGrid Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrievePerformanceGridData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrievePerformanceGridDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePerformanceGridData(portfolio, effectiveDate,"Colombia", (List<PerformanceGridData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PerformanceGrid Data Not Available");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Performance Graph Gadget
        /// <summary>
        /// RetrievePerformanceGraphData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrievePerformanceGraphDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrievePerformanceGraphData(portfolio, effectiveDate,"YTD", "Peru", (List<PerformanceGraphData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "PerformanceGraph Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrievePerformanceGraphData Test Method - Sample Data Which does not retrieve any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrievePerformanceGraphDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "UBEF" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePerformanceGraphData(portfolio, effectiveDate, "YTD", "Peru", (List<PerformanceGraphData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PerformanceGraph Data Not Available");
                EnqueueTestComplete();
            });
        }


        /// <summary>
        /// RetrievePerformanceGraphData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrievePerformanceGraphDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrievePerformanceGraphData(portfolio, effectiveDate, "YTD", "Peru", (List<PerformanceGraphData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PerformanceGraph Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrievePerformanceGraphData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrievePerformanceGraphDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePerformanceGraphData(portfolio, effectiveDate, "YTD", "Peru", (List<PerformanceGraphData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PerformanceGraph Data Not Available");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Top Ten Benchmark Securities Gadget
        /// <summary>
        /// RetrieveTopTenBenchmarkSecuritiesData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopTenBenchmarkDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveTopBenchmarkSecuritiesData(portfolio, effectiveDate, (List<TopBenchmarkSecuritiesData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "TopTenBenchmarkSecuritiesData Data Not Available");
                EnqueueTestComplete();
            });
        }


        /// <summary>
        /// RetrieveTopTenBenchmarkDataNotAvailable Test Method - Sample Data Which does not retrieve any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopTenBenchmarkDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "UBEF" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveTopBenchmarkSecuritiesData(portfolio, effectiveDate, (List<TopBenchmarkSecuritiesData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "TopTenBenchmarkSecuritiesData Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTopTenBenchmarkDataPortfolioIdentifierNull Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopTenBenchmarkDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveTopBenchmarkSecuritiesData(portfolio, effectiveDate, (List<TopBenchmarkSecuritiesData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "TopTenBenchmarkSecuritiesData Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTopTenBenchmarkDataPortfolioIdentifierEmptyTest Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopTenBenchmarkDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveTopBenchmarkSecuritiesData(portfolio, effectiveDate, (List<TopBenchmarkSecuritiesData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "TopTenBenchmarkSecuritiesData Data Not Available");
                EnqueueTestComplete();
            });
        }
       #endregion

        #region Heat Map Gadget
        /// <summary>
        /// RetrieveHeatMapData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHeatMapDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveHeatMapData(portfolio, effectiveDate,"YTD", (List<HeatMapData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "HeatMap Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveHeatMapDataNotAvailable Test Method - Sample Data Which does not retrieve any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHeatMapDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "UBEF" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveHeatMapData(portfolio, effectiveDate, "YTD", (List<HeatMapData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "HeatMap Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveHeatMapDataPortfolioIdentifierNull Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHeatMapDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveHeatMapData(portfolio, effectiveDate, "YTD", (List<HeatMapData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "HeatMap Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveHeatMapDataPortfolioIdentifierEmptyTest Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveHeatMapDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveHeatMapData(portfolio, effectiveDate, "YTD", (List<HeatMapData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "HeatMap Data Not Available");
                EnqueueTestComplete();
            });
        }       

        #endregion

        #region Portfolio Risk Return Gadget
        /// <summary>
        /// RetrieveRiskReturnData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveRiskReturnDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrievePortfolioRiskReturnData(portfolio, effectiveDate, (List<PortfolioRiskReturnData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "PortfolioRiskReturn Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRiskReturnDataNotAvailable Test Method - Sample Data Which does not retrieve any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveRiskReturnDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "UBEF" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePortfolioRiskReturnData(portfolio, effectiveDate, (List<PortfolioRiskReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PortfolioRiskReturn Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        ///RetrieveRiskReturnDataPortfolioIdentifierNull Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveRiskReturnDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrievePortfolioRiskReturnData(portfolio, effectiveDate, (List<PortfolioRiskReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PortfolioRiskReturn Data Not Available");
                EnqueueTestComplete();
            });
        }


        /// <summary>
        ///RetrieveRiskReturnDataPortfolioIdentifierEmptyTest Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveRiskReturnDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePortfolioRiskReturnData(portfolio, effectiveDate, (List<PortfolioRiskReturnData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PortfolioRiskReturn Data Not Available");
                EnqueueTestComplete();
            });
        }
        #endregion

       
        #endregion

        #region Commodity
        /// <summary>
        /// RetrieveCommodityData test method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Commodity")]
        public void RetrieveCommodityDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            string CommodityId = "ALL";
            instance.RetrieveCommodityData(CommodityId, (List<FXCommodityData> resultset) =>
                {
                    Assert.IsNotNull(resultset, "Commodity Data Not Available");
                    EnqueueTestComplete();
                });
        }
        /// <summary>
        /// RetrieveCommodityData test method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Commodity")]
        public void RetrieveCommodityDataNotAvailbleTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            string CommodityId = "ABC";
            instance.RetrieveCommodityData(CommodityId, (List<FXCommodityData> resultset) =>
            {
                Assert.IsNotNull(resultset, "Commodity data not available");
                EnqueueTestComplete();
            });
        }
        /// <summary>
        /// RetrieveCommodityData test method - CommodityId as null - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Commodity")]
        public void RetrieveCommodityDataCommodityIdNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            string CommodityId = null;
            instance.RetrieveCommodityData(CommodityId, (List<FXCommodityData> resultset) =>
            {
                Assert.AreEqual<int>(0,resultset.Count, "Commodity Data should be empty");
                EnqueueTestComplete();
            });
        }
        /// <summary>
        /// RetrieveCommodityData test method - CommodityId as Empty - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Commodity")]
        public void RetrieveCommodityDataCommodityIdEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            string CommodityId = string.Empty;
            instance.RetrieveCommodityData(CommodityId, (List<FXCommodityData> resultset) =>
            {
                Assert.AreEqual<int>(0, resultset.Count, "Commodity Data should be empty");
                EnqueueTestComplete();
            });
        }
        #endregion        

    }
}