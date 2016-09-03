using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Core.Business.Blacklist;
using Core.Business.File;
using Core.Business.Settings;
using Core.Business.Url;
using Core.Models;
using Core.Models.Websites;
using Database;

namespace Core.Business.Websites
{
    public class WebsiteService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();
        private readonly BlackListService _blackListService = new BlackListService();

        public Database.Entities.Websites GetWebsite(int websiteId, bool eagerLoadRedirects = false, bool eagerLoadVotes = false, bool eagerLoadRatings = false, bool eagerLoadUsers = false)
        {
            return _unitOfWorkRepository.WebsitesRepository.GetById(websiteId, eagerLoadRedirects, eagerLoadVotes, eagerLoadRatings, eagerLoadUsers);
        }

        public WebsiteModel GetWebsiteModel(int websiteId)
        {
            return new WebsiteModel(_unitOfWorkRepository.WebsitesRepository.GetById(websiteId));
        }

        public bool IsWebsiteFromUser(int userId, int websiteId)
        {
            var website = GetWebsite(websiteId);

            return website != null && website.UserID == userId;
        }

        public IEnumerable<Database.Entities.Websites> GetAllWebsites( bool includeBanned)
        {
            return
                _unitOfWorkRepository.WebsitesRepository.GetAll(includeBanned);
        }

        public IEnumerable<Database.Entities.Websites> GetPage(bool includeUsers, bool includeBanned, int page,
            int amount)
        {
            return
               _unitOfWorkRepository.WebsitesRepository.GetPage(includeUsers, includeBanned, page, amount);
        }

        public int GetAmountWebsites()
        {
            return _unitOfWorkRepository.WebsitesRepository.GetAmountWebsites();
        }

        public int GetAmountWebsites(int year, int month)
        {
            return _unitOfWorkRepository.WebsitesRepository.GetAmountWebsites(year, month);
        }

        public void UpdateWebsite(int websiteId, string title, string description, string keywords, int categoryId,
            string url, string bannerUrl, string serverIp, int? serverPort, Stream banner, string bannerFileName, bool useBannerFile, bool isSponsored)
        {
            var website = GetWebsite(websiteId);

            if (website != null)
            {
                website.Title = title;
                website.Description = description;
                website.Keywords = keywords;
                website.Url = url;
                website.CategoryID = categoryId;
                website.Sponsored = isSponsored;

                UpdateBanner(url, bannerUrl, banner, bannerFileName, useBannerFile, website);

                UpdateServer(serverIp, serverPort, ref website);

                _unitOfWorkRepository.SaveChanges();
            }
        }

        public void UpdateMonitor(int websiteId, bool isOnline)
        {
            var website = _unitOfWorkRepository.WebsitesRepository.GetById(websiteId);

            website.IsOnline = isOnline;
            website.MonitorCheckedDate = DateTime.Now;;

            _unitOfWorkRepository.SaveChanges();
        }

        public IEnumerable<WebsiteModel> GetWebsitesFromUser(int userId)
        {
            return _unitOfWorkRepository.WebsitesRepository.GetWebsitesFromUser(userId).ToList().Select(w => new WebsiteModel(w));
        }

        public IEnumerable<WebsiteModel> GetWebsitesDbFromUser(int userId)
        {
            return _unitOfWorkRepository.WebsitesRepository.GetWebsitesFromUser(userId).ToList().Select(w => new WebsiteModel(w));
        }

        public ResultModel ValidateWebsite(Stream banner, string bannerName, string bannerUrl, string url, string serverIp, int? serverPort, bool useBannerFile, bool bannerRequired)
        {
            // Blacklist
            var validationBlackListResult = _blackListService.ValidateWebsiteAllowed(url);

            if (!validationBlackListResult.IsSuccess) { return validationBlackListResult; }

            // Server
            var validationServerResult = ValidateServer(serverIp, serverPort);

            if (!validationServerResult.IsSuccess) { return validationServerResult; }

            // Banner
            if (bannerRequired) { return ValidateBanner(banner, bannerName, bannerUrl, useBannerFile);}

            // Success
            return new ResultModel();
        }

