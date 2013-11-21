using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

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

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(ChkSrv);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Globe.WriteLog("Service Stop.");
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
                    System.Timers.Timer tempTimer = (System.Timers.Timer)source;
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

            //  downloading matches 1 minutes later
            if (iHour == Globe.WorkTime_Hour && iMinute == Globe.WorkTime_Minute + 1)
            {
                Globe.WriteLog("Begin downloading matches...");

                try
                {
                    System.Timers.Timer tempTimer = (System.Timers.Timer)source;
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

            //  updating matches to database 10 minutes later
            if (Globe.UpdateDBFlag && (iHour == Globe.WorkTime_Hour && iMinute == Globe.WorkTime_Minute + 10))
            {
                Globe.WriteLog("Begin updating matches...");

                try
                {
                    System.Timers.Timer tempTimer = (System.Timers.Timer)source;
                    tempTimer.Enabled = false;

                    DirectoryInfo directory = new DirectoryInfo(Globe.RootDir);
                    DirectoryInfo[] leagueDir = directory.GetDirectories();

                    foreach (DirectoryInfo league in leagueDir)
                    {
                        UpdateDataBase updateDB = new UpdateDataBase(league.Name, league.FullName);
                        Thread leagueThread = new Thread(new ThreadStart(updateDB.LoadFolder));
                        leagueThread.Start();
                    }

                    tempTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    Globe.WriteLog("WhoScoredSpiderService.ChkSrv(Update): " + ex.Message);
                }
            }
        }
    }
}