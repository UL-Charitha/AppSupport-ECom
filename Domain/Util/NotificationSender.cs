using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;

namespace PayLater.Domain.Util
{
    public class NotificationSender
    {
        public static void SendMailNotification(string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["smtpServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["fromAddress"]);
                mail.To.Add(ConfigurationManager.AppSettings["toAddresses"]);
                mail.Subject = subject;
                mail.Body = body;
                mail.Priority = MailPriority.High;

                //SmtpServer.Port = Convert.ToInt32(_port);
                //SmtpServer.Credentials = new System.Net.NetworkCredential(_authUsername, _authPassword);
                //SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);                
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error("Exception in mail relay " + ex.Message);                
            }

        }
    }
}
