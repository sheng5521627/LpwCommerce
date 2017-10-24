using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Customers;
using Core.Domain.Security;
using Core.Data;
using Services.Customers;
using Core;
using Services.Localization;
using Core.Caching;

namespace Services.Security
{
    public partial class PermissionService : IPermissionService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer role ID
        /// {1} : permission system name
        /// </remarks>
        private const string PERMISSIONS_ALLOWED_KEY = "Nop.permission.allowed-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PERMISSIONS_PATTERN_KEY = "Nop.permission.";
        #endregion

        #region Fields

        private readonly IRepository<PermissionRecord> _permissionRecordRepository;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="permissionRecordRepository">Permission repository</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="cacheManager">Cache manager</param>
        public PermissionService(IRepository<PermissionRecord> permissionRecordRepository,
            ICustomerService customerService,
            IWorkContext workContext,
             ILocalizationService localizationService,
            ILanguageService languageService,
            ICacheManager cacheManager)
        {
            this._permissionRecordRepository = permissionRecordRepository;
            this._customerService = customerService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        protected virtual bool Authorize(string permissionRecordSystemName,CustomerRole customerRole)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            string key = string.Format(PERMISSIONS_ALLOWED_KEY, customerRole.Id, permissionRecordSystemName);
            return _cacheManager.Get(key, () => 
            {
                foreach(var permission in customerRole.PermissionRecords)
                {
                    if (permission.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        #endregion

        public bool Authorize(string permissionRecordSystemName)
        {
            return Authorize(permissionRecordSystemName, _workContext.CurrentCustomer);
        }

        public bool Authorize(PermissionRecord permission)
        {
            return Authorize(permission.SystemName, _workContext.CurrentCustomer);
        }

        public bool Authorize(string permissionRecordSystemName, Customer customer)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var customerRoles = customer.CustomerRoles.Where(m => m.Active);
            foreach(var role in customerRoles)
            {
                if (Authorize(permissionRecordSystemName, role))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Authorize(PermissionRecord permission, Customer customer)
        {
            if (permission == null)
                return false;
            if (customer == null)
                return false;

            return Authorize(permission.SystemName, customer);
        }

        public void DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Delete(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        public IList<PermissionRecord> GetAllPermissionRecords()
        {
            var query = from p in _permissionRecordRepository.Table
                        orderby p.Name
                        select p;
            return query.ToList();
        }

        public PermissionRecord GetPermissionRecordById(int permissionId)
        {
            if (permissionId == 0)
                return null;

            return _permissionRecordRepository.GetById(permissionId);
        }

        public PermissionRecord GetPermissionRecordBySystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            var query = from p in _permissionRecordRepository.Table
                        where p.SystemName == systemName
                        orderby p.Id
                        select p;
            return query.FirstOrDefault();
        }

        public void InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Insert(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        public void InstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 == null)
                {
                    permission1 = new PermissionRecord()
                    {
                        Name = permission.Name,
                        SystemName = permission.SystemName,
                        Category = permission.Category
                    };

                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        var customerRole = _customerService.GetCustomerRoleBySystemName(defaultPermission.CustomerRoleSystemName);
                        if (customerRole == null)
                        {
                            customerRole = new CustomerRole()
                            {
                                Name = defaultPermission.CustomerRoleSystemName,
                                Active = true,
                                SystemName = defaultPermission.CustomerRoleSystemName
                            };
                            _customerService.InsertCustomerRole(customerRole);
                        }

                        var defaultMappingProvider = (from p in defaultPermission.PermissionRecords
                                                      where p.SystemName == permission1.SystemName
                                                      select p).Any();
                        var mappingExists = (from p in customerRole.PermissionRecords
                                             where p.SystemName == permission1.SystemName
                                             select p).Any();
                        if (defaultMappingProvider && !mappingExists)
                        {
                            permission1.CustomerRoles.Add(customerRole);
                        }
                    }

                    InsertPermissionRecord(permission1);
                    permission1.SaveLocalizedPermissionName(_localizationService, _languageService);
                }
            }
        }

        public void UninstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach(var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if(permission1 != null)
                {
                    DeletePermissionRecord(permission1);
                    permission1.DeleteLocalizedPermissionName(_localizationService, _languageService);
                }
            }
        }

        public void UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionRecordRepository.Update(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }
    }
}
