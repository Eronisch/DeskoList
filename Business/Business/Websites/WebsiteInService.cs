using System;
using Database;
using Database.Entities;

namespace Core.Business.Websites
{
    public class WebsiteInService
    {
        private readonly WebsiteService _websiteService = new WebsiteService();
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();
        private Random _random;

        public void AddVote(int websiteId, string ip)
        {
            var website = _websiteService.GetWebsite(websiteId);

            if (website != null)
            {
                var currentDate = DateTime.Now;

                AddWebsiteIn(websiteId, ip, currentDate);

                AddWebsiteInDaily(websiteId, ip, currentDate);

                _unitOfWorkRepository.SaveChanges();
            }
        }

        public void CleanWebsiteInDaily()
        {
            _unitOfWorkRepository.WebsiteInDailyRepository.CleanWebsiteInDaily();
            _unitOfWorkRepository.SaveChanges();
        }

        public bool HasVoted(string ip, int websiteId)
        {
            return !_unitOfWorkRepository.WebsiteInRepository.IsUniqueVote(websiteId, ip);
        }



        public int GetUniqueIn(DateTime date)
        {
            return _unitOfWorkRepository.WebsiteInDailyRepository.GetUniqueWebsiteIn(date);
        }

        public void CleanWebsiteIn()
        {
            _unitOfWorkRepository.WebsiteInRepository.CleanWebsiteIn();

            _unitOfWorkRepository.SaveChanges();
        }

        private void AddWebsiteIn(int websiteId, string ip, DateTime currentDate)
        {
            var isUniqueVote = _unitOfWorkRepository.WebsiteInRepository.IsUniqueVote(websiteId, ip);

            _unitOfWorkRepository.WebsiteInRepository.AddVote(new WebsiteIn
            {
                WebsiteID = websiteId,
                IP = ip,
                Unique = isUniqueVote,
                Date = currentDate
            });
        }

        private void AddWebsiteInDaily(int websiteId, string ip, DateTime currentDate)
        {
            var websiteIn = _unitOfWorkRepository.WebsiteInDailyRepository.GetByDate(currentDate);

            if (websiteIn == null)
            {
                websiteIn = new WebsiteInDaily
                {
                    Date = currentDate,
                    WebsiteId = websiteId
                };

                _unitOfWorkRepository.WebsiteInDailyRepository.Add(websiteIn);
            }

            if (_unitOfWorkRepository.WebsiteInRepository.IsUniqueVoteToday(websiteId, ip, currentDate))
            {
                websiteIn.UniqueIn++;
            }

            websiteIn.TotalIn++;
        }
    }
}
