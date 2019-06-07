using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore.Base.ModelBinders
{
    public class RawStringRequestBodyInputFormatter : InputFormatter
    {
        public RawStringRequestBodyInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;

            using (var reader = new StreamReader(request.Body))
            {
                var content = await reader.ReadToEndAsync();
                return await InputFormatterResult.SuccessAsync(content);
            }
        }

        public override bool CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            if (string.IsNullOrEmpty(contentType) || contentType == "text/plain" || contentType == "application/json")
                return true;

            return false;
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }
    }

    public class RawBytesRequestBodyInputFormatter : InputFormatter
    {
        public RawBytesRequestBodyInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;

            using (var ms = new MemoryStream(2048))
            {
                await request.Body.CopyToAsync(ms);
                var content = ms.ToArray();
                return await InputFormatterResult.SuccessAsync(content);
            }
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(byte[]);
        }
    }
}
