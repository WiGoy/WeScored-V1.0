using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace WhoScoredReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string leagueName = "Italy_SerieA";
            string dir = Globe.RootDir + leagueName + @"\";

            ContentFilter filter = new ContentFilter();
            List<MatchInfo> matchInfos = filter.GetAllMatchInfos(dir);

            //InsertMatchInfo(matchInfos);

            Console.WriteLine("End!");
            Console.ReadKey();
        }

        private static void InsertMatchInfo(List<MatchInfo> matchInfos)
        {
            foreach (MatchInfo matchInfo in matchInfos)
            {
                string matchInfoSQL = "INSERT INTO MatchStatistics (MatchID, StartTime, "
                    + "HomeTeamID, HomeTeamName, HomeTeamFirstHalfScore, HomeTeamSecondHalfScore, HomeTeamRating, "
                    + "AwayTeamID, AwayTeamName, AwayTeamFirstHalfScore, AwayTeamSecondHalfScore, AwayTeamRating) "
                    + "VALUES (" + matchInfo.MatchID + ", '" + matchInfo.StartTime + "', "
                    + matchInfo.HomeTeamID + ", '" + matchInfo.HomeTeamName + "', "
                    + matchInfo.HomeTeamFirstHalfScore + ", " + matchInfo.HomeTeamSecondHalfScore + ", " + matchInfo.HomeTeamRating + ", "
                    + matchInfo.AwayTeamID + ", '" + matchInfo.AwayTeamName + "', "
                    + matchInfo.AwayTeamFirstHalfScore + ", " + matchInfo.AwayTeamSecondHalfScore + ", " + matchInfo.AwayTeamRating + ")";

                int matchInfoResult = MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, matchInfoSQL, null);

                foreach (PlayerStatistics playerStatistics in matchInfo.HomeTeamPlayerStatistics)
                {
                    string playerRatingSQL = "INSERT INTO PlayerStatistics (PlayerID, PlayerName, TeamID, TeamName, MatchID, Rating) "
                        + "VALUES (" + playerStatistics.PlayerID + ", '" + playerStatistics.PlayerName + "', "
                        + matchInfo.HomeTeamID + ", '" + matchInfo.HomeTeamName + "', "
                        + matchInfo.MatchID + ", " + playerStatistics.Rating +  ")";

                    int playerRatingResult = MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, playerRatingSQL, null);
                }

                foreach (PlayerStatistics playerStatistics in matchInfo.AwayTeamPlayerStatistics)
                {
                    string playerRatingSQL = "INSERT INTO PlayerStatistics (PlayerID, PlayerName, TeamID, TeamName, MatchID, Rating) "
                        + "VALUES (" + playerStatistics.PlayerID + ", '" + playerStatistics.PlayerName + "', "
                        + matchInfo.AwayTeamID + ", '" + matchInfo.AwayTeamName + "', "
                        + matchInfo.MatchID + ", " + playerStatistics.Rating + ")";

                    int playerRatingResult = MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, playerRatingSQL, null);
                }
            }
        }
    }
}
