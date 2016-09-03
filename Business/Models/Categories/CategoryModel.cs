namespace Core.Models.Categories
{
    public class CategoryModel : CategorySeoModel
    {
        public string Link { get; private set; }

        public CategoryModel(Database.Entities.Categories category, string categoryLink) : base(category)
        {
            Link = categoryLink;
        }
    }
}
