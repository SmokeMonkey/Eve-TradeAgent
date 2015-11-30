using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using InnerSpaceAPI;
using System.Diagnostics;

namespace Eve_TradingAgent
{
    /// <summary>
    /// Manager class.
    /// </summary>
    public class TradeAgent
    {
        public TradeAgent(MainWindow UIWindow)
        {
            Config = new Config();
            ActiveCharacter = new Character();
            OrderSet = new MarketOrderSet(Config);
            _decide = new Strategy(ActiveCharacter, Config);

            // Load non-character specific configs.
            _loadGeneralConfig();

            // hook with UI, and delete old logs.
            _UIWindow = UIWindow;
            Log.Initialize(_UIWindow.WriteLogToWindow);
            Log.WriteLog("Done initializing trade agent.");

            // init other parts
            // updateInfo()
        }

        ~TradeAgent()
        {
            Config.SaveGeneralConfig();

            if (!string.IsNullOrEmpty(ActiveCharacter.Name))
            {
                _rebuildSkipIDList();
                Config.SaveCharacterConfig();
            }
        }

        public Character ActiveCharacter;
        public MarketOrderSet OrderSet;
        // Character specific config(configs should not be shared between processes).
        public Config Config;
        //public System.Timers.Timer TimerReloadOrderList;
        /// <summary>
        /// Evaluate and modify one order from order list in this timer's elapsed event.
        /// </summary>
        public System.Timers.Timer TimerProcessOrder;
        /// <summary>
        /// Get or set the flag which is used to avoid collision between different timers and one timer with itself.
        /// </summary>
        public bool IsOrderListOccupied = false;

        /// <summary>
        /// This is used for the case that user try to stop the process while some process is under progress.
        /// </summary>
        public bool ShouldStopTimer = true;

        # region public method

        private void _pickAndProcessOrder()
        {
            // comment this out because we don't want to reload orderlist so frequently.
            // Log.WriteLog("Reloading before proceed.");
            // UpdateInfo();
            Log.WriteLog("Trying to pick and process orders.");

            MyMarketOrder OrderToEvaluate = _pickOneOrderToEvaluate();

            if (OrderToEvaluate != null)
            {
                EvaluateAndUpdateOrder(OrderToEvaluate);
            }
            else
            {
                Log.WriteLog("Didn't find valid order to proceed, task cancelled.");
            }

            Log.WriteLog("Done picking and processing orders.");
        }

        public void UpdateInfo()
        {
            Log.WriteLog("Trying to update info.");
            // get char information
            _updateCharacterInfo();

            if (!string.IsNullOrEmpty(ActiveCharacter.Name))
            {
                if (_firstLoad)
                {
                    Log.WriteLog("Loading for first time.");
                    Config.LoadCharacterConfig(ActiveCharacter.Name);

                    //TimerReloadOrderList = new System.Timers.Timer(Config.RandomizedTurnsBetweenReloadOrderList);
                    //TimerReloadOrderList.Elapsed += new System.Timers.ElapsedEventHandler(ReloadIntervalElapsedHandler);

                    _turnsBeforeReloadInterval = Config.RandomizedTurnsBetweenReloadOrderList;

                    TimerProcessOrder = new System.Timers.Timer(Config.RandomizedOrderProcessIntervalInMilliSec);
                    TimerProcessOrder.Elapsed += new System.Timers.ElapsedEventHandler(ProcessIntervalElapsedHandler);

                    _UIWindow.RestoreCharacterConfig();
                    _UIWindow.EnableControls();

                    _firstLoad = false;
                }

                OrderSet.LoadMarketOrderInfo();
                _checkOrderDistance(); // according to character and order info.
            }
            else
            {
                Log.WriteLog("Failed loading character, disabling controls.");
                _UIWindow.DisableControls();
            }

            Log.WriteLog("Done updating info.");
        }

        /// <summary>
        /// for ui use
        /// </summary>
        /// <param name="newPrice"></param>
        public void ModifyOrder(double newPrice) { }

        # endregion

        # region private field

        private MainWindow _UIWindow;
        private Strategy _decide;
        private bool _firstLoad = true;
        private int _turnsBeforeReloadInterval;
        # endregion

        #region private method
        // Using single timer to reduce complicity now.
        //private void ReloadIntervalElapsedHandler(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    Log.WriteLog("Trying to reload order list.");

        //    if (!IsOrderListOccupied)
        //    {
        //        IsOrderListOccupied = true;
        //        //TimerReloadOrderList.Stop();
        //        // Get new randomized interval on every turn.
        //        //   TimerReloadOrderList.Interval = Config.RandomizedTurnsBetweenReloadOrderList;

        //        UpdateInfo();

        //        // Stop timer from restarting if user pressed stop while this is in progress.
        //        if (!ShouldStopTimer)
        //        {
        //            // TimerReloadOrderList.Start();
        //        }
        //        IsOrderListOccupied = false;
        //    }
        //    else
        //    {
        //        Log.WriteLog("Orderlist is busy while trying to reload order list, giving up this turn.");
        //    }

        //    Log.WriteLog("Done reloading order list.");
        //}

        private void ProcessIntervalElapsedHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            _turnsBeforeReloadInterval = _turnsBeforeReloadInterval - 1;
            if (_turnsBeforeReloadInterval == 0)
            {
                _turnsBeforeReloadInterval = Config.RandomizedTurnsBetweenReloadOrderList;
                Log.WriteLog("Trying to reload order list.");

                if (!IsOrderListOccupied)
                {
                    IsOrderListOccupied = true;

                    UpdateInfo();

                    IsOrderListOccupied = false;
                }
                else
                {
                    Log.WriteLog("Orderlist is busy while trying to reload order list, giving up this turn.");
                    _turnsBeforeReloadInterval = 1; // try again next turn.
                }

                Log.WriteLog("Done reloading order list.");
            }

            Log.WriteLog("Trying to process order.");

            if (!IsOrderListOccupied)
            {
                IsOrderListOccupied = true;
                TimerProcessOrder.Stop();
                // Get new randomized interval on every turn.
                TimerProcessOrder.Interval = Config.RandomizedOrderProcessIntervalInMilliSec;

                _pickAndProcessOrder();

                // Stop timer from restarting if user pressed stop while this is in progress.
                if (!ShouldStopTimer)
                {
                    TimerProcessOrder.Start();
                }
                IsOrderListOccupied = false;
            }
            else
            {
                Log.WriteLog("Orderlist is busy while trying to process orders, giving up this turn.");
            }

            Log.WriteLog("Done processing order.");
        }


        public void EvaluateAndUpdateOrder(MyMarketOrder order)
        {
            Debug.Assert(order != null);
            Log.WriteLog("Trying to evaluate and update order " + order.ID.ToString() + ": " + order.Type.ToString());

            //update evaluate cd
            order.EvaluateCoolDownEndTime = System.DateTime.UtcNow.AddMilliseconds(Config.IntervalOfProcessingSameOrder);

            // get good price
            double RecommendedPrice = _decide.GetRecommendedPrice(order);
            // <0 means we cannot find a obvious best price
            // new price >0?
            if (RecommendedPrice > 0)
            {
                // if already good
                if (RecommendedPrice == order.Price)
                {
                    // need no update -> update state 
                    order.Status = OrderStatus.NeedNoUpdate;
                }
                else
                {
                    // TODO check config limit

                    //  Update  -> update state and modify cd /// or hesatate(config restricted)
                    order.Modify(RecommendedPrice);
                    order.Status = OrderStatus.AutoModified;
                }
            }
            else
            {
                order.Status = OrderStatus.Hesitate;
            }

            Log.WriteLog("Done evaluating and updating order " + order.ID.ToString() + ": " + order.Type.ToString());
        }

        /// <summary>
        /// Reload the character information
        /// </summary>
        private void _updateCharacterInfo()
        {
            // TODO what if the character is different from what we loaded last time?
            bool IsCharacterLoaded = ActiveCharacter.LoadCharacterInfo();

            if (IsCharacterLoaded)
            {
                // set log prefix,
                Log.SetCharacter(ActiveCharacter.Name);
                Log.WriteLog("Done loading character: " + ActiveCharacter.Name);
            }
            else
            {
                // TODO: handle this without throw when load button is implemented. also this may because isxeve is not on.
                throw new Exception("Failed to load character info, make sure you are logged in.");
            }
        }

        private void _loadGeneralConfig()
        {

            Config.LoadGeneralConfig();
            // general config includes at least log config
            // throw exception "cannot create/load config" if fails
        }

        /// <summary>
        /// Update order state to mark orders within reach according to char skill and location
        /// </summary>
        private void _checkOrderDistance()
        {

            // TODO this is not finished
            // step one , return only order in same station
            // Final task return order up to whole region.

            foreach (MyMarketOrder o in OrderSet.Orders)
            {
                //assert me in station

                // mark orders out of reach as out_of_reach, clean the 
                if (o.StationID != ActiveCharacter.CurrentStationID)
                {
                    o.Status = OrderStatus.OutOfReach;
                }
                else if (o.Status == OrderStatus.OutOfReach)
                {
                    o.Status = OrderStatus.Unknown;
                }
            }
        }

        private MyMarketOrder _pickOneOrderToEvaluate()
        {
            // pick evaluable orders(within reach and finished evaluate cooldown)
            List<MyMarketOrder> AvailableOrders = OrderSet.AvailableOrders;

            if (AvailableOrders != null)
            {
                MyMarketOrder result = AvailableOrders.OrderBy(o => o.EvaluateCoolDownEndTime).First();
                Log.WriteLog("Picked order " + result.ID.ToString() + ": " + result.Type.ToString());
                return result;
            }

            return null;
        }

        /// <summary>
        /// Some orders marked as "skip" may disappear before we load config file next time, then the saved id becomes otiose.
        /// Rebuild the id list before we save config and exit the program to get rid of the otiosed ids.
        /// </summary>
        private void _rebuildSkipIDList()
        {
            Config.OrderIDsToSkip.Clear();
            foreach (MyMarketOrder o in OrderSet.Orders)
            {
                if (o.Skip)
                {
                    Config.OrderIDsToSkip.Add(o.ID);
                }
            }
        }

        #endregion
    }
}