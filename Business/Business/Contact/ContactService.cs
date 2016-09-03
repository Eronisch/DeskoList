using System;
using System.Collections.Generic;
using System.Globalization;
using Core.Business.Email;
using Core.Extensions;
using Core.Models.Email;
using Database.Entities;

namespace Core.Business.Contact
{
    /// <summary>
    /// Manager for replacing email contents with provided information and sending them
    /// </summary>
    public class ContactService
    {
        private readonly EmailTemplateService _emailTemplateService = new EmailTemplateService();
        private readonly EmailService _emailService = new EmailService();
        private readonly EmailAccountService _emailAccountService = new EmailAccountService();

        /// <summary>
        /// Replaces the email content and sends the contact email to the info email account in the database
        /// Sending email is async
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        public void SendContactEmail(string name, string email, string subject, string message, string ip)
        {
            var emailTemplate = _emailTemplateService.GetContact();

            string emailMessage = emailTemplate.Email.ReplaceText(new Dictionary<string, string>
            {
                {"{email}", email},
                {"{ip}", ip},
                {"{subject}", subject},
                {"{name}", name},
                {"{message}", message},
                {"{sendDate}", DateTime.Now.ToString(CultureInfo.InvariantCulture)}
            });

            // Replace the stuff from the email subject with actual data
           string emailSubject = emailTemplate.Subject.ReplaceText(new Dictionary<string, string>
                {
                    {"{subject}", subject},
                    {"{name}", name}
                });

            // Get email account
           var emailAccount = _emailAccountService.GetEmailAccount(EmailAccountType.Info);

            // Send email
            _emailService.AsyncSendEmail(emailAccount.SMTPEmail, emailSubject, emailMessage,
                emailAccount);
        }

        /// <summary>
        /// Replaces the email content and sends the account activate email
        /// Sending email is async
        /// </summary>
        /// <param name="emailTo"></param>
        /// <param name="userId"></param>
        /// <param name="username"></param>
        /// <param name="hostAdress"></param>
        /// <param name="emailVerificationCode"></param>
        /// <param name="titleWebsite"></param>
        /// <param name="isEmailVerificationRequired"></param>
        public void SendActivateAccountEmail(string emailTo, int userId, string username, string hostAdress, string emailVerificationCode, string titleWebsite, bool isEmailVerificationRequired)
        {
            var emailTemplate = _emailTemplateService.GetWelcome(isEmailVerificationRequired);

            string emailSubject = emailTemplate.Subject.ReplaceText(new Dictionary<string, string>
            {
                {"{username}", username},
                {"{title}", titleWebsite}
            });

            string emailMessage = emailTemplate.Email.ReplaceText(new Dictionary<string, string>
                                    {
                                        {"{username}", username},
                                        {"{title}", titleWebsite},
                                        {"{link}", string.Format("<a target='_blank' href='{0}/Account/Activate?code={1}&accountId={2}'>activate account</a>", hostAdress, emailVerificationCode, userId)}
                                    });

            _emailService.AsyncSendEmail(emailTo, emailSubject, emailMessage, _emailAccountService.GetEmailAccount(EmailAccountType.NoReply));
        }

        /// <summary>
        /// Replaces the email content and sends an email to activate your new email adress
        /// Sending email is async
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="username"></param>
        /// <param name="newEmail"></param>
        /// <param name="hostAdress"></param>
        /// <param name="emailVerificationCode"></param>
        /// <param name="titleWebsite"></param>
        public void SendActivateEmail(int userId, string username, string newEmail, string hostAdress, string emailVerificationCode, string titleWebsite)
        {
            var emailTemplate = _emailTemplateService.GetActivateNewEmail();

            string emailMessage = emailTemplate.Email.ReplaceText(new Dictionary<string, string>
                            {
                                {"{username}", username},
                                {"{title}", titleWebsite},
                                {"{link}", string.Format("<a target='_blank' href='{0}/Account/ActivateNewEmail?code={1}&accountId={2}'>activate account</a>", hostAdress, emailVerificationCode, userId)}
                            });

            string emailSubject = emailTemplate.Subject.ReplaceText(new Dictionary<string, string>
                            {
                                {"{title}", titleWebsite}
                            });

            _emailService.AsyncSendEmail(newEmail, emailSubject, emailMessage, _emailAccountService.GetEmailAccount(EmailAccountType.NoReply));
        }

        /// <summary>
        /// Replaces the email content and sends an email to reset the users password
        /// Sending email is async
        /// </summary>
        /// <param name="generatedPassword"></param>
        /// <param name="toEmail"></param>
        /// <param name="username"></param>
        /// <param name="titleWebsite"></param>
        public void SendPasswordResetEmail(string generatedPassword, string toEmail, string username, string titleWebsite)
        {
            var emailTemplate = _emailTemplateService.GetPasswordReset();

            string emailMessage = emailTemplate.Email.ReplaceText(new Dictionary<string, string>
                            {
                                {"{username}", username},
                                {"{title}", titleWebsite},
                                {"{password}", generatedPassword}
                            });

            string emailSubject = emailTemplate.Subject.ReplaceText(new Dictionary<string, string>
                            {
                                {"{title}", titleWebsite}
                            });

            _emailService.AsyncSendEmail(toEmail, emailSubject, emailMessage, _emailAccountService.GetEmailAccount(EmailAccountType.NoReply));
        }

        /// <summary>
        /// Replaces the email content and sends an email with the users username
        /// Sending email is async
        /// </summary>
        /// <param name="account"></param>
        /// <param name="titleWebsite"></param>
        public void SendRequestUsernameEmail(Users account, string titleWebsite)
        {
            var emailTemplate = _emailTemplateService.GetUsernameRequest();

            string emailMessage = emailTemplate.Email.ReplaceText(new Dictionary<string, string>
                    {
                        {"{username}", account.Username},
                        {"{title}", titleWebsite}
                    });

            string emailSubject = emailTemplate.Subject.ReplaceText(new Dictionary<string, string>
                    {
                        {"{title}", titleWebsite}
                    });

            _emailService.AsyncSendEmail(account.Email, emailSubject, emailMessage, _emailAccountService.GetEmailAccount(EmailAccountType.NoReply));
        }

        /// <summary>
        /// Replaces the email content and sends an email to the user that his account is verified by an administrator
        /// Sending email is async
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="titleWebsite"></param>
        public void SendAccountVerifiedByAnAdministratorEmail(string email, string username, string titleWebsite)
        {
            var emailTemplate = _emailTemplateService.GetAccountVerifiedByAdministratorEmail();

            string emailMessage = emailTemplate.Email.ReplaceText(new Dictionary<string, string>
                    {
                        {"{username}", username},
                        {"{title}", titleWebsite}
                    });

            string emailSubject = emailTemplate.Subject.ReplaceText(new Dictionary<string, string>
                    {
                        {"{title}", titleWebsite}
                    });

            _emailService.AsyncSendEmail(email, emailSubject, emailMessage, _emailAccountService.GetEmailAccount(EmailAccountType.NoReply));
        }
    }
}
