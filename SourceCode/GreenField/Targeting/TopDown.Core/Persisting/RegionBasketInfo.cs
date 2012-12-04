using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	public class RegionBasketInfo
	{
		[DebuggerStepThrough]
		public RegionBasketInfo()
		{
		}

		[DebuggerStepThrough]
		public RegionBasketInfo(Int32 id, String definitionXml)
		{
			this.Id = id;
			this.DefinitionXml = definitionXml;
		}

		public Int32 Id { get; set; }
		public String DefinitionXml { get; set; }
	}
}
