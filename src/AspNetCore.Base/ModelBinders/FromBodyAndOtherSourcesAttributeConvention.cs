using AspNetCore.Base.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.Linq;

namespace AspNetCore.Base.ModelBinders
{
    public class FromBodyAndOtherSourcesAttributeConvention : IParameterModelConvention, IActionModelConvention, IControllerModelConvention, IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controler in application.Controllers)
            {
                Apply(controler);

                foreach (var action in controler.Actions)
                {
                    Apply(action);

                    foreach (var paramater in action.Parameters)
                    {
                        Apply(paramater);
                    }
                }
            }
        }

        public void Apply(ActionModel action)
        {
            var antiForgeryTokenFilters = action.Filters.Where(f => f is ValidateAntiForgeryTokenAttribute || f is AutoValidateAntiforgeryTokenAttribute).ToList();
            if(antiForgeryTokenFilters.Any())
            {
                antiForgeryTokenFilters.ForEach(af => action.Filters.Remove(af));
                action.Filters.Add(new AutoValidateFormAntiforgeryTokenAttribute());
            }
        }

        public void Apply(ControllerModel controller)
        {
            var antiForgeryTokenFilters = controller.Filters.Where(f => f is ValidateAntiForgeryTokenAttribute || f is AutoValidateAntiforgeryTokenAttribute).ToList();
            if (antiForgeryTokenFilters.Any())
            {
                antiForgeryTokenFilters.ForEach(af => controller.Filters.Remove(af));
                controller.Filters.Add(new AutoValidateFormAntiforgeryTokenAttribute());
            }
        }

        public void Apply(ParameterModel parameter)
        {
            //A parameter can only have one binding ource so to allow multiple you need to not set a bindingsource.

            //if no [ModelBinder] = BindingSource, or specific BinderType [FromQuery], [FromRoute], [FromForm], [FromBody], [FromHeader]
            if (parameter.BindingInfo == null || (parameter.BindingInfo.BinderType == null && (parameter.BindingInfo.BindingSource == null || parameter.BindingInfo.BindingSource == BindingSource.Form || parameter.BindingInfo.BindingSource == BindingSource.Query || parameter.BindingInfo.BindingSource == BindingSource.Path || parameter.BindingInfo.BindingSource == BindingSource.ModelBinding)))
            {
                if (parameter.BindingInfo == null)
                    parameter.BindingInfo = new BindingInfo();

                //[FromBodyAndOtherSources]
                parameter.BindingInfo.BinderType = typeof(BodyAndOtherSourcesModelBinder);
            }
        }
    }
}
