using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;
using System.Diagnostics;

namespace TopDown.Core.Testing
{
    public class Tester
    {
        public const String ConnectionString = "Data Source=lonweb1t.ashmore.local;Initial Catalog=AIMS_Data_QA;Persist Security Info=True;User ID=WPSuperUser;Password=Password1;MultipleActiveResultSets=True";
        public void Test()
        {
            var facade = Helper.CreateFacade(ConnectionString);
            var model = facade.GetBptModel(0, "APG60");
            var traverser = new ManagingBpt.GlobeTraverser();
            var lines = traverser.TraverseGlobe(model.Globe);
            var china = lines.OfType<ManagingBpt.BasketRegionModel>().Where(x => x.Countries.Any(y => y.Country.IsoCode == "CN")).FirstOrDefault();
            var other = lines.OfType<ManagingBpt.OtherModel>().FirstOrDefault();
            
            
            
            var ticket = new CalculationTicket();
            var otherValue = other.Overlay.Value(ticket);
            var portfolioScaled = china.PortfolioScaled.Value(ticket);
            Trace.WriteLine(portfolioScaled);
        }

        public void GetTargetingType()
        {
            var facade = Helper.CreateFacade(ConnectionString);
            var targetings = facade.GetTargetingTypePortfolioPickerModel();
        }
    }
}
