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
    public class RegionBasketModel : BasketModel
    {
        [DebuggerStepThrough]
        public RegionBasketModel()
        {
            this.Countries = new List<CountryModel>();
        }

        [DebuggerStepThrough]
        public RegionBasketModel(Int32 id, String name, IEnumerable<CountryModel> countries)
            : this()
        {
            this.Id = id;
            this.Name = name;
            this.Countries.AddRange(countries);
        }


        [DataMember]
        public List<CountryModel> Countries { get; set; }

        [DataMember]
        public String Name { get; set; }
    }
}
