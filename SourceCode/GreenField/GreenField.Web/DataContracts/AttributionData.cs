using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class AttributionData
    {
       [DataMember]
       public String COUNTRY_ID { get; set; }

       [DataMember]
       public String COUNTRY_NAME { get; set; }

       [DataMember]
       public Double PORTFOLIO_WEIGHT { get; set; }

       [DataMember]
       public Double BENCHMARK_WEIGHT { get; set; }

       [DataMember]
       public Double PORTFOLIO_RETURN { get; set; }

       [DataMember]
       public Double BENCHMARK_RETURN { get; set; }

       [DataMember]
       public Double ASSET_ALLOCATION { get; set; }

       [DataMember]
       public Double STOCK_SELECTION_TOTAL { get; set; }
       
    }
}