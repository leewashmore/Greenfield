using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core
{
	public class ExpressionFromJsonDeserializer
	{
		public void PopulateEditableExpression(JsonReader reader, EditableExpression expression)
		{
            expression.InitialValue = expression.Adapter.Give(new JsonPropertyValueGiver(JsonNames.InitialValue, reader));
            expression.EditedValue = expression.Adapter.Give(new JsonPropertyValueGiver(JsonNames.EditedValue, reader));
            expression.LastOneModified = reader.ReadAsBoolean(JsonNames.LastOneModified);
            expression.Comment = reader.ReadAsNullableString(JsonNames.Comment);
		}
		public void PopulateUnchangeableExpression<TValue>(JsonReader reader, UnchangableExpression<TValue> expression)
		{
            expression.InitialValue = expression.Adapter.Give(new JsonPropertyValueGiver(JsonNames.Value, reader));
		}
	}
}
