using System;

namespace AIMS.Composites.Service
{
    public class GF_COMPOSITE_LTHOLDINGS
    {
        public decimal GF_ID { get; set; }
        public DateTime? PORTFOLIO_DATE { get; set; }
        public string PORTFOLIO_ID { get; set; }
        public string A_PFCHOLDINGS_PORLT { get; set; }
        public string PORPATH { get; set; }
        public string PORTFOLIO_THEME_SUBGROUP_CODE { get; set; }
        public string PORTFOLIO_CURRENCY { get; set; }
        public string BENCHMARK_ID { get; set; }
        public string ISSUER_ID { get; set; }
        public string ASEC_SEC_SHORT_NAME { get; set; }
        public string ISSUE_NAME { get; set; }
        public string TICKER { get; set; }
        public string SECURITYTHEMECODE { get; set; }
        public string A_SEC_INSTR_TYPE { get; set; }
        public string SECURITY_TYPE { get; set; }
        public decimal? BALANCE_NOMINAL { get; set; }
        public decimal? DIRTY_PRICE { get; set; }
        public string TRADING_CURRENCY { get; set; }
        public decimal? DIRTY_VALUE_PC { get; set; }
        public decimal? BENCHMARK_WEIGHT { get; set; }
        public decimal? ASH_EMM_MODEL_WEIGHT { get; set; }
        public decimal? MARKET_CAP_IN_USD { get; set; }
        public string ASHEMM_PROP_REGION_CODE { get; set; }
        public string ASHEMM_PROP_REGION_NAME { get; set; }
        public string ISO_COUNTRY_CODE { get; set; }
        public string COUNTRYNAME { get; set; }
        public string GICS_SECTOR { get; set; }
        public string GICS_SECTOR_NAME { get; set; }
        public string GICS_INDUSTRY { get; set; }
        public string GICS_INDUSTRY_NAME { get; set; }
        public string GICS_SUB_INDUSTRY { get; set; }
        public string GICS_SUB_INDUSTRY_NAME { get; set; }
        public string LOOK_THRU_FUND { get; set; }
    }
}