using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WhoScoredSpiderService
{
    class SpiderSync
    {
        private const string LeagueHtmlContentFileName = "LiveScores.txt";

        public void GetLeagueHtmlContent(string leagueName, string leagueUrl)
        {
            string dir = Globe.RootDir +  "\\" + leagueName + "\\";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string url = Globe.WhoScoredUrl + leagueUrl;
            string htmlContent = GetHtmlContentByUrl(url);
            string fileName = dir + LeagueHtmlContentFileName;

            SaveContent(fileName, htmlContent);
        }

        private string GetHtmlContentByUrl(string url)
        {
            string htmlContent = "";

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.UseDefaultCredentials = false;

                WebResponse response = request.GetResponse();
                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream, System.Text.Encoding.Default);

                htmlContent = sr.ReadToEnd();
                
                resStream.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
                Globe.WriteLog("SpiderSync.GetHtmlContentByUrl: " + ex.Message);
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
                Globe.WriteLog("SpiderSync.SaveContent: " + ex.Message);
            }
        }
    }
}
