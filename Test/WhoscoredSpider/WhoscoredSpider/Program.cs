using System;
using System.Collections.Generic;

namespace WhoscoredSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            string leagueName = "Italy_SerieA";
            string dir = Globe.RootDir + leagueName + @"\";
            string liveScorePath = Globe.RootDir + leagueName + @"\LiveScores.txt";

            ContentFilter filter = new ContentFilter();
            List<MatchInfo> matchInfos = filter.GetAllMatchInfos(dir);
            
            /*
            Dictionary<int, string> matchIDs = filter.GetMatchIDs(dir);

            WhoscoredSpiderAsync spider = new WhoscoredSpiderAsync();
            spider.GetMatches(dir, matchIDs);
            */
            Console.ReadKey();
        }
    }
}