using AspNetCore.Base.Security;
using AspNetCore.Base.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AspNetCore.Base.Extensions
{
    public static class ServiceCollectionAuthExtensions
    {
        public static AuthenticationBuilder AddJwtAuthentication(this AuthenticationBuilder authenticationBuilder,
           string bearerTokenKey,
           string bearerTokenPublicSigningKeyPath,
           string bearerTokenPublicSigningCertificatePath,
           string bearerTokenExternalIssuers,
           string bearerTokenLocalIssuer,
           string bearerTokenAudiences)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // keep original claim types

            var signingKeys = new List<SecurityKey>();
            if (!String.IsNullOrWhiteSpace(bearerTokenKey))
            {
                //Symmetric
                signingKeys.Add(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(bearerTokenKey)));
            }

            if (!String.IsNullOrWhiteSpace(bearerTokenPublicSigningKeyPath))
            {
                //Assymetric
                signingKeys.Add(SigningKey.LoadPublicRsaSigningKey(bearerTokenPublicSigningKeyPath));
            }

            if (!String.IsNullOrWhiteSpace(bearerTokenPublicSigningCertificatePath))
            {
                //Assymetric
                signingKeys.Add(SigningKey.LoadPublicSigningCertificate(bearerTokenPublicSigningCertificatePath));
            }

            var validIssuers = new List<string>();
            if (!string.IsNullOrEmpty(bearerTokenExternalIssuers))
            {
                foreach (var externalIssuer in bearerTokenExternalIssuers.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(externalIssuer))
                    {
                        validIssuers.Add(externalIssuer);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(bearerTokenLocalIssuer))
            {
                validIssuers.Add(bearerTokenLocalIssuer);
            }

            var validAudiences = new List<string>();
            foreach (var audience in bearerTokenAudiences.Split(','))
            {
                if (!string.IsNullOrWhiteSpace(audience))
                {
                    validAudiences.Add(audience);
                }
            }

            //https://developer.okta.com/blog/2018/03/23/token-authentication-aspnetcore-complete-guide
            return authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, cfg =>
            {
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    // Specify what in the JWT needs to be checked 
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,

                    ValidIssuers = validIssuers, //in the JWT this is the uri of the Identity Provider which issues the token.
                    ValidAudiences = validAudiences, //in the JWT this is aud. This is the resource the user is expected to have.

                    IssuerSigningKeys = signingKeys
                };
            });
        }

        public static void AddCookiePolicy(this IServiceCollection services, string cookieConsentName)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.ConsentCookie.Name = cookieConsentName;
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        public static void AddIdentity<TContext, TUser, TRole>(this IServiceCollection services,
        int maxFailedAccessAttemptsBeforeLockout,
        int lockoutMinutes,
        bool requireDigit,
        int requiredLength,
        int requiredUniqueChars,
        bool requireLowercase,
        bool requireNonAlphanumeric,
        bool requireUppercase,

        //user
        bool requireConfirmedEmail,
        bool requireUniqueEmail,
        int registrationEmailConfirmationExprireDays,
        int forgotPasswordEmailConfirmationExpireHours,
        int userDetailsChangeLogoutMinutes)
            where TContext : DbContext
            where TUser : class
            where TRole : class
        {
            services.AddIdentity<TUser, TRole>(options =>
            {
                options.Password.RequireDigit = requireDigit;
                options.Password.RequiredLength = requiredLength;
                options.Password.RequiredUniqueChars = requiredUniqueChars;
                options.Password.RequireLowercase = requireLowercase;
                options.Password.RequireNonAlphanumeric = requireNonAlphanumeric;
                options.Password.RequireUppercase = requireUppercase;
                options.User.RequireUniqueEmail = requireUniqueEmail;
                options.SignIn.RequireConfirmedEmail = requireConfirmedEmail;
                options.Tokens.EmailConfirmationTokenProvider = "emailconf";

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = maxFailedAccessAttemptsBeforeLockout;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutMinutes);
            })
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmationTokenProvider<TUser>>("emailconf")
                .AddPasswordValidator<DoesNotContainPasswordValidator<TUser>>();

            //registration email confirmation days
            services.Configure<EmailConfirmationTokenProviderOptions>(options =>
           options.TokenLifespan = TimeSpan.FromDays(registrationEmailConfirmationExprireDays));

            //forgot password hours
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(forgotPasswordEmailConfirmationExpireHours));

            //Security stamp validator validates every x minutes and will log out user if account is changed. e.g password change
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(userDetailsChangeLogoutMinutes);
            });
        }

        public static void AddIdentityCore<TContext, TUser, TRole>(this IServiceCollection services,
        int maxFailedAccessAttemptsBeforeLockout,
        int lockoutMinutes,
        bool requireDigit,
        int requiredLength,
        int requiredUniqueChars,
        bool requireLowercase,
        bool requireNonAlphanumeric,
        bool requireUppercase,

        //user
        bool requireConfirmedEmail,
        bool requireUniqueEmail,
        int registrationEmailConfirmationExprireDays,
        int forgotPasswordEmailConfirmationExpireHours,
        int userDetailsChangeLogoutMinutes)
            where TContext : DbContext
            where TUser : class
            where TRole : class
        {
           services.AddIdentityCore<TUser>(options =>
            {
                options.Password.RequireDigit = requireDigit;
                options.Password.RequiredLength = requiredLength;
                options.Password.RequiredUniqueChars = requiredUniqueChars;
                options.Password.RequireLowercase = requireLowercase;
                options.Password.RequireNonAlphanumeric = requireNonAlphanumeric;
                options.Password.RequireUppercase = requireUppercase;
                options.User.RequireUniqueEmail = requireUniqueEmail;
                options.SignIn.RequireConfirmedEmail = requireConfirmedEmail;
                options.Tokens.EmailConfirmationTokenProvider = "emailconf";

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = maxFailedAccessAttemptsBeforeLockout;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutMinutes);
            })
            .AddRoles<TRole>()
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmationTokenProvider<TUser>>("emailconf")
                .AddPasswordValidator<DoesNotContainPasswordValidator<TUser>>()
                .AddRoleValidator<RoleValidator<IdentityRole>>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddSignInManager<SignInManager<TUser>>();

            //registration email confirmation days
            services.Configure<EmailConfirmationTokenProviderOptions>(options =>
           options.TokenLifespan = TimeSpan.FromDays(registrationEmailConfirmationExprireDays));

            //forgot password hours
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(forgotPasswordEmailConfirmationExpireHours));

            //Security stamp validator validates every x minutes and will log out user if account is changed. e.g password change
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(userDetailsChangeLogoutMinutes);
            });
        }

        public static void ConfigureCorsAllowAnyOrigin(this IServiceCollection services, string name)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name,
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureCorsAllowSpecificOrigin(this IServiceCollection services, string name, params string[] domains)
        {
            services.AddCors(options =>
            {
                //https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.1
                options.AddPolicy(name,
                  builder => builder
                  .SetIsOriginAllowedToAllowWildcardSubdomains()
                  .WithOrigins(domains)
                  .AllowAnyMethod()
                  .AllowAnyHeader());
            });
        }
    }
}
