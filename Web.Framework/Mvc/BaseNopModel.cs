using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Framework.Mvc
{
    [ModelBinder(typeof(NopModelBinder))]
    public partial class BaseNopModel
    {
        public Dictionary<string,object> CustomProperties { get; set; }

        public BaseNopModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        public virtual void BindModel(ControllerContext controllerContext,ModelBindingContext bindingContext)
        {

        }

        protected virtual void PostInitialize()
        {

        }
    }

    public partial class BaseNopEntityModel : BaseNopModel
    {
        public virtual int Id { get; set; }
    }
}
