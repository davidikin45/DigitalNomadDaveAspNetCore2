using System;

namespace AspNetCore.Base.Domain
{
    public interface IEntitySoftDelete
    {
       bool IsDeleted { get; set; }
       DateTime? DeletedOn { get; set; }
       string DeletedBy { get; set; }
    }
}
