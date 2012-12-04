using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class ChangesetModel
    {
        [DebuggerStepThrough]
        public ChangesetModel()
        {
        }
        
        [DebuggerStepThrough]
        public ChangesetModel(Int32 id, String username, DateTime timestamp, Int32 calculationId)
        {
            this.Id = id;
            this.Username = username;
            this.Timestamp = timestamp;
            this.CalculationId = calculationId;
        }

        [DataMember]
        public Int32 Id { get; set; }
        
        [DataMember]
        public String Username { get; set; }
        
        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public Int32 CalculationId { get; set; }
    }
}
