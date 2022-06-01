using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Configuration;

namespace D.UserInterFace
{
    public static class EmailSetting
    {
        #region -- Mail setting --

        public static void SendEmail(MailMessage m)
        {
            if(m.From == null)
                m.From = new MailAddress(ConfigurationManager.AppSettings["mail"], ConfigurationManager.AppSettings["frommsg"]);
            //m.Bcc.Add(new MailAddress("abawankar@gmail.com"));
            SendEmail(m, true);
        }

        public static void SendEmail(MailMessage m, Boolean Async)
        {
            SmtpClient smtpClient = null;
            smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mail"], ConfigurationManager.AppSettings["pwd"]);
            smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            smtpClient.Host = ConfigurationManager.AppSettings["host"];
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["ssl"]);
            if (Async)
            {
                SendEmailDelegate sd = new SendEmailDelegate(smtpClient.Send);
                AsyncCallback cb = new AsyncCallback(SendEmailResponse);
                sd.BeginInvoke(m, cb, sd);
            }
            else
            {
                smtpClient.Send(m);
            }
        }

        private delegate void SendEmailDelegate(MailMessage m);

        private static void SendEmailResponse(IAsyncResult ar)
        {
            SendEmailDelegate sd = (SendEmailDelegate)(ar.AsyncState);
            sd.EndInvoke(ar);
        }

        #endregion
    }
}