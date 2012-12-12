using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Aims.Data.Server;
using System.Diagnostics;

namespace GreenField.IssuerShares.Server
{
    [DataContract]
    public class RootModel
    {
        [DebuggerStepThrough]
        public RootModel()
        {
            this.Items = new List<ItemModel>();
        }

        [DebuggerStepThrough]
        public RootModel(IssuerModel issuer, IEnumerable<ItemModel> items)
            : this()
        {
            this.Issuer = issuer;
            this.Items.AddRange(items);
        }

        [DataMember]
        public IssuerModel Issuer { get; set; }

        [DataMember]
        public List<ItemModel> Items { get; set; }
    }
}
