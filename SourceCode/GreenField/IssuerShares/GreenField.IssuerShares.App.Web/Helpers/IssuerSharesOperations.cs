using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.IssuerShares.App.Web.Services;
using GreenField.IssuerShares.Server;

namespace GreenField.IssuerShares.App.Web
{
    public class IssuerSharesOperationsHostFactory: Aims.Data.Server.ServiceHostFactoryBase<IssuerSharesOperations, IFacade>
    {
        protected override IFacade CreateSettings()
        {
            var facade = new Facade();
            return facade;
        }

        protected override IssuerSharesOperations CreateService(IFacade facade)
        {
            var service = new IssuerSharesOperations(facade);
            return service;
        }
    }
}