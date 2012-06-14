using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace GreenField.Web.Helpers
{
    public class GreenfieldConstants
    {
        //Market Capitalization Constants
        # region Market Capitalization

        public const string LARGE_RANGE = "LargeRange";
        public const string MEDIUM_RANGE = "MediumRange";
        public const string SMALL_RANGE = "SmallRange";
        public const string MICRO_RANGE = "MicroRange";
        public const string UNDEFINED_RANGE = "UndefinedRange";
        public const string MARKET_CAPITALIZATION = "MarketCapitalization";
        public const string NULL_VAL = "Null";
        public const string BLANK_VAL = "Blank";
        public const string DECIMAL_DEF_VAL = "0";
        public const string INVALID_DATA_ERR_MSG = "Invalid Data";
        public const string CASH = "CASH";
        public const string REGION = "Region";
        public const string SECTOR = "Sector";
        public const string COUNTRY = "Country";
        public const string INDUSTRY = "Industry";
        public const string SHOW_EVERYTHING = "Show Everything";


        // Exception Message
        public const string NETWORK_FAULT_ECX_MSG = "NetworkFault";
        public const string SERVICE_NULL_ARG_EXC_MSG = "ServiceNullArgumentException";        

        #endregion 

        #region COMMODITY CONSTANTS
        public const string COMMODITY_ALL = "ALL";
        #endregion

    }

    public class Columns
    {
        public const string COUNTRY_NAME = "COUNTRY_NAME";
        public const string CATEGORY_NAME = "CATEGORY_NAME";
        public const string DISPLAY_TYPE = "DISPLAY_TYPE";
        public const string DESCRIPTION = "DESCRIPTION";
        //public const string Region = "";
    }
    public class Procs
    {
        public const string RETRIEVE_EM_SUMMARY_DATA_REPORT_PER_COUNTRY = "RetrieveEMSummaryDataReportPerCountry";
    }
    public class Params
    {
        public const string COUNTRYNAME = "@country";
    }

    public class Methods
    {
         #region Connection String Methods

        public static string GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"ND1DDYYB6Q1\SQLEXPRESS";
            builder.InitialCatalog = "AshmoreEMMPOC";
            builder.UserID = "sa";
            builder.Password = "India@123";
            builder.MultipleActiveResultSets = true;
            return builder.ConnectionString;
        }
        public static DataTable GetDataTable(string procName, string paramName, string paramVal)//, string paramRegionVal)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(procName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(paramName, paramVal);
                //command.Parameters.Add("@Region", paramRegionVal);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
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