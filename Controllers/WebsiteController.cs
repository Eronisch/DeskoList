using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Business.Plugin;
using Core.Business.Rating;
using Core.Business.Reports;
using Core.Business.Settings;
using Core.Business.Url;
using Core.Business.Websites;
using Core.Models.Reports;
using Core.Models.Statistics;
using Localization.Languages.Controllers;
using Topsite.Action_Filters;
using Topsite.Models;
using Web.Account;
using Web.Ip;
using Web.Messages;
using Web.Seo;
using Web.Website;

namespace Topsite.Controllers
{
    public class WebsiteController : Controller
    {
        private readonly WebsiteService _websiteService;
        private readonly Web.Category.CategoryService _categoryService;
        private readonly SettingsService _settingsService;
        private readonly WebsiteInService _websiteInService;

        public WebsiteController()
        {
            _websiteService = new WebsiteService();
            _categoryService = new Web.Category.CategoryService();
            _settingsService = new SettingsService();
            _websiteInService = new WebsiteInService();
        }

        #region Website details

        // Todo: create action filter that checks if the website exists
        [HttpGet]
        public ActionResult View(int id)
        {
            var statisticsService = new WebsiteStatisticsService();

            var website = _websiteService.GetWebsiteModel(id);

            var ratingService = new RatingService();

            if (website == null)
            {
                this.SetError(Website.NoWebsiteFound);
                return RedirectToAction("Index", "Home");
            }

            var statistics = statisticsService.GetStatisticsFromWebsite(id, false);

            var bundleWebsiteStatisticsModel = new BundleWebsiteStatisticsModel(statistics.In, statistics.Out, statistics.UniqueIn,
                statistics.UniqueOut, website);

            this.UpdateSeo(website.Title, website.Description, website.Keywords);

            return View(new ViewWebsiteModel(bundleWebsiteStatisticsModel, ratingService.HasRated(IpHelper.GetIpFromCurrentRequest(), website.Id)));
        }

        [HttpPost]
        public ActionResult Rate(WebsiteRatingModel websiteRating)
        {
            if (ModelState.IsValid)
            {
                var ratingService = new RatingService();

                ratingService.AddRating(websiteRating.WebsiteId, websiteRating.Rating,
                    IpHelper.GetIpFromCurrentRequest());

                this.SetSuccess(Website.RateAdded);
            }
            else
            {
                this.SetError(ModelState.Values.First(x => x.Errors.Any()).Errors.First().ErrorMessage);
            }


            return RedirectToAction("View", new { id = websiteRating.WebsiteId });
        }

        #endregion

        #region View all users websites

        [LoginRequiredActionFilter]
        public ActionResult ViewAll()
        {
            return View(_websiteService.GetWebsitesFromUser(LoginHelper.GetUserId()));
        }

        #endregion

        #region Edit website

        [LoginRequiredActionFilter]
        public ActionResult Edit(int id)
        {
            int userId = LoginHelper.GetUserId();

            if (_websiteService.IsWebsiteFromUser(userId, id))
            {
                var website = _websiteService.GetWebsiteModel(id);

                return View(new EditWebsiteModel
                {
                    BannerUrl = website.BannerUrl,
                    Categories = GetCategories(website.CategoryId),
                    Category = website.CategoryId,
                    Description = website.Description,
                    Id = website.Id,
                    Keywords = website.Keywords,
                    Title = website.Title,
                    Url = website.Url,
                    UseBannerUrl = !website.HasFileBanner,
                    IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled(),
                    ServerIp = website.ServerIp,
                    ServerPort = website.ServerPort
                });
            }

            return RedirectToAction("ViewAll", "Website");
        }

