using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Catalog;
using Core.Domain.Directory;
using Core.Domain.Localization;
using Core;
using Core.Domain.Tax;
using Services.Localization;
using Services.Directory;
using System.Globalization;

namespace Services.Catalog
{
    /// <summary>
    /// Price formatter
    /// </summary>
    public partial class PriceFormatter : IPriceFormatter
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly TaxSettings _taxSettings;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Constructors

        public PriceFormatter(IWorkContext workContext,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            TaxSettings taxSettings,
            CurrencySettings currencySettings)
        {
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._taxSettings = taxSettings;
            this._currencySettings = currencySettings;
        }

        #endregion

        #region Utilities

        protected virtual string GetCurrencyString(decimal amount)
        {
            return GetCurrencyString(amount, true, _workContext.WorkingCurrency);
        }

        protected virtual string GetCurrencyString (decimal amount,bool showCurrency,Currency targetCurrency)
        {
            if (targetCurrency == null)
                throw new ArgumentNullException("targetCurrency");

            string result;
            if (!string.IsNullOrEmpty(targetCurrency.CustomFormatting))
            {
                result = amount.ToString(targetCurrency.CustomFormatting);
            }
            else
            {
                if (!String.IsNullOrEmpty(targetCurrency.DisplayLocale))
                {
                    //default behavior
                    result = amount.ToString("C", new CultureInfo(targetCurrency.DisplayLocale));
                }
                else
                {
                    //not possible because "DisplayLocale" should be always specified
                    //but anyway let's just handle this behavior
                    result = String.Format("{0} ({1})", amount.ToString("N"), targetCurrency.CurrencyCode);
                    return result;
                }
            }

            //display currency code?
            if (showCurrency && _currencySettings.DisplayCurrencyLabel)
                result = String.Format("{0} ({1})", result, targetCurrency.CurrencyCode);
            return result;
        }

        #endregion

        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency)
        {
            bool priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatPaymentMethodAdditionalFee(price, showCurrency, _workContext.WorkingCurrency,
                _workContext.WorkingLanguage, priceIncludesTax);
        }

        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode)
                ?? new Currency
                {
                    CurrencyCode = currencyCode
                };
            return FormatPaymentMethodAdditionalFee(price, showCurrency, currency,
                language, priceIncludesTax);
        }

        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix;
            return FormatPaymentMethodAdditionalFee(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        public string FormatPrice(decimal price)
        {
            return FormatPrice(price, true, _workContext.WorkingCurrency);
        }

        public string FormatPrice(decimal price, bool showCurrency, bool showTax)
        {
            bool priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatPrice(price, showCurrency, _workContext.WorkingCurrency, _workContext.WorkingLanguage, priceIncludesTax, showTax);
        }

        public string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency)
        {
            bool priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatPrice(price, showCurrency, targetCurrency, _workContext.WorkingLanguage, priceIncludesTax);
        }

        public string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language,
                priceIncludesTax, _taxSettings.DisplayTaxSuffix);
        }

        public string FormatPrice(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode)
                 ?? new Currency
                 {
                     CurrencyCode = currencyCode
                 };
            return FormatPrice(price, showCurrency, currency, language, priceIncludesTax);
        }

        public string FormatPrice(decimal price, bool showCurrency, string currencyCode, bool showTax, Language language)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            bool priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatPrice(price, showCurrency, currency, language, priceIncludesTax, showTax);
        }

        public string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            price = RoundingHelper.RoundPrice(price);

            string currencyString = GetCurrencyString(price, showCurrency, targetCurrency);
            if (showTax)
            {
                string formatStr;
                if (priceIncludesTax)
                {
                    formatStr = _localizationService.GetResource("Products.InclTaxSuffix", language.Id, false);
                    if (string.IsNullOrEmpty(formatStr))
                        formatStr = "{0} incl tax";
                }
                else
                {
                    formatStr = _localizationService.GetResource("Products.ExclTaxSuffix", language.Id, false);
                    if (String.IsNullOrEmpty(formatStr))
                        formatStr = "{0} excl tax";
                }
                return string.Format(formatStr, currencyString);
            }

            return currencyString;
        }

        /// <summary>
        /// 租赁产品的价格（含租赁期）
        /// </summary>
        /// <param name="product"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public string FormatRentalProductPeriod(Product product, string price)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (!product.IsRental)
                return price;

            if (string.IsNullOrWhiteSpace(price))
                return price;

            string result;
            switch (product.RentalPricePeriod)
            {
                case RentalPricePeriod.Days:
                    result = string.Format(_localizationService.GetResource("Products.Price.Rental.Days"), price, product.RentalPriceLength);
                    break;
                case RentalPricePeriod.Weeks:
                    result = string.Format(_localizationService.GetResource("Products.Price.Rental.Weeks"), price, product.RentalPriceLength);
                    break;
                case RentalPricePeriod.Months:
                    result = string.Format(_localizationService.GetResource("Products.Price.Rental.Months"), price, product.RentalPriceLength);
                    break;
                case RentalPricePeriod.Years:
                    result = string.Format(_localizationService.GetResource("Products.Price.Rental.Years"), price, product.RentalPriceLength);
                    break;
                default:
                    throw new NopException("Not supported rental period");
            }

            return result;
        }

        public string FormatShippingPrice(decimal price, bool showCurrency)
        {
            bool priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatShippingPrice(price, showCurrency, _workContext.WorkingCurrency, _workContext.WorkingLanguage, priceIncludesTax);
        }

        public string FormatShippingPrice(decimal price, bool showCurrency, string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode)
                 ?? new Currency
                 {
                     CurrencyCode = currencyCode
                 };
            return FormatShippingPrice(price, showCurrency, currency, language, priceIncludesTax);
        }

        public string FormatShippingPrice(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix;
            return FormatShippingPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        public string FormatShippingPrice(decimal price, bool showCurrency, Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        public string FormatTaxRate(decimal taxRate)
        {
            return taxRate.ToString("G29");
        }
    }
}
