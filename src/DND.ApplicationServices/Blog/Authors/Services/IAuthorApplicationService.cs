using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.Blog.Authors.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Authors.Services
{
    public interface IAuthorApplicationService : IApplicationServiceEntity<AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto>
    {
        Task<AuthorDto> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken);
    }
}
