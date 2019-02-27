using AspNetCore.Base;
using System.Threading.Tasks;

namespace DND.Web
{
    public class Program : ProgramSingleTenantBase<Startup>
    {
        public async static Task<int> Main(string[] args)
        {
            return await RunApp(args);
        }
    }
}