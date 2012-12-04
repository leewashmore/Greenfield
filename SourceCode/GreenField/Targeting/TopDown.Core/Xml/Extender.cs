using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.Xml
{
	public static class Extender
	{
		public static String AsNotEmpty(this String value, String valueName)
		{
			if (value == null)
			{
				throw new ApplicationException("Value of \"" + valueName + "\" is NULL.");
			}
			else if (String.Empty.Equals(value))
			{
				throw new ApplicationException("Value of \"" + valueName + "\" is empty.");
			}
			else if (String.IsNullOrWhiteSpace(value))
			{
				throw new ApplicationException("Value of \"" + valueName + "\" is whitespace.");
			}
			return value;
		}

		public static Double AsDouble(this String value)
		{
			if (String.IsNullOrWhiteSpace(value)) throw new ApplicationException();
			Double result;
			if (Double.TryParse(value, out result)) return result;
			throw new ApplicationException();
		}

		public static IElement TryMultiLockOn(this IElement walker, IEnumerable<String> names)
		{
			var name = names.GetEnumerator();
			if (!name.MoveNext()) throw new ApplicationException("There are no names.");

			var element = walker.TryLockOn(name.Current);
			while (true)
			{
				if (element != null) return element;
				if (!name.MoveNext()) return null;
				element = walker.TryLockOn(name.Current);
			}
		}
        public static IElement ReleaseAndTryMultiLockOnNext(this IElement element, String name, IEnumerable<String> names)
		{
			element.Release(name);
			return element.TryMultiLockOn(names);
		}
		public static IElement ReleaseAndMultiLockOnNext(this IElement element, String name, IEnumerable<String> names)
		{
			element.Release(name);
			return element.MultiLockOn(names);
		}

		public static IElement MultiLockOn(this IElement walker, IEnumerable<String> names)
		{
			var name = names.ToList().GetEnumerator();
			if (!name.MoveNext()) throw new ApplicationException("There are no names.");

			String considerInstead;
			var element = walker.TryLockOn(name.Current, out considerInstead);
			while (true)
			{
				if (element != null) return element;
				if (!name.MoveNext()) break;
				element = walker.TryLockOn(name.Current, out considerInstead);
			}

			if (element == null)
			{
				throw new ApplicationException("None of the elements found: \"" + String.Join("\", \"", names) + "\". However there is \"" + considerInstead + "\" which you may want to consider.");
			}
			return element;
		}

		public static IElement ReleaseAndTryLockOnNext(this IElement element, String name)
		{
			element.Release(name);
			return element.TryLockOn(name);
		}

		public static IElement ReleaseAndLockOnNext(this IElement element, String name)
		{
			element.Release(name);
			return element.LockOn(name);
		}
	}
}
