using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace GreenField.Targeting.Server
{
	[DataContract]
	public class NullableExpressionModel
	{
		[DebuggerStepThrough]
		public NullableExpressionModel()
		{
			this.Issues = new List<IssueModel>();
		}

		[DebuggerStepThrough]
		public NullableExpressionModel(Decimal? value, IEnumerable<IssueModel> issues)
			: this()
		{
			this.Value = value;
			this.Issues.AddRange(issues);
		}

		[DataMember]
		public Decimal? Value { get; set; }

		[DataMember]
		public List<IssueModel> Issues { get; set; }
	}
}
