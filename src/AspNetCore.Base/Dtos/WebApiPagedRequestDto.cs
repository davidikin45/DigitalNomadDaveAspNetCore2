namespace AspNetCore.Base.Dtos
{
    public class WebApiPagedRequestDto
    {
        // no. of records to fetch
        public int? PageSize
        { get; set; }

        // the page index
        public int? Page
        { get; set; }

        public string Fields
        { get; set; }

    }
}
