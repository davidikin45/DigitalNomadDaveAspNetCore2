using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authentication;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.ApiClient;
using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelBinders;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AspNetCore.Base.ValueProviders.DelimitedQueryString;
using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.Api
{
    //Resource naming - Should be noun = thing, not an action. Example. api/getauthors is bad. api/authors is good.
    //Folow this principle for predictability. Represent hierachy. api/authors/{author}/books
    //Pluralize vs not pluralize is OK. Stay consistent
    //Filters,Sorting,Ordering are not resources. Should not be in URL, use query string instead.


    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize]
    //[AllowAnonymous]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + BasicAuthenticationDefaults.AuthenticationScheme)]
    public abstract class ApiControllerEntityReadOnlyAuthorizeBase<TDto, IEntityService> : ApiControllerBase, IApiControllerEntityReadOnly<TDto>
        where TDto : class
        where IEntityService : IApplicationServiceEntityReadOnly<TDto>
    {
        public IEntityService Service { get; private set; }
        public ITypeHelperService TypeHelperService { get; private set; }

        public ApiControllerEntityReadOnlyAuthorizeBase(IEntityService service,  IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
        : base(mapper, emailService, linkGenerator, appSettings)
        {
            Service = service;
            TypeHelperService = typeHelperService;
        }

        #region Search

        /// <summary>
        /// Gets the paged.
        /// </summary>
        /// <param name="resourceParameters">The resource parameters.</param>
        /// <returns></returns>
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("")]
        [Route(".{format}")]
        [HttpGet]
        [HttpHead]
        public virtual async Task<ActionResult<WebApiListResponseDto<TDto>>> Search([FromQuery] WebApiPagedSearchOrderingRequestDto resourceParameters)
        {  
            if(User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value.EndsWith(ResourceCollectionsCore.CRUD.Operations.Read)).Count() == 0)
            {
                resourceParameters.UserId = UserId;
            }

            return await List(resourceParameters);
        }

        private async Task<ActionResult> List(WebApiPagedSearchOrderingRequestDto resourceParameters)
        {
            if (string.IsNullOrEmpty(resourceParameters.OrderBy))
                resourceParameters.OrderBy = "Id";

            if (!TypeHelperService.TypeHasProperties<TDto>(resourceParameters.Fields))
            {
                return BadRequest(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.SearchAsync(cts.Token, resourceParameters.UserId, resourceParameters.Search, null, AutoMapperHelper.GetOrderBy<TDto>(resourceParameters.OrderBy, resourceParameters.OrderType), resourceParameters.Page.HasValue ? resourceParameters.Page - 1 : null, resourceParameters.PageSize, true);

            var totalTask = Service.GetSearchCountAsync(cts.Token, resourceParameters.UserId, resourceParameters.Search, null);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var paginationMetadata = new PagingInfoDto
            {
                Page = resourceParameters.Page.HasValue ? resourceParameters.Page.Value : 1,
                PageSize = resourceParameters.PageSize.HasValue ? resourceParameters.PageSize.Value : data.Count(),
                Records = total,
                PreviousPageLink = null,
                NextPageLink = null
            };

            if (paginationMetadata.HasPrevious)
            {
                paginationMetadata.PreviousPageLink = CreateResourceUri(resourceParameters, ResourceUriType.PreviousPage);
            }

            if (paginationMetadata.HasNext)
            {
                paginationMetadata.NextPageLink = CreateResourceUri(resourceParameters, ResourceUriType.NextPage);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata).Replace(Environment.NewLine, ""));

            var links = CreateLinksForCollections(resourceParameters,
              paginationMetadata.HasNext, paginationMetadata.HasPrevious);

            var shapedData = IEnumerableExtensions.ShapeData(data.ToList(), resourceParameters.Fields);

            var shapedDataWithLinks = shapedData.Select(dto =>
            {
                var dtoAsDictionary = dto as IDictionary<string, object>;
                var dtoLinks = CreateLinks(
                    dtoAsDictionary["Id"].ToString(), resourceParameters.Fields);

                dtoAsDictionary.Add("links", dtoLinks);

                return dtoAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedDataWithLinks,
                links = links
            };

            return Ok(linkedCollectionResource);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read)]
        [FormatFilter]
        [Route("get-all")]
        [Route("get-all.{format}")]
        [HttpGet]
        public virtual async Task<ActionResult<List<TDto>>> GetAll()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetAllAsync(cts.Token, null, null, null, true);

            var list = response.ToList();

            return Ok(list);
        }

        /// <summary>
        /// Gets all paged.
        /// </summary>
        /// <returns></returns>
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read)]
        [FormatFilter]
        [Route("get-all-paged")]
        [Route("get-all-paged.{format}")]
        [HttpGet]
        public virtual async Task<ActionResult<List<TDto>>> GetAllPaged()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.GetAllAsync(cts.Token, null, null, null, true);

            var totalTask = Service.GetCountAsync(cts.Token);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var paginationMetadata = new PagingInfoDto
            {
                Page = 1,
                PageSize = total,
                Records = total,
                PreviousPageLink = "",
                NextPageLink = ""
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata).Replace(Environment.NewLine, ""));

            return Ok(data.ToList());
        }
        #endregion

        #region GetById with Composition Properties
        //http://jakeydocs.readthedocs.io/en/latest/mvc/models/formatting.html
        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("{id}"), Route("{id}.{format}")]
        //[Route("get/{id}"), Route("get/{id}.{format}")]
        [HttpGet]
        public virtual async Task<ActionResult<TDto>> GetById(string id, [FromQuery] WebApiParamsDto parameters)
        {
            if (!TypeHelperService.TypeHasProperties<TDto>(parameters.Fields))
            {
                return BadRequest(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            //By passing true we include the Composition properties which should be any child or join entities.
            var response = await Service.GetByIdAsync(id, cts.Token, true, false);

            if (response == null)
            {
                return NotFound();
            }

            var links = CreateLinks(id, parameters.Fields);

            var linkedResourceToReturn = response.ShapeData(parameters.Fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);

            // Success(shapedData);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("({ids})"), Route("({ids}).{format}")]
        [HttpGet]
        [DelimitedQueryString(',', '|')]
        public virtual async Task<ActionResult<List<TDto>>> BulkGetByIds(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetByIdsAsync(cts.Token, ids, true, false);

            var list = response.ToList();

            if (ids.Count() != list.Count())
            {
                return NotFound();
            }

            return Ok(list);
        }

        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("full-graph/{id}"), Route("full-graph/{id}.{format}")]
        [HttpGet]
        public virtual async Task<ActionResult<TDto>> GetByIdFullGraph(string id, [FromQuery] WebApiParamsDto parameters)
        {
            if (!TypeHelperService.TypeHasProperties<TDto>(parameters.Fields))
            {
                return BadRequest(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            //By passing true we should get the full graph of Composition and Aggregation Properties
            var response = await Service.GetByIdAsync(id, cts.Token, false, true);

            if (response == null)
            {
                return NotFound();
            }

            var links = CreateLinks(id, parameters.Fields, true);

            var linkedResourceToReturn = response.ShapeData(parameters.Fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);

            // Success(shapedData);
        }
        #endregion

        #region Child Collection List and Details
        /// <summary>
        /// Gets the paged.
        /// Service/Collection/Resource
        /// </summary>
        /// <param name="resourceParameters">The resource parameters.</param>
        /// <returns></returns>
        [ResourceAuthorize(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner)]
        [FormatFilter]
        [Route("{id}/{*collection}")]
        //[Route("{id}/{*collection}.{format}")]
        [HttpGet]
        [HttpHead]
        public virtual async Task<IActionResult> GetByIdChildCollection(string id, string collection, WebApiPagedSearchOrderingRequestDto resourceParameters)
        {
            if (string.IsNullOrEmpty(resourceParameters.OrderBy))
                resourceParameters.OrderBy = "Id";

            if (!RelationshipHelper.IsValidCollectionExpression(collection, typeof(TDto)))
            {
                return BadRequest(Messages.CollectionInvalid);
            }

            if (RelationshipHelper.IsCollectionExpressionCollectionItem(collection))
            {
                return await GetCollectionItem(id, collection, resourceParameters.Fields);
            }

            var collectionItemType = RelationshipHelper.GetCollectionExpressionType(collection, typeof(TDto));
            if (!TypeHelperService.TypeHasProperties(collectionItemType, resourceParameters.Fields))
            {
                return BadRequest(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, resourceParameters.Search, resourceParameters.OrderBy, resourceParameters.OrderType == "asc" ? true : false, resourceParameters.Page.HasValue ? resourceParameters.Page - 1 : null, resourceParameters.PageSize);

            var totalTask = Service.GetByIdWithPagedCollectionPropertyCountAsync(cts.Token, id, collection, resourceParameters.Search);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var result = dataTask.Result;

            var total = totalTask.Result;

            IEnumerable<Object> list = ((IEnumerable<Object>)RelationshipHelper.GetCollectionExpressionData(collection, typeof(TDto), result));

            var paginationMetadata = new PagingInfoDto
            {
                Page = resourceParameters.Page.HasValue ? resourceParameters.Page.Value : 1,
                PageSize = resourceParameters.PageSize.HasValue ? resourceParameters.PageSize.Value : list.Count(),
                Records = total,
                PreviousPageLink = null,
                NextPageLink = null
            };

            if (paginationMetadata.HasPrevious)
            {
                paginationMetadata.PreviousPageLink = CreateCollectionPropertyResourceUri(collection, resourceParameters, ResourceUriType.PreviousPage);
            }

            if (paginationMetadata.HasNext)
            {
                paginationMetadata.NextPageLink = CreateCollectionPropertyResourceUri(collection, resourceParameters, ResourceUriType.NextPage);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata).Replace(Environment.NewLine, ""));

            var links = CreateLinksForCollectionProperty(collection, resourceParameters, paginationMetadata.HasNext, paginationMetadata.HasPrevious);

            var shapedData = IEnumerableExtensions.ShapeData(list, collectionItemType, resourceParameters.Fields);

            var shapedDataWithLinks = shapedData.Select(collectionPropertyDtoItem =>
            {
                var collectionPropertyDtoItemAsDictionary = collectionPropertyDtoItem as IDictionary<string, object>;
                var collectionPropertyDtoItemLinks = CreateLinksForCollectionItem(id, collection + "/" + collectionPropertyDtoItemAsDictionary["Id"].ToString(), resourceParameters.Fields);

                collectionPropertyDtoItemAsDictionary.Add("links", collectionPropertyDtoItem);

                return collectionPropertyDtoItemAsDictionary;
            }).ToList();

            var linkedCollectionResource = new WebApiListResponseDto<IDictionary<string, object>>
            {
                Value = shapedDataWithLinks
                ,
                Links = links
            };

            return Ok(linkedCollectionResource);
        }

        private async Task<IActionResult> GetCollectionItem(string id, string collection, [FromQuery] string fields)
        {
            if (!RelationshipHelper.IsValidCollectionExpression(collection, typeof(TDto)))
            {
                return BadRequest(Messages.CollectionInvalid);
            }

            if (!RelationshipHelper.IsCollectionExpressionCollectionItem(collection))
            {
                return BadRequest(Messages.CollectionInvalid);
            }

            var collectionItemType = RelationshipHelper.GetCollectionExpressionType(collection, typeof(TDto));
            if (!TypeHelperService.TypeHasProperties(collectionItemType, fields))
            {
                return BadRequest(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, "", null, false, null, null);

            var collectionItem = RelationshipHelper.GetCollectionExpressionData(collection, typeof(TDto), response);

            if (collectionItem == null)
            {
                return NotFound();
            }

            var links = CreateLinksForCollectionItem(id, collection, fields);

            var linkedResourceToReturn = collectionItem.ShapeData(collectionItemType, fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }
        #endregion

        #region HATEOAS
        private string CreateResourceUri(
WebApiPagedSearchOrderingRequestDto resourceParameters,
ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Action(nameof(Search),
                          Url.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          fields = resourceParameters.Fields,
                          userId = resourceParameters.UserId,
                          orderBy = resourceParameters.OrderBy,
                          search = resourceParameters.Search,
                          page = resourceParameters.Page - 1,
                          pageSize = resourceParameters.PageSize
                      },
                      Url.ActionContext.HttpContext.Request.Scheme);
                case ResourceUriType.NextPage:
                    return Url.Action(nameof(Search),
                          Url.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          fields = resourceParameters.Fields,
                          userId = resourceParameters.UserId,
                          orderBy = resourceParameters.OrderBy,
                          search = resourceParameters.Search,
                          page = resourceParameters.Page + 1,
                          pageSize = resourceParameters.PageSize
                      },
                      Url.ActionContext.HttpContext.Request.Scheme);

                default:
                    return Url.Action(nameof(Search),
                    Url.ActionContext.RouteData.Values["controller"].ToString(),
                    new
                    {
                        fields = resourceParameters.Fields,
                        userId = resourceParameters.UserId,
                        orderBy = resourceParameters.OrderBy,
                        search = resourceParameters.Search,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    },
                      Url.ActionContext.HttpContext.Request.Scheme);
            }
        }

        private string CreateCollectionPropertyResourceUri(
            string collection,
            WebApiPagedRequestDto resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Action(nameof(BulkGetByIds),
                          Url.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          collection = collection,
                          fields = resourceParameters.Fields,
                          page = resourceParameters.Page - 1,
                          pageSize = resourceParameters.PageSize
                      },
                      Url.ActionContext.HttpContext.Request.Scheme);
                case ResourceUriType.NextPage:
                    return Url.Action(nameof(BulkGetByIds),
                          Url.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          collection = collection,
                          fields = resourceParameters.Fields,
                          page = resourceParameters.Page + 1,
                          pageSize = resourceParameters.PageSize
                      },
                      Url.ActionContext.HttpContext.Request.Scheme);

                default:
                    return Url.Action(nameof(BulkGetByIds),
                    Url.ActionContext.RouteData.Values["controller"].ToString(),
                    new
                    {
                        collection = collection,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    },
                      Url.ActionContext.HttpContext.Request.Scheme);
            }
        }

        protected IEnumerable<LinkDto> CreateLinksForCreate()
        {
            var links = new List<LinkDto>();

            links.Add(
           new LinkDto(Url.Action("Create", Url.ActionContext.RouteData.Values["controller"].ToString(), Url.ActionContext.HttpContext.Request.Scheme),
           "create",
           HttpMethod.Post.Method));

            return links;
        }

        protected IEnumerable<LinkDto> CreateLinks(string id, string fields, bool fullGraph = false)
        {
            var links = new List<LinkDto>();

            string action = nameof(GetById);
            if(fullGraph)
            {
                action = nameof(GetByIdFullGraph);
            }

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(Url.Action(action, Url.ActionContext.RouteData.Values["controller"].ToString(), new { id = id }, Url.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  HttpMethod.Get.Method));
            }
            else
            {
                links.Add(
                  new LinkDto(Url.Action(action, Url.ActionContext.RouteData.Values["controller"].ToString(), new { id = id, fields = fields }, Url.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  HttpMethod.Get.Method));
            }

            links.Add(
              new LinkDto(Url.Action("Delete", Url.ActionContext.RouteData.Values["controller"].ToString(), new { id = id }, Url.ActionContext.HttpContext.Request.Scheme),
              "delete",
              HttpMethod.Delete.Method));

            links.Add(
                new LinkDto(Url.Action("Update", Url.ActionContext.RouteData.Values["controller"].ToString(),
                new { id = id }, Url.ActionContext.HttpContext.Request.Scheme),
                "update",
                 HttpMethod.Put.Method));

            links.Add(
                new LinkDto(Url.Action("UpdatePartial", Url.ActionContext.RouteData.Values["controller"].ToString(),
                new { id = id }, Url.ActionContext.HttpContext.Request.Scheme),
                "partially_update",
                new HttpMethod("PATCH").Method));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCollectionItem(string id, string collection, string fields)
        {
            var links = new List<LinkDto>();

            //Create links for Collection Item Get, Delete and Update. Not sure if we want to allow 

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(Url.Action(nameof(BulkGetByIds), Url.ActionContext.RouteData.Values["controller"].ToString(), new {  id = id, collection = collection }, Url.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  HttpMethod.Get.Method));
            }
            else
            {
                links.Add(
                  new LinkDto(Url.Action(nameof(BulkGetByIds), Url.ActionContext.RouteData.Values["controller"].ToString(), new { id = id, collection = collection, fields = fields }, Url.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  HttpMethod.Get.Method));
            }

            //Create links for Collection Item Delete and Update. Not sure if we want to allow this.

            //links.Add(
            //  new LinkDto(Url.Action("Delete", Url.ActionContext.RouteData.Values["controller"].ToString(), new { id = id }, Url.ActionContext.HttpContext.Request.Scheme),
            //  "delete",
            //  "DELETE"));

            //links.Add(
            //    new LinkDto(Url.Action("Update", Url.ActionContext.RouteData.Values["controller"].ToString(),
            //    new { id = id }, Url.ActionContext.HttpContext.Request.Scheme),
            //    "update",
            //    "PUT"));

            //links.Add(
            //    new LinkDto(Url.Action("UpdatePartial", Url.ActionContext.RouteData.Values["controller"].ToString(),
            //    new { id = id }, Url.ActionContext.HttpContext.Request.Scheme),
            //    "partially_update",
            //    "PATCH"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCollections(WebApiPagedSearchOrderingRequestDto resourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateResourceUri(resourceParameters,
               ResourceUriType.Current)
               , "self", HttpMethod.Get.Method));

            links.Add(
           new LinkDto(Url.Action("Create", Url.ActionContext.RouteData.Values["controller"].ToString(),
          null, Url.ActionContext.HttpContext.Request.Scheme),
           "add",
           HttpMethod.Post.Method));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateResourceUri(resourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", HttpMethod.Get.Method));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", HttpMethod.Get.Method));
            }

            return links;
        }

        /// <summary>
        /// Creates the links for collection property.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="resourceParameters">The resource parameters.</param>
        /// <param name="hasNext">if set to <c>true</c> [has next].</param>
        /// <param name="hasPrevious">if set to <c>true</c> [has previous].</param>
        /// <returns></returns>
        private List<LinkDto> CreateLinksForCollectionProperty(string collection, WebApiPagedRequestDto resourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateCollectionPropertyResourceUri(collection, resourceParameters,
               ResourceUriType.Current)
               , "self", HttpMethod.Get.Method));

            //Todo if want to allow Add to collection property
          //  links.Add(
          // new LinkDto(Url.Action("Create", Url.ActionContext.RouteData.Values["controller"].ToString(),
          //null, Url.ActionContext.HttpContext.Request.Scheme),
          // "add",
          // "POST"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateCollectionPropertyResourceUri(collection, resourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", HttpMethod.Get.Method));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateCollectionPropertyResourceUri(collection, resourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", HttpMethod.Get.Method));
            }

            return links;
        }
        #endregion
    }
}

