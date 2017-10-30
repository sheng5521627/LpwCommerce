using Core;
using Core.Infrastructure;
using Services.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Framework.Mvc;

namespace Web.Framework
{
    public class NopResourceDisplayName : DisplayNameAttribute, IModelAttribute
    {
        private string _resourceValue = string.Empty;

        public string ResourceKey { get; set; }

        public NopResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }
        public override string DisplayName
        {
            get
            {
                var languageId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                _resourceValue = EngineContext.Current.Resolve<ILocalizationService>().GetResource(ResourceKey, languageId, true, ResourceKey);
                return _resourceValue;
            }
        }

        public string Name
        {
            get
            {
                return "NopResourceDisplayName";
            }
        }
    }
}
