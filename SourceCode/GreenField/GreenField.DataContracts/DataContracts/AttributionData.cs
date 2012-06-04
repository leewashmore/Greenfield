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
       public Decimal? FBm1AshRcCtn1d { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc1d { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec1d { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgt1w { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgt1w { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtn1w { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtn1w { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAlloc1w { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelec1w { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgtMtd { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgtMtd { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtnMtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtnMtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAllocMtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelecMtd { get; set; }

       [DataMember]
       public Decimal? PorRcAvgWgtQtd { get; set; }

       [DataMember]
       public Decimal? Bm1RcAvgWgtQtd { get; set; }

       [DataMember]
       public Decimal? FPorAshRcCtnQtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshRcCtnQtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshAssetAllocQtd { get; set; }

       [DataMember]
       public Decimal? FBm1AshSecSelecQtd { get; set; }

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

       //[DataMember]
       //public Decimal? PorRcAvgWgt5y { get; set; }

       //[DataMember]
       //public Decimal? Bm1RcAvgWgt5y { get; set; }

       //[DataMember]
       //public Decimal? FPorAshRcCtn5y { get; set; }

       //[DataMember]
       //public Decimal? FBm1AshRcCtn5y { get; set; }

       //[DataMember]
       //public Decimal? FBm1AshAssetAlloc5y { get; set; }

       //[DataMember]
       //public Decimal? FBm1AshSecSelec5y { get; set; }

       //[DataMember]
       //public Decimal? PorRcAvgWgtSi { get; set; }

       //[DataMember]
       //public Decimal? Bm1RcAvgWgtSi { get; set; }

       //[DataMember]
       //public Decimal? FPorAshRcCtnSi { get; set; }

       //[DataMember]
       //public Decimal? FBm1AshRcCtnSi { get; set; }

       //[DataMember]
       //public Decimal? FBm1AshAssetAllocSi { get; set; }

       //[DataMember]
       //public Decimal? FBm1AshSecSelecSi { get; set; }
      
    }
}