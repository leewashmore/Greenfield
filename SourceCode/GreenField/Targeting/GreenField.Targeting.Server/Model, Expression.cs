﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
	[DataContract]
	public class ExpressionModel
	{
		[DebuggerStepThrough]
		public ExpressionModel()
		{
			this.Issues = new List<IssueModel>();
		}

		[DebuggerStepThrough]
		public ExpressionModel(Decimal value, IEnumerable<IssueModel> issues)
			: this()
		{
			this.Value = value;
			this.Issues.AddRange(issues);
		}

		[DataMember]
		public Decimal Value { get; set; }

		[DataMember]
		public List<IssueModel> Issues { get; set; }
	}
}