using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AspNetCore.Base.DomainEvents
{
    public class DomainEventMessage
    {
        public DomainEventMessage(string serverName, IDomainEvent domainEvent)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            EventName = domainEvent.GetType().Name; //ClassName
            Message = JsonConvert.SerializeObject(domainEvent);
            Body = Encoding.UTF8.GetBytes(Message);
            DomainEvent = domainEvent;
            DomainEventTypeExists = true;
        }

        [JsonConstructor]
        public DomainEventMessage(string serverName, Guid id, DateTime createDate, string eventName, byte[] body)
        {
            ServerName = serverName;
            Id = id;
            CreationDate = createDate;
            EventName = eventName;
            Body = body;
            Message = Encoding.UTF8.GetString(body);
            var types = EventAsType(EventName);
            if(types.Length > 1)
            {
                throw new Exception($"More than one type with name: {EventName}");
            }

            if (types.Length == 1)
            {
                DomainEvent = (IDomainEvent)JsonConvert.DeserializeObject(Message, types[0]);
                DomainEventTypeExists = true;
            }
        }

        [JsonProperty]
        public string ServerName { get; private set; }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        [JsonProperty]
        public string EventName { get; private set; }

        [JsonProperty]
        public byte[] Body { get; private set; }

        [JsonIgnore]
        public string Message { get; private set; }

        public dynamic DomainEventAsDynamic()
        {
            dynamic domainEvent = JObject.Parse(Message);
            return domainEvent;
        }

        [JsonIgnore]
        public IDomainEvent DomainEvent { get; private set; }

        [JsonIgnore]
        public bool DomainEventTypeExists { get; private set; }

        private Type[] EventAsType(string className)
        {
            List<Type> returnVal = new List<Type>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = a.GetTypes();
                for (int j = 0; j < assemblyTypes.Length; j++)
                {
                    if (assemblyTypes[j].Name == className)
                    {
                        returnVal.Add(assemblyTypes[j]);
                    }
                }
            }

            return returnVal.ToArray();
        }
    }
}
