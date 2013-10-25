using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WhoscoredSpider
{
    class WhoscoredSpiderAsync
    {
        public static int count = 0;

        public async void GetLiveScores(string league, string leagueName)
        {
            count++;
            string leagueUrl = Globe.WhoScoredUrl + league;
            Task<string> getHtmlContentByUrl = GetHtmlContentByUrl(leagueUrl);
            string htmlContent = await GetHtmlContentByUrl(leagueUrl);

            ContentFilter filter = new ContentFilter();
            htmlContent = filter.GetStandingsInfoFromContent(htmlContent);
            List<LiveScore> liveScoreList = filter.GetLiveScoresFromStandingsInfo(htmlContent, leagueName);

            GetMatches(liveScoreList[1].id);

            for (int i = 0; i < liveScoreList.Count; i++)
            {
                Globe.WriteLog("scores.txt", "League:" + liveScoreList[i].League + " id:" + liveScoreList[i].id + " Home:" + liveScoreList[i].HomeTeam + " Away:" + liveScoreList[i].AwayTeam + " Score:" + liveScoreList[i].Score);
                
            }
            
            count--;
        }

        public async void GetMatches(int matchID)
        {
            string matchUrl = Globe.WhoScoredMatchesUrl + matchID + @"/Live";
            Task<string> getHtmlContentByUrl = GetHtmlContentByUrl(matchUrl);
            string htmlContent = await GetHtmlContentByUrl(matchUrl);
            
            Globe.WriteLog(matchID.ToString() + ".txt", htmlContent);
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
