using System;
using System.Diagnostics;

namespace Aims.Core
{
	public class BroadGlobalActivePortfolio : IPortfolio
	{
		[DebuggerStepThrough]
		public BroadGlobalActivePortfolio(String id, String name)
		{
			this.Id = id;
			this.Name = name;
		}

		public String Id { get; private set; }
		public String Name { get; private set;}
		public override Boolean Equals(Object obj)
		{
			var portfolio = obj as BroadGlobalActivePortfolio;
			if (portfolio == null) return false;
			return portfolio.Id == this.Id;
		}
		public override Int32 GetHashCode()
		{
			return this.Id.GetHashCode();
		}
	}
}
