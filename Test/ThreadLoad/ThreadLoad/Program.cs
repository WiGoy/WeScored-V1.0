using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ThreadLoad
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo directory = new DirectoryInfo(Globe.RootDir);
            DirectoryInfo[] leagueDir = directory.GetDirectories();

            foreach (DirectoryInfo league in leagueDir)
            {
                LoadMatch loadMatch = new LoadMatch(league.Name, league.FullName);
                Thread leagueThread = new Thread(new ThreadStart(loadMatch.LoadFolder));
                leagueThread.Start();
            }

            Console.ReadKey();
        }
    }

    public class LoadMatch
    {
        private string LeagueName;
        private string LeagueDir;

        public LoadMatch(string leagueName, string leagueDir)
        {
            this.LeagueName = leagueName;
            this.LeagueDir = leagueDir;
        }

        public void LoadFolder()
        {
            DirectoryInfo directory = new DirectoryInfo(LeagueDir);
            FileInfo[] fileInfos = directory.GetFiles();

            if (fileInfos.Length > 0)
            {
                Console.WriteLine(LeagueName + ": Start...");

                foreach (FileInfo file in fileInfos)
                {
                    if (file.Length < Globe.IncorrectFileSize)
                        continue;

                    string fileName = Path.GetFileNameWithoutExtension(file.FullName);

                    if (fileName.Equals("LiveScores"))
                        continue;

                    string htmlContent = LoadFile(file.FullName);

                    ContentFilter filter = new ContentFilter();
                    MatchInfo matchInfo = filter.GetMatchInfo(int.Parse(fileName), LeagueName, htmlContent);

                    DAL dal = new DAL();
                    dal.InsertData(matchInfo);
                    Console.WriteLine(LeagueName + ": Match " + fileName + " Loading complete!");
                }

                Console.WriteLine(LeagueName + ": Complete!");
            }
        }

        private static string LoadFile(string fileName)
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
    }
}
