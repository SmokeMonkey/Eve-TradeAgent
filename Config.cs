using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;
using InnerSpaceAPI;
using System.Xml;

namespace Eve_TradingAgent
{
    public class Config
    {
        public Config()
        {
            _random = new Random(DateTime.Now.Millisecond);

            _dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                   AppDomain.CurrentDomain.RelativeSearchPath,
                   "TradeAgent",
                   "Configs");

            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }
        }

        #region general(static) configs

        // All these files should have upper and lower bounds; deal with them when saving /loading
        public static bool WriteLogToFile = true;
        public static bool EchoLoginInnerSpace = false;
        public static int MaxiumLogNumberToDisplay = 500;
        public static int MaxiumDaysToKeepLog = 30;

        /// <summary>
        /// load general configs from file when it exists, otherwise use default value.
        /// General configs are common configs that should be shared between characters or processes.
        /// </summary>
        public void LoadGeneralConfig()
        {
            string filename = Path.Combine(_dir,
                    _getGeneralConfigFilename("General_Config", ".xml"));

            try
            {
                _generalConfigFile = XDocument.Load(filename);

                //load
                _loadBooleanItem(_generalConfigFile, "WriteLogToFile", ref WriteLogToFile);
                _loadBooleanItem(_generalConfigFile, "EchoLoginInnerSpace", ref EchoLoginInnerSpace);
                _loadValueItem(_generalConfigFile, "MaxiumLogNumberToDisplay", 10, -1, ref MaxiumLogNumberToDisplay);
                _loadValueItem(_generalConfigFile, "MaxiumDaysToKeepLog", 1, -1, ref MaxiumDaysToKeepLog);
            }
            catch (Exception e)
            {
                if ((e is XmlException) || (e is FileNotFoundException))
                {
                    // Do nothing here, just use default value;
                }
                else
                {
                    throw;
                }
            }
        }

        public void SaveGeneralConfig()
        {
            string filename = Path.Combine(_dir,
                    _getGeneralConfigFilename("General_Config", ".xml"));

            //try create
            XDocument Doc = new XDocument();

            XElement Root = new XElement(
            new XElement("Configs",
                new XElement("WriteLogToFile", WriteLogToFile),
                new XElement("EchoLoginInnerSpace", EchoLoginInnerSpace),
                new XElement("MaxiumLogNumberToDisplay", MaxiumLogNumberToDisplay),
                new XElement("MaxiumDaysToKeepLog", MaxiumDaysToKeepLog))
                );

            Doc.Add(Root);

            // TODO error handling;
            //if fail throw(not really necessary)
            // This will automatically replace the old values, no need to consider about it.
            Doc.Save(filename);
        }

        #endregion

        #region character specific configs
        // cooldown of evaluate and maybe modify a SINGLE order.
        public int RandomizedOrderProcessIntervalInMilliSec
        {
            get
            {
                return makeRandom(_orderProcessIntervalInMilliSec);
            }
        }
        public int RandomizedOrderModifyIntervalInMilliSec
        {
            get
            {
                return makeRandom(_orderModifyIntervalInMilliSec);
            }
        }
        public int RandomizedTurnsBetweenReloadOrderList
        {
            get
            {
                return makeRandom(_turnsBetweenReloadOrderList);
            }
        }
        public int _orderProcessIntervalInMilliSec = 10000;
        public int _orderModifyIntervalInMilliSec = 500000;
        public int _turnsBetweenReloadOrderList = 8;
        // This value is fixed
        public int IntervalOfProcessingSameOrder = 60000;
        public int ValidGapSizeRatio = 10;

        public byte[] ListViewState;
        public bool ShowOrdersOutOfReach = true;
        public bool ShowOrdersCoolingDown = true;
        public bool ShowOrdersNeedNoUpdate = true;
        public bool ShowOrdersMarkedSkip = true;
        public bool ShowLogWindow = false;
        // TODO save window size and position of mainwindow/logwindow
        public List<long> OrderIDsToSkip = new List<long>();

        /// <summary>
        /// load character configs from file when it exists, otherwise use default value.
        /// Character configs are specified configs for each character or process.
        /// </summary>
        /// <param name="characterName"></param>
        public void LoadCharacterConfig(string characterName)
        {
            if (!string.IsNullOrEmpty(characterName))
            {
                _characterName = characterName;

                string filename = Path.Combine(_dir,
                                   _getCharacterConfigFilename("_Config", ".xml"));

                try
                {
                    _characterConfigFile = XDocument.Load(filename);

                    //load
                    _loadValueItem(_characterConfigFile, "OrderProcessIntervalInMilliSec", 6000, 300000, ref _orderProcessIntervalInMilliSec);
                    _loadValueItem(_characterConfigFile, "OrderModifyIntervalInMilliSec", 390000, 600000, ref _orderModifyIntervalInMilliSec);
                    _loadValueItem(_characterConfigFile, "TurnsBetweenReloadOrderList", 5, 100, ref _turnsBetweenReloadOrderList);
                    _loadValueItem(_characterConfigFile, "ValidGapSizeRatio", 2, 50, ref ValidGapSizeRatio);
                    _loadByteArrayItem(_characterConfigFile, "ListViewState", ref ListViewState);
                    _loadBooleanItem(_characterConfigFile, "ShowOrdersOutOfReach", ref ShowOrdersOutOfReach);
                    _loadBooleanItem(_characterConfigFile, "ShowOrdersCoolingDown", ref ShowOrdersCoolingDown);
                    _loadBooleanItem(_characterConfigFile, "ShowOrdersNeedNoUpdate", ref ShowOrdersNeedNoUpdate);
                    _loadBooleanItem(_characterConfigFile, "ShowOrdersMarkedSkip", ref ShowOrdersMarkedSkip);
                    _loadBooleanItem(_characterConfigFile, "ShowLogWindow", ref ShowLogWindow);
                    _loadListItem(_characterConfigFile, "OrderIDsToSkip", "ID", ref OrderIDsToSkip);
                }
                catch (Exception e)
                {
                    if ((e is XmlException) || (e is FileNotFoundException))
                    {
                        // Do nothing here, just use the default value.
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveCharacterConfig()
        {
            Debug.Assert(!string.IsNullOrEmpty(_characterName));

            string filename = Path.Combine(_dir,
                    _getCharacterConfigFilename("_Config", ".xml"));

            //try create
            XDocument Doc = new XDocument();

            XElement Root = new XElement(
            new XElement("Configs",
                 new XElement("OrderProcessIntervalInMilliSec", _orderProcessIntervalInMilliSec),
                 new XElement("OrderModifyIntervalInMilliSec", _orderModifyIntervalInMilliSec),
                 new XElement("TurnsBetweenReloadOrderList", _turnsBetweenReloadOrderList),
                 new XElement("ValidGapSizeRatio", ValidGapSizeRatio),
                 new XElement("ListViewState", System.Convert.ToBase64String(ListViewState)),
                 new XElement("ShowOrdersOutOfReach", ShowOrdersOutOfReach),
                 new XElement("ShowOrdersCoolingDown", ShowOrdersCoolingDown),
                 new XElement("ShowOrdersNeedNoUpdate", ShowOrdersNeedNoUpdate),
                 new XElement("ShowOrdersMarkedSkip", ShowOrdersMarkedSkip),
                 new XElement("ShowLogWindow", ShowLogWindow),
                 new XElement("OrderIDsToSkip", from id in OrderIDsToSkip
                                                  select new XElement("ID", id)))
                 );

            Doc.Add(Root);

            // TODO error handling;
            //if fail throw(not really necessary)
            // This will automatically replace the old values, no need to consider about it.
            Doc.Save(filename);
        }
        #endregion

        #region private fields and methods

        private Random _random;
        private string _dir = null;
        private string _characterName = null;

        // Configs in this file are static
        private XDocument _generalConfigFile;
        // Configs in this file are character(process) specific and accessed through Config instance.
        private XDocument _characterConfigFile;

        private void _loadValueItem(XDocument document, string itemName, int lowerBound, int upperBound, ref int result)
        {
            Debug.Assert((lowerBound == -1) || (lowerBound > 0));
            Debug.Assert((upperBound == -1) || (upperBound > 0));
            Debug.Assert((upperBound == -1) || (lowerBound == -1) || (upperBound >= lowerBound));

            XElement Configs = document.Element("Configs");

            if (Configs.Elements(itemName).Any() &&
                !string.IsNullOrEmpty(Configs.Elements(itemName).First().Value))
            {
                int ParsedValue;
                if (int.TryParse(Configs.Element(itemName).Value, out ParsedValue))
                {
                    if (((lowerBound == -1) ||
                         (ParsedValue >= lowerBound))
                         &&
                        ((upperBound == -1) ||
                         (ParsedValue <= upperBound)))
                    {
                        result = ParsedValue;
                    }
                }
            }
        }

        private void _loadListItem(XDocument document, string listName, string listItemName, ref List<long> result)
        {
            result = new List<long>();

            XElement Configs = document.Element("Configs");

            if (Configs.Elements(listName).Any() &&
                !string.IsNullOrEmpty(Configs.Elements(listName).First().Value))
            {
                XElement ItemList = Configs.Elements(listName).First();

                if (ItemList.Elements(listItemName).Any() &&
                    !string.IsNullOrEmpty(ItemList.Elements(listItemName).First().Value))
                {
                    var List = from itm in ItemList.Elements(listItemName)
                               select itm;

                    foreach (var item in List)
                    {
                        long ParsedValue;
                        if (long.TryParse(item.Value, out ParsedValue))
                        {
                            if (ParsedValue > 0)
                            {

                                result.Add(ParsedValue);
                            }
                            else
                            {
                                Log.WriteLog("Ignored unexpected order id from config.");
                            }
                        }
                    }

                }
            }
        }

        private void _loadBooleanItem(XDocument document, string itemName, ref bool result)
        {
            XElement Configs = document.Element("Configs");
            if (Configs.Elements(itemName).Any() &&
                    !string.IsNullOrEmpty(Configs.Elements(itemName).First().Value))
            {
                bool ParsedValue;
                if (Boolean.TryParse(Configs.Element(itemName).Value, out ParsedValue))
                {
                    result = ParsedValue;
                }
            }
        }

        private void _loadByteArrayItem(XDocument document, string itemName, ref byte[] result)
        {
            XElement Configs = document.Element("Configs");
            if (Configs.Elements(itemName).Any() &&
                    !string.IsNullOrEmpty(Configs.Elements(itemName).First().Value))
            {
                result = System.Convert.FromBase64String(Configs.Element(itemName).Value);
            }
        }

        private string _getGeneralConfigFilename(string suffix, string extension)
        {
            return suffix + extension;
        }

        private string _getCharacterConfigFilename(string suffix, string extension)
        {
            Debug.Assert(!string.IsNullOrEmpty(_characterName));

            return _characterName + suffix + extension;

        }

        /// <summary>
        /// Make a time interval random to make it hard to detect
        /// </summary>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        private int makeRandom(int timeInterval)
        {
            Debug.Assert(timeInterval > 5000);
            return _random.Next(timeInterval / 5 * 4, timeInterval / 5 * 6);
        }

        #endregion
    }
}
