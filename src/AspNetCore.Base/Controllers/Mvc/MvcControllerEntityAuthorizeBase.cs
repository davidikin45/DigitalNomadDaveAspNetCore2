using AspNetCore.Base.Alerts;
using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Email;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.Mvc
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    //[Authorize(Roles = "admin")]
    public abstract class MvcControllerEntityAuthorizeBase<TCreateDto, TReadDto, TUpdateDto, TDeleteDto, IEntityService> : MvcControllerEntityReadOnlyAuthorizeBase<TReadDto, IEntityService>
        where TCreateDto : class
        where TReadDto : class
        where TUpdateDto : class
        where TDeleteDto : class
        where IEntityService : IApplicationServiceEntity<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
    {
        public MvcControllerEntityAuthorizeBase(Boolean admin, IEntityService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
        : base(admin, service, mapper, emailService, appSettings)
        {
        }

        #region New Instance
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Create)]
        //[Authorize(Policy = ApiScopes.Create)]
        [Route("new")]
        public virtual ActionResult Create()
        {
            var instance = Service.GetCreateDefaultDto();
            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Create", instance);
        }
        #endregion

        #region Create
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Create)]
        //[Authorize(Policy = ApiScopes.Create)]
        [HttpPost]
        [Route("new")]
        public virtual async Task<ActionResult> Create(TCreateDto dto)
        {
            var cts = TaskHelper.CreateNewCancellationTokenSource();

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await Service.CreateAsync(dto, Username, cts.Token);
                    if (result.IsFailure)
                    {
                        HandleUpdateException(result, null, true);
                    }
                    else
                    {
                        return RedirectToControllerDefault().WithSuccess(this, Messages.AddSuccessful);
                    }
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }
            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            //error
            return View("Create", dto);
        }
        #endregion

        #region Get for Edit
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Update, ResourceCollectionsCore.CRUD.Operations.UpdateOwner)]
        //[Authorize(Policy = ApiScopes.Update)]
        [Route("edit/{id}")]
        public virtual async Task<ActionResult> Edit(string id)
        {
            var cts = TaskHelper.CreateNewCancellationTokenSource();
            TUpdateDto data = null;
            try
            {
                data = await Service.GetUpdateDtoByIdAsync(id, cts.Token);
                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Edit", data);
            }
            catch
            {
                return HandleReadException();
            }
        }
        #endregion

        #region Update
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Update, ResourceCollectionsCore.CRUD.Operations.UpdateOwner)]
        //[Authorize(Policy = ApiScopes.Update)]
        [HttpPost]
        [Route("edit/{id}")]
        public virtual async Task<ActionResult> Edit(string id, TUpdateDto dto)
        {
            //dto.Id = id;
            var cts = TaskHelper.CreateNewCancellationTokenSource();

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await Service.UpdateGraphAsync(id, dto, Username, cts.Token);
                    if (result.IsFailure)
                    {
                        HandleUpdateException(result, dto, true);
                    }
                    else
                    {
                        return RedirectToControllerDefault().WithSuccess(this, Messages.UpdateSuccessful);
                    }
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Edit", dto);
        }
        #endregion

        #region Get for Delete
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Delete, ResourceCollectionsCore.CRUD.Operations.DeleteOwner)]
        //[Authorize(Policy = ApiScopes.Delete)]
        [Route("delete/{id}")]
        public virtual async Task<ActionResult> Delete(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            TDeleteDto data = null;
            try
            {
                data = await Service.GetDeleteDtoByIdAsync(id, cts.Token);
                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Delete", data);
            }
            catch
            {
                return HandleReadException();
            }
        }
        #endregion

        #region Delete
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Delete, ResourceCollectionsCore.CRUD.Operations.DeleteOwner)]
        //[Authorize(Policy = ApiScopes.Delete)]
        [HttpPost, ActionName("Delete"), Route("delete/{id}")]
        public virtual async Task<ActionResult> DeleteConfirmed(string id, TDeleteDto dto)
        {
            var cts = TaskHelper.CreateNewCancellationTokenSource();

            if (ModelState.IsValid)
            {
                try
                {
                    //var result = await Service.DeleteAsync(id, cts.Token);
                    var result = await Service.DeleteAsync(dto, Username, cts.Token); // This should give concurrency checking
                    if (result.IsFailure)
                    {
                        HandleUpdateException(result, dto, true);
                    }
                    else
                    {
                        return RedirectToControllerDefault().WithSuccess(this, Messages.DeleteSuccessful);
                    }
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            var data = await Service.GetByIdAsync(id, cts.Token);
            return View("Delete", data);
        }
        #endregion

        #region Create New Collection Item Instance
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Create, ResourceCollectionsCore.CRUD.Operations.Update)]
        //[Authorize(Policy = ApiScopes.Write)]
        [HttpGet]
        [Route("new/{*collection}")]
        public virtual ActionResult CreateCollectionItem(string collection)
        {
            if (!RelationshipHelper.IsValidCollectionItemCreateExpression(collection, typeof(TUpdateDto)))
            {
                return HandleReadException();
            }

            ViewBag.Collection = collection.Replace("/", ".");
            ViewBag.CollectionIndex = Guid.NewGuid().ToString();

            var instance = Service.GetCreateDefaultCollectionItemDto(collection);

            return PartialView("_CreateCollectionItem", instance);
        }
        #endregion

        #region Ajax Remote Validation
        //[Remote(action: "CustomFieldValidation", controller: "Home")]
        //public IActionResult CustomFieldValidation(string fieldValue)
        //{
        //    if (fieldValue == "007")
        //        return Json(data: "007 is already assigned to James Bond!");

        //    return Json(data: true);
        //}
        #endregion
    }
}

