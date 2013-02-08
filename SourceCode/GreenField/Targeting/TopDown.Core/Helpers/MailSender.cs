using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

namespace TopDown.Core.Helpers
{
    public class MailSender
    {
        public static void Send(MailMessage mail)
        {
            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.Send(mail);
            }
        }

       
        internal static void SendTargetingAlert(MailMessage message, string cc = null)
        {
            message.To.Add(ConfigurationManager.AppSettings["TargetingAlertGroup"]);
            message.From = new MailAddress(ConfigurationManager.AppSettings["TargetingAlertSender"]);
            if (!String.IsNullOrEmpty(cc))
            {
                message.CC.Add(cc);
            }

            Send(message);
        }
    }
}
