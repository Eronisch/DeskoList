using Database;
using Database.Entities;

namespace Core.Business.Email
{
    /// <summary>
    /// Manager for retrieving email templates from the database
    /// </summary>
    public class EmailTemplateService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        /// <summary>
        /// Get the welcome email template
        /// </summary>
        /// <param name="verificationRequred"></param>
        /// <returns></returns>
        public EmailTemplates GetWelcome(bool verificationRequred)
        {
            return _unitOfWorkRepository.EmailTemplatesRepository.GetById(verificationRequred ? 2 : 1);
        }

        /// <summary>
        /// Get the password reset email template
        /// </summary>
        /// <returns></returns>
        public EmailTemplates GetPasswordReset()
        {
            return _unitOfWorkRepository.EmailTemplatesRepository.GetById(3);
        }

        /// <summary>
        /// Get the username request email template
        /// </summary>
        /// <returns></returns>
        public EmailTemplates GetUsernameRequest()
        {
            return _unitOfWorkRepository.EmailTemplatesRepository.GetById(4);
        }

        /// <summary>
        /// Get the contact email template
        /// </summary>
        /// <returns></returns>
        public EmailTemplates GetContact()
        {
            return _unitOfWorkRepository.EmailTemplatesRepository.GetById(5);
        }

        /// <summary>
        /// Get the statistics email template
        /// </summary>
        /// <returns></returns>
        public EmailTemplates GetStatistics()
        {
            return _unitOfWorkRepository.EmailTemplatesRepository.GetById(6);
        }

        /// <summary>
        /// Get the activate email template
        /// </summary>
        /// <returns></returns>
        public EmailTemplates GetActivateNewEmail()
        {
            return _unitOfWorkRepository.EmailTemplatesRepository.GetById(7);
        }

        /// <summary>
        /// Get the account verified by an admin email template
        /// </summary>
        /// <returns></returns>
        public EmailTemplates GetAccountVerifiedByAdministratorEmail()
        {
            return _unitOfWorkRepository.EmailTemplatesRepository.GetById(8);
        }

        /// <summary>
        /// Add a new email template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="name"></param>
        public void AddEmailTemplate(int id, string email, string subject, string name)
        {
            _unitOfWorkRepository.EmailTemplatesRepository.AddEmailTemplate(new EmailTemplates
            {
                Id = id,
                Email = email,
                Name = name,
                Subject = subject
            });

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
