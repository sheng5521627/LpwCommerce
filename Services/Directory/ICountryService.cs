using Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Directory
{
    public partial interface ICountryService
    {
        void DeleteCountry(Country country);

        IList<Country> GetAllCountries(int languageId = 0, bool showHidden = false);

        IList<Country> GetAllCountriesForBilling(int languageId = 0, bool showHidden = false);

        IList<Country> GetAllCountriesForShipping(int languageId = 0, bool showHidden = false);

        Country GetCountryById(int countryId);

        IList<Country> GetCountriesByIds(int[] countryIds);

        Country GetCountryByTwoLetterIsoCode(string twoLetterIsoCode);

        Country GetCountryByThreedLetterIsoCode(string threedLetterIsoCode);

        void InsertCountry(Country country);

        void UpdateCountry(Country country);
    }
}
