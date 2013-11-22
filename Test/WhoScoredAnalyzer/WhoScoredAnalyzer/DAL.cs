using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WhoScoredAnalyzer
{
    public class DAL
    {
        public List<int> GetOriginalMatchIDs()
        {
            List<int> originalMatchIDs = new List<int>();
            string strsql = "select match_id From MatchInformation";
            DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, strsql, null);
            originalMatchIDs = (from r in ds.Tables[0].AsEnumerable() select r.Field<int>("match_id")).ToList();

            return originalMatchIDs;
        }

        public void GetPlayerStatistics()
        {
            string field = "rating";
            string strsql = @"SELECT player_id, player_name, team_name, avg(" + field + @"), count(*) FROM `PlayerStatistics` WHERE (((league = 'Germany_Bundesliga') or (league = 'England_BarclaysPL') or (league = 'Spain_LigaBBVA') or (league = 'Italy_SerieA') or (league = 'France_Ligue1')) and (position = 'DL')) group by player_id ORDER BY avg(" + field + @") ASC";
            DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, strsql, null);
            
            var queryInfo = ds.Tables[0].AsEnumerable().Select(player => new
            {
                id = player.Field<int>("player_id"),
                name = player.Field<string>("player_name"),
                team = player.Field<string>("team_name"),
                value = player.Field<double>("avg(rating)"),
                count = player.Field<long>("count(*)")
            });
            
            foreach (var productInfo in queryInfo)
            {
                if (productInfo.count >= 4)
                Console.WriteLine("Player: {0} Club: {1} " + field + ": {2} ", productInfo.name, productInfo.team, productInfo.value);
            }
        }

        public TeamStatistics_Preview GetTeamStatistics(int teamID)
        {
            TeamStatistics_Preview team = new TeamStatistics_Preview();

            string strsql = @"SELECT team_id, team_name, league, avg(rating), "
                + @"avg(total_pass), avg(accurate_pass), "
                + @"avg(total_fwd_zone_pass), avg(accurate_fwd_zone_pass), avg(total_back_zone_pass), avg(accurate_back_zone_pass), "
                + @"avg(total_long_balls), avg(accurate_long_balls), avg(long_pass_own_to_opp), avg(long_pass_own_to_opp_success), "
                + @"avg(total_cross), avg(accurate_cross), "
                + @"avg(fwd_pass), avg(backward_pass), avg(leftside_pass), avg(rightside_pass), avg(passes_left), avg(passes_right), "
                + @"avg(goals), avg(first_half_goals), avg(att_ibox_goal), avg(att_obox_goal), avg(att_one_on_one), "
                + @"avg(forward_goals), avg(midfielder_goals), avg(defender_goals), "
                + @"avg(att_setpiece), avg(att_openplay), avg(goals_openplay), "
                + @"avg(total_att_assist), avg(goal_assist), avg(att_assist_setplay), avg(goal_assist_setplay), avg(att_assist_openplay), avg(goal_assist_openplay), "
                + @"avg(total_scoring_att), avg(ontarget_scoring_att), avg(shot_off_target), "
                + @"avg(big_chance_created), avg(big_chance_scored), avg(big_chance_missed), "
                + @"avg(total_fastbreak), avg(att_fastbreak), avg(shot_fastbreak), avg(goal_fastbreak), "
                + @"avg(total_offside), "
                + @"avg(goals_conceded), avg(goals_conceded_ibox), avg(goals_conceded_obox), "
                + @"avg(saves), avg(diving_save), "
                + @"avg(interception), avg(interception_won), avg(interceptions_in_box), "
                + @"avg(total_tackle), avg(won_tackle), avg(tackle_lost), "
                + @"avg(ball_recovery), avg(total_clearance), avg(clean_sheet), avg(total_yel_card), avg(total_red_card), "
                + @"avg(possession_percentage), avg(touches), avg(unsuccessful_touch), "
                + @"avg(total_contest), avg(won_contest), avg(dribble_lost), "
                + @"avg(fifty_fifty), avg(successful_fifty_fifty), avg(aerial_lost), avg(aerial_won) "
                + @"FROM `TeamStatistics` WHERE (team_id = " + teamID + @")";
            
            DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, strsql, null);

            #region 用Linq从DataSet中读出数据
            var teamStatistics = ds.Tables[0].AsEnumerable().Select(
                result => new
                {
                    id = result.Field<int>("team_id"),
                    name = result.Field<string>("team_name"),
                    league = result.Field<string>("league"),
                    rating = result.Field<double>("avg(rating)"),
                    total_pass = result.Field<double>("avg(total_pass)"),
                    accurate_pass = result.Field<double>("avg(accurate_pass)"),
                    total_fwd_zone_pass = result.Field<double>("avg(total_fwd_zone_pass)"),
                    accurate_fwd_zone_pass = result.Field<double>("avg(accurate_fwd_zone_pass)"),
                    total_back_zone_pass = result.Field<double>("avg(total_back_zone_pass)"),
                    accurate_back_zone_pass = result.Field<double>("avg(accurate_back_zone_pass)"),
                    total_long_balls = result.Field<double>("avg(total_long_balls)"),
                    accurate_long_balls = result.Field<double>("avg(accurate_long_balls)"),
                    long_pass_own_to_opp = result.Field<double>("avg(long_pass_own_to_opp)"),
                    long_pass_own_to_opp_success = result.Field<double>("avg(long_pass_own_to_opp_success)"),
                    total_cross = result.Field<double>("avg(total_cross)"),
                    accurate_cross = result.Field<double>("avg(accurate_cross)"),
                    fwd_pass = result.Field<double>("avg(fwd_pass)"),
                    backward_pass = result.Field<double>("avg(backward_pass)"),
                    leftside_pass = result.Field<double>("avg(leftside_pass)"),
                    rightside_pass = result.Field<double>("avg(rightside_pass)"),
                    passes_left = result.Field<double>("avg(passes_left)"),
                    passes_right = result.Field<double>("avg(passes_right)"),
                    goals = result.Field<double>("avg(goals)"),
                    first_half_goals = result.Field<double>("avg(first_half_goals)"),
                    att_ibox_goal = result.Field<double>("avg(att_ibox_goal)"),
                    att_obox_goal = result.Field<double>("avg(att_obox_goal)"),
                    att_one_on_one = result.Field<double>("avg(att_one_on_one)"),
                    forward_goals = result.Field<double>("avg(forward_goals)"),
                    midfielder_goals = result.Field<double>("avg(midfielder_goals)"),
                    defender_goals = result.Field<double>("avg(defender_goals)"),
                    att_setpiece = result.Field<double>("avg(att_setpiece)"),
                    att_openplay = result.Field<double>("avg(att_openplay)"),
                    goals_openplay = result.Field<double>("avg(goals_openplay)"),
                    total_att_assist = result.Field<double>("avg(total_att_assist)"),
                    goal_assist = result.Field<double>("avg(goal_assist)"),
                    att_assist_setplay = result.Field<double>("avg(att_assist_setplay)"),
                    goal_assist_setplay = result.Field<double>("avg(goal_assist_setplay)"),
                    att_assist_openplay = result.Field<double>("avg(att_assist_openplay)"),
                    goal_assist_openplay = result.Field<double>("avg(goal_assist_openplay)"),
                    total_scoring_att = result.Field<double>("avg(total_scoring_att)"),
                    ontarget_scoring_att = result.Field<double>("avg(ontarget_scoring_att)"),
                    shot_off_target = result.Field<double>("avg(shot_off_target)"),
                    big_chance_created = result.Field<double>("avg(big_chance_created)"),
                    big_chance_scored = result.Field<double>("avg(big_chance_scored)"),
                    big_chance_missed = result.Field<double>("avg(big_chance_missed)"),
                    total_fastbreak = result.Field<double>("avg(total_fastbreak)"),
                    att_fastbreak = result.Field<double>("avg(att_fastbreak)"),
                    shot_fastbreak = result.Field<double>("avg(shot_fastbreak)"),
                    goal_fastbreak = result.Field<double>("avg(goal_fastbreak)"),
                    total_offside = result.Field<double>("avg(total_offside)"),
                    goals_conceded = result.Field<double>("avg(goals_conceded)"),
                    goals_conceded_ibox = result.Field<double>("avg(goals_conceded_ibox)"),
                    goals_conceded_obox = result.Field<double>("avg(goals_conceded_obox)"),
                    saves = result.Field<double>("avg(saves)"),
                    diving_save = result.Field<double>("avg(diving_save)"),
                    interception = result.Field<double>("avg(interception)"),
                    interception_won = result.Field<double>("avg(interception_won)"),
                    interceptions_in_box = result.Field<double>("avg(interceptions_in_box)"),
                    total_tackle = result.Field<double>("avg(total_tackle)"),
                    won_tackle = result.Field<double>("avg(won_tackle)"),
                    tackle_lost = result.Field<double>("avg(tackle_lost)"),
                    ball_recovery = result.Field<double>("avg(ball_recovery)"),
                    total_clearance = result.Field<double>("avg(total_clearance)"),
                    clean_sheet = result.Field<double>("avg(clean_sheet)"),
                    total_yel_card = result.Field<double>("avg(total_yel_card)"),
                    total_red_card = result.Field<double>("avg(total_red_card)"),
                    possession_percentage = result.Field<double>("avg(possession_percentage)"),
                    touches = result.Field<double>("avg(touches)"),
                    unsuccessful_touch = result.Field<double>("avg(unsuccessful_touch)"),
                    total_contest = result.Field<double>("avg(total_contest)"),
                    won_contest = result.Field<double>("avg(won_contest)"),
                    dribble_lost = result.Field<double>("avg(dribble_lost)"),
                    fifty_fifty = result.Field<double>("avg(fifty_fifty)"),
                    successful_fifty_fifty = result.Field<double>("avg(successful_fifty_fifty)"),
                    aerial_lost = result.Field<double>("avg(aerial_lost)"),
                    aerial_won = result.Field<double>("avg(aerial_won)")
                }
            );
            #endregion

            #region 用var数据生成TeamStatistics_Preview
            foreach (var teamSta in teamStatistics)
            {
                team.id = teamSta.id;
                team.name = teamSta.name;
                team.league = teamSta.league;
                team.rating = teamSta.rating;
                
                team.total_pass = teamSta.total_pass;
                team.accurate_pass = teamSta.accurate_pass;
                team.total_fwd_zone_pass = teamSta.total_fwd_zone_pass;
                team.accurate_fwd_zone_pass = teamSta.accurate_fwd_zone_pass;
                team.total_back_zone_pass = teamSta.total_back_zone_pass;
                team.accurate_back_zone_pass = teamSta.accurate_back_zone_pass;
                team.total_long_balls = teamSta.total_long_balls;
                team.accurate_long_balls = teamSta.accurate_long_balls;
                team.long_pass_own_to_opp = teamSta.long_pass_own_to_opp;
                team.long_pass_own_to_opp_success = teamSta.long_pass_own_to_opp_success;
                team.total_cross = teamSta.total_cross;
                team.accurate_cross = teamSta.accurate_cross;
                team.fwd_pass = teamSta.fwd_pass;
                team.backward_pass = teamSta.backward_pass;
                team.leftside_pass = teamSta.leftside_pass;
                team.rightside_pass = teamSta.rightside_pass;
                team.passes_left = teamSta.passes_left;
                team.passes_right = teamSta.passes_right;

                team.goals = teamSta.goals;
                team.first_half_goals = teamSta.first_half_goals;
                team.att_ibox_goal = teamSta.att_ibox_goal;
                team.att_obox_goal = teamSta.att_obox_goal;
                team.att_one_on_one = teamSta.att_one_on_one;
                team.forward_goals = teamSta.forward_goals;
                team.midfielder_goals = teamSta.midfielder_goals;
                team.defender_goals = teamSta.defender_goals;
                team.att_setpiece = teamSta.att_setpiece;
                team.att_openplay = teamSta.att_openplay;
                team.goals_openplay = teamSta.goals_openplay;
                team.total_att_assist = teamSta.total_att_assist;
                team.goal_assist = teamSta.goal_assist;
                team.att_assist_setplay = teamSta.att_assist_setplay;
                team.goal_assist_setplay = teamSta.goal_assist_setplay;
                team.att_assist_openplay = teamSta.att_assist_openplay;
                team.goal_assist_openplay = teamSta.goal_assist_openplay;
                team.total_scoring_att = teamSta.total_scoring_att;
                team.ontarget_scoring_att = teamSta.ontarget_scoring_att;
                team.shot_off_target = teamSta.shot_off_target;
                team.big_chance_created = teamSta.big_chance_created;
                team.big_chance_scored = teamSta.big_chance_scored;
                team.big_chance_missed = teamSta.big_chance_missed;
                team.total_fastbreak = teamSta.total_fastbreak;
                team.att_fastbreak = teamSta.att_fastbreak;
                team.shot_fastbreak = teamSta.shot_fastbreak;
                team.goal_fastbreak = teamSta.goal_fastbreak;
                team.total_offside = teamSta.total_offside;

                team.goals_conceded = teamSta.goals_conceded;
                team.goals_conceded_ibox = teamSta.goals_conceded_ibox;
                team.goals_conceded_obox = teamSta.goals_conceded_obox;
                team.saves = teamSta.saves;
                team.diving_save = teamSta.diving_save;
                team.interception = teamSta.interception;
                team.interception_won = teamSta.interception_won;
                team.interceptions_in_box = teamSta.interceptions_in_box;
                team.total_tackle = teamSta.total_tackle;
                team.won_tackle = teamSta.won_tackle;
                team.tackle_lost = teamSta.tackle_lost;
                team.ball_recovery = teamSta.ball_recovery;
                team.total_clearance = teamSta.total_clearance;
                team.clean_sheet = teamSta.clean_sheet;
                team.total_yel_card = teamSta.total_yel_card;
                team.total_red_card = teamSta.total_red_card;

                team.possession_percentage = teamSta.possession_percentage;
                team.touches = teamSta.touches;
                team.unsuccessful_touch = teamSta.unsuccessful_touch;
                team.total_contest = teamSta.total_contest;
                team.won_contest = teamSta.won_contest;
                team.dribble_lost = teamSta.dribble_lost;
                team.fifty_fifty = teamSta.fifty_fifty;
                team.successful_fifty_fifty = teamSta.successful_fifty_fifty;
                team.aerial_lost = teamSta.aerial_lost;
                team.aerial_won = teamSta.aerial_won;
            }
            #endregion

            return team;
        }

        public TeamStatistics_Preview GetTeamStatistics(int teamID, int matchCount)
        {
            TeamStatistics_Preview team = new TeamStatistics_Preview();

            string strsql = @"SELECT team_id, team_name, league, avg(rating), "
                + @"avg(total_pass), avg(accurate_pass), "
                + @"avg(total_fwd_zone_pass), avg(accurate_fwd_zone_pass), avg(total_back_zone_pass), avg(accurate_back_zone_pass), "
                + @"avg(total_long_balls), avg(accurate_long_balls), avg(long_pass_own_to_opp), avg(long_pass_own_to_opp_success), "
                + @"avg(total_cross), avg(accurate_cross), "
                + @"avg(fwd_pass), avg(backward_pass), avg(leftside_pass), avg(rightside_pass), avg(passes_left), avg(passes_right), "
                + @"avg(goals), avg(first_half_goals), avg(att_ibox_goal), avg(att_obox_goal), avg(att_one_on_one), "
                + @"avg(forward_goals), avg(midfielder_goals), avg(defender_goals), "
                + @"avg(att_setpiece), avg(att_openplay), avg(goals_openplay), "
                + @"avg(total_att_assist), avg(goal_assist), avg(att_assist_setplay), avg(goal_assist_setplay), avg(att_assist_openplay), avg(goal_assist_openplay), "
                + @"avg(total_scoring_att), avg(ontarget_scoring_att), avg(shot_off_target), "
                + @"avg(big_chance_created), avg(big_chance_scored), avg(big_chance_missed), "
                + @"avg(total_fastbreak), avg(att_fastbreak), avg(shot_fastbreak), avg(goal_fastbreak), "
                + @"avg(total_offside), "
                + @"avg(goals_conceded), avg(goals_conceded_ibox), avg(goals_conceded_obox), "
                + @"avg(saves), avg(diving_save), "
                + @"avg(interception), avg(interception_won), avg(interceptions_in_box), "
                + @"avg(total_tackle), avg(won_tackle), avg(tackle_lost), "
                + @"avg(ball_recovery), avg(total_clearance), avg(clean_sheet), avg(total_yel_card), avg(total_red_card), "
                + @"avg(possession_percentage), avg(touches), avg(unsuccessful_touch), "
                + @"avg(total_contest), avg(won_contest), avg(dribble_lost), "
                + @"avg(fifty_fifty), avg(successful_fifty_fifty), avg(aerial_lost), avg(aerial_won) "
                + @"FROM `TeamStatistics` WHERE (team_id = " + teamID + @") ORDER BY id DESC LIMIT 0, " + matchCount;

            DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, strsql, null);

            #region 用Linq从DataSet中读出数据
            var teamStatistics = ds.Tables[0].AsEnumerable().Select(
                result => new
                {
                    id = result.Field<int>("team_id"),
                    name = result.Field<string>("team_name"),
                    league = result.Field<string>("league"),
                    rating = result.Field<double>("avg(rating)"),
                    total_pass = result.Field<double>("avg(total_pass)"),
                    accurate_pass = result.Field<double>("avg(accurate_pass)"),
                    total_fwd_zone_pass = result.Field<double>("avg(total_fwd_zone_pass)"),
                    accurate_fwd_zone_pass = result.Field<double>("avg(accurate_fwd_zone_pass)"),
                    total_back_zone_pass = result.Field<double>("avg(total_back_zone_pass)"),
                    accurate_back_zone_pass = result.Field<double>("avg(accurate_back_zone_pass)"),
                    total_long_balls = result.Field<double>("avg(total_long_balls)"),
                    accurate_long_balls = result.Field<double>("avg(accurate_long_balls)"),
                    long_pass_own_to_opp = result.Field<double>("avg(long_pass_own_to_opp)"),
                    long_pass_own_to_opp_success = result.Field<double>("avg(long_pass_own_to_opp_success)"),
                    total_cross = result.Field<double>("avg(total_cross)"),
                    accurate_cross = result.Field<double>("avg(accurate_cross)"),
                    fwd_pass = result.Field<double>("avg(fwd_pass)"),
                    backward_pass = result.Field<double>("avg(backward_pass)"),
                    leftside_pass = result.Field<double>("avg(leftside_pass)"),
                    rightside_pass = result.Field<double>("avg(rightside_pass)"),
                    passes_left = result.Field<double>("avg(passes_left)"),
                    passes_right = result.Field<double>("avg(passes_right)"),
                    goals = result.Field<double>("avg(goals)"),
                    first_half_goals = result.Field<double>("avg(first_half_goals)"),
                    att_ibox_goal = result.Field<double>("avg(att_ibox_goal)"),
                    att_obox_goal = result.Field<double>("avg(att_obox_goal)"),
                    att_one_on_one = result.Field<double>("avg(att_one_on_one)"),
                    forward_goals = result.Field<double>("avg(forward_goals)"),
                    midfielder_goals = result.Field<double>("avg(midfielder_goals)"),
                    defender_goals = result.Field<double>("avg(defender_goals)"),
                    att_setpiece = result.Field<double>("avg(att_setpiece)"),
                    att_openplay = result.Field<double>("avg(att_openplay)"),
                    goals_openplay = result.Field<double>("avg(goals_openplay)"),
                    total_att_assist = result.Field<double>("avg(total_att_assist)"),
                    goal_assist = result.Field<double>("avg(goal_assist)"),
                    att_assist_setplay = result.Field<double>("avg(att_assist_setplay)"),
                    goal_assist_setplay = result.Field<double>("avg(goal_assist_setplay)"),
                    att_assist_openplay = result.Field<double>("avg(att_assist_openplay)"),
                    goal_assist_openplay = result.Field<double>("avg(goal_assist_openplay)"),
                    total_scoring_att = result.Field<double>("avg(total_scoring_att)"),
                    ontarget_scoring_att = result.Field<double>("avg(ontarget_scoring_att)"),
                    shot_off_target = result.Field<double>("avg(shot_off_target)"),
                    big_chance_created = result.Field<double>("avg(big_chance_created)"),
                    big_chance_scored = result.Field<double>("avg(big_chance_scored)"),
                    big_chance_missed = result.Field<double>("avg(big_chance_missed)"),
                    total_fastbreak = result.Field<double>("avg(total_fastbreak)"),
                    att_fastbreak = result.Field<double>("avg(att_fastbreak)"),
                    shot_fastbreak = result.Field<double>("avg(shot_fastbreak)"),
                    goal_fastbreak = result.Field<double>("avg(goal_fastbreak)"),
                    total_offside = result.Field<double>("avg(total_offside)"),
                    goals_conceded = result.Field<double>("avg(goals_conceded)"),
                    goals_conceded_ibox = result.Field<double>("avg(goals_conceded_ibox)"),
                    goals_conceded_obox = result.Field<double>("avg(goals_conceded_obox)"),
                    saves = result.Field<double>("avg(saves)"),
                    diving_save = result.Field<double>("avg(diving_save)"),
                    interception = result.Field<double>("avg(interception)"),
                    interception_won = result.Field<double>("avg(interception_won)"),
                    interceptions_in_box = result.Field<double>("avg(interceptions_in_box)"),
                    total_tackle = result.Field<double>("avg(total_tackle)"),
                    won_tackle = result.Field<double>("avg(won_tackle)"),
                    tackle_lost = result.Field<double>("avg(tackle_lost)"),
                    ball_recovery = result.Field<double>("avg(ball_recovery)"),
                    total_clearance = result.Field<double>("avg(total_clearance)"),
                    clean_sheet = result.Field<double>("avg(clean_sheet)"),
                    total_yel_card = result.Field<double>("avg(total_yel_card)"),
                    total_red_card = result.Field<double>("avg(total_red_card)"),
                    possession_percentage = result.Field<double>("avg(possession_percentage)"),
                    touches = result.Field<double>("avg(touches)"),
                    unsuccessful_touch = result.Field<double>("avg(unsuccessful_touch)"),
                    total_contest = result.Field<double>("avg(total_contest)"),
                    won_contest = result.Field<double>("avg(won_contest)"),
                    dribble_lost = result.Field<double>("avg(dribble_lost)"),
                    fifty_fifty = result.Field<double>("avg(fifty_fifty)"),
                    successful_fifty_fifty = result.Field<double>("avg(successful_fifty_fifty)"),
                    aerial_lost = result.Field<double>("avg(aerial_lost)"),
                    aerial_won = result.Field<double>("avg(aerial_won)")
                }
            );
            #endregion

            #region 用var数据生成TeamStatistics_Preview
            foreach (var teamSta in teamStatistics)
            {
                team.id = teamSta.id;
                team.name = teamSta.name;
                team.league = teamSta.league;
                team.rating = teamSta.rating;

                team.total_pass = teamSta.total_pass;
                team.accurate_pass = teamSta.accurate_pass;
                team.total_fwd_zone_pass = teamSta.total_fwd_zone_pass;
                team.accurate_fwd_zone_pass = teamSta.accurate_fwd_zone_pass;
                team.total_back_zone_pass = teamSta.total_back_zone_pass;
                team.accurate_back_zone_pass = teamSta.accurate_back_zone_pass;
                team.total_long_balls = teamSta.total_long_balls;
                team.accurate_long_balls = teamSta.accurate_long_balls;
                team.long_pass_own_to_opp = teamSta.long_pass_own_to_opp;
                team.long_pass_own_to_opp_success = teamSta.long_pass_own_to_opp_success;
                team.total_cross = teamSta.total_cross;
                team.accurate_cross = teamSta.accurate_cross;
                team.fwd_pass = teamSta.fwd_pass;
                team.backward_pass = teamSta.backward_pass;
                team.leftside_pass = teamSta.leftside_pass;
                team.rightside_pass = teamSta.rightside_pass;
                team.passes_left = teamSta.passes_left;
                team.passes_right = teamSta.passes_right;

                team.goals = teamSta.goals;
                team.first_half_goals = teamSta.first_half_goals;
                team.att_ibox_goal = teamSta.att_ibox_goal;
                team.att_obox_goal = teamSta.att_obox_goal;
                team.att_one_on_one = teamSta.att_one_on_one;
                team.forward_goals = teamSta.forward_goals;
                team.midfielder_goals = teamSta.midfielder_goals;
                team.defender_goals = teamSta.defender_goals;
                team.att_setpiece = teamSta.att_setpiece;
                team.att_openplay = teamSta.att_openplay;
                team.goals_openplay = teamSta.goals_openplay;
                team.total_att_assist = teamSta.total_att_assist;
                team.goal_assist = teamSta.goal_assist;
                team.att_assist_setplay = teamSta.att_assist_setplay;
                team.goal_assist_setplay = teamSta.goal_assist_setplay;
                team.att_assist_openplay = teamSta.att_assist_openplay;
                team.goal_assist_openplay = teamSta.goal_assist_openplay;
                team.total_scoring_att = teamSta.total_scoring_att;
                team.ontarget_scoring_att = teamSta.ontarget_scoring_att;
                team.shot_off_target = teamSta.shot_off_target;
                team.big_chance_created = teamSta.big_chance_created;
                team.big_chance_scored = teamSta.big_chance_scored;
                team.big_chance_missed = teamSta.big_chance_missed;
                team.total_fastbreak = teamSta.total_fastbreak;
                team.att_fastbreak = teamSta.att_fastbreak;
                team.shot_fastbreak = teamSta.shot_fastbreak;
                team.goal_fastbreak = teamSta.goal_fastbreak;
                team.total_offside = teamSta.total_offside;

                team.goals_conceded = teamSta.goals_conceded;
                team.goals_conceded_ibox = teamSta.goals_conceded_ibox;
                team.goals_conceded_obox = teamSta.goals_conceded_obox;
                team.saves = teamSta.saves;
                team.diving_save = teamSta.diving_save;
                team.interception = teamSta.interception;
                team.interception_won = teamSta.interception_won;
                team.interceptions_in_box = teamSta.interceptions_in_box;
                team.total_tackle = teamSta.total_tackle;
                team.won_tackle = teamSta.won_tackle;
                team.tackle_lost = teamSta.tackle_lost;
                team.ball_recovery = teamSta.ball_recovery;
                team.total_clearance = teamSta.total_clearance;
                team.clean_sheet = teamSta.clean_sheet;
                team.total_yel_card = teamSta.total_yel_card;
                team.total_red_card = teamSta.total_red_card;

                team.possession_percentage = teamSta.possession_percentage;
                team.touches = teamSta.touches;
                team.unsuccessful_touch = teamSta.unsuccessful_touch;
                team.total_contest = teamSta.total_contest;
                team.won_contest = teamSta.won_contest;
                team.dribble_lost = teamSta.dribble_lost;
                team.fifty_fifty = teamSta.fifty_fifty;
                team.successful_fifty_fifty = teamSta.successful_fifty_fifty;
                team.aerial_lost = teamSta.aerial_lost;
                team.aerial_won = teamSta.aerial_won;
            }
            #endregion

            return team;
        }

        public TeamStatistics_Preview GetTeamStatistics(int teamID, bool home)
        {
            TeamStatistics_Preview team = new TeamStatistics_Preview();

            string strsql = @"SELECT team_id, team_name, league, avg(rating), "
                + @"avg(total_pass), avg(accurate_pass), "
                + @"avg(total_fwd_zone_pass), avg(accurate_fwd_zone_pass), avg(total_back_zone_pass), avg(accurate_back_zone_pass), "
                + @"avg(total_long_balls), avg(accurate_long_balls), avg(long_pass_own_to_opp), avg(long_pass_own_to_opp_success), "
                + @"avg(total_cross), avg(accurate_cross), "
                + @"avg(fwd_pass), avg(backward_pass), avg(leftside_pass), avg(rightside_pass), avg(passes_left), avg(passes_right), "
                + @"avg(goals), avg(first_half_goals), avg(att_ibox_goal), avg(att_obox_goal), avg(att_one_on_one), "
                + @"avg(forward_goals), avg(midfielder_goals), avg(defender_goals), "
                + @"avg(att_setpiece), avg(att_openplay), avg(goals_openplay), "
                + @"avg(total_att_assist), avg(goal_assist), avg(att_assist_setplay), avg(goal_assist_setplay), avg(att_assist_openplay), avg(goal_assist_openplay), "
                + @"avg(total_scoring_att), avg(ontarget_scoring_att), avg(shot_off_target), "
                + @"avg(big_chance_created), avg(big_chance_scored), avg(big_chance_missed), "
                + @"avg(total_fastbreak), avg(att_fastbreak), avg(shot_fastbreak), avg(goal_fastbreak), "
                + @"avg(total_offside), "
                + @"avg(goals_conceded), avg(goals_conceded_ibox), avg(goals_conceded_obox), "
                + @"avg(saves), avg(diving_save), "
                + @"avg(interception), avg(interception_won), avg(interceptions_in_box), "
                + @"avg(total_tackle), avg(won_tackle), avg(tackle_lost), "
                + @"avg(ball_recovery), avg(total_clearance), avg(clean_sheet), avg(total_yel_card), avg(total_red_card), "
                + @"avg(possession_percentage), avg(touches), avg(unsuccessful_touch), "
                + @"avg(total_contest), avg(won_contest), avg(dribble_lost), "
                + @"avg(fifty_fifty), avg(successful_fifty_fifty), avg(aerial_lost), avg(aerial_won) "
                + @"FROM `TeamStatistics` WHERE ((team_id = " + teamID + @") and (home = " + home + "))";

            DataSet ds = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, strsql, null);

            #region 用Linq从DataSet中读出数据
            var teamStatistics = ds.Tables[0].AsEnumerable().Select(
                result => new
                {
                    id = result.Field<int>("team_id"),
                    name = result.Field<string>("team_name"),
                    league = result.Field<string>("league"),
                    rating = result.Field<double>("avg(rating)"),
                    total_pass = result.Field<double>("avg(total_pass)"),
                    accurate_pass = result.Field<double>("avg(accurate_pass)"),
                    total_fwd_zone_pass = result.Field<double>("avg(total_fwd_zone_pass)"),
                    accurate_fwd_zone_pass = result.Field<double>("avg(accurate_fwd_zone_pass)"),
                    total_back_zone_pass = result.Field<double>("avg(total_back_zone_pass)"),
                    accurate_back_zone_pass = result.Field<double>("avg(accurate_back_zone_pass)"),
                    total_long_balls = result.Field<double>("avg(total_long_balls)"),
                    accurate_long_balls = result.Field<double>("avg(accurate_long_balls)"),
                    long_pass_own_to_opp = result.Field<double>("avg(long_pass_own_to_opp)"),
                    long_pass_own_to_opp_success = result.Field<double>("avg(long_pass_own_to_opp_success)"),
                    total_cross = result.Field<double>("avg(total_cross)"),
                    accurate_cross = result.Field<double>("avg(accurate_cross)"),
                    fwd_pass = result.Field<double>("avg(fwd_pass)"),
                    backward_pass = result.Field<double>("avg(backward_pass)"),
                    leftside_pass = result.Field<double>("avg(leftside_pass)"),
                    rightside_pass = result.Field<double>("avg(rightside_pass)"),
                    passes_left = result.Field<double>("avg(passes_left)"),
                    passes_right = result.Field<double>("avg(passes_right)"),
                    goals = result.Field<double>("avg(goals)"),
                    first_half_goals = result.Field<double>("avg(first_half_goals)"),
                    att_ibox_goal = result.Field<double>("avg(att_ibox_goal)"),
                    att_obox_goal = result.Field<double>("avg(att_obox_goal)"),
                    att_one_on_one = result.Field<double>("avg(att_one_on_one)"),
                    forward_goals = result.Field<double>("avg(forward_goals)"),
                    midfielder_goals = result.Field<double>("avg(midfielder_goals)"),
                    defender_goals = result.Field<double>("avg(defender_goals)"),
                    att_setpiece = result.Field<double>("avg(att_setpiece)"),
                    att_openplay = result.Field<double>("avg(att_openplay)"),
                    goals_openplay = result.Field<double>("avg(goals_openplay)"),
                    total_att_assist = result.Field<double>("avg(total_att_assist)"),
                    goal_assist = result.Field<double>("avg(goal_assist)"),
                    att_assist_setplay = result.Field<double>("avg(att_assist_setplay)"),
                    goal_assist_setplay = result.Field<double>("avg(goal_assist_setplay)"),
                    att_assist_openplay = result.Field<double>("avg(att_assist_openplay)"),
                    goal_assist_openplay = result.Field<double>("avg(goal_assist_openplay)"),
                    total_scoring_att = result.Field<double>("avg(total_scoring_att)"),
                    ontarget_scoring_att = result.Field<double>("avg(ontarget_scoring_att)"),
                    shot_off_target = result.Field<double>("avg(shot_off_target)"),
                    big_chance_created = result.Field<double>("avg(big_chance_created)"),
                    big_chance_scored = result.Field<double>("avg(big_chance_scored)"),
                    big_chance_missed = result.Field<double>("avg(big_chance_missed)"),
                    total_fastbreak = result.Field<double>("avg(total_fastbreak)"),
                    att_fastbreak = result.Field<double>("avg(att_fastbreak)"),
                    shot_fastbreak = result.Field<double>("avg(shot_fastbreak)"),
                    goal_fastbreak = result.Field<double>("avg(goal_fastbreak)"),
                    total_offside = result.Field<double>("avg(total_offside)"),
                    goals_conceded = result.Field<double>("avg(goals_conceded)"),
                    goals_conceded_ibox = result.Field<double>("avg(goals_conceded_ibox)"),
                    goals_conceded_obox = result.Field<double>("avg(goals_conceded_obox)"),
                    saves = result.Field<double>("avg(saves)"),
                    diving_save = result.Field<double>("avg(diving_save)"),
                    interception = result.Field<double>("avg(interception)"),
                    interception_won = result.Field<double>("avg(interception_won)"),
                    interceptions_in_box = result.Field<double>("avg(interceptions_in_box)"),
                    total_tackle = result.Field<double>("avg(total_tackle)"),
                    won_tackle = result.Field<double>("avg(won_tackle)"),
                    tackle_lost = result.Field<double>("avg(tackle_lost)"),
                    ball_recovery = result.Field<double>("avg(ball_recovery)"),
                    total_clearance = result.Field<double>("avg(total_clearance)"),
                    clean_sheet = result.Field<double>("avg(clean_sheet)"),
                    total_yel_card = result.Field<double>("avg(total_yel_card)"),
                    total_red_card = result.Field<double>("avg(total_red_card)"),
                    possession_percentage = result.Field<double>("avg(possession_percentage)"),
                    touches = result.Field<double>("avg(touches)"),
                    unsuccessful_touch = result.Field<double>("avg(unsuccessful_touch)"),
                    total_contest = result.Field<double>("avg(total_contest)"),
                    won_contest = result.Field<double>("avg(won_contest)"),
                    dribble_lost = result.Field<double>("avg(dribble_lost)"),
                    fifty_fifty = result.Field<double>("avg(fifty_fifty)"),
                    successful_fifty_fifty = result.Field<double>("avg(successful_fifty_fifty)"),
                    aerial_lost = result.Field<double>("avg(aerial_lost)"),
                    aerial_won = result.Field<double>("avg(aerial_won)")
                }
            );
            #endregion

            #region 用var数据生成TeamStatistics_Preview
            foreach (var teamSta in teamStatistics)
            {
                team.id = teamSta.id;
                team.name = teamSta.name;
                team.league = teamSta.league;
                team.rating = teamSta.rating;

                team.total_pass = teamSta.total_pass;
                team.accurate_pass = teamSta.accurate_pass;
                team.total_fwd_zone_pass = teamSta.total_fwd_zone_pass;
                team.accurate_fwd_zone_pass = teamSta.accurate_fwd_zone_pass;
                team.total_back_zone_pass = teamSta.total_back_zone_pass;
                team.accurate_back_zone_pass = teamSta.accurate_back_zone_pass;
                team.total_long_balls = teamSta.total_long_balls;
                team.accurate_long_balls = teamSta.accurate_long_balls;
                team.long_pass_own_to_opp = teamSta.long_pass_own_to_opp;
                team.long_pass_own_to_opp_success = teamSta.long_pass_own_to_opp_success;
                team.total_cross = teamSta.total_cross;
                team.accurate_cross = teamSta.accurate_cross;
                team.fwd_pass = teamSta.fwd_pass;
                team.backward_pass = teamSta.backward_pass;
                team.leftside_pass = teamSta.leftside_pass;
                team.rightside_pass = teamSta.rightside_pass;
                team.passes_left = teamSta.passes_left;
                team.passes_right = teamSta.passes_right;

                team.goals = teamSta.goals;
                team.first_half_goals = teamSta.first_half_goals;
                team.att_ibox_goal = teamSta.att_ibox_goal;
                team.att_obox_goal = teamSta.att_obox_goal;
                team.att_one_on_one = teamSta.att_one_on_one;
                team.forward_goals = teamSta.forward_goals;
                team.midfielder_goals = teamSta.midfielder_goals;
                team.defender_goals = teamSta.defender_goals;
                team.att_setpiece = teamSta.att_setpiece;
                team.att_openplay = teamSta.att_openplay;
                team.goals_openplay = teamSta.goals_openplay;
                team.total_att_assist = teamSta.total_att_assist;
                team.goal_assist = teamSta.goal_assist;
                team.att_assist_setplay = teamSta.att_assist_setplay;
                team.goal_assist_setplay = teamSta.goal_assist_setplay;
                team.att_assist_openplay = teamSta.att_assist_openplay;
                team.goal_assist_openplay = teamSta.goal_assist_openplay;
                team.total_scoring_att = teamSta.total_scoring_att;
                team.ontarget_scoring_att = teamSta.ontarget_scoring_att;
                team.shot_off_target = teamSta.shot_off_target;
                team.big_chance_created = teamSta.big_chance_created;
                team.big_chance_scored = teamSta.big_chance_scored;
                team.big_chance_missed = teamSta.big_chance_missed;
                team.total_fastbreak = teamSta.total_fastbreak;
                team.att_fastbreak = teamSta.att_fastbreak;
                team.shot_fastbreak = teamSta.shot_fastbreak;
                team.goal_fastbreak = teamSta.goal_fastbreak;
                team.total_offside = teamSta.total_offside;

                team.goals_conceded = teamSta.goals_conceded;
                team.goals_conceded_ibox = teamSta.goals_conceded_ibox;
                team.goals_conceded_obox = teamSta.goals_conceded_obox;
                team.saves = teamSta.saves;
                team.diving_save = teamSta.diving_save;
                team.interception = teamSta.interception;
                team.interception_won = teamSta.interception_won;
                team.interceptions_in_box = teamSta.interceptions_in_box;
                team.total_tackle = teamSta.total_tackle;
                team.won_tackle = teamSta.won_tackle;
                team.tackle_lost = teamSta.tackle_lost;
                team.ball_recovery = teamSta.ball_recovery;
                team.total_clearance = teamSta.total_clearance;
                team.clean_sheet = teamSta.clean_sheet;
                team.total_yel_card = teamSta.total_yel_card;
                team.total_red_card = teamSta.total_red_card;

                team.possession_percentage = teamSta.possession_percentage;
                team.touches = teamSta.touches;
                team.unsuccessful_touch = teamSta.unsuccessful_touch;
                team.total_contest = teamSta.total_contest;
                team.won_contest = teamSta.won_contest;
                team.dribble_lost = teamSta.dribble_lost;
                team.fifty_fifty = teamSta.fifty_fifty;
                team.successful_fifty_fifty = teamSta.successful_fifty_fifty;
                team.aerial_lost = teamSta.aerial_lost;
                team.aerial_won = teamSta.aerial_won;
            }
            #endregion

            return team;
        }
    }
}
