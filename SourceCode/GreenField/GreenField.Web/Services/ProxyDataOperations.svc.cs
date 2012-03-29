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
                if (data != null)
                {
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
                object sumPortfolioWeight = dataTable.Compute("Sum(PORTFOLIO_WEIGHT)", "");
                object sumBenchmarkWeight = dataTable.Compute("Sum(BENCHMARK_WEIGHT)", "");

                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new SectorBreakdownData()
                    {
                        Sector = row.Field<string>("GICS_SECTOR_NAME"),
                        Industry = row.Field<string>("GICS_INDUSTRY_NAME"),
                        Security = row.Field<string>("ISSUE_NAME"),
                        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT") / (sumPortfolioWeight as Single?),
                        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT") / (sumBenchmarkWeight as Single?),
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
                object sumPortfolioWeight = dataTable.Compute("Sum(PORTFOLIO_WEIGHT)", "");
                object sumBenchmarkWeight = dataTable.Compute("Sum(BENCHMARK_WEIGHT)", "");

                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new RegionBreakdownData()
                    {
                        Region = row.Field<string>("ASHEMM_PROPRIETARY_REGION_CODE"),
                        Country = row.Field<string>("ISO_COUNTRY_CODE"),
                        Security = row.Field<string>("ISSUE_NAME"),
                        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT") / (sumPortfolioWeight as Single?),
                        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT") / (sumBenchmarkWeight as Single?),
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
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                object sumMarketValue = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "");

                foreach (DataRow row in dataTable.Rows)
                {
                    string country = row.Field<string>("ISO_COUNTRY_CODE");
                    object sumMarketValueCountry = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "ISO_COUNTRY_CODE = '" + country + "'");

                    string industry = row.Field<string>("GICS_INDUSTRY_NAME");
                    object sumMarketValueIndustry = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "GICS_INDUSTRY_NAME = '" + industry + "'");
                    result.Add(new IndexConstituentsData()
                    {
                        ConstituentName = row.Field<string>("ISSUE_NAME"),
                        Country = country,
                        Region = row.Field<string>("ASHEMM_PROPRIETARY_REGION_CODE"),
                        Sector = row.Field<string>("GICS_SECTOR_NAME"),
                        Industry = industry,
                        SubIndustry = row.Field<string>("GICS_SUB_INDUSTRY_NAME"),
                        Weight = (double)row.Field<Single>("DIRTY_VALUE_PC") / (double)sumMarketValue,
                        WeightCountry = (double)row.Field<Single>("DIRTY_VALUE_PC") / (double)sumMarketValueCountry,
                        WeightIndustry = (double)row.Field<Single>("DIRTY_VALUE_PC") / (double)sumMarketValueIndustry,
                        DailyReturnUSD = row.Field<double>("ISSUE_NAME")
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
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate,String filterType,String filterValue)
        {
            //public List<HoldingsPercentageData> RetrieveHoldingsPercentageData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, int classifier) 

            List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
            List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
            HoldingsPercentageData entry = new HoldingsPercentageData();
            ResearchEntities research = new ResearchEntities();
            holdingData = research.tblHoldingsDatas.ToList();          
          
            switch (filterType)
            {
                case "Region":
                  var q = from p in holdingData 
                          where (p.ASHEMM_PROPRIETARY_REGION_CODE.ToString()).Equals(filterValue)
                          group p by p.GICS_SECTOR_NAME into g                          
                          select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };          

                          foreach (var a in q)
                         {
                           entry = new HoldingsPercentageData();
                           entry.SegmentName = a.SectorName;
                           entry.BenchmarkWeight =(Convert.ToDouble(a.BenchmarkSum))*100;
                           entry.PortfolioWeight = (Convert.ToDouble(a.PortfolioSum))*100;
                           result.Add(entry);
                         }
                    break;
                case "Country":
                     var l = from p in holdingData 
                          where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                          group p by p.GICS_SECTOR_NAME into g                          
                          select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };          

                          foreach (var a in l)
                         {
                           entry = new HoldingsPercentageData();
                           entry.SegmentName = a.SectorName;
                           entry.BenchmarkWeight = (Convert.ToDouble(a.BenchmarkSum))*100;
                           entry.PortfolioWeight = (Convert.ToDouble(a.PortfolioSum))*100;
                           result.Add(entry);
                         }
                    break;
                case "Industry":
                    var m = from p in holdingData 
                          where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                          group p by p.GICS_SECTOR_NAME into g                          
                          select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };          

                          foreach (var a in m)
                         {
                           entry = new HoldingsPercentageData();
                           entry.SegmentName = a.SectorName;
                           entry.BenchmarkWeight = (Convert.ToDouble(a.BenchmarkSum))*100;
                           entry.PortfolioWeight = (Convert.ToDouble(a.PortfolioSum))*100;
                           result.Add(entry);
                         }
                    break;
                case "Sector":
                     var n = from p in holdingData 
                          where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                          group p by p.GICS_SUB_INDUSTRY_NAME into g                          
                          select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };          

                          foreach (var a in n)
                         {
                           entry = new HoldingsPercentageData();
                           entry.SegmentName = a.SectorName;
                           entry.BenchmarkWeight = (Convert.ToDouble(a.BenchmarkSum))*100;
                           entry.PortfolioWeight = (Convert.ToDouble(a.PortfolioSum))*100;
                           result.Add(entry);
                         }
                    break;
                default:
                    break;
            }
            return result;         
        }

        [OperationContract]
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageDataForRegion(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, String filterType, String filterValue)
        {
            //public List<HoldingsPercentageData> RetrieveHoldingsPercentageData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, int classifier) 

            List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
            List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
            HoldingsPercentageData entry = new HoldingsPercentageData();
            ResearchEntities research = new ResearchEntities();
            holdingData = research.tblHoldingsDatas.ToList();

            switch (filterType)
            {
                case "Region":
                    var q = from p in holdingData
                            where (p.ASHEMM_PROPRIETARY_REGION_CODE.ToString()).Equals(filterValue)
                            group p by p.ISO_COUNTRY_CODE into g
                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };

                    foreach (var a in q)
                    {
                        entry = new HoldingsPercentageData();
                        entry.SegmentName = a.SectorName;
                        entry.BenchmarkWeight = (Convert.ToDouble(a.BenchmarkSum))*100;
                        entry.PortfolioWeight = (Convert.ToDouble(a.PortfolioSum))*100;
                        result.Add(entry);
                    }
                    break;
                case "Country":
                    var l = from p in holdingData
                            where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                            group p by p.ASHEMM_PROPRIETARY_REGION_CODE into g
                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };

                    foreach (var a in l)
                    {
                        entry = new HoldingsPercentageData();
                        entry.SegmentName = a.SectorName;
                        entry.BenchmarkWeight = (Convert.ToDouble(a.BenchmarkSum))*100;
                        entry.PortfolioWeight = (Convert.ToDouble(a.PortfolioSum))*100;
                        result.Add(entry);
                    }
                    break;
                case "Industry":
                    var m = from p in holdingData
                            where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                            group p by p.ASHEMM_PROPRIETARY_REGION_CODE into g
                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };

                    foreach (var a in m)
                    {
                        entry = new HoldingsPercentageData();
                        entry.SegmentName = a.SectorName;
                        entry.BenchmarkWeight = (Convert.ToDouble(a.BenchmarkSum))*100;
                        entry.PortfolioWeight = (Convert.ToDouble(a.PortfolioSum))*100;
                        result.Add(entry);
                    }
                    break;
                case "Sector":
                    var n = from p in holdingData
                            where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                            group p by p.ASHEMM_PROPRIETARY_REGION_CODE into g
                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };

                    foreach (var a in n)
                    {
                        entry = new HoldingsPercentageData();
                        entry.SegmentName = a.SectorName;
                        entry.BenchmarkWeight = (Convert.ToDouble(a.BenchmarkSum))*100;
                        entry.PortfolioWeight = (Convert.ToDouble(a.PortfolioSum))*100;
                        result.Add(entry);
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        [OperationContract]
        public List<TopBenchmarkSecuritiesData> RetrieveTopBenchmarkSecuritiesData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            List<TopBenchmarkSecuritiesData> result = new List<TopBenchmarkSecuritiesData>();
            List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
            List<tblHoldingsData> top10HoldingData = new List<tblHoldingsData>();
            TopBenchmarkSecuritiesData entry = new TopBenchmarkSecuritiesData();
            ResearchEntities research = new ResearchEntities();
            holdingData = research.tblHoldingsDatas.ToList();
            top10HoldingData = (from p in holdingData orderby p.BENCHMARK_WEIGHT descending select p).Take(10).ToList();

            foreach (tblHoldingsData item in top10HoldingData)
            {
                entry = new TopBenchmarkSecuritiesData();
                entry.Weight = Convert.ToDouble(item.BENCHMARK_WEIGHT);
                entry.IssuerName = item.ISSUE_NAME;
                result.Add(entry);
            }
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

        [OperationContract]
        public List<String> RetrieveValuesForFilters(String filterType)
        {
            List<String> result = new List<String>();
            List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
            ResearchEntities research = new ResearchEntities();
            holdingData = research.tblHoldingsDatas.ToList();
            switch(filterType)
                {
                  case "Region" :
                        result = (from p in holdingData select p.ASHEMM_PROPRIETARY_REGION_CODE.ToString()).Distinct().ToList();
                   break;
                 case "Country":
                        result = (from p in holdingData select p.ISO_COUNTRY_CODE.ToString()).Distinct().ToList();
                   break;         
                 case "Industry":
                       result = (from p in holdingData select p.GICS_INDUSTRY_NAME.ToString()).Distinct().ToList();
                    break;
                 case "Sector":
                    result = (from p in holdingData select p.GICS_SECTOR_NAME.ToString()).Distinct().ToList();
                    break;
                 default:
                   break;
              }
            return result;       
        }

        #region Build2 Services

        /// <summary>
        /// To Retrieve the Data for the PortfolioDetails UI
        /// </summary>
        /// <param name="objPortfolioIdentifier">Selected Portfolio</param>
        /// <returns>List of PortfolioDetailsData</returns>
        [OperationContract]
        public List<PortfolioDetailsData> RetrievePortfolioDetailsData(string objPortfolioIdentifier)
        {
            List<PortfolioDetailsData> result = new List<PortfolioDetailsData>();
            try
            {
                Random random = new Random();
                for (int i = 0; i < 5; i++)
                {
                    result.Add(new PortfolioDetailsData()
                    {
                        EntityTicker = "TCS IN",
                        EntityName = "TATA CONSULTANCY SVCS LTD",
                        Type = "Security",
                        Country = "India",
                        Shares = 345565,
                        Price = random.Next(200, 1000),
                        Currency = "INR",
                        Value = 0.019995,
                        TargetPerc = random.Next(10, 30),
                        PortfolioPerc = random.Next(10, 30),
                        BenchmarkPerc = random.Next(10, 30),
                        BetPerc = random.Next(0, 20),
                        Upside = random.Next(10, 30),
                        YTDReturn = random.Next(0, 30),
                        MarketCap = 100000000,
                        PE_FWD = 0.5,
                        PE_Fair = 0.7,
                        PBE_Fair = 0.23,
                        PBE_FWD = 0.456,
                        EVEBITDA_FWD = 2344786,
                        EVEBITDA_Fair = 2277648,
                        SalesGrowthCurrentYear = 12.34,
                        SalesGrowthNextYear = 23.56,
                        NetIncomeGrowthCurrentYear = 17.897,
                        NetIncomeGrowthNextYear = 19.56,
                        NetDebtEquityCurrentYear = 21.876,
                        FreeFlowCashMarginCurrentYear = -18.987
                    });
                }

                for (int i = 0; i < 5; i++)
                {
                    result.Add(new PortfolioDetailsData()
                    {
                        EntityTicker = "PBR/A US",
                        EntityName = "PETROBRAS - PETROLEO BRASs",
                        Type = "Security",
                        Country = "USA",
                        Shares = random.Next(20000, 50000),
                        Price = random.Next(200, 700),
                        Currency = "USD",
                        Value = 1,
                        TargetPerc = random.Next(10, 30),
                        PortfolioPerc = random.Next(10, 30),
                        BenchmarkPerc = random.Next(10, 30),
                        BetPerc = random.Next(0, 20),
                        Upside = random.Next(10, 30),
                        YTDReturn = random.Next(0, 30),
                        MarketCap = 1000000000,
                        PE_FWD = 0.5,
                        PE_Fair = 0.7,
                        PBE_Fair = 0.23,
                        PBE_FWD = 0.456,
                        EVEBITDA_FWD = 2344786,
                        EVEBITDA_Fair = 2277648,
                        SalesGrowthCurrentYear = 12.34,
                        SalesGrowthNextYear = 23.56,
                        NetIncomeGrowthCurrentYear = 17.897,
                        NetIncomeGrowthNextYear = 19.56,
                        NetDebtEquityCurrentYear = 21.876,
                        FreeFlowCashMarginCurrentYear = -18.987
                    });
                }
                for (int i = 0; i < 5; i++)
                {
                    result.Add(new PortfolioDetailsData()
                    {
                        EntityTicker = "MSCI US",
                        EntityName = "Morgon Stanley Common Index USA",
                        Type = "Index",
                        Country = "USA",
                        Shares = random.Next(20000, 50000),
                        Price = random.Next(200, 700),
                        Currency = "USD",
                        Value = 1,
                        TargetPerc = random.Next(10, 30),
                        PortfolioPerc = random.Next(10, 30),
                        BenchmarkPerc = random.Next(10, 30),
                        BetPerc = random.Next(0, 20),
                        Upside = random.Next(10, 30),
                        YTDReturn = random.Next(0, 30),
                        MarketCap = 500000000,
                        PE_FWD = 0.45,
                        PE_Fair = 0.17,
                        PBE_Fair = 0.83,
                        PBE_FWD = 0.856,
                        EVEBITDA_FWD = 4344786,
                        EVEBITDA_Fair = 8277648,
                        SalesGrowthCurrentYear = 22.34,
                        SalesGrowthNextYear = 17.56,
                        NetIncomeGrowthCurrentYear = 9.897,
                        NetIncomeGrowthNextYear = 2.56,
                        NetDebtEquityCurrentYear = 8.876,
                        FreeFlowCashMarginCurrentYear = -9.987
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

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

        #region Relative Performance
        [OperationContract]
        public List<RelativePerformanceSectorData> RetrieveRelativePerformanceSectorData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
            List<RelativePerformanceSectorData> result = new List<RelativePerformanceSectorData>();
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add(new RelativePerformanceSectorData()
                {
                    SectorID = row.Field<int>("GICS_SECTOR"),
                    SectorName = row.Field<string>("GICS_SECTOR_NAME")
                });
            }
            result = result.Distinct().ToList();
            return result;
        }

        /// <summary>
        /// Retrieves Security Level Relative Performance Data for a particular composite/fund, benchmark and efective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">FundSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <param name="order">(optional)1 for Ascending - data ordering - By default descending</param>
        /// <param name="maxRecords">(optional) Maximum number of records to be retrieved - By default Null</param>
        /// <returns></returns>
        [OperationContract]
        public List<RelativePerformanceSecurityData> RetrieveRelativePerformanceSecurityData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null)
        {
            

            DataTable dataTable = new DataTable();
            string query = "Select " + (maxRecords == null ? "*" : "Top " + maxRecords.ToString() + " *") + " From tblHoldingsData ";
            string queryWhereCondition = "Where ";

            if (countryID == null && sectorID == null)
            {
                queryWhereCondition = String.Empty;
            }

            else if (countryID == null && sectorID != null)
            {
                queryWhereCondition = queryWhereCondition + "GICS_SECTOR = " + sectorID.ToString();
            }

            else if (sectorID == null && countryID != null)
            {
                queryWhereCondition = queryWhereCondition + "ISO_COUNTRY_CODE = '" + countryID + "'";
            }

            else if (sectorID != null && countryID != null)
            {
                queryWhereCondition = queryWhereCondition + "ISO_COUNTRY_CODE = '" + countryID + "' And GICS_SECTOR = " + sectorID.ToString();
            }

            query = query + queryWhereCondition + " Order By DIRTY_VALUE_PC " + (order == 1 ? "Asc" : "Desc");


            dataTable = GetDataTable(query);

            int alpha = 2;
            List<RelativePerformanceSecurityData> result = new List<RelativePerformanceSecurityData>();
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add(new RelativePerformanceSecurityData()
                {
                    SecurityName = row.Field<string>("ISSUE_NAME"),
                    SecurityCountryID = row.Field<string>("ISO_COUNTRY_CODE"),
                    SecuritySectorName = row.Field<string>("GICS_SECTOR_NAME"),
                    SecurityAlpha = alpha++,
                    SecurityActivePosition = (double)
                    ( row.Field<Single?>("PORTFOLIO_WEIGHT") == null ? 0 : row.Field<Single?>("PORTFOLIO_WEIGHT") * 100
                    - row.Field<Single?>("BENCHMARK_WEIGHT") == null ? 0 : row.Field<Single?>("BENCHMARK_WEIGHT") * 100)
                });
            }
            return order == 1 ? result.OrderBy(e => e.SecurityAlpha).ToList() : result.OrderByDescending(e => e.SecurityAlpha).ToList();
        }

        [OperationContract]
        public List<RelativePerformanceData> RetrieveRelativePerformanceData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
            List<string> countryCodes = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                countryCodes.Add(row.Field<string>("ISO_COUNTRY_CODE"));
            }
            countryCodes = countryCodes.Distinct().ToList();

            dataTable = GetDataTable("Select * from tblHoldingsData");
            List<RelativePerformanceSectorData> sectors = new List<RelativePerformanceSectorData>();
            foreach (DataRow row in dataTable.Rows)
            {
                sectors.Add(new RelativePerformanceSectorData()
                {
                    SectorID = row.Field<int>("GICS_SECTOR"),
                    SectorName = row.Field<string>("GICS_SECTOR_NAME")
                });
            }
            sectors = sectors.Distinct().ToList();


            List<RelativePerformanceData> result = new List<RelativePerformanceData>();
            foreach (string countryCode in countryCodes)
            {
                double? aggcsActivePosition = 0.0;
                double? aggcsAlpha = 0.0;
                double? aggcsPortfolioShare = 0.0;
                double? aggcsBenchmarkShare = 0.0;
                List<RelativePerformanceCountrySpecificData> sectorSpecificData = new List<RelativePerformanceCountrySpecificData>();
                foreach (RelativePerformanceSectorData sectorData in sectors)
                {
                    double? aggActivePosition = 0.0;
                    double? aggAlpha = 0.0;
                    double? aggPortfolioShare = 0.0;
                    double? aggBenchmarkShare = 0.0;
                    DataTable specificData = GetDataTable("Select * from tblHoldingsData where ISO_COUNTRY_CODE = '" + countryCode + "' and GICS_SECTOR = " + sectorData.SectorID.ToString());
                    

                    foreach (DataRow row in specificData.Rows)
                    {
                        if (row.Field<Single?>("BENCHMARK_WEIGHT") != null)
                        {
                            aggPortfolioShare = aggPortfolioShare + (double)row.Field<Single>("PORTFOLIO_WEIGHT") * 100;
                            aggBenchmarkShare = aggBenchmarkShare + (double)row.Field<Single>("BENCHMARK_WEIGHT") * 100;
                            aggActivePosition = aggPortfolioShare - aggBenchmarkShare;
                            aggAlpha = aggAlpha + 2;
                        }
                    }

                    if (aggPortfolioShare > 0 || aggBenchmarkShare > 0)
                    {
                        sectorSpecificData.Add(new RelativePerformanceCountrySpecificData()
                                    {
                                        SectorID = sectorData.SectorID,
                                        SectorName = sectorData.SectorName,
                                        Alpha = aggAlpha,
                                        PortfolioShare = aggPortfolioShare,
                                        BenchmarkShare = aggBenchmarkShare,
                                        ActivePosition = aggActivePosition,
                                    });
                    }
                    else
                    {
                        sectorSpecificData.Add(new RelativePerformanceCountrySpecificData()
                        {
                            SectorID = sectorData.SectorID,
                            SectorName = sectorData.SectorName,
                            Alpha = null,
                            PortfolioShare = null,
                            BenchmarkShare = null,
                            ActivePosition = null,
                        });
                    }

                    aggcsAlpha = aggcsAlpha + aggAlpha;
                    aggcsPortfolioShare = aggcsPortfolioShare + aggPortfolioShare;
                    aggcsBenchmarkShare = aggcsBenchmarkShare + aggBenchmarkShare;
                    aggcsBenchmarkShare = aggcsActivePosition + aggActivePosition;
                }

                if (sectorSpecificData.Count > 0)
                {
                    result.Add(new RelativePerformanceData()
                    {
                        CountryID = countryCode,
                        RelativePerformanceCountrySpecificInfo = sectorSpecificData,
                        AggregateCountryAlpha = aggcsAlpha,
                        AggregateCountryPortfolioShare = aggcsPortfolioShare,
                        AggregateCountryBenchmarkShare = aggcsBenchmarkShare,
                        AggregateCountryActivePosition = aggcsBenchmarkShare,
                    });
                }


            }

            return result;
        }
        #endregion

    }
}
     