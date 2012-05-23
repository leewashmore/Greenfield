using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;
using System.ComponentModel;

namespace GreenField.DataContracts
{
    [DataContract(Name = "DataFrequency")]
    public enum DataFrequency
    {
        [EnumMember]
        Daily = 0,

        [EnumMember]
        Weekly = 1,

        [EnumMember]
        Monthly = 2,

        [EnumMember]
        Quarterly = 3,

        [EnumMember]
        SemiYearly = 4,

        [EnumMember]
        Yearly = 5
    }

    [DataContract(Name = "PerformanceType")]
    public enum PerformanceGrade
    {
        [EnumMember]
        NO_RELATION = 0,

        [EnumMember]
        UNDER_PERFORMING = 1,

        [EnumMember]
        FLAT_PERFORMING = 2,

        [EnumMember]
        OVER_PERFORMING = 3
    };
}
