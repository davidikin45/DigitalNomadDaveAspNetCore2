using AspnetCore.Base.Data.Initializers;
using System.Threading.Tasks;

namespace DND.Data.Initializers
{
    public class AppContextInitializerDropCreate : ContextInitializerDropCreate<AppContext>
    {
        public override void Seed(AppContext context, string tenantId)
        {
            context.Seed();
        }

        public override Task OnSeedCompleteAsync(AppContext context)
        {
            return Task.CompletedTask; 
        }
    }
}
