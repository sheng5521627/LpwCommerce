using Core.Data;
using Core.Domain.Customers;
using Core.Plugins;
using Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Authentication.External
{
    /// <summary>
    /// Open authentication service
    /// </summary>
    public partial class OpenAuthenticationService : IOpenAuthenticationService
    {
        private readonly ICustomerService _customerService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;

        public OpenAuthenticationService(IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IPluginFinder pluginFinder,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ICustomerService customerService)
        {
            this._externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            this._pluginFinder = pluginFinder;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._customerService = customerService;
        }

        public IList<IExternalAuthenticationMethod> LoadActiveExternalAuthenticationMethods(int storeId = 0)
        {
            return LoadActiveExternalAuthenticationMethods(storeId)
                .Where(m => _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Contains(m.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                .ToList();
        }

        public IExternalAuthenticationMethod LoadExternalAuthenticationMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IExternalAuthenticationMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IExternalAuthenticationMethod>();

            return null;
        }

        public IList<IExternalAuthenticationMethod> LoadAllExternalAuthenticationMethods(int storeId = 0)
        {
            return _pluginFinder.GetPlugins<IExternalAuthenticationMethod>(storeId: storeId).ToList();
        }

        public bool AccountExists(OpenAuthenticationParameters parameters)
        {
            return GetUser(parameters) != null;
        }

        public void AssociateExternalAccountWithUser(Customer customer, OpenAuthenticationParameters parameters)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            //find email
            string email = null;
            if (parameters.UserClaims != null)
            {
                foreach (var userClaim in parameters.UserClaims
                    .Where(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Email)))
                {
                    //found
                    email = userClaim.Contact.Email;
                    break;
                }
            }

            var externalAuthenticationRecord = new ExternalAuthenticationRecord
            {
                CustomerId = customer.Id,
                Email = email,
                ExternalIdentifier = parameters.ExternalIdentifier,
                ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
                OAuthToken = parameters.OAuthToken,
                OAuthAccessToken = parameters.OAuthAccessToken,
                ProviderSystemName = parameters.ProviderSystemName,
            };

            _externalAuthenticationRecordRepository.Insert(externalAuthenticationRecord);
        }

        public Customer GetUser(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .FirstOrDefault(m => m.ExternalIdentifier == parameters.ExternalIdentifier && m.ProviderSystemName == parameters.ProviderSystemName);

            if (record != null)
                return _customerService.GetCustomerById(record.CustomerId);

            return null;
        }

        public IList<ExternalAuthenticationRecord> GetExternalIdentifiersFor(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            return customer.ExternalAuthenticationRecords.ToList();
        }

        public void DeletExternalAuthenticationRecord(ExternalAuthenticationRecord externalAuthenticationRecord)
        {
            if (externalAuthenticationRecord == null)
                throw new ArgumentNullException("externalAuthenticationRecord");

            _externalAuthenticationRecordRepository.Delete(externalAuthenticationRecord);
        }

        public void RemoveAssociation(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                 .FirstOrDefault(o => o.ExternalIdentifier == parameters.ExternalIdentifier &&
                     o.ProviderSystemName == parameters.ProviderSystemName);

            if (record != null)
                _externalAuthenticationRecordRepository.Delete(record);
        }
    }
}
