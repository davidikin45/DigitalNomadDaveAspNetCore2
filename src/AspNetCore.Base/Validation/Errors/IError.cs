namespace AspnetCore.Base.Validation.Errors
{
    public interface IError
    {
        string PropertyName { get; }
        string PropertyExceptionMessage { get; }
    }
}
