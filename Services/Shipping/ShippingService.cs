using Core;
using Core.Caching;
using Core.Data;
using Core.Domain.Orders;
using Core.Domain.Shipping;
using Core.Plugins;
using Services.Catalog;
using Services.Common;
using Services.Events;
using Services.Localization;
using Services.Logging;
using Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Catalog;
using Core.Domain.Customers;
using Services.Shipping.Pickup;

namespace Services.Shipping
{
    /// <summary>
    /// Shipping service
    /// </summary>
    public partial class ShippingService : IShippingService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : warehouse ID
        /// </remarks>
        private const string WAREHOUSES_BY_ID_KEY = "Nop.warehouse.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string WAREHOUSES_PATTERN_KEY = "Nop.warehouse.";

        #endregion

        #region Fields

        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly IRepository<DeliveryDate> _deliveryDateRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IAddressService _addressService;
        private readonly ShippingSettings _shippingSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shippingMethodRepository">Shipping method repository</param>
        /// <param name="deliveryDateRepository">Delivery date repository</param>
        /// <param name="warehouseRepository">Warehouse repository</param>
        /// <param name="logger">Logger</param>
        /// <param name="productService">Product service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="addressService">Address service</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="cacheManager">Cache manager</param>
        public ShippingService(IRepository<ShippingMethod> shippingMethodRepository,
            IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<Warehouse> warehouseRepository,
            ILogger logger,
            IProductService productService,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeParser checkoutAttributeParser,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IAddressService addressService,
            ShippingSettings shippingSettings,
            IPluginFinder pluginFinder,
            IStoreContext storeContext,
            IEventPublisher eventPublisher,
            ShoppingCartSettings shoppingCartSettings,
            ICacheManager cacheManager)
        {
            this._shippingMethodRepository = shippingMethodRepository;
            this._deliveryDateRepository = deliveryDateRepository;
            this._warehouseRepository = warehouseRepository;
            this._logger = logger;
            this._productService = productService;
            this._productAttributeParser = productAttributeParser;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._addressService = addressService;
            this._shippingSettings = shippingSettings;
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._shoppingCartSettings = shoppingCartSettings;
            this._cacheManager = cacheManager;
        }

        #region Shipping rate computation methods

