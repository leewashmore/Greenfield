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
    public class ItemModel
    {
        [DebuggerStepThrough]
        public ItemModel()
        {
        }

        [DebuggerStepThrough]
        public ItemModel(SecurityModel security, Boolean preferred)
            : this()
        {
            this.Security = security;
            this.Preferred = preferred;
        }

        [DataMember]
        public SecurityModel Security { get; set; }

        [DataMember]
        public Boolean Preferred { get; set; }

    }
}
