using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Web.Category
{
    public class CategoryService
    {
        private readonly Core.Business.Categories.CategoryService _categoryService;

        public CategoryService()
        {
            _categoryService = new Core.Business.Categories.CategoryService();
        }

        public IEnumerable<SelectListItem> GetListItemCategories(int? selectedCategoryId = null)
        {
            return _categoryService.GetCategories().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CategoryId.ToString(),
                Selected = selectedCategoryId != null && x.CategoryId == selectedCategoryId
            });
        }
    }
}
