using System;
using System.Web.Mvc;

namespace Serenity.Services
{
    public class ServiceModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (modelType.IsInterface)
                return null;

            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
    }
}