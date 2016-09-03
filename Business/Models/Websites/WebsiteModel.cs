using System;
using System.Linq;

namespace Core.Models.Websites
{
    public class WebsiteModel
    {
        public WebsiteModel(Database.Entities.Websites website)
        {
            _websiteEntity = website;
        }

        private readonly Database.Entities.Websites _websiteEntity;

        public string Title
        {
            get { return _websiteEntity.Title; }
        }

        public string Description
        {
            get { return _websiteEntity.Description; }
        }

        public string Url
        {
            get { return _websiteEntity.Url; }
        }

        public string BannerUrl
        {
            get { return _websiteEntity.BannerURL; }
        }

        public string Username
        {
            get { return _websiteEntity.Users.Username; }
        }

        public int CategoryId
        {
            get { return _websiteEntity.CategoryID; }
        }

        public int UniqueVisitsToday
        {
            get { return _websiteEntity.WebsiteOut.Count(x => x.Unique); }
        }

        public int UniqueVotesToday
        {
            get { return _websiteEntity.WebsiteIn.Count(x => x.Unique); }
        }

        public int AmountRatings
        {
            get { return _websiteEntity.WebsiteRating.Count; }
        }

        public int AverageRatingRounded
        {
            get { return (int) Math.Round(AverageRatingDouble, 0); }
        }

        public double AverageRatingDouble
        {
            get
            {
                return _websiteEntity.WebsiteRating.Count == 0 ? 5 : Math.Round(_websiteEntity.WebsiteRating.Average(x => x.Rating), 1);
            }
        }

        public int Id
        {
            get { return _websiteEntity.Id; }
        }

        public string Keywords
        {
            get { return _websiteEntity.Keywords; }
        }

        public bool IsSponsored
        {
            get { return _websiteEntity.Sponsored; }
        }

        public bool IsOnline
        {
            get { return _websiteEntity.IsOnline; }
        }

        public DateTime? MonitorDateChecked
        {
            get { return _websiteEntity.MonitorCheckedDate; }
        }

        public string BannerFileName
        {
            get { return _websiteEntity.BannerFileName; }
        }

        public string Thumbnail
        {
            get { return _websiteEntity.Thumbnail; }
        }

        public string ServerIp
        {
            get { return _websiteEntity.ServerIP; }
        }

        public int? ServerPort
        {
            get { return _websiteEntity.ServerPort; }
        }

        public bool Enabled
        {
            get { return _websiteEntity.Enabled; }
        }

        public DateTime DateAdded
        {
            get { return _websiteEntity.DateAdded; }
        }

        public string CategoryName
        {
            get { return _websiteEntity.Categories.Name; }
        }

        public bool HasThumbnail
        {
            get { return !string.IsNullOrEmpty(Thumbnail); }
        }

        public bool HasFileBanner
        {
            get { return !string.IsNullOrEmpty(BannerFileName); }
        }

        public string GetBannerUrl()
        {
               return HasFileBanner ? string.Format("/Banners/{0}/{1}", Username, BannerFileName) : BannerUrl;
        }

        public string GetThumbnail()
        {
            return string.Format("/Thumbnails/{0}/{1}", Username, Thumbnail);
        }

        public bool IsVisible
        {
            get
            {
                return (_websiteEntity.Users.BannedStartDate == null ||
                        DateTime.Now > _websiteEntity.Users.BannedEndDate)
                       && _websiteEntity.Users.IsAdminVerified && _websiteEntity.Users.IsEmailVerified &&
                       _websiteEntity.Enabled;
            }
        }
    }
}
