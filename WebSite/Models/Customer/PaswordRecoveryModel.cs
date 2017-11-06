using System.Web.Mvc;
using FluentValidation.Attributes;
using Web.Framework;
using Web.Framework.Mvc;
using WebSite.Validators.Customer;

namespace WebSite.Models.Customer
{
    [Validator(typeof(PasswordRecoveryValidator))]
    public partial class PasswordRecoveryModel : BaseNopModel
    {
        [AllowHtml]
        [NopResourceDisplayName("Account.PasswordRecovery.Email")]
        public string Email { get; set; }

        public string Result { get; set; }
    }
}