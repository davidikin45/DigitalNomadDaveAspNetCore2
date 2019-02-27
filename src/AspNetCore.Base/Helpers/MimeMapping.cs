using Microsoft.AspNetCore.StaticFiles;

namespace AspNetCore.Base.Helpers
{
    public static class MimeMapping
    {
        public static string GetMimeMapping(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
