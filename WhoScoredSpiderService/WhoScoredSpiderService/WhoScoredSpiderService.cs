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
            timer.Interval = 300;
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

            if (iHour == Globe.WorkTime_Hour && iMinute == Globe.WorkTime_Minute)
            {
                Globe.WriteLog("On Time Event!");

                try
                {
                    Timer tempTimer = (Timer)source;
                    tempTimer.Enabled = false;

                    SpiderSync spider = new SpiderSync();
                    spider.GetAllLeagues();

                    tempTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    Globe.WriteLog("WhoScoredSpiderService.ChkSrv: " + ex.Message);
                }
            }
        }
    }
}
