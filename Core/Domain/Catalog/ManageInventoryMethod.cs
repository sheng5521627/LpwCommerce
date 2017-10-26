using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Catalog
{
    /// <summary>
    /// 表示库存管理的方法
    /// </summary>
    public enum ManageInventoryMethod
    {
        /// <summary>
        /// Don't track inventory for product
        /// 不要跟踪产品的库存
        /// </summary>
        DontManageStock = 0,
        /// <summary>
        /// Track inventory for product
        /// 产品跟踪库存
        /// </summary>
        ManageStock = 1,
        /// <summary>
        /// Track inventory for product by product attributes
        /// 按产品属性跟踪产品库存
        /// </summary>
        ManageStockByAttributes = 2,
    }
}
