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

        public Server.RootModel GetRootModel(String securityShortName)
        {
            return this.facade.GetRootModel(securityShortName);
        }





        public IEnumerable<Aims.Data.Server.SecurityModel> GetIssuerSecurities(string pattern, int atMost, string securityShortName)
        {
            return this.Watch("Unable to get securities for the pattern \"" + pattern + "\" from the issuer (ID: " + securityShortName + ").", delegate
            {
                return this.facade.GetIssuerSecurities(pattern, atMost, securityShortName);
            });
        }


        public Server.RootModel UpdateIssueSharesComposition(RootModel model)
        {
            return this.Watch("Unable to update composition for the issuer (ID: " + model.Issuer.Id + ").", delegate
            {
                return this.facade.UpdateIssueSharesComposition(model);
            });
        }
    }
}
