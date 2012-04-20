using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class AggregatedData
    {
       [DataMember]
        public String PFCD_FROMDATE { get; set; }

       [DataMember]
        public String PORTFOLIOTHEMEGROUPCODE { get; set; }

       [DataMember]
        public String PORTFOLIOCODE { get; set; }
        
       [DataMember]
        public String PORTFOLIONAME { get; set; }

       [DataMember]
       public Double PFCH_BALBOOKVALPC { get; set; }

       [DataMember]
       public Double PFKR_FXRATEQP { get; set; }

       [DataMember]
       public String COUNTRYZONENAME { get; set; }

       [DataMember]
       public String COUNTRYCODE { get; set; }

       [DataMember]
       public String COUNTRYNAME { get; set; }

       [DataMember]
       public String CURR_QUORISK_BEST { get; set; }

       [DataMember]
       public String ISIN { get; set; }

       [DataMember]
       public String SEC_SECSHORT { get; set; }

       [DataMember]
       public String SEC_SECNAME { get; set; }

       [DataMember]
       public String SEC_INSTYPE_NAME { get; set; }

       [DataMember]
       public String COMPANY_NAME { get; set; }

       [DataMember]
       public float? NET_INCOME_ACT_2009 { get; set; }

       [DataMember]
       public float? NET_INCOME_ACT_2010 { get; set; }

       [DataMember]
       public float? NET_INCOME_EST_2011 { get; set; }

       [DataMember]
       public float? NET_INCOME_EST_2012 { get; set; }

       [DataMember]
       public float? NET_INCOME_EST_2013 { get; set; }
    }
}