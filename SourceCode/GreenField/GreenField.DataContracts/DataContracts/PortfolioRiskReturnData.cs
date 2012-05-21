using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class PortfolioRiskReturnData
    {
        [DataMember]
        public String DataPointName;

        [DataMember]
        public Decimal? PortfolioValue;

        [DataMember]
        public Decimal? BenchMarkValue;

    }
}