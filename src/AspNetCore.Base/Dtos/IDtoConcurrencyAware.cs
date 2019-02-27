using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.Dtos
{
    public interface IDtoConcurrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}
