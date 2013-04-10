using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using AIMS.Composites.DAL;

namespace AIMS.Composites.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICompositesOperations" in both code and config file together.
    [ServiceContract]
    public interface ICompositesOperations
    {
        [OperationContract]
        List<GetComposites_Result> GetComposites();
        List<GetComposites_Result> GetCompositePortfolios();
    }
}
