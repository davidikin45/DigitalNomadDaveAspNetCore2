namespace AspNetCore.Base.Dtos
{
    public class WebApiPagedSearchOrderingRequestDto : WebApiPagedRequestDto
    {
        // sort column name
        public string OrderBy
        { get; set; }

        // sort order "asc" or "desc"
        public string OrderType
        { get; set; }

        public string Search
        { get; set; }

        public string UserId
        { get; set; }
    }
}
