using Core;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;
using Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Directory
{
    /// <summary>
    /// GEO lookup service
    /// </summary>
    public partial class GeoLookupService : IGeoLookupService
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;

        #endregion
        
        public GeoLookupService(ILogger logger, IWebHelper webHelper)
        {
            this._logger = logger;
            this._webHelper = webHelper;
        }

        protected virtual CountryResponse GetInformation(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return null;

            try
            {
                var databasePath = _webHelper.MapPath("~/App_Data/GeoLite2-Country.mmdb");
                var reader = new DatabaseReader(databasePath);
                var country = reader.Country(ipAddress);
                return country;
            }
            catch (GeoIP2Exception)
            {
                return null;
            }
            catch(Exception exc)
            {
                _logger.Warning("Cannot load MaxMind record", exc);
                return null;
            }
        }

        public string LookupCountryIsoCode(string ipAddress)
        {
            var response = GetInformation(ipAddress);
            if (response != null && response.Country != null)
            {
                return response.Country.IsoCode;
            }
            return "";
        }

        public string LookupCountryName(string ipAddress)
        {
            var response = GetInformation(ipAddress);
            if (response != null && response.Country != null)
            {
                return response.Country.Name;
            }
            return "";
        }
    }
}
