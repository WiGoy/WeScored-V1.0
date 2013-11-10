using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhoScoredReader
{
    class Globe
    {
        public static string RootDir = @"E:\GitHubWorkspace\WhoScored\Test\";
    }

    #region 数据结构
    public class MatchInfo
    {
        public int MatchID;
        public string StartTime;
        public TeamStatistics HomeTeamStatistics;
        public TeamStatistics AwayTeamStatistics;
        public List<PlayerStatistics> HomeTeamPlayerStatistics;
        public List<PlayerStatistics> AwayTeamPlayerStatistics;
    }

    public class TeamStatistics
    {
        public int ID;
        public string Name;
        public float Rating;
        
        public int Goal_FirstHalf;
        public int Goal_SecondHalf;
        public int Goal_Penalty;

        public int Pass_Total;
        public int Pass_Accurate;
        
        public int Shot_Total;
        public int Shot_OnTarget;
        public int Shot_Goal_LowLeft;
        public int Shot_Goal_HighLeft;
        public int Shot_Goal_LowRight;
        public int Shot_Goal_HighRight;
        public int Shot_Goal_LowCenter;
        public int Shot_Goal_HighCenter;
        public int Shot_Save_LowLeft;
        public int Shot_Save_HighLeft;
        public int Shot_Save_LowRight;
        public int Shot_Save_HighRight;
        public int Shot_Save_LowCenter;
        public int Shot_Save_HighCenter;
        public int Shot_OffTarget;
        public int Shot_Post_Left;
        public int Shot_Post_Right;
        public int Shot_Post_High;
        public int Shot_Miss_Left;
        public int Shot_Miss_Right;
        public int Shot_Miss_High;
        
        public int Blocked;
        public int PenaltySave;

        public int Dribble;

        public int Aerial_Won;
        public int Aerial_Lost;

        public int Tackle;
        public int Offside;
        public int Throw;
        public int Corner;
        public int Foul;

        public float PossessionPercentage;
    }

    public class PlayerStatistics
    {
        public int PlayerID;
        public string PlayerName;
        public float Rating;
    }
    #endregion
}
