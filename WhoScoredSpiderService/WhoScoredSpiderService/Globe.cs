using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace WhoScoredSpiderService
{
    public class Globe
    {
        public const string WhoScoredUrl = "http://www.whoscored.com";
        public const string WhoScoredMatchesUrl = "http://www.whoscored.com/Matches/";

        public static string ConfigFile = System.AppDomain.CurrentDomain.BaseDirectory + @"whoscoredspider.ini";
        public static Dictionary<string, string> ConfigDic = new Dictionary<string, string>();

        public const string CONFIG_ROOT_DIR = "RootDir";
        public static string RootDir = "";

        public const string CONFIG_LEAGUES = "Leagues";
        public static Dictionary<string, string> LeaguesDic = new Dictionary<string, string>();

        private static string LogFile = System.AppDomain.CurrentDomain.BaseDirectory + @"log.txt";

        public static void WriteLog(string txt)
        {
            try
            {
                StreamWriter sw = new StreamWriter(LogFile, true, Encoding.Default);
                sw.WriteLine(txt);
                sw.Flush();
                sw.Close();
            }
            catch (Exception) { }
        }
    }
}
