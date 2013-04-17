using System.Collections.Generic;
using System.ServiceModel;
using AIMS.Composites.DAL;

namespace AIMS.Composites.Service
{
    [ServiceContract]
    public interface ICompositesOperations
    {
        [OperationContract]
        List<GetComposites_Result> GetComposites();

        List<GetCompositePortfolios_Result> GetCompositePortfolios(string compositeId);
        void PopulateCompositeLTHoldings();
    }
}