        public IList<IShippingRateComputationMethod> LoadActiveShippingRateComputationMethods(int storeId = 0)
        {
            return LoadActiveShippingRateComputationMethods(storeId)
                .Where(m => _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Contains(m.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                .ToList();
        }

        public IShippingRateComputationMethod LoadShippingRateComputationMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IShippingRateComputationMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IShippingRateComputationMethod>();
            return null;
        }
        
        public IList<IShippingRateComputationMethod> LoadAllShippingRateComputationMethods(int storeId = 0)
        {
            return _pluginFinder.GetPluginsNews<IShippingRateComputationMethod>(storeId: storeId).ToList();
        }

        #endregion

        /// <summary>
        /// Load active pickup point providers
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Pickup point providers</returns>
        public virtual IList<IPickupPointProvider> LoadActivePickupPointProviders(Customer customer = null, int storeId = 0)
        {
            return LoadAllPickupPointProviders(customer, storeId).Where(provider => _shippingSettings.ActivePickupPointProviderSystemNames
                .Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }
        /// <summary>
        /// Load all pickup point providers
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Pickup point providers</returns>
        public virtual IList<IPickupPointProvider> LoadAllPickupPointProviders(Customer customer = null, int storeId = 0)
        {
            return _pluginFinder.GetPluginsNews<IPickupPointProvider>(customer: customer, storeId: storeId).ToList();
        }

        /// <summary>
        /// Load active shipping rate computation methods
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Shipping rate computation methods</returns>
        public virtual IList<IShippingRateComputationMethod> LoadActiveShippingRateComputationMethods(Customer customer = null, int storeId = 0)
        {
            return LoadAllShippingRateComputationMethods(customer, storeId)
                .Where(provider => _shippingSettings.ActiveShippingRateComputationMethodSystemNames
                    .Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }
        /// <summary>
        /// Load all shipping rate computation methods
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Shipping rate computation methods</returns>
        public virtual IList<IShippingRateComputationMethod> LoadAllShippingRateComputationMethods(Customer customer = null, int storeId = 0)
        {
            return _pluginFinder.GetPluginsNews<IShippingRateComputationMethod>(customer: customer, storeId: storeId).ToList();
        }

        public void DeleteShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            _shippingMethodRepository.Delete(shippingMethod);

            //event notification
            _eventPublisher.EntityDeleted(shippingMethod);
        }

        public ShippingMethod GetShippingMethodById(int shippingMethodId)
        {
            if (shippingMethodId == 0)
                return null;

            return _shippingMethodRepository.GetById(shippingMethodId);
        }

        public IList<ShippingMethod> GetAllShippingMethods(int? filterByCountryId = default(int?))
        {
            if(filterByCountryId.HasValue && filterByCountryId.Value > 0)
            {
                var query1 = from sm in _shippingMethodRepository.Table
                             where sm.RestrictedCountries.Select(m => m.Id).Contains(filterByCountryId.Value)
                             select sm.Id;
                var query2 = from sm in _shippingMethodRepository.Table
                             where !query1.Contains(sm.Id)
                             orderby sm.DisplayOrder
                             select sm;
                return query2.ToList();
            }
            else
            {
                var query = from sm in _shippingMethodRepository.Table
                            orderby sm.DisplayOrder
                            select sm;
                return query.ToList();
            }
        }

        public void InsertShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            _shippingMethodRepository.Insert(shippingMethod);

            //event notification
            _eventPublisher.EntityInserted(shippingMethod);
        }

        public void UpdateShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            _shippingMethodRepository.Update(shippingMethod);

            //event notification
            _eventPublisher.EntityUpdated(shippingMethod);
        }

        public void DeleteDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException("deliveryDate");

            _deliveryDateRepository.Delete(deliveryDate);

