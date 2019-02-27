using AspNetCore.Base;
using AspNetCore.Base.Alerts;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.MailingLists.Dtos;
using DND.ApplicationServices.CMS.MailingLists.Services;
using DND.Web.Areas.Admin.Controllers.MailingList.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Web.Areas.Admin.Controllers.MailingList
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.CMS.MailingList.CollectionId)]
    [Route("admin/cms/mailing-list")]
    public class MailingListController : MvcControllerEntityAuthorizeBase<MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto, IMailingListApplicationService>
    {
        public MailingListController(IMailingListApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }

        [Route("email")]
        public virtual ActionResult Email()
        {
            var instance = new MailingListEmailViewModel();
            ViewBag.PageTitle = "Mailing List Email";
            ViewBag.Admin = Admin;
            return View("Email", instance);
        }

        // POST: Default/Create
        [HttpPost]
        [Route("email")]
        public virtual async Task<ActionResult> Email(MailingListEmailViewModel email)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var data = await Service.GetAllAsync(cts.Token);

                    List<EmailMessage> list = new List<EmailMessage>();

                    foreach (MailingListDto dto in data)
                    {
                        var message = new EmailMessage();
                        message.Body = email.Body;
                        message.IsHtml = true;
                        message.Subject = email.Subject;
                        message.ToEmail = dto.Email;
                        message.ToDisplayName = dto.Name;

                        list.Add(message);
                    }

                    EmailService.SendEmailMessages(list);

                    return RedirectToAction<MailingListController>(c => c.Email()).WithSuccess(this, Messages.AddSuccessful);
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }
            //error
            return View("Email", email);
        }
    }
}
