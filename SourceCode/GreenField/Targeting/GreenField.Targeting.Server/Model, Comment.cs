using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class CommentModel
    {
        public CommentModel()
        {
        }

        public CommentModel(String comment) : this()
        {
            this.Comment = comment;
        }

        [DataMember]
        public String Comment { get; set; }
    }
}
