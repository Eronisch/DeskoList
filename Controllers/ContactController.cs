using System.Web.Mvc;
using Core.Business.Contact;
using Core.Business.Plugin;
using Localization.Languages.Controllers;
using Topsite.Models;
using Web.Ip;
using Web.Messages;

namespace Topsite.Controllers
{
    public class ContactController : Controller
    {
        private readonly ContactService _contactService;

        public ContactController()
        {
            _contactService = new ContactService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(EmailModel emailModel)
        {
            if (ModelState.IsValid)
            {
                var pluginResult = PluginHookActivateService.FireHook(PluginHooks.SendContact, emailModel.Name,
                    emailModel.Email, emailModel.Subject, emailModel.Message, IpHelper.GetIpFromCurrentRequest());

                if (pluginResult.IsSuccess)
                {
                    _contactService.SendContactEmail(emailModel.Name, emailModel.Email, emailModel.Subject, emailModel.Message, IpHelper.GetIpFromCurrentRequest());

                    this.SetSuccess(Contact.SendSuccess);

                    return RedirectToAction("Index", "Home");
                }

                this.SetError(pluginResult.ErrorMessage);
            }

            return View(emailModel);
        }
    }
}