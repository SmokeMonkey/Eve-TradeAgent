using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVE.ISXEVE;
using LavishScriptAPI;
using LavishVMAPI;
using InnerSpaceAPI;
using System.Diagnostics;

namespace Eve_TradingAgent
{
    public class MarketOrderSet
    {
        private Config _config;

        public MarketOrderSet(Config config)
        {
            _config = config;
            Orders = new List<MyMarketOrder>();
        }

        public void LoadMarketOrderInfo()
        {
            Log.WriteLog("Trying to load market orders.");

            List<MyOrder> retrivedOrders = null;
            Me me;
            bool success;
            using (new FrameLock(true))
            {
                EVE.ISXEVE.EVE eve = new EVE.ISXEVE.EVE();

                // me is not persistent, we can't just keep a static reference somewhere, have to renew every frame;
                me = new Me();

                success = me.UpdateMyOrders();
            }

            if (success)
            {
                Log.WriteLog("Fired update order request, wait to get result.");

                int counter = 0;

                while (retrivedOrders == null && counter < 100)
                {
                    Frame.Wait(false);
                    using (new FrameLock(true))
                    {
                        counter++;
                        // me is not persistent, we can't just keep a static reference somewhere,have to renew every frame;
                        me = new Me();

                        // IMPORTANT: This is shallow copy, we need to finish all DEEP copy value operations while locked, otherwise the memory will be flushed;
                        retrivedOrders = me.GetMyOrders();

                        if (retrivedOrders != null)
                        {
                            if (ValidateOrderData(retrivedOrders))
                            {
                                _combineNewOrder(retrivedOrders);
                            }
                            else
                            {
                                Log.WriteLog("Found invalid data in retrived order, retrying.");
                                // Enter next loop;
                                retrivedOrders = null;
                            }
                        }
                    }
                }

                if (retrivedOrders != null)
                {
                    Log.WriteLog("Got " + retrivedOrders.Count.ToString() + " orders with (" + counter.ToString() + ") attempts.");
                }
                else
                {
                    Log.WriteLog("Failed to load orders with (" + counter.ToString() + ") attempts.");
                }
            }
            else
            {
                Log.WriteLog("Failed to send update order request.");
            }

            Log.WriteLog("Done loading market orders.");
        }

        /// <summary>
        /// Pick orders within reach,not marked as "Skip", and finished evaluate&modify cool down.
        /// </summary>
        public List<MyMarketOrder> AvailableOrders
        {
            get
            {
                if (Orders.Any())
                {
                    List<MyMarketOrder> result = Orders.Where(o => (o.Status != OrderStatus.OutOfReach && !o.Skip)).ToList();

                    if (result.Any())
                    {
                        result = result.Where(o => (o.IsEvaluateCoolDownFinished && o.IsModifyCoolDownFinished)).ToList();

                        if (result.Any())
                        {
                            return result;
                        }
                        else
                        {
                            Log.WriteLog("Every order is modified or evaluated recently.");
                        }
                    }
                    else
                    {
                        Log.WriteLog("All orders are out of reach or marked as Skip.");
                    }
                }
                else
                {
                    Log.WriteLog("Seems no order to proceed at all.");
                }

                return null;
            }
        }

        public List<MyMarketOrder> Orders;

        /// <summary>
        /// Basic validation of the data retrived.
        /// </summary>
        /// <param name="retrivedOrders"></param>
        /// <returns></returns>
        public static bool ValidateOrderData(List<MyOrder> retrivedOrders)
        {
            // TODO: validate characters, datarange, quantity of each item
            return true;
        }

        /// <summary>
        /// Basic validation of the data retrived.
        /// </summary>
        /// <param name="retrivedOrders"></param>
        /// <returns></returns>
        public static bool ValidateOrderData(List<EVE.ISXEVE.MarketOrder> retrivedOrders)
        {
            // TODO: validate characters, datarange, quantity of each item
            return true;
        }

