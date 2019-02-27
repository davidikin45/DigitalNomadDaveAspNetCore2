namespace AspNetCore.Base.Domain
{
    public interface IEntityConcurrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}
