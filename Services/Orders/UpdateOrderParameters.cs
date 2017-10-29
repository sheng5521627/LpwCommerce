﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Orders;
using Core.Domain.Shipping;
using Services.Discounts;

namespace Services.Orders
{
    /// <summary>
    /// Parameters for the updating order totals
    /// </summary>
    public class UpdateOrderParameters
    {
        public UpdateOrderParameters()
        {
            Warnings = new List<string>();
            AppliedDiscounts = new List<DiscountForCaching>();
        }

        /// <summary>
        /// The updated order
        /// </summary>
        public Order UpdatedOrder { get; set; }

        /// <summary>
        /// The updated order item
        /// </summary>
        public OrderItem UpdatedOrderItem { get; set; }

        /// <summary>
        /// The price of item with tax
        /// </summary>
        public decimal PriceInclTax { get; set; }

        /// <summary>
        /// The price of item without tax
        /// </summary>
        public decimal PriceExclTax { get; set; }

        /// <summary>
        /// The quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The amount of discount with tax
        /// </summary>
        public decimal DiscountAmountInclTax { get; set; }

        /// <summary>
        /// The amount of discount without tax
        /// </summary>
        public decimal DiscountAmountExclTax { get; set; }

        /// <summary>
        /// Subtotal of item with tax
        /// </summary>
        public decimal SubTotalInclTax { get; set; }

        /// <summary>
        /// Subtotal of item without tax
        /// </summary>
        public decimal SubTotalExclTax { get; set; }

        /// <summary>
        /// Warnings
        /// </summary>
        public List<string> Warnings { get; set; }

        /// <summary>
        /// Applied discounts
        /// </summary>
        public List<DiscountForCaching> AppliedDiscounts { get; set; }

        /// <summary>
        /// Pickup point
        /// </summary>
        public PickupPoint PickupPoint { get; set; }
    }
}
