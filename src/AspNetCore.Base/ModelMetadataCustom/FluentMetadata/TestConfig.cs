namespace AspNetCore.Base.ModelMetadataCustom.FluentMetadata
{
    public class DynamicConfig : ModelMetadataConfiguration<dynamic>
    {
        public DynamicConfig()
        { 
            //Configure<string>("TenantName").Required();
        }
    }
}