        /// <summary>
        /// Refresh in game data while retain other fields for duplicated orders,
        /// remove disappeared orders, and add new orders to cache.
        /// </summary>
        /// <param name="newOrders"></param>
        private void _combineNewOrder(List<MyOrder> newOrders)
        {
            List<long> OldOrderIDToDelete = new List<long>();
            foreach (MyMarketOrder o in Orders)
            {
                OldOrderIDToDelete.Add(o.ID);
            }

            List<long> NewOrderIDToAdd = new List<long>();
            foreach (MyOrder o in newOrders)
            {
                NewOrderIDToAdd.Add(o.ID);
            }

            List<MyMarketOrder> NewOrderToAppend = new List<MyMarketOrder>();

            Debug.Assert(Orders != null);
            // TODO: validate newOrder;

            foreach (MyMarketOrder o in Orders)
            {
                foreach (MyOrder n in newOrders)
                {
                    if (o.ID == n.ID)
                    {
                        DateTime NewLastModifiedTime = new System.DateTime(1601, 1, 1).AddSeconds(n.TimeStampWhenIssued / 10000000);
                        o.ModifyCoolDownEndTime = o.ModifyCoolDownEndTime > NewLastModifiedTime.AddMilliseconds(_config.RandomizedOrderModifyIntervalInMilliSec) ? o.ModifyCoolDownEndTime : NewLastModifiedTime.AddMilliseconds(_config.RandomizedOrderModifyIntervalInMilliSec);
                        // Merge items when matched.
                        o.QuantityRemaining = n.QuantityRemaining;
                        o.Price = n.Price;
                        o.DateWhenLastModified = n.DateWhenIssued;
                        o.TimeWhenLastModified = n.TimeWhenIssued;
                        o.TimeStampWhenLastModified = n.TimeStampWhenIssued;

                        // Mark the new item as not add. (already exist)
                        NewOrderIDToAdd.Remove(n.ID);
                        // Mark the old item as not delete. (still alive)
                        OldOrderIDToDelete.Remove(o.ID);
                        break;
                    }
                }
            }

            // Remove old market orders that have disappeared.
            foreach (long id in OldOrderIDToDelete)
            {
                Debug.Assert(Orders.Where(o => o.ID == id).ToList().Count == 1);

                Orders.RemoveAll(o => o.ID == id);

                string ItemName = Orders.Where(o => o.ID == id).First().Type;
                Log.WriteLog("Order fulfilled or cancelled, ID : " + id.ToString() + ", Item: " + ItemName);

            }

            // Add new orders to cached list.
            foreach (long id in NewOrderIDToAdd)
            {
                Debug.Assert(newOrders.Where(o => o.ID == id).ToList().Count == 1);

                Orders.Add(new MyMarketOrder(newOrders.Where(o => o.ID == id).First(), Orders, _config));

                string ItemName = newOrders.Where(o => o.ID == id).First().Name;
                Log.WriteLog("New order detected, ID : " + id.ToString() + ", Item: " + ItemName);
            }
        }
    }


    /// <summary>
    /// Only contain essential message to speed up the process.
    /// </summary>
    public class MarketOrderBase
    {
        // For retrieving other's orders
        public MarketOrderBase(EVE.ISXEVE.MarketOrder isxEveOrderObject)
        {
            ID = isxEveOrderObject.ID;
            TypeID = isxEveOrderObject.TypeID;
            OrderType = isxEveOrderObject.IsBuyOrder ? OrderType.Buy : OrderType.Sell;
            QuantityRemaining = isxEveOrderObject.QuantityRemaining;
            InitialQuantity = isxEveOrderObject.InitialQuantity;
            Price = isxEveOrderObject.Price;
            StationID = isxEveOrderObject.StationID;
            SolarSystemID = isxEveOrderObject.SolarSystemID;
            RegionID = isxEveOrderObject.RegionID;
            MinQuantityToBuy = isxEveOrderObject.MinQuantityToBuy;
            Range = isxEveOrderObject.Range;
        }

