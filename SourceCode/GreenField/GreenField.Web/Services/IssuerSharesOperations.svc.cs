using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GreenField.IssuerShares.Server;
using System.ServiceModel.Activation;

namespace GreenField.Web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class IssuerSharesOperations : IFacade
    {
        private IFacade facade;
        public IssuerSharesOperations(FacadeSettings settings)
        {
            this.facade = new GreenField.IssuerShares.Server.Facade(settings);
        }

        protected TResult Watch<TResult>(String failureMessage, Func<TResult> action)
        {
            TResult result;
            // Stopwatch
            Stopwatch stopwatch = new Stopwatch();
            DateTime dateTime = new DateTime();
            stopwatch.Start();
            dateTime = DateTime.Now;
            Trace.WriteLine(string.Format("{0} started at {1}", action.ToString(), dateTime.ToString()));

            try
            {

                result = action();

                stopwatch.Stop();
                Trace.WriteLine(string.Format("{1}: {2} = {0} seconds.", (stopwatch.ElapsedMilliseconds / 1000.00).ToString(), dateTime.ToString(), action.ToString()));
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                Trace.WriteLine(string.Format("{1}: {2} = {0} seconds (timed out).", (stopwatch.ElapsedMilliseconds / 1000.00).ToString(), dateTime.ToString(), action.ToString()));
                throw new ApplicationException(failureMessage + " Reason: " + exception.Message, exception);
            }
            return result;
        }

        public GreenField.IssuerShares.Server.RootModel GetRootModel(String securityShortName)
        {
            return this.Watch("Unable to get model for the securty (Short name: " + securityShortName + ").", delegate
            {
                return this.facade.GetRootModel(securityShortName);
            });
        }





        public IEnumerable<Aims.Data.Server.SecurityModel> GetIssuerSecurities(string pattern, int atMost, string securityShortName)
        {
            return this.Watch("Unable to get securities for the pattern \"" + pattern + "\" from the issuer (ID: " + securityShortName + ").", delegate
            {
                return this.facade.GetIssuerSecurities(pattern, atMost, securityShortName);
            });
        }


        public GreenField.IssuerShares.Server.RootModel UpdateIssueSharesComposition(RootModel model)
        {
            return this.Watch("Unable to update composition for the issuer (ID: " + model.Issuer.Id + ").", delegate
            {
                return this.facade.UpdateIssueSharesComposition(model);
            });
        }


        public IEnumerable<IssuerSecurityShareRecordModel> GetIssuerSharesBySecurityShortName(string securityShortName)
        {
            return this.Watch("Unable to get share records for the securty (Short name: " + securityShortName + ").", delegate
            {
                return this.facade.GetIssuerSharesBySecurityShortName(securityShortName);
            });
        }
    }
}
