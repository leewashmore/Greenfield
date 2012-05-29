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
       public Decimal? PorRcAvgWgt1m { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt1m { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn1m { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtn1m { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc1m { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec1m { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgt3m { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt3m { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn3m { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtn3m { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc3m { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec3m { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgt6m { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt6m { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn6m { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtn6m { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc6m { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec6m { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgtYtd { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgtYtd { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtnYtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtnYtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAllocYtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelecYtd { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgt1y { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt1y { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn1y { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtn1y { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc1y { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec1y { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgt3y { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt3y { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn3y { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtn3y { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc3y { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec3y { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgt5y { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt5y { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn5y { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtn5y { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc5y { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec5y { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgtSi { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgtSi { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtnSi { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtnSi { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAllocSi { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelecSi { get; set; }
      
    }
}