            //event notification
            _eventPublisher.EntityDeleted(deliveryDate);
        }

        public DeliveryDate GetDeliveryDateById(int deliveryDateId)
        {
            if (deliveryDateId == 0)
                return null;

            return _deliveryDateRepository.GetById(deliveryDateId);
        }

        public IList<DeliveryDate> GetAllDeliveryDates()
        {
            var query = from dd in _deliveryDateRepository.Table
                        orderby dd.DisplayOrder
                        select dd;
            var deliveryDates = query.ToList();
            return deliveryDates;
        }

        public void InsertDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException("deliveryDate");

            _deliveryDateRepository.Insert(deliveryDate);

            //event notification
            _eventPublisher.EntityInserted(deliveryDate);
        }

        public void UpdateDeliveryDate(DeliveryDate deliveryDate)
        {
            if (deliveryDate == null)
                throw new ArgumentNullException("deliveryDate");

            _deliveryDateRepository.Update(deliveryDate);

            //event notification
            _eventPublisher.EntityUpdated(deliveryDate);
        }

        public void DeleteWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException("warehouse");

            _warehouseRepository.Delete(warehouse);

            //clear cache
            _cacheManager.RemoveByPattern(WAREHOUSES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(warehouse);
        }

        public Warehouse GetWarehouseById(int warehouseId)
        {
            if (warehouseId == 0)
                return null;

            string key = string.Format(WAREHOUSES_BY_ID_KEY, warehouseId);
            return _cacheManager.Get(key, () => _warehouseRepository.GetById(warehouseId));
        }

        public IList<Warehouse> GetAllWarehouses()
        {
            var query = from wh in _warehouseRepository.Table
                        orderby wh.Name
                        select wh;
            var warehouses = query.ToList();
            return warehouses;
        }

        public void InsertWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException("warehouse");

            _warehouseRepository.Insert(warehouse);

            //clear cache
            _cacheManager.RemoveByPattern(WAREHOUSES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(warehouse);
        }

        public void UpdateWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException("warehouse");

            _warehouseRepository.Update(warehouse);

            //clear cache
            _cacheManager.RemoveByPattern(WAREHOUSES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(warehouse);
        }

        public decimal GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            if (shoppingCartItem.Product == null)
                return decimal.Zero;

            //attribute weight
            decimal attributesTotalWeight = decimal.Zero;
            if (!String.IsNullOrEmpty(shoppingCartItem.AttributesXml))
            {
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml);
                foreach (var attributeValue in attributeValues)
                {
                    switch (attributeValue.AttributeValueType)
                    {
                        case AttributeValueType.Simple:
                            {
                                //simple attribute
                                attributesTotalWeight += attributeValue.WeightAdjustment;
                            }
                            break;
                        case AttributeValueType.AssociatedToProduct:
                            {
                                //bundled product
                                var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                                if (associatedProduct != null && associatedProduct.IsShipEnabled)
                                {
                                    attributesTotalWeight += associatedProduct.Weight * attributeValue.Quantity;
                                }
                            }
                            break;
                    }
                }
            }

            var weight = shoppingCartItem.Product.Weight + attributesTotalWeight;
            return weight;
        }

        public decimal GetTotalWeight(GetShippingOptionRequest request, bool includeCheckoutAttributes = true)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            Customer customer = request.Customer;

            decimal totalWeight = decimal.Zero;
            //shopping cart items
            foreach (var packageItem in request.Items)
                totalWeight += GetShoppingCartItemWeight(packageItem.ShoppingCartItem) * packageItem.GetQuantity();

            //checkout attributes
            if (customer != null && includeCheckoutAttributes)
            {
                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
                if (!String.IsNullOrEmpty(checkoutAttributesXml))
                {
                    var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                    foreach (var attributeValue in attributeValues)
                        totalWeight += attributeValue.WeightAdjustment;
                }
            }
            return totalWeight;
        }

        /// <summary>
        /// 尺寸
        /// </summary>
        /// <param name="shoppingCartItem"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="height"></param>
        public void GetAssociatedProductDimensions(ShoppingCartItem shoppingCartItem, out decimal width, out decimal length, out decimal height)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            width = length = height = decimal.Zero;

            //attributes
            if (String.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return;

            //bundled products (associated attributes)
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(x => x.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .ToList();
            foreach (var attributeValue in attributeValues)
            {
                var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                if (associatedProduct != null && associatedProduct.IsShipEnabled)
                {
                    width += associatedProduct.Width * attributeValue.Quantity;
                    length += associatedProduct.Length * attributeValue.Quantity;
                    height += associatedProduct.Height * attributeValue.Quantity;
                }
            }
        }

        public void GetDimensions(IList<GetShippingOptionRequest.PackageItem> packageItems, out decimal width, out decimal length, out decimal height)
        {
            if (packageItems == null)
                throw new ArgumentNullException("packageItems");

            if (_shippingSettings.UseCubeRootMethod)
            {
                //cube root of volume
                decimal totalVolume = 0;
                decimal maxProductWidth = 0;
                decimal maxProductLength = 0;
                decimal maxProductHeight = 0;
                foreach (var packageItem in packageItems)
                {
                    var shoppingCartItem = packageItem.ShoppingCartItem;
                    var product = shoppingCartItem.Product;
                    var qty = packageItem.GetQuantity();

                    //associated products
                    decimal associatedProductsWidth;
                    decimal associatedProductsLength;
                    decimal associatedProductsHeight;
                    GetAssociatedProductDimensions(shoppingCartItem, out associatedProductsWidth,
                        out associatedProductsLength, out associatedProductsHeight);

                    var productWidth = product.Width + associatedProductsWidth;
                    var productLength = product.Length + associatedProductsLength;
                    var productHeight = product.Height + associatedProductsHeight;

                    //we do not use cube root method when we have only one item with "qty" set to 1
                    if (packageItems.Count == 1 && qty == 1)
                    {
                        width = productWidth;
                        length = productLength;
                        height = productHeight;
                        return;
                    }

                    totalVolume += qty * productHeight * productWidth * productLength;

                    if (productWidth > maxProductWidth)
                        maxProductWidth = productWidth;
                    if (productLength > maxProductLength)
                        maxProductLength = productLength;
                    if (productHeight > maxProductHeight)
                        maxProductHeight = productHeight;
                }
                decimal dimension = Convert.ToDecimal(Math.Pow(Convert.ToDouble(totalVolume), (double)(1.0 / 3.0)));
                length = width = height = dimension;

                //sometimes we have products with sizes like 1x1x20
                //that's why let's ensure that a maximum dimension is always preserved
                //otherwise, shipping rate computation methods can return low rates
                if (width < maxProductWidth)
                    width = maxProductWidth;
                if (length < maxProductLength)
                    length = maxProductLength;
                if (height < maxProductHeight)
                    height = maxProductHeight;
            }
            else
            {
                //summarize all values (very inaccurate with multiple items)
                width = length = height = decimal.Zero;
                foreach (var packageItem in packageItems)
                {
                    var shoppingCartItem = packageItem.ShoppingCartItem;
                    var product = shoppingCartItem.Product;
                    var qty = packageItem.GetQuantity();
                    width += product.Width * qty;
                    length += product.Length * qty;
                    height += product.Height * qty;

                    //associated products
                    decimal associatedProductsWidth;
                    decimal associatedProductsLength;
                    decimal associatedProductsHeight;
                    GetAssociatedProductDimensions(shoppingCartItem, out associatedProductsWidth,
                        out associatedProductsLength, out associatedProductsHeight);

                    width += associatedProductsWidth;
                    length += associatedProductsLength;
                    height += associatedProductsHeight;
                }
            }
        }

        public Warehouse GetNearestWarehouse(Address address, IList<Warehouse> warehouses = null)
        {
            warehouses = warehouses ?? GetAllWarehouses();

            //no address specified. return any
            if (address == null)
                return warehouses.FirstOrDefault();

            //of course, we should use some better logic to find nearest warehouse
            //but we don't have a built-in geographic database which supports "distance" functionality
            //that's why we simply look for exact matches

            //find by country
            var matchedByCountry = new List<Warehouse>();
            foreach (var warehouse in warehouses)
            {
                var warehouseAddress = _addressService.GetAddressById(warehouse.AddressId);
                if (warehouseAddress != null)
                    if (warehouseAddress.CountryId == address.CountryId)
                        matchedByCountry.Add(warehouse);
            }
            //no country matches. return any
            if (matchedByCountry.Count == 0)
                return warehouses.FirstOrDefault();


            //find by state
            var matchedByState = new List<Warehouse>();
            foreach (var warehouse in matchedByCountry)
            {
                var warehouseAddress = _addressService.GetAddressById(warehouse.AddressId);
                if (warehouseAddress != null)
                    if (warehouseAddress.StateProvinceId == address.StateProvinceId)
                        matchedByState.Add(warehouse);
            }
            if (matchedByState.Any())
                return matchedByState.FirstOrDefault();

            //no state matches. return any
            return matchedByCountry.FirstOrDefault();
        }

        public IList<GetShippingOptionRequest> CreateShippingOptionRequests(IList<ShoppingCartItem> cart, Address shippingAddress, int storeId, out bool shippingFromMultipleLocations)
        {
            //if we always ship from the default shipping origin, then there's only one request
            //if we ship from warehouses ("ShippingSettings.UseWarehouseLocation" enabled),
            //then there could be several requests


            //key - warehouse identifier (0 - default shipping origin)
            //value - request
            var requests = new Dictionary<int, GetShippingOptionRequest>();

            //a list of requests with products which should be shipped separately
            var separateRequests = new List<GetShippingOptionRequest>();

            foreach (var sci in cart)
            {
                if (!sci.IsShipEnabled)
                    continue;

                var product = sci.Product;

                //warehouses
                Warehouse warehouse = null;
                if (_shippingSettings.UseWarehouseLocation)
                {
                    if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock && product.UseMultipleWarehouses)
                    {
                        var allWarehouses = new List<Warehouse>();
                        //multiple warehouses supported
                        foreach (var pwi in product.ProductWarehouseInventory)
                        {
                            //TODO validate stock quantity when backorder is not allowed?
                            var tmpWarehouse = GetWarehouseById(pwi.WarehouseId);
                            if (tmpWarehouse != null)
                                allWarehouses.Add(tmpWarehouse);
                        }
                        warehouse = GetNearestWarehouse(shippingAddress, allWarehouses);
                    }
                    else
                    {
                        //multiple warehouses are not supported
                        warehouse = GetWarehouseById(product.WarehouseId);
                    }
                }
                int warehouseId = warehouse != null ? warehouse.Id : 0;

                if (requests.ContainsKey(warehouseId) && !product.ShipSeparately)
                {
                    //add item to existing request
                    requests[warehouseId].Items.Add(new GetShippingOptionRequest.PackageItem(sci));
                }
                else
                {
                    //create a new request
                    var request = new GetShippingOptionRequest();
                    //store
                    request.StoreId = storeId;
                    //add item
                    request.Items.Add(new GetShippingOptionRequest.PackageItem(sci));
                    //customer
                    request.Customer = cart.GetCustomer();
                    //ship to
                    request.ShippingAddress = shippingAddress;
                    //ship from
                    Address originAddress = null;
                    if (warehouse != null)
                    {
                        //warehouse address
                        originAddress = _addressService.GetAddressById(warehouse.AddressId);
                        request.WarehouseFrom = warehouse;
                    }
                    if (originAddress == null)
                    {
                        //no warehouse address. in this case use the default shipping origin
                        originAddress = _addressService.GetAddressById(_shippingSettings.ShippingOriginAddressId);
                    }
                    if (originAddress != null)
                    {
                        request.CountryFrom = originAddress.Country;
                        request.StateProvinceFrom = originAddress.StateProvince;
                        request.ZipPostalCodeFrom = originAddress.ZipPostalCode;
                        request.CityFrom = originAddress.City;
                        request.AddressFrom = originAddress.Address1;
                    }

                    if (product.ShipSeparately)
                    {
                        //ship separately
                        separateRequests.Add(request);
                    }
                    else
                    {
                        //usual request
                        requests.Add(warehouseId, request);
                    }
                }
            }

            //multiple locations?
            //currently we just compare warehouses
            //but we should also consider cases when several warehouses are located in the same address
            shippingFromMultipleLocations = requests.Select(x => x.Key).Distinct().Count() > 1;


            var result = requests.Values.ToList();
            result.AddRange(separateRequests);

            return result;
        }

        public GetShippingOptionResponse GetShippingOptions(IList<ShoppingCartItem> cart, Address shippingAddress, string allowedShippingRateComputationMethodSystemName = "", int storeId = 0)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var result = new GetShippingOptionResponse();

            //create a package
            bool shippingFromMultipleLocations;
            var shippingOptionRequests = CreateShippingOptionRequests(cart, shippingAddress, storeId, out shippingFromMultipleLocations);
            result.ShippingFromMultipleLocations = shippingFromMultipleLocations;

            var shippingRateComputationMethods = LoadActiveShippingRateComputationMethods(storeId);
            //filter by system name
            if (!String.IsNullOrWhiteSpace(allowedShippingRateComputationMethodSystemName))
            {
                shippingRateComputationMethods = shippingRateComputationMethods
                    .Where(srcm => allowedShippingRateComputationMethodSystemName.Equals(srcm.PluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            if (shippingRateComputationMethods.Count == 0)
                throw new NopException("Shipping rate computation method could not be loaded");



            //request shipping options from each shipping rate computation methods
            foreach (var srcm in shippingRateComputationMethods)
            {
                //request shipping options (separately for each package-request)
                IList<ShippingOption> srcmShippingOptions = null;
                foreach (var shippingOptionRequest in shippingOptionRequests)
                {
                    var getShippingOptionResponse = srcm.GetShippingOptions(shippingOptionRequest);

                    if (getShippingOptionResponse.Success)
                    {
                        //success
                        if (srcmShippingOptions == null)
                        {
                            //first shipping option request
                            srcmShippingOptions = getShippingOptionResponse.ShippingOptions;
                        }
                        else
                        {
                            //get shipping options which already exist for prior requested packages for this scrm (i.e. common options)
                            srcmShippingOptions = srcmShippingOptions
                                .Where(existingso => getShippingOptionResponse.ShippingOptions.Any(newso => newso.Name == existingso.Name))
                                .ToList();

                            //and sum the rates
                            foreach (var existingso in srcmShippingOptions)
                            {
                                existingso.Rate += getShippingOptionResponse
                                    .ShippingOptions
                                    .First(newso => newso.Name == existingso.Name)
                                    .Rate;
                            }
                        }
                    }
                    else
                    {
                        //errors
                        foreach (string error in getShippingOptionResponse.Errors)
                        {
                            result.AddError(error);
                            _logger.Warning(string.Format("Shipping ({0}). {1}", srcm.PluginDescriptor.FriendlyName, error));
                        }
                        //clear the shipping options in this case
                        srcmShippingOptions = new List<ShippingOption>();
                        break;
                    }
                }

                //add this scrm's options to the result
                if (srcmShippingOptions != null)
                {
                    foreach (var so in srcmShippingOptions)
                    {
                        //set system name if not set yet
                        if (String.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName))
                            so.ShippingRateComputationMethodSystemName = srcm.PluginDescriptor.SystemName;
                        if (_shoppingCartSettings.RoundPricesDuringCalculation)
                            so.Rate = RoundingHelper.RoundPrice(so.Rate);
                        result.ShippingOptions.Add(so);
                    }
                }
            }

            if (_shippingSettings.ReturnValidOptionsIfThereAreAny)
            {
                //return valid options if there are any (no matter of the errors returned by other shipping rate compuation methods).
                if (result.ShippingOptions.Count > 0 && result.Errors.Count > 0)
                    result.Errors.Clear();
            }

            //no shipping options loaded
            if (result.ShippingOptions.Count == 0 && result.Errors.Count == 0)
                result.Errors.Add(_localizationService.GetResource("Checkout.ShippingOptionCouldNotBeLoaded"));

            return result;
        }

        /// <summary>
        /// Gets available pickup points
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="providerSystemName">Filter by provider identifier; null to load pickup points of all providers</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Pickup points</returns>
        public virtual GetPickupPointsResponse GetPickupPoints(Address address, Customer customer = null, string providerSystemName = null, int storeId = 0)
        {
            var result = new GetPickupPointsResponse();
            var pickupPointsProviders = LoadActivePickupPointProviders(customer, storeId);
            if (!string.IsNullOrEmpty(providerSystemName))
                pickupPointsProviders = pickupPointsProviders
                    .Where(x => x.PluginDescriptor.SystemName.Equals(providerSystemName, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (pickupPointsProviders.Count == 0)
                return result;

            var allPickupPoints = new List<PickupPoint>();
            foreach (var provider in pickupPointsProviders)
            {
                var pickPointsResponse = provider.GetPickupPoints(address);
                if (pickPointsResponse.Success)
                    allPickupPoints.AddRange(pickPointsResponse.PickupPoints);
                else
                {
                    foreach (string error in pickPointsResponse.Errors)
                    {
                        result.AddError(error);
                        _logger.Warning(string.Format("PickupPoints ({0}). {1}", provider.PluginDescriptor.FriendlyName, error));
                    }
                }
            }

            //any pickup points is enough
            if (allPickupPoints.Count > 0)
            {
                result.Errors.Clear();
                result.PickupPoints = allPickupPoints;
            }

            return result;
        }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Shipping options</returns>
        public virtual GetShippingOptionResponse GetShippingOptions(IList<ShoppingCartItem> cart,
            Address shippingAddress, Customer customer = null, string allowedShippingRateComputationMethodSystemName = "",
            int storeId = 0)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var result = new GetShippingOptionResponse();

            //create a package
            bool shippingFromMultipleLocations;
            var shippingOptionRequests = CreateShippingOptionRequests(cart, shippingAddress, storeId, out shippingFromMultipleLocations);
            result.ShippingFromMultipleLocations = shippingFromMultipleLocations;

            var shippingRateComputationMethods = LoadActiveShippingRateComputationMethods(customer, storeId);
            //filter by system name
            if (!String.IsNullOrWhiteSpace(allowedShippingRateComputationMethodSystemName))
            {
                shippingRateComputationMethods = shippingRateComputationMethods
                    .Where(srcm => allowedShippingRateComputationMethodSystemName.Equals(srcm.PluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            if (!shippingRateComputationMethods.Any())
                //throw new NopException("Shipping rate computation method could not be loaded");
                return result;



            //request shipping options from each shipping rate computation methods
            foreach (var srcm in shippingRateComputationMethods)
            {
                //request shipping options (separately for each package-request)
                IList<ShippingOption> srcmShippingOptions = null;
                foreach (var shippingOptionRequest in shippingOptionRequests)
                {
                    var getShippingOptionResponse = srcm.GetShippingOptions(shippingOptionRequest);

                    if (getShippingOptionResponse.Success)
                    {
                        //success
                        if (srcmShippingOptions == null)
                        {
                            //first shipping option request
                            srcmShippingOptions = getShippingOptionResponse.ShippingOptions;
                        }
                        else
                        {
                            //get shipping options which already exist for prior requested packages for this scrm (i.e. common options)
                            srcmShippingOptions = srcmShippingOptions
                                .Where(existingso => getShippingOptionResponse.ShippingOptions.Any(newso => newso.Name == existingso.Name))
                                .ToList();

                            //and sum the rates
                            foreach (var existingso in srcmShippingOptions)
                            {
                                existingso.Rate += getShippingOptionResponse
                                    .ShippingOptions
                                    .First(newso => newso.Name == existingso.Name)
                                    .Rate;
                            }
                        }
                    }
                    else
                    {
                        //errors
                        foreach (string error in getShippingOptionResponse.Errors)
                        {
                            result.AddError(error);
                            _logger.Warning(string.Format("Shipping ({0}). {1}", srcm.PluginDescriptor.FriendlyName, error));
                        }
                        //clear the shipping options in this case
                        srcmShippingOptions = new List<ShippingOption>();
                        break;
                    }
                }

                //add this scrm's options to the result
                if (srcmShippingOptions != null)
                {
                    foreach (var so in srcmShippingOptions)
                    {
                        //set system name if not set yet
                        if (String.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName))
                            so.ShippingRateComputationMethodSystemName = srcm.PluginDescriptor.SystemName;
                        if (_shoppingCartSettings.RoundPricesDuringCalculation)
                            so.Rate = RoundingHelper.RoundPrice(so.Rate);
                        result.ShippingOptions.Add(so);
                    }
                }
            }

            if (_shippingSettings.ReturnValidOptionsIfThereAreAny)
            {
                //return valid options if there are any (no matter of the errors returned by other shipping rate compuation methods).
                if (result.ShippingOptions.Any() && result.Errors.Any())
                    result.Errors.Clear();
            }

            //no shipping options loaded
            if (!result.ShippingOptions.Any() && !result.Errors.Any())
                result.Errors.Add(_localizationService.GetResource("Checkout.ShippingOptionCouldNotBeLoaded"));

            return result;
        }
        #endregion
    }
}
