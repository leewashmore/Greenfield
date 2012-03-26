using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GreenField.DAL;
using System.Data;
using GreenField.Web.DataContracts;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using GreenField.Web.Helpers;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ProxyDataOperations
    {
        private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));

                return dimensionEntity;
            }
        }

        private StringBuilder secShortList = new StringBuilder();

        [OperationContract]
        public String RetrievePrintValue()
        {
            return "Message from web service";
        }

        [OperationContract]
        public List<DetailedEstimates_Result> RetrieveDetailedEstimates(String companyName, String periodType, String estimateType)
        {
            List<DetailedEstimates_Result> result = new List<DetailedEstimates_Result>();
            using (ResearchEntities entity = new ResearchEntities())
            {
                String company = String.IsNullOrEmpty(companyName) ? null : companyName;
                String period = String.IsNullOrEmpty(periodType) ? null : periodType;
                String estimate = String.IsNullOrEmpty(estimateType) ? null : estimateType;

                var resultVal = entity.DetailedEstimates(null, company, period, estimate);

                result = resultVal.ToList();
            }

            return result;
        }

        [OperationContract]
        public List<GetCompanies_Result> RetrieveCompaniesList()
        {
            List<GetCompanies_Result> result = new List<GetCompanies_Result>();
            using (ResearchEntities entity = new ResearchEntities())
            {
                var resultVal = entity.GetCompanies(null, null);

                result = resultVal.ToList();
            }

            return result;
        }

        [OperationContract]
        public List<String> RetrieveDimensionDataListView()
        {
            List<String> Result = new List<String>();
            try
            {
                DimensionDataLocalService.SchemaClient client = new DimensionDataLocalService.SchemaClient();
                String[] ListResult = client.ListViews();

                Result = ListResult.ToList();
            }
            catch
            { //TODO: Add appropriate exception handler
            }
            return Result;
        }

        [OperationContract]
        public List<HoldingsData> RetrieveDimensionDataForSelectedView(String viewName)
        {
            List<HoldingsData> resultHoldings = new List<HoldingsData>();
            viewName = "U_POS_EXP_BASEVIEW WHERE " + viewName;

            try
            {
                DimensionDataLocalService.SchemaClient client = new DimensionDataLocalService.SchemaClient();
                DataSet result = client.RunView(viewName);

                foreach (DataRow row in result.Tables[0].Rows)
                {
                    HoldingsData holding = new HoldingsData()
                    {
                        PFCD_FROMDATE = row[0].ToString(),
                        PFCD_TODATE = row[1].ToString(),
                        PORTFOLIOTHEMEGROUPCODE = row[2].ToString(),
                        PORTFOLIOTHEMEGROUP = row[3].ToString(),
                        PORTFOLIOTHEMEGROUP_SORT = row[4].ToString(),
                        PORTFOLIOTHEMESUBGROUPCODE = row[5].ToString(),
                        PORTFOLIOTHEMESUBGROUP = row[6].ToString(),
                        PORTFOLIOTHEMESUBGROUP_SORT = row[7].ToString(),
                        PORTFOLIOCODE = row[8].ToString(),
                        PORTFOLIONAME = row[9].ToString(),
                        PORTFOLIO_SORT = row[10].ToString(),
                        IRP_LEVERAGELIMIT = row[11].ToString(),
                        PFCH_BALNOMVAL = row[12].ToString(),
                        PFCH_BALBOOKVALPC = row[13].ToString(),
                        PFCH_BALBOOKVALQC = row[14].ToString(),
                        PFCH_BALCOSTVALPC = row[15].ToString(),
                        PFCH_BALCOSTVALQC = row[16].ToString(),
                        PFCH_BALCOSTPRI = row[17].ToString(),
                        PFCH_BCOSTYLD = row[18].ToString(),
                        PFCH_UNSETAMTINPC = row[19].ToString(),
                        PFCH_UNSETAMTOUTPC = row[20].ToString(),
                        PFCH_BALBOOKPRI = row[21].ToString(),
                        PFKR_CURYLD = row[22].ToString(),
                        PFKR_FXRATEQP = row[23].ToString(),
                        PFKR_PFCHOLIK = row[24].ToString(),
                        PFKR_PV01 = row[25].ToString(),
                        PFCM_PFCM = row[26].ToString(),
                        HOLK_PORIK = row[27].ToString(),
                        HOLK_SECIK = row[28].ToString(),
                        BENCHMARKSECURITY = row[29].ToString(),
                        PFKR_ACR = row[30].ToString(),
                        PFKR_ACRVALPC = row[31].ToString(),
                        PFKR_CLEAN = row[32].ToString(),
                        PFKR_DIRTY = row[33].ToString(),
                        PFKR_DIRTYVALPC = row[34].ToString(),
                        PFKR_DIRTYVALQC = row[35].ToString(),
                        PFKR_EXPOSUREPC = row[36].ToString(),
                        PFKR_DOLDURVALDFPC = row[37].ToString(),
                        DOLDUR_0TO3YRS = row[38].ToString(),
                        DOLDUR_3TO5YRS = row[39].ToString(),
                        DOLDUR_5TO7YRS = row[40].ToString(),
                        DOLDUR_7TO10YRS = row[41].ToString(),
                        DOLDUR_5TO10YRS = row[42].ToString(),
                        DOLDUR_GT10YRS = row[43].ToString(),
                        DOLDUR_0TO3MODDUR = row[44].ToString(),
                        DOLDUR_3TO5MODDUR = row[45].ToString(),
                        DOLDUR_5TO7MODDUR = row[46].ToString(),
                        DOLDUR_7TO10MODDUR = row[47].ToString(),
                        DOLDUR_5TO10MODDUR = row[48].ToString(),
                        DOLDUR_GT10MODDUR = row[49].ToString(),
                        PFKR_MODSPRDUR = row[50].ToString(),
                        PFKR_YTMYLD = row[51].ToString(),
                        PFKR_YLDFIN = row[52].ToString(),
                        PFKR_COUDATENEXT = row[53].ToString(),
                        PFKR_COURATEBOOK = row[54].ToString(),
                        PFKR_YLDTOWORST = row[55].ToString(),
                        PFKR_MATDATE = row[56].ToString(),
                        PFKR_MAT = row[57].ToString(),
                        EXP_0TO3YRS = row[58].ToString(),
                        EXP_3TO5YRS = row[59].ToString(),
                        EXP_5TO7YRS = row[60].ToString(),
                        EXP_7TO10YRS = row[61].ToString(),
                        EXP_5TO10YRS = row[62].ToString(),
                        EXP_GT10YRS = row[63].ToString(),
                        EXP_0TO3MODDUR = row[64].ToString(),
                        EXP_3TO5MODDUR = row[65].ToString(),
                        EXP_5TO7MODDUR = row[66].ToString(),
                        EXP_7TO10MODDUR = row[67].ToString(),
                        EXP_5TO10MODDUR = row[68].ToString(),
                        EXP_GT10MODDUR = row[69].ToString(),
                        PFKR_MODDURDF = row[70].ToString(),
                        PFKR_UPRLCOSTSECPC = row[71].ToString(),
                        PFKR_UPRLCOSTCURPC = row[72].ToString(),
                        PFKR_UPRLBOOKSECPC = row[73].ToString(),
                        PFKR_UPRLBOOKCURPC = row[74].ToString(),
                        PFCP_PERPRLBOOKSECPC = row[75].ToString(),
                        PFCP_PERPRLBOOKCURPC = row[76].ToString(),
                        PFKR_PRICETYPE = row[77].ToString(),
                        PFKR_QUOTEDATE = row[78].ToString(),
                        COUNTRYZONECODE = row[79].ToString(),
                        COUNTRYZONENAME = row[80].ToString(),
                        COUNTRYZONE_SORT = row[81].ToString(),
                        CNTY_CTYIK = row[82].ToString(),
                        COUNTRYCODE = row[83].ToString(),
                        COUNTRYNAME = row[84].ToString(),
                        CNTY_NON_EM = row[85].ToString(),
                        HOLK_QUOCUR = row[86].ToString(),
                        CURR_NON_EM = row[87].ToString(),
                        SEC_SECFC90_CURRENCYOFRISK = row[88].ToString(),
                        CURR_RISK_CUR = row[89].ToString(),
                        CURR_RISK_NON_EM = row[90].ToString(),
                        CURR_QUORISK_BEST = row[91].ToString(),
                        CURR_QUORISK_NONEM_BEST = row[92].ToString(),
                        IRP_INSTYPE_GROUPS_1 = row[93].ToString(),
                        IRP_INSTYPE_GROUPS_2 = row[94].ToString(),
                        ISIN = row[95].ToString(),
                        SEC_SECFC2IK = row[96].ToString(),
                        SEC_SECSHORT = row[97].ToString(),
                        SEC_SECNAME = row[98].ToString(),
                        SIDS_IDENT = row[99].ToString(),
                        IXCT_UNDERLY = row[100].ToString(),
                        IXCS_SECNO = row[101].ToString(),
                        IXCS_SECSHORT = row[102].ToString(),
                        SEC_SECNAMEROW1 = row[103].ToString(),
                        SEC_ISSVOL = row[104].ToString(),
                        SEC_ISSVOLOUT = row[105].ToString(),
                        SEC_ISSVOL_BEST = row[106].ToString(),
                        SEC_DEFQUOCUR = row[107].ToString(),
                        SEC_INSTYPE = row[108].ToString(),
                        INSTRUMENTTYPE = row[109].ToString(),
                        SEC_INSTYPE_NAME = row[110].ToString(),
                        INSTRUMENTTYPENAME = row[111].ToString(),
                        SEC_SECFC88_LIQUIDITY = row[112].ToString(),
                        SEC_SECFC89_BETA = row[113].ToString(),
                        SECT_SECTYPE = row[114].ToString(),
                        SECT_SORT = row[115].ToString(),
                        IBI_SPREAD = row[116].ToString(),
                        SECURITYTHEMECODE = row[117].ToString(),
                        SECURITYTHEMENAME = row[118].ToString(),
                        SECURITYTHEME_SORT = row[119].ToString(),
                        PARG_PARGRP = row[120].ToString(),
                        PARG_PARGRPNAME = row[121].ToString(),
                        PART_PARIK = row[122].ToString(),
                        PART_PAR = row[123].ToString(),
                        ISSUERNAME = row[124].ToString(),
                        OWNERSHIPCODE = row[125].ToString(),
                        OWNERSHIPNAME = row[126].ToString(),
                        OWNERSHIP_SORT = row[127].ToString(),
                        BM1_BMIK = row[128].ToString(),
                        BM2_BMIK = row[129].ToString(),
                        BM3_BMIK = row[130].ToString(),
                        SECFC15_SECFC15 = row[131].ToString(),
                        SECFC15_PERFORMANCECURRENCY = row[132].ToString(),
                        SECFC5_OWNERSHIP_PCENT = row[133].ToString(),
                        COUNTRYCODEEXP = row[134].ToString(),
                        COUNTRYNAMEEXP = row[135].ToString(),
                        COUNTRYZONECODEEXP = row[136].ToString(),
                        COUNTRYZONENAMEEXP = row[137].ToString(),
                        COUNTRYZONESORTEXP = row[138].ToString(),
                        ASHMMORE_EXP_DEF = row[139].ToString(),
                        ASHMORE_EXP_DEF_CCY = row[140].ToString(),
                        ASHMORE_EXP_BRK = row[141].ToString(),
                        SECFC12_MANAGEDBYCODE = row[142].ToString(),
                        SECFC12_MANAGEDBYNAME = row[143].ToString(),
                        SECFC17_QUASISOVGUARCODE = row[144].ToString(),
                        SECFC17_QUASISOVGUARNAME = row[145].ToString(),
                        SECFC18_SECURITYLIQUIDITYCODE = row[146].ToString(),
                        SECFC18_SECURITYLIQUIDITYNAME = row[147].ToString(),
                        SECFC18_SECURITYLIQUIDITYSORT = row[148].ToString(),
                        SECFC19_ORDERREASON = row[149].ToString(),
                        SECFC19_ORDERREASONSORT = row[150].ToString(),
                        SECFC20_IRPLOOKTHRUPOSS = row[151].ToString(),
                        TOTAL_CORPORATE_RESTRICT_LIMIT = row[152].ToString(),
                        TOTAL_CORPORATE_HOUSE_LIMIT = row[153].ToString(),
                        LEVERAGE_LIMIT = row[154].ToString(),
                        EXTERNAL_DEBT_SUB_GROUP = row[155].ToString(),
                        CORP_ISSUER_LIMIT = row[156].ToString(),
                        BENCHMARKSECTHEMECODE = row[157].ToString(),
                        PF_BENCHMARK1CODE = row[158].ToString(),
                        PF_BM1_IXCOVERAGE = row[159].ToString(),
                        PF_BM1_BMFC4 = row[160].ToString(),
                        PF_BENCHMARK2CODE = row[161].ToString(),
                        PF_BM2_IXCOVERAGE = row[162].ToString(),
                        PF_BM2_BMFC4 = row[163].ToString(),
                        PF_BENCHMARK3CODE = row[164].ToString(),
                        PF_BM3_IXCOVERAGE = row[165].ToString(),
                        PF_BM3_BMFC4 = row[166].ToString(),
                        ST_BENCHMARK1CODE = row[167].ToString(),
                        ST_BM1_IXCOVERAGE = row[168].ToString(),
                        ST_BM1_BMFC4 = row[169].ToString(),
                        ST_BENCHMARK2CODE = row[170].ToString(),
                        ST_BM2_IXCOVERAGE = row[171].ToString(),
                        ST_BM2_BMFC4 = row[172].ToString(),
                        ST_BENCHMARK3CODE = row[173].ToString(),
                        ST_BM3_IXCOVERAGE = row[174].ToString(),
                        ST_BM3_BMFC4 = row[175].ToString(),
                        PFSC_COMPTYPE = row[176].ToString(),
                        HOLK_QUOCUR_PERFCUR_OVERRIDE = row[177].ToString(),
                        HOLK_LEGNO = row[178].ToString(),
                        PFKR_EXPOSUREPC_SWAP_EXCL_LEG2 = row[179].ToString(),
                        PFKR_EXPOSUREPC_SWAP_INCL_LEG2 = row[180].ToString(),
                        SECFC43_SEC_LIQUID_IL_CODE = row[181].ToString(),
                        SECFC43_SEC_LIQUID_IL_NAME = row[182].ToString(),
                        SECFC43_SEC_LIQUID_IL_SORT = row[183].ToString(),
                        SECFC83_SECURITY_TRS_CPARTY = row[184].ToString(),
                        SECFC4_NON_PERFORMING = row[185].ToString(),
                        SECFC97_IS_PIK_PPN = row[186].ToString(),
                        PFKR_MODDURUND = row[187].ToString(),
                        SEC_SECFC78_ZSPREAD = row[188].ToString()
                    };
                    resultHoldings.Add(holding);
                }
            }
            catch
            {
                //TODO: Add appropriate exception handler
            }
            return resultHoldings;
        }

        [OperationContract]
        public List<ConsensusEstimates_Result> RetrieveConsensusEstimates(String companyName, String periodType)
        {
            List<ConsensusEstimates_Result> result = new List<ConsensusEstimates_Result>();
            using (ResearchEntities entity = new ResearchEntities())
            {
                String company = String.IsNullOrEmpty(companyName) ? null : companyName;
                String period = String.IsNullOrEmpty(periodType) ? null : periodType;

                var resultVal = entity.ConsensusEstimates(null, company, period);

                result = resultVal.ToList();
            }

            return result;
        }

        [OperationContract]
        public List<PerformanceData> RetrievePerformanceDataForSelectedView(String viewName)
        {
            List<PerformanceData> performanceData = new List<PerformanceData>();
            viewName = "irp_perf_bmk_curr WHERE " + viewName;
            try
            {
                DimensionDataLocalService.SchemaClient client = new DimensionDataLocalService.SchemaClient();
                DataSet result = client.RunView(viewName);

                foreach (DataRow row in result.Tables[0].Rows)
                {
                    PerformanceData performanceDataItem = new PerformanceData()
                    {
                        PORTFOLIOCODE = row[0].ToString(),
                        QUOTATIONCURRENCYCODE = row[1].ToString(),
                        PERFORMANCECURRENCYCODE = row[2].ToString(),
                        CONTRIBRETBM1CURRRCMTD = Convert.ToDecimal(row[3].ToString()),
                        CONTRIBRETBM1CURRTOPRCMTD = Convert.ToDecimal(row[4].ToString()),
                        IRP_TWRBM1CURRMTDRC = Convert.ToDecimal(row[5].ToString()),
                        A_PERFREP_BM1WEIGHT_TO_TOP_EOD = Convert.ToDecimal(row[6].ToString()),
                        A_PERFREP_BM1WEIGHT_TO_TOP_SOD = Convert.ToDecimal(row[7].ToString()),
                        A_PERFREP_TO_DATE = Convert.ToDateTime(row[8].ToString())
                    };

                    performanceData.Add(performanceDataItem);
                }
            }
            catch
            {
                //TODO: Add appropriate exception handler
            }
            return performanceData;
        }


        [OperationContract]
        public List<ReferenceData> RetrieveReferenceDataForSelectedView(String viewName)
        {
            List<ReferenceData> referenceData = new List<ReferenceData>();
            viewName = "IRP2_SEC_MASTER_BASEVIEW WHERE " + viewName;
            try
            {
                DimensionDataService.SchemaClient client = new DimensionDataService.SchemaClient();
                DataSet result = client.RunView(viewName);

                foreach (DataRow row in result.Tables[0].Rows)
                {

                    //if (row[8].ToString().Equals("AREF"))
                    //{
                    ReferenceData ReferenceDataItem = new ReferenceData()
                {
                    PFCD_FROMDATE = row[0].ToString(),
                    PFCD_TODATE = row[1].ToString(),
                    PORTFOLIOTHEMEGROUPCODE = row[2].ToString(),
                    PORTFOLIOTHEMEGROUP = row[3].ToString(),
                    PORTFOLIOTHEMEGROUP_SORT = row[4].ToString(),
                    PORTFOLIOTHEMESUBGROUPCODE = row[5].ToString(),
                    PORTFOLIOTHEMESUBGROUP = row[6].ToString(),
                    PORTFOLIOTHEMESUBGROUP_SORT = row[7].ToString(),
                    PORTFOLIOCODE = row[8].ToString(),
                    PORTFOLIONAME = row[9].ToString(),
                    PORTFOLIO_SORT = row[10].ToString(),
                    IRP_LEVERAGELIMIT = row[11].ToString(),
                    PFCH_BALNOMVAL = row[12].ToString(),
                    PFCH_BALBOOKVALPC = row[13].ToString(),
                    PFCH_BALBOOKVALQC = row[14].ToString(),
                    PFCH_BALCOSTVALPC = row[15].ToString(),
                    PFCH_BALCOSTVALQC = row[16].ToString(),
                    PFCH_BALCOSTPRI = row[17].ToString(),
                    PFCH_BCOSTYLD = row[18].ToString(),
                    PFCH_UNSETAMTINPC = row[19].ToString(),
                    PFCH_UNSETAMTOUTPC = row[20].ToString(),
                    PFCH_BALBOOKPRI = row[21].ToString(),
                    PFKR_CURYLD = row[22].ToString(),
                    PFKR_FXRATEQP = row[23].ToString(),
                    PFKR_PFCHOLIK = row[24].ToString(),
                    PFKR_PV01 = row[25].ToString(),
                    PFCM_PFCM = row[26].ToString(),
                    HOLK_PORIK = row[27].ToString(),
                    HOLK_SECIK = row[28].ToString(),
                    BENCHMARKSECURITY = row[29].ToString(),
                    PFKR_ACR = row[30].ToString(),
                    PFKR_ACRVALPC = row[31].ToString(),
                    PFKR_CLEAN = row[32].ToString(),
                    PFKR_DIRTY = row[33].ToString(),
                    PFKR_DIRTYVALPC = row[34].ToString(),
                    PFKR_DIRTYVALQC = row[35].ToString(),
                    PFKR_EXPOSUREPC = row[36].ToString(),
                    PFKR_DOLDURVALDFPC = row[37].ToString(),
                    DOLDUR_0TO3YRS = row[38].ToString(),
                    DOLDUR_3TO5YRS = row[39].ToString(),
                    DOLDUR_5TO7YRS = row[40].ToString(),
                    DOLDUR_7TO10YRS = row[41].ToString(),
                    DOLDUR_5TO10YRS = row[42].ToString(),
                    DOLDUR_GT10YRS = row[43].ToString(),
                    DOLDUR_0TO3MODDUR = row[44].ToString(),
                    DOLDUR_3TO5MODDUR = row[45].ToString(),
                    DOLDUR_5TO7MODDUR = row[46].ToString(),
                    DOLDUR_7TO10MODDUR = row[47].ToString(),
                    DOLDUR_5TO10MODDUR = row[48].ToString(),
                    DOLDUR_GT10MODDUR = row[49].ToString(),
                    PFKR_MODSPRDUR = row[50].ToString(),
                    PFKR_YTMYLD = row[51].ToString(),
                    PFKR_YLDFIN = row[52].ToString(),
                    PFKR_COUDATENEXT = row[53].ToString(),
                    PFKR_COURATEBOOK = row[54].ToString(),
                    PFKR_YLDTOWORST = row[55].ToString(),
                    PFKR_MATDATE = row[56].ToString(),
                    PFKR_MAT = row[57].ToString(),
                    EXP_0TO3YRS = row[58].ToString(),
                    EXP_3TO5YRS = row[59].ToString(),
                    EXP_5TO7YRS = row[60].ToString(),
                    EXP_7TO10YRS = row[61].ToString(),
                    EXP_5TO10YRS = row[62].ToString(),
                    EXP_GT10YRS = row[63].ToString(),
                    EXP_0TO3MODDUR = row[64].ToString(),
                    EXP_3TO5MODDUR = row[65].ToString(),
                    EXP_5TO7MODDUR = row[66].ToString(),
                    EXP_7TO10MODDUR = row[67].ToString(),
                    EXP_5TO10MODDUR = row[68].ToString(),
                    EXP_GT10MODDUR = row[69].ToString(),
                    PFKR_MODDURDF = row[70].ToString(),
                    PFKR_UPRLCOSTSECPC = row[71].ToString(),
                    PFKR_UPRLCOSTCURPC = row[72].ToString(),
                    PFKR_UPRLBOOKSECPC = row[73].ToString(),
                    PFKR_UPRLBOOKCURPC = row[74].ToString(),
                    PFCP_PERPRLBOOKSECPC = row[75].ToString(),
                    PFCP_PERPRLBOOKCURPC = row[76].ToString(),
                    PFKR_PRICETYPE = row[77].ToString(),
                    PFKR_QUOTEDATE = row[78].ToString(),
                    COUNTRYZONECODE = row[79].ToString(),
                    COUNTRYZONENAME = row[80].ToString(),
                    COUNTRYZONE_SORT = row[81].ToString(),
                    CNTY_CTYIK = row[82].ToString(),
                    COUNTRYCODE = row[83].ToString(),
                    COUNTRYNAME = row[84].ToString(),
                    CNTY_NON_EM = row[85].ToString(),
                    HOLK_QUOCUR = row[86].ToString(),
                    CURR_NON_EM = row[87].ToString(),
                    SEC_SECFC90_CURRENCYOFRISK = row[88].ToString(),
                    CURR_RISK_CUR = row[89].ToString(),
                    CURR_RISK_NON_EM = row[90].ToString(),
                    CURR_QUORISK_BEST = row[91].ToString(),
                    CURR_QUORISK_NONEM_BEST = row[92].ToString(),
                    IRP_INSTYPE_GROUPS_1 = row[93].ToString(),
                    IRP_INSTYPE_GROUPS_2 = row[94].ToString(),
                    ISIN = row[95].ToString(),
                    SEC_SECFC2IK = row[96].ToString(),
                    SEC_SECSHORT = row[97].ToString(),
                    SEC_SECNAME = row[98].ToString(),
                    SIDS_IDENT = row[99].ToString(),
                    IXCT_UNDERLY = row[100].ToString(),
                    IXCS_SECNO = row[101].ToString(),
                    IXCS_SECSHORT = row[102].ToString(),
                    SEC_SECNAMEROW1 = row[103].ToString(),
                    SEC_ISSVOL = row[104].ToString(),
                    SEC_ISSVOLOUT = row[105].ToString(),
                    SEC_ISSVOL_BEST = row[106].ToString(),
                    SEC_DEFQUOCUR = row[107].ToString(),
                    SEC_INSTYPE = row[108].ToString(),
                    INSTRUMENTTYPE = row[109].ToString(),
                    SEC_INSTYPE_NAME = row[110].ToString(),
                    INSTRUMENTTYPENAME = row[111].ToString(),
                    SEC_SECFC88_LIQUIDITY = row[112].ToString(),
                    SEC_SECFC89_BETA = row[113].ToString(),
                    SECT_SECTYPE = row[114].ToString(),
                    SECT_SORT = row[115].ToString(),
                    IBI_SPREAD = row[116].ToString(),
                    SECURITYTHEMECODE = row[117].ToString(),
                    SECURITYTHEMENAME = row[118].ToString(),
                    SECURITYTHEME_SORT = row[119].ToString(),
                    PARG_PARGRP = row[120].ToString(),
                    PARG_PARGRPNAME = row[121].ToString(),
                    PART_PARIK = row[122].ToString(),
                    PART_PAR = row[123].ToString(),
                    ISSUERNAME = row[124].ToString(),
                    OWNERSHIPCODE = row[125].ToString(),
                    OWNERSHIPNAME = row[126].ToString(),
                    OWNERSHIP_SORT = row[127].ToString(),
                    BM1_BMIK = row[128].ToString(),
                    BM2_BMIK = row[129].ToString(),
                    BM3_BMIK = row[130].ToString(),
                    SECFC15_SECFC15 = row[131].ToString(),
                    SECFC15_PERFORMANCECURRENCY = row[132].ToString(),
                    SECFC5_OWNERSHIP_PCENT = row[133].ToString(),
                    COUNTRYCODEEXP = row[134].ToString(),
                    COUNTRYNAMEEXP = row[135].ToString(),
                    COUNTRYZONECODEEXP = row[136].ToString(),
                    COUNTRYZONENAMEEXP = row[137].ToString(),
                    COUNTRYZONESORTEXP = row[138].ToString(),
                    ASHMMORE_EXP_DEF = row[139].ToString(),
                    ASHMORE_EXP_DEF_CCY = row[140].ToString(),
                    ASHMORE_EXP_BRK = row[141].ToString(),
                    SECFC12_MANAGEDBYCODE = row[142].ToString(),
                    SECFC12_MANAGEDBYNAME = row[143].ToString(),
                    SECFC17_QUASISOVGUARCODE = row[144].ToString(),
                    SECFC17_QUASISOVGUARNAME = row[145].ToString(),
                    SECFC18_SECURITYLIQUIDITYCODE = row[146].ToString(),
                    SECFC18_SECURITYLIQUIDITYNAME = row[147].ToString(),
                    SECFC18_SECURITYLIQUIDITYSORT = row[148].ToString(),
                    SECFC19_ORDERREASON = row[149].ToString(),
                    SECFC19_ORDERREASONSORT = row[150].ToString(),
                    SECFC20_IRPLOOKTHRUPOSS = row[151].ToString(),
                    TOTAL_CORPORATE_RESTRICT_LIMIT = row[152].ToString(),
                    TOTAL_CORPORATE_HOUSE_LIMIT = row[153].ToString(),
                    LEVERAGE_LIMIT = row[154].ToString(),
                    EXTERNAL_DEBT_SUB_GROUP = row[155].ToString(),
                    CORP_ISSUER_LIMIT = row[156].ToString(),
                    BENCHMARKSECTHEMECODE = row[157].ToString(),
                    PF_BENCHMARK1CODE = row[158].ToString(),
                    PF_BM1_IXCOVERAGE = row[159].ToString(),
                    PF_BM1_BMFC4 = row[160].ToString(),
                    PF_BENCHMARK2CODE = row[161].ToString(),
                    PF_BM2_IXCOVERAGE = row[162].ToString(),
                    PF_BM2_BMFC4 = row[163].ToString(),
                    PF_BENCHMARK3CODE = row[164].ToString(),
                    PF_BM3_IXCOVERAGE = row[165].ToString(),
                    PF_BM3_BMFC4 = row[166].ToString(),
                    ST_BENCHMARK1CODE = row[167].ToString(),
                    ST_BM1_IXCOVERAGE = row[168].ToString(),
                    ST_BM1_BMFC4 = row[169].ToString(),
                    ST_BENCHMARK2CODE = row[170].ToString(),
                    ST_BM2_IXCOVERAGE = row[171].ToString(),
                    ST_BM2_BMFC4 = row[172].ToString(),
                    ST_BENCHMARK3CODE = row[173].ToString(),
                    ST_BM3_IXCOVERAGE = row[174].ToString(),
                    ST_BM3_BMFC4 = row[175].ToString(),
                    PFSC_COMPTYPE = row[176].ToString(),
                    HOLK_QUOCUR_PERFCUR_OVERRIDE = row[177].ToString(),
                    HOLK_LEGNO = row[178].ToString(),
                    PFKR_EXPOSUREPC_SWAP_EXCL_LEG2 = row[179].ToString(),
                    PFKR_EXPOSUREPC_SWAP_INCL_LEG2 = row[180].ToString(),
                    SECFC43_SEC_LIQUID_IL_CODE = row[181].ToString(),
                    SECFC43_SEC_LIQUID_IL_NAME = row[182].ToString(),
                    SECFC43_SEC_LIQUID_IL_SORT = row[183].ToString(),
                    SECFC83_SECURITY_TRS_CPARTY = row[184].ToString(),
                    SECFC4_NON_PERFORMING = row[185].ToString(),
                    SECFC97_IS_PIK_PPN = row[186].ToString(),
                    PFKR_MODDURUND = row[187].ToString(),
                    SEC_SECFC78_ZSPREAD = row[188].ToString(),
                    SEC_SECIK = row[189].ToString(),
                    SECFC18_SECFC18IK = row[190].ToString(),
                    SECFC19_SECFC19IK = row[191].ToString(),
                    SECFC2_SECFC2SHORT = row[192].ToString(),
                    SECFC20_SECFC20IK = row[193].ToString(),
                    CURR_QT_CUR = row[194].ToString(),
                    SEC_QUOCUR_PERFCUR_OVERRIDE = row[195].ToString(),
                    SEC_TRS_UNDERLY = row[196].ToString()

                };

                    referenceData.Add(ReferenceDataItem);
                }

            }
            //}

            catch
            {
                //TODO: Add appropriate exception handler
            }
            return referenceData;
        }


        [OperationContract]
        public List<AggregatedData> RetrieveAggregateDataForSelectedView(String portfolioName)
        {
            List<AggregatedData> resultAggData = new List<AggregatedData>();
            try
            {
                DimensionDataLocalService.SchemaClient client = new DimensionDataLocalService.SchemaClient();
                DataSet result = client.RunView("U_POS_EXP_BASEVIEW");
                DataRow[] fileteredValues = result.Tables[0].Select(String.Format("PORTFOLIOCODE='{0}'", portfolioName));

                foreach (DataRow row in fileteredValues)
                {
                    AggregatedData holding = new AggregatedData()
                    {
                        PFCD_FROMDATE = row[0].ToString(),
                        PORTFOLIOTHEMEGROUPCODE = row[2].ToString(),
                        PORTFOLIOCODE = row[8].ToString(),
                        PORTFOLIONAME = row[9].ToString(),
                        PFCH_BALBOOKVALPC = Convert.ToDouble(row[13].ToString()),
                        PFKR_FXRATEQP = Convert.ToDouble(row[23].ToString()),
                        COUNTRYZONENAME = row[80].ToString(),
                        COUNTRYCODE = row[83].ToString(),
                        COUNTRYNAME = row[84].ToString(),
                        CURR_QUORISK_BEST = row[91].ToString(),
                        ISIN = row[95].ToString(),
                        SEC_SECSHORT = row[97].ToString(),
                        SEC_SECNAME = row[98].ToString(),
                        SEC_INSTYPE_NAME = row[110].ToString(),
                        //COMPANY_NAME = row[14].ToString(),

                    };
                    resultAggData.Add(holding);
                    secShortList.Append(holding.SEC_SECSHORT);
                    secShortList.Append(",");
                }

                secShortList.Append("CNSHENDEVE");
                //secShortList.Remove(secShortList.Length - 1, 1);

                List<A_PROC_Result> annualResult = new List<A_PROC_Result>();
                using (ResearchEntities entity = new ResearchEntities())
                {
                    var resultList = entity.A_PROC(secShortList.ToString());
                    annualResult = resultList.ToList();
                }

                for (int i = 0; i < annualResult.Count; i++)
                {
                    AggregatedData data = (from p in resultAggData
                                           where p.SEC_SECSHORT.Equals(annualResult[i].SEC_SECSHORT)
                                           select p).First();

                    data.NET_INCOME_ACT_2009 = annualResult[i].YEAR2009;
                    data.NET_INCOME_ACT_2010 = annualResult[i].YEAR2010;
                    data.NET_INCOME_EST_2011 = annualResult[i].YEAR2011;
                    data.NET_INCOME_EST_2012 = annualResult[i].YEAR2012;
                    data.NET_INCOME_EST_2013 = annualResult[i].YEAR2013;
                }
            }
            catch
            {
                //TODO: Add appropriate exception handler
            }
            return resultAggData;
        }

        [OperationContract]
        public List<String> RetrievePortfolioNames(String viewName)
        {
            List<String> result = new List<String>();
            try
            {
                DimensionDataLocalService.SchemaClient client = new DimensionDataLocalService.SchemaClient();

                DataSet resultDataSet = client.RunView(viewName);

                foreach (DataRow row in resultDataSet.Tables[0].Rows)
                {
                    result.Add(row[8].ToString());
                }
            }
            catch
            { //TODO: Add appropriate exception handler
            }
            return result.Distinct().ToList();
        }

        [OperationContract]
        public List<SecurityOverviewData> RetrieveSecurityReferenceData()
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<DimensionEntitiesService.GF_SECURITY_BASEVIEW> data = entity.GF_SECURITY_BASEVIEW.ToList();

                List<SecurityOverviewData> result = new List<SecurityOverviewData>();
                foreach (DimensionEntitiesService.GF_SECURITY_BASEVIEW record in data)
                {
                    result.Add(new SecurityOverviewData()
                {
                    IssueName = record.ISSUE_NAME,
                    Ticker = record.TICKER,
                    Country = record.ISO_COUNTRY_CODE,
                    Sector = record.GICS_SECTOR_NAME,
                    Industry = record.GICS_INDUSTRY_NAME,
                    SubIndustry = record.GICS_SUB_INDUSTRY_NAME,
                    PrimaryAnalyst = record.ASHMOREEMM_PRIMARY_ANALYST,
                    Currency = record.TRADING_CURRENCY,
                    FiscalYearEnd = record.FISCAL_YEAR_END,
                    Website = record.WEBSITE,
                    Description = record.BLOOMBERG_DESCRIPTION
                });
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public SecurityOverviewData RetrieveSecurityReferenceDataByTicker(string ticker)
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW.Where(o => o.TICKER == ticker).First();

                SecurityOverviewData result = new SecurityOverviewData()
                {
                    IssueName = data.ISSUE_NAME,
                    Ticker = data.TICKER,
                    Country = data.ISO_COUNTRY_CODE,
                    Sector = data.GICS_SECTOR_NAME,
                    Industry = data.GICS_INDUSTRY_NAME,
                    SubIndustry = data.GICS_SUB_INDUSTRY_NAME,
                    PrimaryAnalyst = data.ASHMOREEMM_PRIMARY_ANALYST,
                    Currency = data.TRADING_CURRENCY,
                    FiscalYearEnd = data.FISCAL_YEAR_END,
                    Website = data.WEBSITE,
                    Description = data.BLOOMBERG_DESCRIPTION
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieving the Pricing Reference Data for selected Entity.
        /// </summary>
        /// <param name="entityIdentifiers"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="totalReturnCheck"></param>
        /// <param name="frequencyDuration"></param>
        /// <param name="chartEntityTypes"></param>
        /// <returns>List of PricingReferenceData</returns>
        [OperationContract]
        public List<PricingReferenceData> RetrievePricingReferenceData(List<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyDuration, bool chartEntityTypes)
        {
            try
            {
                decimal objAdjustedDollarPrice = 0;
                decimal objPreviousDailySpotFx = 0;
                decimal objIndexedPrice = 0;
                decimal objReturn = 0;

                decimal curPrice = 0;
                decimal curReturn = 0;
                decimal calculatedPrice = 0;
                decimal previousPrice = 0;
                decimal previousFXAdjust = 0;
                decimal calculatedFXPrice = 0;
                decimal fxAdjusted = 0;
                string entityType = "";
                string entityInstrumentID = "";
                DateTime startDate = Convert.ToDateTime(startDateTime);
                DateTime endDate = Convert.ToDateTime(endDateTime);

                //List Containing the names of Securities/Commodities/Indexes to be added
                List<string> entityNames = (from p in entityIdentifiers
                                            select p.InstrumentID).ToList();

                List<PricingReferenceData> pricingDataResult = new List<PricingReferenceData>();

                DimensionEntitiesService.Entities entity = DimensionEntity;

                //Plotting a Single Line Chart
                #region SingleLineChart

                if (entityIdentifiers.Count() == 1)
                {
                    entityInstrumentID = Convert.ToString(entityIdentifiers[0].InstrumentID);
                    entityType = Convert.ToString(entityIdentifiers[0].Type);

                    List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData = entity.GF_PRICING_BASEVIEW
                        .Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >= startDate) && (r.FROMDATE < endDate))
                        .OrderByDescending(res => res.FROMDATE).ToList();

                    // Calcluating the values of curPrice,curReturn,calculatedPrice
                    if (dimensionServicePricingData.Count != 0)
                    {
                        curPrice = Convert.ToDecimal(dimensionServicePricingData[0].DAILY_CLOSING_PRICE);
                        curReturn = (totalReturnCheck) ? (Convert.ToDecimal(dimensionServicePricingData[0].DAILY_GROSS_RETURN)) : (Convert.ToDecimal(dimensionServicePricingData[0].DAILY_PRICE_RETURN));
                        calculatedPrice = curPrice;

                        foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                        {
                            PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                            objPricingReferenceData.Type = pricingItem.TYPE;
                            objPricingReferenceData.Ticker = pricingItem.TICKER;
                            objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                            objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                            objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                            objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                            objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                            objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                            objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                            objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);

                            //Checking if the Item is the first item in the list
                            if ((pricingItem.INSTRUMENT_ID == dimensionServicePricingData[0].INSTRUMENT_ID) && (pricingItem.FROMDATE == dimensionServicePricingData[0].FROMDATE))
                            {
                                // if it is the first item in the list then simply save the value of calculated price
                                objPricingReferenceData.IndexedPrice = calculatedPrice;
                            }
                            else
                            {
                                //if it is not the first item then executing the logic.
                                calculatedPrice = (curPrice / (curReturn + 1));
                                curPrice = calculatedPrice;
                                objPricingReferenceData.IndexedPrice = calculatedPrice;
                                curReturn = (totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN));
                            }
                            pricingDataResult.Add(objPricingReferenceData);
                        }
                    }
                }

                #endregion

                //Plotting a Multi-Line Comparison Chart
                #region MultiLineChart

                if (entityIdentifiers.Count() > 1)
                {
                    foreach (EntitySelectionData item in entityIdentifiers)
                    {
                        if (Convert.ToString(item.Type) == "SECURITY")
                        {
                            entityInstrumentID = Convert.ToString(item.InstrumentID);
                            List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData =
                                entity.GF_PRICING_BASEVIEW.Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >=
                                    startDate) && (r.FROMDATE <= endDate)).OrderByDescending(res => res.FROMDATE).ToList();


                            if (dimensionServicePricingData.Count != 0)
                            {
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                                {
                                    PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                                    objPricingReferenceData.Type = pricingItem.TYPE;
                                    objPricingReferenceData.Ticker = pricingItem.TICKER;
                                    objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                                    objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                                    objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                                    objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                                    objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                                    objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                                    objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);

                                    //Checking if the current object is first in the series
                                    if (pricingItem.FROMDATE == dimensionServicePricingData[0].FROMDATE)
                                    {
                                        objPricingReferenceData.AdjustedDollarPrice = (Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE) / objPricingReferenceData.DailySpotFX);
                                    }
                                    else
                                    {
                                        objPricingReferenceData.AdjustedDollarPrice =
                                            objAdjustedDollarPrice / ((1 + (objReturn / 100)) * (Convert.ToDecimal(pricingItem.DAILY_SPOT_FX) / objPreviousDailySpotFx));
                                    }
                                    objAdjustedDollarPrice = objPricingReferenceData.AdjustedDollarPrice;
                                    objPreviousDailySpotFx = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objReturn = ((totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN)));
                                    pricingDataResult.Add(objPricingReferenceData);
                                }
                            }
                        }

                        else if ((Convert.ToString(item.Type) == "COMMODITY") || ((Convert.ToString(item.Type) == "INDEX")) || ((Convert.ToString(item.Type) == "FX")))
                        {
                            entityInstrumentID = Convert.ToString(item.InstrumentID);
                            List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData =
                                entity.GF_PRICING_BASEVIEW.Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >=
                                    startDate) && (r.FROMDATE <= endDate)).OrderBy(res => res.FROMDATE).ToList();

                            if (dimensionServicePricingData.Count != 0)
                            {
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                                {
                                    PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                                    objPricingReferenceData.Type = pricingItem.TYPE;
                                    objPricingReferenceData.Ticker = pricingItem.TICKER;
                                    objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                                    objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                                    objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                                    objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                                    objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                                    objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                                    objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);
                                    objReturn = ((totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN)));
                                    objPricingReferenceData.AdjustedDollarPrice =
                                        (Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE) / Convert.ToDecimal(pricingItem.DAILY_SPOT_FX));
                                    pricingDataResult.Add(objPricingReferenceData);
                                }
                            }
                        }

                        pricingDataResult = pricingDataResult.OrderBy(r => r.FromDate).ToList();

                        pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).First().IndexedPrice = 100;

                        foreach (PricingReferenceData objPricingDataResult in pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).ToList())
                        {
                            if (objPricingDataResult.FromDate == pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).First().FromDate)
                            {
                                objAdjustedDollarPrice = objPricingDataResult.AdjustedDollarPrice;
                                objIndexedPrice = objPricingDataResult.IndexedPrice;
                            }
                            else
                            {
                                objPricingDataResult.IndexedPrice = (objPricingDataResult.AdjustedDollarPrice / objAdjustedDollarPrice) * objIndexedPrice;
                                objIndexedPrice = objPricingDataResult.IndexedPrice;
                                objAdjustedDollarPrice = objPricingDataResult.AdjustedDollarPrice;
                            }
                        }
                    }

                    foreach (PricingReferenceData item in pricingDataResult)
                    {
                        item.IndexedPrice = item.IndexedPrice - 100;
                    }
                }

                #endregion

                #region FilterDataAccordingToFrequency

                List<DateTime> endDates = new List<DateTime>();
                endDates = (from p in pricingDataResult
                            select p.FromDate).Distinct().ToList();

                List<DateTime> allEndDates = new List<DateTime>();

                allEndDates = FrequencyCalculator.RetrieveDatesAccordingToFrequency(endDates, startDateTime, endDateTime, frequencyDuration);

                List<PricingReferenceData> result = new List<PricingReferenceData>();

                if (frequencyDuration != "Daily")
                {
                    foreach (EntitySelectionData item in entityIdentifiers)
                    {
                        List<PricingReferenceData> individualSeriesResult = RetrievePricingDataAccordingFrequency(pricingDataResult.Where(r => r.InstrumentID == item.InstrumentID).ToList(), allEndDates);
                        result.AddRange(individualSeriesResult);
                    }

                }
                else
                {
                    result = pricingDataResult;
                }
                #endregion

                return result;
            }

            catch (Exception)
            {
                return null;
            }

        }


        [OperationContract]
        public List<EntitySelectionData> RetrieveEntitySelectionData()
        {
            try
            {
                List<DimensionEntitiesService.GF_SELECTION_BASEVIEW> data = DimensionEntity.GF_SELECTION_BASEVIEW.ToList();
                List<EntitySelectionData> result = new List<EntitySelectionData>();


                foreach (DimensionEntitiesService.GF_SELECTION_BASEVIEW record in data)
                {
                    result.Add(new EntitySelectionData()
                    {
                        ShortName = record.SHORT_NAME,
                        LongName = record.LONG_NAME,
                        InstrumentID = record.INSTRUMENT_ID,
                        Type = record.TYPE,
                        SecurityType = record.SECURITY_TYPE
                    });
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<FundSelectionData> RetrieveFundSelectionData()
        {
            try
            {
                List<FundSelectionData> result = new List<FundSelectionData>();

                for (int i = 0; i < 10; i++)
                {
                    result.Add(new FundSelectionData()
                    {
                        Category = i % 2 == 0 ? "Funds" : "Composites",
                        Name = i % 2 == 0 ? "Fund " + (i + 1).ToString() : "Composite " + (i + 1).ToString()
                    });
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<BenchmarkSelectionData> RetrieveBenchmarkSelectionData()
        {
            try
            {
                List<BenchmarkSelectionData> result = new List<BenchmarkSelectionData>();

                result.Add(new BenchmarkSelectionData() { Name = "EM Emerging Markets" });
                result.Add(new BenchmarkSelectionData() { Name = "IMI Emerging Markets" });
                result.Add(new BenchmarkSelectionData() { Name = "Indonesia" });
                result.Add(new BenchmarkSelectionData() { Name = "India" });
                result.Add(new BenchmarkSelectionData() { Name = "China" });

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public MarketCapitalizationData RetrieveMarketCapitalizationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                return new MarketCapitalizationData()
                    {
                        MegaLowerLimit = "100 Billion",
                        LargeLowerLimit = "10 Billion",
                        MediumLowerLimit = "2 Billion",
                        SmallLowerLimit = "250 Million",

                        PortfolioWeightedAverage = 20340,
                        BenchmarkWeightedAverage = 32450,
                        PortfolioWeightedMedian = 9123,
                        BenchmarkWeightedMedian = 13678,
                        PortfolioMegaShare = 44.9,
                        BenchmarkMegaShare = 39.6,
                        PortfolioLargeShare = 39.6,
                        BenchmarkLargeShare = 32.5,
                        PortfolioMediumShare = 15.1,
                        BenchmarkMediumShare = 11.1,
                        PortfolioSmallShare = 0.5,
                        BenchmarkSmallShare = 0,
                        PortfolioMicroShare = 0,
                        BenchmarkMicroShare = 0
                    };
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<AssetAllocationData> RetrieveAssetAllocationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<AssetAllocationData> result = new List<AssetAllocationData>();
                result.Add(new AssetAllocationData() { Country = "Mideast Regional", PortfolioShare = 4.4, ModelShare = 4.5, BenchmarkShare = 0, BetShare = 4.5 });
                result.Add(new AssetAllocationData() { Country = "Ex-South Africa", PortfolioShare = 1.9, ModelShare = 2.0, BenchmarkShare = 0.6, BetShare = 1.4 });
                result.Add(new AssetAllocationData() { Country = "Cash", PortfolioShare = 0.7, ModelShare = 0.7, BenchmarkShare = 0, BetShare = 0.7 });
                result.Add(new AssetAllocationData() { Country = "Russia", PortfolioShare = 6.6, ModelShare = 6.6, BenchmarkShare = 6.1, BetShare = 0.5 });
                result.Add(new AssetAllocationData() { Country = "Mexico", PortfolioShare = 4.5, ModelShare = 4.4, BenchmarkShare = 4.1, BetShare = 0.3 });
                result.Add(new AssetAllocationData() { Country = "Korea", PortfolioShare = 15.6, ModelShare = 15.3, BenchmarkShare = 15.1, BetShare = 0.2 });
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<SectorBreakdownData> RetrieveSectorBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<SectorBreakdownData> result = new List<SectorBreakdownData>();
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new SectorBreakdownData()
                    {
                        Sector = row.Field<string>("GICS_SECTOR_NAME"),
                        Industry = row.Field<string>("GICS_INDUSTRY_NAME"),
                        Security = row.Field<string>("ISSUE_NAME"),
                        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT"),
                        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT"),
                        BetShare = row.Field<Single?>("PORTFOLIO_WEIGHT") - row.Field<Single?>("BENCHMARK_WEIGHT")
                    });
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<RegionBreakdownData> RetrieveRegionBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<RegionBreakdownData> result = new List<RegionBreakdownData>();
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new RegionBreakdownData()
                    {
                        Region = row.Field<string>("ASHEMM_PROPRIETARY_REGION_CODE"),
                        Country = row.Field<string>("ISO_COUNTRY_CODE"),
                        Security = row.Field<string>("ISSUE_NAME"),
                        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT"),
                        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT"),
                        BetShare = row.Field<Single?>("PORTFOLIO_WEIGHT") - row.Field<Single?>("BENCHMARK_WEIGHT")
                    });
                } 

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<TopHoldingsData> RetrieveTopHoldingsData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<TopHoldingsData> result = new List<TopHoldingsData>();
                result.Add(new TopHoldingsData() { Ticker = "Ticker1", Holding = "Holding1", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker2", Holding = "Holding2", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker3", Holding = "Holding3", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker4", Holding = "Holding4", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker5", Holding = "Holding5", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker6", Holding = "Holding6", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker7", Holding = "Holding7", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker8", Holding = "Holding8", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker9", Holding = "Holding9", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });
                result.Add(new TopHoldingsData() { Ticker = "Ticker10", Holding = "Holding10", MarketValue = 23321000, PortfolioShare = 8.6, BenchmarkShare = 6.2, BetShare = 2.4 });

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<IndexConstituentsData> RetrieveIndexConstituentsData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<IndexConstituentsData> result = new List<IndexConstituentsData>();
                #region add
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCO MACRO B", Country = "Argentina", Region = "South America", Sector = "Banking", Industry = "Bank", SubIndustry = "Bank", Weight = 0.00026458, WeightCountry = 0.037883, WeightIndustry = 0.001848 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PETROBRAS ENERGIA PART B", Country = "Argentina", Region = "South America", Sector = "Basic Materials", Industry = "Major Integrated Oil & Gas", SubIndustry = "Oil", Weight = 0.00037193, WeightCountry = 0.053455, WeightIndustry = 0.001876 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SIDERAR A", Country = "Argentina", Region = "South America", Sector = "Basic Materials", Industry = "Steel & Iron", SubIndustry = "Steel", Weight = 0.00040585, WeightCountry = 0.058112, WeightIndustry = 0.003455 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELECOM ARGENTINA B", Country = "Argentina", Region = "South America", Sector = "Technology", Industry = "Wireless Communications", SubIndustry = "IT", Weight = 0.00038802, WeightCountry = 0.055559, WeightIndustry = 0.009766 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TENARIS", Country = "Argentina", Region = "South America", Sector = "Basic Materials", Industry = "Steel & Iron", SubIndustry = "Steel", Weight = 0.00555358, WeightCountry = 0.795192, WeightIndustry = 0.737379 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ALL AMER LAT UNIT", Country = "Brazil", Region = "South America", Sector = "Services", Industry = "Railroad", SubIndustry = "", Weight = 0.0014799, WeightCountry = 0.008537, WeightIndustry = 0.747454 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AMBEV PN", Country = "BRA", Region = "South America", Sector = "Consumer Goods", Industry = "Beverages - Brewers", SubIndustry = "", Weight = 0.00312237, WeightCountry = 0.018011, WeightIndustry = 0.333457 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ARACRUZ CELULOSE PNB", Country = "BRA", Region = "South America", Sector = "Materials", Industry = "Wood products", SubIndustry = "Lumber", Weight = 0.00101799, WeightCountry = 0.005872, WeightIndustry = 0.222387 });
                result.Add(new IndexConstituentsData() { ConstituentName = "B2W", Country = "BRA", Region = "South America", Sector = "Technology", Industry = "Wireless Communications", SubIndustry = "IT", Weight = 0.00059668, WeightCountry = 0.003442, WeightIndustry = 1 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCO BRADESCO PN", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00886874, WeightCountry = 0.051159, WeightIndustry = 0.061933 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCO BRASIL", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00154475, WeightCountry = 0.008911, WeightIndustry = 0.010787 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCO ITAU HLDG FIN. PN", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00744352, WeightCountry = 0.042938, WeightIndustry = 0.05198 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANRISUL PNB", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031303, WeightCountry = 0.001806, WeightIndustry = 0.002186 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BOLSA DE MERCADORIAS", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00075346, WeightCountry = 0.004345, WeightIndustry = 0.045618 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BOVESPA HOLDING", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00100856, WeightCountry = 0.005818, WeightIndustry = 0.061079 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BRADESPAR PN", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0018227, WeightCountry = 0.010514, WeightIndustry = 0.110384 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BRASIL TELECOM PART. ON", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00066208, WeightCountry = 0.003819, WeightIndustry = 0.016664 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BRASIL TELECOM PART. PN", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00100634, WeightCountry = 0.005805, WeightIndustry = 0.025348 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BRASIL TELECOM PN", Country = "BRA", Region = "South America", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00063551, WeightCountry = 0.003666, WeightIndustry = 0.015995 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BRASKEM PNA", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00038917, WeightCountry = 0.002245, WeightIndustry = 0.014785 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CCR RODOVIAS ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00071408, WeightCountry = 0.004119, WeightIndustry = 0.142303 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CEMIG PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00205073, WeightCountry = 0.01183, WeightIndustry = 0.109676 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CESP PNB", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00097462, WeightCountry = 0.005622, WeightIndustry = 0.130045 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COPEL PNB", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0006244, WeightCountry = 0.003602, WeightIndustry = 0.033394 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COSAN ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043707, WeightCountry = 0.002521, WeightIndustry = 0.03431 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CPFL ENERGIA ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00079741, WeightCountry = 0.0046, WeightIndustry = 0.042647 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CSN SIDERURGICA NAC'L ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00475829, WeightCountry = 0.027448, WeightIndustry = 0.038158 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CYRELA BRAZIL REALTY", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00087917, WeightCountry = 0.005071, WeightIndustry = 0.083331 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DURATEX PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00044508, WeightCountry = 0.002567, WeightIndustry = 0.282992 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EDP ENERGIAS DO BRASIL", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039702, WeightCountry = 0.00229, WeightIndustry = 0.021233 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ELETROBRAS ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00121202, WeightCountry = 0.006991, WeightIndustry = 0.064821 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ELETROBRAS PNB", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00096361, WeightCountry = 0.005559, WeightIndustry = 0.051535 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ELETROPAULO METROPOL.PNB", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0006468, WeightCountry = 0.003731, WeightIndustry = 0.034592 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EMBRAER", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00110362, WeightCountry = 0.006366, WeightIndustry = 0.722685 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FOSFERTIL PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00071079, WeightCountry = 0.0041, WeightIndustry = 0.027004 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GAFISA", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059053, WeightCountry = 0.003406, WeightIndustry = 0.055973 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GERDAU METALURGICA PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00265793, WeightCountry = 0.015334, WeightIndustry = 0.021315 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GERDAU ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00102992, WeightCountry = 0.005941, WeightIndustry = 0.008259 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GERDAU PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00469718, WeightCountry = 0.027095, WeightIndustry = 0.037668 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GOL PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00017898, WeightCountry = 0.001034, WeightIndustry = 0.065534 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GVT HOLDING", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00055651, WeightCountry = 0.00341, WeightIndustry = 0.014007 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ITAUSA PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00387281, WeightCountry = 0.02234, WeightIndustry = 0.027045 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JBS", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00053447, WeightCountry = 0.003072, WeightIndustry = 0.039363 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KLABIN PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00046418, WeightCountry = 0.002678, WeightIndustry = 1 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LOCALIZA RENT A CAR", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031173, WeightCountry = 0.001798, WeightIndustry = 0.157444 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LOJAS AMERICANAS PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00061638, WeightCountry = 0.003556, WeightIndustry = 0.14804 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LOJAS RENNER", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00070963, WeightCountry = 0.004093, WeightIndustry = 0.170435 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MMX MINERACAO METALICOS", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00119681, WeightCountry = 0.006904, WeightIndustry = 0.009598 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MRV ENGENHARIA", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042376, WeightCountry = 0.002444, WeightIndustry = 0.040165 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NATURA COSMETICOS ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00040748, WeightCountry = 0.002351, WeightIndustry = 0.264959 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NET SERV DE COM PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00065974, WeightCountry = 0.003806, WeightIndustry = 0.074961 });
                result.Add(new IndexConstituentsData() { ConstituentName = "OGX PETROLEO", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00275625, WeightCountry = 0.015899, WeightIndustry = 0.013901 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PAO DE ACUCAR PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00053014, WeightCountry = 0.003058, WeightIndustry = 0.049983 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PERDIGAO ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00091086, WeightCountry = 0.005254, WeightIndustry = 0.067335 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PETROBRAS ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.02437292, WeightCountry = 0.140594, WeightIndustry = 0.122924 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PETROBRAS PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.02749191, WeightCountry = 0.158586, WeightIndustry = 0.138655 });
                result.Add(new IndexConstituentsData() { ConstituentName = "REDECARD", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00150581, WeightCountry = 0.008686, WeightIndustry = 0.139856 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SABESP ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00088307, WeightCountry = 0.005094, WeightIndustry = 0.587094 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SADIA PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0007128, WeightCountry = 0.004112, WeightIndustry = 0.052693 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SOUZA CRUZ ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00062342, WeightCountry = 0.003596, WeightIndustry = 0.112473 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SUZANO PAPEL E CELUL PNA", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00063405, WeightCountry = 0.003646, WeightIndustry = 0.138075 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAM PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00038634, WeightCountry = 0.002229, WeightIndustry = 0.141452 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELE NORTE LESTE PART.ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00055478, WeightCountry = 0.0034, WeightIndustry = 0.013963 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELE NORTE LESTE PART.PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00186545, WeightCountry = 0.010761, WeightIndustry = 0.046951 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELEMAR NORTE LESTE PNA", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00073707, WeightCountry = 0.004252, WeightIndustry = 0.018551 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TIM PART. PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0004952, WeightCountry = 0.002857, WeightIndustry = 0.007134 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TRACTEBEL ENERGIA ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059412, WeightCountry = 0.003427, WeightIndustry = 0.079274 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ULTRAPAR PART PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00089415, WeightCountry = 0.005158, WeightIndustry = 0.225834 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNIBANCO UNIT", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00406109, WeightCountry = 0.023426, WeightIndustry = 0.02836 });
                result.Add(new IndexConstituentsData() { ConstituentName = "USIMINAS ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00087465, WeightCountry = 0.005045, WeightIndustry = 0.007014 });
                result.Add(new IndexConstituentsData() { ConstituentName = "USIMINAS PNA", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00362285, WeightCountry = 0.020898, WeightIndustry = 0.029053 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VALE DO RIO DOCE ON", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.01408115, WeightCountry = 0.081226, WeightIndustry = 0.112922 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VALE DO RIO DOCE PNA", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.01675788, WeightCountry = 0.096667, WeightIndustry = 0.134387 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VIVO PART. PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00098046, WeightCountry = 0.005656, WeightIndustry = 0.01412 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VOTORANTIM CELULOSE PN", Country = "BRA", Region = "BRL", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00074695, WeightCountry = 0.004309, WeightIndustry = 0.163175 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCO DE CREDITO E INV.", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023399, WeightCountry = 0.02112, WeightIndustry = 0.001634 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BCO SANTANDER CHILE (NEW", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0005806, WeightCountry = 0.052406, WeightIndustry = 0.004055 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CAP", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00102422, WeightCountry = 0.092447, WeightIndustry = 0.008214 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CENCOSUD", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0006165, WeightCountry = 0.055646, WeightIndustry = 0.058125 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CERVEZAS", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022451, WeightCountry = 0.020264, WeightIndustry = 0.023962 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CMPC (EMPRESAS)", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0009099, WeightCountry = 0.082128, WeightIndustry = 0.198774 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COLBUN", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034131, WeightCountry = 0.029002, WeightIndustry = 0.042873 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CTC TELECOM. CHILE(CIA)A", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0002265, WeightCountry = 0.020444, WeightIndustry = 0.005701 });
                result.Add(new IndexConstituentsData() { ConstituentName = "D & S DISTRIBUCION SERV", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029438, WeightCountry = 0.02657, WeightIndustry = 0.027754 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EMPRESAS COPEC", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00148552, WeightCountry = 0.134083, WeightIndustry = 0.078189 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ENDESA (CHILE)", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00142643, WeightCountry = 0.128751, WeightIndustry = 0.190331 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ENERSIS", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00120342, WeightCountry = 0.108621, WeightIndustry = 0.064361 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ENTEL", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043029, WeightCountry = 0.038838, WeightIndustry = 0.01083 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FALABELLA", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00036177, WeightCountry = 0.034653, WeightIndustry = 0.086887 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LAN AIRLINES", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00040488, WeightCountry = 0.036545, WeightIndustry = 0.148242 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MASISA (NEW)", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00014704, WeightCountry = 0.013472, WeightIndustry = 0.034122 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SOQUIMICH B", Country = "CHL", Region = "CLP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00118777, WeightCountry = 0.107209, WeightIndustry = 0.045125 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AGILE PROPERTY HLDGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0004471, WeightCountry = 0.003127, WeightIndustry = 0.040774 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AIR CHINA H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00036296, WeightCountry = 0.002539, WeightIndustry = 0.134894 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ALIBABA.COM", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043358, WeightCountry = 0.003033, WeightIndustry = 0.094972 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ALUMINUM CORP OF CHINA H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00138308, WeightCountry = 0.009674, WeightIndustry = 0.011091 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ANGANG STEEL H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00066243, WeightCountry = 0.004633, WeightIndustry = 0.005312 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ANHUI CONCH CEMENT H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00088125, WeightCountry = 0.006164, WeightIndustry = 0.073194 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK OF CHINA H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00360149, WeightCountry = 0.02519, WeightIndustry = 0.02515 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK OF COMMUNICATIONS H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00205357, WeightCountry = 0.014363, WeightIndustry = 0.014341 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BEIJING CAP INT'L AIRP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00047579, WeightCountry = 0.003348, WeightIndustry = 0.094816 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BEIJING ENTERPRISES HLDG", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039646, WeightCountry = 0.002773, WeightIndustry = 0.020867 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BELLE INT'L HLDGS(CN)", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00067626, WeightCountry = 0.00473, WeightIndustry = 0.170801 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BYD CO H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022161, WeightCountry = 0.00155, WeightIndustry = 0.044784 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHAODA MODERN AGRI.", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00074284, WeightCountry = 0.005196, WeightIndustry = 0.054914 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA AGRI HOLDINGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031964, WeightCountry = 0.002236, WeightIndustry = 0.023629 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA BLUECHEMICAL H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027979, WeightCountry = 0.001957, WeightIndustry = 0.010629 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA CITIC BANK H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00084563, WeightCountry = 0.005915, WeightIndustry = 0.005905 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA COAL ENERGY H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00163878, WeightCountry = 0.011462, WeightIndustry = 0.008265 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA COMM SERVI H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039809, WeightCountry = 0.002784, WeightIndustry = 0.01002 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA COMMUNIC CONSTRU-H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00218874, WeightCountry = 0.015308, WeightIndustry = 0.12912 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA CONSTRUCTION BK H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00825635, WeightCountry = 0.057747, WeightIndustry = 0.057657 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA COSCO HOLDINGS H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00191665, WeightCountry = 0.013405, WeightIndustry = 0.274198 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA EASTERN AIRLINES H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013478, WeightCountry = 0.000943, WeightIndustry = 0.049347 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA EVERBRIGHT", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00041837, WeightCountry = 0.002926, WeightIndustry = 0.025337 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA HIGH SPEED TRANSMI", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00050508, WeightCountry = 0.003533, WeightIndustry = 0.102071 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA INSURANCE INT' L", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051343, WeightCountry = 0.003591, WeightIndustry = 0.025331 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA INTL MARINE B SZ", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00028204, WeightCountry = 0.001973, WeightIndustry = 0.026882 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA LIFE INSURANCE H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00752806, WeightCountry = 0.052653, WeightIndustry = 0.371417 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA MENGNIU DAIRY CO", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00092207, WeightCountry = 0.006449, WeightIndustry = 0.068164 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA MERCHANTS BANK H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00254407, WeightCountry = 0.017794, WeightIndustry = 0.017766 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA MERCHANTS HLDGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00127342, WeightCountry = 0.008907, WeightIndustry = 0.253769 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA MOBILE", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.02456963, WeightCountry = 0.171845, WeightIndustry = 0.353841 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA NAT BUILDING MAT H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00052783, WeightCountry = 0.003692, WeightIndustry = 0.04384 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA NETCOM GROUP", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00138313, WeightCountry = 0.009674, WeightIndustry = 0.034812 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA OILFIELD SVCS H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0008382, WeightCountry = 0.005863, WeightIndustry = 0.111293 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA OVERSEAS LAND &INV", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00186113, WeightCountry = 0.013017, WeightIndustry = 0.169731 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA PETRO & CHEM H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0047784, WeightCountry = 0.033421, WeightIndustry = 0.0241 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA RAILWAY CONST H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00068445, WeightCountry = 0.004787, WeightIndustry = 0.040377 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA RAILWAY GROUP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0008582, WeightCountry = 0.006002, WeightIndustry = 0.050628 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA RESOURCES ENT.", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0010349, WeightCountry = 0.007238, WeightIndustry = 0.760434 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA RESOURCES LAND", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042474, WeightCountry = 0.002971, WeightIndustry = 0.038735 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA RESOURCES POWER", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00107402, WeightCountry = 0.007512, WeightIndustry = 0.143307 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA SHENHUA ENERGY H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00385388, WeightCountry = 0.026955, WeightIndustry = 0.019437 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA SHIPPING CONTAIN H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042396, WeightCountry = 0.002965, WeightIndustry = 0.060652 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA SHIPPING DEV H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00112383, WeightCountry = 0.00786, WeightIndustry = 0.160776 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA STH AIRLINES H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013489, WeightCountry = 0.000943, WeightIndustry = 0.049388 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA TELECOM CORP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00229525, WeightCountry = 0.016053, WeightIndustry = 0.057769 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA TRAVEL INT'L", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023438, WeightCountry = 0.001639, WeightIndustry = 0.06668 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA UNICOM", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00192529, WeightCountry = 0.013466, WeightIndustry = 0.027727 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA VANKE CO B", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043856, WeightCountry = 0.003067, WeightIndustry = 0.039996 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CITIC PACIFIC", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0012319, WeightCountry = 0.008616, WeightIndustry = 0.06484 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CITIC RESOURCES HOLDINGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035238, WeightCountry = 0.002465, WeightIndustry = 0.101528 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CNOOC", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00812141, WeightCountry = 0.056803, WeightIndustry = 0.04096 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CNPC HONG KONG", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034775, WeightCountry = 0.002434, WeightIndustry = 0.001754 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COSCO PACIFIC", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00055956, WeightCountry = 0.003914, WeightIndustry = 0.111511 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COUNTRY GARDEN HLDGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00064583, WeightCountry = 0.004517, WeightIndustry = 0.058898 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DATANG INT'L POWER H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059054, WeightCountry = 0.00413, WeightIndustry = 0.078796 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DENWAY MOTORS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00057382, WeightCountry = 0.004013, WeightIndustry = 0.070715 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DONGFANG ELECTRIC CORP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00015186, WeightCountry = 0.001062, WeightIndustry = 0.030689 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DONGFENG MOTOR GROUP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033018, WeightCountry = 0.002309, WeightIndustry = 0.040689 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FOSUN INT'L(CN)", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035658, WeightCountry = 0.002494, WeightIndustry = 0.00286 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GOME ELEC APPLIANCES", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00094671, WeightCountry = 0.006622, WeightIndustry = 0.239108 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GUANGDONG INVESTMENT", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030096, WeightCountry = 0.002105, WeightIndustry = 0.200089 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GUANGSHEN RAILWAY H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00018829, WeightCountry = 0.001317, WeightIndustry = 0.095102 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GUANGZHOU INVESTMENT", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0001803, WeightCountry = 0.001261, WeightIndustry = 0.016443 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GUANGZHOU R&F PROP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00057583, WeightCountry = 0.004027, WeightIndustry = 0.052515 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HARBIN POWER EQUIPMENT H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029779, WeightCountry = 0.002083, WeightIndustry = 0.06018 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HENGAN INT'L GROUP CO", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00056338, WeightCountry = 0.00394, WeightIndustry = 0.366334 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HIDILI INDUSTRY INTL DEV", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00049178, WeightCountry = 0.00344, WeightIndustry = 0.003944 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HOPSON DEVELOPMENT HDGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00020148, WeightCountry = 0.001409, WeightIndustry = 0.018374 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HUANENG POWER INTL H(HKD", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00064478, WeightCountry = 0.00451, WeightIndustry = 0.086034 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ICBC H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00863428, WeightCountry = 0.06039, WeightIndustry = 0.060296 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INNER MONGOLIA YITAI B", Country = "CHN/HK", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00057646, WeightCountry = 0.004034, WeightIndustry = 0.002907 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JIANGSU EXPRESSWAY CO H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003046, WeightCountry = 0.00213, WeightIndustry = 0.060701 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JIANGXI COPPER CO H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00078668, WeightCountry = 0.005502, WeightIndustry = 0.006309 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KWG PPTY HLDG", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016998, WeightCountry = 0.001189, WeightIndustry = 0.015502 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LENOVO GROUP", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00082999, WeightCountry = 0.005805, WeightIndustry = 0.055347 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LI NING CO", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.000474, WeightCountry = 0.003315, WeightIndustry = 0.516401 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MAANSHAN IRON&STEEL H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030622, WeightCountry = 0.002142, WeightIndustry = 0.002456 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NINE DRAGONS PAPER", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035645, WeightCountry = 0.002493, WeightIndustry = 0.077868 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PARKSON RETAIL GROUP(CN)", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00049471, WeightCountry = 0.00346, WeightIndustry = 0.118818 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PETROCHINA CO H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00831259, WeightCountry = 0.05814, WeightIndustry = 0.041924 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PICC PPTY & CASUALTY H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00048883, WeightCountry = 0.003419, WeightIndustry = 0.024118 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PING AN INSURANCE H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00289443, WeightCountry = 0.020244, WeightIndustry = 0.142804 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHANGHAI ELECTRIC GRP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.000443, WeightCountry = 0.003098, WeightIndustry = 0.089526 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHANGHAI IND HLDGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00047849, WeightCountry = 0.003347, WeightIndustry = 0.025185 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHANGHAI LUJIA. B SS(USD", Country = "CHN/HK", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00019161, WeightCountry = 0.00134, WeightIndustry = 0.017474 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHANGHAI ZHENHUA PORT- B", Country = "CHN/HK", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00028611, WeightCountry = 0.002001, WeightIndustry = 0.02727 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHIMAO PROPERTY HLDGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00046054, WeightCountry = 0.003421, WeightIndustry = 0.042001 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHOUGANG CONCORD INT'L", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027865, WeightCountry = 0.001949, WeightIndustry = 0.002235 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHUI ON LAND(CN)", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00047684, WeightCountry = 0.003335, WeightIndustry = 0.043486 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SINO-OCEAN LAND HLDGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00053771, WeightCountry = 0.003761, WeightIndustry = 0.049038 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SINOFERT HOLDINGS", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00049385, WeightCountry = 0.003454, WeightIndustry = 0.018762 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SINOPEC SHANGHAI PETRO H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024358, WeightCountry = 0.001704, WeightIndustry = 0.009254 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SOHO CHINA", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00026024, WeightCountry = 0.00182, WeightIndustry = 0.023733 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TENCENT HOLDINGS LIM(CN)", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00210767, WeightCountry = 0.014741, WeightIndustry = 0.461663 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TINGYI HOLDING CORP(CN)", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00063047, WeightCountry = 0.00441, WeightIndustry = 0.046607 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YANTAI CHANGYU PIONEER B", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031744, WeightCountry = 0.00222, WeightIndustry = 0.033881 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YANZHOU COAL MINING CO H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00105234, WeightCountry = 0.00736, WeightIndustry = 0.005307 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ZHEJIANG EXPRESSWAY H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033615, WeightCountry = 0.002351, WeightIndustry = 0.066989 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ZIJIN MINING GROUP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0010359, WeightCountry = 0.007245, WeightIndustry = 0.008307 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ZTE CORP H", Country = "CHN/HK", Region = "HKD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029439, WeightCountry = 0.002059, WeightIndustry = 0.260005 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCOLOMBIA ORD", Country = "COL", Region = "COP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00049911, WeightCountry = 0.11667, WeightIndustry = 0.003485 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCOLOMBIA PREF", Country = "COL", Region = "COP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0006372, WeightCountry = 0.148949, WeightIndustry = 0.00445 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CEMENTOS ARGOS (NEW)", Country = "COL", Region = "COP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003174, WeightCountry = 0.074194, WeightIndustry = 0.026362 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ECOPETROL", Country = "COL", Region = "COP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00149106, WeightCountry = 0.348546, WeightIndustry = 0.00752 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INTERCONEXION ELEC (NEW)", Country = "COL", Region = "COP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00046315, WeightCountry = 0.108265, WeightIndustry = 0.02477 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INVERSIONES ARGOS", Country = "COL", Region = "COP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042689, WeightCountry = 0.099789, WeightIndustry = 0.035457 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SURAMERICANA INVERSIONES", Country = "COL", Region = "COP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00044314, WeightCountry = 0.103587, WeightIndustry = 0.026837 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CENTRAL EUROPEAN MEDIA A", Country = "CZE", Region = "CZK", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00081264, WeightCountry = 0.085426, WeightIndustry = 0.092334 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CEZ CESKE ENERG. ZAVODY", Country = "CZE", Region = "CZK", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00569819, WeightCountry = 0.599006, WeightIndustry = 0.304747 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOMERCNI BANKA", Country = "CZE", Region = "CZK", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0010359, WeightCountry = 0.108896, WeightIndustry = 0.007234 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELEFONICA O2 CZECH REP.", Country = "CZE", Region = "CZK", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00109593, WeightCountry = 0.115206, WeightIndustry = 0.027583 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNIPETROL", Country = "CZE", Region = "CZK", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035717, WeightCountry = 0.037547, WeightIndustry = 0.013569 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ZENTIVA", Country = "CZE", Region = "CZK", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051291, WeightCountry = 0.053919, WeightIndustry = 0.034739 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COMMERCIAL INT'L BANK", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00072808, WeightCountry = 0.099343, WeightIndustry = 0.005084 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EFG-HERMES HOLDING", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00053081, WeightCountry = 0.072426, WeightIndustry = 0.083883 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EGYPT KUWAIT HOLDING", Country = "EGP", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00048906, WeightCountry = 0.06673, WeightIndustry = 0.077287 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EGYPTIAN MOBILE SERVICES", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034319, WeightCountry = 0.044098, WeightIndustry = 0.004654 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EL EZZ ALDEKHELA STEEL", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029234, WeightCountry = 0.039886, WeightIndustry = 0.002344 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EL EZZ STEEL REBARS", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033492, WeightCountry = 0.045698, WeightIndustry = 0.002686 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EL SEWEDY CABLES HLDG CO", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031724, WeightCountry = 0.043486, WeightIndustry = 0.064111 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ORASCOM CONSTRUCTION IND", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00166244, WeightCountry = 0.226834, WeightIndustry = 0.098073 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ORASCOM TELECOM HOLDING", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00191848, WeightCountry = 0.261768, WeightIndustry = 0.027629 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SIDI KERIR PETROCHEMCIAL", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021091, WeightCountry = 0.028778, WeightIndustry = 0.008013 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELECOM EGYPT", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.000313, WeightCountry = 0.042708, WeightIndustry = 0.007878 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TMG HOLDING", Country = "EGP", Region = "EGP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00020848, WeightCountry = 0.028446, WeightIndustry = 0.019013 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MAGYAR TELEKOM", Country = "HUN", Region = "HUF", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00067995, WeightCountry = 0.090419, WeightIndustry = 0.017113 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MOL MAGYAR OLAJ GAZIPARI", Country = "HUN", Region = "HUF", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00245562, WeightCountry = 0.346548, WeightIndustry = 0.012385 });
                result.Add(new IndexConstituentsData() { ConstituentName = "OTP BANK", Country = "HUN", Region = "HUF", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0034667, WeightCountry = 0.461002, WeightIndustry = 0.024209 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RICHTER GEDEON", Country = "HUN", Region = "HUF", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00091766, WeightCountry = 0.12203, WeightIndustry = 0.058573 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ANEKA TAMBANG", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035249, WeightCountry = 0.021123, WeightIndustry = 0.002827 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASTRA AGRO LESTARI", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003819, WeightCountry = 0.022885, WeightIndustry = 0.028231 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASTRA INTERNATIONAL", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00129568, WeightCountry = 0.077644, WeightIndustry = 0.159672 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK CENTRAL ASIA", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00104737, WeightCountry = 0.062764, WeightIndustry = 0.007314 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK DANAMON", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027909, WeightCountry = 0.016724, WeightIndustry = 0.001949 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK INT'L INDONESIA", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034601, WeightCountry = 0.019536, WeightIndustry = 0.002277 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK MANDIRI", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00063051, WeightCountry = 0.037783, WeightIndustry = 0.004403 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK RAKYAT INDONESIA", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00096013, WeightCountry = 0.057536, WeightIndustry = 0.006705 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BUMI RESOURCES", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00484016, WeightCountry = 0.290047, WeightIndustry = 0.024411 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INDOCEMENT", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016397, WeightCountry = 0.009826, WeightIndustry = 0.013619 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INDOFOOD SUKSES MAKMUR", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034004, WeightCountry = 0.020377, WeightIndustry = 0.025138 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INDOSAT", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00054057, WeightCountry = 0.034393, WeightIndustry = 0.013605 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INT'L NICKEL INDONESIA", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039998, WeightCountry = 0.023969, WeightIndustry = 0.003408 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PERUSAHAAN GAS NEGARA", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00090368, WeightCountry = 0.054153, WeightIndustry = 0.365889 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SEMEN GRESIK", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00019571, WeightCountry = 0.011728, WeightIndustry = 0.016255 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TB BATUBARA BUKIT ASAM", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00045235, WeightCountry = 0.027107, WeightIndustry = 0.002281 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELEKOMUNIKASI INDONESIA", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00251104, WeightCountry = 0.150475, WeightIndustry = 0.0634 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TRUBA ALAM MANUNGGAL ENG", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00018461, WeightCountry = 0.011063, WeightIndustry = 0.010891 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNILEVER INDONESIA", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034238, WeightCountry = 0.020517, WeightIndustry = 0.123396 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNITED TRACTORS", Country = "IDO", Region = "IDR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00053983, WeightCountry = 0.03435, WeightIndustry = 0.051453 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AFRICA ISRAEL INV", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023668, WeightCountry = 0.009345, WeightIndustry = 0.022434 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK HAPOALIM", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0012859, WeightCountry = 0.050664, WeightIndustry = 0.00898 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK LEUMI LE-ISRAEL", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00152018, WeightCountry = 0.059895, WeightIndustry = 0.010616 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BEZEQ ISRAEL TELECOM.", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00063438, WeightCountry = 0.024916, WeightIndustry = 0.015916 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DELEK GROUP", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021884, WeightCountry = 0.008622, WeightIndustry = 0.011519 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DISCOUNT INVESTMENT CORP", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0002006, WeightCountry = 0.007904, WeightIndustry = 0.010559 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ELBIT SYSTEMS", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042349, WeightCountry = 0.016686, WeightIndustry = 0.277315 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GAZIT GLOBE (1982)", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00018031, WeightCountry = 0.007104, WeightIndustry = 0.016444 });
                result.Add(new IndexConstituentsData() { ConstituentName = "IDB DEVELOPMENT CORP", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0001309, WeightCountry = 0.005157, WeightIndustry = 0.00689 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ISRAEL CHEMICALS", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0036931, WeightCountry = 0.145508, WeightIndustry = 0.140305 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ISRAEL CORP", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00110255, WeightCountry = 0.04344, WeightIndustry = 0.041887 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ISRAEL DISCOUNT BANK", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033408, WeightCountry = 0.013163, WeightIndustry = 0.002333 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOOR INDUSTRIES", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00017313, WeightCountry = 0.006821, WeightIndustry = 0.009113 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MA MAKHTESHIM-AGAN IND", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00083618, WeightCountry = 0.034946, WeightIndustry = 0.031767 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MIZRAHI TEFAHOT BANK", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029794, WeightCountry = 0.011739, WeightIndustry = 0.002081 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NICE SYSTEMS", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00054069, WeightCountry = 0.021303, WeightIndustry = 0.477534 });
                result.Add(new IndexConstituentsData() { ConstituentName = "OIL RAFINERIES", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0001839, WeightCountry = 0.007246, WeightIndustry = 0.000928 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ORMAT INDUSTRIES", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030788, WeightCountry = 0.012131, WeightIndustry = 0.06222 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PARTNER COMMUNICATIONS", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00057691, WeightCountry = 0.02273, WeightIndustry = 0.008308 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TEVA PHARMACEUTICAL IND", Country = "ISR", Region = "ILS", Sector = "", Industry = "", SubIndustry = "", Weight = 0.01083359, WeightCountry = 0.426843, WeightIndustry = 0.691499 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ABB INDIA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0002738, WeightCountry = 0.004954, WeightIndustry = 0.055333 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ACC", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00018631, WeightCountry = 0.003371, WeightIndustry = 0.015474 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ADITYA BIRLA NUVO", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00025995, WeightCountry = 0.004703, WeightIndustry = 0.013682 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AMBUJA CEMENTS", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031206, WeightCountry = 0.005646, WeightIndustry = 0.025919 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AXIS BANK", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00082002, WeightCountry = 0.014837, WeightIndustry = 0.005726 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BHARAT HEAVY ELECTRICALS", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00111844, WeightCountry = 0.020237, WeightIndustry = 0.226026 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BHARAT PETROLEUM CORP", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013154, WeightCountry = 0.00238, WeightIndustry = 0.000663 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CAIRN INDIA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051585, WeightCountry = 0.009334, WeightIndustry = 0.002602 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CIPLA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00046165, WeightCountry = 0.008353, WeightIndustry = 0.029467 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DLF", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00053009, WeightCountry = 0.009591, WeightIndustry = 0.048342 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DR REDDY'S LABORATORIES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00046735, WeightCountry = 0.008456, WeightIndustry = 0.02983 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ESSAR OIL", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034341, WeightCountry = 0.005852, WeightIndustry = 0.001631 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GAIL INDIA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00058567, WeightCountry = 0.010597, WeightIndustry = 0.23713 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GLENMARK PHARMACEUTICALS", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042592, WeightCountry = 0.007706, WeightIndustry = 0.027186 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GMR INFRASTRUCTURE", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0002477, WeightCountry = 0.004482, WeightIndustry = 0.014612 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRASIM INDUSTRIES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043983, WeightCountry = 0.007958, WeightIndustry = 0.036531 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HCL TECHNOLOGIES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029082, WeightCountry = 0.005262, WeightIndustry = 0.02701 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HDFC BANK", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00185065, WeightCountry = 0.033485, WeightIndustry = 0.012924 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HERO HONDA MOTORS", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00038188, WeightCountry = 0.00691, WeightIndustry = 0.047061 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HINDALCO INDUSTRIES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00048754, WeightCountry = 0.008821, WeightIndustry = 0.00391 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HINDUSTAN UNILEVER", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00120865, WeightCountry = 0.021869, WeightIndustry = 0.435602 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HOUSING DEV FINANCE CORP", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00270712, WeightCountry = 0.048982, WeightIndustry = 1 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ICICI BANK", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00316986, WeightCountry = 0.057354, WeightIndustry = 0.022136 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INDIABULLS FINL SERVICE", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023557, WeightCountry = 0.004262, WeightIndustry = 0.014266 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INDIABULLS REAL ESTATE", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003389, WeightCountry = 0.006134, WeightIndustry = 0.030906 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INDIAN HOTELS CO", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00015459, WeightCountry = 0.002797, WeightIndustry = 0.043979 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INFOSYS TECHNOLOGIES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00551098, WeightCountry = 0.099714, WeightIndustry = 0.511845 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INFRASTRUCTURE DEV FIN", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00045725, WeightCountry = 0.008273, WeightIndustry = 0.027691 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ITC", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00140351, WeightCountry = 0.025395, WeightIndustry = 0.253411 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JAIPRAKASH ASSOCIATES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00049605, WeightCountry = 0.008975, WeightIndustry = 0.026109 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JINDAL STEEL & POWER", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00070849, WeightCountry = 0.012819, WeightIndustry = 0.005682 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JSW STEEL", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043888, WeightCountry = 0.007941, WeightIndustry = 0.00352 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOTAK MAHINDRA BANK", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00037617, WeightCountry = 0.006806, WeightIndustry = 0.022781 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LARSEN & TOUBRO", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00149306, WeightCountry = 0.027015, WeightIndustry = 0.08808 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MAHINDRA & MAHINDRA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042465, WeightCountry = 0.007683, WeightIndustry = 0.052331 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MARUTI SUZUKI INDIA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027818, WeightCountry = 0.005033, WeightIndustry = 0.034282 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NTPC", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00088648, WeightCountry = 0.01604, WeightIndustry = 0.118285 });
                result.Add(new IndexConstituentsData() { ConstituentName = "OIL & NATURAL GAS CORP", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00154945, WeightCountry = 0.028035, WeightIndustry = 0.007815 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RANBAXY LABORATORIES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051761, WeightCountry = 0.009365, WeightIndustry = 0.033038 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RELIANCE CAPITAL", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00067842, WeightCountry = 0.012275, WeightIndustry = 0.041085 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RELIANCE COMMUNICATION", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00172092, WeightCountry = 0.031138, WeightIndustry = 0.024784 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RELIANCE INDUSTRIES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00938693, WeightCountry = 0.169844, WeightIndustry = 0.047343 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RELIANCE INFRASTRUCTURE", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00052405, WeightCountry = 0.009482, WeightIndustry = 0.028027 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RELIANCE NATURAL RESOURC", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023574, WeightCountry = 0.004265, WeightIndustry = 0.031455 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RELIANCE PETROLEUM", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00062928, WeightCountry = 0.011386, WeightIndustry = 0.003174 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SATYAM COMPUTER SERVICES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00144383, WeightCountry = 0.026124, WeightIndustry = 0.134099 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SIEMENS INDIA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021177, WeightCountry = 0.003834, WeightIndustry = 0.011146 });
                result.Add(new IndexConstituentsData() { ConstituentName = "STATE BANK OF INDIA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00045457, WeightCountry = 0.008225, WeightIndustry = 0.003174 });
                result.Add(new IndexConstituentsData() { ConstituentName = "STEEL AUTHORITY OF INDIA", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043999, WeightCountry = 0.007961, WeightIndustry = 0.003528 });
                result.Add(new IndexConstituentsData() { ConstituentName = "STERLITE INDUSTRIES", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00116576, WeightCountry = 0.021093, WeightIndustry = 0.009349 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SUN PHARMACEUTICAL IND", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00058502, WeightCountry = 0.010585, WeightIndustry = 0.037342 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TATA COMMUNICATIONS", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00015215, WeightCountry = 0.002753, WeightIndustry = 0.00383 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TATA CONSULTANCY", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00116472, WeightCountry = 0.021074, WeightIndustry = 0.108176 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TATA MOTORS", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00055273, WeightCountry = 0.010001, WeightIndustry = 0.052682 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TATA POWER CO", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00054257, WeightCountry = 0.009817, WeightIndustry = 0.029018 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TATA STEEL", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00085879, WeightCountry = 0.015539, WeightIndustry = 0.006887 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ULTRATECH CEMENT", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00012778, WeightCountry = 0.002312, WeightIndustry = 0.010613 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNITECH", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00036809, WeightCountry = 0.00666, WeightIndustry = 0.033569 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNITED SPIRITS", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00041984, WeightCountry = 0.007596, WeightIndustry = 0.04481 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WIPRO", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00085073, WeightCountry = 0.015393, WeightIndustry = 0.079013 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ZEE ENTERTAINMENT ENT", Country = "IND", Region = "INR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00028829, WeightCountry = 0.005216, WeightIndustry = 0.034756 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ARAB BANK", Country = "JOR", Region = "JOD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00050106, WeightCountry = 0.618198, WeightIndustry = 0.003499 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JORDAN ELECTRIC POWER", Country = "JOR", Region = "JOD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00017775, WeightCountry = 0.219298, WeightIndustry = 0.009506 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNITED ARAB INVESTORS", Country = "JOR", Region = "JOD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013171, WeightCountry = 0.162505, WeightIndustry = 0.007977 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AMOREPACIFIC CORP (NEW)", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00056703, WeightCountry = 0.004415, WeightIndustry = 0.36871 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHEIL INDUSTRIAL", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00065209, WeightCountry = 0.005077, WeightIndustry = 0.465116 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CJ CHEILJEDANG CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00058352, WeightCountry = 0.004543, WeightIndustry = 0.043136 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DACOM CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00020098, WeightCountry = 0.001565, WeightIndustry = 0.005058 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DAEGU BANK", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051056, WeightCountry = 0.003975, WeightIndustry = 0.003565 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DAELIM INDUSTRIAL CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.000821, WeightCountry = 0.006392, WeightIndustry = 0.048433 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DAEWOO ENGR. & CONSTR.", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00080677, WeightCountry = 0.006282, WeightIndustry = 0.047594 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DAEWOO INT'L CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059612, WeightCountry = 0.004641, WeightIndustry = 0.171754 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DAEWOO SECURITIES CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00066956, WeightCountry = 0.005213, WeightIndustry = 0.10581 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DAEWOO SHIPBUILDING", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00108434, WeightCountry = 0.008443, WeightIndustry = 0.103351 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DC CHEMICAL CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.001251, WeightCountry = 0.00974, WeightIndustry = 0.047527 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DONGBU INSURANCE CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039804, WeightCountry = 0.003099, WeightIndustry = 0.019638 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DONGKUK STEEL MILL CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00048822, WeightCountry = 0.003801, WeightIndustry = 0.003915 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DOOSAN CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00047055, WeightCountry = 0.003664, WeightIndustry = 0.024767 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DOOSAN HEAVY INDUSTRIES", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00085545, WeightCountry = 0.006661, WeightIndustry = 0.050466 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DOOSAN INFRACORE CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00063758, WeightCountry = 0.004964, WeightIndustry = 0.06077 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GS ENGINEERING & CONSTR.", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00107391, WeightCountry = 0.008362, WeightIndustry = 0.063353 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GS HOLDINGS CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059242, WeightCountry = 0.004613, WeightIndustry = 0.002988 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HANA FINANCIAL HOLDINGS", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00148833, WeightCountry = 0.011588, WeightIndustry = 0.010393 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HANJIN HEAVY INDUSTRIES", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003929, WeightCountry = 0.003059, WeightIndustry = 0.037449 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HANJIN SHIPPING CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00067398, WeightCountry = 0.005248, WeightIndustry = 0.09642 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HANKOOK TIRE MFG CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039265, WeightCountry = 0.003057, WeightIndustry = 0.167173 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HANWHA CHEMICAL CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024123, WeightCountry = 0.001878, WeightIndustry = 0.009165 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HANWHA CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00049002, WeightCountry = 0.003815, WeightIndustry = 0.018617 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HITE BREWERY CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039801, WeightCountry = 0.003099, WeightIndustry = 0.04248 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HONAM PETROCHEMICAL", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030242, WeightCountry = 0.002355, WeightIndustry = 0.011489 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYNIX SEMICONDUCTOR", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00229314, WeightCountry = 0.017855, WeightIndustry = 0.044762 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYOSUNG  CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00048082, WeightCountry = 0.003744, WeightIndustry = 0.018267 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI DEPT STORE", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003633, WeightCountry = 0.002829, WeightIndustry = 0.087255 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI DEVELOPMENT CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00093392, WeightCountry = 0.007272, WeightIndustry = 0.055094 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI ENGR. & CONSTR.", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00099161, WeightCountry = 0.007721, WeightIndustry = 0.058498 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI HEAVY INDUSTRIES", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00386201, WeightCountry = 0.03007, WeightIndustry = 0.368097 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI MIPO DOCKYARD", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00069033, WeightCountry = 0.005375, WeightIndustry = 0.065797 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI MOBIS", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00136625, WeightCountry = 0.010638, WeightIndustry = 0.581683 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI MOTOR CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00330466, WeightCountry = 0.02573, WeightIndustry = 0.407246 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI MOTOR CO PREF 2", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034239, WeightCountry = 0.00251, WeightIndustry = 0.039729 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI SECURITIES CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0004959, WeightCountry = 0.003861, WeightIndustry = 0.078367 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HYUNDAI STEEL CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00123456, WeightCountry = 0.009612, WeightIndustry = 0.0099 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INDUSTRIAL BANK OF KOREA", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00084754, WeightCountry = 0.006599, WeightIndustry = 0.005919 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KANGWON LAND", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00071483, WeightCountry = 0.005566, WeightIndustry = 0.203362 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KCC CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00065257, WeightCountry = 0.005081, WeightIndustry = 0.414913 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KEPCO KOREA ELECT. POWER", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00229648, WeightCountry = 0.017881, WeightIndustry = 0.122819 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KIA MOTORS CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00064087, WeightCountry = 0.00499, WeightIndustry = 0.078978 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOOKMIN BANK", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0060301, WeightCountry = 0.046951, WeightIndustry = 0.04211 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOREA EXCHANGE BANK", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00106428, WeightCountry = 0.008287, WeightIndustry = 0.007434 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOREA GAS CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00052207, WeightCountry = 0.004065, WeightIndustry = 0.21138 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOREA INVESTMENT HLDG", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00048047, WeightCountry = 0.003741, WeightIndustry = 0.075929 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOREA LINE CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034855, WeightCountry = 0.002714, WeightIndustry = 0.049864 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOREA ZINC", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003491, WeightCountry = 0.002718, WeightIndustry = 0.0028 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOREAN AIR CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00050186, WeightCountry = 0.003908, WeightIndustry = 0.183749 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KT CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00176699, WeightCountry = 0.013758, WeightIndustry = 0.044473 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KT FREETEL", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00062153, WeightCountry = 0.004839, WeightIndustry = 0.008951 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KT&G CORP(KOREA TOBACCO)", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0029442, WeightCountry = 0.022924, WeightIndustry = 0.53117 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KUMHO INDUSTRIAL CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016292, WeightCountry = 0.001269, WeightIndustry = 0.009611 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LG CHEM", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00157611, WeightCountry = 0.012272, WeightIndustry = 0.059878 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LG CORP (NEW)", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00184748, WeightCountry = 0.014385, WeightIndustry = 0.097241 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LG DISPLAY CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00201424, WeightCountry = 0.015683, WeightIndustry = 0.09512 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LG ELECTRONICS (NEW)", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00347806, WeightCountry = 0.025523, WeightIndustry = 0.310706 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LG ELECTRONICS PREF", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00026215, WeightCountry = 0.002041, WeightIndustry = 0.024847 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LG HOUSEHOLD & HEALTH", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00054861, WeightCountry = 0.004272, WeightIndustry = 0.197721 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LG TELECOM", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031185, WeightCountry = 0.002428, WeightIndustry = 0.004491 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LOTTE CONFECTIONERY CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00026038, WeightCountry = 0.002027, WeightIndustry = 0.019249 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LOTTE SHOPPING CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00092885, WeightCountry = 0.007234, WeightIndustry = 0.223087 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LS CABLE", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043737, WeightCountry = 0.003405, WeightIndustry = 0.088388 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MIRAE ASSET SEC CO LTD", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00064226, WeightCountry = 0.005001, WeightIndustry = 0.101496 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NHN CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00202413, WeightCountry = 0.01576, WeightIndustry = 0.443365 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POSCO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00964624, WeightCountry = 0.075107, WeightIndustry = 0.077357 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PUSAN BANK", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00050172, WeightCountry = 0.003906, WeightIndustry = 0.003504 });
                result.Add(new IndexConstituentsData() { ConstituentName = "S-OIL CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00088186, WeightCountry = 0.006866, WeightIndustry = 0.004448 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG C&T CORP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00214889, WeightCountry = 0.016731, WeightIndustry = 0.619134 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG CARD CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00047595, WeightCountry = 0.003706, WeightIndustry = 0.644973 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG ELECTRO-MECH. CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00065886, WeightCountry = 0.00513, WeightIndustry = 0.031114 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG ELECTRONICS CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.02041405, WeightCountry = 0.158946, WeightIndustry = 0.398483 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG ELECTRONICS PREF", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00272263, WeightCountry = 0.021199, WeightIndustry = 0.053146 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG ENGINEERING CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00073635, WeightCountry = 0.005733, WeightIndustry = 0.04344 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG FIRE & MARINE", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00242804, WeightCountry = 0.018905, WeightIndustry = 0.119794 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG HEAVY INDUSTRIES", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00176552, WeightCountry = 0.013747, WeightIndustry = 0.168276 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG SDI CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00082895, WeightCountry = 0.006454, WeightIndustry = 0.039146 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG SECURITIES CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00095539, WeightCountry = 0.007439, WeightIndustry = 0.15098 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAMSUNG TECHWIN CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00072585, WeightCountry = 0.005652, WeightIndustry = 0.038205 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHINHAN FINANCIAL GROUP", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00460855, WeightCountry = 0.035883, WeightIndustry = 0.034183 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHINSEGAE CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00233028, WeightCountry = 0.018144, WeightIndustry = 0.219704 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SK ENERGY CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00205183, WeightCountry = 0.015976, WeightIndustry = 0.010348 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SK HOLDINGS", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00130986, WeightCountry = 0.010199, WeightIndustry = 0.068944 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SK NETWORKS", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003734, WeightCountry = 0.002907, WeightIndustry = 0.107584 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SK TELECOM CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00219625, WeightCountry = 0.0171, WeightIndustry = 0.031629 });
                result.Add(new IndexConstituentsData() { ConstituentName = "STX PAN OCEAN", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0006011, WeightCountry = 0.00468, WeightIndustry = 0.085993 });
                result.Add(new IndexConstituentsData() { ConstituentName = "STX SHIPBUILDING CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003984, WeightCountry = 0.003102, WeightIndustry = 0.037973 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIHAN ELECTRIC WIRE CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030112, WeightCountry = 0.002345, WeightIndustry = 0.060853 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TONG YANG INVEST BANK", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023734, WeightCountry = 0.001848, WeightIndustry = 0.037508 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WOONGJIN COWAY CO", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043576, WeightCountry = 0.003393, WeightIndustry = 0.041303 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WOORI FINANCE HOLDINGS", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00116974, WeightCountry = 0.009108, WeightIndustry = 0.008169 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WOORI INVESTMENT & SEC", Country = "KOR", Region = "KRW", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00047674, WeightCountry = 0.003712, WeightIndustry = 0.075339 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ATTIJARIWAFA BANK", Country = "MOR", Region = "MAD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039857, WeightCountry = 0.100595, WeightIndustry = 0.002783 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BMCE", Country = "MOR", Region = "MAD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051196, WeightCountry = 0.129212, WeightIndustry = 0.003575 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CGI", Country = "MOR", Region = "MAD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00036441, WeightCountry = 0.091972, WeightIndustry = 0.033433 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DOUJA PROM GROUPE ADDOHA", Country = "MOR", Region = "MAD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00088479, WeightCountry = 0.22331, WeightIndustry = 0.083863 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MAROC TELECOM", Country = "MOR", Region = "MAD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00139456, WeightCountry = 0.35197, WeightIndustry = 0.035099 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ONA OMNIUM NORD AFRICAIN", Country = "MOR", Region = "MAD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00040787, WeightCountry = 0.102942, WeightIndustry = 0.021468 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ALFA", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059565, WeightCountry = 0.012093, WeightIndustry = 0.031352 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AMERICA MOVIL L", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.01434263, WeightCountry = 0.291178, WeightIndustry = 0.206556 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AXTEL CPO", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027371, WeightCountry = 0.005557, WeightIndustry = 0.006889 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCO COMPARTAMOS", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00026199, WeightCountry = 0.005319, WeightIndustry = 0.355027 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CARSO GLOBAL TELECOM A1", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00120189, WeightCountry = 0.0244, WeightIndustry = 0.03025 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CEMEX CPO", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.005162, WeightCountry = 0.104797, WeightIndustry = 0.428744 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COCA-COLA FEMSA L", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00045036, WeightCountry = 0.009143, WeightIndustry = 0.048068 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CONTROLADORA COM MEX UBC", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031977, WeightCountry = 0.006492, WeightIndustry = 0.030148 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CORPORACION GEO B", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042791, WeightCountry = 0.008687, WeightIndustry = 0.040559 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DESARROLLADORA HOMEX", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059404, WeightCountry = 0.01206, WeightIndustry = 0.056305 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EMPRESAS ICA", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00087849, WeightCountry = 0.017835, WeightIndustry = 0.051825 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FEMSA UNIT UBD", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00294504, WeightCountry = 0.059789, WeightIndustry = 0.314331 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO AEROP PACIFICO B", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042752, WeightCountry = 0.008679, WeightIndustry = 0.085197 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO BIMBO A", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00056369, WeightCountry = 0.011444, WeightIndustry = 0.04167 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO CARSO", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00080887, WeightCountry = 0.016421, WeightIndustry = 0.042574 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO ELEKTRA", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00081854, WeightCountry = 0.016618, WeightIndustry = 0.206736 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO FIN BANORTE O", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00179699, WeightCountry = 0.036482, WeightIndustry = 0.012549 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO FIN INBURSA O", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00110633, WeightCountry = 0.02246, WeightIndustry = 0.007726 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO MEXICO B", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00253684, WeightCountry = 0.051502, WeightIndustry = 0.020344 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO MODELO C", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00079999, WeightCountry = 0.016241, WeightIndustry = 0.085385 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GRUPO TELEVISA CPO", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00350028, WeightCountry = 0.071061, WeightIndustry = 0.397712 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INDUSTRIAS PENOLES CP", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00081086, WeightCountry = 0.016462, WeightIndustry = 0.006503 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KIMBERLY-CLARK MEXICO A", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00067502, WeightCountry = 0.013704, WeightIndustry = 0.243481 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELEFONOS MEXICO L", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00235299, WeightCountry = 0.047769, WeightIndustry = 0.059222 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELMEX INTERNACIONAL L", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00154631, WeightCountry = 0.031392, WeightIndustry = 0.038919 });
                result.Add(new IndexConstituentsData() { ConstituentName = "URBI DESARROLLOS URBANOS", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00045112, WeightCountry = 0.009158, WeightIndustry = 0.042759 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WALMART MEXICO V", Country = "MEX", Region = "MXN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0036084, WeightCountry = 0.073456, WeightIndustry = 0.340209 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AIRASIA BHD", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0000915, WeightCountry = 0.003972, WeightIndustry = 0.033502 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ALLIANCE FINANCIAL GROUP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021965, WeightCountry = 0.009536, WeightIndustry = 0.001534 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AMMB HOLDINGS", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00048479, WeightCountry = 0.021046, WeightIndustry = 0.029359 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASIATIC DEVELOPMENT", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00017317, WeightCountry = 0.007518, WeightIndustry = 0.012802 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASTRO ALL ASIA NETWORKS", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00012248, WeightCountry = 0.005317, WeightIndustry = 0.013917 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BERJAYA SPORTS TOTO", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033494, WeightCountry = 0.014541, WeightIndustry = 0.095288 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BRITISH AMER TOBACCO MY", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00057172, WeightCountry = 0.02482, WeightIndustry = 0.103146 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BUMIPUTRA-COMMERCE HLDGS", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00178944, WeightCountry = 0.077685, WeightIndustry = 0.012496 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BURSA MALAYSIA", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00020923, WeightCountry = 0.009084, WeightIndustry = 0.012671 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DIGI.COM", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00066775, WeightCountry = 0.028989, WeightIndustry = 0.009617 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GAMUDA", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034764, WeightCountry = 0.015092, WeightIndustry = 0.020509 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GENTING", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00111754, WeightCountry = 0.048516, WeightIndustry = 0.317934 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HONG LEONG BANK", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00025605, WeightCountry = 0.011116, WeightIndustry = 0.001788 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HONG LEONG FINANCIAL GRP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011275, WeightCountry = 0.004895, WeightIndustry = 0.000787 });
                result.Add(new IndexConstituentsData() { ConstituentName = "IGB CORP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011823, WeightCountry = 0.005133, WeightIndustry = 0.010782 });
                result.Add(new IndexConstituentsData() { ConstituentName = "IJM CORP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039234, WeightCountry = 0.017033, WeightIndustry = 0.023145 });
                result.Add(new IndexConstituentsData() { ConstituentName = "IOI CORP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00231948, WeightCountry = 0.100696, WeightIndustry = 0.171466 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KLCC PROPERTY HOLDINGS", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00012092, WeightCountry = 0.005249, WeightIndustry = 0.011027 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KNM GROUP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.000504, WeightCountry = 0.02188, WeightIndustry = 0.066919 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KUALA LUMPUR KEPONG", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00077937, WeightCountry = 0.033835, WeightIndustry = 0.057614 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LAFARGE MALAYAN CEMENT", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013358, WeightCountry = 0.005799, WeightIndustry = 0.011095 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MALAYAN BANKING", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00160998, WeightCountry = 0.069895, WeightIndustry = 0.011243 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MALAYSIA MINING CORP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023141, WeightCountry = 0.010046, WeightIndustry = 0.01218 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MALAYSIAN AIRLINE SYSTEM", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00012138, WeightCountry = 0.00527, WeightIndustry = 0.044443 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MISC FGN", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00085742, WeightCountry = 0.037223, WeightIndustry = 0.122663 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PARKSON BHD", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013423, WeightCountry = 0.005827, WeightIndustry = 0.034239 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PETRONAS DAGANGAN", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00017234, WeightCountry = 0.007481, WeightIndustry = 0.000869 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PETRONAS GAS", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0004584, WeightCountry = 0.019901, WeightIndustry = 0.185601 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PLUS EXPRESSWAYS", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003674, WeightCountry = 0.01595, WeightIndustry = 0.073417 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PPB GROUP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00053658, WeightCountry = 0.023495, WeightIndustry = 0.039666 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PUBLIC BANK FGN", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00103438, WeightCountry = 0.044819, WeightIndustry = 0.007209 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RESORTS WORLD", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00069912, WeightCountry = 0.030351, WeightIndustry = 0.198894 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RHB CAPITAL", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016847, WeightCountry = 0.007314, WeightIndustry = 0.001176 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SIME DARBY", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0020483, WeightCountry = 0.088923, WeightIndustry = 0.10781 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SP SETIA", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021537, WeightCountry = 0.00935, WeightIndustry = 0.019641 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TANJONG", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00025961, WeightCountry = 0.011271, WeightIndustry = 0.03464 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELEKOM MALAYSIA", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003133, WeightCountry = 0.013601, WeightIndustry = 0.007885 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELEKOM MALAYSIA INT'L", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059104, WeightCountry = 0.025659, WeightIndustry = 0.008512 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TENAGA NASIONAL", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00098676, WeightCountry = 0.042838, WeightIndustry = 0.052773 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UEM WORLD", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00018745, WeightCountry = 0.008138, WeightIndustry = 0.009866 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UMW HOLDINGS", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034742, WeightCountry = 0.014214, WeightIndustry = 0.1394 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YTL CORP", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00052938, WeightCountry = 0.022982, WeightIndustry = 1 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YTL POWER INT'L", Country = "MAL", Region = "MYR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034011, WeightCountry = 0.013897, WeightIndustry = 0.212817 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BUENAVENTURA (MINAS)", Country = "PER", Region = "PEN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0016549, WeightCountry = 0.234934, WeightIndustry = 0.013471 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CIA MINERA MILPO", Country = "PER", Region = "PEN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00038583, WeightCountry = 0.054306, WeightIndustry = 0.003094 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CREDICORP (USD)", Country = "PER", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00165823, WeightCountry = 0.233401, WeightIndustry = 0.01158 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MINSUR I", Country = "PER", Region = "PEN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034431, WeightCountry = 0.045647, WeightIndustry = 0.002601 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SOUTHERN COPPER C(USD", Country = "PER", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00283387, WeightCountry = 0.398876, WeightIndustry = 0.022726 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VOLCAN COMPANIA MINERA B", Country = "PER", Region = "PEN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024751, WeightCountry = 0.034838, WeightIndustry = 0.001985 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AYALA CORP", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029968, WeightCountry = 0.086671, WeightIndustry = 0.018149 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AYALA LAND", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034775, WeightCountry = 0.094791, WeightIndustry = 0.02989 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANCO DE ORO UNIBANK", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022872, WeightCountry = 0.066148, WeightIndustry = 0.001597 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK OF PHIL ISLANDS", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031808, WeightCountry = 0.091992, WeightIndustry = 0.002221 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GLOBE TELECOM", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00025491, WeightCountry = 0.073724, WeightIndustry = 0.003671 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ICTSI INT'L CONTAINER", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00015273, WeightCountry = 0.044173, WeightIndustry = 0.030437 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JOLLIBEE FOODS CORP", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00009481, WeightCountry = 0.027419, WeightIndustry = 0.026972 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MANILA ELECTRIC CO", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011372, WeightCountry = 0.034891, WeightIndustry = 0.006082 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MEGAWORLD CORP", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00006697, WeightCountry = 0.019368, WeightIndustry = 0.006107 });
                result.Add(new IndexConstituentsData() { ConstituentName = "METROPOLITAN BANK &TRUST", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011543, WeightCountry = 0.033384, WeightIndustry = 0.000806 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PHIL LONG DISTANCE TEL", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00090094, WeightCountry = 0.260565, WeightIndustry = 0.012975 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PNOC ENERGY DEV CORP", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021087, WeightCountry = 0.060988, WeightIndustry = 0.028137 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SM INVESTMENTS", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00019905, WeightCountry = 0.057569, WeightIndustry = 0.010477 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SM PRIME HOLDINGS", Country = "PHI", Region = "PHP", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00017398, WeightCountry = 0.050317, WeightIndustry = 0.015866 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FAUJI FERTILIZER CO", Country = "PAK", Region = "PKR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011492, WeightCountry = 0.071999, WeightIndustry = 0.004366 });
                result.Add(new IndexConstituentsData() { ConstituentName = "JAHANGIR SIDDIQUI & CO L", Country = "PAK", Region = "PKR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024427, WeightCountry = 0.153035, WeightIndustry = 0.038601 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MCB BANK", Country = "PAK", Region = "PKR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00040601, WeightCountry = 0.254365, WeightIndustry = 0.002835 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NATIONAL BANK PAKISTAN", Country = "PAK", Region = "PKR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011643, WeightCountry = 0.072943, WeightIndustry = 0.000813 });
                result.Add(new IndexConstituentsData() { ConstituentName = "OIL & GAS DEVELOPMENT", Country = "PAK", Region = "PKR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030912, WeightCountry = 0.193667, WeightIndustry = 0.001559 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PAKISTAN STATE OIL CO", Country = "PAK", Region = "PKR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016343, WeightCountry = 0.10239, WeightIndustry = 0.000824 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PAKISTAN TELECOM. CO", Country = "PAK", Region = "PKR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00012835, WeightCountry = 0.080409, WeightIndustry = 0.00343 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNITED BANK", Country = "PAK", Region = "PKR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011363, WeightCountry = 0.071192, WeightIndustry = 0.000794 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AGORA", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021062, WeightCountry = 0.01288, WeightIndustry = 0.023931 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASSECO POLAND", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035955, WeightCountry = 0.021988, WeightIndustry = 0.220482 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK MILLENNIUM", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00028253, WeightCountry = 0.017278, WeightIndustry = 0.001973 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK PEKAO", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00276507, WeightCountry = 0.169093, WeightIndustry = 0.019309 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK ZACHODNI WBK", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00041614, WeightCountry = 0.025448, WeightIndustry = 0.002906 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BIOTON", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011535, WeightCountry = 0.007054, WeightIndustry = 1 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BPH PBK", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00008608, WeightCountry = 0.005264, WeightIndustry = 0.000601 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BRE BK ROZWOJU EKSPORTU", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00044534, WeightCountry = 0.027233, WeightIndustry = 0.00311 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CERSANIT-KRASNYSTAW", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022231, WeightCountry = 0.013595, WeightIndustry = 0.141346 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ECHO INVESTMENTS", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00015415, WeightCountry = 0.009427, WeightIndustry = 0.014058 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GETIN SERVICE PROVIDER", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00038809, WeightCountry = 0.023733, WeightIndustry = 0.00271 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GLOBE TRADE CENTRE", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0005019, WeightCountry = 0.030693, WeightIndustry = 0.045772 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KGHM POLSKA MIEDZ", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00171365, WeightCountry = 0.104795, WeightIndustry = 0.013742 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LOTOS GROUP", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00019309, WeightCountry = 0.011808, WeightIndustry = 0.000974 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ORBIS", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016483, WeightCountry = 0.01008, WeightIndustry = 0.046892 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PBG", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033731, WeightCountry = 0.020627, WeightIndustry = 0.019899 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PKO BANK POLSKI", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00345205, WeightCountry = 0.198873, WeightIndustry = 0.02271 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POLIMEX MOSTOSTAL SIEDLC", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029734, WeightCountry = 0.018183, WeightIndustry = 0.017541 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POLISH OIL & GAS", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00053639, WeightCountry = 0.034802, WeightIndustry = 0.002705 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POLSKI KONCERN NAF ORLEN", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00152697, WeightCountry = 0.093379, WeightIndustry = 0.007701 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TPSA TELEKOM POLSKA", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00204467, WeightCountry = 0.125038, WeightIndustry = 0.051462 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TVN", Country = "POL", Region = "PLN", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033898, WeightCountry = 0.02073, WeightIndustry = 0.038516 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AFK SISTEMA GDR (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.000844, WeightCountry = 0.007801, WeightIndustry = 0.012155 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COMSTAR UTS GDR", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00050349, WeightCountry = 0.004654, WeightIndustry = 0.012672 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GAZPROM (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.04024358, WeightCountry = 0.37198, WeightIndustry = 0.202967 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GAZPROM NEFT (RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00078893, WeightCountry = 0.007292, WeightIndustry = 0.003979 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LUKOIL HOLDING (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0136052, WeightCountry = 0.125756, WeightIndustry = 0.068617 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MAGNITOGORSK IRON&STEEL", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00060105, WeightCountry = 0.005556, WeightIndustry = 0.00482 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MECHEL ADR (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00215938, WeightCountry = 0.01996, WeightIndustry = 0.017317 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MOBILE TELESYS ADR (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0045516, WeightCountry = 0.042071, WeightIndustry = 0.06555 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NORILSK NICKEL MMC (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00631526, WeightCountry = 0.058373, WeightIndustry = 0.050644 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NOVATEK GDR (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00199747, WeightCountry = 0.018463, WeightIndustry = 0.010074 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NOVOLIPETSK METAL GDR", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0013473, WeightCountry = 0.012269, WeightIndustry = 0.010644 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PHARMSTANDARD GDR", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00062084, WeightCountry = 0.005739, WeightIndustry = 0.039628 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PIK GROUP GDR", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0008103, WeightCountry = 0.00749, WeightIndustry = 0.076803 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POLYUS GOLD (RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00130089, WeightCountry = 0.012024, WeightIndustry = 0.010434 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ROSNEFT (RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00434442, WeightCountry = 0.040156, WeightIndustry = 0.021911 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ROSTELECOM COMMON (RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00134119, WeightCountry = 0.012212, WeightIndustry = 0.033453 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SBERBANK RUSSIA COM(RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00800763, WeightCountry = 0.074016, WeightIndustry = 0.05592 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SBERBANK RUSSIA PREF(RUB", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024894, WeightCountry = 0.002301, WeightIndustry = 0.001738 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SEVERSTAL (RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00154009, WeightCountry = 0.014235, WeightIndustry = 0.01235 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SURGUTNEFTEGAZ COMN(USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00234755, WeightCountry = 0.021699, WeightIndustry = 0.01184 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SURGUTNEFTEGAZ PREF(RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00106812, WeightCountry = 0.009873, WeightIndustry = 0.005387 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TATNEFT COMMON (RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00283547, WeightCountry = 0.026209, WeightIndustry = 0.014301 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TMK GDR", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00063573, WeightCountry = 0.005876, WeightIndustry = 0.084409 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TRANSNEFT PREF (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00061965, WeightCountry = 0.005728, WeightIndustry = 0.003125 });
                result.Add(new IndexConstituentsData() { ConstituentName = "URALKALI COMMON (RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00342451, WeightCountry = 0.031654, WeightIndustry = 0.130101 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VIMPELCOM ADR (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00392146, WeightCountry = 0.036247, WeightIndustry = 0.098699 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VTB BANK(RUB)", Country = "RUS", Region = "RUB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00172731, WeightCountry = 0.015966, WeightIndustry = 0.012062 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WIMM-BILL-DANN ADR (USD)", Country = "RUS", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00047608, WeightCountry = 0.0044, WeightIndustry = 0.035194 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ADVANCED INFO SERVICE", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00074701, WeightCountry = 0.054805, WeightIndustry = 0.010758 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AIRPORTS OF THAILAND", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0001901, WeightCountry = 0.013947, WeightIndustry = 0.037883 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANGKOK BANK", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051666, WeightCountry = 0.037905, WeightIndustry = 0.003608 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANGKOK BANK FGN", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00102116, WeightCountry = 0.074918, WeightIndustry = 0.007131 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANK OF AYUDHYA", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051907, WeightCountry = 0.038082, WeightIndustry = 0.003625 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BANPU", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00077024, WeightCountry = 0.05651, WeightIndustry = 0.003885 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BEC WORLD", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00020062, WeightCountry = 0.014719, WeightIndustry = 0.022796 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CENTRAL PATTANA PUB CO", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013917, WeightCountry = 0.01021, WeightIndustry = 0.012692 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CP ALL PCL", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021264, WeightCountry = 0.0156, WeightIndustry = 0.020048 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GLOW ENERGY", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013743, WeightCountry = 0.010082, WeightIndustry = 0.018337 });
                result.Add(new IndexConstituentsData() { ConstituentName = "IRPC PUBLIC COMPANY", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035297, WeightCountry = 0.025896, WeightIndustry = 0.00178 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KASIKORNBANK", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030921, WeightCountry = 0.022685, WeightIndustry = 0.002159 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KASIKORNBANK FGN", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00076824, WeightCountry = 0.056362, WeightIndustry = 0.005365 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KRUNG THAI BANK", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021488, WeightCountry = 0.015765, WeightIndustry = 0.001501 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LAND & HOUSES FGN", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00014616, WeightCountry = 0.010723, WeightIndustry = 0.013853 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PTT", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00234248, WeightCountry = 0.17039, WeightIndustry = 0.011713 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PTT AROM & REFIN PUBLIC", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024808, WeightCountry = 0.0182, WeightIndustry = 0.001251 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PTT CHEMICAL", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00028595, WeightCountry = 0.020979, WeightIndustry = 0.010864 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PTT EXPLORATION & PROD", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00202718, WeightCountry = 0.148725, WeightIndustry = 0.010224 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RATCHABURI ELECTRICITY", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013853, WeightCountry = 0.010163, WeightIndustry = 0.018484 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SIAM CEMENT", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00018965, WeightCountry = 0.013914, WeightIndustry = 0.015752 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SIAM CEMENT FGN", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00054319, WeightCountry = 0.039851, WeightIndustry = 0.045116 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SIAM COMMERCIAL BANK", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00069691, WeightCountry = 0.051129, WeightIndustry = 0.004867 });
                result.Add(new IndexConstituentsData() { ConstituentName = "THAI MILITARY BANK ORD", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022037, WeightCountry = 0.016168, WeightIndustry = 0.001539 });
                result.Add(new IndexConstituentsData() { ConstituentName = "THAI OIL", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00038608, WeightCountry = 0.028345, WeightIndustry = 0.001947 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TOTAL ACCESS COM", Country = "THA", Region = "THB", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034639, WeightCountry = 0.023946, WeightIndustry = 0.004701 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AKBANK", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0008703, WeightCountry = 0.069178, WeightIndustry = 0.006078 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AKSIGORTA", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00011235, WeightCountry = 0.008931, WeightIndustry = 0.005543 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ANADOLU EFES BIRACILIK", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051688, WeightCountry = 0.041085, WeightIndustry = 0.055168 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ARCELIK", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00012192, WeightCountry = 0.009691, WeightIndustry = 0.011556 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASYA KATILIM BANKASI", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021868, WeightCountry = 0.017382, WeightIndustry = 0.001527 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BIM BIRLESIK MAGAZALAR", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00040051, WeightCountry = 0.031835, WeightIndustry = 0.037761 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COCA-COLA ICECEK", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00017481, WeightCountry = 0.013895, WeightIndustry = 0.018657 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DOGAN SIRKETLER GRUBU", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016879, WeightCountry = 0.013417, WeightIndustry = 0.008884 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DOGAN YAYIN HOLDING", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00006801, WeightCountry = 0.005406, WeightIndustry = 0.007727 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ENKA INSAAT VE SANAYI", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00084821, WeightCountry = 0.067422, WeightIndustry = 0.044645 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EREGLI DEMIR CELIK FAB.", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00131814, WeightCountry = 0.104775, WeightIndustry = 0.010571 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FORD OTOMOTIVE SANAYII", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013608, WeightCountry = 0.010816, WeightIndustry = 0.016769 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KOC HOLDING", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031337, WeightCountry = 0.024909, WeightIndustry = 0.016494 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MIGROS TURK TICARET", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043913, WeightCountry = 0.034905, WeightIndustry = 0.041402 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PETKIM PETROKIMYA HLDG", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00009407, WeightCountry = 0.007478, WeightIndustry = 0.003574 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SABANCI HLDG (HACI OMER)", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00043073, WeightCountry = 0.034238, WeightIndustry = 0.026085 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAV HAVALIMANLARI HLDG", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021666, WeightCountry = 0.017222, WeightIndustry = 0.043177 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TEKFEN HOLDING AS", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024911, WeightCountry = 0.019801, WeightIndustry = 0.014696 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TOFAS TURK OTOMOBIL FAB.", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00010431, WeightCountry = 0.008291, WeightIndustry = 0.012854 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TUPRAS TURKIYE PETROL", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00078363, WeightCountry = 0.062288, WeightIndustry = 0.003952 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TURK TELEKOMUNIKASYON", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00048191, WeightCountry = 0.038305, WeightIndustry = 0.012129 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TURKCELL ILETISIM HIZMET", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0011258, WeightCountry = 0.089487, WeightIndustry = 0.016213 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TURKIYE GARANTI BANKASI", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00138129, WeightCountry = 0.109795, WeightIndustry = 0.009646 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TURKIYE HALK BANKASI", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00041417, WeightCountry = 0.034921, WeightIndustry = 0.002892 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TURKIYE IS BANKASI C", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00088084, WeightCountry = 0.070016, WeightIndustry = 0.006151 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TURKIYE SISE VE CAM FAB.", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00008389, WeightCountry = 0.006668, WeightIndustry = 0.007951 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TURKIYE VAKIFLAR BANKASI", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00026875, WeightCountry = 0.021362, WeightIndustry = 0.001877 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YAPI VE KREDI BANKASI", Country = "TUR", Region = "TRY", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035831, WeightCountry = 0.028481, WeightIndustry = 0.002502 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ACER", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00136948, WeightCountry = 0.012986, WeightIndustry = 0.091342 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ADVANTECH CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022595, WeightCountry = 0.002143, WeightIndustry = 0.015068 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASE", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00106864, WeightCountry = 0.010133, WeightIndustry = 0.02086 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASIA CEMENT CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00073008, WeightCountry = 0.006923, WeightIndustry = 0.060638 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASIA OPTICAL", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00010039, WeightCountry = 0.000952, WeightIndustry = 0.10937 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASUSTEK COMPUTER", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00279103, WeightCountry = 0.026465, WeightIndustry = 0.186118 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AU OPTRONICS CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00319643, WeightCountry = 0.030309, WeightIndustry = 0.150948 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CATCHER TECH CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00038817, WeightCountry = 0.003681, WeightIndustry = 0.025885 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CATHAY FINANCIAL HLDS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0035644, WeightCountry = 0.033798, WeightIndustry = 0.175859 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CATHAY REAL ESTATE DEV", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013114, WeightCountry = 0.001244, WeightIndustry = 0.01196 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHANG HWA COMMERCIAL BK", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00079237, WeightCountry = 0.007513, WeightIndustry = 0.005533 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHENG SHIN RUBBER IND", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00026247, WeightCountry = 0.002489, WeightIndustry = 0.111745 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHENG UEI PRECISION IND", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00015544, WeightCountry = 0.001474, WeightIndustry = 0.007341 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHI MEI OPTOELECTRONICS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00143444, WeightCountry = 0.013583, WeightIndustry = 0.067645 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA AIRLINES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00018003, WeightCountry = 0.001707, WeightIndustry = 0.065916 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA DEV FINANCIAL HLDS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00103463, WeightCountry = 0.009792, WeightIndustry = 0.007211 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA MOTOR CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00010987, WeightCountry = 0.001042, WeightIndustry = 0.01354 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINA STEEL CORP COMMON", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00410386, WeightCountry = 0.038914, WeightIndustry = 0.03491 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHINATRUST FINL HLDGS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00206688, WeightCountry = 0.019599, WeightIndustry = 0.014434 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHUNGHWA PICTURE TUBES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00058468, WeightCountry = 0.005544, WeightIndustry = 0.027611 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHUNGHWA TELECOM CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00361277, WeightCountry = 0.034257, WeightIndustry = 0.090929 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CMC MAGNETICS CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0002279, WeightCountry = 0.002161, WeightIndustry = 0.015198 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COMPAL COMMUNICATIONS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00009512, WeightCountry = 0.000902, WeightIndustry = 0.084008 });
                result.Add(new IndexConstituentsData() { ConstituentName = "COMPAL ELECTRONICS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00122561, WeightCountry = 0.011621, WeightIndustry = 0.081729 });
                result.Add(new IndexConstituentsData() { ConstituentName = "D-LINK CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00020206, WeightCountry = 0.001916, WeightIndustry = 0.178453 });
                result.Add(new IndexConstituentsData() { ConstituentName = "DELTA ELECTRONICS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00125304, WeightCountry = 0.011882, WeightIndustry = 0.059173 });
                result.Add(new IndexConstituentsData() { ConstituentName = "E.SUN FINANCIAL HOLDINGS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00046006, WeightCountry = 0.004362, WeightIndustry = 0.003413 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EPISTAR CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027803, WeightCountry = 0.002636, WeightIndustry = 0.005427 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ETERNAL CHEMICAL CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013698, WeightCountry = 0.001299, WeightIndustry = 0.005204 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EVA AIRWAYS CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023362, WeightCountry = 0.002215, WeightIndustry = 0.085535 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EVERGREEN MARINE CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0002394, WeightCountry = 0.00227, WeightIndustry = 0.034249 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EVERLIGHT ELECTRONICS CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021216, WeightCountry = 0.002012, WeightIndustry = 0.010019 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FAR EAST DEPT STORES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024315, WeightCountry = 0.002306, WeightIndustry = 0.058399 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FAR EASTERN TEXTILE", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00103986, WeightCountry = 0.00986, WeightIndustry = 0.054734 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FAR EASTONE TELECOM. CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00076804, WeightCountry = 0.007283, WeightIndustry = 0.011061 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FARGLORY DEVELOPERS CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00015349, WeightCountry = 0.001455, WeightIndustry = 0.013997 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FENG HSIN IRON & STEEL", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030777, WeightCountry = 0.002918, WeightIndustry = 0.002468 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FIRICH ENTERPRISES CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00012795, WeightCountry = 0.001213, WeightIndustry = 0.006042 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FIRST FINANCIAL HLDG CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00137171, WeightCountry = 0.013007, WeightIndustry = 0.009579 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FORMOSA CHEMICAL FIBERS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00164712, WeightCountry = 0.015618, WeightIndustry = 0.062576 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FORMOSA PETROCHEMICAL CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00098679, WeightCountry = 0.009357, WeightIndustry = 0.004977 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FORMOSA PLASTIC CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00295749, WeightCountry = 0.028043, WeightIndustry = 0.112358 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FORMOSA TAFFETA CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00025155, WeightCountry = 0.002385, WeightIndustry = 0.179424 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FOXCONN TECHNOLOGY CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0006366, WeightCountry = 0.006036, WeightIndustry = 0.042451 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FUBON FINANCIAL HOLDING", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00128913, WeightCountry = 0.012224, WeightIndustry = 0.07807 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HANNSTAR DISPLAY", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00045002, WeightCountry = 0.004267, WeightIndustry = 0.021251 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HIGH TECH COMPUTER CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0034164, WeightCountry = 0.030499, WeightIndustry = 0.214483 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HON HAI PRECISION IND CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00748938, WeightCountry = 0.071016, WeightIndustry = 0.353677 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HUA NAN FINANCIAL HLDGS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00081666, WeightCountry = 0.007744, WeightIndustry = 0.005703 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INNOLUX DISPLAY", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0009197, WeightCountry = 0.008721, WeightIndustry = 0.06133 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INOTERA MEMORIES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00014967, WeightCountry = 0.001419, WeightIndustry = 0.002922 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INVENTEC APPLIANCES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00010568, WeightCountry = 0.001002, WeightIndustry = 0.010016 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INVENTEC CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0002613, WeightCountry = 0.002478, WeightIndustry = 0.017425 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KGI SECURITIES CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00044537, WeightCountry = 0.004223, WeightIndustry = 0.070381 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KINSUS INTERCONNECT TECH", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016103, WeightCountry = 0.001527, WeightIndustry = 0.007604 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LARGAN PRECISION CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003435, WeightCountry = 0.003457, WeightIndustry = 0.374229 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LITE-ON TECHNOLOGY CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00058991, WeightCountry = 0.005594, WeightIndustry = 0.039338 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MACRONIX INTERNATIONAL", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00036734, WeightCountry = 0.003483, WeightIndustry = 0.00717 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MEDIATEK INC", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00299675, WeightCountry = 0.028416, WeightIndustry = 0.058497 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MEGA FINANCIAL HLDG(CTB)", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0019617, WeightCountry = 0.018601, WeightIndustry = 0.013699 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MITAC INTERNATIONAL", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022442, WeightCountry = 0.002128, WeightIndustry = 0.014965 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MOSEL VITELIC", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00017549, WeightCountry = 0.001664, WeightIndustry = 0.003426 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MOTECH INDUSTRIES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00037544, WeightCountry = 0.00356, WeightIndustry = 0.01773 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NAN YA PLASTIC", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00307886, WeightCountry = 0.029194, WeightIndustry = 0.116969 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NAN YA PRINTED CIRCUIT", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027587, WeightCountry = 0.002616, WeightIndustry = 0.013028 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NANYA TECHNOLOGY CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00035345, WeightCountry = 0.00335, WeightIndustry = 0.006895 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NOVATEK MICROELECTRS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00039575, WeightCountry = 0.003753, WeightIndustry = 0.007725 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PAN-INTERNATIONAL IND", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00013605, WeightCountry = 0.00129, WeightIndustry = 0.006425 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POLARIS SECURITIES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034834, WeightCountry = 0.003113, WeightIndustry = 0.051888 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POU CHEN CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00049835, WeightCountry = 0.004725, WeightIndustry = 0.35546 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POWERCHIP SEMICONDUCTOR", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00065785, WeightCountry = 0.006238, WeightIndustry = 0.012841 });
                result.Add(new IndexConstituentsData() { ConstituentName = "POWERTECH TECHNOLOGY", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051888, WeightCountry = 0.00492, WeightIndustry = 0.010129 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PRESIDENT CHAIN STORE", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00050946, WeightCountry = 0.004831, WeightIndustry = 0.048033 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PRO MOS TECHNOLOGIES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00031034, WeightCountry = 0.002943, WeightIndustry = 0.006057 });
                result.Add(new IndexConstituentsData() { ConstituentName = "QISDA CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00025259, WeightCountry = 0.002395, WeightIndustry = 0.016844 });
                result.Add(new IndexConstituentsData() { ConstituentName = "QUANTA COMPUTER", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00089453, WeightCountry = 0.008482, WeightIndustry = 0.059651 });
                result.Add(new IndexConstituentsData() { ConstituentName = "REALTEK SEMICONDUCTOR", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024734, WeightCountry = 0.002345, WeightIndustry = 0.004828 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RICHTEK TECHNOLOGY CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00025094, WeightCountry = 0.002379, WeightIndustry = 0.004898 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHIN KONG FINL HLDGS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0007297, WeightCountry = 0.006919, WeightIndustry = 0.036002 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SILICONWARE PRECISION", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00121105, WeightCountry = 0.011483, WeightIndustry = 0.02364 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SINOAMERICAN SILICON PRO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00020595, WeightCountry = 0.001953, WeightIndustry = 0.00402 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SINOPAC HOLDINGS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00079539, WeightCountry = 0.007542, WeightIndustry = 0.005554 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SYNNEX TECHNOLOGY INT'L", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00052958, WeightCountry = 0.005022, WeightIndustry = 0.025009 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAISHIN FINANCIAL HLDGS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051874, WeightCountry = 0.004919, WeightIndustry = 0.003623 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIWAN BUSINESS BANK", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00023646, WeightCountry = 0.002242, WeightIndustry = 0.001651 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIWAN CEMENT CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00112626, WeightCountry = 0.010679, WeightIndustry = 0.093545 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIWAN COOPERATIVE BANK", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00078873, WeightCountry = 0.007479, WeightIndustry = 0.005508 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIWAN FERTILIZER CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00073559, WeightCountry = 0.006975, WeightIndustry = 0.027946 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIWAN GLASS IND'L CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00025282, WeightCountry = 0.002397, WeightIndustry = 0.160749 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIWAN MOBILE", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00093799, WeightCountry = 0.008894, WeightIndustry = 0.013509 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIWAN SECOM", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00015342, WeightCountry = 0.001455, WeightIndustry = 1 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TAIWAN SEMICONDUCTOR MFG", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.01360146, WeightCountry = 0.128972, WeightIndustry = 0.265501 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TATUNG", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00042275, WeightCountry = 0.004009, WeightIndustry = 0.028191 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TECO ELECTRIC & MACH.", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0002883, WeightCountry = 0.002734, WeightIndustry = 0.058263 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TRANSCEND INFORMATION", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022114, WeightCountry = 0.002097, WeightIndustry = 0.004317 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TRIPOD TECHNOLOGY CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027781, WeightCountry = 0.002634, WeightIndustry = 0.013119 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TUNG HO STEEL ENTERPRISE", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00041616, WeightCountry = 0.003946, WeightIndustry = 0.003337 });
                result.Add(new IndexConstituentsData() { ConstituentName = "U-MING MARINE TRANSPORT", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033405, WeightCountry = 0.003168, WeightIndustry = 0.04779 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNI-PRESIDENT ENT.", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00102756, WeightCountry = 0.009744, WeightIndustry = 0.075962 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNIMICRON TECHNOLOGY", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00029153, WeightCountry = 0.002764, WeightIndustry = 0.013767 });
                result.Add(new IndexConstituentsData() { ConstituentName = "UNITED MICROELECTRONICS", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00203176, WeightCountry = 0.019266, WeightIndustry = 0.03966 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VANGUARD INT'L SEMICON", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00018826, WeightCountry = 0.001785, WeightIndustry = 0.003675 });
                result.Add(new IndexConstituentsData() { ConstituentName = "VIA TECHNOLOGIES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016971, WeightCountry = 0.001609, WeightIndustry = 0.003313 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WAFER WORKS CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00020467, WeightCountry = 0.001941, WeightIndustry = 0.003995 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WALSIN LIHWA CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00028481, WeightCountry = 0.002701, WeightIndustry = 0.057557 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WAN HAI LINES", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024298, WeightCountry = 0.002304, WeightIndustry = 0.03476 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WINBOND ELECTRONICS CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00019541, WeightCountry = 0.001853, WeightIndustry = 0.003814 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WINTEK", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.000179, WeightCountry = 0.001697, WeightIndustry = 0.008453 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WISTRON CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00051971, WeightCountry = 0.004928, WeightIndustry = 0.034656 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YAGEO CORP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021866, WeightCountry = 0.002073, WeightIndustry = 0.010346 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YANG MING MARINE TRANSP", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022812, WeightCountry = 0.002163, WeightIndustry = 0.034634 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YUANTA FINANCIAL HOLDING", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00151555, WeightCountry = 0.014371, WeightIndustry = 0.091782 });
                result.Add(new IndexConstituentsData() { ConstituentName = "YULON MOTOR CO", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00021207, WeightCountry = 0.002011, WeightIndustry = 0.026134 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ZINWELL CORPORATION", Country = "TAI", Region = "TWD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00016575, WeightCountry = 0.001572, WeightIndustry = 0.015711 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CELLCOM ISRAEL", Country = "ISR", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0004003, WeightCountry = 0.015772, WeightIndustry = 0.005765 });
                result.Add(new IndexConstituentsData() { ConstituentName = "CHECK POINT SOFTW. (USD)", Country = "ISR", Region = "USD", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00127121, WeightCountry = 0.050085, WeightIndustry = 0.779518 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ABSA GROUP", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00095645, WeightCountry = 0.014544, WeightIndustry = 0.006679 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AFRICAN BANK INVESTMENTS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00069639, WeightCountry = 0.01059, WeightIndustry = 0.042174 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AFRICAN RAINBOW MINERALS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00126394, WeightCountry = 0.01922, WeightIndustry = 0.010136 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ANGLO PLATINUM", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00355808, WeightCountry = 0.054106, WeightIndustry = 0.028534 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ANGLOGOLD ASHANTI", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00277964, WeightCountry = 0.042269, WeightIndustry = 0.022291 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ARCELORMITTAL STH AFRICA", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00170122, WeightCountry = 0.02587, WeightIndustry = 0.013643 });
                result.Add(new IndexConstituentsData() { ConstituentName = "ASPEN PHARMACARE HLDGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034427, WeightCountry = 0.004931, WeightIndustry = 0.020698 });
                result.Add(new IndexConstituentsData() { ConstituentName = "AVENG", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00074782, WeightCountry = 0.011372, WeightIndustry = 0.044116 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BARLOWORLD", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0006154, WeightCountry = 0.009358, WeightIndustry = 0.034391 });
                result.Add(new IndexConstituentsData() { ConstituentName = "BIDVEST GROUP", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0009394, WeightCountry = 0.014285, WeightIndustry = 0.049445 });
                result.Add(new IndexConstituentsData() { ConstituentName = "EXXARO RESOURCES", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00067164, WeightCountry = 0.010213, WeightIndustry = 0.005386 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FIRSTRAND", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00142197, WeightCountry = 0.021623, WeightIndustry = 0.086115 });
                result.Add(new IndexConstituentsData() { ConstituentName = "FOSCHINI", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00024766, WeightCountry = 0.003766, WeightIndustry = 0.06255 });
                result.Add(new IndexConstituentsData() { ConstituentName = "GOLD FIELDS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00241241, WeightCountry = 0.036685, WeightIndustry = 0.019346 });
                result.Add(new IndexConstituentsData() { ConstituentName = "HARMONY GOLD MINING CO", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00127259, WeightCountry = 0.019352, WeightIndustry = 0.010205 });
                result.Add(new IndexConstituentsData() { ConstituentName = "IMPALA PLATINUM HOLDINGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00627077, WeightCountry = 0.095357, WeightIndustry = 0.050287 });
                result.Add(new IndexConstituentsData() { ConstituentName = "IMPERIAL HOLDINGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034603, WeightCountry = 0.004958, WeightIndustry = 0.239566 });
                result.Add(new IndexConstituentsData() { ConstituentName = "INVESTEC LTD", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00033441, WeightCountry = 0.005055, WeightIndustry = 0.052531 });
                result.Add(new IndexConstituentsData() { ConstituentName = "KUMBA IRON ORE", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00096268, WeightCountry = 0.014639, WeightIndustry = 0.00772 });
                result.Add(new IndexConstituentsData() { ConstituentName = "LIBERTY GROUP", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00027403, WeightCountry = 0.004167, WeightIndustry = 0.01352 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MASSMART HOLDINGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00046914, WeightCountry = 0.007134, WeightIndustry = 0.044234 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MTN GROUP", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00733881, WeightCountry = 0.111598, WeightIndustry = 0.10569 });
                result.Add(new IndexConstituentsData() { ConstituentName = "MURRAY & ROBERTS HLDGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00095807, WeightCountry = 0.014569, WeightIndustry = 0.050427 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NASPERS N", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00259937, WeightCountry = 0.039528, WeightIndustry = 0.295349 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NEDBANK GROUP", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0007144, WeightCountry = 0.010864, WeightIndustry = 0.004989 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NETCARE", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030665, WeightCountry = 0.004663, WeightIndustry = 1 });
                result.Add(new IndexConstituentsData() { ConstituentName = "NORTHAM PLATINUM", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00030249, WeightCountry = 0.0046, WeightIndustry = 0.002426 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PICK'N PAY STORES", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00022829, WeightCountry = 0.003471, WeightIndustry = 0.021524 });
                result.Add(new IndexConstituentsData() { ConstituentName = "PRETORIA PORTLAND CEMENT", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00057604, WeightCountry = 0.00876, WeightIndustry = 0.047845 });
                result.Add(new IndexConstituentsData() { ConstituentName = "REMGRO", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0034472, WeightCountry = 0.049379, WeightIndustry = 0.196652 });
                result.Add(new IndexConstituentsData() { ConstituentName = "REUNERT", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00034717, WeightCountry = 0.004975, WeightIndustry = 0.01545 });
                result.Add(new IndexConstituentsData() { ConstituentName = "RMB HOLDINGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00059263, WeightCountry = 0.009012, WeightIndustry = 0.03589 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SANLAM", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00133719, WeightCountry = 0.020334, WeightIndustry = 0.065974 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SAPPI", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0007672, WeightCountry = 0.011666, WeightIndustry = 0.167599 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SASOL", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.01048696, WeightCountry = 0.159471, WeightIndustry = 0.052891 });
                result.Add(new IndexConstituentsData() { ConstituentName = "SHOPRITE HOLDINGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00064782, WeightCountry = 0.009851, WeightIndustry = 0.061078 });
                result.Add(new IndexConstituentsData() { ConstituentName = "STANDARD BANK GROUP", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00346585, WeightCountry = 0.049663, WeightIndustry = 0.022806 });
                result.Add(new IndexConstituentsData() { ConstituentName = "STEINHOFF INT'L HOLDINGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0006527, WeightCountry = 0.009925, WeightIndustry = 0.061865 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TELKOM SOUTH AFRICA", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00160308, WeightCountry = 0.024377, WeightIndustry = 0.040348 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TIGER BRANDS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00087734, WeightCountry = 0.013341, WeightIndustry = 0.064857 });
                result.Add(new IndexConstituentsData() { ConstituentName = "TRUWORTHS INTERNATIONAL", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.00037602, WeightCountry = 0.005718, WeightIndustry = 0.09497 });
                result.Add(new IndexConstituentsData() { ConstituentName = "WOOLWORTHS HOLDINGS", Country = "SA", Region = "ZAR", Sector = "", Industry = "", SubIndustry = "", Weight = 0.0003116, WeightCountry = 0.004738, WeightIndustry = 0.07484 });
                #endregion

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
            result.Add(new HoldingsPercentageData() { SegmentName = "Segment1", SegmentHoldingsShare = 20 });
            result.Add(new HoldingsPercentageData() { SegmentName = "Segment2", SegmentHoldingsShare = 10 });
            result.Add(new HoldingsPercentageData() { SegmentName = "Segment3", SegmentHoldingsShare = 30 });
            result.Add(new HoldingsPercentageData() { SegmentName = "Segment4", SegmentHoldingsShare = 40 });
            return result;
        }

        [OperationContract]
        public List<TopBenchmarkSecuritiesData> RetrieveTopBenchmarkSecuritiesData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            List<TopBenchmarkSecuritiesData> result = new List<TopBenchmarkSecuritiesData>();
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 1", Weight = 10.111, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 2", Weight = 1.999, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 3", Weight = 3.99988, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 4", Weight = 1.9909, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 5", Weight = 18.098, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 6", Weight = 19.987, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 7", Weight = 19.0976, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 8", Weight = 15.677, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 9", Weight = 10.3777, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            result.Add(new TopBenchmarkSecuritiesData() { IssuerName = "Company 10", Weight = 10.999, MTD = 5, QTD = 6, YTD = 8, PreviousYear = 9, SecondPreviousYear = 7, ThirdPreviousYear = 12 });
            return result;
        }

        [OperationContract]
        public List<PortfolioRiskReturnData> RetrievePortfolioRiskReturnData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            List<PortfolioRiskReturnData> portfolioRiskReturnValues = new List<PortfolioRiskReturnData>();
            try
            {
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Expected Return",
                    PortfolioValue = 18.1.ToString(),
                    BenchMarkValue = 15.3.ToString()
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Alpha",
                    PortfolioValue = 1.8.ToString(),
                    BenchMarkValue = "N/A"
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Beta",
                    PortfolioValue = 0.95.ToString(),
                    BenchMarkValue = "N/A"
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Standard Deviation",
                    PortfolioValue = 15.1.ToString(),
                    BenchMarkValue = 15.7.ToString()
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Sharpe Ratio",
                    PortfolioValue = 0.18.ToString(),
                    BenchMarkValue = 0.13.ToString()
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Information Ratio",
                    PortfolioValue = 1.81.ToString()
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Turnover Ratio",
                    PortfolioValue = 11.14.ToString()
                });
            }
            catch
            {
            }

            return portfolioRiskReturnValues;
        }
        /// <summary>
        /// Retrieving the Theoretical Unrealized Gain Loss Data for selected Entity.
        /// </summary>
        /// <param name="entityIdentifier">Ticker for the security</param>
        /// <param name="startDateTime">Start Date of the Time Period that is selected</param>
        /// <param name="endDateTime">End Date of the Time Period that is selected</param>       
        /// <param name="frequencyInterval">Frequency Duration selected</param>       
        /// <returns>List of UnrealozedGainLossData</returns>
        [OperationContract]
        public List<UnrealizedGainLossData> RetrieveUnrealizedGainLossData(string entityIdentifier, DateTime startDateTime, DateTime endDateTime, string frequencyInterval)
        {

            List<UnrealizedGainLossData> timeAndFrequencyFilteredGainLossResult = new List<UnrealizedGainLossData>();
            int noOfRows;
            try
            {
                if (entityIdentifier != null && startDateTime != null && endDateTime != null && frequencyInterval != null)
                {
                    DimensionEntitiesService.Entities entity = DimensionEntity;
                    List<DimensionEntitiesService.GF_PRICING_BASEVIEW> arrangedByDescRecord = entity.GF_PRICING_BASEVIEW
                    .Where(r => (r.TICKER == entityIdentifier)).OrderByDescending(res => res.FROMDATE).ToList();
                    noOfRows = arrangedByDescRecord.Count();
                    //Calculating the Adjusted price for a security and storing it in the list.
                    List<UnrealizedGainLossData> adjustedPriceResult = UnrealizedGainLossCalculations.CalculateAdjustedPrice(arrangedByDescRecord, noOfRows);
                    //Calculating the Moving Average for a security and storing it in the list.
                    List<UnrealizedGainLossData> movingAverageResult = UnrealizedGainLossCalculations.CalculateMovingAverage(adjustedPriceResult, noOfRows);
                    //Calculating the Ninety Day Weight for a security and storing it in the list.
                    List<UnrealizedGainLossData> ninetyDayWtResult = UnrealizedGainLossCalculations.CalculateNinetyDayWtAvg(movingAverageResult, noOfRows);
                    //Calculating the Cost for a security and storing it in the list.
                    List<UnrealizedGainLossData> costResult = UnrealizedGainLossCalculations.CalculateCost(ninetyDayWtResult, noOfRows);
                    //Calculating the Weighted Average Cost for a security and storing it in the list.
                    List<UnrealizedGainLossData> wtAvgCostResult = UnrealizedGainLossCalculations.CalculateWtAvgCost(costResult, noOfRows);
                    //Calculating the Unrealized Gain loss for a security and storing it in the list.
                    List<UnrealizedGainLossData> unrealizedGainLossResult = UnrealizedGainLossCalculations.CalculateUnrealizedGainLoss(wtAvgCostResult, noOfRows);
                    //Filtering the list according to the time period selected
                    List<UnrealizedGainLossData> timeFilteredUnrealizedGainLossResult = unrealizedGainLossResult.Where(r => (r.FromDate >= startDateTime) && (r.FromDate < endDateTime)).ToList();
                    //Filtering the list according to the frequency selected.
                    List<DateTime> EndDates = (from p in timeFilteredUnrealizedGainLossResult
                                               select p.FromDate).ToList();                   
                    List<DateTime> allEndDates = FrequencyCalculator.RetrieveDatesAccordingToFrequency(EndDates, startDateTime, endDateTime, frequencyInterval);
                    timeAndFrequencyFilteredGainLossResult = RetrieveUnrealizedGainLossData(timeFilteredUnrealizedGainLossResult, allEndDates);
                }

                return timeAndFrequencyFilteredGainLossResult;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Morning Snapshot Operation Contracts

        [OperationContract]
        public List<UserBenchmarkPreference> RetrieveUserPreferenceBenchmarkData(string userName)
        {
            try
            {
                if (userName != null)
                {
                    ResearchEntities entity = new ResearchEntities();
                    List<UserBenchmarkPreference> userPreference = (entity.GetUserBenchmarkPreference(userName)).ToList<UserBenchmarkPreference>();
                    return userPreference;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [OperationContract]
        public List<MorningSnapshotData> RetrieveMorningSnapshotData(List<UserBenchmarkPreference> userBenchmarkPreference)
        {

            List<MorningSnapshotData> result = new List<MorningSnapshotData>();
            foreach (UserBenchmarkPreference preference in userBenchmarkPreference)
            {
                if (preference.BenchmarkName != null)
                {
                    result.Add(new MorningSnapshotData()
                    {
                        MorningSnapshotPreferenceInfo = preference,
                        DTD = -0.1,
                        WTD = -0.1,
                        MTD = 4.4,
                        QTD = 4.4,
                        YTD = 7.4,
                        PreviousYearPrice = 4.6,
                        IIPreviousYearPrice = 52.3,
                        IIIPreviousYearPrice = -50.8
                    });
                }
                else
                {
                    result.Add(new MorningSnapshotData()
                    {
                        MorningSnapshotPreferenceInfo = preference
                    });
                }
            }

            return result;
        }

        [OperationContract]
        public bool AddUserPreferenceBenchmarkGroup(string userName, string groupName)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.SetUserGroupPreference(userName, groupName);
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        [OperationContract]
        public bool RemoveUserPreferenceBenchmarkGroup(string userName, string groupname)
        {
            try
            {
                ResearchEntities entity = new ResearchEntities();
                entity.DeleteUserGroupPreference(userName, groupname);
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        [OperationContract]
        public bool AddUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.SetUserBenchmarkPreference(userName, userBenchmarkPreference.GroupName, userBenchmarkPreference.BenchmarkName, userBenchmarkPreference.BenchmarkReturnType);
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        [OperationContract]
        public bool RemoveUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.DeleteUserBenchmarkPreference(userName, userBenchmarkPreference.GroupName, userBenchmarkPreference.BenchmarkName);
                return true;
            }

            catch (Exception)
            { return false; }
        }
        #endregion

        #region Pricing Chart Helper Methods

        /// <summary>
        /// Method to Retrieve Data Filtered according to Frequency.
        /// If for a particular day , data is not present then the data for 1-day before is considered.
        /// </summary>
        /// <param name="objPricingData"></param>
        /// <param name="objEndDates"></param>
        /// <returns></returns>
        private List<PricingReferenceData> RetrievePricingDataAccordingFrequency(List<PricingReferenceData> objPricingData, List<DateTime> objEndDates)
        {
            List<PricingReferenceData> resultFrequency = new List<PricingReferenceData>();
            List<DateTime> EndDates = objEndDates;

            foreach (DateTime item in EndDates)
            {
                int i = 1;
                bool dateObjectFound = true;

                if (objPricingData.Any(r => r.FromDate == item))
                {
                    resultFrequency.Add(objPricingData.Where(r => r.FromDate == item).First());
                    dateObjectFound = false;
                    continue;
                }
                else
                {
                    dateObjectFound = true;
                }

                while (dateObjectFound)
                {
                    bool objDataFoundDec = objPricingData.Any(r => r.FromDate == item.AddDays(-i));
                    if (objDataFoundDec)
                    {
                        resultFrequency.Add(objPricingData.Where(r => r.FromDate == item.AddDays(-i)).First());
                        dateObjectFound = false;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return resultFrequency;
        }

        /// <summary>
        /// Method to Retrieve Data Filtered according to Frequency.
        /// If for a particular day , data is not present then the data for 1-day before is considered.
        /// </summary>
        /// <param name="objUnrealizedGainLossData"></param>
        /// <param name="objEndDates"></param>
        /// <returns></returns>
        private List<UnrealizedGainLossData> RetrieveUnrealizedGainLossData(List<UnrealizedGainLossData> objUnrealizedGainLossData, List<DateTime> objEndDates)
        {
            List<UnrealizedGainLossData> resultFrequency = new List<UnrealizedGainLossData>();

            List<DateTime> EndDates = objEndDates;
            foreach (DateTime item in EndDates)
            {
                int i = 1;
                bool dateObjectFound = true;

                if (objUnrealizedGainLossData.Any(r => r.FromDate == item))
                {
                    resultFrequency.Add(objUnrealizedGainLossData.Where(r => r.FromDate == item).First());
                    dateObjectFound = false;
                    continue;
                }
                else
                {
                    dateObjectFound = true;
                }

                while (dateObjectFound)
                {
                    bool objDataFoundDec = objUnrealizedGainLossData.Any(r => r.FromDate == item.AddDays(-i));
                    if (objDataFoundDec)
                    {
                        resultFrequency.Add(objUnrealizedGainLossData.Where(r => r.FromDate == item.AddDays(-i)).First());
                        dateObjectFound = false;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return resultFrequency;
        }

        #endregion

        #region Connection String Methods
        private string GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"ND1DDYYB6Q1\SQLEXPRESS";
            builder.InitialCatalog = "AshmoreEMMPOC";
            builder.UserID = "sa";
            builder.Password = "India@123";
            builder.MultipleActiveResultSets = true;
            return builder.ConnectionString;
        }

        private DataTable GetDataTable(string queryString)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable dataTable = new DataTable();
                dataTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

                try
                {
                    sqlDataAdapter.Fill(dataTable);
                    connection.Close();
                }
                catch (Exception)
                {

                    return null;
                }

                return dataTable;
            }
        }
        #endregion

    }
}
