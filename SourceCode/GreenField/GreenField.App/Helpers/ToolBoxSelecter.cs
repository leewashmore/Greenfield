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
using GreenField.Common;

namespace GreenField.App.Helpers
{
    public enum ToolBoxItems
    {
        SECURITY_SELECTOR,
        SNAPSHOT_SELECTOR,
        PORTFOLIO_SELECTOR,
        EFFECTIVE_DATE_SELECTOR,
        PERIOD_SELECTOR,
        COUNTRY_SELECTOR,
        SECTOR_SELECTOR,
        INDUSTRY_SELECTOR,
        REGION_SELECTOR,
        FILTER_SELECTOR,
        MKT_CAP_SELECTOR,
        COMMODITY_SELECTOR,
        REGIONFX_SELECTOR,
        LOOKTHRU_SELECTOR,
        NODENAME_SELECTOR
    }

    public static class ToolBoxItemVisibility
    {
        public static Visibility SECURITY_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility SNAPSHOT_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility PORTFOLIO_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility EFFECTIVE_DATE_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility PERIOD_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility COUNTRY_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility SECTOR_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility INDUSTRY_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility REGION_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility FILTER_TYPE_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility FILTER_VALUE_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility MKT_CAP_VISIBILITY = Visibility.Collapsed;
        public static Visibility COMMODITY_SELECTOR_VISIBILTY = Visibility.Collapsed;
        public static Visibility REGIONFX_SELECTOR_VISIBILITY = Visibility.Collapsed;
        public static Visibility LOOK_THRU_VISIBILITY = Visibility.Collapsed;
        public static Visibility NODENAME_SELECTOR_VISIBILITY = Visibility.Collapsed;
    }

    public static class ToolBoxSelecter
    {
        private static void UpdateToolBoxItemVisibility
            (
                Visibility securitySelectorVisibility = Visibility.Collapsed,
                Visibility snapshotSelectorVisibility = Visibility.Collapsed,
                Visibility portfolioSelectorVisibility = Visibility.Collapsed,
                Visibility effectiveDateSelectorVisibility = Visibility.Collapsed,
                Visibility periodSelectorVisibility = Visibility.Collapsed,
                Visibility countrySelectorVisibility = Visibility.Collapsed,
                Visibility sectorSelectorVisibility = Visibility.Collapsed,
                Visibility industrySelectorVisibility = Visibility.Collapsed,
                Visibility regionSelectorVisibility = Visibility.Collapsed,
                Visibility filterTypeSelectorVisibility = Visibility.Collapsed,
                Visibility filterValueSelectorVisibility = Visibility.Collapsed,
                Visibility mktCapSelectorVisibility = Visibility.Collapsed,
                Visibility commoditySelectorVisibility = Visibility.Collapsed,
                Visibility regionFXSelectorVisibility = Visibility.Collapsed,
                Visibility lookThruVisibility = Visibility.Collapsed,
                Visibility nodeNameSelectorVisibility = Visibility.Collapsed,
                bool allVisible = false
            )
        {
            ToolBoxItemVisibility.SECURITY_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : securitySelectorVisibility;
            ToolBoxItemVisibility.SNAPSHOT_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : snapshotSelectorVisibility;
            ToolBoxItemVisibility.PORTFOLIO_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : portfolioSelectorVisibility;
            ToolBoxItemVisibility.EFFECTIVE_DATE_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : effectiveDateSelectorVisibility;
            ToolBoxItemVisibility.PERIOD_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : periodSelectorVisibility;
            ToolBoxItemVisibility.COUNTRY_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : countrySelectorVisibility;
            ToolBoxItemVisibility.SECTOR_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : sectorSelectorVisibility;
            ToolBoxItemVisibility.INDUSTRY_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : industrySelectorVisibility;
            ToolBoxItemVisibility.REGION_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : regionSelectorVisibility;
            ToolBoxItemVisibility.FILTER_TYPE_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : filterTypeSelectorVisibility;
            ToolBoxItemVisibility.FILTER_VALUE_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : filterValueSelectorVisibility;
            ToolBoxItemVisibility.MKT_CAP_VISIBILITY = allVisible ? Visibility.Visible : mktCapSelectorVisibility;
            ToolBoxItemVisibility.COMMODITY_SELECTOR_VISIBILTY = allVisible ? Visibility.Visible : commoditySelectorVisibility;
            ToolBoxItemVisibility.REGIONFX_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : regionFXSelectorVisibility;
            ToolBoxItemVisibility.LOOK_THRU_VISIBILITY = allVisible ? Visibility.Visible : lookThruVisibility;
            ToolBoxItemVisibility.NODENAME_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : nodeNameSelectorVisibility;
        }

