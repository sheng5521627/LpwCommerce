using Core.Infrastructure;
using Services.Localization;
using Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Web.Framework.Localization;

namespace Web.Framework
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString Hint(this HtmlHelper helper, string value)
        {
            var builder = new TagBuilder("img");

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = MvcHtmlString.Create(urlHelper.Content("~/Administration/Content/images/ico-help.gif")).ToHtmlString();

            builder.MergeAttribute("src", url);
            builder.MergeAttribute("alt", value);
            builder.MergeAttribute("title", value);

            return MvcHtmlString.Create(builder.ToString());
        }

        public static HelperResult LocalizedEditor<T, TLocalizedModelLocal>(this HtmlHelper<T> helper,
           string name,
           Func<int, HelperResult> localizedTemplate,
           Func<T, HelperResult> standardTemplate,
           bool ignoreIfSeveralStores = false)
           where T : ILocalizedModel<TLocalizedModelLocal>
           where TLocalizedModelLocal : ILocalizedModelLocal
        {
            return new HelperResult(writer =>
            {
                var localizationSupported = helper.ViewData.Model.Locales.Count > 1;
                if (ignoreIfSeveralStores)
                {
                    var storeService = EngineContext.Current.Resolve<IStoreService>();
                    if (storeService.GetAllStores().Count >= 2)
                    {
                        localizationSupported = false;
                    }
                }
                if (localizationSupported)
                {
                    var tabStrip = new StringBuilder();
                    tabStrip.AppendLine(string.Format("<div id='{0}'>", name));
                    tabStrip.AppendLine("<ul>");

                    //default tab
                    tabStrip.AppendLine("<li class='k-state-active'>");
                    tabStrip.AppendLine("Standard");
                    tabStrip.AppendLine("</li>");

                    foreach (var locale in helper.ViewData.Model.Locales)
                    {
                        //languages
                        var language = EngineContext.Current.Resolve<ILanguageService>().GetLanguageById(locale.LanguageId);

                        tabStrip.AppendLine("<li>");
                        var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                        var iconUrl = urlHelper.Content("~/Content/images/flags/" + language.FlagImageFileName);
                        tabStrip.AppendLine(string.Format("<img class='k-image' alt='' src='{0}'>", iconUrl));
                        tabStrip.AppendLine(HttpUtility.HtmlEncode(language.Name));
                        tabStrip.AppendLine("</li>");
                    }
                    tabStrip.AppendLine("</ul>");



                    //default tab
                    tabStrip.AppendLine("<div>");
                    tabStrip.AppendLine(standardTemplate(helper.ViewData.Model).ToHtmlString());
                    tabStrip.AppendLine("</div>");

                    for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                    {
                        //languages
                        tabStrip.AppendLine("<div>");
                        tabStrip.AppendLine(localizedTemplate(i).ToHtmlString());
                        tabStrip.AppendLine("</div>");
                    }
                    tabStrip.AppendLine("</div>");
                    tabStrip.AppendLine("<script>");
                    tabStrip.AppendLine("$(document).ready(function() {");
                    tabStrip.AppendLine(string.Format("$('#{0}').kendoTabStrip(", name));
                    tabStrip.AppendLine("{");
                    tabStrip.AppendLine("animation:  {");
                    tabStrip.AppendLine("open: {");
                    tabStrip.AppendLine("effects: \"fadeIn\"");
                    tabStrip.AppendLine("}");
                    tabStrip.AppendLine("}");
                    tabStrip.AppendLine("});");
                    tabStrip.AppendLine("});");
                    tabStrip.AppendLine("</script>");
                    writer.Write(new MvcHtmlString(tabStrip.ToString()));
                }
                else
                {
                    standardTemplate(helper.ViewData.Model).WriteTo(writer);
                }
            });
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string buttonsSelector) where T : BaseNopEntityModel
        {
            return DeleteConfirmation(helper, "", buttonsSelector);
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string actionName,
            string buttonsSelector) where T : BaseNopEntityModel
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Delete";

            var modalId = MvcHtmlString.Create(helper.ViewData.ModelMetadata.ModelType.Name.ToLower() + "-delete-confirmation")
                .ToHtmlString();

            var deleteConfirmationModel = new DeleteConfirmationModel
            {
                Id = helper.ViewData.Model.Id,
                ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
                ActionName = actionName,
                WindowId = modalId
            };

            var window = new StringBuilder();
            window.AppendLine(string.Format("<div id='{0}' style='display:none;'>", modalId));
            window.AppendLine(helper.Partial("Delete", deleteConfirmationModel).ToHtmlString());
            window.AppendLine("</div>");
            window.AppendLine("<script>");
            window.AppendLine("$(document).ready(function() {");
            window.AppendLine(string.Format("$('#{0}').click(function (e) ", buttonsSelector));
            window.AppendLine("{");
            window.AppendLine("e.preventDefault();");
            window.AppendLine(string.Format("var window = $('#{0}');", modalId));
            window.AppendLine("if (!window.data('kendoWindow')) {");
            window.AppendLine("window.kendoWindow({");
            window.AppendLine("modal: true,");
            window.AppendLine(string.Format("title: '{0}',", EngineContext.Current.Resolve<ILocalizationService>().GetResource("Admin.Common.AreYouSure")));
            window.AppendLine("actions: ['Close']");
            window.AppendLine("});");
            window.AppendLine("}");
            window.AppendLine("window.data('kendoWindow').center().open();");
            window.AppendLine("});");
            window.AppendLine("});");
            window.AppendLine("</script>");

            return MvcHtmlString.Create(window.ToString());
        }
    }
}
