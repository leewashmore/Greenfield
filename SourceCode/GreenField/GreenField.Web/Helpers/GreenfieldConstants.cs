using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

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
        public const string CASH = "Cash";
        public const string REGION = "Region";
        public const string SECTOR = "Sector";
        public const string COUNTRY = "Country";
        public const string INDUSTRY = "Industry";


        // Exception Message
        public const string NETWORK_FAULT_ECX_MSG = "NetworkFault";
        public const string SERVICE_NULL_ARG_EXC_MSG = "ServiceNullArgumentException";


        #endregion
    }
}