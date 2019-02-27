using System.Collections.Generic;

namespace AspNetCore.Base.Dtos
{
    public class WebApiListResponseDto<TDto>
    {
        public List<TDto> Value { get; set; }
        public List<LinkDto> Links { get; set; }
    }
}
