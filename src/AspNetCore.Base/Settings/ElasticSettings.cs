namespace AspNetCore.Base.Settings
{
    public class ElasticSettings
    {
        public string Uri { get; set; }
        public string DefaultIndex { get; set; } = "default";
        public string DefaultTypeName { get; set; } = "doc";
        public bool Log { get; set; } = false;
    }
}
