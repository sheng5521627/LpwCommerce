using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Customers;
using Core.Domain.Orders;
using Core.Domain.Payments;
using Core.Domain.Shipping;
using Core.Data;
using Services.Helpers;

namespace Services.Customers
{
    public partial class CustomerReportService : ICustomerReportService
    {
        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="dateTimeHelper">Date time helper</param>
        public CustomerReportService(IRepository<Customer> customerRepository,
            IRepository<Order> orderRepository, ICustomerService customerService,
            IDateTimeHelper dateTimeHelper)
        {
            this._customerRepository = customerRepository;
            this._orderRepository = orderRepository;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        public IPagedList<BestCustomerReportLine> GetBestCustomersReport(DateTime? createdFromUtc, DateTime? createdToUtc, OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, int orderBy, int pageIndex = 0, int pageSize = 214748364)
        {
            int? orderStatusId = null;
            if (os.HasValue)
            {
                orderStatusId = (int)os.Value;
            }
            int? paymentStatus = null;
            if (ps.HasValue)
            {
                paymentStatus = (int)ps.Value;
            }
            int? shippingStatus = null;
            if (ss.HasValue)
            {
                shippingStatus = (int)ss.Value;
            }
            var query1 = from c in _customerRepository.Table
                         join o in _orderRepository.Table on c.Id equals o.CustomerId
                         where (!createdFromUtc.HasValue || createdFromUtc.Value <= c.CreatedOnUtc) &&
                         (!createdToUtc.HasValue || createdToUtc.Value >= c.CreatedOnUtc) &&
                         (!orderStatusId.HasValue || orderStatusId.Value == o.OrderStatusId) &&
                         (!paymentStatus.HasValue || paymentStatus.Value == o.PaymentStatusId) &&
                         (!shippingStatus.HasValue || shippingStatus.Value == o.ShippingStatusId) &&
                         !o.Deleted &&
                         !c.Deleted
                         select new { c, o };
            var query2 = from co in query1
                         group co by co.c.Id into g
                         select new
                         {
                             CustomerId = g.Key,
                             OrderTotal = g.Sum(x => x.o.OrderTotal),
                             OrderCount = g.Count()
                         };
            switch (orderBy)
            {
                case 1:
                    query2 = query2.OrderByDescending(x => x.OrderTotal);
                    break;
                case 2:
                    query2 = query2.OrderByDescending(x => x.OrderCount);
                    break;
                default:
                    throw new ArgumentException("Wrong orderBy parameter", "orderBy");
            }

            var temp = new PagedList<dynamic>(query2, pageIndex, pageSize);
            return new PagedList<BestCustomerReportLine>(temp.Select(m => new BestCustomerReportLine()
            {
                CustomerId = m.CustomerId,
                OrderTotal = m.OrderTotal,
                OrderCount = m.OrderCount
            }),
                temp.PageIndex, temp.PageSize, temp.TotalCount);
        }

        public int GetRegisteredCustomersReport(int days)
        {
            DateTime date = _dateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);
            var registeredCustomerRole = _customerService.GetCustomerBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredCustomerRole == null)
                return 0;

            var query = from c in _customerRepository.Table
                        from cr in c.CustomerRoles
                        where !c.Deleted && cr.Id == registeredCustomerRole.Id
                        && c.CreatedOnUtc >= date
                        select c;
            return query.Count();
        }
    }
}
