using Core.Data;
using Core.Domain.Localization;
using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Web.Framework.Localization
{
    public class LocalizedRoute : Route
    {
        #region Fields

        private bool? _seoFriendlyUrlsForLanguagesEnabled;

        #endregion

        public LocalizedRoute(string url,IRouteHandler routeHandler)
            : base(url, routeHandler)
        {

        }

        public LocalizedRoute(string url,RouteValueDictionary defaults,IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {

        }

        public LocalizedRoute(string url,RouteValueDictionary defaults,RouteValueDictionary constraints,IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {

        }

        public LocalizedRoute(string url,RouteValueDictionary defaults,RouteValueDictionary constraints,RouteValueDictionary dataTokens,IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {

        }

        protected bool SeoFriendlyUrlsForLanguagesEnabled
        {
            get
            {
                if (!_seoFriendlyUrlsForLanguagesEnabled.HasValue)
                {
                    _seoFriendlyUrlsForLanguagesEnabled = EngineContext.Current.Resolve<LocalizationSettings>().SeoFriendlyUrlsForLanguagesEnabled;
                }
                return _seoFriendlyUrlsForLanguagesEnabled.Value;
            }
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if(DataSettingsHelper.DatabaseIsInstalled() && this.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath;
                string applicationPath = httpContext.Request.ApplicationPath;
                if (virtualPath.IsLocalizedUrl(applicationPath, false))
                {
                    //In ASP.NET Development Server, an URL like "http://localhost/Blog.aspx/Categories/BabyFrog" will return 
                    //"~/Blog.aspx/Categories/BabyFrog" as AppRelativeCurrentExecutionFilePath.
                    //However, in II6, the AppRelativeCurrentExecutionFilePath is "~/Blog.aspx"
                    //It seems that IIS6 think we're process Blog.aspx page.
                    //So, I'll use RawUrl to re-create an AppRelativeCurrentExecutionFilePath like ASP.NET Development Server.

                    //Question: should we do path rewriting right here?

                    string rawUrl = httpContext.Request.RawUrl;
                    var newVirtualPath = rawUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                    if (string.IsNullOrEmpty(newVirtualPath))
                        newVirtualPath = "/";
                    newVirtualPath = newVirtualPath.RemoveApplicationPathFromRawUrl(applicationPath);
                    newVirtualPath = "~" + newVirtualPath;
                    httpContext.RewritePath(newVirtualPath);
                }
            }
            return base.GetRouteData(httpContext);
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData data = base.GetVirtualPath(requestContext, values);

            if (data != null && DataSettingsHelper.DatabaseIsInstalled() && this.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string rawUrl = requestContext.HttpContext.Request.RawUrl;
                string applicationPath = requestContext.HttpContext.Request.ApplicationPath;
                if (rawUrl.IsLocalizedUrl(applicationPath, true))
                {
                    data.VirtualPath = string.Concat(rawUrl.GetLanguageSeoCodeFromUrl(applicationPath, true), "/", data.VirtualPath);
                }
            }

            return data;
        }

        public virtual void ClearSeoFriendlyUrlsCachedValue()
        {
            _seoFriendlyUrlsForLanguagesEnabled = null;
        }
    }
}
