using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Core
{
	public interface IMonitor
	{
		TValue DefaultIfFails<TValue>(String message, Func<TValue> handler);
        void SwallowIfFails(String message, Action handler);
	}
}
