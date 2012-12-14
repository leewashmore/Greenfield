using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class CommentModel
    {
        public CommentModel()
        {
        }

        [DebuggerStepThrough]
        public CommentModel(String comment, Decimal? before, Decimal? after, String username, DateTime timestamp)
            : this()
        {
            this.Comment = comment;
            this.Before = before;
            this.After = after;
            this.Username = username;
            this.Timestamp = timestamp;
        }

        [DataMember]
        public Decimal? Before { get; set; }

        [DataMember]
        public Decimal? After { get; set; }

        [DataMember]
        public String Username { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public String Comment { get; set; }
    }
}
