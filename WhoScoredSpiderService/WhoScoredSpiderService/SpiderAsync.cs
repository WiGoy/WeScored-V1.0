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
        private const string LeagueHtmlContentFileName = "LiveScores.txt";

        public void GetAllLeagues()
        {
            foreach (KeyValuePair<string, string> item in Globe.LeaguesDic)
            {
                GetLeague(item.Key, item.Value);
            }
        }

        public async void GetLeague(string leagueName, string leagueUrl)
        {
            string dir = Globe.RootDir + "\\" + leagueName + "\\";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string url = Globe.WhoScoredUrl + leagueUrl;
            string fileName = dir + LeagueHtmlContentFileName;

            Task<string> getHtmlContentByUrl = GetHtmlContentByUrl(url);
            string htmlContent = await GetHtmlContentByUrl(url);

            SaveContent(fileName, htmlContent);
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
    }
}