        public Database.Entities.Websites AddWebsite(int userId, string username, string title, string description, string url,
               string bannerUrl, string keywords, int categoryId, Stream banner, string bannerFileName, string serverIp, int? serverPort, bool useBannerFile)
        {
            var website = new Database.Entities.Websites
            {
                BannerURL = bannerUrl,
                CategoryID = categoryId,
                DateAdded = DateTime.Now,
                Description = description,
                Keywords = keywords,
                Title = title,
                Url = url,
                UserID = userId,
                Enabled = true,
                ServerIP = serverIp,
                ServerPort = serverPort
            };

            if (useBannerFile)
            {
                string fileName = UrlHelpers.GetHost(url, false) + Path.GetExtension(bannerFileName);

                FileService.SaveFile(banner, string.Format("Banners/{0}", username), fileName);

                website.BannerFileName = fileName;
            }
            else
            {
                website.BannerURL = bannerUrl;
            }

            _unitOfWorkRepository.WebsitesRepository.AddWebsite(website);

            _unitOfWorkRepository.SaveChanges();

            return website;
        }

        public void DeleteWebsite(int id)
        {
            var website = _unitOfWorkRepository.WebsitesRepository.GetById(id);

            _unitOfWorkRepository.WebsitesRepository.RemoveWebsite(website);

            _unitOfWorkRepository.SaveChanges();
        }

        private ResultModel ValidateServer(string ip, int? port)
        {
            var settingsService = new SettingsService();

            // Check if website monitoring is enabled
            if (settingsService.IsUserServerMonitoringEnabled())
            {
                // Validate ip
                IPAddress serverIp;

                if (!IPAddress.TryParse(ip, out serverIp))
                {
                    return new ResultModel(Localization.Languages.Business.Website.WebsiteService.InvalidServerIp);
                }

                // Validate port
                if (!port.HasValue)
                {
                    return new ResultModel(Localization.Languages.Business.Website.WebsiteService.InvalidServerPort);
                }
            }

            // Success
            return new ResultModel();
        }

        private ResultModel ValidateBanner(Stream banner, string bannerFileName, string bannerUrl, bool useBannerFile)
        {
            const int maxSizeMb = 5;

            if (useBannerFile)
            {
                if (banner == null)
                {
                    return new ResultModel(Localization.Languages.Business.Website.WebsiteService.Empty);
                }
                if (!FileService.GetAllowedImageExtensions()
                        .Any(extension => Path.GetExtension(bannerFileName).Equals(extension, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return new ResultModel(Localization.Languages.Business.Website.WebsiteService.InvalidExtension);
                }
                if (!FileService.IsValidSize(banner, maxSizeMb))
                {
                    return new ResultModel(string.Format(Localization.Languages.Business.Website.WebsiteService.InvalidSize, maxSizeMb));
                }
            }
            else
            {
                if (!UrlValidator.ValidateUrl(bannerUrl)) { return new ResultModel(Localization.Languages.Business.Website.WebsiteService.InvalidUrl); }
            }

            return new ResultModel();
        }

        private void UpdateBanner(string url, string bannerUrl, Stream banner, string bannerFileName, bool useBannerFile, Database.Entities.Websites website)
        {
            if (string.IsNullOrEmpty(bannerUrl) && banner == null) { return; }

            if (useBannerFile)
            {
                website.BannerURL = null;

                string fileName = UrlHelpers.GetHost(url, false) + Path.GetExtension(bannerFileName);

                FileService.SaveFile(banner, string.Format("Banners/{0}", website.Users.Username),
                    fileName);

                website.BannerFileName = fileName;
            }
            else
            {
                FileService.DeleteFile(string.Format("Banners/{0}", website.BannerFileName));

                website.BannerFileName = null;
                website.BannerURL = bannerUrl;
            }
        }

        private void UpdateServer(string serverIp, int? serverPort, ref Database.Entities.Websites website)
        {
            var settingsService = new SettingsService();

            if (settingsService.IsUserServerMonitoringEnabled())
            {
                website.ServerIP = serverIp;
                website.ServerPort = serverPort;
            }
        }

        public void UpdateThumbnail(int websiteId, string filename)
        {
            var website = _unitOfWorkRepository.WebsitesRepository.GetById(websiteId);

            website.Thumbnail = filename;

            _unitOfWorkRepository.SaveChanges();
        }
    }
}
