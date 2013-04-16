using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AIMS.Composites.Service
{
    internal class CompositeLTHoldingsManager : PusherBase<GF_COMPOSITE_LTHOLDINGS>
    {
        private readonly String connectionString;
        private readonly IDumper dumper;
        private readonly Int32 recordsPerBulk;

        public CompositeLTHoldingsManager(String connectionString, Int32 recordsPerBulk, IDumper dumper)
        {
            this.recordsPerBulk = recordsPerBulk;
            this.dumper = dumper;
            this.connectionString = connectionString;
        }

        public void Save(IEnumerable<GF_COMPOSITE_LTHOLDINGS> gfCompositeLtholdingss)
        {
            dumper.WriteLine("There are " + gfCompositeLtholdingss.Count() + " records to push.");

            IEnumerable<DataTable> tables = SplitIntoBulks(gfCompositeLtholdingss, recordsPerBulk);
            dumper.WriteLine("There are " + tables.Count() + " bulks, " + recordsPerBulk + " records each.");

            int bulkNumber = 1;
            foreach (DataTable table in tables)
            {
                PushOneBulk(bulkNumber, table, "dbo.GF_COMPOSITE_LTHOLDINGS");
                dumper.WriteLine("Bulk " + bulkNumber + " has been pushed to database.");
                bulkNumber++;
            }
            dumper.WriteLine(gfCompositeLtholdingss.Count() + " composite holdings have been pushed.");
        }

        protected override DataTable CreateBulk(IEnumerable<GF_COMPOSITE_LTHOLDINGS> GF_COMPOSITE_LTHOLDINGS)
        {
            var table = new DataTable("GF_COMPOSITE_LTHOLDINGS");

            var GF_ID = new DataColumn("GF_ID", typeof (Decimal));
            table.Columns.Add(GF_ID);

            var PORTFOLIO_DATE = new DataColumn("PORTFOLIO_DATE", typeof (DateTime)) {AllowDBNull = true};
            table.Columns.Add(PORTFOLIO_DATE);

            var PORTFOLIO_ID = new DataColumn("PORTFOLIO_ID", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(PORTFOLIO_ID);

            var A_PFCHOLDINGS_PORLT = new DataColumn("A_PFCHOLDINGS_PORLT", typeof (String));
            PORTFOLIO_ID.AllowDBNull = true;
            table.Columns.Add(A_PFCHOLDINGS_PORLT);

            var PORPATH = new DataColumn("PORPATH", typeof (String));
            PORTFOLIO_ID.AllowDBNull = true;
            table.Columns.Add(PORPATH);

            var PORTFOLIO_THEME_SUBGROUP_CODE = new DataColumn("PORTFOLIO_THEME_SUBGROUP_CODE", typeof (String))
                {
                    AllowDBNull = true
                };
            table.Columns.Add(PORTFOLIO_THEME_SUBGROUP_CODE);

            var PORTFOLIO_CURRENCY = new DataColumn("PORTFOLIO_CURRENCY", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(PORTFOLIO_CURRENCY);

            var BENCHMARK_ID = new DataColumn("BENCHMARK_ID", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(BENCHMARK_ID);

            var ISSUER_ID = new DataColumn("ISSUER_ID", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(ISSUER_ID);

            var ASEC_SEC_SHORT_NAME = new DataColumn("ASEC_SEC_SHORT_NAME", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(ASEC_SEC_SHORT_NAME);

            var ISSUE_NAME = new DataColumn("ISSUE_NAME", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(ISSUE_NAME);

            var TICKER = new DataColumn("TICKER", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(TICKER);

            var SECURITYTHEMECODE = new DataColumn("SECURITYTHEMECODE", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(SECURITYTHEMECODE);

            var A_SEC_INSTR_TYPE = new DataColumn("A_SEC_INSTR_TYPE", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(A_SEC_INSTR_TYPE);

            var SECURITY_TYPE = new DataColumn("SECURITY_TYPE", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(SECURITY_TYPE);

            var BALANCE_NOMINAL = new DataColumn("BALANCE_NOMINAL", typeof (Decimal)) {AllowDBNull = true};
            table.Columns.Add(BALANCE_NOMINAL);

            var DIRTY_PRICE = new DataColumn("DIRTY_PRICE", typeof (Decimal)) {AllowDBNull = true};
            table.Columns.Add(DIRTY_PRICE);

            var TRADING_CURRENCY = new DataColumn("TRADING_CURRENCY", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(TRADING_CURRENCY);

            var DIRTY_VALUE_PC = new DataColumn("DIRTY_VALUE_PC", typeof (Decimal)) {AllowDBNull = true};
            table.Columns.Add(DIRTY_VALUE_PC);

            var BENCHMARK_WEIGHT = new DataColumn("BENCHMARK_WEIGHT", typeof (Decimal)) {AllowDBNull = true};
            table.Columns.Add(BENCHMARK_WEIGHT);

            var ASH_EMM_MODEL_WEIGHT = new DataColumn("ASH_EMM_MODEL_WEIGHT", typeof (Decimal)) {AllowDBNull = true};
            table.Columns.Add(ASH_EMM_MODEL_WEIGHT);

            var MARKET_CAP_IN_USD = new DataColumn("MARKET_CAP_IN_USD", typeof (Decimal)) {AllowDBNull = true};
            table.Columns.Add(MARKET_CAP_IN_USD);

            var ASHEMM_PROP_REGION_CODE = new DataColumn("ASHEMM_PROP_REGION_CODE", typeof (String))
                {
                    AllowDBNull = true
                };
            table.Columns.Add(ASHEMM_PROP_REGION_CODE);

            var ASHEMM_PROP_REGION_NAME = new DataColumn("ASHEMM_PROP_REGION_NAME", typeof (String))
                {
                    AllowDBNull = true
                };
            table.Columns.Add(ASHEMM_PROP_REGION_NAME);

            var ISO_COUNTRY_CODE = new DataColumn("ISO_COUNTRY_CODE", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(ISO_COUNTRY_CODE);

            var COUNTRYNAME = new DataColumn("COUNTRYNAME", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(COUNTRYNAME);

            var GICS_SECTOR = new DataColumn("GICS_SECTOR", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(GICS_SECTOR);

            var GICS_SECTOR_NAME = new DataColumn("GICS_SECTOR_NAME", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(GICS_SECTOR_NAME);

            var GICS_INDUSTRY = new DataColumn("GICS_INDUSTRY", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(GICS_INDUSTRY);

            var GICS_INDUSTRY_NAME = new DataColumn("GICS_INDUSTRY_NAME", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(GICS_INDUSTRY_NAME);

            var GICS_SUB_INDUSTRY = new DataColumn("GICS_SUB_INDUSTRY", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(GICS_SUB_INDUSTRY);

            var GICS_SUB_INDUSTRY_NAME = new DataColumn("GICS_SUB_INDUSTRY_NAME", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(GICS_SUB_INDUSTRY_NAME);

            var LOOK_THRU_FUND = new DataColumn("LOOK_THRU_FUND", typeof (String)) {AllowDBNull = true};
            table.Columns.Add(LOOK_THRU_FUND);

            foreach (GF_COMPOSITE_LTHOLDINGS ltHOLDINGS in GF_COMPOSITE_LTHOLDINGS)
            {
                DataRow row = table.NewRow();
                row[GF_ID] = ltHOLDINGS.GF_ID;
                row[PORTFOLIO_DATE] = ltHOLDINGS.PORTFOLIO_DATE;
                row[PORTFOLIO_ID] = ltHOLDINGS.PORTFOLIO_ID;
                row[PORTFOLIO_THEME_SUBGROUP_CODE] = ltHOLDINGS.PORTFOLIO_THEME_SUBGROUP_CODE;
                row[PORTFOLIO_CURRENCY] = ltHOLDINGS.PORTFOLIO_CURRENCY;
                row[BENCHMARK_ID] = ltHOLDINGS.BENCHMARK_ID;
                row[ISSUER_ID] = ltHOLDINGS.ISSUER_ID;
                row[ASEC_SEC_SHORT_NAME] = ltHOLDINGS.ASEC_SEC_SHORT_NAME;
                row[ISSUE_NAME] = ltHOLDINGS.ISSUE_NAME;
                row[TICKER] = ltHOLDINGS.TICKER;
                row[SECURITYTHEMECODE] = ltHOLDINGS.SECURITYTHEMECODE;
                row[A_SEC_INSTR_TYPE] = ltHOLDINGS.A_SEC_INSTR_TYPE;
                row[SECURITY_TYPE] = ltHOLDINGS.SECURITY_TYPE;
                row[BALANCE_NOMINAL] = TryValue(ltHOLDINGS.BALANCE_NOMINAL);
                row[DIRTY_PRICE] = TryValue(ltHOLDINGS.DIRTY_PRICE);
                row[TRADING_CURRENCY] = ltHOLDINGS.TRADING_CURRENCY;
                row[DIRTY_VALUE_PC] = TryValue(ltHOLDINGS.DIRTY_VALUE_PC);
                row[BENCHMARK_WEIGHT] = TryValue(ltHOLDINGS.BENCHMARK_WEIGHT);
                row[ASH_EMM_MODEL_WEIGHT] = TryValue(ltHOLDINGS.ASH_EMM_MODEL_WEIGHT);
                row[MARKET_CAP_IN_USD] = TryValue(ltHOLDINGS.MARKET_CAP_IN_USD);
                row[ASHEMM_PROP_REGION_CODE] = ltHOLDINGS.ASHEMM_PROP_REGION_CODE;
                row[ASHEMM_PROP_REGION_NAME] = ltHOLDINGS.ASHEMM_PROP_REGION_NAME;
                row[ISO_COUNTRY_CODE] = ltHOLDINGS.ISO_COUNTRY_CODE;
                row[COUNTRYNAME] = ltHOLDINGS.COUNTRYNAME;
                row[GICS_SECTOR] = ltHOLDINGS.GICS_SECTOR;
                row[GICS_SECTOR_NAME] = ltHOLDINGS.GICS_SECTOR_NAME;
                row[GICS_INDUSTRY] = ltHOLDINGS.GICS_INDUSTRY;
                row[GICS_INDUSTRY_NAME] = ltHOLDINGS.GICS_INDUSTRY_NAME;
                row[GICS_SUB_INDUSTRY] = ltHOLDINGS.GICS_SUB_INDUSTRY;
                row[GICS_SUB_INDUSTRY_NAME] = ltHOLDINGS.GICS_SUB_INDUSTRY_NAME;
                row[LOOK_THRU_FUND] = ltHOLDINGS.LOOK_THRU_FUND;

                table.Rows.Add(row);
            }
            return table;
        }

        private object TryValue(decimal? val)
        {
            if (val.HasValue)
                return val.Value;

            return DBNull.Value;
        }

        protected override SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}