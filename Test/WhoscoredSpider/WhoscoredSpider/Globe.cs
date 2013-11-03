using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhoscoredSpider
{
    public class Globe
    {
        public const string WhoScoredUrl = "http://www.whoscored.com";
        public const string WhoScoredMatchesUrl = "http://www.whoscored.com/Matches/";
        public static Dictionary<string, string> ConfigDic = new Dictionary<string, string>();

        public static string configFile = @"E:\whoscoredspider.ini";

        public const string CONFIG_ROOT_DIR = "RootDir";
        public static string RootDir = @"C:\Workspace\GitHub\WhoScored\Test\";

        public const string CONFIG_LEAGUES = "Leagues";
        public static Dictionary<string, string> LeaguesDic = new Dictionary<string, string>();

        

        public static void WriteLog(string logFile, string txt)
        {
            try
            {
                StreamWriter sw = new StreamWriter(logFile, true, Encoding.Default);
                sw.WriteLine(txt);
                sw.Flush();
                sw.Close();
            }
            catch (Exception) { }
        }
    }
    
    #region 各联赛对应关键字
    public class Leagues
    {
        public const string England_BarclaysPL = @"/Regions/252/Tournaments/2/England-Premier-League";
        public const string France_Ligue1 = @"/Regions/74/Tournaments/22/France-Ligue-1";
        public const string Germany_Bundesliga = @"/Regions/81/Tournaments/3/Germany-Bundesliga";
        public const string Italy_SerieA = @"/Regions/108/Tournaments/5/Italy-Serie-A";
        public const string Netherlands_Eredivisie = @"/Regions/155/Tournaments/13/Netherlands-Eredivisie";
        public const string Portugal_LigaPortuguesa = @"/Regions/177/Tournaments/21/Portugal-Liga-Sagres";
        public const string Russia_RussianLeague = @"/Regions/182/Tournaments/77/Russia-Premier-League";
        public const string Spain_LigaBBVA = @"/Regions/206/Tournaments/4/Spain-La-Liga";
    }
    #endregion

    #region 数据结构
    public class MatchInfo
    {
        public int MatchID;
        public string StartTime;
        public int HomeTeamID;
        public string HomeTeam;
        public int AwayTeamID;
        public string AwayTeam;
        public string Score;
        public MatchRating Rating;
    }

    public class MatchRating
    {
        public int MatchID;
        public int HomeTeamID;
        public string HomeTeam;
        public float HomeTeamRating;
        public List<PlayerRating> HomeTeamPlayerRatings;
        public int AwayTeamID;
        public string AwayTeam;
        public float AwayTeamRating;
        public List<PlayerRating> AwayTeamPlayerRatings;
    }

    public class PlayerRating
    {
        public int PlayerID;
        public string Player;
        public float Rating;
    }
    #endregion
}
