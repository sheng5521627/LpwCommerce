using Core.Data;
using Core.Domain.Orders;
using Core.Domain.Shipping;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Domain.Catalog;

namespace Services.Shipping
{
    /// <summary>
    /// Shipment service
    /// </summary>
    public partial class ShipmentService : IShipmentService
    {
        #region Fields

        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<ShipmentItem> _siRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shipmentRepository">Shipment repository</param>
        /// <param name="siRepository">Shipment item repository</param>
        /// <param name="orderItemRepository">Order item repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ShipmentService(IRepository<Shipment> shipmentRepository,
            IRepository<ShipmentItem> siRepository,
            IRepository<OrderItem> orderItemRepository,
            IEventPublisher eventPublisher)
        {
            this._shipmentRepository = shipmentRepository;
            this._siRepository = siRepository;
            this._orderItemRepository = orderItemRepository;
            this._eventPublisher = eventPublisher;
        }

        public void DeleteShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Delete(shipment);

            //event notification
            _eventPublisher.EntityDeleted(shipment);
        }

        public IPagedList<Shipment> GetAllShipments(int vendorId = 0, int warehouseId = 0, int shippingCountryId = 0, int shippingStateId = 0, 
            string shippingCity = null, string trackingNumber = null, bool loadNotShipped = false, 
            DateTime? createdFromUtc = default(DateTime?), DateTime? createdToUtc = default(DateTime?), int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _shipmentRepository.Table;
            if (!String.IsNullOrEmpty(trackingNumber))
                query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));
            if (shippingCountryId > 0)
                query = query.Where(s => s.Order.ShippingAddress.CountryId == shippingCountryId);
            if (shippingStateId > 0)
                query = query.Where(s => s.Order.ShippingAddress.StateProvinceId == shippingStateId);
            if (!String.IsNullOrWhiteSpace(shippingCity))
                query = query.Where(s => s.Order.ShippingAddress.City.Contains(shippingCity));
            if (loadNotShipped)
                query = query.Where(s => !s.ShippedDateUtc.HasValue);
            if (createdFromUtc.HasValue)
                query = query.Where(s => createdFromUtc.Value <= s.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(s => createdToUtc.Value >= s.CreatedOnUtc);
            query = query.Where(s => s.Order != null && !s.Order.Deleted);
            if (vendorId > 0)
            {
                var queryVendorOrderItems = from orderItem in _orderItemRepository.Table
                                            where orderItem.Product.VendorId == vendorId
                                            select orderItem.Id;

                query = from s in query
                        where queryVendorOrderItems.Intersect(s.ShipmentItems.Select(si => si.OrderItemId)).Any()
                        select s;
            }
            if (warehouseId > 0)
            {
                query = from s in query
                        where s.ShipmentItems.Any(si => si.WarehouseId == warehouseId)
                        select s;
            }
            query = query.OrderByDescending(s => s.CreatedOnUtc);

            var shipments = new PagedList<Shipment>(query, pageIndex, pageSize);
            return shipments;
        }

        public IList<Shipment> GetShipmentsByIds(int[] shipmentIds)
        {
            if (shipmentIds == null || shipmentIds.Length == 0)
                return new List<Shipment>();

            var query = from o in _shipmentRepository.Table
                        where shipmentIds.Contains(o.Id)
                        select o;
            var shipments = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<Shipment>();
            foreach (int id in shipmentIds)
            {
                var shipment = shipments.Find(x => x.Id == id);
                if (shipment != null)
                    sortedOrders.Add(shipment);
            }
            return sortedOrders;
        }

        public Shipment GetShipmentById(int shipmentId)
        {
            if (shipmentId == 0)
                return null;

            return _shipmentRepository.GetById(shipmentId);
        }

        public void InsertShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Insert(shipment);

            //event notification
            _eventPublisher.EntityInserted(shipment);
        }

        public void UpdateShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            _shipmentRepository.Update(shipment);

            //event notification
            _eventPublisher.EntityUpdated(shipment);
        }

        public void DeleteShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException("shipmentItem");

            _siRepository.Delete(shipmentItem);

            //event notification
            _eventPublisher.EntityDeleted(shipmentItem);
        }

        public ShipmentItem GetShipmentItemById(int shipmentItemId)
        {
            if (shipmentItemId == 0)
                return null;

            return _siRepository.GetById(shipmentItemId);
        }

        public void InsertShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException("shipmentItem");

            _siRepository.Insert(shipmentItem);

            //event notification
            _eventPublisher.EntityInserted(shipmentItem);
        }

        public void UpdateShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException("shipmentItem");

            _siRepository.Update(shipmentItem);

            //event notification
            _eventPublisher.EntityUpdated(shipmentItem);
        }

        public int GetQuantityInShipments(Product product, int warehouseId, bool ignoreShipped, bool ignoreDelivered)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
                return 0;
            if (!product.UseMultipleWarehouses)
                return 0;

            const int cancelledOrderStatusId = (int)OrderStatus.Cancelled;


            var query = _siRepository.Table;
            query = query.Where(si => !si.Shipment.Order.Deleted);
            query = query.Where(si => si.Shipment.Order.OrderStatusId != cancelledOrderStatusId);
            if (warehouseId > 0)
                query = query.Where(si => si.WarehouseId == warehouseId);
            if (ignoreShipped)
                query = query.Where(si => !si.Shipment.ShippedDateUtc.HasValue);
            if (ignoreDelivered)
                query = query.Where(si => !si.Shipment.DeliveryDateUtc.HasValue);

            var queryProductOrderItems = from orderItem in _orderItemRepository.Table
                                         where orderItem.ProductId == product.Id
                                         select orderItem.Id;
            query = from si in query
                    where queryProductOrderItems.Any(orderItemId => orderItemId == si.OrderItemId)
                    select si;

            //some null validation
            var result = Convert.ToInt32(query.Sum(si => (int?)si.Quantity));
            return result;
        }

        #endregion
    }
}
