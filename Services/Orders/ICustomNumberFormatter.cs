using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Orders;

namespace Services.Orders
{
    public partial interface ICustomNumberFormatter
    {
        string GenerateReturnRequestCustomNumber(ReturnRequest returnRequest);

        string GenerateOrderCustomNumber(Order order);
    }
}
