using System;
using Database;
using Database.Entities;

namespace Core.Business.Websites
{
    public class WebsiteOutService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        public void CleanWebsiteOut()
        {
            _unitOfWorkRepository.WebsiteOutRepository.CleanWebsiteOut();
            _unitOfWorkRepository.SaveChanges();

        }

        public void CleanWebsiteOutDaily()
        {
            _unitOfWorkRepository.WebsiteOutDailyRepository.CleanWebsiteOutDaily();
            _unitOfWorkRepository.SaveChanges();
        }

        public void AddRedirect(int websiteId, string ip)
        {
            var currentDate = DateTime.Now;

            AddWebsiteOut(websiteId, ip, currentDate);

            AddWebsiteOutDaily(websiteId, ip, currentDate);

            _unitOfWorkRepository.SaveChanges();
        }

        private void AddWebsiteOut(int websiteId, string ip, DateTime currentDate)
        {
            _unitOfWorkRepository.WebsiteOutRepository.AddRedirect(new WebsiteOut
            {
                Date = currentDate,
                IP = ip,
                WebsiteID = websiteId,
                Unique = _unitOfWorkRepository.WebsiteOutRepository.IsUniqueRedirect(websiteId, ip)
            });
        }

        private void AddWebsiteOutDaily(int websiteId, string ip, DateTime currentDate)
        {
            var websiteOutDaily = _unitOfWorkRepository.WebsiteOutDailyRepository.GetByDate(currentDate);

            if (websiteOutDaily == null)
            {
                websiteOutDaily = new WebsiteOutDaily
                {
                    Date = currentDate,
                    WebsiteId = websiteId
                };

                _unitOfWorkRepository.WebsiteOutDailyRepository.Add(websiteOutDaily);
            }
          

            if (_unitOfWorkRepository.WebsiteOutRepository.IsUniqueRedirectToday(websiteId, ip, currentDate))
            {
                websiteOutDaily.UniqueOut++;
            }

            websiteOutDaily.TotalOut++;
        }

        public int GetUniqueOut(DateTime date)
        {
            return _unitOfWorkRepository.WebsiteOutDailyRepository.GetUniqueWebsiteOut(date);
        }
    }
}
