using System.Collections.Generic;
using System.Linq;
using Core.Business.Url;
using Core.Models.Categories;
using Database;

namespace Core.Business.Categories
{
    /// <summary>
    /// Manager for the categories in the database
    /// </summary>
    public class CategoryService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        /// <summary>
        /// Get the category id, title, keywords
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CategorySeoModel GetSeoByCategoryId(int categoryId)
        {
            var categoryDb = _unitOfWorkRepository.CategoryRepository.GetById(categoryId);

            if (categoryDb != null)
            {
                return new CategorySeoModel(categoryDb);
            }

            return null;
        }

        /// <summary>
        /// Get the categories database entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Database.Entities.Categories GetCategory(int id)
        {
            return _unitOfWorkRepository.CategoryRepository.GetById(id);
        }

        /// <summary>
        /// Get all the categories
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoryModel> GetCategories()
        {
            return from c in _unitOfWorkRepository.CategoryRepository.GetAll().ToList()
                select new CategoryModel(c, UrlHelpers.GetCleanUrl(c.Name));
        }

        /// <summary>
        /// Update a specific category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="name"></param>
        /// <param name="keywords"></param>
        public void UpdateCategory(int categoryId, string name, string keywords)
        {
            var category = GetCategory(categoryId);

            if (category != null)
            {
                category.Name = name;
                category.Keywords = keywords;

                _unitOfWorkRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Add a category
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keywords"></param>
        public void AddCategory(string name, string keywords)
        {
            _unitOfWorkRepository.CategoryRepository.AddCategory(new Database.Entities.Categories
            {
                Name = name,
                Keywords = keywords
            });

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
