using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WhoscoredSpider
{
    class ContentFilter
    {
        private const string standingsFilter = @"(?:DataStore\.prime\(\'standings\'[\s\S]).*?(?:\)\;)";
        private const string liveScoresFilter = @"(?:<a\sclass=).*?(?:\/>)";
        //  u0022是双引号的ASCII码
        private const string scoreIDFilter = @"(id=\u0022).*?(\u0022)";
        private const string scoreTitleFilter = @"(?:title=\u0022).*?(?:\u0022)";
        //  用来匹配比分，如1-0,2-1之类
        private const string scoreFilter = @"\d+-\d+";

        private const string ratingFilter = @"(?:var initialMatchData =).*?(?:\]\;)";

        private Hashtable liveScoreHashtable = new Hashtable();

        /// <summary>
        /// 根据关键字standings过滤htmlContent
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public string GetStandingsInfoFromContent(string htmlContent)
        {
            string standingsInfo = "";
            MatchCollection matches = Regex.Matches(htmlContent, standingsFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                standingsInfo += matches[i].Value;
            }

            return standingsInfo;
        }

        /// <summary>
        /// 将standingsInfo中的数据转换成LiveScore结构
        /// </summary>
        /// <param name="standingsInfo"></param>
        /// <returns></returns>
        public List<LiveScore> GetLiveScoresFromStandingsInfo(string standingsInfo, string league)
        {
            List<LiveScore> liveScoreList = new List<LiveScore>();
            MatchCollection matches = Regex.Matches(standingsInfo, liveScoresFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                string liveScoreStr = matches[i].Value;
                int id = int.Parse(Regex.Match(Regex.Match(liveScoreStr, scoreIDFilter).Value, @"\d+").Value);

                if (!liveScoreHashtable.ContainsKey(id))
                {
                    Match titleMatch = Regex.Match(liveScoreStr, scoreTitleFilter);
                    LiveScore liveScore = AnalyzeTitle(titleMatch.Value.Split('\"')[1]);
                    liveScore.id = id;
                    liveScore.League = league;
                
                    liveScoreHashtable.Add(liveScore.id, liveScore.id);
                    liveScoreList.Add(liveScore);
                }
            }

            liveScoreHashtable.Clear();
            liveScoreHashtable = null;

            return liveScoreList;
        }

        public string GetRatingInfoFromContent(string htmlContent)
        {
            string ratingInfo = "";
            MatchCollection matches = Regex.Matches(htmlContent, ratingFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                ratingInfo += matches[i].Value;
            }

            return ratingInfo;
        }

        /// <summary>
        /// 把title拆分成HomeTeam, AwayTeam以及Score
        /// </summary>
        /// <param name="title"></param>
        /// <param name="liveScore"></param>
        private LiveScore AnalyzeTitle(string title)
        {
            LiveScore liveScore = new LiveScore();
            string[] titleArray = title.Split(' ');
            int scoreIndex = 0;

            for ( ; scoreIndex < titleArray.Length; scoreIndex++)
            {
                if (!Regex.IsMatch(titleArray[scoreIndex], scoreFilter))
                {
                    liveScore.HomeTeam += titleArray[scoreIndex] + " ";
                }
                else
                {
                    liveScore.Score = titleArray[scoreIndex];
                    break;
                }
            }

            for (scoreIndex += 1; scoreIndex < titleArray.Length; scoreIndex++)
            {
                liveScore.AwayTeam += titleArray[scoreIndex] + " ";
            }

            liveScore.HomeTeam = liveScore.HomeTeam.Trim();
            liveScore.AwayTeam = liveScore.AwayTeam.Trim();

            return liveScore;
        }
    }
}
