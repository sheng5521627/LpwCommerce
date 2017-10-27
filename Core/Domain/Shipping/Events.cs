using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Shipping
{
    /// <summary>
    /// Shipment sent event(货物发送事件)
    /// </summary>
    public class ShipmentSentEvent
    {
        public ShipmentSentEvent(Shipment shipment)
        {
            this.Shipment = shipment;
        }

        /// <summary>
        /// Shipment
        /// </summary>
        public Shipment Shipment { get; private set; }
    }

    /// <summary>
    /// Shipment delivered(交付) event
    /// </summary>
    public class ShipmentDeliveredEvent
    {
        public ShipmentDeliveredEvent(Shipment shipment)
        {
            this.Shipment = shipment;
        }

        /// <summary>
        /// Shipment
        /// </summary>
        public Shipment Shipment { get; private set; }
    }
}
