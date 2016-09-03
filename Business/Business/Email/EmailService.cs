using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Database.Entities;

namespace Core.Business.Email
{
    /// <summary>
    /// Manager for sending emails
    /// </summary>
    public class EmailService
    {
        /// <summary>
        /// Sends a new email async from the a specific email account
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="fromAccount"></param>
        public void AsyncSendEmail(string toEmail, string subject, string body, EmailAccounts fromAccount)
        {
            Task.Run(() => ThreadSendInfoEmail(toEmail, subject, body, fromAccount));
        }

        private void ThreadSendInfoEmail(string toEmail, string subject, string body, EmailAccounts fromEmailAccount)
        {
            var mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.From = new MailAddress(fromEmailAccount.SMTPEmail);
            mail.Subject = subject;
            mail.Body = body;

            var smtp = new SmtpClient(fromEmailAccount.SMTPServer, fromEmailAccount.SMTPPort)
            {
                Credentials = new NetworkCredential(fromEmailAccount.SMTPEmail, fromEmailAccount.SMTPPassword),
                EnableSsl = fromEmailAccount.SMTPSecure
            };
            mail.IsBodyHtml = true;
            try
            {
                smtp.Send(mail);
            }
            catch (SmtpException)
            {
                // ignore
            }
            finally
            {
                smtp.Dispose();
                mail.Dispose();
            }
        }
    }
}
