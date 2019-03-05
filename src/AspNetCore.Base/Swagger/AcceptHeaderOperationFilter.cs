using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore.Base.Swagger
{
    //Need to also add 
    //[ApiExplorerSettings(IgnoreApi = true)]
    //[Produces("Accept")] or [Consumes("ContentType")]
    public abstract class AcceptHeaderOperationFilter<TSchemaType> : IOperationFilter
    {
        private readonly string _operationId;
        private readonly string _acceptHeader;

        public AcceptHeaderOperationFilter(string operationId, string acceptHeader)
        {
            _operationId = operationId;
            _acceptHeader = acceptHeader;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if(operation.OperationId != _operationId)
            {

            }

            operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(_acceptHeader, new OpenApiMediaType() {
                Schema = context.SchemaRegistry.GetOrRegister(typeof(TSchemaType))
            });
        }
    }
}