        [HttpPost]
        [LoginRequiredActionFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditWebsiteModel editWebsiteModel)
        {
            int userId = LoginHelper.GetUserId();

            if (!_websiteService.IsWebsiteFromUser(userId, editWebsiteModel.Id)) { return RedirectToAction("ViewAll", "Website"); }

            if (ModelState.IsValid)
            {
                var websiteValidationResult = _websiteService.ValidateWebsite(editWebsiteModel.BannerFile?.InputStream, editWebsiteModel.BannerFile?.FileName,
                    editWebsiteModel.BannerUrl, editWebsiteModel.Url, editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, !editWebsiteModel.UseBannerUrl, false);

                string errorMessage = websiteValidationResult.ErrorMessage;

                var website = _websiteService.GetWebsite(editWebsiteModel.Id);

                if (websiteValidationResult.IsSuccess)
                {
                    var pluginResult = PluginHookActivateService.FireHook(PluginHooks.EditWebsite, LoginHelper.GetUserId(), editWebsiteModel.Title,
                      editWebsiteModel.Description, editWebsiteModel.Url, editWebsiteModel.BannerUrl,
                      editWebsiteModel.Keywords, editWebsiteModel.Category, editWebsiteModel.BannerFile,
                      editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, !editWebsiteModel.UseBannerUrl, website.Sponsored);

                    errorMessage = pluginResult.ErrorMessage;

                    if (pluginResult.IsSuccess)
                    {
                        _websiteService.UpdateWebsite(editWebsiteModel.Id, editWebsiteModel.Title, editWebsiteModel.Description, editWebsiteModel.Keywords, editWebsiteModel.Category, editWebsiteModel.Url, editWebsiteModel.BannerUrl, editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, editWebsiteModel.BannerFile?.InputStream, editWebsiteModel.BannerFile?.FileName, !editWebsiteModel.UseBannerUrl, website.Sponsored);
                        this.SetSuccess(Website.WebsiteUpdated);
                        return RedirectToAction("ViewAll", "Website");
                    }
                }

                this.SetError(errorMessage);
            }

            editWebsiteModel.Categories = GetCategories(editWebsiteModel.Category);
            editWebsiteModel.IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled();

            return View(editWebsiteModel);
        }

        #endregion

        #region Delete website

        [LoginRequiredActionFilter]
        public ActionResult Delete(int id)
        {
            int userId = LoginHelper.GetUserId();

            if (_websiteService.IsWebsiteFromUser(userId, id))
            {
                this.SetSuccess(Website.DeletedSuccessfully);
                _websiteService.DeleteWebsite(id);
            }

            return RedirectToAction("ViewAll", "Website");
        }

        #endregion

        #region Add website

