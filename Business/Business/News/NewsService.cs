using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models.News;
using Database;

namespace Core.Business.News
{
    /// <summary>
    /// Manager for news articles
    /// </summary>
    public class NewsService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        /// <summary>
        /// Get a specific amount of news articles from the database
        /// </summary>
        /// <param name="amountArticles"></param>
        /// <returns></returns>
        public IEnumerable<NewsModel> GetNewsArticles(int amountArticles)
        {
           return _unitOfWorkRepository.NewsRepository.GetAll(amountArticles).ToList().Select(a => new NewsModel(a));
        }

        /// <summary>
        /// Get all the news articles from the database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NewsModel> GetNewsArticles()
        {
            return _unitOfWorkRepository.NewsRepository.GetAll().ToList().Select(a => new NewsModel(a));
        }

        /// <summary>
        /// Get article by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NewsModel GetArticleById(int id)
        {
            var newsArticle = _unitOfWorkRepository.NewsRepository.GetById(id);

            if (newsArticle != null)
            {
                return new NewsModel(newsArticle);
            }

            return null;
        }

        /// <summary>
        /// Update a specific article
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subject"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="information"></param>
        public void UpdateArticle(int id, string subject, string title, string description, string information)
        {
            var newsArticle = _unitOfWorkRepository.NewsRepository.GetById(id);

            if (newsArticle != null)
            {
                newsArticle.Subject = subject;
                newsArticle.Title = title;
                newsArticle.Description = description;
                newsArticle.Information = information;

                _unitOfWorkRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Add a new article
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="information"></param>
        /// <param name="userId"></param>
        public void AddArticle(string subject, string title, string description, string information, int userId)
        {
            _unitOfWorkRepository.NewsRepository.AddArticle(new Database.Entities.News
            {
                Date = DateTime.Now,
                AuthorID = userId,
                Description = description,
                Information = information,
                Subject = subject,
                Title = title
            });

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Remove a specific article
        /// </summary>
        /// <param name="id"></param>
        public void RemoveArticle(int id)
        {
            var article = _unitOfWorkRepository.NewsRepository.GetById(id);

            if (article != null)
            {
                _unitOfWorkRepository.NewsRepository.RemoveArticle(article);

                _unitOfWorkRepository.SaveChanges();
            }
        }
    }
}
