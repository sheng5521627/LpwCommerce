using Core;
using Core.Data;
using Core.Domain.Customers;
using Core.Domain.Localization;
using Core.Domain.Stores;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Core.Infrastructure;
using Services.Localization;
using Services.Customers;

namespace Services.Installation
{
    public partial class SqlFileInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IDbContext _dbContext;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public SqlFileInstallationService(IRepository<Language> languageRepository,
            IRepository<Customer> customerRepository,
            IRepository<Store> storeRepository,
            IDbContext dbContext,
            IWebHelper webHelper)
        {
            this._languageRepository = languageRepository;
            this._customerRepository = customerRepository;
            this._storeRepository = storeRepository;
            this._dbContext = dbContext;
            this._webHelper = webHelper;
        }

        #region Utilities

        protected virtual void InstallLocaleResources()
        {
            var language = _languageRepository.Table.Single(m => m.Name == "English");

            foreach(var filePath in System.IO.Directory.EnumerateFiles(_webHelper.MapPath("~/App_Data/Localization/"), "*.nopres.xml", SearchOption.TopDirectoryOnly))
            {
                var localXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourceFromXml(language, localXml);
            }
        }

        protected virtual void UpdateDefaultCustomer(string defaultUserEmail,string defaultUserPassword)
        {
            var adminUser = _customerRepository.Table.Single(m => !m.IsSystemAccount);
            if (adminUser == null)
                throw new Exception("Admin user cannot be loaded");

            adminUser.CustomerGuid = Guid.NewGuid();
            adminUser.Email = defaultUserEmail;
            adminUser.Username = defaultUserEmail;
            _customerRepository.Update(adminUser);

            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false, PasswordFormat.Hashed, defaultUserPassword));
        }

        protected virtual void UpdateDefaultStoreUrl()
        {
            var store = _storeRepository.Table.FirstOrDefault();
            if (store == null)
                throw new Exception("Default store cannot be loaded");
            store.Url = _webHelper.GetStoreLocation(false);
            _storeRepository.Update(store);
        }

        protected virtual void ExecuteSqlFile(string path)
        {
            var statements = new List<string>();
            using (var stream = File.OpenRead(path))
            {
                using(var reader = new StreamReader(stream))
                {
                    string statement;
                    while ((statement = ReadNextStatementFromStream(reader)) != null)
                        statements.Add(statement);
                }

                foreach (var stmt in statements)
                    _dbContext.ExecuteSqlCommand(stmt);
            }
        }

        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var lineOfText = reader.ReadLine();
                if(lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    return null;
                }
                if(lineOfText.Trim().ToUpper() == "GO")
                {
                    break;
                }
                sb.Append(lineOfText + Environment.NewLine);
            }
            return sb.ToString();
        }

        #endregion

        public void InstallData(string defaultUserEmail, string defaultUserPassword, bool installSampleData = true)
        {
            ExecuteSqlFile(_webHelper.MapPath("~/App_Data/Install/create_required_data.sql"));
            InstallLocaleResources();
            UpdateDefaultCustomer(defaultUserEmail, defaultUserPassword);
            UpdateDefaultStoreUrl();

            if (installSampleData)
            {
                ExecuteSqlFile(_webHelper.MapPath("~/App_Data/Install/create_sample_data.sql"));
            }
        }

        #endregion
    }
}
