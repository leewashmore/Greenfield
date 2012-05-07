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
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;
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
    }
}