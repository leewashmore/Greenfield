using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class EditableExpressionModel
    {
        [DebuggerStepThrough]
        public EditableExpressionModel()
        {
            this.Issues = new List<IssueModel>();
        }

        [DebuggerStepThrough]
        public EditableExpressionModel(
            Decimal? defaultValue,
            Decimal? initialValue,
            Decimal? editedValue,
            String comment,
            IEnumerable<IssueModel> issues,
            Boolean isLastEdited
        ) : this()
        {
            this.DefaultValue = defaultValue;
            this.InitialValue = initialValue;
            this.EditedValue = editedValue;
            this.Comment = comment;
            this.Issues.AddRange(issues);
            this.DisplayValue = null;
            this.IsLastEdited = isLastEdited;
        }

        [DataMember]
        public Decimal? DefaultValue { get; set; }

        [DataMember]
        public Decimal? InitialValue { get; set; }

        [DataMember]
        public Decimal? EditedValue { get; set; }

        [DataMember]
        public List<IssueModel> Issues { get; set; }

        [DataMember]
        public String Comment { get; set; }

        [DataMember]
        public Decimal? DisplayValue { get; set; }

        [DataMember]
        public Boolean IsLastEdited { get; set; }
    }
}
