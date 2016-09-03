using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Database;
using Database.Entities;
using Localization.Languages.Business.Website;

namespace Core.Business.Blacklist
{
    public class BlackListService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        /// <summary>
        /// Add a website to the blacklist
        /// Doesn't validate the website
        /// </summary>
        /// <param name="domain"></param>
        public void AddWebsite(string domain)
        {
            _unitOfWorkRepository.WebsiteBlackListRepository.AddWebsite(new WebsiteBlackList
            {
                Domain = domain
            });

            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Removes a specific website from the blacklist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if deleted</returns>
        public bool RemoveWebsite(int id)
        {
            var websiteBlackList = _unitOfWorkRepository.WebsiteBlackListRepository.GetById(id);

            if (websiteBlackList != null)
            {
                _unitOfWorkRepository.WebsiteBlackListRepository.RemoveWebsite(websiteBlackList);

                _unitOfWorkRepository.SaveChanges();
            }

            return websiteBlackList != null;
        }

        /// <summary>
        /// Compares the host from the given website with the hosts of blacklist websites
        /// </summary>
        /// <param name="websiteUrl">Needs to be a valid uri string (http://google.nl not google.nl)</param>
        /// <returns></returns>
        public ResultModel ValidateWebsiteAllowed(string websiteUrl)
        {
            string givenWebsiteHost = new Uri(websiteUrl).Host;

            bool isAllowed = !((from website in _unitOfWorkRepository.WebsiteBlackListRepository.GetAll().ToList().Select(x => x.Domain)
                                let hostWebsite = new Uri(website).Host
                                where hostWebsite.Equals(givenWebsiteHost, StringComparison.CurrentCultureIgnoreCase)
                                select website).Any());

            return new ResultModel(!isAllowed ? WebsiteService.BlackList : string.Empty);
        }

        public IEnumerable<WebsiteBlackList> GetAll()
        {
            return _unitOfWorkRepository.WebsiteBlackListRepository.GetAll().ToList();
        }

        public WebsiteBlackList GetById(int id)
        {
            return _unitOfWorkRepository.WebsiteBlackListRepository.GetById(id);
        }

        public void Update(int id, string domain)
        {
            var record = GetById(id);

            if (record != null)
            {
                record.Domain = domain;

                _unitOfWorkRepository.SaveChanges();
            }
        }
    }
}
