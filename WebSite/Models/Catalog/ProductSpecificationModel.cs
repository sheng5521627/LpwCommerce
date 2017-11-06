using Web.Framework.Mvc;

namespace WebSite.Models.Catalog
{
    public partial class ProductSpecificationModel : BaseNopModel
    {
        public int SpecificationAttributeId { get; set; }

        public string SpecificationAttributeName { get; set; }

        //this value is already HTML encoded
        public string ValueRaw { get; set; }
    }
}