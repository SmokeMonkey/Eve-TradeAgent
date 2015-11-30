using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve_TradingAgent
{
    public enum OrderStatus
    {
        Unknown,
        OutOfReach, // Character cannot modify the order.
        NeedNoUpdate, // Price is optimized(not necessary winning).
        Hesitate, // The order is not winning but we cannot decide a best price for it. Or user marked it as not touch.
        AutoModified, // 
        ManuallySpecified, // User specified a price, the state lasts until next modifiable preiod.
    }

    public enum OrderType
    {
        Sell,
        Buy,
    }
}
