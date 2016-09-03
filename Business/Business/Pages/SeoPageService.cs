using Database;
using Database.Entities;

namespace Core.Business.Pages
{
    /// <summary>
    /// Manager for seo (title, description, keywords)
    /// </summary>
    public class SeoPageService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public SeoPageService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
        }

        /// <summary>
        /// Get the seo info from a specific page
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public SeoPages GetSeo(string controller, string action)
        {
            return _unitOfWorkRepository.SeoPagesRepository.GetByControllerAndAction(controller, action);
        }

        /// <summary>
        /// Add a new seo item to a specific page
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="resourceBase">Localization file full classname, including namespace</param>
        /// <param name="resourceTitle"></param>
        /// <param name="resourceDescription"></param>
        public void AddSeo(string controller, string action, string resourceBase, string resourceTitle, string resourceDescription)
        {
            _unitOfWorkRepository.SeoPagesRepository.AddSeo(new SeoPages
            {
                ResourceBaseName = resourceBase,
                ResourceDescriptionName = resourceDescription,
                PageController = controller,
                PageIndex = action,
                ResourceTitleName = resourceTitle
            });

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
