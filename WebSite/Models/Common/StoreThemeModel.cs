using Web.Framework.Mvc;

namespace WebSite.Models.Common
{
    public partial class StoreThemeModel : BaseNopModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}