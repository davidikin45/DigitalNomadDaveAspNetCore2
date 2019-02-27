using Microsoft.Azure.Search.Models;
using System.Threading.Tasks;

namespace AspNetCore.Base.AzureStorage
{
    public interface IAzureSearch
    {
        Task<DocumentSearchResult> SearchAsync(string searchTerm);
    }
}
