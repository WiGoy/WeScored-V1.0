using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Timers;

using System.Diagnostics;

namespace WhoScoredSpiderService
{
    public partial class WhoScoredSpiderService : ServiceBase
    {
        public WhoScoredSpiderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Globe.WriteLog("Service start.");

            Configuration config = new Configuration();
            config.GetConfiguration();

            Timer timer = new Timer();
            timer.Interval = 60000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(ChkSrv);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
        }

        private void ChkSrv(object source, System.Timers.ElapsedEventArgs e)
        {
            int iHour = e.SignalTime.Hour;
            int iMinute = e.SignalTime.Minute;

            //  downloading Leagues
            if (iHour == Globe.WorkTime_Hour && iMinute == Globe.WorkTime_Minute)
            {
                Globe.WriteLog("Begin downloading leagues...");

                try
                {
                    Timer tempTimer = (Timer)source;
                    tempTimer.Enabled = false;

                    SpiderAsync spider = new SpiderAsync();
                    spider.GetAllLeagues();

                    tempTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    Globe.WriteLog("WhoScoredSpiderService.ChkSrv(Leagues): " + ex.Message);
                }
            }

            //  downloading matches 10 minutes later
            if (iHour == Globe.WorkTime_Hour && iMinute == Globe.WorkTime_Minute + 10)
            {
                Globe.WriteLog("Begin downloading matches...");

                try
                {
                    Timer tempTimer = (Timer)source;
                    tempTimer.Enabled = false;

                    SpiderAsync spider = new SpiderAsync();
                    spider.GetAllMatches();

                    tempTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    Globe.WriteLog("WhoScoredSpiderService.ChkSrv(Matches): " + ex.Message);
                }
            }
        }
    }
}
