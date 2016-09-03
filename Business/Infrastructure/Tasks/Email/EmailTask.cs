using System.Collections.Generic;
using System.Linq;
using Core.Business.Email;
using Core.Business.Websites;
using Core.Extensions;
using Core.Models.Email;
using Quartz;

namespace Core.Infrastructure.Tasks.Email
{
    public class EmailTask : IJob
    {
        private readonly WebsiteService _websiteService = new WebsiteService();
        private readonly WebsiteStatisticsService _websiteStatisticsService = new WebsiteStatisticsService();
        private readonly EmailService _emailService = new EmailService();
        private readonly EmailTemplateService _emailTemplateService = new EmailTemplateService();
        private readonly EmailAccountService _emailAccountService = new EmailAccountService();

        public void Execute(IJobExecutionContext context)
        {
            var emailTemplate = _emailTemplateService.GetStatistics();

            var websites = _websiteService.GetAllWebsites(includeBanned:false);

            foreach (var website in websites)
            {
                var statistics = _websiteStatisticsService.GetStatisticsFromWebsite(website.Id, false);
                var uniqueIn = statistics.UniqueIn.ToList();
                var uniqueOut = statistics.UniqueOut.ToList();

                // Todo: Create a view for this
                string htmlTable =
                    "<table style='border-spacing: 8px;border: 1px solid #000'><tr><th style='text-align:left'>Unique In</th><th style='text-align:left'>Unique Out</th><th style='text-align:left'>Date</th></tr>";

                for (int counterDays = 0; uniqueIn.Count() > counterDays; counterDays++)
                {
                    htmlTable += "<tr>" + uniqueIn[counterDays].Amount + "</td>" + "<td>" +
                                 uniqueOut[counterDays].Amount + "</td><td> " +
                                 uniqueIn[counterDays].Date.ToShortDateString() + "</td></tr>";
                }

                htmlTable += "</table>";

                string emailMessage = emailTemplate.Email.ReplaceText(new Dictionary<string, string>
                {
                    {"{username}", website.Users.Username},
                    {"{title}", website.Title},
                    {"{table}", htmlTable}
                });

                string emailSubject = emailTemplate.Subject.ReplaceText(new Dictionary<string, string>
                {
                    {"{website}", website.Title}
                });

                _emailService.AsyncSendEmail(website.Users.Email, emailSubject, emailMessage, _emailAccountService.GetEmailAccount(EmailAccountType.NoReply));
            }
        }
    }
}
