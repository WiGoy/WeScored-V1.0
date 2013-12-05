using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WhoScoredSpiderService
{
    class ContentFilter
    {
        private const string liveScoresFilter = @"(?:<a\sclass=).*?(?:\/>)";
        private const string matchInfoFilter = @"(?<=var initialData =).*?(?:])";
        private const string playerStatisticsFilter = @"(?:\[\d+,\').+?(?:]\r\n,)";
        private const string scoreIDFilter = @"(id=\u0022).*?(\u0022)";
        private const string standingsFilter = @"(?:DataStore\.prime\(\'standings\'[\s\S]).*?(?:\)\;)";
        private const string statisticsFilter = @"(?:\[\d+,\').+?(?:]]]],)";

        public List<int> GetMatchIDs(string htmlContent, List<int> originalMatchIDs)
        {
            string standingsContent = GetStandingsContent(htmlContent);
            List<int> matchIDs = GetMatchIDsFromStandingsContent(standingsContent, originalMatchIDs);

            return matchIDs;
        }

        public MatchInfo GetMatchInfo(int matchID, string league, string htmlContent)
        {
            MatchInfo matchInfo = new MatchInfo();
            matchInfo.id = matchID;
            matchInfo.League = league;

            string infoString = "";
            MatchCollection matches = Regex.Matches(htmlContent, matchInfoFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                infoString += matches[i].Value;
            }

            string[] infos = infoString.Split(new Char[] { '[', ',', '\'', ']' }, StringSplitOptions.RemoveEmptyEntries);

            if (infos.Length == 12)
            {
                string homeTeamFilter = @"(?:\[" + int.Parse(infos[1]) + @",'" + infos[3] + @"',).*?(?:]]]],)";
                string homeTeamContent = Regex.Match(htmlContent, homeTeamFilter, RegexOptions.Singleline).Value;
                string awayTeamFilter = @"(?:,\[" + int.Parse(infos[2]) + @",'" + infos[4] + @"',).*?(?:]]]],)";
                string awayTeamContent = Regex.Match(htmlContent, awayTeamFilter, RegexOptions.Singleline).Value;

                TeamStatistics homeTeamStatistics = GetTeamStatistics(homeTeamContent);
                TeamStatistics awayTeamStatistics = GetTeamStatistics(awayTeamContent);

                homeTeamStatistics.id = int.Parse(infos[1]);
                homeTeamStatistics.name = infos[3];
                homeTeamStatistics.rating = float.Parse(homeTeamContent.Split(new Char[] { '[', ',' }, StringSplitOptions.RemoveEmptyEntries)[2]);
                awayTeamStatistics.id = int.Parse(infos[2]);
                awayTeamStatistics.name = infos[4];
                awayTeamStatistics.rating = float.Parse(awayTeamContent.Split(new Char[] { '[', ',' }, StringSplitOptions.RemoveEmptyEntries)[2]);

                matchInfo.StartTime = DateTime.Parse(infos[5]).ToString("yyyy/MM/dd HH:mm:ss");
                matchInfo.HomeTeamStatistics = homeTeamStatistics;
                matchInfo.AwayTeamStatistics = awayTeamStatistics;
                matchInfo.HomeTeamPlayerStatistics = GetPlayerStatisticsList(htmlContent, homeTeamStatistics.id, homeTeamStatistics.name,
                    ref matchInfo.ManOfTheMatchPlayerID, ref matchInfo.ManOfTheMatchPlayerName);
                matchInfo.AwayTeamPlayerStatistics = GetPlayerStatisticsList(htmlContent, awayTeamStatistics.id, awayTeamStatistics.name,
                    ref matchInfo.ManOfTheMatchPlayerID, ref matchInfo.ManOfTheMatchPlayerName);
            }

            return matchInfo;
        }

        private string GetStandingsContent(string htmlContent)
        {
            string standingsContent = "";
            MatchCollection matches = Regex.Matches(htmlContent, standingsFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                standingsContent += matches[i].Value;
            }

            return standingsContent;
        }

        private List<int> GetMatchIDsFromStandingsContent(string standingsContent, List<int> originalMatchIDs)
        {
            List<int> matchIDs = new List<int>();
            MatchCollection matches = Regex.Matches(standingsContent, liveScoresFilter, RegexOptions.Singleline);

            for (int i = 0; i < matches.Count; i++)
            {
                int id = int.Parse(Regex.Match(Regex.Match(matches[i].Value, scoreIDFilter).Value, @"\d+").Value);

                if (!matchIDs.Contains(id) && !originalMatchIDs.Contains(id))
                    matchIDs.Add(id);
            }

            return matchIDs;
        }

        private TeamStatistics GetTeamStatistics(string teamContent)
        {
            TeamStatistics team = new TeamStatistics();

            team.accurate_back_zone_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_BACK_ZONE_PASS));
            team.accurate_chipped_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_CHIPPED_PASS));
            team.accurate_corners_intobox = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_CORNERS_INTOBOX));
            team.accurate_cross = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_CROSS));
            team.accurate_cross_nocorner = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_CROSS_NOCORNER));
            team.accurate_flick_on = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_FLICK_ON));
            team.accurate_freekick_cross = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_FREEKICK_CROSS));
            team.accurate_fwd_zone_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_FWD_ZONE_PASS));
            team.accurate_goal_kicks = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_GOAL_KICKS));
            team.accurate_keeper_sweeper = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_KEEPER_SWEEPER));
            team.accurate_keeper_throws = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_KEEPER_THROWS));
            team.accurate_launches = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_LAUNCHES));
            team.accurate_layoffs = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_LAYOFFS));
            team.accurate_long_balls = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_LONG_BALLS));
            team.accurate_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_PASS));
            team.accurate_through_ball = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_THROUGH_BALL));
            team.accurate_throws = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ACCURATE_THROWS));
            team.aerial_lost = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.AERIAL_LOST));
            team.aerial_won = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.AERIAL_WON));
            team.att_assist_openplay = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_ASSIST_OPENPLAY));
            team.att_assist_setplay = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_ASSIST_SETPLAY));
            team.att_bx_centre = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_BX_CENTRE));
            team.att_bx_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_BX_LEFT));
            team.att_bx_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_BX_RIGHT));
            team.att_cmiss_high = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_CMISS_HIGH));
            team.att_cmiss_high_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_CMISS_HIGH_RIGHT));
            team.att_cmiss_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_CMISS_LEFT));
            team.att_cmiss_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_CMISS_RIGHT));
            team.att_fastbreak = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_FASTBREAK));
            team.att_freekick_goal = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_FREEKICK_GOAL));
            team.att_freekick_miss = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_FREEKICK_MISS));
            team.att_freekick_post = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_FREEKICK_POST));
            team.att_freekick_target = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_FREEKICK_TARGET));
            team.att_freekick_total = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_FREEKICK_TOTAL));
            team.att_goal_high_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_GOAL_HIGH_LEFT));
            team.att_goal_high_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_GOAL_HIGH_RIGHT));
            team.att_goal_low_centre = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_GOAL_LOW_CENTRE));
            team.att_goal_low_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_GOAL_LOW_LEFT));
            team.att_goal_low_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_GOAL_LOW_RIGHT));
            team.att_hd_goal = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_HD_GOAL));
            team.att_hd_miss = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_HD_MISS));
            team.att_hd_post = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_HD_POST));
            team.att_hd_target = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_HD_TARGET));
            team.att_hd_total = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_HD_TOTAL));
            team.att_ibox_blocked = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_IBOX_BLOCKED));
            team.att_ibox_goal = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_IBOX_GOAL));
            team.att_ibox_miss = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_IBOX_MISS));
            team.att_ibox_post = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_IBOX_POST));
            team.att_ibox_target = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_IBOX_TARGET));
            team.att_lf_goal = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_LF_GOAL));
            team.att_lf_target = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_LF_TARGET));
            team.att_lf_total = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_LF_TOTAL));
            team.att_lg_centre = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_LG_CENTRE));
            team.att_miss_high = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_MISS_HIGH));
            team.att_miss_high_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_MISS_HIGH_LEFT));
            team.att_miss_high_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_MISS_HIGH_RIGHT));
            team.att_miss_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_MISS_LEFT));
            team.att_miss_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_MISS_RIGHT));
            team.att_obox_blocked = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OBOX_BLOCKED));
            team.att_obox_goal = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OBOX_GOAL));
            team.att_obox_miss = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OBOX_MISS));
            team.att_obox_post = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OBOX_POST));
            team.att_obox_target = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OBOX_TARGET));
            team.att_obx_centre = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OBX_CENTRE));
            team.att_obx_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OBX_LEFT));
            team.att_obxd_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OBXD_RIGHT));
            team.att_one_on_one = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_ONE_ON_ONE));
            team.att_openplay = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_OPENPLAY));
            team.att_pen_goal = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_PEN_GOAL));
            team.att_pen_target = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_PEN_TARGET));
            team.att_post_high = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_POST_HIGH));
            team.att_post_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_POST_RIGHT));
            team.att_rf_goal = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_RF_GOAL));
            team.att_rf_target = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_RF_TARGET));
            team.att_rf_total = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_RF_TOTAL));
            team.att_setpiece = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_SETPIECE));
            team.att_sv_high_centre = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_SV_HIGH_CENTRE));
            team.att_sv_high_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_SV_HIGH_LEFT));
            team.att_sv_high_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_SV_HIGH_RIGHT));
            team.att_sv_low_centre = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_SV_LOW_CENTRE));
            team.att_sv_low_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_SV_LOW_LEFT));
            team.att_sv_low_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATT_SV_LOW_RIGHT));
            team.attempted_tackle_foul = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATTEMPTED_TACKLE_FOUL));
            team.attempts_conceded_ibox = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATTEMPTS_CONCEDED_IBOX));
            team.attempts_conceded_obox = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ATTEMPTS_CONCEDED_OBOX));
            team.backward_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.BACKWARD_PASS));
            team.ball_recovery = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.BALL_RECOVERY));
            team.big_chance_created = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.BIG_CHANCE_CREATED));
            team.big_chance_missed = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.BIG_CHANCE_MISSED));
            team.big_chance_scored = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.BIG_CHANCE_SCORED));
            team.blocked_cross = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.BLOCKED_CROSS));
            team.blocked_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.BLOCKED_PASS));
            team.blocked_scoring_att = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.BLOCKED_SCORING_ATT));
            team.challenge_lost = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.CHALLENGE_LOST));
            team.clean_sheet = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.CLEAN_SHEET));
            team.clearance_off_line = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.CLEARANCE_OFF_LINE));
            team.contentious_decision = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.CONTENTIOUS_DECISION));
            team.corner_taken = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.CORNER_TAKEN));
            team.cross_inaccurate = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.CROSS_INACCURATE));
            team.crosses_18yard = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.CROSSES_18YARD));
            team.crosses_18yardplus = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.CROSSES_18YARDPLUS));
            team.defender_goals = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.DEFENDER_GOALS));
            team.dispossessed = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.DISPOSSESSED));
            team.diving_save = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.DIVING_SAVE));
            team.dribble_lost = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.DRIBBLE_LOST));
            team.duel_ground_lost = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.DUEL_GROUND_LOST));
            team.duel_ground_won = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.DUEL_GROUND_WON));
            team.duel_lost = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.DUEL_LOST));
            team.duel_won = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.DUEL_WON));
            team.effective_blocked_cross = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.EFFECTIVE_BLOCKED_CROSS));
            team.effective_clearance = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.EFFECTIVE_CLEARANCE));
            team.effective_head_clearance = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.EFFECTIVE_HEAD_CLEARANCE));
            team.error_lead_to_goal = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ERROR_LEAD_TO_GOAL));
            team.error_lead_to_shot = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ERROR_LEAD_TO_SHOT));
            team.failed_to_block = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FAILED_TO_BLOCK));
            team.fifty_fifty = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FIFTY_FIFTY));
            team.final_third_entries = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FINAL_THIRD_ENTRIES));
            team.first_half_goals = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FIRST_HALF_GOALS));
            team.fk_foul_lost = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FK_FOUL_LOST));
            team.fk_foul_won = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FK_FOUL_WON));
            team.formation_used = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FORMATION_USED));
            team.forward_goals = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FORWARD_GOALS));
            team.fouled_final_third = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FOULED_FINAL_THIRD));
            team.freekick_cross = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FREEKICK_CROSS));
            team.fwd_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.FWD_PASS));
            team.goal_assist = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOAL_ASSIST));
            team.goal_assist_intentional = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOAL_ASSIST_INTENTIONAL));
            team.goal_assist_openplay = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOAL_ASSIST_OPENPLAY));
            team.goal_assist_setplay = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOAL_ASSIST_SETPLAY));
            team.goal_fastbreak = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOAL_FASTBREAK));
            team.goal_kicks = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOAL_KICKS));
            team.goals = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOALS));
            team.goals_conceded = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOALS_CONCEDED));
            team.goals_conceded_ibox = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOALS_CONCEDED_IBOX));
            team.goals_conceded_obox = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOALS_CONCEDED_OBOX));
            team.goals_openplay = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOALS_OPENPLAY));
            team.good_high_claim = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.GOOD_HIGH_CLAIM));
            team.hand_ball = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.HAND_BALL));
            team.head_clearance = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.HEAD_CLEARANCE));
            team.hit_woodwork = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.HIT_WOODWORK));
            team.interception = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.INTERCEPTION));
            team.interception_won = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.INTERCEPTION_WON));
            team.interceptions_in_box = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.INTERCEPTIONS_IN_BOX));
            team.keeper_throws = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.KEEPER_THROWS));
            team.last_man_tackle = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.LAST_MAN_TACKLE));
            team.leftside_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.LEFTSIDE_PASS));
            team.long_pass_own_to_opp = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.LONG_PASS_OWN_TO_OPP));
            team.long_pass_own_to_opp_success = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.LONG_PASS_OWN_TO_OPP_SUCCESS));
            team.lost_corners = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.LOST_CORNERS));
            team.midfielder_goals = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.MIDFIELDER_GOALS));
            team.offtarget_att_assist = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.OFFTARGET_ATT_ASSIST));
            team.ontarget_att_assist = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ONTARGET_ATT_ASSIST));
            team.ontarget_scoring_att = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.ONTARGET_SCORING_ATT));
            team.open_play_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.OPEN_PLAY_PASS));
            team.outfielder_block = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.OUTFIELDER_BLOCK));
            team.overrun = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.OVERRUN));
            team.passes_left = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PASSES_LEFT));
            team.passes_right = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PASSES_RIGHT));
            team.pen_area_entries = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PEN_AREA_ENTRIES));
            team.pen_goals_conceded = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PEN_GOALS_CONCEDED));
            team.penalty_conceded = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PENALTY_CONCEDED));
            team.penalty_faced = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PENALTY_FACED));
            team.penalty_missed = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PENALTY_MISSED));
            team.penalty_save = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PENALTY_SAVE));
            team.penalty_won = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PENALTY_WON));
            team.poss_lost_all = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.POSS_LOST_ALL));
            team.poss_lost_ctrl = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.POSS_LOST_CTRL));
            team.poss_won_att_3rd = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.POSS_WON_ATT_3RD));
            team.poss_won_def_3rd = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.POSS_WON_DEF_3RD));
            team.poss_won_mid_3rd = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.POSS_WON_MID_3RD));
            team.possession_percentage = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.POSSESSION_PERCENTAGE));
            team.post_scoring_att = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.POST_SCORING_ATT));
            team.punches = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PUNCHES));
            team.put_through = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.PUT_THROUGH));
            team.rightside_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.RIGHTSIDE_PASS));
            team.saved_ibox = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SAVED_IBOX));
            team.saved_obox = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SAVED_OBOX));
            team.saves = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SAVES));
            team.second_yellow = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SECOND_YELLOW));
            team.shield_ball_oop = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SHIELD_BALL_OOP));
            team.shot_fastbreak = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SHOT_FASTBREAK));
            team.shot_off_target = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SHOT_OFF_TARGET));
            team.six_yard_block = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SIX_YARD_BLOCK));
            team.subs_made = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SUBS_MADE));
            team.successful_fifty_fifty = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SUCCESSFUL_FIFTY_FIFTY));
            team.successful_final_third_passes = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SUCCESSFUL_FINAL_THIRD_PASSES));
            team.successful_open_play_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SUCCESSFUL_OPEN_PLAY_PASS));
            team.successful_put_through = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.SUCCESSFUL_PUT_THROUGH));
            team.tackle_lost = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TACKLE_LOST));
            team.total_att_assist = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_ATT_ASSIST));
            team.total_back_zone_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_BACK_ZONE_PASS));
            team.total_chipped_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_CHIPPED_PASS));
            team.total_clearance = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_CLEARANCE));
            team.total_contest = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_CONTEST));
            team.total_corners_intobox = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_CORNERS_INTOBOX));
            team.total_cross = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_CROSS));
            team.total_cross_nocorner = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_CROSS_NOCORNER));
            team.total_fastbreak = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_FASTBREAK));
            team.total_final_third_passes = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_FINAL_THIRD_PASSES));
            team.total_flick_on = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_FLICK_ON));
            team.total_fwd_zone_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_FWD_ZONE_PASS));
            team.total_high_claim = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_HIGH_CLAIM));
            team.total_keeper_sweeper = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_KEEPER_SWEEPER));
            team.total_launches = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_LAUNCHES));
            team.total_layoffs = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_LAYOFFS));
            team.total_long_balls = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_LONG_BALLS));
            team.total_offside = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_OFFSIDE));
            team.total_pass = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_PASS));
            team.total_pull_back = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_PULL_BACK));
            team.total_red_card = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_RED_CARD));
            team.total_scoring_att = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_SCORING_ATT));
            team.total_tackle = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_TACKLE));
            team.total_through_ball = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_THROUGH_BALL));
            team.total_throws = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_THROWS));
            team.total_yel_card = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOTAL_YEL_CARD));
            team.touches = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.TOUCHES));
            team.unsuccessful_touch = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.UNSUCCESSFUL_TOUCH));
            team.won_contest = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.WON_CONTEST));
            team.won_corners = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.WON_CORNERS));
            team.won_tackle = float.Parse(GetStatistics(teamContent, TeamStatisticsFilter.WON_TACKLE));

            return team;
        }

        private List<PlayerStatistics> GetPlayerStatisticsList(string htmlContent, int teamID, string teamName,
            ref int manOfTheMatchPlayerID, ref string manOfTheMatchPlayerName)
        {
            List<PlayerStatistics> playerStatisticsList = new List<PlayerStatistics>();

            string teamContentFilter = @"(?<=\[" + teamID + @",'" + teamName + @"',).*?(?:]]\r)";
            string playerContent = Regex.Match(htmlContent, teamContentFilter, RegexOptions.Singleline).Value;

            MatchCollection playerStatisticsMatches = Regex.Matches(playerContent, playerStatisticsFilter, RegexOptions.Singleline);

            for (int i = 0; i < playerStatisticsMatches.Count; i++)
            {
                string playerStatisticsStr = playerStatisticsMatches[i].Value;
                PlayerStatistics playerStatistics = GetPlayerStatistics(playerStatisticsStr);

                string[] playerInfos = playerStatisticsStr.Split(new Char[] { '[', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (playerInfos.Length > 2)
                {
                    playerStatistics.id = int.Parse(playerInfos[0]);

                    string tempName = playerInfos[1];
                    playerStatistics.name = tempName.Substring(1, tempName.Length - 2);
                }

                if (playerStatistics.man_of_the_match == 1)
                {
                    manOfTheMatchPlayerID = playerStatistics.id;
                    manOfTheMatchPlayerName = playerStatistics.name;
                }

                playerStatisticsList.Add(playerStatistics);
            }

            return playerStatisticsList;
        }

        private PlayerStatistics GetPlayerStatistics(string playerContent)
        {
            PlayerStatistics player = new PlayerStatistics();

            player.accurate_back_zone_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_BACK_ZONE_PASS));
            player.accurate_chipped_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_CHIPPED_PASS));
            player.accurate_corners_intobox = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_CORNERS_INTOBOX));
            player.accurate_cross = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_CROSS));
            player.accurate_cross_nocorner = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_CROSS_NOCORNER));
            player.accurate_flick_on = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_FLICK_ON));
            player.accurate_freekick_cross = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_FREEKICK_CROSS));
            player.accurate_fwd_zone_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_FWD_ZONE_PASS));
            player.accurate_goal_kicks = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_GOAL_KICKS));
            player.accurate_keeper_sweeper = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_KEEPER_SWEEPER));
            player.accurate_keeper_throws = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_KEEPER_THROWS));
            player.accurate_launches = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_LAUNCHES));
            player.accurate_layoffs = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_LAYOFFS));
            player.accurate_long_balls = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_LONG_BALLS));
            player.accurate_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_PASS));
            player.accurate_through_ball = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_THROUGH_BALL));
            player.accurate_throws = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ACCURATE_THROWS));
            player.aerial_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.AERIAL_LOST));
            player.aerial_won = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.AERIAL_WON));
            player.assist_penalty_won = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ASSIST_PENALTY_WON));
            player.att_assist_openplay = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_ASSIST_OPENPLAY));
            player.att_assist_setplay = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_ASSIST_SETPLAY));
            player.att_bx_centre = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_BX_CENTRE));
            player.att_bx_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_BX_LEFT));
            player.att_bx_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_BX_RIGHT));
            player.att_cmiss_high = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_CMISS_HIGH));
            player.att_cmiss_high_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_CMISS_HIGH_RIGHT));
            player.att_cmiss_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_CMISS_LEFT));
            player.att_cmiss_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_CMISS_RIGHT));
            player.att_fastbreak = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_FASTBREAK));
            player.att_freekick_goal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_FREEKICK_GOAL));
            player.att_freekick_miss = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_FREEKICK_MISS));
            player.att_freekick_post = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_FREEKICK_POST));
            player.att_freekick_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_FREEKICK_TARGET));
            player.att_freekick_total = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_FREEKICK_TOTAL));
            player.att_goal_high_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_GOAL_HIGH_LEFT));
            player.att_goal_high_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_GOAL_HIGH_RIGHT));
            player.att_goal_low_centre = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_GOAL_LOW_CENTRE));
            player.att_goal_low_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_GOAL_LOW_LEFT));
            player.att_goal_low_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_GOAL_LOW_RIGHT));
            player.att_hd_goal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_HD_GOAL));
            player.att_hd_miss = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_HD_MISS));
            player.att_hd_post = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_HD_POST));
            player.att_hd_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_HD_TARGET));
            player.att_hd_total = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_HD_TOTAL));
            player.att_ibox_blocked = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_IBOX_BLOCKED));
            player.att_ibox_goal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_IBOX_GOAL));
            player.att_ibox_miss = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_IBOX_MISS));
            player.att_ibox_post = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_IBOX_POST));
            player.att_ibox_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_IBOX_TARGET));
            player.att_lf_goal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_LF_GOAL));
            player.att_lf_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_LF_TARGET));
            player.att_lf_total = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_LF_TOTAL));
            player.att_lg_centre = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_LG_CENTRE));
            player.att_miss_high = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_MISS_HIGH));
            player.att_miss_high_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_MISS_HIGH_LEFT));
            player.att_miss_high_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_MISS_HIGH_RIGHT));
            player.att_miss_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_MISS_LEFT));
            player.att_miss_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_MISS_RIGHT));
            player.att_obox_blocked = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OBOX_BLOCKED));
            player.att_obox_goal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OBOX_GOAL));
            player.att_obox_miss = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OBOX_MISS));
            player.att_obox_post = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OBOX_POST));
            player.att_obox_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OBOX_TARGET));
            player.att_obx_centre = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OBX_CENTRE));
            player.att_obx_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OBX_LEFT));
            player.att_obxd_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OBXD_RIGHT));
            player.att_one_on_one = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_ONE_ON_ONE));
            player.att_openplay = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_OPENPLAY));
            player.att_pen_goal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_PEN_GOAL));
            player.att_pen_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_PEN_TARGET));
            player.att_post_high = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_POST_HIGH));
            player.att_post_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_POST_RIGHT));
            player.att_rf_goal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_RF_GOAL));
            player.att_rf_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_RF_TARGET));
            player.att_rf_total = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_RF_TOTAL));
            player.att_setpiece = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_SETPIECE));
            player.att_sv_high_centre = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_SV_HIGH_CENTRE));
            player.att_sv_high_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_SV_HIGH_LEFT));
            player.att_sv_high_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_SV_HIGH_RIGHT));
            player.att_sv_low_centre = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_SV_LOW_CENTRE));
            player.att_sv_low_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_SV_LOW_LEFT));
            player.att_sv_low_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATT_SV_LOW_RIGHT));
            player.attempted_tackle_foul = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATTEMPTED_TACKLE_FOUL));
            player.attempts_conceded_ibox = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATTEMPTS_CONCEDED_IBOX));
            player.attempts_conceded_obox = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ATTEMPTS_CONCEDED_OBOX));
            player.backward_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.BACKWARD_PASS));
            player.ball_recovery = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.BALL_RECOVERY));
            player.big_chance_created = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.BIG_CHANCE_CREATED));
            player.big_chance_missed = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.BIG_CHANCE_MISSED));
            player.big_chance_scored = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.BIG_CHANCE_SCORED));
            player.blocked_cross = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.BLOCKED_CROSS));
            player.blocked_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.BLOCKED_PASS));
            player.blocked_scoring_att = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.BLOCKED_SCORING_ATT));
            player.challenge_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CHALLENGE_LOST));
            player.clean_sheet_amc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_AMC));
            player.clean_sheet_amr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_AMR));
            player.clean_sheet_dc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_DC));
            player.clean_sheet_dl = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_DL));
            player.clean_sheet_dmc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_DMC));
            player.clean_sheet_dml = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_DML));
            player.clean_sheet_dmr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_DMR));
            player.clean_sheet_dr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_DR));
            player.clean_sheet_fw = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_FW));
            player.clean_sheet_gk = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEAN_SHEET_GK));
            player.clearance_off_line = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CLEARANCE_OFF_LINE));
            player.corner_taken = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CORNER_TAKEN));
            player.cross_inaccurate = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CROSS_INACCURATE));
            player.crosses_18yard = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CROSSES_18YARD));
            player.crosses_18yardplus = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.CROSSES_18YARDPLUS));
            player.dangerous_play = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DANGEROUS_PLAY));
            player.dispossessed = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DISPOSSESSED));
            player.dive_catch = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DIVE_CATCH));
            player.dive_save = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DIVE_SAVE));
            player.diving_save = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DIVING_SAVE));
            player.dribble_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DRIBBLE_LOST));
            player.duel_ground_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DUEL_GROUND_LOST));
            player.duel_ground_won = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DUEL_GROUND_WON));
            player.duel_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DUEL_LOST));
            player.duel_won = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.DUEL_WON));
            player.effective_blocked_cross = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.EFFECTIVE_BLOCKED_CROSS));
            player.effective_clearance = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.EFFECTIVE_CLEARANCE));
            player.effective_head_clearance = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.EFFECTIVE_HEAD_CLEARANCE));
            player.error_lead_to_goal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ERROR_LEAD_TO_GOAL));
            player.error_lead_to_shot = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ERROR_LEAD_TO_SHOT));
            player.failed_to_block = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.FAILED_TO_BLOCK));
            player.fifty_fifty = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.FIFTY_FIFTY));
            player.final_third_entries = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.FINAL_THIRD_ENTRIES));
            player.formation_place = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.FORMATION_PLACE));
            player.fouled_final_third = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.FOULED_FINAL_THIRD));
            player.fouls = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.FOULS));
            player.freekick_cross = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.FREEKICK_CROSS));
            player.fwd_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.FWD_PASS));
            player.game_started = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GAME_STARTED));
            player.gk_smother = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GK_SMOTHER));
            player.goal_assist = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_ASSIST));
            player.goal_assist_intentional = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_ASSIST_INTENTIONAL));
            player.goal_assist_openplay = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_ASSIST_OPENPLAY));
            player.goal_assist_setplay = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_ASSIST_SETPLAY));
            player.goal_fastbreak = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_FASTBREAK));
            player.goal_kicks = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_KICKS));
            player.goal_normal = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_NORMAL));
            player.goal_scored_by_team_amc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_AMC));
            player.goal_scored_by_team_aml = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_AML));
            player.goal_scored_by_team_amr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_AMR));
            player.goal_scored_by_team_dc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_DC));
            player.goal_scored_by_team_dl = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_DL));
            player.goal_scored_by_team_dmc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_DMC));
            player.goal_scored_by_team_dml = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_DML));
            player.goal_scored_by_team_dmr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_DMR));
            player.goal_scored_by_team_dr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_DR));
            player.goal_scored_by_team_fw = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_FW));
            player.goal_scored_by_team_fwl = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_FWL));
            player.goal_scored_by_team_fwr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_FWR));
            player.goal_scored_by_team_gk = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_GK));
            player.goal_scored_by_team_mc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_MC));
            player.goal_scored_by_team_ml = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_ML));
            player.goal_scored_by_team_mr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_MR));
            player.goal_scored_by_team_sub = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOAL_SCORED_BY_TEAM_SUB));
            player.goals = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS));
            player.goals_conceded_amc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_AMC));
            player.goals_conceded_aml = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_AML));
            player.goals_conceded_amr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_AMR));
            player.goals_conceded_dc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_DC));
            player.goals_conceded_dl = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_DL));
            player.goals_conceded_dmc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_DMC));
            player.goals_conceded_dr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_DR));
            player.goals_conceded_fw = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_FW));
            player.goals_conceded_fwl = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_FWL));
            player.goals_conceded_fwr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_FWR));
            player.goals_conceded_gk = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_GK));
            player.goals_conceded_ibox = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_IBOX));
            player.goals_conceded_mc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_MC));
            player.goals_conceded_ml = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_ML));
            player.goals_conceded_mr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_MR));
            player.goals_conceded_obox_amc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_AMC));
            player.goals_conceded_obox_aml = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_AML));
            player.goals_conceded_obox_amr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_AMR));
            player.goals_conceded_obox_dc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_DC));
            player.goals_conceded_obox_dl = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_DL));
            player.goals_conceded_obox_dmc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_DMC));
            player.goals_conceded_obox_dr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_DR));
            player.goals_conceded_obox_fw = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_FW));
            player.goals_conceded_obox_gk = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_GK));
            player.goals_conceded_obox_mc = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_MC));
            player.goals_conceded_obox_ml = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_ML));
            player.goals_conceded_obox_mr = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_MR));
            player.goals_conceded_obox_sub = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_OBOX_SUB));
            player.goals_conceded_sub = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_CONCEDED_SUB));
            player.goals_openplay = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOALS_OPENPLAY));
            player.good_high_claim = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.GOOD_HIGH_CLAIM));
            player.hand_ball = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.HAND_BALL));
            player.head_clearance = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.HEAD_CLEARANCE));
            player.head_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.HEAD_PASS));
            player.hit_woodwork = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.HIT_WOODWORK));
            player.interception = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.INTERCEPTION));
            player.interception_won = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.INTERCEPTION_WON));
            player.interceptions_in_box = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.INTERCEPTIONS_IN_BOX));
            player.keeper_claim_high_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.KEEPER_CLAIM_HIGH_LOST));
            player.keeper_claim_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.KEEPER_CLAIM_LOST));
            player.keeper_pick_up = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.KEEPER_PICK_UP));
            player.keeper_sweeper_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.KEEPER_SWEEPER_LOST));
            player.keeper_throws = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.KEEPER_THROWS));
            player.last_man_tackle = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.LAST_MAN_TACKLE));
            player.leftside_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.LEFTSIDE_PASS));
            player.long_pass_own_to_opp = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.LONG_PASS_OWN_TO_OPP));
            player.long_pass_own_to_opp_success = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.LONG_PASS_OWN_TO_OPP_SUCCESS));
            player.lost_corners = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.LOST_CORNERS));
            player.man_of_the_match = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.MAN_OF_THE_MATCH));
            player.mins_played = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.MINS_PLAYED));
            player.offside_provoked = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.OFFSIDE_PROVOKED));
            player.offtarget_att_assist = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.OFFTARGET_ATT_ASSIST));
            player.ontarget_att_assist = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ONTARGET_ATT_ASSIST));
            player.ontarget_scoring_att = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.ONTARGET_SCORING_ATT));
            player.open_play_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.OPEN_PLAY_PASS));
            player.outfielder_block = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.OUTFIELDER_BLOCK));
            player.overrun = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.OVERRUN));
            player.pass_backzone_inaccurate = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PASS_BACKZONE_INACCURATE));
            player.pass_forwardzone_inaccurate = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PASS_FORWARDZONE_INACCURATE));
            player.pass_inaccurate = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PASS_INACCURATE));
            player.pass_longball_inaccurate = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PASS_LONGBALL_INACCURATE));
            player.pass_throughball_inacurate = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PASS_THROUGHBALL_INACURATE));
            player.passes_left = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PASSES_LEFT));
            player.passes_right = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PASSES_RIGHT));
            player.pen_area_entries = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PEN_AREA_ENTRIES));
            player.pen_goals_conceded = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PEN_GOALS_CONCEDED));
            player.penalty_conceded = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_CONCEDED));
            player.penalty_faced = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_FACED));
            player.penalty_missed = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_MISSED));
            player.penalty_save = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_SAVE));
            player.penalty_shootout_conceded_gk = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_SHOOTOUT_CONCEDED_GK));
            player.penalty_shootout_missed_off_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_SHOOTOUT_MISSED_OFF_TARGET));
            player.penalty_shootout_saved = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_SHOOTOUT_SAVED));
            player.penalty_shootout_saved_gk = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_SHOOTOUT_SAVED_GK));
            player.penalty_shootout_scored = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_SHOOTOUT_SCORED));
            player.penalty_won = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PENALTY_WON));
            player.position = GetStatistics(playerContent, PlayerStatisticsFilter.POSITION).Replace("'", "");
            player.poss_lost_all = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.POSS_LOST_ALL));
            player.poss_lost_ctrl = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.POSS_LOST_CTRL));
            player.poss_won_att_3rd = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.POSS_WON_ATT_3RD));
            player.poss_won_def_3rd = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.POSS_WON_DEF_3RD));
            player.poss_won_mid_3rd = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.POSS_WON_MID_3RD));
            player.post_scoring_att = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.POST_SCORING_ATT));
            player.punches = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PUNCHES));
            player.put_through = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.PUT_THROUGH));
            player.rating = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.RATING));
            player.rating_defensive = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.RATING_DEFENSIVE));
            player.rating_defensive_points = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.RATING_DEFENSIVE_POINTS));
            player.rating_offensive = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.RATING_OFFENSIVE));
            player.rating_offensive_points = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.RATING_OFFENSIVE_POINTS));
            player.rating_points = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.RATING_POINTS));
            player.red_card = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.RED_CARD));
            player.rightside_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.RIGHTSIDE_PASS));
            player.saved_ibox = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SAVED_IBOX));
            player.saved_obox = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SAVED_OBOX));
            player.saves = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SAVES));
            player.second_goal_assist = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SECOND_GOAL_ASSIST));
            player.second_yellow = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SECOND_YELLOW));
            player.shield_ball_oop = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SHIELD_BALL_OOP));
            player.shot_fastbreak = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SHOT_FASTBREAK));
            player.shot_off_target = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SHOT_OFF_TARGET));
            player.six_yard_block = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SIX_YARD_BLOCK));
            player.stand_catch = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.STAND_CATCH));
            player.successful_fifty_fifty = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SUCCESSFUL_FIFTY_FIFTY));
            player.successful_final_third_passes = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SUCCESSFUL_FINAL_THIRD_PASSES));
            player.successful_open_play_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SUCCESSFUL_OPEN_PLAY_PASS));
            player.successful_put_through = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.SUCCESSFUL_PUT_THROUGH));
            player.tackle_lost = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TACKLE_LOST));
            player.total_att_assist = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_ATT_ASSIST));
            player.total_back_zone_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_BACK_ZONE_PASS));
            player.total_chipped_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_CHIPPED_PASS));
            player.total_clearance = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_CLEARANCE));
            player.total_contest = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_CONTEST));
            player.total_corners_intobox = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_CORNERS_INTOBOX));
            player.total_cross = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_CROSS));
            player.total_cross_nocorner = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_CROSS_NOCORNER));
            player.total_fastbreak = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_FASTBREAK));
            player.total_final_third_passes = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_FINAL_THIRD_PASSES));
            player.total_flick_on = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_FLICK_ON));
            player.total_fwd_zone_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_FWD_ZONE_PASS));
            player.total_high_claim = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_HIGH_CLAIM));
            player.total_keeper_sweeper = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_KEEPER_SWEEPER));
            player.total_launches = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_LAUNCHES));
            player.total_layoffs = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_LAYOFFS));
            player.total_long_balls = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_LONG_BALLS));
            player.total_offside = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_OFFSIDE));
            player.total_pass = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_PASS));
            player.total_pull_back = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_PULL_BACK));
            player.total_scoring_att = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_SCORING_ATT));
            player.total_sub_off = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_SUB_OFF));
            player.total_sub_on = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_SUB_ON));
            player.total_tackle = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_TACKLE));
            player.total_through_ball = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_THROUGH_BALL));
            player.total_throws = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOTAL_THROWS));
            player.touches = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TOUCHES));
            player.turnover = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.TURNOVER));
            player.unsuccessful_touch = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.UNSUCCESSFUL_TOUCH));
            player.was_fouled = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.WAS_FOULED));
            player.won_contest = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.WON_CONTEST));
            player.won_corners = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.WON_CORNERS));
            player.won_tackle = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.WON_TACKLE));
            player.yellow_card = float.Parse(GetStatistics(playerContent, PlayerStatisticsFilter.YELLOW_CARD));

            return player;
        }

        private string GetStatistics(string content, string filter)
        {
            string statisticsFilter = @"(?<=\['" + filter + @"',\[).*?(?=]+,)";
            string ret = Regex.Match(content, statisticsFilter, RegexOptions.Singleline).Value;

            if (ret.Equals(""))
                ret = "0";

            return ret;
        }
    }
}