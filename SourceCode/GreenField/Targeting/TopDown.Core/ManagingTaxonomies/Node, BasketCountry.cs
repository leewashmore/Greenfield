using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBaskets;
using System.Diagnostics;

namespace TopDown.Core.ManagingTaxonomies
{
	public class BasketCountryNode : IRegionNodeResident, INode
	{
		[DebuggerStepThrough]
		public BasketCountryNode(CountryBasket basket)
		{
			this.Basket = basket;
		}

		public CountryBasket Basket { get; private set; }

		[DebuggerStepThrough]
		public void Accept(IRegionNodeResidentResolver resolver)
		{
			resolver.Resolve(this);
		}

		[DebuggerStepThrough]
		public void Accept(INodeResolver resolver)
		{
			resolver.Resolve(this);
		}
	}
}
