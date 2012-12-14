using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.IssuerShares.App.Web.Services;
using GreenField.IssuerShares.Server;
using Aims.Core.Sql;
using System.Configuration;
using GreenField.IssuerShares.Core;

namespace GreenField.IssuerShares.App.Web
{
    public class IssuerSharesOperationsHostFactory: Aims.Data.Server.ServiceHostFactoryBase<IssuerSharesOperations, IFacade>
    {
        protected override IFacade CreateSettings()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[""].ConnectionString;
            var connectionFactory = new SqlConnectionFactory(connectionString);
            var dataManagerFactory = new DataManagerFactory();
            var repositoryManager = 
            var manager = new Core.ModelManager(connectionFactory, dataManagerFactory, );
            var serializer = new Serializer(new Aims.Data.Server.Serializer());
            var facade = new Facade(manager, serializer);
            return facade;
        }

        protected override IssuerSharesOperations CreateService(IFacade facade)
        {
            var service = new IssuerSharesOperations(facade);
            return service;
        }
    }
}