        // For retrieving my orders
        public MarketOrderBase(MyOrder isxEveOrderObject)
        {
            ID = isxEveOrderObject.ID;
            TypeID = isxEveOrderObject.TypeID;
            OrderType = isxEveOrderObject.IsBuyOrder ? OrderType.Buy : OrderType.Sell;
            QuantityRemaining = isxEveOrderObject.QuantityRemaining;
            InitialQuantity = isxEveOrderObject.InitialQuantity;
            Price = isxEveOrderObject.Price;
            StationID = isxEveOrderObject.StationID;
            SolarSystemID = isxEveOrderObject.SolarSystemID;
            RegionID = isxEveOrderObject.RegionID;
            MinQuantityToBuy = isxEveOrderObject.MinQuantityToBuy;
            Range = isxEveOrderObject.Range;
        }

        #region retrived from MyOrder Object
        public long ID;
        public int TypeID;
        public OrderType OrderType;
        public double QuantityRemaining;
        public int InitialQuantity;
        public double Price;
        public int StationID;
        public int SolarSystemID;
        public int RegionID;
        public int MinQuantityToBuy;
        public int Range;
        #endregion
    }

    /// <summary>
    /// Data container for order informations.
    /// This middle layer is not directly used, just to make class clearer in defination.
    /// </summary>
    public class MarketOrder : MarketOrderBase
    {
        // For retrieving other's orders
        public MarketOrder(EVE.ISXEVE.MarketOrder isxEveOrderObject)
            : base(isxEveOrderObject)
        {
            Type = isxEveOrderObject.Name;
            Station = isxEveOrderObject.Station;
            SolarSystem = isxEveOrderObject.SolarSystem;
            Region = isxEveOrderObject.Region;
            Duration = isxEveOrderObject.Duration;
            DateWhenLastModified = isxEveOrderObject.DateWhenIssued;
            TimeWhenLastModified = isxEveOrderObject.TimeWhenIssued;
            TimeStampWhenLastModified = (long)isxEveOrderObject.TimeStampWhenIssued;
            IsContraband = isxEveOrderObject.IsContraband;
        }

        // For retrieving my orders
        public MarketOrder(MyOrder isxEveOrderObject)
            : base(isxEveOrderObject)
        {
            Type = isxEveOrderObject.Name;
            Station = isxEveOrderObject.Station;
            SolarSystem = isxEveOrderObject.SolarSystem;
            Region = isxEveOrderObject.Region;
            Duration = isxEveOrderObject.Duration;
            DateWhenLastModified = isxEveOrderObject.DateWhenIssued;
            TimeWhenLastModified = isxEveOrderObject.TimeWhenIssued;
            TimeStampWhenLastModified = isxEveOrderObject.TimeStampWhenIssued;
            IsContraband = isxEveOrderObject.IsContraband;
        }

        #region retrived from MyOrder Object
        public string Type;
        public string Station;
        public string SolarSystem;
        public string Region;
        public int Duration;
        public string DateWhenLastModified;
        public string TimeWhenLastModified;
        // Seconds * 10000000 since Jan/1/1601
        public long TimeStampWhenLastModified;
        public bool IsContraband;
        #endregion
    }

    public class MyMarketOrder : MarketOrder
    {
        public MyMarketOrder(MyOrder isxEveOrderObject, List<MyMarketOrder> orderList, Config config)
            : base(isxEveOrderObject)
        {
            _orderList = orderList;
            _config = config;
            EvaluateCoolDownEndTime = System.DateTime.UtcNow;
            DateTime lastModifiedTime = new System.DateTime(1601, 1, 1).AddSeconds(isxEveOrderObject.TimeStampWhenIssued / 10000000);
            ModifyCoolDownEndTime = lastModifiedTime.AddMilliseconds(_config.RandomizedOrderModifyIntervalInMilliSec);
            Status = OrderStatus.Unknown;

            ID = isxEveOrderObject.ID;
            _skip = false;
            foreach (long id in _config.OrderIDsToSkip)
            {
                // When we load orders for first time, we need to retrive the saved "Skip" mark from config file.
                if (ID == id)
                {
                    _skip = true;
                }
            }
        }

