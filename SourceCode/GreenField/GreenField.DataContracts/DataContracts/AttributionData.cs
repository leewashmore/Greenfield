using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class AttributionData
    {
       [DataMember]
        public String COUNTRY { get; set; }

       [DataMember]
       public String COUNTRY_NAME { get; set; }

       [DataMember]
       public Decimal? POR_RC_AVG_WGT_1M { get; set; }

       [DataMember]
       public Decimal? BM1_RC_AVG_WGT_1M { get; set; }

       [DataMember]
       public Decimal? F_POR_ASH_RC_CTN_1M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_RC_CTN_1M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_ASSET_ALLOC_1M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_SEC_SELEC_1M { get; set; }

       [DataMember]
       public Decimal? POR_RC_AVG_WGT_3M { get; set; }

       [DataMember]
       public Decimal? BM1_RC_AVG_WGT_3M { get; set; }

       [DataMember]
       public Decimal? F_POR_ASH_RC_CTN_3M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_RC_CTN_3M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_ASSET_ALLOC_3M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_SEC_SELEC_3M { get; set; }

       [DataMember]
       public Decimal? POR_RC_AVG_WGT_6M { get; set; }

       [DataMember]
       public Decimal? BM1_RC_AVG_WGT_6M { get; set; }

       [DataMember]
       public Decimal? F_POR_ASH_RC_CTN_6M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_RC_CTN_6M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_ASSET_ALLOC_6M { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_SEC_SELEC_6M { get; set; }

       [DataMember]
       public Decimal? POR_RC_AVG_WGT_YTD { get; set; }

       [DataMember]
       public Decimal? BM1_RC_AVG_WGT_YTD { get; set; }

       [DataMember]
       public Decimal? F_POR_ASH_RC_CTN_YTD { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_RC_CTN_YTD { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_ASSET_ALLOC_YTD { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_SEC_SELEC_YTD { get; set; }

       [DataMember]
       public Decimal? POR_RC_AVG_WGT_1Y { get; set; }

       [DataMember]
       public Decimal? BM1_RC_AVG_WGT_1Y { get; set; }

       [DataMember]
       public Decimal? F_POR_ASH_RC_CTN_1Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_RC_CTN_1Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_ASSET_ALLOC_1Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_SEC_SELEC_1Y { get; set; }

       [DataMember]
       public Decimal? POR_RC_AVG_WGT_3Y { get; set; }

       [DataMember]
       public Decimal? BM1_RC_AVG_WGT_3Y { get; set; }

       [DataMember]
       public Decimal? F_POR_ASH_RC_CTN_3Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_RC_CTN_3Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_ASSET_ALLOC_3Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_SEC_SELEC_3Y { get; set; }

       [DataMember]
       public Decimal? POR_RC_AVG_WGT_5Y { get; set; }

       [DataMember]
       public Decimal? BM1_RC_AVG_WGT_5Y { get; set; }

       [DataMember]
       public Decimal? F_POR_ASH_RC_CTN_5Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_RC_CTN_5Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_ASSET_ALLOC_5Y { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_SEC_SELEC_5Y { get; set; }

       [DataMember]
       public Decimal? POR_RC_AVG_WGT_SI { get; set; }

       [DataMember]
       public Decimal? BM1_RC_AVG_WGT_SI { get; set; }

       [DataMember]
       public Decimal? F_POR_ASH_RC_CTN_SI { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_RC_CTN_SI { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_ASSET_ALLOC_SI { get; set; }

       [DataMember]
       public Decimal? F_BM1_ASH_SEC_SELEC_SI { get; set; }
      
    }
}