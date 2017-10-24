using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Customers;

namespace Services.Customers
{
    class CustomerRegistrationService : ICustomerRegistrationService
    {
        public ChangePasswordResult ChangePassword(ChangePasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {
            throw new NotImplementedException();
        }

        public void SetEmail(Customer customer, string newEmail)
        {
            throw new NotImplementedException();
        }

        public void SetUsername(Customer customer, string newUsername)
        {
            throw new NotImplementedException();
        }

        public CustomerLoginResults ValidateCustomer(string usernameOrEmail, string password)
        {
            throw new NotImplementedException();
        }
    }
}
