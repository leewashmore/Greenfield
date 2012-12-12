using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using GreenField.IssuerShares.Server;

namespace GreenField.IssuerShares.App.Web.Services
{
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class IssuerSharesOperations : GreenField.IssuerShares.Server.IFacade
    {
        private IFacade facade;
        public IssuerSharesOperations(IFacade facade)
        {
            this.facade = facade;
        }

        public Server.RootModel GetRootModel(String issuerId)
        {
            return this.facade.GetRootModel(issuerId);
        }
    }
}
