using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Poll;
using Core.Business.Poll;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Poll;
using Topsite.Areas.Administration.Models.Poll;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class PollController : AdminController
    {
        private readonly PollService _pollService;

        public PollController()
        {
            _pollService = new PollService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View(new AddPollModel
            {
                Answers = new[] {new PollAnswerModel {Answer =  ""}, new PollAnswerModel { Answer = "" } }
            });
        }

        public ActionResult View(int id)
        {
            return View(_pollService.GetById(id));
        }
         
        public JsonResult Get()
        {
            return Json(_pollService.GetPolls().Select(item => new
            {
                Id = item.Id,
                Question = item.Question,
                AmountVotes = item.AmountVotes,
                View =
                    BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.View,
                        ControllerContext.RequestContext.GetActionRoute("View", "Poll", new { id = item.Id }),
                        BootstrapButtonType.Warning,
                        BootstrapSize.ExtraSmall),
                Edit =
                    BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Edit,
                        ControllerContext.RequestContext.GetActionRoute("Edit", "Poll", new {id = item.Id}),
                        BootstrapButtonType.Success,
                        BootstrapSize.ExtraSmall),
                Delete =
                    BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete,
                        ControllerContext.RequestContext.GetActionRoute("Delete", "Poll", new {id = item.Id}),
                        BootstrapButtonType.Error, BootstrapSize.ExtraSmall, new Dictionary<string, string>
                        {
                            {"id", "deleteRecord"}
                        })
            }), JsonRequestBehavior.AllowGet);
        }

       public ActionResult Edit(int id)
        {
            var record = _pollService.GetById(id);

            if (record == null) { this.SetError(Poll.NotFound); return RedirectToAction("Index");}

            return View(new EditPollModel
            {
                Id = record.Id,
                Question = record.Question,
                Answers = record.Answers.Select(a => new PollEditAnswerModel
                {
                    Id = a.Id,
                    Answer = a.Answer
                }).ToList()
            });
        }

        [HttpPost]
        public ActionResult Edit(EditPollModel editPollModel)
        {
            if (ModelState.IsValid)
            {
                _pollService.Update(editPollModel.Id, editPollModel.Question, editPollModel.Answers.ToDictionary(a => a.Id, a => a.Answer));

                this.SetSuccess(Poll.SuccessfullyUpdated);

                return RedirectToAction("Index");
            }

            return View(editPollModel);
        }

        [HttpPost]
        public ActionResult Add(AddPollModel addPollModel)
        {
            if (ModelState.IsValid)
            {
                _pollService.Add(addPollModel.Question, addPollModel.Answers.Select(a => a.Answer));

                this.SetSuccess(Poll.SuccessfullyAdded);

                return RedirectToAction("Index");
            }

            return View(addPollModel);
        }

        public ActionResult Delete(int id)
        {
            if (_pollService.Remove(id))
            {
                this.SetSuccess(Poll.SuccessfullyRemoved);
            }
            else
            {
                this.SetError(Poll.NotFound);
            }

            return RedirectToAction("Index");
        }

    }
}
