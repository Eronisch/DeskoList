namespace Core.Models.Categories
{
    public class CategorySeoModel
    {
        public CategorySeoModel(Database.Entities.Categories category)
        {
            Name = category.Name;
            Keywords = category.Keywords;
            CategoryId = category.Id;
        }

        public string Name { get; private set; }
        public string Keywords { get; private set; }
        public int CategoryId { get; private set; }
    }
}
