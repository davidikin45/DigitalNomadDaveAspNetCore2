using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EncryptedAttribute : Attribute
    { }
}
