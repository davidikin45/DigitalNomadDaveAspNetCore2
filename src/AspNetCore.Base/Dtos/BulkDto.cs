using System;

namespace AspNetCore.Base.Dtos
{
    public class BulkDto<T> where T : class
    {
        public Object Id { get; set; }
        public T Value { get; set; }
    }
}
