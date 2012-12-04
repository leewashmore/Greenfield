using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GreenField.Targeting.Server
{
    [DataContract]
    [KnownType(typeof(RegionBasketModel))]
    [KnownType(typeof(CountryBasketModel))]
    public class BasketModel
    {
        [DataMember]
        public Int32 Id { get; set; }
    }
}