        public void Cancel()
        {
            Log.WriteLog("Trying to confirm order " + this.ID.ToString() + ": \"" + this.Type.ToString() + "\" before cancelling.");

            List<MyOrder> retrivedOrders = null;
            Me me;
            bool success;
            using (new FrameLock(true))
            {
                EVE.ISXEVE.EVE eve = new EVE.ISXEVE.EVE();

                // me is not persistent, we can't just keep a static reference somewhere, have to renew every frame;
                me = new Me();
                success = me.UpdateMyOrders();
            }

            if (success)
            {
                Log.WriteLog("Fired update order request, wait to get result.");

                int counter = 0;

                while (retrivedOrders == null && counter < 100)
                {
                    Frame.Wait(false);
                    using (new FrameLock(true))
                    {
                        counter++;
                        // me is not persistent, we can't just keep a static reference somewhere,have to renew every frame;
                        me = new Me();

                        EVE.ISXEVE.Character.OrderType orderType = this.OrderType == OrderType.Buy ? EVE.ISXEVE.Character.OrderType.Buy : EVE.ISXEVE.Character.OrderType.Sell;

                        // IMPORTANT: This is shallow copy, we need to finish all DEEP copy value operations while locked, otherwise the memory will be flushed;
                        retrivedOrders = me.GetMyOrders(orderType, this.TypeID);


                        if (retrivedOrders != null)
                        {
                            if (MarketOrderSet.ValidateOrderData(retrivedOrders))
                            {
                                MyOrder orderToCancel = null;

                                foreach (MyOrder o in retrivedOrders)
                                {
                                    if (o.ID == ID)
                                    {
                                        orderToCancel = o;
                                        break;
                                    }
                                }

                                if (orderToCancel != null)
                                {
                                    Log.WriteLog("Confirmed order with (" + counter.ToString() + ") attempts.");

                                    DateTime lastModifiedTime = new System.DateTime(1601, 1, 1).AddSeconds(orderToCancel.TimeStampWhenIssued / 10000000);

                                    //real cool down takes 5 minutes
                                    if (lastModifiedTime.AddMilliseconds(301000) < System.DateTime.UtcNow)
                                    {
                                        orderToCancel.Cancel();
                                        _orderList.Remove(this);
                                        Log.WriteLog("Order " + this.ID.ToString() + ": \"" + this.Type.ToString() + "\" cancelled");
                                    }
                                    else
                                    {
                                        ModifyCoolDownEndTime = lastModifiedTime.AddMilliseconds(_config.RandomizedOrderModifyIntervalInMilliSec);
                                        Log.WriteLog("Order cooldown not finished yet. Job \"Cancel\" cancelled.");
                                    }
                                }
                                else
                                {
                                    Log.WriteLog("Failed to find order to cancel with (" + counter.ToString() + ") attempts. Job \"Cancel\" cancelled.");
                                }
                            }
                            else
                            {
                                Log.WriteLog("Found invalid data in retrived order, retrying.");
                                // Enter next loop;
                                retrivedOrders = null;
                            }
                        }
                    }
                }
            }
            else
            {
                Log.WriteLog("Failed to send update order request. Job \"Cancel\" cancelled.");
            }
        }

