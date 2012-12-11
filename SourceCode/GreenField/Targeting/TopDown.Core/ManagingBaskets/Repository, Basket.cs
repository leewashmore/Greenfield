using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using TopDown.Core.ManagingCountries;
using Aims.Core;

namespace TopDown.Core.ManagingBaskets
{
	public class BasketRepository : KeyedRepository<Int32, IBasket>
	{
		[DebuggerStepThrough]
		internal protected BasketRepository()
		{
            /// need this constructor for testing
		}

		public BasketRepository(IEnumerable<IBasket> baskets)
		{
            baskets.ForEach(x => this.RegisterValue(x, x.Id));
		}


		public IBasket GetBasket(Int32 basketId)
		{
			var found = base.FindValue(basketId);
			if (found == null) throw new ApplicationException("There is no basket with the \"" + basketId + "\" ID.");
			return found;
		}
	}
}
