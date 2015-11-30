using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InnerSpaceAPI;

namespace Eve_TradingAgent
{
    class Strategy
    {

        public Strategy(Character character, Config config)
        {
            _character = character;
            _config = config;
        }

        public double GetRecommendedPrice(MyMarketOrder order)
        {
            Log.WriteLog("Trying to find new price for picked order.");

            // get order situation (not mine)
            MarketSituation situation = order.GetMarketSituation();

            if (situation != null)
            {
                // filter orders by distance(currently compete with orders in same station.)
                //Temporaty only compete with same station.
                int StationID = _character.CurrentStationID;

                double HighestBuy = 0;
                List<MarketOrderBase> buyOrders = situation.BuyOrdersInMarket.OrderByDescending(o => o.Price).ToList();
                if (buyOrders.Any())
                {
                    HighestBuy = buyOrders.First().Price;
                    //  InnerSpace.Echo("highest buy" + HighestBuy.ToString());
                }

                double LowestSell = 0;
                List<MarketOrderBase> sellOrders = situation.SellOrdersInMarket.OrderBy(o => o.Price).ToList();
                if (sellOrders.Any())
                {
                    LowestSell = sellOrders.First().Price;
                    // InnerSpace.Echo("lowest sell" + LowestSell.ToString());

                }
                if (HighestBuy != 0 && LowestSell != 0)
                {
                    double score = (LowestSell - HighestBuy) / ((HighestBuy + LowestSell) / 2) * 100;

                    // InnerSpace.Echo("score" + score.ToString());

                    if (score > _config.ValidGapSizeRatio)
                    {
                        if (order.OrderType == OrderType.Sell)
                        {
                            return LowestSell - 0.01;
                        }
                        else
                        {
                            return HighestBuy + 0.01;
                        }
                    }
                }

                if (HighestBuy != 0)//means no sell order
                {
                    if (order.OrderType == OrderType.Buy)
                    {
                        return HighestBuy + 0.01;
                    }
                }

                if (LowestSell != 0)//means no buy order
                {
                    if (order.OrderType == OrderType.Sell)
                    {
                        return LowestSell - 0.01;
                    }
                }
            }

            Log.WriteLog("Done finding new price for picked order.");
            return -1;
        }

        private Character _character;
        private Config _config;
    }
}
