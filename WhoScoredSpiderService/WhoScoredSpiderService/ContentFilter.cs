using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WhoScoredSpiderService
{
    class ContentFilter
    {
        //  u0022是双引号的ASCII码
        private const string standingsFilter = @"(?:DataStore\.prime\(\'standings\'[\s\S]).*?(?:\)\;)";
        private const string liveScoresFilter = @"(?:<a\sclass=).*?(?:\/>)";
        private const string scoreIDFilter = @"(id=\u0022).*?(\u0022)";

        public Dictionary<int, string> GetMatchIDs(string htmlContent, Dictionary<int, int> originalMatchIDs)
        {
            string standingsContent = GetStandingsContent(htmlContent);
            Dictionary<int, string> matchIDs = GetMatchIDsFromStandingsContent(standingsContent, originalMatchIDs);

            return matchIDs;
        }

        /// <summary>
        /// 根据关键字DataStore.prime('standings'过滤htmlContent
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        private string GetStandingsContent(string htmlContent)
        {
            string standingsContent = "";
            MatchCollection matches = Regex.Matches(htmlContent, standingsFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                standingsContent += matches[i].Value;
            }

            return standingsContent;
        }

        /// <summary>
        /// 将standingsInfo中的数据转换成LiveScore结构
        /// </summary>
        /// <param name="standingsInfo"></param>
        /// <returns></returns>
        private Dictionary<int, string> GetMatchIDsFromStandingsContent(string standingsContent, Dictionary<int, int> originalMatchIDs)
        {
            Dictionary<int, string> matchIDs = new Dictionary<int, string>();
            MatchCollection matches = Regex.Matches(standingsContent, liveScoresFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                int id = int.Parse(Regex.Match(Regex.Match(matches[i].Value, scoreIDFilter).Value, @"\d+").Value);

                if (!matchIDs.ContainsKey(id) && !originalMatchIDs.ContainsKey(id))
                {
                    string url = Globe.WhoScoredMatchesUrl + id + @"/live";
                    matchIDs.Add(id, url);
                }
            }

            return matchIDs;
        }
    }
}