        [HttpPost]
        [LoginRequiredActionFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Add(EditWebsiteModel editWebsiteModel)
        {
            if (ModelState.IsValid)
            {
                var websiteValidationResult = _websiteService.ValidateWebsite(editWebsiteModel.BannerFile?.InputStream, editWebsiteModel.BannerFile?.FileName,
                    editWebsiteModel.BannerUrl, editWebsiteModel.Url, editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, !editWebsiteModel.UseBannerUrl, true);

                var errorMessage = websiteValidationResult.ErrorMessage;

                if (websiteValidationResult.IsSuccess)
                {
                    var pluginsResult = PluginHookActivateService.FireHook(PluginHooks.AddWebsite, LoginHelper.GetUserId(), editWebsiteModel.Title,
                      editWebsiteModel.Description, editWebsiteModel.Url, editWebsiteModel.BannerUrl,
                      editWebsiteModel.Keywords, editWebsiteModel.Category, editWebsiteModel.BannerFile,
                      editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, !editWebsiteModel.UseBannerUrl, editWebsiteModel.IsSponsored);

                    errorMessage = pluginsResult.ErrorMessage;

                    if (pluginsResult.IsSuccess)
                    {
                        var website = _websiteService.AddWebsite(LoginHelper.GetUserId(), LoginHelper.GetUsername(), editWebsiteModel.Title,
                      editWebsiteModel.Description, editWebsiteModel.Url, editWebsiteModel.BannerUrl,
                      editWebsiteModel.Keywords, editWebsiteModel.Category, editWebsiteModel.BannerFile?.InputStream, editWebsiteModel.BannerFile?.FileName,
                      editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, !editWebsiteModel.UseBannerUrl);

                        this.SetSuccess(Website.AddedSuccesfully);

                        return RedirectToAction("Code", new { id = website.Id });
                    }
                }

                this.SetError(errorMessage);
            }

            editWebsiteModel.Categories = GetCategories(editWebsiteModel.Category);
            editWebsiteModel.IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled();

            return View(editWebsiteModel);
        }

        [LoginRequiredActionFilter]
        public ActionResult Add()
        {
            var editWebsiteModel = new EditWebsiteModel
            {
                Categories = GetCategories(),
                IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled()
            };

            return View(editWebsiteModel);
        }

        #endregion

        #region Get code for specific website

        [LoginRequiredActionFilter]
        public ActionResult Code(int id)
        {
            if (_websiteService.IsWebsiteFromUser(LoginHelper.GetUserId(), id))
            {
                var website = _websiteService.GetWebsiteModel(id);

                return View(new GetCodeModel(_settingsService.GetTitle(), UrlHelpers.GetCurrentBaseUrl(), website));
            }

            return RedirectToAction("ViewAll", "Website");
        }

        #endregion

        #region Get codes vor all websites

        [LoginRequiredActionFilter]
        public ActionResult Codes()
        {
            var websiteService = new WebsiteService();

            var myWebsites = websiteService.GetWebsitesFromUser(LoginHelper.GetUserId());

            string baseUrl = UrlHelpers.GetCurrentBaseUrl();

            return View(myWebsites.Select(w => new GetCodeModel(_settingsService.GetTitle(), baseUrl, w)));
        }

        #endregion

        #region Api

        public ActionResult Api()
        {
            return View();
        }

        #endregion

        #region Statistics

        [LoginRequiredActionFilter]
        public ActionResult Statistics()
        {
            var statisticsService = new WebsiteStatisticsService();

            return View(statisticsService.GetBundleStatisticsFromUser(LoginHelper.GetUserId(), IpHelper.GetIpFromCurrentRequest(), true));
        }

        #endregion

        #region Report

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(ReportUserModel reportUserModel)
        {
            if (ModelState.IsValid)
            {
                var reportService = new ReportsService();

                var reportStatus = reportService.AddReport(reportUserModel.WebsiteId,
                    IpHelper.GetIpFromCurrentRequest(),
                    reportUserModel.Reason);

                if (reportStatus == AddReportType.WebsiteNotFound)
                {
                    this.SetError(Website.NoWebsiteFound);
                }
                else
                {
                    this.SetSuccess(Website.WebsiteReportedSuccessfully);
                }
            }
            else
            {
                this.SetError(Website.InvalidReasonField);
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Check Vote

        public ActionResult HasVoted(int id)
        {
            var voteService = new WebsiteInService();

            var website = _websiteService.GetWebsite(id);

            if (website != null)
            {
                if (voteService.HasVoted(IpHelper.GetIpFromCurrentRequest(), id))
                {
                    return Content("true");
                }

                return Content("false");
            }

            // Invalid website
            return Content("error");
        }

        #endregion

        #region Vote

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(VoteModel voteModel)
        {
            var websiteVoteService = new WebsiteVoteService();

            if (!websiteVoteService.ValidateAnswer(voteModel.Answer, TempData)) { return SetIncorrectGoToVote(voteModel, Website.InvalidAnswer); }

            var pluginResult = PluginHookActivateService.FireHook(PluginHooks.VoteWebsite, voteModel.Id, voteModel.Redirect);

            if (!pluginResult.IsSuccess) { return SetIncorrectGoToVote(voteModel, pluginResult.ErrorMessage); }

            _websiteInService.AddVote(voteModel.Id, IpHelper.GetIpFromCurrentRequest());

            if (UrlValidator.ValidateUrl(voteModel.Redirect)) { return Redirect(voteModel.Redirect); }

            return RedirectToAction("Index", "Home");
        }

        [ValidateInput(false)]
        public ActionResult Vote(int id, string redirect = null)
        {
            var websiteVoteService = new WebsiteVoteService();

            var website = _websiteService.GetWebsite(id);

            // Set answer validates if the website exists, so we don't have to check if the website is empty
            if (!websiteVoteService.SetAnswer(id, TempData))
            {
                this.SetError(Website.NoWebsiteFound);
                return RedirectToAction("Index", "Home");
            }

            return View(new VoteModel
            {
                Id = id,
                Redirect = redirect,
                WebsiteTitle = website.Title,
                Answer = websiteVoteService.GetAnswer(TempData)
            });
        }

        #endregion

        #region Go

        public ActionResult Go(string user, int id)
        {
            var redirectService = new WebsiteOutService();
            var websiteService = new WebsiteService();

            var website = websiteService.GetWebsite(id);

            if (website != null)
            {
                redirectService.AddRedirect(website.Id, IpHelper.GetIpFromCurrentRequest());

                return Redirect(website.Url);
            }

            return RedirectToAction("Index", "Home");
        }


        #endregion

        #region Private Methods

        private ActionResult SetIncorrectGoToVote(VoteModel voteModel, string errorMessage)
        {
            var websiteVoteService = new WebsiteVoteService();

            this.SetError(errorMessage);

            websiteVoteService.SetAnswer(voteModel.Id, TempData);

            return View(voteModel);
        }

        private IEnumerable<SelectListItem> GetCategories(int? selectedCategoryId = null)
        {
            return _categoryService.GetListItemCategories(selectedCategoryId);
        }

        #endregion
    }
}