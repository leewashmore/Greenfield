﻿using System;
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
    public static class ToolBoxSelecter
    {
        public enum ToolBoxItems
        {
            SECURITY_SELECTOR,
            PORTFOLIO_SELECTOR,
            EFFECTIVE_DATE_SELECTOR,
            COUNTRY_SELECTOR,
            SECTOR_SELECTOR,
            INDUSTRY_SELECTOR,
            REGION_SELECTOR,
            PERIOD_SELECTOR
        }

        
        public static class ToolBoxItemVisibility
        {
            public static Visibility SECURITY_SELECTOR_VISIBILITY = Visibility.Collapsed;
            public static Visibility PORTFOLIO_SELECTOR_VISIBILITY = Visibility.Collapsed;
            public static Visibility EFFECTIVE_DATE_SELECTOR_VISIBILITY = Visibility.Collapsed;
            public static Visibility COUNTRY_SELECTOR_VISIBILITY = Visibility.Collapsed;
            public static Visibility SECTOR_SELECTOR_VISIBILITY = Visibility.Collapsed;
            public static Visibility INDUSTRY_SELECTOR_VISIBILITY = Visibility.Collapsed;
            public static Visibility REGION_SELECTOR_VISIBILITY = Visibility.Collapsed;
            public static Visibility PERIOD_SELECTOR_VISIBILITY = Visibility.Collapsed;
        }

        private static void UpdateToolBoxItemVisibility
            (
                Visibility securitySelectorVisibility = Visibility.Collapsed,
                Visibility portfolioSelectorVisibility = Visibility.Collapsed, 
                Visibility effectiveDateSelectorVisibility = Visibility.Collapsed,
                Visibility countrySelectorVisibility = Visibility.Collapsed,
                Visibility sectorSelectorVisibility = Visibility.Collapsed,
                Visibility industrySelectorVisibility = Visibility.Collapsed,
                Visibility regionSelectorVisibility = Visibility.Collapsed,
                Visibility periodSelectorVisibility = Visibility.Collapsed
            )
        {
            ToolBoxItemVisibility.SECURITY_SELECTOR_VISIBILITY = securitySelectorVisibility;
            ToolBoxItemVisibility.PORTFOLIO_SELECTOR_VISIBILITY = portfolioSelectorVisibility;
            ToolBoxItemVisibility.EFFECTIVE_DATE_SELECTOR_VISIBILITY = effectiveDateSelectorVisibility;
            ToolBoxItemVisibility.COUNTRY_SELECTOR_VISIBILITY = countrySelectorVisibility;
            ToolBoxItemVisibility.SECTOR_SELECTOR_VISIBILITY = sectorSelectorVisibility;
            ToolBoxItemVisibility.INDUSTRY_SELECTOR_VISIBILITY = industrySelectorVisibility;
            ToolBoxItemVisibility.REGION_SELECTOR_VISIBILITY = regionSelectorVisibility;
            ToolBoxItemVisibility.PERIOD_SELECTOR_VISIBILITY = periodSelectorVisibility;
        }

        public static void SetToolBoxItemVisibility(DashboardCategory.DashboardCategoryTypes dashboardType)
        {
            switch (dashboardType)
            {
                case DashboardCategory.DashboardCategoryTypes.MARKETS_SNAPSHOT_SUMMARY:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.MARKETS_SNAPSHOT_MARKET_PERFORMANCE:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.MARKETS_SNAPSHOT_INTERNAL_MODEL_VALUATION:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.MARKETS_MACROECONOMIC_EM_SUMMARY:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.MARKETS_MACROECONOMIC_COUNTRY_SUMMARY:
                    UpdateToolBoxItemVisibility(countrySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.MARKETS_COMMODITIES_SUMMARY:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_SNAPSHOT:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, effectiveDateSelectorVisibility: Visibility.Visible
                        , countrySelectorVisibility: Visibility.Visible, sectorSelectorVisibility: Visibility.Visible
                        , industrySelectorVisibility: Visibility.Visible, regionSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_HOLDINGS:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, effectiveDateSelectorVisibility: Visibility.Visible
                        , countrySelectorVisibility: Visibility.Visible, sectorSelectorVisibility: Visibility.Visible
                        , industrySelectorVisibility: Visibility.Visible, regionSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_PERFORMANCE_SUMMARY:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, periodSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_PERFORMANCE_ATTRIBUTION:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, periodSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_PERFORMANCE_RELATIVE_PERFORMANCE:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, periodSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_BENCHMARK_SUMMARY:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, effectiveDateSelectorVisibility: Visibility.Visible
                        , countrySelectorVisibility: Visibility.Visible, sectorSelectorVisibility: Visibility.Visible
                        , industrySelectorVisibility: Visibility.Visible, regionSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_BENCHMARK_COMPOSITION:
                    UpdateToolBoxItemVisibility(portfolioSelectorVisibility: Visibility.Visible, effectiveDateSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_MODELS_ASSET_ALLOCATION:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_MODELS_STOCK_SELECTION:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_MODELS_BOTTOM_UP:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.PORTFOLIO_MODELS_DIRECT_OVERLAY:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_SNAPSHOT_SUMMARY:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_SNAPSHOT_COMPANY_PROFILE:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_SNAPSHOT_TEAR_SHEET:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible, portfolioSelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_FINANCIALS_SUMMARY:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_FINANCIALS_INCOME_STATEMENT:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_FINANCIALS_BALANCE_SHEET:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_FINANCIALS_CASH_FLOW:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_FINANCIALS_FINSTAT:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_FINANCIALS_PEER_COMPARISON:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_ESTIMATES_CONSENSUS:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_ESTIMATES_DETAILED:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_ESTIMATES_COMPARISON:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_VALUATION_FAIR_VALUE:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_VALUATION_DCF:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_DOCUMENTS:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_CHARTING_PRICE_COMPARISON:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_CHARTING_UNREALIZED_GAIN_LOSS:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_CHARTING_CONTEXT:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_CHARTING_VALUATION:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_CORPORATE_GOVERNANCE_QUESTIONNAIRE:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.COMPANY_CORPORATE_GOVERNANCE_REPORT:
                    UpdateToolBoxItemVisibility(securitySelectorVisibility: Visibility.Visible);
                    break;
                case DashboardCategory.DashboardCategoryTypes.SCREENING_STOCK:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.SCREENING_QUARTERLY_COMPARISON:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.INVESTMENT_COMMITTEE_CREATE_EDIT:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.INVESTMENT_COMMITTEE_VOTE:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.INVESTMENT_COMMITTEE_PRE_MEETING_REPORT:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.INVESTMENT_COMMITTEE_MEETING_MINUTES:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.INVESTMENT_COMMITTEE_SUMMARY_REPORT:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.INVESTMENT_COMMITTEE_METRICS_REPORT:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.ADMIN_INVESTMENT_COMMITTEE_VIEW_AGENDA:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.ADMIN_INVESTMENT_COMMITTEE_EDIT_DATE:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.ADMIN_INVESTMENT_COMMITTEE_MEETING_DETAILS:
                    UpdateToolBoxItemVisibility();
                    break;
                case DashboardCategory.DashboardCategoryTypes.ADMIN_BROKER_RESEARCH:
                    UpdateToolBoxItemVisibility();
                    break;
                default:
                    break;
            }

        }
    }
}