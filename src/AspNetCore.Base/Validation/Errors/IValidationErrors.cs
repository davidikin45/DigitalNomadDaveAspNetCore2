using System.Collections.Generic;

namespace AspnetCore.Base.Validation.Errors
{
    public interface IValidationErrors
    {
        List<IError> Errors { get; set; }
    }
}
