using System;
using System.Collections.Generic;

namespace WhoScoredAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            int homeTeamID = 41;
            int awayTeamID = 134;
            DAL dal = new DAL();
            TeamStatistics_Preview homeTeam = dal.GetTeamStatistics(homeTeamID, true);

            Console.ReadKey();
        }

        private void MatchPreview(int homeTeamID, int awayTeamID)
        {
        }
    }
}
