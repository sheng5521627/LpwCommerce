using Core;
using Core.Configuration;
using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserAgentStringLibrary;

namespace Services.Helpers
{
    public partial class UserAgentHelper : IUserAgentHelper
    {
        private readonly NopConfig _config;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;

        public UserAgentHelper(NopConfig config,IWebHelper webHelper,HttpContextBase httpContext)
        {
            this._config = config;
            this._webHelper = webHelper;
            this._httpContext = httpContext;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual UasParser GetUasParser()
        {
            if (Singleton<UasParser>.Instance == null)
            {
                if (string.IsNullOrEmpty(_config.UserAgentStringsPath))
                {
                    return null;
                }
                var filePath = _webHelper.MapPath(_config.UserAgentStringsPath);
                var uasParser = new UasParser(filePath);
                Singleton<UasParser>.Instance = uasParser;
            }
            return Singleton<UasParser>.Instance;
        }

        /// <summary>
        /// 判断请求是否是搜索引擎
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSearchEngine()
        {
            if (_httpContext == null)
                return false;

            bool result = false;
            try
            {
                var uasParser = GetUasParser();
                if (uasParser == null)
                    return false;
                var userAgent = _httpContext.Request.UserAgent;
                result = uasParser.IsBot(userAgent);
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
