using System;
using System.Collections.Generic;
using System.IO;
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

        private const string matchInfoFilter = @"(?<=var initialMatchData =).*?(?:])";
        private const string playerRatingFilter = @"(?:\[\d+,\').+?(?:,\[\[\[)";

        public Dictionary<int, string> GetMatchIDs(string dir)
        {
            string liveScorePath = dir + @"LiveScores.txt";

            string htmlContent = ReadLiveStore(liveScorePath);
            string standingsContent = GetStandingsContent(htmlContent);

            Dictionary<int, int> lastMatches = GetLastMatches(dir);
            Dictionary<int, string> matchIDs = GetMatchIDsFromStandingsContent(standingsContent, lastMatches);

            return matchIDs;
        }

        private string ReadLiveStore(string fileName)
        {
            string htmlContent = "";

            try
            {
                StreamReader sr = new StreamReader(fileName, Encoding.Default);
                htmlContent = sr.ReadToEnd();
            }
            catch (Exception) { }

            return htmlContent;
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
        private Dictionary<int, string> GetMatchIDsFromStandingsContent(string standingsContent, Dictionary<int, int> lastMatches)
        {
            Dictionary<int, string> matchIDs = new Dictionary<int, string>();
            MatchCollection matches = Regex.Matches(standingsContent, liveScoresFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                int id = int.Parse(Regex.Match(Regex.Match(matches[i].Value, scoreIDFilter).Value, @"\d+").Value);

                if (!matchIDs.ContainsKey(id) && !lastMatches.ContainsKey(id))
                {
                    string url = Globe.WhoScoredMatchesUrl + id + @"/live";
                    matchIDs.Add(id, url);
                }
            }

            return matchIDs;
        }

        private Dictionary<int, int> GetLastMatches(string path)
        {
            Dictionary<int, int> lastMatches = new Dictionary<int, int>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fileInfos = dir.GetFiles();

            if (fileInfos.Length > 0)
            {
                foreach (FileInfo file in fileInfos)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.FullName);

                    if (fileName.Equals("LiveScores"))
                        continue;

                    int id = int.Parse(fileName);

                    if (!lastMatches.ContainsKey(id))
                        lastMatches.Add(id, id);
                }
            }

            return lastMatches;
        }

        public List<MatchInfo> GetAllMatchInfos(string dir)
        {
            List<MatchInfo> matchInfos = new List<MatchInfo>();
            DirectoryInfo directory = new DirectoryInfo(dir);
            FileInfo[] fileInfos = directory.GetFiles();

            if (fileInfos.Length > 0)
            {
                foreach (FileInfo file in fileInfos)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.FullName);

                    if (fileName.Equals("LiveScores"))
                        continue;

                    string htmlContent = ReadLiveStore(file.FullName);
                    MatchInfo matchInfo = GetMatchInfo(htmlContent);
                    matchInfo.MatchID = int.Parse(fileName);
                    matchInfo.Rating = GetMatchRating(htmlContent, matchInfo.HomeTeamID, matchInfo.HomeTeam, matchInfo.AwayTeamID, matchInfo.AwayTeam);

                    matchInfos.Add(matchInfo);
                }
            }

            return matchInfos;
        }

        private MatchInfo GetMatchInfo(string htmlContent)
        {
            MatchInfo matchInfo = new MatchInfo();

            string infoString = "";
            MatchCollection matches = Regex.Matches(htmlContent, matchInfoFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                infoString += matches[i].Value;
            }

            string[] infos = infoString.Split(new Char[] { '[', ',', '\'', ']' }, StringSplitOptions.RemoveEmptyEntries);

            if (infos.Length == 12)
            {
                matchInfo.HomeTeamID = int.Parse(infos[1]);
                matchInfo.AwayTeamID = int.Parse(infos[2]);
                matchInfo.HomeTeam = infos[3];
                matchInfo.AwayTeam = infos[4];
                matchInfo.StartTime = infos[5];
                matchInfo.Score = infos[11];
            }

            return matchInfo;
        }

        private MatchRating GetMatchRating(string htmlContent, int homeTeamID, string homeTeam, int awayTeamID, string awayTeam)
        {
            MatchRating matchRating = new MatchRating();
            string homeTeamFilter = @"(?<=\[\[" + homeTeamID + @",'" + homeTeam + @"',).*?(?:]]\r)";
            string awayTeamFilter = @"(?<=,\[" + awayTeamID + @",'" + awayTeam + @"',).*?(?:]]\r)";
            string homeTeamRatingString = "";
            string awayTeamRatingString = "";

            MatchCollection homeTeamMatches = Regex.Matches(htmlContent, homeTeamFilter, RegexOptions.Singleline);

            for (int i = 0; i < homeTeamMatches.Count; i++)
            {
                homeTeamRatingString += homeTeamMatches[i].Value;
            }

            MatchCollection awayTeamMatches = Regex.Matches(htmlContent, awayTeamFilter, RegexOptions.Singleline);

            for (int i = 0; i < awayTeamMatches.Count; i++)
            {
                awayTeamRatingString += awayTeamMatches[i].Value;
            }

            matchRating.HomeTeamID = homeTeamID;
            matchRating.HomeTeam = homeTeam;
            matchRating.HomeTeamRating = float.Parse(homeTeamRatingString.Split(',')[0]);
            matchRating.HomeTeamPlayerRatings = GetPlayerRatings(homeTeamRatingString);
            matchRating.AwayTeamID = awayTeamID;
            matchRating.AwayTeam = awayTeam;
            matchRating.AwayTeamRating = float.Parse(awayTeamRatingString.Split(',')[0]);
            matchRating.AwayTeamPlayerRatings = GetPlayerRatings(awayTeamRatingString);

            return matchRating;
        }

        private List<PlayerRating> GetPlayerRatings(string teamRatingString)
        {
            List<PlayerRating> playerRatings = new List<PlayerRating>();

            MatchCollection matches = Regex.Matches(teamRatingString, playerRatingFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                string[] playerRatingStrings = matches[i].Value.Split(new Char[] { '[', ',', '\'' }, StringSplitOptions.RemoveEmptyEntries);

                if (playerRatingStrings.Length == 3 && float.Parse(playerRatingStrings[2]) > 0)
                {
                    PlayerRating playerRating = new PlayerRating();
                    playerRating.PlayerID = int.Parse(playerRatingStrings[0]);
                    playerRating.Player = playerRatingStrings[1];
                    playerRating.Rating = float.Parse(playerRatingStrings[2]);

                    playerRatings.Add(playerRating);
                }
            }

            return playerRatings;
        }
    }
}
