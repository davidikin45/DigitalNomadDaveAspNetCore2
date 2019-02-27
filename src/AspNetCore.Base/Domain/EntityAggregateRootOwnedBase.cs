using System;

namespace AspNetCore.Base.Domain
{
    public abstract class EntityAggregateRootOwnedBase<T> : EntityAggregateRootBase<T>, IEntityOwned where T : IEquatable<T>
    {
        public string OwnedBy { get; set; }
    }
}
