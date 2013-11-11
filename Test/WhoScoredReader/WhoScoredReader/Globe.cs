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

        public int Blocked;
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
        public int Shot_Post;
        public int Shot_Post_Left;
        public int Shot_Post_Right;
        public int Shot_Post_High;
        public int Shot_Miss_Left;
        public int Shot_Miss_Right;
        public int Shot_Miss_High;
        public int Shot_Miss_HighLeft;
        public int Shot_Miss_HighRight;
        
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

    public class MatchStatisticsFilter
    {
        //  goal
        public const string Goal = "goals";
        public const string OwnGoal = "own_goals";
        public const string Assist = "goal_assist";

        //  pass
        public const string Pass_Total = "total_pass";
        public const string Pass_Accurate = "accurate_pass";
        
        //  shot
        public const string Blocked = "blocked_scoring_att";
        public const string Shot_Total = "total_scoring_att";
        public const string Shot_OnTarget = "ontarget_scoring_att";
        public const string Shot_Goal_LowLeft = "att_goal_low_left";
        public const string Shot_Goal_HighLeft = "att_goal_high_left";
        public const string Shot_Goal_LowRight = "att_goal_low_right";
        public const string Shot_Goal_HighRight = "att_goal_high_right";
        public const string Shot_Goal_LowCenter = "att_goal_low_centre";
        public const string Shot_Goal_HighCenter = "att_goal_high_centre";
        public const string Shot_Save_LowLeft = "att_sv_low_left";
        public const string Shot_Save_HighLeft = "att_sv_high_left";
        public const string Shot_Save_LowRight = "att_sv_low_right";
        public const string Shot_Save_HighRight = "att_sv_high_right";
        public const string Shot_Save_LowCenter = "att_sv_low_centre";
        public const string Shot_Save_HighCenter = "att_sv_high_centre";
        public const string Shot_OffTarget = "shot_off_target";
        public const string Shot_Post = "post_scoring_att";
        public const string Shot_Post_Left = "att_post_left";
        public const string Shot_Post_Right = "att_post_right";
        public const string Shot_Post_High = "att_post_high";
        public const string Shot_Miss_Left = "att_miss_left";
        public const string Shot_Miss_Right = "att_miss_right";
        public const string Shot_Miss_High = "att_miss_high";
        public const string Shot_Miss_HighLeft = "att_miss_high_left";
        public const string Shot_Miss_HighRight = "att_miss_high_right";

        //  penalty
        public const string Penalty_Goal = "att_pen_goal";
        public const string Penalty_Save = "penalty_save";
        public const string Penalty_Concede = "penalty_conceded";
        
        //  save
        public const string Save = "saves";
        
        // dribble
        public const string Dribble = "won_contest";

        // touch
        public const string Touch = "touches";

        //  aerial
        public const string Aerial_Won = "aerial_won";
        public const string Aerial_Lost = "aerial_lost";

        //  other statistics
        public const string Tackle = "total_tackle";
        public const string Offside = "total_offside";
        public const string Throw = "total_throws";
        public const string Corner = "won_corners";

        //  foul
        public const string Foul_Team = "fk_foul_lost";
        public const string Foul_Player = "fouls";
            
        //  possession percentage
        public const string PossessionPercentage = "possession_percentage";

        //  card
        public const string YellowCard = "yellow_card";
        public const string RedCard = "red_card";
    }
    #endregion
}
