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
			var securities = facade.GetTargetingTypePortfolioPickerModel();
		}

		public void GetTargetingType()
		{
			var facade = Helper.CreateFacade();
			var targetings = facade.GetTargetingTypePortfolioPickerModel();
		}
	}
}
