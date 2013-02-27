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
        public String Country { get; set; }

       [DataMember]
       public String CountryName { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgt1d { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt1d { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn1d { get; set; }

       [DataMember]
       public Decimal? BM1_RC_TWR_1D { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc1d { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec1d { get; set; }

       [DataMember]
       public Decimal? PorTopTwr1d { get; set; }

       [DataMember]
       public Decimal? BM1TopTwr1d { get; set; }



       [DataMember]
       public Decimal? PorRcAvgWgt1w { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt1w { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn1w { get; set; }

       [DataMember]
       public Decimal? BM1_RC_TWR_1W { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc1w { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec1w { get; set; }

       [DataMember]
       public Decimal? PorTopTwr1w { get; set; }

       [DataMember]
       public Decimal? BM1TopTwr1w { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgtMtd { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgtMtd { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtnMtd { get; set; }

       [DataMember]
       public Decimal? BM1_RC_TWR_MTD { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAllocMtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelecMtd { get; set; }

       [DataMember]
       public Decimal? PorTopTwrMtd { get; set; }

       [DataMember]
       public Decimal? BM1TopTwrMtd { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgtQtd { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgtQtd { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtnQtd { get; set; }

       [DataMember]
       public Decimal? BM1_RC_TWR_QTD { get; set; }

       
       [DataMember]
       public Decimal? FBm1AshAssetAllocQtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelecQtd { get; set; }

       [DataMember]
       public Decimal? PorTopTwrQtd { get; set; }

       [DataMember]
       public Decimal? BM1TopTwrQtd { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgtYtd { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgtYtd { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtnYtd { get; set; }

       [DataMember]
       public Decimal? BM1_RC_TWR_YTD { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAllocYtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelecYtd { get; set; }

       [DataMember]
       public Decimal? PorTopTwrYTD { get; set; }

       [DataMember]
       public Decimal? BM1TopTwrYtd { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgt1y { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt1y { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn1y { get; set; }

       [DataMember]
       public Decimal? BM1_RC_TWR_1Y { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc1y { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec1y { get; set; }

       [DataMember]
       public Decimal? PorTopTwr1y { get; set; }

       [DataMember]
       public Decimal? BM1TopTwr1y { get; set; }

       [DataMember]
       public String PorInceptionDate { get; set; }

       [DataMember]
       public DateTime? EffectiveDate { get; set; }
       
    }
}