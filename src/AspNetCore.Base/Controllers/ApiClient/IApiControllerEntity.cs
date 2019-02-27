using AspNetCore.Base.Alerts;
using AspNetCore.Base.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.ApiClient
{
    public interface IApiControllerEntity<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> : IApiControllerEntityReadOnly<TReadDto>
        where TCreateDto : class
        where TReadDto : class
        where TUpdateDto : class
        where TDeleteDto : class
    {
        ActionResult<TCreateDto> NewDefault();

        Task<ActionResult<TReadDto>> Create(TCreateDto dto);
        Task<ActionResult<List<WebApiMessage>>> BulkCreate(TCreateDto[] dtos);

        Task<ActionResult<TUpdateDto>> GetByIdForEdit(string id);
        Task<ActionResult<List<TUpdateDto>>> BulkGetByIdsForEditAsync(IEnumerable<string> ids);

        Task<IActionResult> Update(string id, TUpdateDto dto);
        Task<ActionResult<List<WebApiMessage>>> BulkUpdate(BulkDto<TUpdateDto>[] dtos);

        Task<IActionResult> UpdatePartial(string id, JsonPatchDocument dtoPatch);
        Task<ActionResult<List<WebApiMessage>>> BulkUpdatePartial(BulkDto<JsonPatchDocument>[] dtos);

        Task<ActionResult<TDeleteDto>> GetByIdForDelete(string id);
        Task<ActionResult<List<TDeleteDto>>> BulkGetByIdsForDeleteAsync(IEnumerable<string> ids);

        Task<IActionResult> DeleteDto(string id, [FromBody] TDeleteDto dto);
        Task<ActionResult<List<WebApiMessage>>> BulkDelete([FromBody] TDeleteDto[] dtos);

        IActionResult NewCollectionItem(string collection);
    }

    public interface IApiControllerEntityClient<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> : IApiControllerEntityReadOnlyClient<TReadDto>
        where TCreateDto : class
        where TReadDto : class
        where TUpdateDto : class
        where TDeleteDto : class
    {
        Task<TCreateDto> NewDefaultAsync(CancellationToken cancellationToken);

        Task<TReadDto> CreateAsync(TCreateDto dto);
        Task<List<WebApiMessage>> BulkCreateAsync(TCreateDto[] dtos);

        Task<TUpdateDto> GetByIdForEditAsync(object id, CancellationToken cancellationToken);
        Task<List<TUpdateDto>> BulkGetByIdsForEditAsync(IEnumerable<object> ids);

        Task UpdateAsync(object id, TUpdateDto dto);
        Task<List<WebApiMessage>> BulkUpdateAsync(BulkDto<TUpdateDto>[] dtos);

        Task UpdatePartialAsync(object id, JsonPatchDocument dtoPatch);
        Task<List<WebApiMessage>> BulkUpdatePartialAsync(BulkDto<JsonPatchDocument>[] dtos);

        Task<TDeleteDto> GetByIdForDeleteAsync(object id, CancellationToken cancellationToken);
        Task<List<TDeleteDto>> BulkGetByIdsForDeleteAsync(IEnumerable<object> ids);

        Task DeleteAsync(object id, TDeleteDto dto);
        Task<List<WebApiMessage>> BulkDeleteAsync(TDeleteDto[] dtos);

        Task<CollectionItemTypeDto> NewCollectionItemAsync<CollectionItemTypeDto>(string collection, CancellationToken cancellationToken);
    }
}
