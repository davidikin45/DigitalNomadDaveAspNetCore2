﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Base.Validation
{
    public interface IValidationService
    {
        bool IsValid(object o);
        IEnumerable<ValidationResult> ValidateObject(object o);
    }
}
