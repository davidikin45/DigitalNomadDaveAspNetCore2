﻿using AspNetCore.Base.Validation;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Base.DomainEvents
{
    public class DomainEvent
    {
        public DomainEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public DomainEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }
    }
}
