using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Configuration;
using System.Net.Mail;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.Web.Helpers;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AlertOperations
    {
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean SendAlert(List<String> messageTo, List<String> messageCc, String messageBody, String messageSubject)
        {
            Boolean result = false;
            try
            {

                String networkWebmasterEmail = ConfigurationManager.AppSettings.Get("NetworkWebmasterEmail");

                String networkCredentialPassword = ConfigurationManager.AppSettings.Get("NetworkCredentialPassword");
                String networkCredentialUsername = ConfigurationManager.AppSettings.Get("NetworkCredentialUsername");
                String networkCredentialDomain = Convert.ToString(ConfigurationManager.AppSettings.Get("NetworkCredentialDomain"));

                Int32 networkConnectionPort = Convert.ToInt32(ConfigurationManager.AppSettings.Get("NetworkConnectionPort"));
                String networkConnectionHost = Convert.ToString(ConfigurationManager.AppSettings.Get("NetworkConnectionHost"));

                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(networkWebmasterEmail);

                foreach (String email in messageTo)
                {
                    mm.To.Add(new MailAddress(email));
                }

                foreach (String email in messageCc)
                {
                    mm.CC.Add(new MailAddress(email));
                }

                mm.Subject = messageSubject;
                mm.Body = messageBody;
                mm.IsBodyHtml = false;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = networkConnectionHost;
                smtpClient.Port = networkConnectionPort;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.EnableSsl = true;

                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = networkCredentialUsername;
                NetworkCred.Domain = networkCredentialDomain;
                NetworkCred.Password = networkCredentialPassword;
                smtpClient.Credentials = NetworkCred;

                ServicePointManager.ServerCertificateValidationCallback = delegate(object s
                    , X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                smtpClient.Send(mm);
                result = true;

            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

            return result;
        }

        private String BuildBody(String ClientName, String StartDate, String EndDate)
        {
            StringBuilder Str = new StringBuilder();
            Str.Append("Dear RBC Projects" + "<br><br>");
            Str.Append(ClientName + " Applied for leave from" + "<br><br>");
            Str.Append(StartDate + " " + "To the " + EndDate + "<br><br>");
            Str.Append("Kind Regards" + "<br><br><br>");
            Str.Append("Human Resource System(VLS)" + "<br>");
            return Str.ToString();
        }

        private String BuildBody(String ClientName, String ClientEmail, int BestWayToContactus, String ClientPhone)
        {
            StringBuilder Str = new StringBuilder();
            Str.Append("Dear Madam/Sir" + "<br><br>");
            Str.Append("A Client Sent you a message via the Website" + "<br><br>");
            Str.Append("Name: " + ClientName + "<br><br>");
            Str.Append("Email:" + ClientEmail + "<br><br>");
            Str.Append("TEl:" + ClientPhone + "<br><br>");

            if (BestWayToContactus == 1)
            {
                Str.Append("Best way to to Contact the Client:" + "Phone" + "<br><br>");
            }
            else
            {
                Str.Append("Best way to to Contact the Client: " + "Email" + "<br><br>");
            }

            Str.Append("Kind Regards" + "<br><br><br>");
            Str.Append("WebMaster" + "<br>");

            return Str.ToString();
        }

        private String BuildBody(String ClientName, String ClientEmail, int BestWayToContactus, String ClientPhone, String Comment)
        {
            StringBuilder Str = new StringBuilder();
            Str.Append("Dear Madam/Sir" + "<br><br>");
            Str.Append(ClientName + " Below are the details" + "<br><br>");
            Str.Append("Email:" + ClientEmail + "<br><br>");
            Str.Append("TEl:" + ClientPhone + "<br><br>");

            if (BestWayToContactus == 1)
            {
                Str.Append("Best way to to Contact the Client:" + "Phone" + "<br><br>");
            }
            else
            {
                Str.Append("Best way to to Contact the Client: " + "Email" + "<br><br>");
            }
            Str.Append("Comment:" + Comment + "<br><br>");
            Str.Append("Kind Regards" + "<br><br><br>");
            Str.Append("WebMaster" + "<br>");

            return Str.ToString();
        }
    }
}
