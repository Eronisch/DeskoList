using System.Collections.Generic;
using System.Linq;
using Database;
using Database.Entities;

namespace Core.Business.Pages
{
    /// <summary>
    /// Manager for the navigation items
    /// </summary>
    public class NavigationService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public NavigationService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
        }

        /// <summary>
        /// Get all the navigation items
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NavigationPages> GetNavigation()
        {
            return _unitOfWorkRepository.NavigationPagesRepository.GetAll().ToList();
        }

        /// <summary>
        /// Add a navigation item
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="order"></param>
        /// <param name="parentId">navigation id parent, not required (for sub tabs)</param>
        public void AddNavigation(string title, string url, int order, int? parentId)
        {
            _unitOfWorkRepository.NavigationPagesRepository.AddNavigation(new NavigationPages
            {
                Title = title,
                Url = url,
                ParentId = parentId,
                OrderNumber = order
            });

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
