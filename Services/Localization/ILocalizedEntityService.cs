using Core;
using Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Localization
{
    public partial interface ILocalizedEntityService
    {
        void DeleteLocalizedProperty(LocalizedProperty localizedProperty);

        LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId);

        string GetLocalizedValue(int languageId, int entityId, string localeKeyGroup, string localeKey);

        void InsertLocalizedProperty(LocalizedProperty localizedProperty);

        void UpdateLocalizedProperty(LocalizedProperty localizedProperty);

        void SaveLocalizedValue<T>(T entity, Expression<Func<T, string>> keySelector, string localeValue, int languageId) where T : BaseEntity, ILocalizedEntity;

        void SaveLocalizedValue<T, TProType>(T entity, Expression<Func<T, TProType>> keySelector, string localeValue, int languageId) where T : BaseEntity, ILocalizedEntity;
    }
}
