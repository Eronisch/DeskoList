using System.Collections.Generic;
using System.Linq;
using Database;
using Database.Entities;

namespace Core.Business.Pages
{
    /// <summary>
    /// Manager for dynamic pages
    /// </summary>
    public class DynamicPageService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        /// <summary>
        /// Get page by id
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public DynamicPages GetPageById(int pageId)
        {
            return _unitOfWorkRepository.DynamicPagesRepository.GetById(pageId);
        }

        /// <summary>
        /// Get all the pages database entities
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DynamicPages> GetPages()
        {
            return _unitOfWorkRepository.DynamicPagesRepository.GetAll().ToList();
        }

        /// <summary>
        /// Update a specific page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="keywords"></param>
        /// <param name="message"></param>
        public void UpdatePage(int pageId, string title, string description, string keywords, string message)
        {
            var page = GetPageById(pageId);

            if (page != null)
            {
                page.Title = title;
                page.Description = description;
                page.Keywords = keywords;
                page.Message = message;

                _unitOfWorkRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Delete a specific page
        /// </summary>
        /// <param name="pageId"></param>
        public void DeletePage(int pageId)
        {
            var page = GetPageById(pageId);

            if (page != null)
            {
                _unitOfWorkRepository.DynamicPagesRepository.Remove(page);

                _unitOfWorkRepository.SaveChanges();
            }

        }

        /// <summary>
        /// Add a specific page
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="keywords"></param>
        /// <param name="message"></param>
        public void AddPage(string title, string description, string keywords, string message)
        {
            _unitOfWorkRepository.DynamicPagesRepository.Add(new DynamicPages
            {
                Title = title,
                Description = description,
                Keywords = keywords,
                Message = message
            });

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
