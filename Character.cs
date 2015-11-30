using System;
using EVE.ISXEVE;
using LavishScriptAPI;
using LavishVMAPI;
using InnerSpaceAPI;

namespace Eve_TradingAgent
{
    /// <summary>
    /// Data set about a character.
    /// </summary>
    // TODO: Add order number related skills when we want to enable auto selling.
    public class Character
    {
        /// <summary>
        /// Get the information of the character.
        /// </summary>
        public bool LoadCharacterInfo()
        {
            Me me;
            using (new FrameLock(true))
            {
                EVE.ISXEVE.EVE eve = new EVE.ISXEVE.EVE();

                // me is not persistent, we can't just keep a static reference somewhere,have to renew every frame;
                me = new Me();

                name = me.Name;
                currentStationID = me.StationID;

                // TODO: Support remote modify in future.
                daytradingLevel = 0;
            }

            bool succeed = !string.IsNullOrEmpty(Name);

            return succeed;
        }

        # region public property

        public string Name
        {
            get
            {
                return name;
            }
        }

        public int CurrentStationID
        {
            get
            {
                return currentStationID;
            }
        }

        public int ModifyRange
        {
            get
            {
                return getOperationRange(daytradingLevel);
            }
        }

        # endregion

        # region private field
        private string name;
        private int currentStationID;

        /// <summary>
        /// Allows for remote modification of buy and sell orders.  Each level of skill increases the range at which orders may be modified. 
        /// Level 1 allows for modification of orders within the same solar system, Level 2 extends that range to systems within 5 jumps, 
        /// and each subsequent level then doubles it. Level 5 allows for market order modification anywhere within current region.
        /// </summary>
        private int daytradingLevel;

        # endregion

        /// <summary>
        /// Compute the range within which the character is able to operate (create/modify/etc) the market orders.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        private int getOperationRange(int skillLevel)
        {
            if (skillLevel >= 0 && skillLevel <= 1)
            {
                return skillLevel - 1;
            }
            else if (skillLevel <= 4)
            {
                int i = skillLevel - 2;
                int jumps = 5;
                while (i > 0)
                {
                    jumps = jumps * 2;
                    i--;
                }
                return jumps;
            }
            else if (skillLevel == 5)
            {
                return 32767;
            }
            else
            {
                Log.WriteLog(new ArgumentOutOfRangeException("Unexpected Skill Level"));
                // Return minimal range instead of crash.
                return -1;
            }
        }
    }
}