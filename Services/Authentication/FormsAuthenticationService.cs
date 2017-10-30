using Core.Domain.Customers;
using Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Services.Authentication
{
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly TimeSpan _expirationTimeSpan;
        private Customer _cacheCustomer;

        public FormsAuthenticationService(
            HttpContextBase httpContextBase,
            ICustomerService customerService,
            CustomerSettings customerSettings)
        {
            this._httpContext = httpContextBase;
            this._customerService = customerService;
            this._customerSettings = customerSettings;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
        }


        /// <summary>
        /// 从票据中获取用户登录信息
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        protected virtual Customer GetAuthenticatedCustomerFormTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var usernameOrEmail = ticket.UserData;
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
                return null;

            var customer = _customerSettings.UsernamesEnabled ?
                _customerService.GetCustomerByUsername(usernameOrEmail) :
                _customerService.GetCustomerByEmail(usernameOrEmail);

            return customer;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="createPersistentCookie"></param>
        public virtual void SignIn(Customer customer, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                    1,
                    _customerSettings.UsernamesEnabled ? customer.Username : customer.Email,
                    now,
                    now.Add(_expirationTimeSpan),
                    createPersistentCookie,
                    _customerSettings.UsernamesEnabled ? customer.Username : customer.Email,
                    FormsAuthentication.FormsCookiePath
                );

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.SetCookie(cookie);
            _cacheCustomer = customer;
        }

        public virtual void SignOut()
        {
            _cacheCustomer = null;
            FormsAuthentication.SignOut();
        }

        public virtual Customer GetAuthenticatedCustomer()
        {
            if (_cacheCustomer != null)
                return _cacheCustomer;

            if (_httpContext == null || _httpContext.Request == null ||
               !_httpContext.Request.IsAuthenticated || !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var customer = GetAuthenticatedCustomerFormTicket(formsIdentity.Ticket);
            if (customer != null && customer.Active && !customer.Deleted && customer.IsRegistered())
                _cacheCustomer = customer;
            return _cacheCustomer;
        }
    }
}
