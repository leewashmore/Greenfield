using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Aims.Data.Server
{
    [DataContract]
    public class Issuer
    {
        public Issuer()
        {
        }

        public Issuer(String id, String name)
            : this()
        {
            this.Id = id;
            this.Name = name;
        }

        [DataMember]
        public String Id { get; set; }

        [DataMember]
        public String Name { get; set; }
    }
}
