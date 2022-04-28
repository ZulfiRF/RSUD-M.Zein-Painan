using Core.Framework.Helper.Logging;

namespace Core.Framework.Helper
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Threading;

    public class MailClient
    {
        #region Constructors and Destructors

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

        #endregion

        #region Public Properties

        public string Password { get; set; }
        public string SmtpServer { get; set; }
        public string Username { get; set; }

        #endregion

        #region Public Methods and Operators

        public bool SendEmail(string from, string to, string title, string message, string[] cc, object attachment = null)
        {
            try
            {
                var mySmtpClient = new SmtpClient(SmtpServer, 587);

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = true;
                mySmtpClient.EnableSsl = true;
                mySmtpClient.Credentials = new NetworkCredential(Username, Password);
                // add from,to mailaddresses                
                var myMail = new MailMessage(from, to);
                if (cc != null)
                {
                    foreach (var ccEmail in cc)
                    {
                        myMail.CC.Add(ccEmail);
                    }
                }

                // set subject and encoding
                myMail.Subject = title;
                myMail.SubjectEncoding = Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = message;
                myMail.BodyEncoding = Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;
                if (attachment != null)
                    myMail.Attachments.Add(attachment as Attachment);

                mySmtpClient.Send(myMail);
                return true;
            }

            catch (SmtpException ex)
            {
                Log.Error(ex);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public void SendEmailAsycn(string from, string to, string title, string message, string[] cc, Attachment attachment = null)
        {
            ThreadPool.QueueUserWorkItem(SendEmail, new object[] { from, to, title, message, cc, attachment });
        }

        #endregion

        #region Methods

        private void SendEmail(object state)
        {
            var arr = (object[])state;
            int i = 0;
            while (!SendEmail(arr[0].ToString(), arr[1].ToString(), arr[2].ToString(), arr[3].ToString(), arr[4] as string[], arr[5]))
            {
                i++;
                if (i == 20)
                {
                    break;
                }
            }
        }

        #endregion
    }
}