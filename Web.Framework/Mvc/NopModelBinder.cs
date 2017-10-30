using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Framework.Mvc
{
    public class NopModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if(model is BaseNopModel)
            {
                ((BaseNopModel)model).BindModel(controllerContext, bindingContext);
            }

            return model;
        }
    }
}
