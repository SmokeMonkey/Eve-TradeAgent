using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO;
using InnerSpaceAPI;

namespace Eve_TradingAgent
{
    class Log
    {
        public delegate void WriteLogEventHandler(Object evt);
        public static WriteLogEventHandler WriteLog;

        static Log()
        {
            _dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                   AppDomain.CurrentDomain.RelativeSearchPath,
                   "TradeAgent",
                   "Logs");

            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }
        }

        public static void Initialize(Action<object> outputToUIConsole)
        {
            WriteLog = new WriteLogEventHandler(outputToUIConsole);
            
            if (Config.EchoLoginInnerSpace)
            {
                WriteLog += echoLogToInnerSpaceConsole;
            }

            if (Config.WriteLogToFile)
            {
                WriteLog += writeLogToFile;
            }

            deleteOldLogs();
        }

        public static void SetCharacter(string characterName)
        {
            _characterName = characterName;
        }

        private static void deleteOldLogs()
        {
            Debug.Assert(Config.MaxiumDaysToKeepLog >= 7);

            System.IO.DirectoryInfo logFileInfo = new DirectoryInfo(_dir);

            foreach (FileInfo file in logFileInfo.GetFiles())
            {
                if ((System.DateTime.Now - file.CreationTime).Days > Config.MaxiumDaysToKeepLog)
                {
                    try
                    {
                        string filename = file.Name;
                        file.Delete();
                        WriteLog("Deteted old log: " + filename);
                    }
                    catch (Exception e)
                    {
                        WriteLog("Error when trying to detete old log: " + file.Name + ", exception: " + e.Message);
                    }
                }
            }
        }

        private static void echoLogToInnerSpaceConsole(Object evt)
        {
            Debug.Assert((evt is string) || (evt is Exception));

            if (evt is string)
            {
                InnerSpace.Echo(evt as string);
            }
            else if (evt is Exception)
            {
                InnerSpace.Echo("Exception: " + (evt as Exception).Message);
            }
        }

        private static void writeLogToFile(Object evt)
        {
            Debug.Assert((evt is string) || (evt is Exception));


            //just in case: we protect code with try.
            try
            {
                string filename = Path.Combine(_dir,
                    GetFilenameYYYMMDD("", ".log"));

                XElement xmlEntry = null;

                System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, true);

                if (evt is string)
                {
                    xmlEntry = new XElement("logEntry",
                            new XElement("Date", System.DateTime.Now.ToString()),
                            new XElement("Message", (evt as string)));
                }
                else if (evt is Exception)
                {
                    xmlEntry = new XElement("logEntry",
                            new XElement("Date", System.DateTime.Now.ToString()),
                            new XElement("Exception",
                                new XElement("Source", (evt as Exception).Source),
                                new XElement("Message", (evt as Exception).Message),
                                new XElement("Stack", (evt as Exception).StackTrace)
                             )//end exception
                        );
                    //has inner exception?
                    if ((evt as Exception).InnerException != null)
                    {
                        xmlEntry.Element("Exception").Add(
                            new XElement("InnerException",
                                new XElement("Source", (evt as Exception).InnerException.Source),
                                new XElement("Message", (evt as Exception).InnerException.Message),
                                new XElement("Stack", (evt as Exception).InnerException.StackTrace))
                            );
                    }
                }

                sw.WriteLine(xmlEntry);
                sw.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to write log to disk:" + e.Message, e);
            }
        }

        private static string GetFilenameYYYMMDD(string suffix, string extension)
        {
            if (!string.IsNullOrEmpty(_characterName))
            {
                return _characterName + System.DateTime.Now.ToString(" yyyy_MM_dd") + suffix + extension;
            }
            else
            {
                return "(CharacterUnknown)" + System.DateTime.Now.ToString(" yyyy_MM_dd") + suffix + extension;
            }
        }

        private static string _dir = null;
        private static string _characterName = null;
    }
}
