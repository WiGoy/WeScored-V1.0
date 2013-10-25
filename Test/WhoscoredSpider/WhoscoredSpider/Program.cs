using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WhoscoredSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration config = new Configuration();
            config.SetConfiguration();
            
            WhoScoredSpider whoscoredSpider = new WhoScoredSpider();

            foreach (KeyValuePair<string, string> item in Globe.LeaguesDic)
            {
                whoscoredSpider.GetLiveScores(item.Value, item.Key);
            }

            Console.ReadKey();
        }
    }
}