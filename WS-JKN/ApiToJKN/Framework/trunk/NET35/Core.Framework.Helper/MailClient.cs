using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Core.Framework.Helper
{
    public class ConnectionDomain
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Modul { get; set; }
        public string KdProfile { get; set; }
        public string Profile { get; set; }
    }
    public interface ILoadModel
    {
        bool IsLoad { get; set; }
        void OnInitLoad(IDataRecord read);
        bool Skip { get; set; }
    }
    public class MailClient
    {
        public string SmtpServer { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public MailClient(string smtpServer, string username, string password)
        {
            SmtpServer = smtpServer;
            Username = username;
            Password = password;
        }

        public MailClient()
        {
            SmtpServer = ConfigurationManager.AppSettings["smtpUrl"];
            Username = ConfigurationManager.AppSettings["username"];
            Password = ConfigurationManager.AppSettings["password"];
        }

        public void SendEmailAsycn(string from, string to, string title, string message, string cc)
        {
            ThreadPool.QueueUserWorkItem(SendEmail, new[] { from, to, title, message, cc });
        }

        private void SendEmail(object state)
        {
            var arr = (string[])state;
            var i = 0;
            while (!SendEmail(arr[0], arr[1], arr[2], arr[3], arr[4]))
            {
                i++;
                if (i == 20)
                    break;
            }
        }
        public bool SendEmail(string from, string to, string title, string message, string cc)
        {
            try
            {

                var mySmtpClient = new SmtpClient(SmtpServer, 587);

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = true;
                mySmtpClient.EnableSsl = true;
                mySmtpClient.Credentials = new NetworkCredential(Username, Password);
                // add from,to mailaddresses                
                var myMail = new System.Net.Mail.MailMessage(from, to);
                if (!string.IsNullOrEmpty(cc))
                    myMail.CC.Add(cc);

                // set subject and encoding
                myMail.Subject = title;
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = message;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
                return true;
            }

            catch (SmtpException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
