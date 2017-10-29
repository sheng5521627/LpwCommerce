using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Customers;
using Core.Domain.Orders;
using Core.Data;
using Core.Domain.Common;
using Core.Domain.Forums;
using Core.Domain.Blogs;
using Core.Domain.News;
using Core.Domain.Polls;
using Core.Domain.Catalog;
using Services.Events;
using Services.Common;
using Data;
using Core.Caching;
using System.Globalization;
using Core.Domain.Shipping;
using System.Data;
using System.Diagnostics;

namespace Services.Customers
{
    public partial class CustomerService : ICustomerService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        private const string CUSTOMERROLES_BY_SYSTEMNAME_KEY = "Nop.customerrole.systemname-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CUSTOMERROLES_PATTERN_KEY = "Nop.customerrole.";

        #endregion

        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<CustomerPassword> _customerPasswordRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ForumPost> _forumPostRepository;
        private readonly IRepository<ForumTopic> _forumTopicRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IRepository<NewsComment> _newsCommentRepository;
        private readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly CustomerSettings _customerSettings;
        private readonly CommonSettings _commonSettings;

        #endregion

        #region Ctor

        public CustomerService(ICacheManager cacheManager,
            IRepository<Customer> customerRepository,
            IRepository<CustomerPassword> customerPasswordRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<GenericAttribute> gaRepository,
            IRepository<Order> orderRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<BlogComment> blogCommentRepository,
            IRepository<NewsComment> newsCommentRepository,
            IRepository<PollVotingRecord> pollVotingRecordRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
            IGenericAttributeService genericAttributeService,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            CustomerSettings customerSettings,
            CommonSettings commonSettings)
        {
            this._cacheManager = cacheManager;
            this._customerRepository = customerRepository;
            this._customerPasswordRepository = customerPasswordRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._gaRepository = gaRepository;
            this._orderRepository = orderRepository;
            this._forumPostRepository = forumPostRepository;
            this._forumTopicRepository = forumTopicRepository;
            this._blogCommentRepository = blogCommentRepository;
            this._newsCommentRepository = newsCommentRepository;
            this._pollVotingRecordRepository = pollVotingRecordRepository;
            this._productReviewRepository = productReviewRepository;
            this._productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
            this._genericAttributeService = genericAttributeService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._eventPublisher = eventPublisher;
            this._customerSettings = customerSettings;
            this._commonSettings = commonSettings;
        }

        #endregion

        /// <summary>
        /// Get current customer password
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer password</returns>
        public virtual CustomerPassword GetCurrentPassword(int customerId)
        {
            if (customerId == 0)
                return null;

            //return the latest password
            return GetCustomerPasswords(customerId, passwordsToReturn: 1).FirstOrDefault();
        }

        /// <summary>
        /// Gets customer passwords
        /// </summary>
        /// <param name="customerId">Customer identifier; pass null to load all records</param>
        /// <param name="passwordFormat">Password format; pass null to load all records</param>
        /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
        /// <returns>List of customer passwords</returns>
        public virtual IList<CustomerPassword> GetCustomerPasswords(int? customerId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
        {
            var query = _customerPasswordRepository.Table;

            //filter by customer
            if (customerId.HasValue)
                query = query.Where(password => password.CustomerId == customerId.Value);

            //filter by password format
            if (passwordFormat.HasValue)
                query = query.Where(password => password.PasswordFormatId == (int)(passwordFormat.Value));

            //get the latest passwords
            if (passwordsToReturn.HasValue)
                query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

            return query.ToList();
        }

        public void DeleteCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (customer.IsSystemAccount)
                throw new NopException(string.Format("System customer account ({0}) could not be deleted", customer.SystemName));

            customer.Deleted = true;

            if (_customerSettings.SuffixDeletedCustomers)
            {
                if (!string.IsNullOrEmpty(customer.Email))
                    customer.Email += "-DELETED";
                if (!string.IsNullOrEmpty(customer.Username))
                    customer.Username += "-DELETED";
            }

            UpdateCustomer(customer);
        }

        public void DeleteCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            if (customerRole.IsSystemRole)
                throw new NopException("System role could not be deleted");

            _customerRoleRepository.Delete(customerRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);

