using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Stores;
using Services.Stores;

namespace Web.Framework
{
    public partial class WebStoreContext : IStoreContext
    {
        private IStoreService _storeService;
        private IWebHelper _webHelper;
        private Store _cacheStore;

        public WebStoreContext(IStoreService storeService,IWebHelper webHelper)
        {
            this._storeService = storeService;
            this._webHelper = webHelper;
        }

        public Store CurrentStore
        {
            get
            {
                if (_cacheStore != null)
                    return _cacheStore;

                var host = _webHelper.ServerVariables("HTPP_HOST");
                var allStores = _storeService.GetAllStores();
                var store = allStores.FirstOrDefault(s => s.ContainsHostValue(host));
                if(store == null)
                {
                    return allStores.FirstOrDefault();
                }
                if (store == null)
                    throw new Exception("没有加载到商店信息");
                _cacheStore = store;
                return _cacheStore;
            }
        }
    }
}
