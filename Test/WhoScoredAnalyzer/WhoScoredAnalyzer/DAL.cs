using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WhoScoredAnalyzer
{
    public class DAL
    {
        public List<int> GetOriginalMatchIDs()
        {
            List<int> originalMatchIDs = new List<int>();
            string strsql = "select match_id From MatchInformation";
            DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, strsql, null);
            originalMatchIDs = (from r in ds.Tables[0].AsEnumerable() select r.Field<int>("match_id")).ToList();

            return originalMatchIDs;
        }

        public void GetPlayerStatistics()
        {
            string field = "rating";
            string strsql = @"SELECT player_id, player_name, team_name, avg(" + field + @"), count(*) FROM `PlayerStatistics` WHERE (((league = 'Germany_Bundesliga') or (league = 'England_BarclaysPL') or (league = 'Spain_LigaBBVA') or (league = 'Italy_SerieA') or (league = 'France_Ligue1')) and ((position = 'DL') or (position = 'DR'))) group by player_id ORDER BY avg(" + field + @") ASC";
            DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, strsql, null);
            
            var queryInfo = ds.Tables[0].AsEnumerable().Select(player => new
            {
                id = player.Field<int>("player_id"),
                name = player.Field<string>("player_name"),
                team = player.Field<string>("team_name"),
                value = player.Field<double>("avg(rating)"),
                count = player.Field<long>("count(*)")
            });
            
            foreach (var productInfo in queryInfo)
            {
                if (productInfo.count >= 5)
                    Console.WriteLine("Player: {0} Club: {1} " + field + ": {2} ", productInfo.name, productInfo.team, productInfo.value);
            }
        }

        public void GetTeamStatistics()
        {
            string field = "total_scoring_att";
            string strsql = @"SELECT team_id, team_name, league, avg(" + field + @") FROM `TeamStatistics` WHERE ((league = 'Germany_Bundesliga') or (league = 'England_BarclaysPL') or (league = 'Spain_LigaBBVA') or (league = 'Italy_SerieA') or (league = 'France_Ligue1')) group by team_id ORDER BY avg(" + field + @") ASC";
            DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, strsql, null);

            var queryInfo = ds.Tables[0].AsEnumerable().Select(team => new
            {
                id = team.Field<int>("team_id"),
                name = team.Field<string>("team_name"),
                league = team.Field<string>("league"),
                value = team.Field<double>("avg(total_scoring_att)")
            });

            foreach (var productInfo in queryInfo)
            {
                Console.WriteLine("Team: {0} League: {1} " + field + ": {2} ",
                    productInfo.name, productInfo.league, productInfo.value);
            }
        }
    }
}
