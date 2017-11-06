using Web.Framework.Mvc;

namespace WebSite.Models.Newsletter
{
    public partial class NewsletterBoxModel : BaseNopModel
    {
        public string NewsletterEmail { get; set; }
        public bool AllowToUnsubscribe { get; set; }
    }
}