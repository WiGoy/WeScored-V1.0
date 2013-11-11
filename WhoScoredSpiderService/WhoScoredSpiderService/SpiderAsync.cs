using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WhoScoredSpiderService
{
    class SpiderAsync
    {
        private const string LeagueFileName = "LiveScores.txt";
        private const long IncorrectFileSize = 5 * 1024;

        public void GetAllLeagues()
        {
            if (Globe.LeaguesDic.Count > 0)
            {
                foreach (KeyValuePair<string, string> item in Globe.LeaguesDic)
                {
                    GetLeague(item.Key, item.Value);
                }
            }
        }

        public async void GetLeague(string leagueName, string leagueUrl)
        {
            string dir = Globe.RootDir + "\\" + leagueName + "\\";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string url = Globe.WhoScoredUrl + leagueUrl;
            string fileName = dir + LeagueFileName;

            if (File.Exists(fileName))
                File.Delete(fileName);

            Task<string> getHtmlContentByUrl = GetHtmlContentByUrl(url);
            string htmlContent = await GetHtmlContentByUrl(url);

            SaveContent(fileName, htmlContent);
        }

        public void GetAllMatches()
        {
            if (Globe.LeaguesDic.Count > 0)
            {
                ContentFilter filter = new ContentFilter();

                foreach (KeyValuePair<string, string> item in Globe.LeaguesDic)
                {
                    string directory = Globe.RootDir + "\\" + item.Key + "\\";
                    string fileName = directory + LeagueFileName;
                    string htmlContent = LoadContent(fileName);

                    Dictionary<int, int> originalMatchIDs = GetOriginalMatchIDs(directory);
                    Dictionary<int, string> matchIDs = filter.GetMatchIDs(htmlContent, originalMatchIDs);
                    GetMatches(item.Key, matchIDs);
                }
            }
        }

        public async void GetMatches(string leagueName, Dictionary<int, string> matchIDs)
        {
            if (matchIDs.Count > 0)
            {
                string dir = Globe.RootDir + "\\" + leagueName + "\\";

                foreach (KeyValuePair<int, string> match in matchIDs)
                {
                    Task<string> getHtmlContentByUrl = GetHtmlContentByUrl(match.Value);
                    string matchHtmlContent = await GetHtmlContentByUrl(match.Value);

                    string fileName = dir + match.Key + ".txt";
                    SaveContent(fileName, matchHtmlContent);
                }
            }
        }

        private async Task<string> GetHtmlContentByUrl(string url)
        {
            string htmlContent = "";
            Globe.WriteLog("Downloading from url: " + url);

            try
            {
                // Create a New HttpClient object.
                HttpClient client = new HttpClient();
                Task<string> getHtmlContentByUrl = client.GetStringAsync(url);

                htmlContent = await getHtmlContentByUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SpiderAsync.GetHtmlContentByUrl: " + ex.Message);
            }

            return htmlContent;
        }

        private void SaveContent(string fileName, string htmlContent)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fileName, true, Encoding.Default);
                sw.WriteLine(htmlContent);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                Globe.WriteLog("SpiderAsync.SaveContent: " + ex.Message);
            }
        }

        private string LoadContent(string fileName)
        {
            string htmlContent = "";

            try
            {
                StreamReader sr = new StreamReader(fileName, Encoding.Default);
                htmlContent = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Globe.WriteLog("SpiderAsync.LoadContent: " + ex.Message);
            }

            return htmlContent;
        }

        private Dictionary<int, int> GetOriginalMatchIDs(string directory)
        {
            Dictionary<int, int> originalMatchIDs = new Dictionary<int, int>();
            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] fileInfos = dir.GetFiles();

            if (fileInfos.Length > 0)
            {
                foreach (FileInfo file in fileInfos)
                {
                    if (file.FullName.Contains(LeagueFileName))
                        continue;

                    if (file.Length < IncorrectFileSize)
                    {
                        file.Delete();
                        continue;
                    }

                    int id = int.Parse(Path.GetFileNameWithoutExtension(file.FullName));

                    if (!originalMatchIDs.ContainsKey(id))
                        originalMatchIDs.Add(id, id);
                }
            }

            return originalMatchIDs;
        }
    }
}
