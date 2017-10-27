using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Shipping.Tracking
{
    /// <summary>
    /// General shipment tracker (finds an appropriate tracker by tracking number)
    /// </summary>
    public partial class GeneralShipmentTracker : IShipmentTracker
    {
        #region Fields

        private readonly ITypeFinder _typeFinder;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="typeFinder">Type finder</param>
        public GeneralShipmentTracker(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        protected virtual IList<IShipmentTracker> GetAllTrackers()
        {
            return _typeFinder.FindClassesOfType<IShipmentTracker>()
                .Where(m => m != typeof(GeneralShipmentTracker))
                .Select(m => EngineContext.Current.ContainerManager.ResolveUnregistered(m) as IShipmentTracker)
                .ToList();
        }

        protected virtual IShipmentTracker GetTrackerByTrackingNumber(string trackingNumber)
        {
            return GetAllTrackers().Where(m => m.IsMatch(trackingNumber)).FirstOrDefault();
        }

        public IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                return new List<ShipmentStatusEvent>();

            var tracker = GetTrackerByTrackingNumber(trackingNumber);
            if (tracker != null)
                return tracker.GetShipmentEvents(trackingNumber);
            return new List<ShipmentStatusEvent>();
        }

        public string GetUrl(string trackingNumber)
        {
            var tracker = GetTrackerByTrackingNumber(trackingNumber);
            if (tracker != null)
                return tracker.GetUrl(trackingNumber);
            return null;
        }

        public bool IsMatch(string trackingNumber)
        {
            var tracker = GetTrackerByTrackingNumber(trackingNumber);
            if (tracker != null)
                return tracker.IsMatch(trackingNumber);
            return false;
        }

        #endregion
    }
}
