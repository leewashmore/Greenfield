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
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using GreenField.DashboardModule.Views;
using GreenField.Common;
using Microsoft.Practices.Prism.Modularity;

namespace GreenField.DashboardModule
{
    [ModuleExport(typeof(DashboardModule))]
    public class DashboardModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public DashboardModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboard));

            #region Company
            #region Charting
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyChartingClosingPrice));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyChartingContext));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyChartingUnrealizedGainLoss));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyChartingValuation));
            #endregion

            #region Corporate Governance
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyCorporateGovernanceQuestionnaire));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyCorporateGovernanceReport));
            #endregion

            #region Documents
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyDocuments));            
            #endregion

            #region Estimates
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyEstimatesComparison));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyEstimatesConsensus));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyEstimatesDetailed));
            #endregion

            #region Financials
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsBalanceSheet));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsCashFlow));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsFinStat));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsIncomeStatement));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsPeerComparison));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsSummary));
            #endregion

            #region Snapshot
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanySnapshotCompanyProfile));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanySnapshotSummary));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanySnapshotTearSheet));
            #endregion

            #region Valuation
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyValuationDiscountedCashFlow));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyValuationFairValue));
            #endregion 
            #endregion
            
            
        }
    }
}
