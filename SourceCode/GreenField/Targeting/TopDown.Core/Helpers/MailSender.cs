using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

namespace TopDown.Core.Helpers
{
    public class EmailNotificationException : ApplicationException
    {
        public EmailNotificationException(String message, Exception e) : base(message, e) { }
    }

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
            message.From = cc != null ? new MailAddress(cc) : new MailAddress(ConfigurationManager.AppSettings["TargetingAlertSender"]);
            if (!String.IsNullOrEmpty(cc))
            {
                message.CC.Add(cc);
            }

            Send(message);
        }

        public static String TransformTargetToString(Decimal? target)
        {

            return target.HasValue ? Math.Round(target.Value * 100, 2).ToString() : null;
        }
    }
}