        public void Modify(double newPrice)
        {
            Debug.Assert(newPrice > 0);

            Log.WriteLog("Trying to confirm order " + this.ID.ToString() + ": \"" + this.Type.ToString() + "\" before modifying.");

            List<MyOrder> retrivedOrders = null;
            Me me;
            bool success;
            using (new FrameLock(true))
            {
                EVE.ISXEVE.EVE eve = new EVE.ISXEVE.EVE();

                // me is not persistent, we can't just keep a static reference somewhere, have to renew every frame;
                me = new Me();
                success = me.UpdateMyOrders();
            }

            if (success)
            {
                Log.WriteLog("Fired update order request, wait to get result.");

                int counter = 0;

                while (retrivedOrders == null && counter < 100)
                {
                    Frame.Wait(false);
                    using (new FrameLock(true))
                    {
                        counter++;
                        // me is not persistent, we can't just keep a static reference somewhere,have to renew every frame;
                        me = new Me();

                        EVE.ISXEVE.Character.OrderType orderType = this.OrderType == OrderType.Buy ? EVE.ISXEVE.Character.OrderType.Buy : EVE.ISXEVE.Character.OrderType.Sell;

                        // IMPORTANT: This is shallow copy, we need to finish all DEEP copy value operations while locked, otherwise the memory will be flushed;
                        retrivedOrders = me.GetMyOrders(orderType, this.TypeID);

                        if (retrivedOrders != null)
                        {
                            if (MarketOrderSet.ValidateOrderData(retrivedOrders))
                            {
                                MyOrder orderToModify = null;

                                foreach (MyOrder o in retrivedOrders)
                                {
                                    if (o.ID == ID)
                                    {
                                        orderToModify = o;
                                        break;
                                    }
                                }

                                if (orderToModify != null)
                                {
                                    Log.WriteLog("Confirmed order with (" + counter.ToString() + ") attempts.");

                                    DateTime lastModifiedTime = new System.DateTime(1601, 1, 1).AddSeconds(orderToModify.TimeStampWhenIssued / 10000000);

                                    //real cool down takes 5 minutes
                                    if (lastModifiedTime.AddMilliseconds(301000) < System.DateTime.UtcNow)
                                    {
                                        orderToModify.Modify(newPrice);

                                        Price = newPrice;
                                        ModifyCoolDownEndTime = System.DateTime.UtcNow.AddMilliseconds(_config.RandomizedOrderModifyIntervalInMilliSec);
                                        //I think the date, time and timestamp dont need updating.

                                        Log.WriteLog("Order " + this.ID.ToString() + ": \"" + this.Type.ToString() + "\" modified");
                                    }
                                    else
                                    {
                                        ModifyCoolDownEndTime = lastModifiedTime.AddMilliseconds(_config.RandomizedOrderModifyIntervalInMilliSec);
                                        Log.WriteLog("Order cooldown not finished yet. Job \"Modify\" cancelled.");
                                    }
                                }
                                else
                                {
                                    Log.WriteLog("Failed to find order to modify with (" + counter.ToString() + ") attempts. Job \"Modify\" cancelled.");
                                }
                            }
                            else
                            {
                                Log.WriteLog("Found invalid data in retrived order, retrying.");
                                // Enter next loop;
                                retrivedOrders = null;
                            }
                        }
                    }
                }
            }
            else
            {
                Log.WriteLog("Failed to send update order request. Job \"Modify\" cancelled.");
            }
        }

        public bool IsEvaluateCoolDownFinished
        {
            get
            {
                return EvaluateCoolDownEndTime < System.DateTime.UtcNow;
            }
        }

        public bool IsModifyCoolDownFinished
        {
            get
            {
                return ModifyCoolDownEndTime < System.DateTime.UtcNow;
            }
        }

