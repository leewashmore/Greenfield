using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class HoldingsData
    {       
        //[DataMember]
        //public String COUNTRYZONECODE { get; set; }

        //[DataMember]
        //public String COUNTRYZONENAME { get; set; }

        //[DataMember]
        //public String COUNTRYCODE { get; set; }

        //[DataMember]
        //public String COUNTRYNAME { get; set; }

        //[DataMember]
        //public Decimal CNTY_DIRTYVALPC { get; set; }

        //[DataMember]
        //public Decimal CNTYEXPOSUREPC { get; set; }

        [DataMember]
        public String PFCD_FROMDATE { get; set; }

        [DataMember]
        public String PFCD_TODATE { get; set; }

        [DataMember]
        public String PORTFOLIOTHEMEGROUPCODE { get; set; }

        [DataMember]
        public String PORTFOLIOTHEMEGROUP { get; set; }

        [DataMember]
        public String PORTFOLIOTHEMEGROUP_SORT { get; set; }

        [DataMember]
        public String PORTFOLIOTHEMESUBGROUPCODE { get; set; }

        [DataMember]
        public String PORTFOLIOTHEMESUBGROUP { get; set; }

        [DataMember]
        public String PORTFOLIOTHEMESUBGROUP_SORT { get; set; }

        [DataMember]
        public String PORTFOLIOCODE { get; set; }

        [DataMember]
        public String PORTFOLIONAME { get; set; }

        [DataMember]
        public String PORTFOLIO_SORT { get; set; }

        [DataMember]
        public String IRP_LEVERAGELIMIT { get; set; }

        [DataMember]
        public String PFCH_BALNOMVAL { get; set; }

        [DataMember]
        public String PFCH_BALBOOKVALPC { get; set; }

        [DataMember]
        public String PFCH_BALBOOKVALQC { get; set; }

        [DataMember]
        public String PFCH_BALCOSTVALPC { get; set; }

        [DataMember]
        public String PFCH_BALCOSTVALQC { get; set; }

        [DataMember]
        public String PFCH_BALCOSTPRI { get; set; }

        [DataMember]
        public String PFCH_BCOSTYLD { get; set; }

        [DataMember]
        public String PFCH_UNSETAMTINPC { get; set; }

        [DataMember]
        public String PFCH_UNSETAMTOUTPC { get; set; }

        [DataMember]
        public String PFCH_BALBOOKPRI { get; set; }

        [DataMember]
        public String PFKR_CURYLD { get; set; }

        [DataMember]
        public String PFKR_FXRATEQP { get; set; }

        [DataMember]
        public String PFKR_PFCHOLIK { get; set; }

        [DataMember]
        public String PFKR_PV01 { get; set; }

        [DataMember]
        public String PFCM_PFCM { get; set; }

        [DataMember]
        public String HOLK_PORIK { get; set; }

        [DataMember]
        public String HOLK_SECIK { get; set; }

        [DataMember]
        public String BENCHMARKSECURITY { get; set; }

        [DataMember]
        public String PFKR_ACR { get; set; }

        [DataMember]
        public String PFKR_ACRVALPC { get; set; }

        [DataMember]
        public String PFKR_CLEAN { get; set; }

        [DataMember]
        public String PFKR_DIRTY { get; set; }

        [DataMember]
        public String PFKR_DIRTYVALPC { get; set; }

        [DataMember]
        public String PFKR_DIRTYVALQC { get; set; }

        [DataMember]
        public String PFKR_EXPOSUREPC { get; set; }

        [DataMember]
        public String PFKR_DOLDURVALDFPC { get; set; }

        [DataMember]
        public String DOLDUR_0TO3YRS { get; set; }

        [DataMember]
        public String DOLDUR_3TO5YRS { get; set; }

        [DataMember]
        public String DOLDUR_5TO7YRS { get; set; }

        [DataMember]
        public String DOLDUR_7TO10YRS { get; set; }

        [DataMember]
        public String DOLDUR_5TO10YRS { get; set; }

        [DataMember]
        public String DOLDUR_GT10YRS { get; set; }

        [DataMember]
        public String DOLDUR_0TO3MODDUR { get; set; }

        [DataMember]
        public String DOLDUR_3TO5MODDUR { get; set; }

        [DataMember]
        public String DOLDUR_5TO7MODDUR { get; set; }

        [DataMember]
        public String DOLDUR_7TO10MODDUR { get; set; }

        [DataMember]
        public String DOLDUR_5TO10MODDUR { get; set; }

        [DataMember]
        public String DOLDUR_GT10MODDUR { get; set; }

        [DataMember]
        public String PFKR_MODSPRDUR { get; set; }

        [DataMember]
        public String PFKR_YTMYLD { get; set; }


        [DataMember]
        public String PFKR_YLDFIN { get; set; }

        [DataMember]
        public String PFKR_COUDATENEXT { get; set; }

        [DataMember]
        public String PFKR_COURATEBOOK { get; set; }

        [DataMember]
        public String PFKR_YLDTOWORST { get; set; }

        [DataMember]
        public String PFKR_MATDATE { get; set; }

        [DataMember]
        public String PFKR_MAT { get; set; }

        [DataMember]
        public String EXP_0TO3YRS { get; set; }

        [DataMember]
        public String EXP_3TO5YRS { get; set; }

        [DataMember]
        public String EXP_5TO7YRS { get; set; }

        [DataMember]
        public String EXP_7TO10YRS { get; set; }

        [DataMember]
        public String EXP_5TO10YRS { get; set; }

        [DataMember]
        public String EXP_GT10YRS { get; set; }

        [DataMember]
        public String EXP_0TO3MODDUR { get; set; }

        [DataMember]
        public String EXP_3TO5MODDUR { get; set; }

        [DataMember]
        public String EXP_5TO7MODDUR { get; set; }

        [DataMember]
        public String EXP_7TO10MODDUR { get; set; }

        [DataMember]
        public String EXP_5TO10MODDUR { get; set; }

        [DataMember]
        public String EXP_GT10MODDUR { get; set; }

        [DataMember]
        public String PFKR_MODDURDF { get; set; }

        [DataMember]
        public String PFKR_UPRLCOSTSECPC { get; set; }

        [DataMember]
        public String PFKR_UPRLCOSTCURPC { get; set; }

        [DataMember]
        public String PFKR_UPRLBOOKSECPC { get; set; }

        [DataMember]
        public String PFKR_UPRLBOOKCURPC { get; set; }

        [DataMember]
        public String PFCP_PERPRLBOOKSECPC { get; set; }

        [DataMember]
        public String PFCP_PERPRLBOOKCURPC { get; set; }

        [DataMember]
        public String PFKR_PRICETYPE { get; set; }

        [DataMember]
        public String PFKR_QUOTEDATE { get; set; }

        [DataMember]
        public String COUNTRYZONECODE { get; set; }

        [DataMember]
        public String COUNTRYZONENAME { get; set; }

        [DataMember]
        public String COUNTRYZONE_SORT { get; set; }

        [DataMember]
        public String CNTY_CTYIK { get; set; }

        [DataMember]
        public String COUNTRYCODE { get; set; }

        [DataMember]
        public String COUNTRYNAME { get; set; }

        [DataMember]
        public String CNTY_NON_EM { get; set; }

        [DataMember]
        public String HOLK_QUOCUR { get; set; }

        [DataMember]
        public String CURR_NON_EM { get; set; }

        [DataMember]
        public String SEC_SECFC90_CURRENCYOFRISK { get; set; }

        [DataMember]
        public String CURR_RISK_CUR { get; set; }

        [DataMember]
        public String CURR_RISK_NON_EM { get; set; }

        [DataMember]
        public String CURR_QUORISK_BEST { get; set; }

        [DataMember]
        public String CURR_QUORISK_NONEM_BEST { get; set; }

        [DataMember]
        public String IRP_INSTYPE_GROUPS_1 { get; set; }

        [DataMember]
        public String IRP_INSTYPE_GROUPS_2 { get; set; }

        [DataMember]
        public String ISIN { get; set; }

        [DataMember]
        public String SEC_SECFC2IK { get; set; }

        [DataMember]
        public String SEC_SECSHORT { get; set; }

        [DataMember]
        public String SEC_SECNAME { get; set; }

        [DataMember]
        public String SIDS_IDENT { get; set; }

        [DataMember]
        public String IXCT_UNDERLY { get; set; }

        [DataMember]
        public String IXCS_SECNO { get; set; }

        [DataMember]
        public String IXCS_SECSHORT { get; set; }

        [DataMember]
        public String SEC_SECNAMEROW1 { get; set; }

        [DataMember]
        public String SEC_ISSVOL { get; set; }

        [DataMember]
        public String SEC_ISSVOLOUT { get; set; }

        [DataMember]
        public String SEC_ISSVOL_BEST { get; set; }

        [DataMember]
        public String SEC_DEFQUOCUR { get; set; }

        [DataMember]
        public String SEC_INSTYPE { get; set; }

        [DataMember]
        public String INSTRUMENTTYPE { get; set; }

        [DataMember]
        public String SEC_INSTYPE_NAME { get; set; }

        [DataMember]
        public String INSTRUMENTTYPENAME { get; set; }

        [DataMember]
        public String SEC_SECFC78_ZSPREAD { get; set; }

        [DataMember]
        public String SEC_SECFC88_LIQUIDITY { get; set; }

        [DataMember]
        public String SEC_SECFC89_BETA { get; set; }

        [DataMember]
        public String SECT_SECTYPE { get; set; }

        [DataMember]
        public String SECT_SORT { get; set; }

        [DataMember]
        public String IBI_SPREAD { get; set; }

        [DataMember]
        public String SECURITYTHEMECODE { get; set; }

        [DataMember]
        public String SECURITYTHEMENAME { get; set; }

        [DataMember]
        public String SECURITYTHEME_SORT { get; set; }

        [DataMember]
        public String PARG_PARGRP { get; set; }

        [DataMember]
        public String PARG_PARGRPNAME { get; set; }

        [DataMember]
        public String PART_PARIK { get; set; }

        [DataMember]
        public String PART_PAR { get; set; }

        [DataMember]
        public String ISSUERNAME { get; set; }

        [DataMember]
        public String OWNERSHIPCODE { get; set; }

        [DataMember]
        public String OWNERSHIPNAME { get; set; }

        [DataMember]
        public String OWNERSHIP_SORT { get; set; }

        [DataMember]
        public String BM1_BMIK { get; set; }

        [DataMember]
        public String BM2_BMIK { get; set; }

        [DataMember]
        public String BM3_BMIK { get; set; }

        [DataMember]
        public String SECFC15_SECFC15 { get; set; }

        [DataMember]
        public String SECFC15_PERFORMANCECURRENCY { get; set; }

        [DataMember]
        public String SECFC5_OWNERSHIP_PCENT { get; set; }

        [DataMember]
        public String COUNTRYCODEEXP { get; set; }

        [DataMember]
        public String COUNTRYNAMEEXP { get; set; }

        [DataMember]
        public String COUNTRYZONECODEEXP { get; set; }

        [DataMember]
        public String COUNTRYZONENAMEEXP { get; set; }

        [DataMember]
        public String COUNTRYZONESORTEXP { get; set; }

        [DataMember]
        public String ASHMMORE_EXP_DEF { get; set; }

        [DataMember]
        public String ASHMORE_EXP_DEF_CCY { get; set; }

        [DataMember]
        public String ASHMORE_EXP_BRK { get; set; }

        [DataMember]
        public String SECFC12_MANAGEDBYCODE { get; set; }

        [DataMember]
        public String SECFC12_MANAGEDBYNAME { get; set; }

        [DataMember]
        public String SECFC17_QUASISOVGUARCODE { get; set; }

        [DataMember]
        public String SECFC17_QUASISOVGUARNAME { get; set; }

        [DataMember]
        public String SECFC18_SECURITYLIQUIDITYCODE { get; set; }

        [DataMember]
        public String SECFC18_SECURITYLIQUIDITYNAME { get; set; }

        [DataMember]
        public String SECFC18_SECURITYLIQUIDITYSORT { get; set; }

        [DataMember]
        public String SECFC19_ORDERREASON { get; set; }

        [DataMember]
        public String SECFC19_ORDERREASONSORT { get; set; }

        [DataMember]
        public String SECFC20_IRPLOOKTHRUPOSS { get; set; }

        [DataMember]
        public String TOTAL_CORPORATE_RESTRICT_LIMIT { get; set; }

        [DataMember]
        public String TOTAL_CORPORATE_HOUSE_LIMIT { get; set; }

        [DataMember]
        public String LEVERAGE_LIMIT { get; set; }

        [DataMember]
        public String EXTERNAL_DEBT_SUB_GROUP { get; set; }

        [DataMember]
        public String CORP_ISSUER_LIMIT { get; set; }

        [DataMember]
        public String BENCHMARKSECTHEMECODE { get; set; }

        [DataMember]
        public String PF_BENCHMARK1CODE { get; set; }

        [DataMember]
        public String PF_BM1_IXCOVERAGE { get; set; }

        [DataMember]
        public String PF_BM1_BMFC4 { get; set; }

        [DataMember]
        public String PF_BENCHMARK2CODE { get; set; }

        [DataMember]
        public String PF_BM2_IXCOVERAGE { get; set; }

        [DataMember]
        public String PF_BM2_BMFC4 { get; set; }

        [DataMember]
        public String PF_BENCHMARK3CODE { get; set; }

        [DataMember]
        public String PF_BM3_IXCOVERAGE { get; set; }

        [DataMember]
        public String PF_BM3_BMFC4 { get; set; }

        [DataMember]
        public String ST_BENCHMARK1CODE { get; set; }

        [DataMember]
        public String ST_BM1_IXCOVERAGE { get; set; }

        [DataMember]
        public String ST_BM1_BMFC4 { get; set; }

        [DataMember]
        public String ST_BENCHMARK2CODE { get; set; }

        [DataMember]
        public String ST_BM2_IXCOVERAGE { get; set; }

        [DataMember]
        public String ST_BM2_BMFC4 { get; set; }

        [DataMember]
        public String ST_BENCHMARK3CODE { get; set; }

        [DataMember]
        public String ST_BM3_IXCOVERAGE { get; set; }

        [DataMember]
        public String ST_BM3_BMFC4 { get; set; }

        [DataMember]
        public String PFSC_COMPTYPE { get; set; }

        [DataMember]
        public String HOLK_QUOCUR_PERFCUR_OVERRIDE { get; set; }

        [DataMember]
        public String HOLK_LEGNO { get; set; }

        [DataMember]
        public String PFKR_EXPOSUREPC_SWAP_EXCL_LEG2 { get; set; }

        [DataMember]
        public String PFKR_EXPOSUREPC_SWAP_INCL_LEG2 { get; set; }

        [DataMember]
        public String SECFC43_SEC_LIQUID_IL_CODE { get; set; }

        [DataMember]
        public String SECFC43_SEC_LIQUID_IL_NAME { get; set; }

        [DataMember]
        public String SECFC43_SEC_LIQUID_IL_SORT { get; set; }

        [DataMember]
        public String SECFC83_SECURITY_TRS_CPARTY { get; set; }

        [DataMember]
        public String SECFC4_NON_PERFORMING { get; set; }

        [DataMember]
        public String SECFC97_IS_PIK_PPN { get; set; }

        [DataMember]
        public String PFKR_MODDURUND { get; set; }
     

           }
}