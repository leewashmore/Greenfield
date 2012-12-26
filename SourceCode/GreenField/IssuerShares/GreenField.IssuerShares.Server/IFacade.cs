using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Aims.Data.Server;

namespace GreenField.IssuerShares.Server
{
    [ServiceContract]
    public interface IFacade
    {
        [OperationContract]
        RootModel GetRootModel(String issuerId);

        [OperationContract]
        IEnumerable<SecurityModel> GetIssuerSecurities(String pattern, Int32 atMost, String issuerId);

    }
}
