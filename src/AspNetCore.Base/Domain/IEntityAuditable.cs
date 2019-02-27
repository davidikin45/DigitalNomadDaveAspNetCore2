using System;

namespace AspNetCore.Base.Domain
{
    public interface IEntityAuditable
    {
        DateTime CreatedOn { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
        string UpdatedBy { get; set; }
    }
}
