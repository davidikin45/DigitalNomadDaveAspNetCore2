﻿using AspNetCore.Base.Validation;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs
{
    public abstract class UserCommand<T> : ICommand<T>
    {
        public string User { get; }

        public UserCommand(string user)
        {
            User = user;
        }
    }

    public interface ICommand<TResult>
    {

    }

    public interface ICommand : ICommand<string>
    {

    }
}
