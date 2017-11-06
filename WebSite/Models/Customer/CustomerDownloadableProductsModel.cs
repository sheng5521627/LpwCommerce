﻿using System;
using System.Collections.Generic;
using Web.Framework.Mvc;

namespace WebSite.Models.Customer
{
    public partial class CustomerDownloadableProductsModel : BaseNopModel
    {
        public CustomerDownloadableProductsModel()
        {
            Items = new List<DownloadableProductsModel>();
        }

        public IList<DownloadableProductsModel> Items { get; set; }

        #region Nested classes
        public partial class DownloadableProductsModel : BaseNopModel
        {
            public Guid OrderItemGuid { get; set; }

            public int OrderId { get; set; }

            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string ProductAttributes { get; set; }

            public int DownloadId { get; set; }
            public int LicenseId { get; set; }

            public DateTime CreatedOn { get; set; }
        }
        #endregion
    }

    public partial class UserAgreementModel : BaseNopModel
    {
        public Guid OrderItemGuid { get; set; }
        public string UserAgreementText { get; set; }
    }
}