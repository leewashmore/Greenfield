using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Aims.Data.Server;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class CountryBasketModel : BasketModel
    {
        [DebuggerStepThrough]
        public CountryBasketModel()
        {
        }

        [DebuggerStepThrough]
        public CountryBasketModel(Int32 id, CountryModel country)
        {
            this.Id = id;
            this.Country = country;
        }

        [DataMember]
        public CountryModel Country { get; set; }
    }
}
