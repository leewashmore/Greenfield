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
        NODENAME_SELECTOR,
        GADGET_SELECTOR
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
        public static Visibility GADGET_SELECTOR_VISIBILITY = Visibility.Collapsed;
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
                Visibility gadgetSelectorVisibility = Visibility.Collapsed,
                bool allVisible = false)//,
                //bool switchOthersOff = true
            //)
        {
            //if (switchOthersOff)
            //{
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
                ToolBoxItemVisibility.GADGET_SELECTOR_VISIBILITY = allVisible ? Visibility.Visible : gadgetSelectorVisibility;
            //}
            //else
            //{
            //    if (securitySelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.SECURITY_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (portfolioSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.PORTFOLIO_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (effectiveDateSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.EFFECTIVE_DATE_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (periodSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.PERIOD_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (countrySelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.COUNTRY_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (sectorSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.SECTOR_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (industrySelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.INDUSTRY_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (regionSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.REGION_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (filterTypeSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.FILTER_TYPE_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (filterValueSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.FILTER_VALUE_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (mktCapSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.MKT_CAP_VISIBILITY = Visibility.Visible;
            //    }
            //    if (commoditySelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.COMMODITY_SELECTOR_VISIBILTY = Visibility.Visible;
            //    }
            //    if (regionFXSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.REGIONFX_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (lookThruVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.LOOK_THRU_VISIBILITY = Visibility.Visible;
            //    }
            //    if (nodeNameSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.NODENAME_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //    if (gadgetSelectorVisibility == Visibility.Visible)
            //    {
            //        ToolBoxItemVisibility.GADGET_SELECTOR_VISIBILITY = Visibility.Visible;
            //    }
            //}
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
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Collapsed);
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
                case DashboardCategoryType.INVESTMENT_COMMITTEE_NEW_PRESENTATION:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_VOTE:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_PRESENTATIONS:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_EDIT_PRESENTATION:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategoryType.INVESTMENT_COMMITTEE_PRESENTATION_CHANGE_DATE:
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

        //public static void SetToolBoxItemVisibility(String gadgetName)
        //{
        //    switch (gadgetName)
        //    {
        //        case GadgetNames.BENCHMARK_HOLDINGS_REGION_PIECHART:
        //            UpdateToolBoxItemVisibility(effectiveDateSelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible,
        //            filterTypeSelectorVisibility: Visibility.Visible, filterValueSelectorVisibility: Visibility.Visible,
        //            lookThruVisibility: Visibility.Visible, switchOthersOff: false);
        //            break;
        //        case GadgetNames.BENCHMARK_HOLDINGS_SECTOR_PIECHART: 
        //            UpdateToolBoxItemVisibility(effectiveDateSelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible,
        //            filterTypeSelectorVisibility: Visibility.Visible, filterValueSelectorVisibility: Visibility.Visible,
        //            lookThruVisibility: Visibility.Visible, switchOthersOff: false);
        //            break;
        //        case GadgetNames.BENCHMARK_INDEX_CONSTITUENTS: 
        //            UpdateToolBoxItemVisibility(effectiveDateSelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible,
        //            lookThruVisibility: Visibility.Visible, switchOthersOff: false);
        //            break;
        //        case GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE: 
        //            UpdateToolBoxItemVisibility(effectiveDateSelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible,
        //            securitySelectorVisibility: Visibility.Visible, lookThruVisibility: Visibility.Visible, switchOthersOff: false);
        //            break;
        //        case GadgetNames.BENCHMARK_TOP_TEN_CONSTITUENTS:
        //            UpdateToolBoxItemVisibility(effectiveDateSelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible,
        //                switchOthersOff: false);
        //            break;
        //        case GadgetNames.BENCHMARKS_MARKET_PERFORMANCE_SNAPSHOT:
        //            UpdateToolBoxItemVisibility(snapshotSelectorVisibility: Visibility.Visible, switchOthersOff: false);
        //            break;
        //        case GadgetNames.BENCHMARKS_MULTILINE_BENCHMARK: 
        //            UpdateToolBoxItemVisibility(effectiveDateSelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible,
        //            securitySelectorVisibility: Visibility.Visible, lookThruVisibility: Visibility.Visible, switchOthersOff: false);
        //            break;
        //        case GadgetNames.CUSTOM_SCREENING_TOOL: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_ASSET_QUALITY_CASH_FLOW: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_BALANCE_SHEET: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_BASIC_DATA: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CASH_FLOW: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_COMPARISON_CHART: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_DETAIL: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_ESTIMATES_SUMMARY: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_MEDIAN_ESTIMATES: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_OVERVIEW: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_RECOMMENDATION: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_TARGET_PRICE: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_VALUATIONS: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_FUNDAMENTALS_SUMMARY: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_GROWTH: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_BANK: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_DividendYield: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_EVEBITDA: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_FCFYield: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_INDUSTRIAL: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_INSURANCE: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PBV: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PCE: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PREVENUE: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_UTILITY: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_INCOME_STATEMENT: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_LEVERAGE_CAPITAL_FINANCIAL_STRENGTH: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_MARGINS: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_PRICING: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_PROFITABILITY: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_BANK: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_INDUSTRIAL: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_INSURANCE: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_UTILITY: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.EXTERNAL_RESEARCH_VALUATIONS: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_CHART_EXTENTION: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_FREE_CASH_FLOW: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_MARKET_CAPITALIZATION: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_PORTFOLIO_DETAILS_UI: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_REGION_BREAKDOWN: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_RELATIVE_RISK: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_RISK_RETURN: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_SECTOR_BREAKDOWN: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.HOLDINGS_VALUATION_QUALITY_GROWTH_MEASURES: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.INTERNAL_RESEARCH_COMPANY_PROFILE_REPORT: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.INTERNAL_RESEARCH_CONSESUS_ESTIMATE_SUMMARY: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.INTERNAL_RESEARCH_FINSTAT_REPORT: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.INTERNAL_RESEARCH_PRICING_DETAILED: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.INTERNAL_RESEARCH_VALUATIONS_DETAILED: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.MODELS_FX_MACRO_ECONOMICS_COMMODITY_INDEX_RETURN: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.MODELS_FX_MACRO_ECONOMICS_EM_DATA_REPORT: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.MODELS_FX_MACRO_ECONOMICS_INTERNAL_MODELS_EVALUATION_REPORT: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_ANNUAL_DATA_REPORT: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_DATA_REPORT: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_ATTRIBUTION: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_CONTRIBUTOR_DETRACTOR: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_COUNTRY_ACTIVE_POSITION: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_GRAPH: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_GRID: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_HEAT_MAP: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_RELATIVE_PERFORMANCE: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PERFORMANCE_SECURITY_ACTIVE_POSITION: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PORTAL_ENHANCEMENTS_DOCUMENTS: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PORTAL_ENHANCEMENTS_TEAR_SHEET: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION_SUMMARY: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PORTFOLIO_ENRICHMENT_REPORT: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.PORTFOLIO_ENRICHMENT_SCREEN_MOCKUP: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.QUARTERLY_RESULTS_COMPARISON: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.SECURITY_OVERVIEW: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON: UpdateToolBoxItemVisibility(); break;
        //        case GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS: UpdateToolBoxItemVisibility(); break;
        //        default:
        //            break;
        //    }

        //}
    }
}
