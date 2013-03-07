using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.ServiceCaller.DCFDefinitions;
using GreenField.DataContracts.DataContracts;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;


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

            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<TopHoldingsData> resultSet) =>
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
            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<TopHoldingsData> resultSet) =>
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
            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<TopHoldingsData> resultSet) =>
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
            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<TopHoldingsData> resultSet) =>
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
            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate, islookthruenabled, (List<IndexConstituentsData> resultSet) =>
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
            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate, islookthruenabled, (List<IndexConstituentsData> resultSet) =>
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
            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate, islookthruenabled, (List<IndexConstituentsData> resultSet) =>
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
            instance.RetrieveSectorBreakdownData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<SectorBreakdownData> resultSet) =>
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
            instance.RetrieveSectorBreakdownData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<SectorBreakdownData> resultSet) =>
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
            instance.RetrieveSectorBreakdownData(portfolio, effectiveDate, isCashExclude, islookthruenabled, (List<SectorBreakdownData> resultSet) =>
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
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, filterType, filterValue,lookThru, excludeCash, getBenchmark, (List<PortfolioDetailsData> resultSet) =>
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
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, filterType, filterValue, lookThru, excludeCash, getBenchmark, (List<PortfolioDetailsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Portfolio Details Data should be Empty");
                EnqueueTestComplete();
            });
        }

        [TestMethod]
        [Asynchronous]
        [Tag("Max")]
        public void RetrievePortfolioDetailsDataPortfolioIdentifierActual()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData { PortfolioId = "EMIF" };
            bool lookThru = true;
            bool getBenchmark = false;
            bool excludeCash = false;
            DateTime effectiveDate = new DateTime(2012, 11, 30);
            XDocument doc = XDocument.Load("dataset.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<PortfolioDetailsData>));
            StringReader reader = new StringReader(doc.ToString());
            var array = (List<PortfolioDetailsData>)serializer.Deserialize(reader);
            var data = GetGroupedPortfolios(portfolio.PortfolioId, array);
            var i = 2;
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, filterType, filterValue, lookThru, excludeCash, getBenchmark, (List<PortfolioDetailsData> resultSet) =>
            {
                //XmlSerializer serializer = new XmlSerializer(typeof(List<PortfolioDetailsData>));
                MemoryStream ms = new MemoryStream();
                StringBuilder sb = new StringBuilder();
                using (var streamWriter = new StringWriter(sb))
                {
                    serializer.Serialize(streamWriter, resultSet);
                }
                var xml = sb.ToString();


                Assert.AreNotEqual<int>(0, resultSet.Count, "Portfolio Details Data shouldn't be Empty");
                EnqueueTestComplete();
            });
        }

        private List<PortfolioDetailsData> GetGroupedPortfolios(string portfolioId, List<PortfolioDetailsData> list)
        {
            var result = new List<PortfolioDetailsData>();
            var query = from d in list
                        //where d.Ticker == "2018 HK"
                        group d by d.AsecSecShortName into grp
                        select grp;
            var groups = query.ToList();

            foreach (var group in groups)
            {
                var main = group.Where(x => x.PortfolioPath == portfolioId).FirstOrDefault();
                if (main == null)
                {
                    result.AddRange(group.AsEnumerable());
                }
                else
                {
                    var holding = new PortfolioDetailsData
                    {
                        A_Sec_Instr_Type = group.First().A_Sec_Instr_Type,
                        ActivePosition = group.Sum(x => x.ActivePosition ?? 0.0m),
                        AsecSecShortName = group.Key,
                        AshEmmModelWeight = group.Sum(x => x.AshEmmModelWeight ?? 0.0m),
                        BalanceNominal = group.Sum(x => x.BalanceNominal ?? 0.0m),
                        BenchmarkWeight = main.BenchmarkWeight,
                        DirtyValuePC = group.Sum(x => x.DirtyValuePC ?? 0.0m),
                        ForwardEB_EBITDA = main.ForwardEB_EBITDA,
                        ForwardPE = main.ForwardPE,
                        ForwardPBV = main.ForwardPBV,
                        FreecashFlowMargin = main.FreecashFlowMargin,
                        FromDate = main.FromDate,
                        IndustryName = main.IndustryName,
                        IsoCountryCode = main.IsoCountryCode,
                        IssueName = main.IssueName,
                        IssuerId = main.IssuerId,
                        MarketCap = main.MarketCap,
                        MarketCapUSD = main.MarketCapUSD,
                        NetDebtEquity = main.NetDebtEquity,
                        NetIncomeGrowthCurrentYear = main.NetIncomeGrowthCurrentYear,
                        NetIncomeGrowthNextYear = main.NetIncomeGrowthNextYear,
                        PfcHoldingPortfolio = main.PfcHoldingPortfolio,
                        PortfolioDirtyValuePC = group.Sum(x => x.PortfolioDirtyValuePC),
                        PortfolioPath = main.PortfolioPath,
                        PortfolioWeight = group.Sum(x => x.PortfolioWeight ?? 0.0m),
                        ProprietaryRegionCode = main.ProprietaryRegionCode,
                        ReAshEmmModelWeight = group.Sum(x => x.ReAshEmmModelWeight ?? 0.0m),
                        RePortfolioWeight = group.Sum(x => x.RePortfolioWeight ?? 0.0m),
                        ReBenchmarkWeight = main.ReBenchmarkWeight,
                        RevenueGrowthCurrentYear = main.RevenueGrowthCurrentYear,
                        RevenueGrowthNextYear = main.RevenueGrowthNextYear,
                        ROE = main.ROE,
                        SectorName = main.SectorName,
                        SecurityId = main.SecurityId,
                        SecurityThemeCode = main.SecurityThemeCode,
                        SecurityType = main.SecurityType,
                        SubIndustryName = main.SubIndustryName,
                        Ticker = main.Ticker,
                        TradingCurrency = main.TradingCurrency,
                        Type = main.Type,
                        Upside = main.Upside,
                        IsExpanded = true

                    };
                    result.Add(holding);
                }
            }

            return result;
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
            String filterType = "Region";
            String filterValue = "AFRICA";
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, filterType,filterValue,lookThru, excludeCash, getBenchamrk, (List<PortfolioDetailsData> resultSet) =>
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
            instance.RetrieveMarketSnapshotPerformanceData(null, (List<MarketSnapshotPerformanceData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MarketPerformanceSnapshotData should not be null");
                Assert.AreEqual<int>(0, resultSet.Count, "MarketPerformanceSnapshotData should be empty");
                instance.RetrieveMarketSnapshotPerformanceData(new List<MarketSnapshotPreference>(), (List<MarketSnapshotPerformanceData> resultSet2) =>
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

            instance.RetrieveMarketSnapshotPerformanceData(marketPerformanceSnapshotData, (List<MarketSnapshotPerformanceData> resultSet) =>
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

            instance.RetrieveMarketSnapshotPerformanceData(marketPerformanceSnapshotData, (List<MarketSnapshotPerformanceData> resultSet) =>
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

            instance.RetrieveMarketSnapshotPerformanceData(marketPerformanceSnapshotData, (List<MarketSnapshotPerformanceData> resultSet) =>
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

            instance.RetrieveMarketSnapshotPerformanceData(marketPerformanceSnapshotData, (List<MarketSnapshotPerformanceData> resultSet) =>
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
            instance.SaveAsMarketSnapshotPreference("#root##/root#", (PopulatedMarketSnapshotPerformanceData resultSet) =>
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
            instance.SaveAsMarketSnapshotPreference(null, (PopulatedMarketSnapshotPerformanceData resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.MarketPerformanceSnapshotInfo.Count, "MarketPerformanceSnapshotInfo should be empty");
                Assert.IsNotNull(resultSet.MarketSnapshotSelectionInfo, "MarketSnapshotSelectionInfo should not be null");
                instance.SaveAsMarketSnapshotPreference(String.Empty, (PopulatedMarketSnapshotPerformanceData resultSet2) =>
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
            instance.SaveAsMarketSnapshotPreference("<root UserName=\"rvig\" SnapshotName=\"snapshot\"></root>", (PopulatedMarketSnapshotPerformanceData resultSet) =>
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
            instance.RetrieveHoldingsPercentageData(portfolio, effectiveDate, filterType, filterValue, false, (List<HoldingsPercentageData> resultSet) =>
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
            instance.RetrieveHoldingsPercentageData(portfolio, effectiveDate, filterType, filterValue, false, (List<HoldingsPercentageData> resultSet) =>
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
            instance.RetrieveHoldingsPercentageData(portfolio, effectiveDate, filterType, filterValue, false, (List<HoldingsPercentageData> resultSet) =>
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
            instance.RetrieveHoldingsPercentageData(portfolio, effectiveDate, filterType, filterValue, false, (List<HoldingsPercentageData> resultSet) =>
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
            instance.RetrieveHoldingsPercentageDataForRegion(portfolio, effectiveDate, filterType, filterValue, false, (List<HoldingsPercentageData> resultSet) =>
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
            instance.RetrieveHoldingsPercentageDataForRegion(portfolio, effectiveDate, filterType, filterValue, false, (List<HoldingsPercentageData> resultSet) =>
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
            instance.RetrieveHoldingsPercentageDataForRegion(portfolio, effectiveDate, filterType, filterValue, false, (List<HoldingsPercentageData> resultSet) =>
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
            instance.RetrieveHoldingsPercentageDataForRegion(portfolio, effectiveDate, filterType, filterValue, false, (List<HoldingsPercentageData> resultSet) =>
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

            instance.RetrieveAttributionData(portfolio, effectiveDate, nodeName, (List<AttributionData> resultSet) =>
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
            instance.RetrieveAttributionData(portfolio, effectiveDate, nodeName, (List<AttributionData> resultSet) =>
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
            instance.RetrieveAttributionData(portfolio, effectiveDate, nodeName, (List<AttributionData> resultSet) =>
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
            instance.RetrieveAttributionData(portfolio, effectiveDate, nodeName, (List<AttributionData> resultSet) =>
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

            instance.RetrievePerformanceGridData(portfolio, effectiveDate, "Colombia", (List<PerformanceGridData> resultSet) =>
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
            instance.RetrievePerformanceGridData(portfolio, effectiveDate, "Colombia", (List<PerformanceGridData> resultSet) =>
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

            instance.RetrievePerformanceGridData(portfolio, effectiveDate, "Colombia", (List<PerformanceGridData> resultSet) =>
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
            instance.RetrievePerformanceGridData(portfolio, effectiveDate, "Colombia", (List<PerformanceGridData> resultSet) =>
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

            instance.RetrievePerformanceGraphData(portfolio, effectiveDate, "YTD", "Peru", (List<PerformanceGraphData> resultSet) =>
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

            instance.RetrieveHeatMapData(portfolio, effectiveDate, "YTD", (List<HeatMapData> resultSet) =>
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

        #region Risk Index Exposures Gadget
        /// <summary>
        /// RetrieveRiskIndexExposuresData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RiskIndexExposures")]
        public void RetrieveRiskIndexExposuresDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            string filterType = "Sector";
            string filterValue = "Utilities";
            instance.RetrieveRiskIndexExposuresData(portfolio, effectiveDate, isCashExclude, islookthruenabled, filterType, filterValue,
                (List<RiskIndexExposuresData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "RiskIndexExposures Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRiskIndexExposuresData Test Method - Sample Data Which Does Not Retrieves Any Data - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RiskIndexExposures")]
        public void RetrieveRiskIndexExposuresDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABC" };
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            string filterType = "Sector";
            string filterValue = "XYZ";
            instance.RetrieveRiskIndexExposuresData(portfolio, effectiveDate, isCashExclude, islookthruenabled, filterType, filterValue,
                (List<RiskIndexExposuresData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "RiskIndexExposures Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRiskIndexExposuresData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RiskIndexExposures")]
        public void RetrieveRiskIndexExposuresDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            string filterType = "Sector";
            string filterValue = "Utilities";
            instance.RetrieveRiskIndexExposuresData(portfolio, effectiveDate, isCashExclude, islookthruenabled, filterType, filterValue,
                (List<RiskIndexExposuresData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "RiskIndexExposures Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveRiskIndexExposuresData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RiskIndexExposures")]
        public void RetrieveRiskIndexExposuresDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            bool isCashExclude = false;
            bool islookthruenabled = false;
            string filterType = "Sector";
            string filterValue = "Utilities";
            instance.RetrieveRiskIndexExposuresData(portfolio, effectiveDate, isCashExclude, islookthruenabled, filterType, filterValue,
                (List<RiskIndexExposuresData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "RiskIndexExposures Should Be Empty");
                EnqueueTestComplete();
            });
        }
        #endregion

        #region Macro Database Key Annual Report Data
        /// <summary>
        /// RetrieveMacroDatabaseKeyAnnualReportData Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMacroDatabaseKeyAnnualReportDataTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            String countryCode = "RU";
            instance.RetrieveMacroDatabaseKeyAnnualReportData(countryCode, (List<MacroDatabaseKeyAnnualReportData> resultSet) =>
            {
                Assert.IsNotNull(resultSet, "MacroDatabaseKeyAnnualReportData Data Not Available");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveMacroDatabaseKeyAnnualReportData Test Method - Sample Data Which does not retrieve any Data -
        /// should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMacroDatabaseKeyAnnualReportDataNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            String countryCode = "DUMMY";
            instance.RetrieveMacroDatabaseKeyAnnualReportData(countryCode,
                (List<MacroDatabaseKeyAnnualReportData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "MacroDatabaseKeyAnnualReportData Data Not Available");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveMacroDatabaseKeyAnnualReportData Null Test Method - currency code as null - 
        /// should return an empty result set        
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMacroDatabaseKeyAnnualReportDataNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            String countryCode = null;
            instance.RetrieveMacroDatabaseKeyAnnualReportData(countryCode,
                (List<MacroDatabaseKeyAnnualReportData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "MacroDatabaseKeyAnnualReportData Data Not Available");
                    EnqueueTestComplete();
                });
        }


        #endregion

        #region Macro Database Key Annual Report Data EM Summary
        /// <summary>
        /// RetrieveMacroDatabaseKeyAnnualReportDataEMSummary Test Method - Sample Data
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMacroDatabaseKeyAnnualReportDataEMSummaryTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            String countryCode = "RU";
            List<String> countryValues = new List<string> { "BR", "RU" };
            instance.RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(countryCode, countryValues,
                (List<MacroDatabaseKeyAnnualReportData> resultSet) =>
                {
                    Assert.IsNotNull(resultSet, "MacroDatabaseKeyAnnualReportDataEMSummary Data Not Available");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveMacroDatabaseKeyAnnualReportDataEMSummary Test Method - Sample Data Which does not retrieve any Data -
        /// should return an empty result set
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMacroDatabaseKeyAnnualReportDataEMSummaryNotAvailableTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            String countryCode = "A";
            List<String> countryValues = new List<string> { "B", "C" };
            instance.RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(countryCode, countryValues,
                (List<MacroDatabaseKeyAnnualReportData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "MacroDatabaseKeyAnnualReportDataEMSummary Data Not Available");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveMacroDatabaseKeyAnnualReportDataEMSummary Null Test Method - COUNTRY VALUES as null - 
        /// should return an empty result set        
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveMacroDatabaseKeyAnnualReportDataEMSummaryNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            String countryCode = null;
            List<String> countryValues = null;
            instance.RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(countryCode, countryValues,
                (List<MacroDatabaseKeyAnnualReportData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "MacroDatabaseKeyAnnualReportDataEMSummary Data Not Available");
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
                Assert.AreEqual<int>(0, resultset.Count, "Commodity Data should be empty");
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

        #region ExternalResearch

        #region ConsensusGadgets

        /// <summary>
        /// RetrieveTargetPriceData Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Consensus")]
        public void GetTargetPriceDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;

            instance.RetrieveTargetPriceData(entitySelectionData, (List<TargetPriceCEData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "TargetPriceCEData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTargetPriceData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Consensus")]
        public void GetTargetPriceDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.RetrieveTargetPriceData(entitySelectionData, (List<TargetPriceCEData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "TargetPriceCEData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveConsensusEstimatesMedianData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Consensus")]
        public void RetrieveConsensusEstimatesMedianDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            string issuerId = null;
            FinancialStatementPeriodType periodType = FinancialStatementPeriodType.ANNUAL;
            string currency = null;
            instance.RetrieveConsensusEstimatesMedianData(issuerId, periodType, currency, (List<ConsensusEstimateMedian> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ConsensusEstimateMedian should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveConsensusEstimatesMedianData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Consensus")]
        public void RetrieveConsensusEstimatesMedianDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            string issuerId = "A";
            FinancialStatementPeriodType periodType = FinancialStatementPeriodType.ANNUAL;
            string currency = "X";
            instance.RetrieveConsensusEstimatesMedianData(issuerId, periodType, currency, (List<ConsensusEstimateMedian> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ConsensusEstimateMedian should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveConsensusEstimatesValuationsData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Consensus")]
        public void RetrieveConsensusEstimatesValuationDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            string issuerId = null;
            FinancialStatementPeriodType periodType = FinancialStatementPeriodType.ANNUAL;
            string currency = null;
            string longName = null;
            instance.RetrieveConsensusEstimatesValuationsData(issuerId, longName, periodType, currency, (List<ConsensusEstimatesValuations> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ConsensusEstimateMedian should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveConsensusEstimatesValuationsData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Consensus")]
        public void RetrieveConsensusEstimatesValuationDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            string issuerId = "A";
            FinancialStatementPeriodType periodType = FinancialStatementPeriodType.ANNUAL;
            string currency = "X";
            string longName = "A";
            instance.RetrieveConsensusEstimatesValuationsData(issuerId, longName, periodType, currency, (List<ConsensusEstimatesValuations> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ConsensusEstimateMedian should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveConsensusEstimateDetailedData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Consensus")]
        public void RetrieveConsensusEstimateDetailedDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            string issuerId = null;
            FinancialStatementPeriodType periodType = FinancialStatementPeriodType.ANNUAL;
            string currency = null;
            instance.RetrieveConsensusEstimateDetailedData(issuerId, periodType, currency, (List<ConsensusEstimateDetail> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ConsensusEstimateDetail should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveConsensusEstimateDetailData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Consensus")]
        public void RetrieveConsensusEstimateDetailDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            string issuerId = "A";
            FinancialStatementPeriodType periodType = FinancialStatementPeriodType.ANNUAL;
            string currency = "X";
            instance.RetrieveConsensusEstimateDetailedData(issuerId, periodType, currency, (List<ConsensusEstimateDetail> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "ConsensusEstimateDetail should be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region Finstat Gadget

        /// <summary>
        /// RetrieveFinstatData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Finstat")]
        public void RetrieveFinstatDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            string issuerId = null;
            string securityId = null;
            FinancialStatementDataSource dataSource = FinancialStatementDataSource.REUTERS;
            FinancialStatementFiscalType fiscalType = FinancialStatementFiscalType.FISCAL;
            string currency = null;
            Int32 yearRangeStart = 0;
            instance.RetrieveFinstatDetailData(issuerId, securityId, dataSource, fiscalType, currency, yearRangeStart,
                (List<FinstatDetailData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "FinstatDetailData should be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveFinstatData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Finstat")]
        public void RetrieveFinstatDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            string issuerId = "A";
            string securityId = "B";
            FinancialStatementDataSource dataSource = FinancialStatementDataSource.REUTERS;
            FinancialStatementFiscalType fiscalType = FinancialStatementFiscalType.FISCAL;
            string currency = "X";
            Int32 yearRangeStart = 2009;
            instance.RetrieveFinstatDetailData(issuerId, securityId, dataSource, fiscalType, currency, yearRangeStart,
                (List<FinstatDetailData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "FinstatDetailData should be Empty");
                    EnqueueTestComplete();
                });
        } 

        #endregion

        #region Consensus Estimates Summary Gadget

        /// <summary>
        /// RetrieveConsensusEstimatesSummaryData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Comparison To Consensus")]
        public void RetrieveConsensusSummaryDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entityIdentifier = new EntitySelectionData { InstrumentID = null };
            instance.RetrieveConsensusEstimatesSummaryData(entityIdentifier,
                (List<ConsensusEstimatesSummaryData> resultSet) =>
                {
                    Assert.AreEqual<int>(7, resultSet.Count, "ConsensusSummaryData should be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveConsensusEstimatesSummaryData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Comparison To Consensus")]
        public void RetrieveConsensusSummaryDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entityIdentifier = new EntitySelectionData { InstrumentID = "Dummy" };
            instance.RetrieveConsensusEstimatesSummaryData(entityIdentifier,
                (List<ConsensusEstimatesSummaryData> resultSet) =>
                {
                    Assert.AreEqual<int>(7, resultSet.Count, "ConsensusSummaryData should be Empty");
                    EnqueueTestComplete();
                });
        }

        #endregion

        #region Valuation Quality Growth

        /// <summary>
        /// RetrieveValuationQualityGrowthData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Valuation Quality Growth")]
        public void RetrieveValuationQualityGrowthDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData selectedPortfolio = new PortfolioSelectionData { PortfolioId = null };
            DateTime? effectiveDate = Convert.ToDateTime("05/31/2012");
            String filterType = "Region";
            String filterValue = "Latin";
            bool lookThruEnabled = true;
            instance.RetrieveValuationGrowthData(selectedPortfolio, effectiveDate, filterType, filterValue, lookThruEnabled,
                (List<ValuationQualityGrowthData> resultSet) =>
                {
                    Assert.AreEqual<int>(9, resultSet.Count, "ValuationQualityGrowthData should be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveValuationQualityGrowthData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Valuation Quality Growth")]
        public void RetrieveValuationQualityGrowthDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData selectedPortfolio = new PortfolioSelectionData { PortfolioId = "AB" };
            DateTime? effectiveDate = Convert.ToDateTime("05/31/2012");
            String filterType = "R";
            String filterValue = "L";
            bool lookThruEnabled = true;
            instance.RetrieveValuationGrowthData(selectedPortfolio, effectiveDate, filterType, filterValue, lookThruEnabled,
                (List<ValuationQualityGrowthData> resultSet) =>
                {
                    Assert.AreEqual<int>(9, resultSet.Count, "ValuationQualityGrowthData should be Empty");
                    EnqueueTestComplete();
                });
        }

        #endregion

        #region Quarterly Results Comparison
        /// <summary>
        /// RetrieveQuarterlyResultsComparisonData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Quarterly Results Comparison")]
        public void RetrieveQuarterlyResultsComparisonDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            String fieldValue = null;
            int yearValue = 0;
            instance.RetrieveQuarterlyResultsData(fieldValue, yearValue,
                (List<QuarterlyResultsData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "QuarterlyResultsComparisonData should be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveQuarterlyResultsComparisonData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Quarterly Results Comparison")]
        public void RetrieveQuarterlyResultsComparisonDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            String fieldValue = "abc";
            int yearValue = 9000;
            instance.RetrieveQuarterlyResultsData(fieldValue, yearValue,
                (List<QuarterlyResultsData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "QuarterlyResultsComparisonData should be Empty");
                    EnqueueTestComplete();
                });
        }
        #endregion       

        #region Gadget With Period Columns
        /// <summary>
        /// RetrieveCOASpecificData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Gadget With Period Columns")]
        public void RetrieveCOASpecificDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            String issuerId = null;
            int? securityId = 0;
            FinancialStatementDataSource cSource = FinancialStatementDataSource.INDUSTRY;
            FinancialStatementFiscalType cFiscalType = FinancialStatementFiscalType.CALENDAR;
            String cCurrency = null;
            instance.RetrieveCOASpecificData(issuerId, securityId, cSource, cFiscalType, cCurrency,
                (List<COASpecificData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "COASpecificData should be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveQuarterlyResultsComparisonData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Gadget With Period Columns")]
        public void RetrieveCOASpecificDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            String issuerId = "Dum";
            int? securityId = 0;
            FinancialStatementDataSource cSource = FinancialStatementDataSource.INDUSTRY;
            FinancialStatementFiscalType cFiscalType = FinancialStatementFiscalType.CALENDAR;
            String cCurrency = "opmn";
            instance.RetrieveCOASpecificData(issuerId, securityId, cSource, cFiscalType, cCurrency,
                (List<COASpecificData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "COASpecificData should be Empty");
                    EnqueueTestComplete();
                });
        }
        #endregion

        #region Summary Of EM Data
        /// <summary>
        /// RetrieveEMMarketData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Summary Of EM Data")]
        public void RetrieveEMMarketDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            String selectedPortfolio = null;
            instance.RetrieveEMSummaryMarketData(selectedPortfolio,
                (List<EMSummaryMarketData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "EMMarketData should be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveEMMarketData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Summary Of EM Data")]
        public void RetrieveEMMarketDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            String selectedPortfolio = "Dum";
            instance.RetrieveEMSummaryMarketData(selectedPortfolio,
                (List<EMSummaryMarketData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "EMMarketData should be Empty");
                    EnqueueTestComplete();
                });
        }
        #endregion

        #endregion

        #region DCF

        /// <summary>
        /// RetrieveDCFAnalysisDataDummy Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveDCFAnalysisDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;
            instance.RetrieveDCFAnalysisData(entitySelectionData, (List<DCFAnalysisSummaryData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "DCFAnalysisSummaryData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveDCFAnalysisDataDummy Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveDCFAnalysisDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.RetrieveDCFAnalysisData(entitySelectionData, (List<DCFAnalysisSummaryData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "DCFAnalysisSummaryData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTerminalValueCalculationsData Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveTerminalValueCalculationsDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;
            instance.RetrieveDCFTerminalValueCalculationsData(entitySelectionData, (List<DCFTerminalValueCalculationsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "DCFAnalysisSummaryData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTerminalValueCalculationsData Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveTerminalValueCalculationsDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.RetrieveDCFTerminalValueCalculationsData(entitySelectionData, (List<DCFTerminalValueCalculationsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "DCFAnalysisSummaryData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveCashFlows Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveCashFlowsDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;
            instance.RetrieveCashFlows(entitySelectionData, (List<DCFCashFlowData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "DCFCashFlowData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveCashFlows Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveCashFlowsDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.RetrieveCashFlows(entitySelectionData, (List<DCFCashFlowData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "DCFCashFlowData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveDCFSummaryData Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveDCFSummaryDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;
            instance.RetrieveDCFSummaryData(entitySelectionData, (List<DCFSummaryData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "DCFSummaryData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveDCFSummaryData Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveDCFSummaryDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.RetrieveDCFSummaryData(entitySelectionData, (List<DCFSummaryData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "DCFSummaryData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveCurrentPriceData Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveCurrentPriceDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;
            instance.RetrieveDCFCurrentPrice(entitySelectionData, (decimal? resultSet) =>
            {
                Assert.AreEqual<decimal?>(0, resultSet, "Price should be 0");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveCurrentPriceData Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveCurrentPriceDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.RetrieveDCFCurrentPrice(entitySelectionData, (decimal? resultSet) =>
            {
                Assert.AreEqual<decimal?>(0, resultSet, "Price should be 0");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// FetchDCFCountryName Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveCountryNameNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;
            instance.FetchDCFCountryName(entitySelectionData, (string resultSet) =>
            {
                Assert.AreEqual<string>(string.Empty, resultSet, "Country should be Blank");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// FetchDCFCountryName Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveCountryNameDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.FetchDCFCountryName(entitySelectionData, (string resultSet) =>
            {
                Assert.AreEqual<string>(string.Empty, resultSet, "Country should be Blank");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// FetchDCFCountryName Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveDCFFairValueDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;
            instance.RetrieveDCFFairValueData(entitySelectionData, (List<PERIOD_FINANCIALS> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PERIOD_FINANCIALS should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// FetchDCFCountryName Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void RetrieveDCFFairValueDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.RetrieveDCFFairValueData(entitySelectionData, (List<PERIOD_FINANCIALS> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "PERIOD_FINANCIALS should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// InsertDCFFairValueData Test Method - Null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void InsertDCFFairValueDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = null;
            string valueType = string.Empty;
            int? fvMeasure = null;
            decimal? fvbuy = null;
            decimal? fvSell = null;
            decimal? currentMeasureValue = null;
            decimal? upside = null;
            DateTime? updated = null;
            instance.InsertDCFFairValueData(entitySelectionData, valueType, fvMeasure, fvbuy, fvSell, currentMeasureValue, upside, updated, (bool resultSet) =>
            {
                Assert.AreEqual<bool>(false, resultSet, "False should be returned");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// InsertDCFFairValueData Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void InsertDCFFairValueDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            string valueType = "A";
            int? fvMeasure = 1;
            decimal? fvbuy = 1;
            decimal? fvSell = 1;
            decimal? currentMeasureValue = 1;
            decimal? upside = 1;
            DateTime? updated = DateTime.Now;
            instance.InsertDCFFairValueData(entitySelectionData, valueType, fvMeasure, fvbuy, fvSell, currentMeasureValue, upside, updated, (bool resultSet) =>
            {
                Assert.AreEqual<bool>(false, resultSet, "False should be returned");
                EnqueueTestComplete();
            });
        }

        

        /// <summary>
        /// DeleteDCFFairValue Test Method - Dummy
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("DCF")]
        public void DeleteDCFFairValueDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
            instance.DeleteDCFFairValue(entitySelectionData, (bool resultSet) =>
            {
                Assert.AreEqual<bool>(false, resultSet, "False should be returned");
                EnqueueTestComplete();
            });
        }

        #endregion

        #region ExcelModel

        ///// <summary>
        ///// RetrieveDocumentsData - null Check
        ///// </summary>
        //[TestMethod]
        //[Asynchronous]
        //[Tag("ExcelModel")]
        //public void RetrieveStatementData()
        //{
        //    DBInteractivity instance = new DBInteractivity();
        //    EntitySelectionData entitySelectionData = new EntitySelectionData() { InstrumentID = "A", LongName = "B", SecurityType = "A", ShortName = "C", SortOrder = 1, Type = "A" };
        //    instance.RetrieveDocumentsData(entitySelectionData, (byte[] resultSet) =>
        //    {
        //        Assert.AreEqual<byte[]>(byte[1], resultSet, "False should be returned");
        //        EnqueueTestComplete();
        //    });
        //}

        #endregion

        #region Custom Screening Tool

        #region Composite Fund Gadget

        /// <summary>
        /// RetrieveCompositeFundData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Composite Fund")]
        public void RetrieveCompositeFundDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entityIdentifiers = null;
            PortfolioSelectionData portfolio = null;
            instance.RetrieveCompositeFundData(entityIdentifiers, portfolio, (List<CompositeFundData> resultSet) =>
                {
                    Assert.AreEqual<int>(0, resultSet.Count, "CompositeFundData should be Empty");
                    EnqueueTestComplete();
                });
        }

        /// <summary>
        /// RetrieveCompositeFundData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Composite Fund")]
        public void RetrieveCompositeFundDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData entityIdentifiers = new EntitySelectionData()
            {
                InstrumentID = "BRPETROBRE",
                LongName = "PETROBRAS - PETROLEO BRAS",
                ShortName = "PETR3 BZ",
                SecurityType = "EQUITY"
            };
            PortfolioSelectionData portfolio = new PortfolioSelectionData() { PortfolioId = "ABPEQ" };
            instance.RetrieveCompositeFundData(entityIdentifiers, portfolio, (List<CompositeFundData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "CompositeFundData should be Empty");
                EnqueueTestComplete();
            });
        } 

        #endregion

        #region Custom Screening Tool

        /// <summary>
        /// RetrieveSecurityData Test Method - Null Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Custom Screening Tool")]
        public void RetrieveSecurityDataNull()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData benchmark = null;
            PortfolioSelectionData portfolio = null;
            String region = null;
            String country = null;
            String sector = null;
            String industry = null;
            List<CSTUserPreferenceInfo> userPreference = null;
            instance.RetrieveSecurityData(portfolio, benchmark, region, country, sector, industry, userPreference, (List<CustomScreeningSecurityData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "CustomScreeningToolData should be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveSecurityData Test Method - Dummy Values
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("Custom Screening Tool")]
        public void RetrieveSecurityDataDummy()
        {
            DBInteractivity instance = new DBInteractivity();
            EntitySelectionData benchmark = new EntitySelectionData()
            {
                InstrumentID = "BRPETROBRE",
                LongName = "PETROBRAS - PETROLEO BRAS",
                ShortName = "PETR3 BZ",
                SecurityType = "EQUITY"
            };
            PortfolioSelectionData portfolio = null;
            String region = null;
            String country = null;
            String sector = null;
            String industry = null;
            List<CSTUserPreferenceInfo> userPreference = new List<CSTUserPreferenceInfo>();
            userPreference.Add(new CSTUserPreferenceInfo()
            {
                UserName = "ABC",
                ListName = "XYZ",
                Accessibility = "Private",
                ScreeningId = "REF017",
            });
            instance.RetrieveSecurityData(portfolio, benchmark, region, country, sector, industry, userPreference, (List<CustomScreeningSecurityData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "CustomScreeningToolData should be Empty");
                EnqueueTestComplete();
            });
        } 

        #endregion

        #endregion
    }
}