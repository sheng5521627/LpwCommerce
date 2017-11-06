using System.Collections.Generic;
using Web.Framework.Mvc;
using WebSite.Models.Common;

namespace WebSite.Models.Customer
{
    public partial class CustomerAddressListModel : BaseNopModel
    {
        public CustomerAddressListModel()
        {
            Addresses = new List<AddressModel>();
        }

        public IList<AddressModel> Addresses { get; set; }
    }
}