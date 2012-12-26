using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using GreenField.IssuerShares.Server;
using System.Collections.Generic;

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

        protected TResult Watch<TResult>(String failureMessage, Func<TResult> action)
        {
            TResult result;
            try
            {
                result = action();
            }
            catch (Exception exception)
            {
                throw new ApplicationException(failureMessage + " Reason: " + exception.Message, exception);
            }
            return result;
        }

        public IssuerSharesOperations()
        { 
        
        }

        public Server.RootModel GetRootModel(String issuerId)
        {
            return this.facade.GetRootModel(issuerId);
        }


        public IEnumerable<Aims.Data.Server.SecurityModel> GetIssuerSecurities(String pattern, Int32 atMost, String issuerId)
        {
            return this.Watch("Unable to get securities for the pattern \"" + pattern + "\" from the issuer (ID: " + issuerId + ").", delegate
            {
                return this.facade.GetIssuerSecurities(pattern, atMost, issuerId);
            });
        }
    }
}
