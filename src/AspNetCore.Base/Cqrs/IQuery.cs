using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs
{
    public abstract class UserQuery<TResult> : IQuery<TResult>
    {
        public string User { get; }

        public UserQuery(string user)
        {
            User = user;
        }
    }

    public interface IQuery<TResult>
    {
    }
}
