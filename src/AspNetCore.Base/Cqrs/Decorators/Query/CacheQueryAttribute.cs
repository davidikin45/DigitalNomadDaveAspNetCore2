using System;

namespace AspNetCore.Base.Cqrs.Decorators.Command
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CacheQueryAttribute : Attribute
    {
        public CacheQueryAttribute()
        {
        }
    }
}
