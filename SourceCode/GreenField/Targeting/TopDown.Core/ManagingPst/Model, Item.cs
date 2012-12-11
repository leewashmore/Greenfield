using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingSecurities;
using System.Diagnostics;
using Aims.Expressions;
using Aims.Core;

namespace TopDown.Core.ManagingPst
{
	public class ItemModel
	{
		[DebuggerStepThrough]
		public ItemModel(
            ISecurity security,
			EditableExpression  targetExpression
		)
		{
            this.Security = security;
			this.Target = targetExpression;
		}

        public ISecurity Security { get; private set; }
		public EditableExpression Target { get; private set; }
	}
}
