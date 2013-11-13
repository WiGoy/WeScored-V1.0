using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace StatisticsFinder
{
    class Program
    {
        private static string RootDir = @"E:\WhoScored\htmlContent";
        private static string FileName_TeamProperty = System.AppDomain.CurrentDomain.BaseDirectory + "TeamProperty.txt";
        private static string FileName_PlayerProperty = System.AppDomain.CurrentDomain.BaseDirectory + "PlayerProperty.txt";

        private static List<string> TeamProperty = new List<string>();
        private static List<string> PlayerProperty = new List<string>();
        
        private const string matchInfoFilter = @"(?<=var initialData =).*?(?:])";
        private const string playerFilter = @"(?:\[\d+,\').+?(?:]\r\n,)";
        private const string propertyFilter = @"(?<=\[')\D\w*?(?=',\[)";

        private const string POSITION = "position";

        static void Main(string[] args)
        {
            DirectoryInfo root = new DirectoryInfo(RootDir);
            DirectoryInfo[] leagueDir = root.GetDirectories();

            for (int i = 0; i < leagueDir.Length; i++)
            {
                FileInfo[] matchFileInfos = leagueDir[i].GetFiles();

                for (int j = 0; j < matchFileInfos.Length; j++)
                {
                    string matchFile = Path.GetFileNameWithoutExtension(matchFileInfos[i].FullName);

                    if (matchFile.Equals("LiveScores"))
                        continue;

                    string htmlContent = LoadContent(matchFileInfos[i].FullName);
                    GetTeamProperty(htmlContent);
                    GetPlayerProperty(htmlContent);
                }
            }

            PrintTeamProperty(TeamProperty);
            PrintPlayerProperty(PlayerProperty);

            Console.WriteLine("Complete!");
            Console.ReadKey();
        }

        private static void GetTeamProperty(string htmlContent)
        {
            string[] infos = Regex.Match(htmlContent, matchInfoFilter, RegexOptions.Singleline).Value.Split(new Char[] { '[', ',', '\'', ']' }, StringSplitOptions.RemoveEmptyEntries);

            if (infos.Length == 12)
            {
                string homeTeamFilter = @"(?:\[" + int.Parse(infos[1]) + @",'" + infos[3] + @"',).*?(?:]]]],)";
                string homeTeamPropertyContent = Regex.Match(htmlContent, homeTeamFilter, RegexOptions.Singleline).Value;
                string awayTeamFilter = @"(?:,\[" + int.Parse(infos[2]) + @",'" + infos[4] + @"',).*?(?:]]]],)";
                string awayTeamPropertyContent = Regex.Match(htmlContent, awayTeamFilter, RegexOptions.Singleline).Value;

                UpdateTeamProperty(homeTeamPropertyContent);
                UpdateTeamProperty(awayTeamPropertyContent);
            }
        }

        private static void UpdateTeamProperty(string propertyContent)
        {
            MatchCollection propertyMatches = Regex.Matches(propertyContent, propertyFilter, RegexOptions.Singleline);

            foreach (Match match in propertyMatches)
            {
                string property = match.Value;

                if (!TeamProperty.Contains(property))
                    TeamProperty.Add(property);
            }
        }

        private static void PrintTeamProperty(List<string> propertyList)
        {
            propertyList.Sort();

            string structStr = "";
            string constantStr = "";
            string functionStr = "";
            string dbCreateStr = "";
            string dbInsertStr1 = "\"INSERT INTO TeamStatistics (team_id, team_name, league, match_id, home, rating";
            string dbInsertStr2 = ") VALUES (\" + team.id + \", '\" + team.name + \"', '\" + league + \"', \" + matchID + \", \" + home + \", \" + team.rating + \", ";

            foreach (string property in propertyList)
            {
                structStr += "public float " + property + ";\r\n";
                constantStr += "public const string " + property.ToUpper() + " = \"" + property + "\";\r\n";
                functionStr += "team." + property + " = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter." + property.ToUpper() + "));\r\n";
                dbCreateStr += "  `" + property + "` FLOAT DEFAULT NULL,\r\n";
                dbInsertStr1 += ", " + property;
                dbInsertStr2 += "\" + team." + property + " + \", ";
            }

            string dbInsertStr = dbInsertStr1 + dbInsertStr2;
            dbInsertStr = dbInsertStr.Remove(dbInsertStr.Length - 2) + ")\";\r\n";

            WriteLog(FileName_TeamProperty, structStr);
            WriteLog(FileName_TeamProperty, constantStr);
            WriteLog(FileName_TeamProperty, functionStr);
            WriteLog(FileName_TeamProperty, dbCreateStr);
            WriteLog(FileName_TeamProperty, dbInsertStr);
        }

        private static void GetPlayerProperty(string htmlContent)
        {
            string[] infos = Regex.Match(htmlContent, matchInfoFilter, RegexOptions.Singleline).Value.Split(new Char[] { '[', ',', '\'', ']' }, StringSplitOptions.RemoveEmptyEntries);

            if (infos.Length == 12)
            {
                string homeTeamFilter = @"(?<=\[" + int.Parse(infos[1]) + @",'" + infos[3] + @"',).*?(?:]]\r)";
                string homeTeamPropertyContent = Regex.Match(htmlContent, homeTeamFilter, RegexOptions.Singleline).Value;
                string awayTeamFilter = @"(?<=,\[" + int.Parse(infos[2]) + @",'" + infos[4] + @"',).*?(?:]]\r)";
                string awayTeamPropertyContent = Regex.Match(htmlContent, awayTeamFilter, RegexOptions.Singleline).Value;

                UpdatePlayerProperty(homeTeamPropertyContent);
                UpdatePlayerProperty(awayTeamPropertyContent);
            }
        }

        private static void UpdatePlayerProperty(string propertyContent)
        {
            MatchCollection playerMatches = Regex.Matches(propertyContent, playerFilter, RegexOptions.Singleline);

            for (int i = 0; i < playerMatches.Count; i++)
            {
                MatchCollection propertyMatches = Regex.Matches(playerMatches[i].Value, propertyFilter, RegexOptions.Singleline);

                foreach (Match match in propertyMatches)
                {
                    string property = match.Value;

                    if (!PlayerProperty.Contains(property))
                        PlayerProperty.Add(property);
                }
            }
        }

        private static void PrintPlayerProperty(List<string> propertyList)
        {
            propertyList.Sort();

            string structStr = "";
            string constantStr = "";
            string functionStr = "";
            string dbCreateStr = "";
            string dbInsertStr1 = "\"INSERT INTO PlayerStatistics (player_id, player_name, team_id, team_name, league, match_id, home";
            string dbInsertStr2 = ") VALUES (\" + player.id + \", '\" + player.name + \"', \" + teamID + \", '\" + teamName + \"', '\" + league + \"', \" + matchID + \", \" + home + \", ";

            foreach (string property in propertyList)
            {
                if (property.Equals(POSITION))
                {
                    structStr += "public string " + property + ";\r\n";
                    functionStr += "player." + property + " = GetStatistics(playerContent, PlayerStatisticsFilter." + property.ToUpper() + ").Replace(\"'\", \"\");\r\n";
                    dbCreateStr += "  `" + property + "` VARCHAR(10) NULL,\r\n";
                    dbInsertStr2 += "'\" + player." + property + " + \"', ";
                }
                else
                {
                    structStr += "public float " + property + ";\r\n";
                    functionStr += "player." + property + " = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter." + property.ToUpper() + "));\r\n";
                    dbCreateStr += "  `" + property + "` FLOAT NULL,\r\n";
                    dbInsertStr2 += "\" + player." + property + " + \", ";
                }

                constantStr += "public const string " + property.ToUpper() + " = \"" + property + "\";\r\n";
                dbInsertStr1 += ", " + property;
            }

            string dbInsertStr = dbInsertStr1 + dbInsertStr2;
            dbInsertStr = dbInsertStr.Remove(dbInsertStr.Length - 2) + ")\";\r\n";

            WriteLog(FileName_PlayerProperty, structStr);
            WriteLog(FileName_PlayerProperty, constantStr);
            WriteLog(FileName_PlayerProperty, functionStr);
            WriteLog(FileName_PlayerProperty, dbCreateStr);
            WriteLog(FileName_PlayerProperty, dbInsertStr);
        }

        private static string LoadContent(string fileName)
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

        private static void WriteLog(string fileName, string txt)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fileName, true, Encoding.Default);
                sw.WriteLine(txt);
                sw.Flush();
                sw.Close();
            }
            catch (Exception) { }
        }
    }
}
