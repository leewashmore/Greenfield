using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.Testing
{
	public class Tester
	{
		public void Test()
		{
			var facade = Helper.CreateFacade();
			var securities = facade.GetSecurities("S", 20);
		}

		public void GetTargetingType()
		{
			var facade = Helper.CreateFacade();
			var targetings = facade.GetTargetingTypePortfolioPickerModel();
		}
	}
}
