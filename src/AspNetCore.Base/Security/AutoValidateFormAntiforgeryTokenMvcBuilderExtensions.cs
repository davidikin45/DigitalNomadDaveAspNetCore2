using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.Security
{
    public static class AutoValidateFormAntiforgeryTokenMvcBuilderExtensions
    {
        public static IMvcBuilder AddAutoValidateFormAntiforgeryTokenService(this IMvcBuilder builder)
        {
            builder.Services.AddSingleton<AutoValidateFormAntiforgeryTokenAuthorizationFilter>();
            return builder;
        }
    }
}
