using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions.ManagingBpt
{
	public interface IExpressionPicker<TModel, TValue>
	{
		IExpression<TValue> TryPickExpression(TModel model);
	}
}
