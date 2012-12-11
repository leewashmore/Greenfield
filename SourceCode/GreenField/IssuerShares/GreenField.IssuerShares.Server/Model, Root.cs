using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Aims.Data.Server;

namespace GreenField.IssuerShares.Server
{
    [DataContract]
    public class RootModel
    {
        public RootModel()
        {
            this.Items = new List<ItemModel>();
        }

        public RootModel(Issuer issuer, IEnumerable<ItemModel> items)
            : this()
        {
            this.Issuer = issuer;
            this.Items.AddRange(items);
        }

        [DataMember]
        public Issuer Issuer { get; set; }

        [DataMember]
        public List<ItemModel> Items { get; set; }
    }
}
