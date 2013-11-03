using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WhoscoredSpider
{
    class WhoscoredSpiderAsync
    {
        public static int count = 0;
        
        public async void GetMatches(string dir, Dictionary<int, string> matchIDs)
        {
            if (matchIDs.Count > 0)
            {
                foreach (KeyValuePair<int, string> match in matchIDs)
                {
                    Task<string> getHtmlContentByUrl = GetHtmlContentByUrl(match.Value);
                    string matchHtmlContent = await GetHtmlContentByUrl(match.Value);

                    string fileName = dir + match.Key + ".txt";

                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    Globe.WriteLog(fileName, matchHtmlContent);
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
                Console.WriteLine(ex.Message);
            }

            return htmlContent;
        }
    }
}
