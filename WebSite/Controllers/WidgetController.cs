using Core;
using Core.Caching;
using Services.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Framework.Themes;
using WebSite.Infrastructure.Cache;
using WebSite.Models.Cms;

namespace WebSite.Controllers
{
    public class WidgetController : BasePublicController
    {
        #region Fields

        private readonly IWidgetService _widgetService;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        public WidgetController(IWidgetService widgetService,
            IStoreContext storeContext,
            IThemeContext themeContext,
            ICacheManager cacheManager)
        {
            _widgetService = widgetService;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _cacheManager = cacheManager;
        }

        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone, object additionalData)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.WIDGET_MODEL_KEY, _storeContext.CurrentStore.Id, widgetZone, _themeContext.WorkingThemeName);
            var cacheModel = _cacheManager.Get(cacheKey, () => 
            {
                var model = new List<RenderWidgetModel>();

                var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone, _storeContext.CurrentStore.Id);
                foreach(var widget in widgets)
                {
                    var widgetModel = new RenderWidgetModel();
                    string controllerName;
                    string actionName;
                    RouteValueDictionary routeValues;
                    widget.GetDisplayWidgetRoute(widgetZone, out actionName, out controllerName, out routeValues);
                    widgetModel.ControllerName = controllerName;
                    widgetModel.ActionName = actionName;
                    widgetModel.RouteValues = routeValues;

                    model.Add(widgetModel);
                }
                return model;
            });

            if (cacheModel.Count == 0)
                return Content("");

            //"RouteValues" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var cloneModel = new List<RenderWidgetModel>();
            foreach(var widgetModel in cacheModel)
            {
                var cloneWidgetModel = new RenderWidgetModel();
                cloneWidgetModel.ControllerName = widgetModel.ControllerName;
                cloneWidgetModel.ActionName = widgetModel.ActionName;
                if (widgetModel.RouteValues != null)
                    cloneWidgetModel.RouteValues = new RouteValueDictionary(widgetModel.RouteValues);

                if (additionalData != null)
                {
                    if (cloneWidgetModel.RouteValues == null)
                        cloneWidgetModel.RouteValues = new RouteValueDictionary();

                    cloneWidgetModel.RouteValues.Add("additionalData", additionalData);
                }

                cloneModel.Add(cloneWidgetModel);
            }

            return PartialView(cloneModel);
        }
    }
}