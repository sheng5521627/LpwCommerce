using System.Web.Mvc;
using FluentValidation.Attributes;
using Web.Framework;
using Web.Framework.Mvc;
using WebSite.Validators.Vendors;

namespace WebSite.Models.Vendors
{
    [Validator(typeof(ApplyVendorValidator))]
    public partial class ApplyVendorModel : BaseNopModel
    {
        [NopResourceDisplayName("Vendors.ApplyAccount.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Vendors.ApplyAccount.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool DisableFormInput { get; set; }
        public string Result { get; set; }
    }
}