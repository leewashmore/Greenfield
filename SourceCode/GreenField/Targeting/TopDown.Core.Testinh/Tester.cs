using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;
using System.Diagnostics;
using System.IO;
using TopDown.Core.ManagingCalculations;
using TopDown.Core.Persisting;
using Aims.Core.Sql;

namespace TopDown.Core.Testing
{
    public class Tester
    {
        public const String ConnectionString = "Data Source=lonweb1t.ashmore.local;Initial Catalog=AIMS_Data_QA;Persist Security Info=True;User ID=WPSuperUser;Password=Password1;MultipleActiveResultSets=True";
        public void Test()
        {
            var facade = Helper.CreateFacade(ConnectionString);
            var model = facade.GetBptModel(0, "APG60", "mrusaev");
            model.Factors.Items.First().OverlayFactor.EditedValue = null;
            var ticket = new CalculationTicket();
            facade.RecalculateBptModel(model, ticket);

            var traverser = new ManagingBpt.GlobeTraverser();
            var lines = traverser.TraverseGlobe(model.Globe);
            var china = lines.OfType<ManagingBpt.BasketRegionModel>().Where(x => x.Countries.Any(y => y.Country.IsoCode == "CN")).FirstOrDefault();
            var other = lines.OfType<ManagingBpt.OtherModel>().FirstOrDefault();
            
            
            
            
            var otherValue = other.Overlay.Value(ticket);
            var portfolioScaled = china.PortfolioScaled.Value(ticket);
            Trace.WriteLine(portfolioScaled);
        }

        public void GetTargetingType()
        {
            var facade = Helper.CreateFacade(ConnectionString);
            var targetings = facade.GetTargetingTypePortfolioPickerModel();
        }

        public void TestFileOutput()
        {
            var facade = Helper.CreateFacade(ConnectionString);
            IDataManagerFactory dataManagerFactory = new FakeDataManagerFactory();
            var connectionFactory = new SqlConnectionFactory(ConnectionString);
            var dataManager = dataManagerFactory.CreateDataManager(connectionFactory.CreateConnection(), null);
            var securityRepository = facade.RepositoryManager.ClaimSecurityRepository(dataManager);

            var fileManager = new TradingTargetsFileManager();
            var targetings = fileManager.GetFileContent(securityRepository, dataManager);
            
            using (StreamWriter sw = new StreamWriter(String.Format(@"C:\temp\AshmoreEMM_Models - as of {0:yyyyMMdd}-{0:HHmmss}.CSV", DateTime.Now)))
            {
                sw.Write(targetings);
            }
        }
    }
}
