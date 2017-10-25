using Core.Domain.Directory;
using Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Directory
{
    public partial class UpdateExchangeRateTask : ITask
    {
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;

        public UpdateExchangeRateTask(ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
        }

        public void Execute()
        {
            if (!_currencySettings.AutoUpdateEnabled)
                return;

            var primaryCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            var exchangeRates = _currencyService.GetCurrencyLiveRates(primaryCurrencyCode);

            foreach(var exchangeRate in exchangeRates)
            {
                var currency = _currencyService.GetCurrencyByCode(exchangeRate.CurrencyCode);
                if(currency != null)
                {
                    currency.Rate = exchangeRate.Rate;
                    currency.UpdatedOnUtc = DateTime.UtcNow;
                    _currencyService.UpdateCurrency(currency);
                }
            }
        }
    }
}
