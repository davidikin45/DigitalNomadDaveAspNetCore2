using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.ErrorHandling
{
    public class AngularFormattedValidationError
    {
        public string ValidatorKey { get; private set; }
        public string Message { get; private set; }

        public AngularFormattedValidationError(string message, string validatorKey = "")
        {
            ValidatorKey = validatorKey;
            Message = message;
        }
    }
}
