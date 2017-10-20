using Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Localization
{
    public partial interface ILanguageService
    {
        void DeleteLanguage(Language language);

        IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0);

        Language GetLanguageById(int languageId);

        void InsertLanguage(Language language);

        void UpdateLanguage(Language language);
    }
}
