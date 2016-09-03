using System.Collections.Generic;
using System.Linq;
using Core.Business.Websites;
using Core.Models.Reports;
using Database;

namespace Core.Business.Reports
{
    public class ReportsService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();
        private readonly WebsiteService _websiteService = new WebsiteService();

        public AddReportType AddReport(int websiteId, string senderIp, string message)
        {
            var website = _websiteService.GetWebsite(websiteId);

            if (website == null) { return AddReportType.WebsiteNotFound; }

            _unitOfWorkRepository.ReportRepository.AddReport(new Database.Entities.Reports
            {
                Message = message,
                SenderIP = senderIp,
                WebsiteId = websiteId
            });

            _unitOfWorkRepository.SaveChanges();

            return AddReportType.Success;
        }

        public int GetAmountReports()
        {
            return _unitOfWorkRepository.ReportRepository.GetAll().Count();
        }

        public IEnumerable<Database.Entities.Reports> GetReports(int amountReports)
        {
            return _unitOfWorkRepository.ReportRepository.GetSpecificAmountOfReports(amountReports);
        }

        public IEnumerable<Database.Entities.Reports> GetReports()
        {
            return _unitOfWorkRepository.ReportRepository.GetAll();
        }

        public Database.Entities.Reports GetReport(int id)
        {
            return _unitOfWorkRepository.ReportRepository.GetById(id);
        }

        public void RemoveReport(int id)
        {
            var report = _unitOfWorkRepository.ReportRepository.GetById(id);

            if (report != null)
            {
                _unitOfWorkRepository.ReportRepository.RemoveReport(report);
                _unitOfWorkRepository.SaveChanges();
            }
            
        }
    }
}
