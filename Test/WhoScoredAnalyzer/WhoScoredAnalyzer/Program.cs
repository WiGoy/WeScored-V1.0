using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WhoScoredAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Versus> matchPreview = new List<Versus>();
            matchPreview.Add(new Versus(560, 63));
            matchPreview.Add(new Versus(44, 276));
            matchPreview.Add(new Versus(130, 65));
            matchPreview.Add(new Versus(13, 249));
            matchPreview.Add(new Versus(847, 37));
            matchPreview.Add(new Versus(36, 32));

            foreach (Versus vs in matchPreview)
            {
                TeamStatistics_Review homeTeam = DAL.GetTeamStatistics(vs.HomeTeamID, true);
                homeTeam.points = DAL.GetTeamPointsHistory(vs.HomeTeamID, true);

                TeamStatistics_Review awayTeam = DAL.GetTeamStatistics(vs.AwayTeamID, false);
                awayTeam.points = DAL.GetTeamPointsHistory(vs.AwayTeamID, false);

                PrintPreview(homeTeam, awayTeam);
            }
            
            Console.ReadKey();
        }

        private static void PrintPreview(TeamStatistics_Review homeTeam, TeamStatistics_Review awayTeam)
        {
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "", homeTeam.name + "(H)", awayTeam.name + "(A)");
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "League", Regex.Match(homeTeam.league, @"^[A-Za-z]+"), Regex.Match(awayTeam.league, @"^[A-Za-z]+"));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Rating", String.Format("{0:F}", homeTeam.rating), String.Format("{0:F}", awayTeam.rating));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "", PrintPointsString(homeTeam.points), PrintPointsString(awayTeam.points));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Possession %", String.Format("{0:N1}", homeTeam.possession_percentage) + " %", String.Format("{0:N1}", awayTeam.possession_percentage) + " %");
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Pass PG", String.Format("{0:F}", homeTeam.total_pass), String.Format("{0:F}", awayTeam.total_pass));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "PassAccuracy %", String.Format("{0:N1}", (homeTeam.accurate_pass / homeTeam.total_pass) * 100) + " %", String.Format("{0:N1}", (awayTeam.accurate_pass / awayTeam.total_pass) * 100) + " %");
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "ForwardPass %", String.Format("{0:N1}", (homeTeam.fwd_pass / homeTeam.total_pass) * 100) + " %", String.Format("{0:N1}", (awayTeam.fwd_pass / awayTeam.total_pass) * 100) + " %");
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "LongPass %", String.Format("{0:N1}", (homeTeam.total_long_balls / homeTeam.total_pass) * 100) + " %", String.Format("{0:N1}", (awayTeam.total_long_balls / awayTeam.total_pass) * 100) + " %");
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Cross PG", String.Format("{0:F}", homeTeam.total_cross), String.Format("{0:F}", awayTeam.total_cross));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "CrossAccuracy %", String.Format("{0:N1}", (homeTeam.accurate_cross / homeTeam.total_cross) * 100) + " %", String.Format("{0:N1}", (awayTeam.accurate_cross / awayTeam.total_cross) * 100) + " %");
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Goals PG", String.Format("{0:F}", homeTeam.goals), String.Format("{0:F}", awayTeam.goals));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "OpenPlay PG", String.Format("{0:F}", homeTeam.goals_openplay), String.Format("{0:F}", awayTeam.goals_openplay));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "GoalsAgainst PG", String.Format("{0:F}", homeTeam.goals_conceded), String.Format("{0:F}", awayTeam.goals_conceded));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Shot PG", String.Format("{0:F}", homeTeam.total_scoring_att), String.Format("{0:F}", awayTeam.total_scoring_att));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "ShotOnTarget PG", String.Format("{0:F}", homeTeam.ontarget_scoring_att), String.Format("{0:F}", awayTeam.ontarget_scoring_att));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Dribbling PG", String.Format("{0:F}", homeTeam.won_contest), String.Format("{0:F}", awayTeam.won_contest));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Interception PG", String.Format("{0:F}", homeTeam.interception_won), String.Format("{0:F}", awayTeam.interception_won));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Tackle PG", String.Format("{0:F}", homeTeam.won_tackle), String.Format("{0:F}", awayTeam.won_tackle));
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Tackle Won %", String.Format("{0:N1}", (homeTeam.won_tackle / homeTeam.total_tackle) * 100) + " %", String.Format("{0:N1}", (awayTeam.won_tackle / awayTeam.total_tackle) * 100) + " %");
            Console.WriteLine("{0, -20}{1, -20}{2, -20}", "Aerial Won %", String.Format("{0:N1}", (homeTeam.aerial_won / (homeTeam.aerial_won + homeTeam.aerial_lost)) * 100) + " %", String.Format("{0:N1}", (awayTeam.aerial_won / (awayTeam.aerial_won + awayTeam.aerial_lost)) * 100) + " %");
            Console.WriteLine();
        }

        private static string PrintPointsString(List<int> points)
        {
            string pointsString = "";

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == 3)
                {
                    pointsString = pointsString.Insert(0, "W ");
                }
                else if (points[i] == 1)
                {
                    pointsString = pointsString.Insert(0, "D ");
                }
                else
                {
                    pointsString = pointsString.Insert(0, "L ");
                }
            }

            return pointsString.Trim();
        }
    }
}
