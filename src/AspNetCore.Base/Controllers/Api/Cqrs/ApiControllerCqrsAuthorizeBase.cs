using AspNetCore.Base.Authentication;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Cqrs;
using AspNetCore.Base.Email;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Settings;
using AspNetCore.Base.Validation;
using AspNetCore.Cqrs;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + BasicAuthenticationDefaults.AuthenticationScheme)]
    public abstract class ApiControllerCqrsAuthorizeBase : ApiControllerBase
    {
        protected readonly ICqrsMediator _cqrsMediator;

        public ApiControllerCqrsAuthorizeBase(ICqrsMediator cqrsMediator, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, AppSettings appSetings)
        : base(mapper, emailService, linkGenerator, appSetings)
        {
            _cqrsMediator = cqrsMediator;
        }

        #region Queries
        [CqrsAuthorize]
        [Route("queries/{type}")]
        [HttpGet]
        public virtual async Task<IActionResult> QueryFromRoute([FromRoute] string type)
        {
            var payload = Request.Query;

            var action = new ActionDto()
            {
                Type = type,
                Payload = ToDynamic(payload)
            };

            return await QueryFromJson(action);
        }

        private dynamic ToDynamic(IQueryCollection collection)
        {
            ExpandoObject expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            var keys = collection.Keys;

            foreach (var item in keys.ToDictionary(key => key, value => collection[value]))
            {
                dictionary.Add(item.Key, item.Value);
            }

            return expando;
        }

        [CqrsAuthorize]
        [Route("queries")]
        [HttpPost]
        public virtual async Task<IActionResult> QueryFromJson([FromBody] ActionDto action)
        {
            if (action == null)
            {
                return Error(Messages.RequestInvalid);
            }

            bool isQuery = false;
            if (_cqrsMediator.CqrsQuerySubscriptionManager.HasSubscriptionsForQuery(action.Type))
            {
                isQuery = true;
            }

            if (string.IsNullOrWhiteSpace(action.Type) || (!isQuery))
            {
                return Error(Messages.ActionInvalid);
            }

            if (!ModelState.IsValid)
            {
                return ValidationErrors(ModelState);
            }

            Type payloadType;
            CancellationTokenSource cts;

            cts = TaskHelper.CreateLinkedCancellationTokenSource(ClientDisconnectedToken());
            payloadType = _cqrsMediator.CqrsQuerySubscriptionManager.GetQueryTypeByName(action.Type);

            //query
            if (payloadType != null)
            {

                string payloadString = JsonConvert.SerializeObject(action.Payload);
                dynamic typedPayload = JsonConvert.DeserializeObject(payloadString, payloadType);

                var actionForValidation = new ActionDto<dynamic>()
                {
                    Type = action.Type,
                    Payload = typedPayload
                };

                if (!TryValidateModel(actionForValidation))
                {
                    return ValidationErrors(ModelState);
                }

                dynamic result = await _cqrsMediator.DispatchAsync(typedPayload, cts.Token);

                return Ok(result);
            }
            else
            {
                //dynamic query
                string payloadString = JsonConvert.SerializeObject(action.Payload);
                dynamic result = await _cqrsMediator.DispatchQueryAsync(action.Type, payloadString, cts.Token);
                return Ok(result);
            }
        }

        [AllowAnonymous]
        [Route("queries")]
        [HttpGet]
        public virtual IActionResult Queries()
        {
            return Ok(_cqrsMediator.CqrsQuerySubscriptionManager.GetSubscriptions().OrderBy(i => i.Key).Select(c => new { Type = c.Key, Payload = c.Value.QueryType != null ? Activator.CreateInstance(c.Value.QueryType) : null, Return = c.Value.ReturnType != null ? Activator.CreateInstance(c.Value.ReturnType) : null }));
        }
        #endregion

        #region Commands
        [CqrsAuthorize]
        [Route("commands")]
        [HttpPost]
        public virtual async Task<IActionResult> Command([FromBody] ActionDto action)
        {
            if (action == null)
            {
                return Error(Messages.RequestInvalid);
            }

            bool isCommand = false;
            if (_cqrsMediator.CqrsCommandSubscriptionManager.HasSubscriptionsForCommand(action.Type))
            {
                isCommand = true;
            }

            if (string.IsNullOrWhiteSpace(action.Type) || (!isCommand))
            {
                return Error(Messages.ActionInvalid);
            }

            if (!ModelState.IsValid)
            {
                return ValidationErrors(ModelState);
            }

            Type payloadType;
            CancellationTokenSource cts;

            cts = TaskHelper.CreateNewCancellationTokenSource();
            payloadType = _cqrsMediator.CqrsCommandSubscriptionManager.GetCommandTypeByName(action.Type);

            //command
            if (payloadType != null)
            {
                string payloadString = JsonConvert.SerializeObject(action.Payload);
                dynamic typedPayload = JsonConvert.DeserializeObject(payloadString, payloadType);

                var actionForValidation = new ActionDto()
                {
                    Type = action.Type,
                    Payload = typedPayload
                };

                if (!TryValidateModel(actionForValidation))
                {
                    return ValidationErrors(ModelState);
                }

                dynamic result = await _cqrsMediator.DispatchAsync(typedPayload, cts.Token);

                return FromResult(result);
            }
            else
            {
                //dynamic command
                string payloadString = JsonConvert.SerializeObject(action.Payload);
                Result<dynamic> result = await _cqrsMediator.DispatchCommandAsync(action.Type, payloadString, cts.Token);
                return FromResult(result);
            }
        }

        [AllowAnonymous]
        [Route("commands")]
        [HttpGet]
        public virtual IActionResult Commands()
        {
            return Ok(_cqrsMediator.CqrsCommandSubscriptionManager.GetSubscriptions().OrderBy(i => i.Key).Select(c => new { Type = c.Key, Payload = c.Value.CommandType != null ? Activator.CreateInstance(c.Value.CommandType) : null, Return = c.Value.ReturnType != null ? Activator.CreateInstance(c.Value.ReturnType) : null }));
        }
        #endregion
    }
}

