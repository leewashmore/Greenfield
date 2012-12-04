using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace TopDown.Core.Xml
{
	public interface IValueAccessor
	{
		String ReadAttributeAsString(String name, String defaultValue);
		String ReadAttributeAsNotEmptyString(String name);
		Double ReadAttributeAsDouble(String name);
		Boolean ReadAttributeAsBoolean(String name, Boolean defaultValue);
        Int32 ReadAttributeAsInt32(String name);
		String ReadContentAsNotEmptyString();
		Double ReadContentAsDouble();
	}
}
