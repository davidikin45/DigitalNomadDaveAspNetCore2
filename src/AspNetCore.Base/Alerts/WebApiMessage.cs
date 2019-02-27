using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AspNetCore.Base.Alerts
{
    [DataContract]
    public class WebApiMessage
    {
        public static WebApiMessage CreateWebApiMessage(string message, IList<string> errors, Object id)
        {
            return new WebApiMessage(message, errors.ToArray(), id);
        }

        private WebApiMessage(string message, string[] errors, Object id)
        {
            this.Message = message;
            this.Errors = errors;
            this.Id = id;
            TimeGenerated = DateTime.UtcNow;
        }

        public static WebApiMessage CreateWebApiMessage(string message, IList<string> errors)
        {
            return new WebApiMessage(message, errors.ToArray());
        }

        private WebApiMessage()
        {
            TimeGenerated = DateTime.UtcNow;
        }

        private WebApiMessage(string message, string[] errors)
        {
            this.Message = message;
            this.Errors = errors;
            TimeGenerated = DateTime.UtcNow;
        }

        public static WebApiMessage CreateWebApiMessage(string message, IList<string> errors, ModelStateDictionary modelState)
        {
            return new WebApiMessage(message, errors.ToArray(), modelState);
        }

        private WebApiMessage(string message, string[] errors, ModelStateDictionary modelState)
        {
            this.Message = message;
            this.Errors = errors;

            this.ModelState = new SerializableDictionary<string, List<AngularFormattedValidationError>>();

            if (modelState != null)
            {

                foreach (KeyValuePair<string, ModelStateEntry> property in modelState)
                {
                    var propertyMessages = new List<AngularFormattedValidationError>();
                    foreach (ModelError modelError in property.Value.Errors)
                    {
                        var keyAndMessage = modelError.ErrorMessage.Split('|');
                        if (keyAndMessage.Count() > 1)
                        {
                            //Formatted for Angular Binding
                            //e.g required|Error Message
                            propertyMessages.Add(new AngularFormattedValidationError(
                                keyAndMessage[1],
                                keyAndMessage[0]));
                        }
                        else
                        {
                            propertyMessages.Add(new AngularFormattedValidationError(
                                keyAndMessage[0]));
                        }
                    }

                    this.ModelState.Add(property.Key, propertyMessages);
                }

            }
        }

        [DataMember]
        public DateTime TimeGenerated { get; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object Id { get; set; }

        [DataMember]
        public Boolean Success
        {
            get
            {
                return Errors.Count() == 0;
            }
            set { }
        }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Error
        {
            get
            {
                return string.Join("\n", Errors);
            }
            set { }
        }

        [DataMember]
        public string[] Errors { get; set; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SerializableDictionary<string, List<AngularFormattedValidationError>> ModelState { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class AngularFormattedValidationError
    {
        public string ValidatorKey { get; private set; }
        public string Message { get; private set; }

        public AngularFormattedValidationError(string message, string validatorKey = "")
        {
            ValidatorKey = validatorKey;
            Message = message;
        }
    }
}
