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

            PrintProperty(FileName_TeamProperty, TeamProperty);
            PrintProperty(FileName_PlayerProperty, PlayerProperty);

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

        private static void PrintProperty(string fileName, List<string> propertyList)
        {
            propertyList.Sort();

            //  for struct
            foreach (string property in propertyList)
            {
                if (property.Equals(POSITION))
                    WriteLog(fileName, "public string " + property + ";");
                else
                    WriteLog(fileName, "public float " + property + ";");
            }

            WriteLog(fileName, "");

            //  for const string
            foreach (string property in propertyList)
            {
                WriteLog(fileName, "public const string " + property.ToUpper() + " = \"" + property + "\";");
            }

            WriteLog(fileName, "");

            //  for database
            foreach (string property in propertyList)
            {
                if (property.Equals(POSITION))
                    WriteLog(fileName, "`" + property + "` VARCHAR(10) NULL,");
                else
                    WriteLog(fileName, "`" + property + "` FLOAT NULL,");
            }
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
