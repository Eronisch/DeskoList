using System;

namespace Core.Models.News
{
    public class NewsModel
    {
        public NewsModel(Database.Entities.News article)
        {
            Id = article.Id;
            Subject = article.Subject;
            Date = article.Date;
            Information = article.Information;
            Description = article.Description;
            AuthorId = article.AuthorID;
            AuthorName = article.Users.Username;
            Title = article.Title;
        }

        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public string Information { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
    }
}