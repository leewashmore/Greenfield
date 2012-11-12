using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.App.Helpers;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.DashboardModule.Helpers;
using GreenField.DataContracts;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.UserSession;
using System.Reflection;

namespace GreenField.App.ViewModel
{
    public partial class ViewModelShell : NotificationObject
    {
        /// <summary>
        /// LogoutCommand Execution Method - Navigate to Login Page
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void LogOutCommandMethod(object param)
        {
            try
            {
                HtmlPage.Window.Navigate(new Uri(@"Login.aspx", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        #region Dashboard
        
        #region Company
        #region Snapshot
        private void DashboardCompanySnapshotSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_SNAPSHOT_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanySnapshotSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanySnapshotCompanyProfileCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_SNAPSHOT_COMPANY_PROFILE);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanySnapshotCompanyProfile", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanySnapshotTearSheetCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_SNAPSHOT_TEAR_SHEET);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanySnapshotTearSheet", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardConsensusEstimateSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_SNAPSHOT_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanySnapshotSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }


        private void DashboardCompanySnapshotBasicDataCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_SNAPSHOT_BASICDATA_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewBasicData", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }


        #endregion

        #region Financials
        private void DashboardCompanyFinancialsSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsIncomeStatementCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_INCOME_STATEMENT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsIncomeStatement", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsBalanceSheetCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_BALANCE_SHEET);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsBalanceSheet", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsCashFlowCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_CASH_FLOW);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsCashFlow", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyFinstatCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_FINSTAT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsFinStat", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsFinStatCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_FINSTAT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsFinStat", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsPeerComparisonCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_PEER_COMPARISON);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsPeerComparison", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Estimates
        private void DashboardCompanyEstimatesConsensusCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_ESTIMATES_CONSENSUS);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyEstimatesConsensus", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyEstimatesDetailedCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_ESTIMATES_DETAILED);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyEstimatesDetailed", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyEstimatesComparisonCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_ESTIMATES_COMPARISON);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyEstimatesComparison", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Valuation
        private void DashboardCompanyValuationFairValueCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_VALUATION_FAIR_VALUE);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyValuationFairValue", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyValuationDiscountedCashFlowCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_VALUATION_DCF);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyValuationDiscountedCashFlow", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Documents
        private void DashboardCompanyDocumentsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_DOCUMENTS);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyDocuments", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyDocumentsLoadModelCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_DOCUMENTS);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyDocumentsLoad", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion

        #region Charting
        private void DashboardCompanyChartingClosingPriceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CHARTING_PRICE_COMPARISON);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingClosingPrice", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyChartingUnrealizedGainCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CHARTING_UNREALIZED_GAIN_LOSS);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingUnrealizedGainLoss", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyChartingContextCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CHARTING_CONTEXT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingContext", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyChartingValuationCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CHARTING_VALUATION);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingValuation", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Corporate Governance
        private void DashboardCompanyCorporateGovernanceQuestionnaireCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CORPORATE_GOVERNANCE_QUESTIONNAIRE);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyCorporateGovernanceQuestionnaire", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardCompanyCorporateGovernanceReportCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CORPORATE_GOVERNANCE_REPORT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyCorporateGovernanceReport", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion
        #endregion

        #region Markets
        #region Snapshot
        private void DashboardMarketsSnapshotSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_SNAPSHOT_SUMMARY);
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsSnapshotSummary", UriKind.Relative));
                UpdateToolBoxSelectorVisibility();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardMarketsSnapshotMarketPerformanceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (dbInteractivity != null && (EntitySelectionInfo == null || EntitySelectionInfo.Count == 0))
                {
                    BusyIndicatorNotification(true, "Retrieving reference data...");
                    dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallbackMethod);
                }
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_SNAPSHOT_MARKET_PERFORMANCE);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsSnapshotMarketPerformance", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardMarketsSnapshotInternalModelValuationCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_SNAPSHOT_INTERNAL_MODEL_VALUATION);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsSnapshotInternalModelValuation", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region MacroEconomic
        private void DashboardMarketsMacroEconomicsEMSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_MACROECONOMIC_EM_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsMacroEconomicsEMSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardMarketsMacroEconomicsCountrySummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_MACROECONOMIC_COUNTRY_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsMacroEconomicsCountrySummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Commodities
        private void DashboardMarketsCommoditiesSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_COMMODITIES_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsCommoditiesSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion
        #endregion

        #region Portfolio
        #region Snapshot
        private void DashboardPortfolioSnapshotCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_SNAPSHOT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioSnapshot", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Holdings
        private void DashboardPortfolioHoldingsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_HOLDINGS);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioHoldings", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Performance
        private void DashboardPortfolioPerformanceSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_PERFORMANCE_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioPerformanceSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardPortfolioPerformanceAttributionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_PERFORMANCE_ATTRIBUTION);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioPerformanceAttribution", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardPortfolioPerformanceRelativePerformanceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_PERFORMANCE_RELATIVE_PERFORMANCE);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioPerformanceRelativePerformance", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Benchmark
        private void DashboardPortfolioBenchmarkSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_BENCHMARK_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioBenchmarkSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardPortfolioBenchmarkComponentsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_BENCHMARK_COMPOSITION);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioBenchmarkComponents", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion
        #endregion

        #region Screening
        #region Quarterly Comparison

        private void QuarterlyComparisonCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.QUARTERLY_RESULTS_COMPARISON,
                            DashboardTileObject = new ViewQuarterlyResultsComparison(new ViewModelQuarterlyResultsComparison(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion
        #endregion

        #region Investment Committee
        private void DashboardInvestmentCommitteeCreateEditCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.INVESTMENT_COMMITTEE_CREATE_EDIT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteePresentations", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardInvestmentCommitteeVoteCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.INVESTMENT_COMMITTEE_VOTE);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeVote", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardInvestmentCommitteePreMeetingReportCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.INVESTMENT_COMMITTEE_PRE_MEETING_REPORT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteePreMeetingReport", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardInvestmentCommitteeMeetingMinutesCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.INVESTMENT_COMMITTEE_MEETING_MINUTES);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeMeetingMinutes", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardInvestmentCommitteeSummaryReportCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.INVESTMENT_COMMITTEE_SUMMARY_REPORT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeSummaryReport", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void DashboardInvestmentCommitteeMetricsReportCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.INVESTMENT_COMMITTEE_METRICS_REPORT);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeMetricsReport", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion

        #region Admin

        #region Investment Committee
        private void DashboardAdminInvestmentCommitteeChangeDateCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.ADMIN_INVESTMENT_COMMITTEE_EDIT_DATE);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardAdminInvestmentCommitteeChangeDate", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #endregion

        private void DashboardQuarterlyResultsComparisonCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.SCREENING_QUARTERLY_COMPARISON);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardQuarterlyResultsComparison", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);

        }

        private void DashboardCustomScreeningToolCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.SCREENING_STOCK);
                UpdateToolBoxSelectorVisibility();
                //flag to refresh the custom screening tool view
                RefreshScreen.refreshFlag = true;
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningTool", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);

        }

        #endregion

        #region ToolBox
        private void UserDashboardSaveCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardGadgetSave>().Publish(null);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// MarketSnapshotAddCommand execution method - creates new market performance snapshot
        /// </summary>
        /// <param name="param">sender info</param>
        private void MarketSnapshotAddCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>()
                    .Publish(new MarketPerformanceSnapshotActionPayload()
                    {
                        ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_ADD,
                        MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo,
                        SelectedMarketSnapshotSelectionInfo = SelectedMarketSnapshotSelectionInfo
                    });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// MarketSnapshotSaveCommand Validation Method
        /// </summary>
        /// <param name="param">sender info</param>
        /// <returns>True/False</returns>
        private bool MarketSnapshotSaveCommandValidationMethod(object param)
        {
            return SelectedMarketSnapshotSelectionInfo != null;
        }

        /// <summary>
        /// MarketSnapshotSaveCommand execution method - saves changes in existing market performance snapshot
        /// </summary>
        /// <param name="param">sender info</param>
        private void MarketSnapshotSaveCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>()
                   .Publish(new MarketPerformanceSnapshotActionPayload()
                   {
                       ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE,
                       MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo,
                       SelectedMarketSnapshotSelectionInfo = SelectedMarketSnapshotSelectionInfo
                   });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// MarketSnapshotSaveAsCommand execution method - saves existing market performance snapshot by new name
        /// </summary>
        /// <param name="param">sender info</param>
        private void MarketSnapshotSaveAsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>()
                   .Publish(new MarketPerformanceSnapshotActionPayload()
                   {
                       ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE_AS,
                       MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo,
                       SelectedMarketSnapshotSelectionInfo = SelectedMarketSnapshotSelectionInfo
                   });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// MarketSnapshotRemoveCommand Validation Method
        /// </summary>
        /// <param name="param">sender info</param>
        /// <returns>True/False</returns>
        private bool MarketSnapshotRemoveCommandValidationMethod(object param)
        {
            return SelectedMarketSnapshotSelectionInfo != null;
        }

        /// <summary>
        /// MarketSnapshotRemoveCommand execution method - deletes existing market performance snapshot
        /// </summary>
        /// <param name="param">sender info</param>
        private void MarketSnapshotRemoveCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>()
                   .Publish(new MarketPerformanceSnapshotActionPayload()
                   {
                       ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_REMOVE,
                       MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo,
                       SelectedMarketSnapshotSelectionInfo = SelectedMarketSnapshotSelectionInfo
                   });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion

        private void UserManagementCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewManageUsers", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        private void RoleManagementCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewManageRoles", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        private void DailyMorningSnapshotCommandMethod(object param)
        {
            regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewMorningSnapshot", UriKind.Relative));
        }

        /// <summary>
        /// MyDashboardCommand Execution Method - Opens Dashboard 
        /// </summary>
        /// <param name="param"></param>
        private void MyDashboardCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                IsApplicationMenuExpanded = false;
                eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.USER_DASHBOARD);
                UpdateToolBoxSelectorVisibility();
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboard", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetSecurityOverviewCommand Execution Method - Add Gadget - SECURITY_OVERVIEW
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSecurityOverviewCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                       (new DashboardTileViewItemInfo
                       {
                           DashboardTileHeader = GadgetNames.SECURITY_OVERVIEW,
                           DashboardTileObject = new ViewSecurityOverview(new ViewModelSecurityOverview(GetDashboardGadgetParam()))
                       });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetPricingCommand Execution Method - Add Gadget - PRICING
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetPricingCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                       (new DashboardTileViewItemInfo
                       {
                           DashboardTileHeader = GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON,
                           DashboardTileObject = new ViewClosingPriceChart(new ViewModelClosingPriceChart(GetDashboardGadgetParam()))
                       });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetTheoreticalUnrealizedGainLoss Execution Method - Add Gadget - UNREALIZED_GAINLOSS
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetTheoreticalUnrealizedGainLossCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS,
                            DashboardTileObject = new ViewUnrealizedGainLoss(new ViewModelUnrealizedGainLoss(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetRegionBreakdownCommand Execution Method - Add Gadget - REGION_BREAKDOWN
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetRegionBreakdownCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_REGION_BREAKDOWN,
                            DashboardTileObject = new ViewRegionBreakdown(new ViewModelRegionBreakdown(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetSectorBreakdownCommand Execution Method - Add Gadget - SECTOR_BREAKDOWN
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSectorBreakdownCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_SECTOR_BREAKDOWN,
                            DashboardTileObject = new ViewSectorBreakdown(new ViewModelSectorBreakdown(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetIndexConstituentsCommand Execution Method - Add Gadget - INDEX_CONSTITUENTS
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetIndexConstituentsCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.BENCHMARK_INDEX_CONSTITUENTS,
                            DashboardTileObject = new ViewIndexConstituents(new ViewModelIndexConstituents(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetMarketCapitalizationCommand Execution Method - Add Gadget - MARKET_CAPITALIZATION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetMarketCapitalizationCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_MARKET_CAPITALIZATION,
                            DashboardTileObject = new ViewMarketCapitalization(new ViewModelMarketCapitalization(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetTopHoldingsCommand Execution Method - Add Gadget - TOP_HOLDINGS
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetTopHoldingsCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS,
                            DashboardTileObject = new ViewTopHoldings(new ViewModelTopHoldings(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetTopHoldingsCommand Execution Method - Add Gadget - ASSET_ALLOCATION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetAssetAllocationCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_ASSET_ALLOCATION,
                            DashboardTileObject = new ViewAssetAllocation(new ViewModelAssetAllocation(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetHoldingsPieChartCommand Execution Method - Add Gadget - HOLDINGS_PIECHART
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetHoldingsPieChartCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.BENCHMARK_HOLDINGS_SECTOR_PIECHART,
                            DashboardTileObject = new ViewHoldingsPieChart(new ViewModelHoldingsPieChart(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetPortfolioRiskReturnsCommand Execution Method - Add Gadget - PORTFOLIO_RISK_RETURNS
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetPortfolioRiskReturnsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_RISK_RETURN,
                            DashboardTileObject = new ViewPortfolioRiskReturns(new ViewModelPortfolioRiskReturns(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetTopBenchmarkSecuritiesCommand Execution Method - Add Gadget - TOP_BENCHMARK_SECURITIES
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetTopBenchmarkSecuritiesCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.BENCHMARK_TOP_TEN_CONSTITUENTS,
                            DashboardTileObject = new ViewTopBenchmarkSecurities(new ViewModelTopBenchmarkSecurities(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetRelativeRiskCommand Execution Method - Add Gadget - RELATIVE_RISK
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetRelativeRiskCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_RELATIVE_RISK,
                            DashboardTileObject = new ViewRiskIndexExposures(new ViewModelRiskIndexExposures(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetRelativePerformaceCommand Execution Method - Add Gadget - RELATIVE_PERFORMANCE
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetRelativePerformanceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_RELATIVE_PERFORMANCE,
                            DashboardTileObject = new ViewRelativePerformance(new ViewModelRelativePerformance(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetCountryActivePositionCommand Execution Method - Add Gadget - COUNTRY_ACTIVE_POSITION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetCountryActivePositionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_COUNTRY_ACTIVE_POSITION,
                            DashboardTileObject = new ViewRelativePerformanceCountryActivePosition(new ViewModelRelativePerformanceCountryActivePosition(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetSectorActivePositionCommand Execution Method - Add Gadget - SECTOR_ACTIVE_POSITION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSectorActivePositionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION,
                            DashboardTileObject = new ViewRelativePerformanceSectorActivePosition(new ViewModelRelativePerformanceSectorActivePosition(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetSecurityActivePositionCommand Execution Method - Add Gadget - SECURITY_ACTIVE_POSITION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSecurityActivePositionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION,
                            DashboardTileObject = new ViewRelativePerformanceSecurityActivePosition(new ViewModelRelativePerformanceSecurityActivePosition(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetContributorDetractorCommand Execution Method - Add Gadget - CONTRIBUTOR_DETRACTOR
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetContributorDetractorCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_CONTRIBUTOR_DETRACTOR,
                            DashboardTileObject = new ViewContributorDetractor(new ViewModelContributorDetractor(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// GadgetSaveCommand Execution Method - Save Dashboard Preference
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSaveCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                eventAggregator.GetEvent<DashboardGadgetSave>().Publish(null);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        private void RelativePerformanceCommandMethod(object param)
        {
            regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewRelativePerformance", UriKind.Relative));
        }

        /// <summary>
        /// Portfolio Details navigation Method
        /// </summary>
        /// <param name="param"></param>
        private void PortfolioDetailsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_PORTFOLIO_DETAILS_UI,
                            DashboardTileObject = new ViewPortfolioDetails(new ViewModelPortfolioDetails(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Asset Allocation navigation Method
        /// </summary>
        /// <param name="param"></param>
        private void AssetAllocationCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_ASSET_ALLOCATION,
                            DashboardTileObject = new ViewAssetAllocation(new ViewModelAssetAllocation(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void PerformanceGraphCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_GRAPH,
                            DashboardTileObject = new ViewPerformanceGadget(new ViewModelPerformanceGadget(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void PerformanceGridCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_GRID,
                            DashboardTileObject = new ViewPerformanceGrid(new ViewModelPerformanceGrid(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void AttributionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_ATTRIBUTION,
                            DashboardTileObject = new ViewAttribution(new ViewModelAttribution(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        private void RetrieveRegionDataCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                List<String> selectedCountries = new List<String>();

                foreach (RegionDataItem item in RegionItems)
                {
                    List<RegionDataItem> children = item.SubCategories.ToList();

                    foreach (RegionDataItem child in children)
                    {
                        if (child.IsChecked != null && (bool)child.IsChecked)
                        {
                            selectedCountries.Add(child.Name);
                        }
                    }

                }

                RegionCountryNames = selectedCountries;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }


        // targeting

        private void TargetingBroadGlobalActiveCommandMethod(Object whatever)
        {
            this.WatchMe(this.GetMethodSignature("TargetingBroadGlobalActiveCommandMethod"), delegate
            {
                throw new NotImplementedException("Handler for targeting: broad global active hasn't been implemented yet.");
            });
        }

        private void TargetingBottomUpCommandMethod(Object whatever)
        {
            this.WatchMe(this.GetMethodSignature("TargetingBottomUpCommandMethod"), delegate
            {
                throw new NotImplementedException("Handler for targeting: bottom up hasn't been implemented yet.");
            });
        }

        private void TargetingBasketTargetsCommandMethod(Object whatever)
        {
            this.WatchMe(this.GetMethodSignature("TargetingBasketTargetsCommandMethod"), delegate
            {
                throw new NotImplementedException("Handler for targeting: basket targets hasn't been implemented yet.");
            });
        }

        
        // helper methods

        protected String GetMethodSignature(MethodInfo method)
        {
            var signature = this.GetMethodSignature(method.Name);
            return signature;
        }
        protected String GetMethodSignature(String methodName)
        {
            var signature = String.Format("{0}.{1}", GetType().FullName, methodName);
            return signature;
        }
        
        /// <summary>
        /// Takes care of exception logging.
        /// </summary>
        protected virtual void WatchMe(String signature, Action anything)
        {
            Logging.LogBeginMethod(logger, signature);
            try
            {
                anything();
            }
            catch (Exception exception)
            {
                Prompt.ShowDialog("Message: " + exception.Message + "\nStackTrace: " + Logging.StackTraceToString(exception), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, exception);
            }
            Logging.LogEndMethod(logger, signature);
        }
    }
}
