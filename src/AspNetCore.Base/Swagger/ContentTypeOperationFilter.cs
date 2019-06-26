using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore.Base.Swagger
{
    //Need to also add 
    //[ApiExplorerSettings(IgnoreApi = true)]
    //[Produces("Accept")] or [Consumes("ContentType")]
    public abstract class ContentTypeOperationFilter<TSchemaType> : IOperationFilter
    {
        private readonly string _operationId;
        private readonly string _contentTypeHeader;

        public ContentTypeOperationFilter(string operationId, string contentTypeHeader)
        {
            _operationId = operationId;
            _contentTypeHeader = contentTypeHeader;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if(operation.OperationId != _operationId)
            {
                return;
            }

            operation.RequestBody.Content.Add(
                _contentTypeHeader,
                new OpenApiMediaType()
                {
                    Schema = context.SchemaGenerator.GenerateSchema(typeof(TSchemaType), context.SchemaRepository)
                });
        }
    }
}
