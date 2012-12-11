using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace GreenField.IssuerShares.Server
{
    [ServiceContract]
    public interface IFacade
    {
        RootModel GetRootModel(String issuerId);
    }
}