            _eventPublisher.EntityDeleted(customerRole);
        }

        public int DeleteGuestCustomers(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart)
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database. 
                //It's much faster than the LINQ implementation below 

                #region Stored procedure

                //prepare parameters
                var pOnlyWithoutShoppingCart = _dataProvider.GetParameter();
                pOnlyWithoutShoppingCart.ParameterName = "OnlyWithoutShoppingCart";
                pOnlyWithoutShoppingCart.Value = onlyWithoutShoppingCart;
                pOnlyWithoutShoppingCart.DbType = DbType.Boolean;

                var pCreatedFromUtc = _dataProvider.GetParameter();
                pCreatedFromUtc.ParameterName = "CreatedFromUtc";
                pCreatedFromUtc.Value = createdFromUtc.HasValue ? (object)createdFromUtc.Value : DBNull.Value;
                pCreatedFromUtc.DbType = DbType.DateTime;

                var pCreatedToUtc = _dataProvider.GetParameter();
                pCreatedToUtc.ParameterName = "CreatedToUtc";
                pCreatedToUtc.Value = createdToUtc.HasValue ? (object)createdToUtc.Value : DBNull.Value;
                pCreatedToUtc.DbType = DbType.DateTime;

                var pTotalRecordsDeleted = _dataProvider.GetParameter();
                pTotalRecordsDeleted.ParameterName = "TotalRecordsDeleted";
                pTotalRecordsDeleted.Direction = ParameterDirection.Output;
                pTotalRecordsDeleted.DbType = DbType.Int32;

                //invoke stored procedure
                _dbContext.ExecuteSqlCommand(
                    "EXEC [DeleteGuests] @OnlyWithoutShoppingCart, @CreatedFromUtc, @CreatedToUtc, @TotalRecordsDeleted OUTPUT",
                    false, null,
                    pOnlyWithoutShoppingCart,
                    pCreatedFromUtc,
                    pCreatedToUtc,
                    pTotalRecordsDeleted);

                int totalRecordsDeleted = (pTotalRecordsDeleted.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecordsDeleted.Value) : 0;
                return totalRecordsDeleted;

                #endregion
            }
            else
            {
                var guestRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
                if (guestRole == null)
                    throw new NopException("'Guests' role could not be loaded");

                var query = _customerRepository.Table;
                if (createdFromUtc.HasValue)
                    query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
                if (createdToUtc.HasValue)
                    query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
                query = query.Where(c => c.CustomerRoles.Select(cr => cr.Id).Contains(guestRole.Id));
                if (onlyWithoutShoppingCart)
                    query = query.Where(c => !c.ShoppingCartItems.Any());

                //no orders
                query = from c in query
                        join o in _orderRepository.Table on c.Id equals o.CustomerId into c_o
                        from o in c_o.DefaultIfEmpty()
                        where !c_o.Any()
                        select c;
                //no blog comments
                query = from c in query
                        join b in _blogCommentRepository.Table on c.Id equals b.CustomerId into c_b
                        from b in c_b.DefaultIfEmpty()
                        where !c_b.Any()
                        select c;
                query = from c in query
                        join n in _newsCommentRepository.Table on c.Id equals n.CustomerId into c_a
                        from n in c_a.DefaultIfEmpty()
                        where !c_a.Any()
                        select c;
                query = from c in query
                        join p in _productReviewRepository.Table on c.Id equals p.CustomerId into c_p
                        from p in c_p.DefaultIfEmpty()
                        where !c_p.Any()
                        select c;
                query = from c in query
                        join p in _productReviewHelpfulnessRepository.Table on c.Id equals p.CustomerId into c_p
                        from p in c_p.DefaultIfEmpty()
                        where !c_p.Any()
                        select c;
                query = from c in query
                        join v in _pollVotingRecordRepository.Table on c.Id equals v.CustomerId into c_v
                        from v in c_v.DefaultIfEmpty()
                        where !c_v.Any()
                        select c;
                query = from c in query
                        join f in _forumPostRepository.Table on c.Id equals f.CustomerId into c_f
                        from f in c_f.DefaultIfEmpty()
                        where !c_f.Any()
                        select c;
                query = from c in query
                        join t in _forumTopicRepository.Table on c.Id equals t.CustomerId into c_t
                        from t in c_t.DefaultIfEmpty()
                        where !c_t.Any()
                        select c;
                query = query.Where(m => !m.IsSystemAccount);
                query = from c in query
                        group c by c.Id into cGroup
                        orderby cGroup.Key
                        select cGroup.FirstOrDefault();
                query = query.OrderBy(m => m.Id);
                var customers = query.ToList();

                int totalRecordDeleted = 0;
                foreach(var customer in customers)
                {
                    try
                    {
                        var attributes = _genericAttributeService.GetAttributesForEntity(customer.Id, "Customer");
                        foreach (var attribute in attributes)
                            _genericAttributeService.DeleteAttribute(attribute);

                        _customerRepository.Delete(customer);
                        totalRecordDeleted++;
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc);
                    }
                }
                return totalRecordDeleted;
            }
        }

        public IList<CustomerRole> GetAllCustomerRoles(bool showHidden = false)
        {
            string key = string.Format(CUSTOMERROLES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _customerRoleRepository.Table
                            orderby c.Name
                            where (showHidden || c.Active)
                            select c;
                return query.ToList();
            });
        }

        public IPagedList<Customer> GetAllCustomers(
            DateTime? createdFromUtc = default(DateTime?), DateTime? createdToUtc = default(DateTime?), int affiliateId = 0, int vendorId = 0, 
            int[] customerRoleIds = null, string email = null, string username = null, string firstName = null, string lastName = null, 
            int dayOfBirth = 0, int monthOfBirth = 0, string company = null, string phone = null, string zipPostalCode = null, bool loadOnlyWithShoppingCart = false, 
            ShoppingCartType? sct = default(ShoppingCartType?), int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _customerRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(m => m.CreatedOnUtc >= createdFromUtc.Value);
            if (createdToUtc.HasValue)
                query = query.Where(m => m.CreatedOnUtc >= createdToUtc.Value);
            if (affiliateId > 0)
                query = query.Where(m => m.AffiliateId == affiliateId);
            if (vendorId > 0)
                query = query.Where(m => m.VendorId == vendorId);
            query = query.Where(m => !m.Deleted);
            if (customerRoleIds != null && customerRoleIds.Length > 0)
                query = query.Where(m => m.CustomerRoles.Select(r => r.Id).Intersect(customerRoleIds).Any());
            if (!string.IsNullOrEmpty(email))
                query = query.Where(m => m.Email == email);
            if (!string.IsNullOrEmpty(username))
                query = query.Where(m => m.Username == username);
            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(m => m.Attribute.KeyGroup == "Cusomer" && m.Attribute.Key == SystemCustomerAttributeNames.FirstName && m.Attribute.Value.Contains(firstName))
                    .Select(m => m.Customer);
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(m => m.Attribute.KeyGroup == "Cusomer" && m.Attribute.Key == SystemCustomerAttributeNames.LastName && m.Attribute.Value.Contains(lastName))
                    .Select(m => m.Customer);
            }
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                string dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InstalledUICulture) + "-" + dayOfBirth.ToString("00", CultureInfo.InstalledUICulture);
                query = query.Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(m => m.Attribute.KeyGroup == "Cusomer" && m.Attribute.Key == SystemCustomerAttributeNames.DateOfBirth && m.Attribute.Value.Substring(5, 5).Contains(dateOfBirthStr))
                    .Select(m => m.Customer);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                string dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                //dateOfBirthStr.Length = 2
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                        z.Attribute.Key == SystemCustomerAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(8, 2) == dateOfBirthStr))
                    .Select(z => z.Customer);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                string dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                        z.Attribute.Key == SystemCustomerAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Contains(dateOfBirthStr)))
                    .Select(z => z.Customer);
            }
            //search by company
            if (!String.IsNullOrWhiteSpace(company))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                        z.Attribute.Key == SystemCustomerAttributeNames.Company &&
                        z.Attribute.Value.Contains(company)))
                    .Select(z => z.Customer);
            }
            //search by phone
            if (!String.IsNullOrWhiteSpace(phone))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                        z.Attribute.Key == SystemCustomerAttributeNames.Phone &&
                        z.Attribute.Value.Contains(phone)))
                    .Select(z => z.Customer);
            }
            //search by zip
            if (!String.IsNullOrWhiteSpace(zipPostalCode))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                        z.Attribute.Key == SystemCustomerAttributeNames.ZipPostalCode &&
                        z.Attribute.Value.Contains(zipPostalCode)))
                    .Select(z => z.Customer);
            }
            if (loadOnlyWithShoppingCart)
            {
                int? sctId = null;
                if (sct.HasValue)
                    sctId = (int)sctId.Value;
                query = sctId.HasValue ?
                    query.Where(m => m.ShoppingCartItems.Any(c => c.ShoppingCartTypeId == sctId)) :
                    query.Where(m => m.ShoppingCartItems.Any());
            }
            query = query.OrderByDescending(m => m.CreatedOnUtc);
            var customers = new PagedList<Customer>(query, pageIndex, pageSize);
            return customers;
        }

        public IList<Customer> GetAllCustomersByPasswordFormat(PasswordFormat passwordFormat)
        {
            var passwordFormatId = (int)passwordFormat;
            var query = _customerRepository.Table;
            query = query.Where(m => m.PasswordFormatId == passwordFormatId);
            query = query.OrderByDescending(m => m.CreatedOnUtc);
            return query.ToList();
        }

        public Customer GetCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        public Customer GetCustomerByGuid(Guid customerGuid)
        {
            if (customerGuid == Guid.Empty)
                return null;

            var query = from c in _customerRepository.Table
                        where c.CustomerGuid == customerGuid
                        orderby c.Id
                        select c;
            return query.FirstOrDefault();
        }

        public Customer GetCustomerById(int customerId)
        {
            if (customerId == 0)
                return null;

            return _customerRepository.GetById(customerId);
        }

        public Customer GetCustomerBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.SystemName == systemName
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        public Customer GetCustomerByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Username == username
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        public CustomerRole GetCustomerRoleById(int customerRoleId)
        {
            if (customerRoleId == 0)
                return null;

            return _customerRoleRepository.GetById(customerRoleId);
        }

        public CustomerRole GetCustomerRoleBySystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            string key = string.Format(CUSTOMERROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, () => 
            {
                var query = from cr in _customerRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var customerRole = query.FirstOrDefault();
                return customerRole;
            });
        }

        public IList<Customer> GetCustomersByIds(int[] customerIds)
        {
            if(customerIds ==null ||customerIds.Length == 0)
            {
                return new List<Customer>();
            }

            var query = from c in _customerRepository.Table
                        where customerIds.Contains(c.Id)
                        select c;
            var customers = query.ToList();
            var sortedCustomers = new List<Customer>();
            foreach(var id in customerIds)
            {
                var customer = customers.FirstOrDefault(m => m.Id == id);
                if (customer != null)
                    sortedCustomers.Add(customer);
            }
            return sortedCustomers;
        }

        public IPagedList<Customer> GetOnlineCustomers(DateTime lastActivityFromUtc, int[] customerRoleIds, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _customerRepository.Table;
            query = query.Where(m => m.LastActivityDateUtc > lastActivityFromUtc);
            query = query.Where(m => !m.Deleted);
            if(customerRoleIds != null && customerRoleIds.Length > 0)
            {
                query = query.Where(m => m.CustomerRoles.Select(c => c.Id).Intersect(customerRoleIds).Any());
            }
            query = query.OrderByDescending(m => m.CreatedOnUtc);
            var customers = new PagedList<Customer>(query, pageIndex, pageSize);
            return customers;
        }

        public void InsertCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            _customerRepository.Insert(customer);

            //event notification
            _eventPublisher.EntityInserted(customer);
        }

        public void InsertCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            _customerRoleRepository.Insert(customerRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);

            _eventPublisher.EntityInserted(customerRole);
        }

        public Customer InsertGuestCustomer()
        {
            var customer = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                Active =true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            var gustRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (gustRole == null)
                throw new NopException("访客没有角色");

            customer.CustomerRoles.Add(gustRole);
            _customerRepository.Insert(customer);
            return customer;
        }

        public void ResetCheckoutData(Customer customer, int storeId, bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
            bool clearRewardPoints = true, bool clearShippingMethod = true, bool clearPaymentMethod = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (clearCouponCodes)
            {
                _genericAttributeService.SaveAttribute<ShippingOption>(customer, SystemCustomerAttributeNames.DiscountCouponCode, null);
                _genericAttributeService.SaveAttribute<ShippingOption>(customer, SystemCustomerAttributeNames.GiftCardCouponCodes, null);
            }

            if (clearCheckoutAttributes)
            {
                _genericAttributeService.SaveAttribute<ShippingOption>(customer, SystemCustomerAttributeNames.CheckoutAttributes, null, storeId);
            }
            if (clearRewardPoints)
            {
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, false, storeId);
            }
            if (clearShippingMethod)
            {
                _genericAttributeService.SaveAttribute<ShippingOption>(customer, SystemCustomerAttributeNames.SelectedShippingOption, null, storeId);
                _genericAttributeService.SaveAttribute<ShippingOption>(customer, SystemCustomerAttributeNames.OfferedShippingOptions, null, storeId);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.SelectedPickUpInStore, false, storeId);
            }
            if (clearPaymentMethod)
            {
                _genericAttributeService.SaveAttribute<string>(customer, SystemCustomerAttributeNames.SelectedPaymentMethod, null, storeId);
            }

            UpdateCustomer(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            _customerRepository.Update(customer);

            //event notification
            _eventPublisher.EntityUpdated(customer);
        }

        public void UpdateCustomerRole(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException("customerRole");

            _customerRoleRepository.Update(customerRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);

            _eventPublisher.EntityUpdated(customerRole);
        }
    }
}
