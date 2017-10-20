using Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Localization
{
    public partial interface ILocalizationService
    {
        void DeleteLocaleStringResource(LocaleStringResource localeStringResource);

        LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId);

        LocaleStringResource GetLocaleStringResourceByName(string resourceName);

        LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId, bool logIfNotFound = true);

        IList<LocaleStringResource> GetAllResource(int languageId);

        void InsertLocaleStringResource(LocaleStringResource localeStringResource);

        void UpdateLocaleStringResource(LocaleStringResource localeStringResource);

        Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId);

        string GetResource(string resourceKey);

        string GetResource(string resourceKey, int languageId, bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false);

        string ExportResourceToXml(Language language);

        void ImportResourceFromXml(Language language, string xml);
    }
}
