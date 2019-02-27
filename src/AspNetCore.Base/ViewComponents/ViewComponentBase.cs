using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace AspNetCore.Base.ViewComponents
{
    public abstract class ViewComponentBase : ViewComponent
    {
        protected CancellationToken ClientDisconnectedToken()
        {
            return HttpContext.RequestAborted;
        }
    }
}