        public MarketSituation GetMarketSituation()
        {
            Log.WriteLog("Trying to get the market situation of  \"" + this.Type.ToString() + "\".");

            List<EVE.ISXEVE.MarketOrder> retrivedOrders = null;
            MarketSituation result = null;

            bool success;
            using (new FrameLock(true))
            {
                EVE.ISXEVE.EVE eve = new EVE.ISXEVE.EVE();

                success = eve.FetchMarketOrders(this.TypeID);
            }

            if (success)
            {
                Log.WriteLog("Fired fetching orders request, wait to get result.");

                int counter = 0;

                while (retrivedOrders == null && counter < 100)
                {
                    Frame.Wait(false);
                    using (new FrameLock(true))
                    {
                        counter++;

                        EVE.ISXEVE.EVE eve = new EVE.ISXEVE.EVE();

                        // IMPORTANT: This is shallow copy, we need to finish all DEEP copy value operations while locked, otherwise the memory will be flushed;
                        retrivedOrders = eve.GetMarketOrders(this.TypeID);

                        if (retrivedOrders != null)
                        {
                            if (MarketOrderSet.ValidateOrderData(retrivedOrders))
                            {
                                // Prepare my own order id list to speed up the combine.
                                List<long> MyOwnOrderIds = new List<long>();
                                foreach (MyMarketOrder mmo in _orderList)
                                {
                                    if (mmo.TypeID == TypeID)
                                    {
                                        MyOwnOrderIds.Add(mmo.ID);
                                    }
                                }

                                result = new MarketSituation(retrivedOrders, MyOwnOrderIds);
                            }
                            else
                            {
                                Log.WriteLog("Found invalid data in retrived order, retrying.");
                                // Enter next loop;
                                retrivedOrders = null;
                            }
                        }
                    }
                }
                if (retrivedOrders != null)
                {
                    Log.WriteLog("Got " + retrivedOrders.Count.ToString() + " orders with (" + counter.ToString() + ") attempts.");
                }
                else
                {
                    Log.WriteLog("Failed to load orders with (" + counter.ToString() + ") attempts.");
                }
            }
            else
            {
                Log.WriteLog("Failed to send update order request.");
            }

            Log.WriteLog("Done loading market orders.");

            return result;
        }

        #region public data fields

        /// <summary>
        /// Can reevaluate the order market status after this time(UTC time).
        /// </summary>
        public DateTime EvaluateCoolDownEndTime;

        /// <summary>
        /// Can modify the order after this time.(UTC time)
        /// </summary>
        public DateTime ModifyCoolDownEndTime;

        public OrderStatus Status;

        // Marked as 'skip' by user.
        public bool Skip
        {
            get
            {
                try
                {
                    return _skip;
                }
                catch (Exception) { return false; }
            }
            set
            {
                // It seems safe to modify this value even if the OrderList is being updated(occupied==true), because we are not destroying old objects to update it.
                // But what if the object is destroyed because the order is cancelled or fulfilled?

                // Try to protect with 'try', don't know if it works. :P
                try
                {
                    _skip = value;

                    if (_skip)
                    {
                        _config.OrderIDsToSkip.Add(this.ID);
                    }
                    else
                    {
                        _config.OrderIDsToSkip.Remove(this.ID);
                    }
                }
                catch (Exception) { }
            }
        }

        #endregion

        private bool _skip;
        private Config _config;
        private List<MyMarketOrder> _orderList;
    }

    public class MarketSituation
    {
        public MarketSituation(List<EVE.ISXEVE.MarketOrder> retrivedOrders, List<long> myOwnOrderIDs)
        {
            BuyOrdersInMarket = new List<MarketOrderBase>();
            SellOrdersInMarket = new List<MarketOrderBase>();

            bool IsMyOwnOrder = false;
            foreach (EVE.ISXEVE.MarketOrder mo in retrivedOrders)
            {
                IsMyOwnOrder = false;
                foreach (long i in myOwnOrderIDs)
                {
                    if (i == mo.ID)
                    {
                        IsMyOwnOrder = true;
                        break;
                    }
                }
                if (!IsMyOwnOrder)
                {
                    if (mo.IsBuyOrder)
                    {
                        BuyOrdersInMarket.Add(new MarketOrderBase(mo));
                    }
                    else
                    {
                        SellOrdersInMarket.Add(new MarketOrderBase(mo));
                    }
                }
            }
        }

        // expell my own orders(do not compete with myself)
        // get PriceGaps

        //public MarketSituation(MarketOrderSet myOrders)
        //{
        // }

        public List<MarketOrderBase> BuyOrdersInMarket;
        public List<MarketOrderBase> SellOrdersInMarket;
    }

}
