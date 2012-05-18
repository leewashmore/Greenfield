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
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, (List<TopHoldingsData> resultSet) =>
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

            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, (List<TopHoldingsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Top 10 Holdings Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTopHoldingsData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopHoldingsDataSelectionDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, (List<TopHoldingsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Top 10 Holdings Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveTopHoldingsData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveTopHoldingsDataSelectionDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveTopHoldingsData(portfolio, effectiveDate, (List<TopHoldingsData> resultSet) =>
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

            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate, (List<IndexConstituentsData> resultSet) =>
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

            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate, (List<IndexConstituentsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Index constituent Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveIndexConstituentData Test Method - portfolioIdentifiers as null - should return an empty result set
        /// portfolioIdentifiers - null
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveIndexConstituentDataPortfolioIdentifierNullTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = null;
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate, (List<IndexConstituentsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Index constituent Should Be Empty");
                EnqueueTestComplete();
            });
        }

        /// <summary>
        /// RetrieveIndexConstituentData Test Method - portfolioIdentifiers as Empty - should return an empty result set
        /// portfolioIdentifiers - Empty
        /// effectiveDate - Convert.ToDateTime("01 / 31 / 2012")
        /// </summary>
        [TestMethod]
        [Asynchronous]
        public void RetrieveIndexConstituentDataPortfolioIdentifierEmptyTestMethod()
        {
            DBInteractivity instance = new DBInteractivity();
            PortfolioSelectionData portfolio = new PortfolioSelectionData();
            DateTime effectiveDate = new DateTime(2012, 1, 31);

            instance.RetrieveIndexConstituentsData(portfolio, effectiveDate, (List<IndexConstituentsData> resultSet) =>
            {
                Assert.AreEqual<int>(0, resultSet.Count, "Index constituent Should Be Empty");
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
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrieveAssetAllocationData(portfolio, effectiveDate, (List<AssetAllocationData> resultSet) =>
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
            instance.RetrieveAssetAllocationData(portfolio, effectiveDate, (List<AssetAllocationData> resultSet) =>
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

            instance.RetrieveAssetAllocationData(portfolio, effectiveDate, (List<AssetAllocationData> resultSet) =>
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

            instance.RetrieveAssetAllocationData(portfolio, effectiveDate, (List<AssetAllocationData> resultSet) =>
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
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, getBenchmark, (List<PortfolioDetailsData> resultSet) =>
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
            bool getBenchmark = false;
            DateTime effectiveDate = new DateTime(2012, 1, 31);
            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, getBenchmark, (List<PortfolioDetailsData> resultSet) =>
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
            bool getBenchamrk = false;

            instance.RetrievePortfolioDetailsData(portfolio, effectiveDate, getBenchamrk, (List<PortfolioDetailsData> resultSet) =>
            {
                Assert.AreEqual(0, resultSet.Count, "Portfolio Details Data Should Be Empty");
                EnqueueTestComplete();
            });
        }

        #endregion

        #endregion
    }
}