        public static void SetToolBoxItemVisibility(DashboardCategoryType dashboardType)
        {
            switch (dashboardType)
            {
                case DashboardCategoryType.MARKETS_SNAPSHOT_SUMMARY:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.MARKETS_SNAPSHOT_MARKET_PERFORMANCE:
                    UpdateToolBoxItemVisibility(snapshotSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.MARKETS_SNAPSHOT_INTERNAL_MODEL_VALUATION:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.MARKETS_MACROECONOMIC_EM_SUMMARY:
                    UpdateToolBoxItemVisibility(regionFXSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.MARKETS_MACROECONOMIC_COUNTRY_SUMMARY:
                    UpdateToolBoxItemVisibility(countrySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.MARKETS_COMMODITIES_SUMMARY:
                    UpdateToolBoxItemVisibility(commoditySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.PORTFOLIO_SNAPSHOT:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible,
                                                effectiveDateSelectorVisibility: Visibility.Visible,
                                                filterTypeSelectorVisibility: Visibility.Visible,
                                                filterValueSelectorVisibility: Visibility.Visible,
                                                mktCapSelectorVisibility: Visibility.Visible,
                                                periodSelectorVisibility: Visibility.Visible,
                                                lookThruVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.PORTFOLIO_HOLDINGS:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible,
                                                effectiveDateSelectorVisibility: Visibility.Visible,
                                                filterTypeSelectorVisibility: Visibility.Collapsed,
                                                filterValueSelectorVisibility: Visibility.Collapsed,
                                                mktCapSelectorVisibility: Visibility.Visible,
                                                lookThruVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.PORTFOLIO_PERFORMANCE_SUMMARY:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, effectiveDateSelectorVisibility: Visibility.Visible, periodSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.PORTFOLIO_PERFORMANCE_ATTRIBUTION:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, periodSelectorVisibility: Visibility.Visible, effectiveDateSelectorVisibility: Visibility.Visible,nodeNameSelectorVisibility:Visibility.Visible);
                    break;
                case DashboardCategoryType.PORTFOLIO_PERFORMANCE_RELATIVE_PERFORMANCE:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, periodSelectorVisibility: Visibility.Visible, effectiveDateSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.PORTFOLIO_BENCHMARK_SUMMARY:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible,
                    effectiveDateSelectorVisibility: Visibility.Visible,
                         filterTypeSelectorVisibility: Visibility.Visible,
                        filterValueSelectorVisibility: Visibility.Visible, 
                        periodSelectorVisibility: Visibility.Visible,
                        lookThruVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.PORTFOLIO_BENCHMARK_COMPOSITION:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible,
                                                effectiveDateSelectorVisibility: Visibility.Visible,
                                                filterTypeSelectorVisibility: Visibility.Collapsed,
                                                filterValueSelectorVisibility: Visibility.Collapsed,
                                                lookThruVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.PORTFOLIO_MODELS_ASSET_ALLOCATION:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.PORTFOLIO_MODELS_STOCK_SELECTION:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.PORTFOLIO_MODELS_BOTTOM_UP:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.PORTFOLIO_MODELS_DIRECT_OVERLAY:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.COMPANY_SNAPSHOT_SUMMARY:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible, effectiveDateSelectorVisibility: Visibility.Visible, periodSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_SNAPSHOT_COMPANY_PROFILE:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_SNAPSHOT_TEAR_SHEET:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_FINANCIALS_SUMMARY:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_FINANCIALS_INCOME_STATEMENT:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_FINANCIALS_BALANCE_SHEET:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_FINANCIALS_CASH_FLOW:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_FINANCIALS_FINSTAT:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_FINANCIALS_PEER_COMPARISON:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_ESTIMATES_CONSENSUS:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_ESTIMATES_DETAILED:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_ESTIMATES_COMPARISON:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_VALUATION_FAIR_VALUE:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_VALUATION_DCF:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_DOCUMENTS:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_CHARTING_PRICE_COMPARISON:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_CHARTING_UNREALIZED_GAIN_LOSS:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_CHARTING_CONTEXT:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_CHARTING_VALUATION:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_CORPORATE_GOVERNANCE_QUESTIONNAIRE:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.COMPANY_CORPORATE_GOVERNANCE_REPORT:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.SCREENING_STOCK:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.SCREENING_QUARTERLY_COMPARISON:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_CREATE_EDIT:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_VOTE:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_PRE_MEETING_REPORT:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_MEETING_MINUTES:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_SUMMARY_REPORT:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_METRICS_REPORT:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.ADMIN_INVESTMENT_COMMITTEE_VIEW_AGENDA:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.ADMIN_INVESTMENT_COMMITTEE_EDIT_DATE:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.ADMIN_INVESTMENT_COMMITTEE_MEETING_DETAILS:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.ADMIN_BROKER_RESEARCH:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.USER_DASHBOARD:
                    UpdateToolBoxItemVisibility(allVisible: true);
                    break;
                case DashboardCategoryType.MKT_CAP:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.COMPANY_SNAPSHOT_BASICDATA_SUMMARY:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility : Visibility.Visible);
                    break;
                default:
                    break;
            }

        }
    }
}
