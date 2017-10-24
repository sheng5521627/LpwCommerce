using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Customers;
using Core.Domain.Security;
using Core.Data;
using Core.Caching;
using Services.Events;
using Core.Domain.Catalog;

namespace Services.Security
{
    public partial class AclService : IAclService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        private const string ACLRECORD_BY_ENTITYID_NAME_KEY = "Nop.aclrecord.entityid-name-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ACLRECORD_PATTERN_KEY = "Nop.aclrecord.";

        #endregion

        #region Fields

        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="workContext">Work context</param>
        /// <param name="aclRecordRepository">ACL record repository</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        public AclService(ICacheManager cacheManager,
            IWorkContext workContext,
            IRepository<AclRecord> aclRecordRepository,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings)
        {
            this._cacheManager = cacheManager;
            this._workContext = workContext;
            this._aclRecordRepository = aclRecordRepository;
            this._eventPublisher = eventPublisher;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        public bool Authorize<T>(T entity) where T : BaseEntity, IAclSupported
        {
            return Authorize(entity, _workContext.CurrentCustomer);
        }

        public bool Authorize<T>(T entity, Customer customer) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (customer == null)
                return false;

            if (_catalogSettings.IgnoreAcl)
                return true;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role in customer.CustomerRoles.Where(m => m.Active))
            {
                foreach (var roleId in GetCustomerRoleIdsWithAccess(entity))
                {
                    if (role.Id == roleId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void DeleteAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Delete(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(aclRecord);
        }

        public AclRecord GetAclRecordById(int aclRecordId)
        {
            if (aclRecordId == 0)
                return null;

            return _aclRecordRepository.GetById(aclRecordId);
        }

        public IList<AclRecord> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var query = from acl in _aclRecordRepository.Table
                        where acl.EntityId == entityId && acl.EntityName == entityName
                        select acl;
            return query.ToList();
        }

        public int[] GetCustomerRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;
            string key = string.Format(ACLRECORD_BY_ENTITYID_NAME_KEY, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from acl in _aclRecordRepository.Table
                            where acl.EntityId == entityId && acl.EntityName == entityName
                            select acl.CustomerRoleId;
                return query.ToArray();
            });
        }

        public void InsertAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Insert(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(aclRecord);
        }

        public void InsertAclRecord<T>(T entity, int customerRoleId) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (customerRoleId == 0)
                throw new ArgumentOutOfRangeException("customerRoleId");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var aclRecord = new AclRecord
            {
                EntityId = entityId,
                EntityName = entityName,
                CustomerRoleId = customerRoleId
            };

            InsertAclRecord(aclRecord);
        }

        public void UpdateAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Update(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(aclRecord);
        }
    }
}
