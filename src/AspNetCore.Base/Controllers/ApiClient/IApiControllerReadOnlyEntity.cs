using AspNetCore.Base.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.ApiClient
{
    public interface IApiControllerEntityReadOnly<TReadDto>
        where TReadDto : class
    {
        Task<ActionResult<WebApiListResponseDto<TReadDto>>> Search(WebApiPagedSearchOrderingRequestDto resourceParameters);

        Task<ActionResult<List<TReadDto>>> GetAll();
        Task<ActionResult<List<TReadDto>>> GetAllPaged();

        Task<ActionResult<TReadDto>> GetById(string id, WebApiParamsDto parameters);
        Task<ActionResult<TReadDto>> GetByIdFullGraph(string id, WebApiParamsDto parameters);

        Task<ActionResult<List<TReadDto>>> BulkGetByIds(IEnumerable<string> ids);

        Task<IActionResult> GetByIdChildCollection(string id, string collection, WebApiPagedSearchOrderingRequestDto resourceParameters);
    }

    public interface IApiControllerEntityReadOnlyClient<TReadDto>
         where TReadDto : class
    {
        string ResourceCollection { get; }

        Task<(WebApiListResponseDto<TReadDto> data, PagingInfoDto pagingInfo)> SearchAsync(WebApiPagedSearchOrderingRequestDto resourceParameters, CancellationToken cancellationToken);

        Task<List<TReadDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<TReadDto>> GetAllPagedAsync(CancellationToken cancellationToken);

        Task<TReadDto> GetByIdAsync(object id, WebApiParamsDto parameters, CancellationToken cancellationToken);
        Task<TReadDto> GetByIdFullGraphAsync(object id, WebApiParamsDto parameters, CancellationToken cancellationToken);

        Task<List<TReadDto>> BulkGetByIdsAsync(IEnumerable<object> ids, CancellationToken cancellationToken);

        Task<(WebApiListResponseDto<TCollectionItemDto> data, PagingInfoDto pagingInfo)> GetByIdChildCollectionAsync<TCollectionItemDto>(object id, string collection, WebApiPagedSearchOrderingRequestDto resourceParameters, CancellationToken cancellationToken) where TCollectionItemDto : class;
        Task<TCollectionItemDto> GetByIdChildCollectionItemAsync<TCollectionItemDto>(object id, string collection, string collectionItemId, CancellationToken cancellationToken) where TCollectionItemDto : class;
    }
}
