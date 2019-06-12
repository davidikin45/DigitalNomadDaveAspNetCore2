using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace AspNetCore.Base.ModelBinders
{
    public class FromBodyAndFormAttribute : FromBodyAndAttribute
    {
        public FromBodyAndFormAttribute()
               : base(BindingSource.Form)
        {

        }
    }

    public class FromBodyAndQueryAttribute : FromBodyAndAttribute
    {
        public FromBodyAndQueryAttribute()
               : base(BindingSource.Query)
        {

        }
    }

    public class FromBodyAndRouteAttribute : FromBodyAndAttribute
    {
        public FromBodyAndRouteAttribute()
               : base(BindingSource.Path)
        {

        }
    }

    public class FromBodyAndFormQueryRouteAttribute : FromBodyAndAttribute
    {
     public FromBodyAndFormQueryRouteAttribute()
            :base(BindingSource.ModelBinding)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FromBodyAndAttribute : ModelBinderAttribute, IBindingSourceMetadata
    {
        public FromBodyAndAttribute(BindingSource bindingSource = null) :base(typeof(BodyAndOtherSourcesModelBinder))
        {
            //ModelBinderAttribute will return ModelBinder.Custom if BindingSource is null.
            BindingSource = bindingSource ?? BindingSource.ModelBinding;
        }

    }
}