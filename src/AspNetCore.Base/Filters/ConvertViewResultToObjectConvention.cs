using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AspNetCore.Base.Filters
{
    public class ConvertViewResultToObjectConvention : IControllerModelConvention, IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                Apply(controller);
            }
        }

        public void Apply(ControllerModel controller)
        {
            controller.Filters.Add(new ConvertViewResultToObjectResultAttribute());
        }
    }
}
