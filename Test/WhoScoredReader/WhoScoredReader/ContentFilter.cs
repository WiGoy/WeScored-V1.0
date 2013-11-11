using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WhoScoredReader
{
    class ContentFilter
    {
        private const string matchInfoFilter = @"(?<=var initialMatchData =).*?(?:])";
        private const string statisticsFilter = @"(?:\[\d+,\').+?(?:]]]],)";
        private const string playerRatingFilter = @"(?:\[\d+,\').+?(?:,\[\[\[)";

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
                matchInfo.HomeTeamName = infos[3];
                matchInfo.AwayTeamName = infos[4];
                matchInfo.StartTime = DateTime.Parse(infos[5]).ToString("yyyy/MM/dd HH:mm:ss");
                string[] firstHalfScore = infos[9].Split(':');

                if (firstHalfScore.Length == 2)
                {
                    matchInfo.HomeTeamFirstHalfScore = int.Parse(firstHalfScore[0]);
                    matchInfo.AwayTeamFirstHalfScore = int.Parse(firstHalfScore[1]);
                }

                string[] secondHalfScore = infos[10].Split(':');

                if (secondHalfScore.Length == 2)
                {
                    matchInfo.HomeTeamSecondHalfScore = int.Parse(secondHalfScore[0]) - matchInfo.HomeTeamFirstHalfScore;
                    matchInfo.AwayTeamSecondHalfScore = int.Parse(secondHalfScore[1]) - matchInfo.AwayTeamFirstHalfScore;
                }

                string homeTeamFilter = @"(?:\[\[" + matchInfo.HomeTeamID + @",'" + matchInfo.HomeTeamName + @"',).*?(?:]]\r)";
                string homeTeamStr = Regex.Match(htmlContent, homeTeamFilter, RegexOptions.Singleline).Value;
                string homeTeamStatisticsStr = Regex.Match(homeTeamStr, statisticsFilter, RegexOptions.Singleline).Value;

                string awayTeamFilter = @"(?:,\[" + matchInfo.AwayTeamID + @",'" + matchInfo.AwayTeamName + @"',).*?(?:]]\r)";
                string awayTeamStr = Regex.Match(htmlContent, awayTeamFilter, RegexOptions.Singleline).Value;
                string awayTeamStatisticsStr = Regex.Match(awayTeamStr, statisticsFilter, RegexOptions.Singleline).Value;

                /*matchInfo.HomeTeamRating = float.Parse(homeTeamString.Split(',')[0]);
                matchInfo.HomeTeamPlayerStatistics = GetPlayerStatistics(homeTeamString);


                string awayTeamFilter = @"(?:,\[" + matchInfo.AwayTeamID + @",'" + matchInfo.AwayTeamName + @"',).*?(?:]]\r)";
                string awayTeamRatingString = "";
                MatchCollection awayTeamMatches = Regex.Matches(htmlContent, awayTeamFilter, RegexOptions.Singleline);
                
                for (int i = 0; i < awayTeamMatches.Count; i++)
                {
                    awayTeamRatingString += awayTeamMatches[i].Value;
                }

                matchInfo.AwayTeamRating = float.Parse(awayTeamRatingString.Split(',')[0]);
                matchInfo.AwayTeamPlayerStatistics = GetPlayerStatistics(awayTeamRatingString);*/
            }

            return matchInfo;
        }

        private List<PlayerStatistics> GetPlayerStatistics(string ratingString)
        {
            List<PlayerStatistics> playerStatisticsList = new List<PlayerStatistics>();

            MatchCollection playerStatisticsMatches = Regex.Matches(ratingString, playerRatingFilter, RegexOptions.Singleline);

            for (int i = 0; i < playerStatisticsMatches.Count; i++)
            {
                string[] playerStatisticsStrings = playerStatisticsMatches[i].Value.Split(new Char[] { '[', ',', '\'' }, StringSplitOptions.RemoveEmptyEntries);

                if (playerStatisticsStrings.Length == 3 && float.Parse(playerStatisticsStrings[2]) > 0)
                {
                    PlayerStatistics playerStatistics = new PlayerStatistics();
                    playerStatistics.PlayerID = int.Parse(playerStatisticsStrings[0]);
                    playerStatistics.PlayerName = playerStatisticsStrings[1];
                    playerStatistics.Rating = float.Parse(playerStatisticsStrings[2]);

                    playerStatisticsList.Add(playerStatistics);
                }
            }

            return playerStatisticsList;
        }

        private void GetTeamStatistics(string teamStatisticsStr)
        {

        }
    }
}
