using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Customers;
using Core.Domain.Directory;
using Core.Domain.Localization;
using Core.Domain.Tax;
using Core.Domain.Vendors;

namespace Core
{
    public interface IWorkContext
    {
        /// <summary>
        /// 
        /// </summary>
        Customer CurrentCustomer { get; set; }

        /// <summary>
        /// 模拟用户
        /// </summary>
        Customer OriginalCustomerIfImpersonated { get; }

        /// <summary>
        /// 卖主
        /// </summary>
        Vendor CurrentVendor { get; }

        Language WorkingLanguage { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        Currency WorkingCurrency { get; set; }

        /// <summary>
        /// 显示税收模式
        /// </summary>
        TaxDisplayType TaxDisplayType { get; set; }

        bool IsAdmin { get; set; }
    }
}
