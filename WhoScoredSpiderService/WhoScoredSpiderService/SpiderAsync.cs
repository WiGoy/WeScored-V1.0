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
            string fileName = dir + Globe.LeagueFileName;

            if (File.Exists(fileName))
                File.Delete(fileName);

            Globe.WriteLog("Downloading from url: " + url);
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
                    string fileName = directory + Globe.LeagueFileName;
                    string htmlContent = Globe.LoadFile(fileName);

                    List<int> originalMatchIDs = GetOriginalMatchIDs(directory);
                    List<int> matchIDs = filter.GetMatchIDs(htmlContent, originalMatchIDs);
                    GetMatches(item.Key, matchIDs);
                }
            }
        }

        public async void GetMatches(string leagueName, List<int> matchIDs)
        {
            if (matchIDs.Count > 0)
            {
                string dir = Globe.RootDir + "\\" + leagueName + "\\";

                foreach (int id in matchIDs)
                {
                    string url = Globe.WhoScoredMatchesUrl + id + @"/LiveStatistics";

                    Globe.WriteLog("Downloading from url: " + url);
                    Task<string> getHtmlContentByUrl = GetHtmlContentByUrl(url);
                    string matchHtmlContent = await GetHtmlContentByUrl(url);

                    string fileName = dir + id + ".txt";
                    SaveContent(fileName, matchHtmlContent);
                }
            }
        }

        private async Task<string> GetHtmlContentByUrl(string url)
        {
            string htmlContent = "";

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

        private List<int> GetOriginalMatchIDs(string directory)
        {
            List<int> originalMatchIDs = new List<int>();
            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] fileInfos = dir.GetFiles();

            if (fileInfos.Length > 0)
            {
                foreach (FileInfo file in fileInfos)
                {
                    if (file.FullName.Contains(Globe.LeagueFileName))
                        continue;

                    if (file.Length < Globe.IncorrectFileSize)
                    {
                        file.Delete();
                        continue;
                    }

                    int id = int.Parse(Path.GetFileNameWithoutExtension(file.FullName));

                    if (!originalMatchIDs.Contains(id))
                        originalMatchIDs.Add(id);
                }
            }

            return originalMatchIDs;
        }
    }
}
