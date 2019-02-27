using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.Blog.Tags.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Tags.Services
{
    public interface ITagApplicationService : IApplicationServiceEntity<TagDto, TagDto, TagDto, TagDeleteDto>
    {
        Task<TagDto> GetTagAsync(string tagSlug, CancellationToken cancellationToken);
    }
}
