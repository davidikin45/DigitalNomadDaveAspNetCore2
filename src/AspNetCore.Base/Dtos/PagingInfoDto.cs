using Newtonsoft.Json;
using System;

namespace AspNetCore.Base.Dtos
{
    public class PagingInfoDto
    {
        public int Page
        { get; set; }

        public int PageSize
        { get; set; }

        [JsonIgnore]
        public int TotalItems
        {
            get { return Records; }
            set { Records = value; }
        }

        public int Records
        { get; set; }

        public int Total
        {
            get
            {
                return Pages;
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Search
        { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OrderColumn
        { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OrderType
        { get; set; }

        public string PreviousPageLink
        { get; set; }

        public string NextPageLink
        { get; set; }

        public bool HasPrevious
        {
            get
            {
                return (Page > 1);
            }
        }

        public bool HasNext
        {
            get
            {
                return (Page < Total);
            }
        }


        [JsonIgnore]
        public int Pages
        {
            get
            {
                return Math.Max((int)Math.Ceiling(Convert.ToDouble(Records) / PageSize), 0);
            }
        }
    }
}
