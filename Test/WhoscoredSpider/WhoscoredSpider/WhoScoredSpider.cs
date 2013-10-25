using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WhoscoredSpider
{
    class WhoScoredSpider
    {
        public void GetLiveScores(string league, string leagueName)
        {
            string logDir = Globe.RootDir +  "\\" + leagueName;

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string leagueUrl = Globe.WhoScoredUrl + league;
            string htmlContent = GetHtmlContentByUrl(leagueUrl);

            Globe.WriteLog(logDir + @"\LiveScores.txt", htmlContent);
            /*ContentFilter filter = new ContentFilter();
            htmlContent = filter.GetStandingsInfoFromContent(htmlContent);
            List<LiveScore> liveScoreList = filter.GetLiveScoresFromStandingsInfo(htmlContent, leagueName);

            

            for (int i = 0; i < liveScoreList.Count; i++)
            {
                string rating = filter.GetRatingInfoFromContent(GetMatches(liveScoreList[i].id));
                Globe.WriteLog(liveScoreList[i].id.ToString() + ".txt", rating);
                Globe.WriteLog("scores.txt", "League:" + liveScoreList[i].League + " id:" + liveScoreList[i].id + " Home:" + liveScoreList[i].HomeTeam + " Away:" + liveScoreList[i].AwayTeam + " Score:" + liveScoreList[i].Score);
            }*/
        }

        public string GetMatches(int matchID)
        {
            string matchUrl = Globe.WhoScoredMatchesUrl + matchID + @"/Live";
            string htmlContent = GetHtmlContentByUrl(matchUrl);

            return htmlContent;
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
                Console.WriteLine(ex.Message);
            }

            return htmlContent;
        }
    }
}
