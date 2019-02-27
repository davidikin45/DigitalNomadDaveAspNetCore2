using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.DomainEvents;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
    [Authorize()]
    //[Authorize(Policy = ApiScopes.Read)]
    public abstract class MvcControllerEntityReadOnlyAuthorizeBase<TDto, IEntityService> : MvcControllerBase
        where TDto : class
        where IEntityService : IApplicationServiceEntityReadOnly<TDto>
    {
        public IEntityService Service { get; private set; }
        public Boolean Admin { get; set; }

        public MvcControllerEntityReadOnlyAuthorizeBase(Boolean admin, IEntityService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
        : base(mapper, emailService, appSettings)
        {
            Admin = admin;
            Service = service;
        }

        #region Search
        // GET: Default
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner)]
        //[Authorize(Policy = ApiScopes.Read)]
        [Route("")]
        public virtual async Task<ActionResult> Index(int page = 1, int pageSize = 10, string orderColumn = "Id", string orderType = "desc", string search = "")
        {

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var dataTask = Service.SearchAsync(cts.Token, null, search, null, AutoMapperHelper.GetOrderBy<TDto>(orderColumn, orderType), page - 1, pageSize, true, false, null);
                var totalTask = Service.GetSearchCountAsync(cts.Token, null, search);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var response = new WebApiPagedResponseDto<TDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    Records = total,
                    Rows = data.ToList(),
                    OrderColumn = orderColumn,
                    OrderType = orderType,
                    Search = search
                };

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("List", response);
            }
            catch
            {
                return HandleReadException();
            }
        }
        #endregion

        #region GetById
        // GET: Default/Details/5
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner)]
        [Route("details/{id}")]
        public virtual async Task<ActionResult> Details(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            TDto data = null;
            try
            {
                data = await Service.GetByIdAsync(id, cts.Token, true);
                if (data == null)
                    return HandleReadException();
            }
            catch
            {
                return HandleReadException();
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Details", data);
        }
        #endregion

        #region Child Collection List and Details
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner)]
        [Route("details/{id}/{*collection}")]
        public virtual async Task<ActionResult> Collection(string id, string collection, int page = 1, int pageSize = 10, string orderColumn = "Id", string orderType = "desc", string search = "")
        {
            if (!RelationshipHelper.IsValidCollectionExpression(collection, typeof(TDto)))
            {
                return HandleReadException();
            }

            if (RelationshipHelper.IsCollectionExpressionCollectionItem(collection))
            {
                return await CollectionItemDetails(id, collection);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {

                var dataTask = Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, search, orderColumn, orderType == "asc" ? true : false, page - 1, pageSize);

                var totalTask = Service.GetByIdWithPagedCollectionPropertyCountAsync(cts.Token, id, collection, search);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var result = dataTask.Result;

                Type collectionItemType = RelationshipHelper.GetCollectionExpressionType(collection, typeof(TDto));
                object list = RelationshipHelper.GetCollectionExpressionData(collection, typeof(TDto), result);

                var total = totalTask.Result;

                var webApiPagedResponseDtoType = typeof(WebApiPagedResponseDto<>).MakeGenericType(collectionItemType);
                var response = Activator.CreateInstance(webApiPagedResponseDtoType);

                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.Page), page);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.PageSize), pageSize);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.Records), total);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.Rows), list);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.OrderColumn), orderColumn);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.OrderType), orderType);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.Search), search);

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                ViewBag.Collection = collection;
                ViewBag.Id = id;

                //For the time being collection properties are read only. DDD states that only the Aggregate Root should get updated.
                ViewBag.DisableCreate = true;
                ViewBag.DisableEdit = true;
                ViewBag.DisableDelete = true;
                ViewBag.DisableSorting = false;
                ViewBag.DisableEntityEvents = true;
                ViewBag.DisableSearch = false;

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("List", response);
            }
            catch
            {
                return HandleReadException();
            }
        }

        private async Task<ActionResult> CollectionItemDetails(string id, string collection)
        {
            if (!RelationshipHelper.IsValidCollectionExpression(collection, typeof(TDto)))
            {
                return HandleReadException();
            }

            if (!RelationshipHelper.IsCollectionExpressionCollectionItem(collection))
            {
                return HandleReadException();
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            Object data = null;
            try
            {
                var response = await Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, "", null, false, null, null);

                var collectionItem = RelationshipHelper.GetCollectionExpressionData(collection, typeof(TDto), response);

                if (collectionItem == null)
                {
                    return HandleReadException();
                }

                data = collectionItem;
            }
            catch
            {
                return HandleReadException();
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;

            ViewBag.DisableEdit = true;
            ViewBag.Collection = RelationshipHelper.GetCollectionExpressionWithoutCurrentCollectionItem(collection);
            ViewBag.Id = id;

            return View("Details", data);
        }
        #endregion
    }
}

