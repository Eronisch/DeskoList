using Core.Models.Email;
using Database;
using Database.Entities;

namespace Core.Business.Email
{
    /// <summary>
    /// Manager for email accounts in the database
    /// </summary>
    public class EmailAccountService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        /// <summary>
        /// Get the info or no reply database email entity
        /// </summary>
        /// <param name="emailAccountType"></param>
        /// <returns></returns>
        public EmailAccounts GetEmailAccount(EmailAccountType emailAccountType)
        {
            if (emailAccountType == EmailAccountType.Info)
            {
                return _unitOfWorkRepository.EmailAccountRepository.GetInfoAccount();
            }

            return _unitOfWorkRepository.EmailAccountRepository.GetNoReply();
        }

        /// <summary>
        /// Add a new email account
        /// Should only be used when installing the software
        /// </summary>
        /// <param name="id"></param>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="port"></param>
        /// <param name="secure"></param>
        public void AddEmailAccount(int id, string host, string username, string password, int port, bool secure)
        {
            _unitOfWorkRepository.EmailAccountRepository.AddEmailAccount(new EmailAccounts
            {
                Id = id,
                SMTPEmail = username,
                SMTPPassword = password,
                SMTPServer = host,
                SMTPPort = port,
                SMTPSecure = secure
            });

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
