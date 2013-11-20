using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhoScoredSpiderService
{
    class UpdateDataBase
    {
        private string LeagueName;
        private string LeagueDir;

        public UpdateDataBase(string leagueName, string leagueDir)
        {
            this.LeagueName = leagueName;
            this.LeagueDir = leagueDir;
        }

        public void LoadFolder()
        {
            DirectoryInfo directory = new DirectoryInfo(LeagueDir);
            FileInfo[] fileInfos = directory.GetFiles();

            DAL dal = new DAL();
            List<int> originalMatchIDs = dal.GetOriginalMatchIDs();

            if (fileInfos.Length > 0)
            {
                Console.WriteLine(LeagueName + ": Start...");

                foreach (FileInfo file in fileInfos)
                {
                    if (file.Length < Globe.IncorrectFileSize)
                        continue;

                    string fileName = Path.GetFileNameWithoutExtension(file.FullName);

                    if (fileName.Equals("LiveScores") || originalMatchIDs.Contains(int.Parse(fileName)))
                        continue;

                    string htmlContent = Globe.LoadFile(file.FullName);

                    ContentFilter filter = new ContentFilter();
                    MatchInfo matchInfo = filter.GetMatchInfo(int.Parse(fileName), LeagueName, htmlContent);

                    InsertData(matchInfo);
                }
            }
        }

        private void InsertData(MatchInfo matchInfo)
        {
            DAL dal = new DAL();
            dal.InsertMatchInformation(matchInfo);
            dal.InsertTeamStatistics(matchInfo.HomeTeamStatistics, matchInfo.League, matchInfo.id, true);
            dal.InsertTeamStatistics(matchInfo.AwayTeamStatistics, matchInfo.League, matchInfo.id, false);

            foreach (PlayerStatistics player in matchInfo.HomeTeamPlayerStatistics)
            {
                dal.InsertPlayerStatistics(player, matchInfo.HomeTeamStatistics.id, matchInfo.HomeTeamStatistics.name, matchInfo.League, matchInfo.id, true);
            }

            foreach (PlayerStatistics player in matchInfo.AwayTeamPlayerStatistics)
            {
                dal.InsertPlayerStatistics(player, matchInfo.AwayTeamStatistics.id, matchInfo.AwayTeamStatistics.name, matchInfo.League, matchInfo.id, false);
            }
        }
    }
}
