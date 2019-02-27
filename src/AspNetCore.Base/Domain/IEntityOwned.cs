namespace AspNetCore.Base.Domain
{
    public interface IEntityOwned
    {
        string OwnedBy { get; set; }
    }
}
