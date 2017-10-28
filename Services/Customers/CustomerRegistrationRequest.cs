using Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Customers
{
    public class CustomerRegistrationRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="email"></param>
        /// <param name="usernaem"></param>
        /// <param name="password"></param>
        /// <param name="passwordFormat"></param>
        /// <param name="storeId"></param>
        /// <param name="isApproved">是否同意</param>
        public CustomerRegistrationRequest(Customer customer,string email,string username,
            string password,PasswordFormat passwordFormat,int storeId,bool isApproved = true)
        {
            this.Customer = customer;
            this.Email = email;
            this.Username = username;
            this.Password = password;
            this.PasswordFormat = passwordFormat;
            this.StoreId = storeId;
            this.IsApproved = isApproved;
        }

        // <summary>
        /// Customer
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Password format
        /// </summary>
        public PasswordFormat PasswordFormat { get; set; }
        /// <summary>
        /// Store identifier
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// Is approved
        /// </summary>
        public bool IsApproved { get; set; }
    }
}
