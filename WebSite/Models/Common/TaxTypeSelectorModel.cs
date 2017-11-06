using Core.Domain.Tax;
using Web.Framework.Mvc;

namespace WebSite.Models.Common
{
    public partial class TaxTypeSelectorModel : BaseNopModel
    {
        public TaxDisplayType CurrentTaxType { get; set; }
    }
}