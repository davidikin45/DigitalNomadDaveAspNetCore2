using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.Authorization
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthorizationOptions _options;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, RoleManager<IdentityRole> roleManager) : base(options)
        {
            _options = options.Value;
            _roleManager = roleManager;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                bool allowAnonymousAccess = false;
                var role = await _roleManager.FindByNameAsync("anonymous");
                if (role != null)
                {
                    var roleScopes = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                    var scopes = policyName.Split(',').Select(p => p.Trim()).ToList();
                    if (roleScopes.Union(scopes).Count() > 0 ||
                        roleScopes.Contains(ResourceCollectionsCore.Admin.Scopes.Full))
                    {
                        allowAnonymousAccess = true;
                    }
                }

                if (allowAnonymousAccess)
                {
                    policy = new AuthorizationPolicyBuilder().AddRequirements(new AnonymousAuthorizationRequirement()).Build();
                }
                else
                {
                    //must have one or more to pass
                    var scopes = policyName.Split(',').Select(p => p.Trim()).ToList();

                    if (!scopes.Contains(ResourceCollectionsCore.Admin.Scopes.Full))
                        scopes.Add(ResourceCollectionsCore.Admin.Scopes.Full);

                    policy = new AuthorizationPolicyBuilder().RequireScope(scopes.ToArray()).Build();
                }

                // Add policy to the AuthorizationOptions, so we don't have to re-create it each time
                _options.AddPolicy(policyName, policy);
            }

            return policy;
        }

        public new Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return base.GetDefaultPolicyAsync();
        }
    }
}
