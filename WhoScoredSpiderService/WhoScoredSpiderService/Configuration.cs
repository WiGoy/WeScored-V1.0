using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhoScoredSpiderService
{
    class Configuration
    {
        public void GetConfiguration()
        {
            ReadConfig();
            InitializeRootDir();
            InitializeLeagues();
        }

        private void ReadConfig()
        {
            Globe.ConfigDic.Clear();

            try
            {
                StreamReader configReader = new StreamReader(Globe.ConfigFile, Encoding.Default);

                string line = "";

                while (line != null)
                {
                    line = configReader.ReadLine();

                    if (line != null && !line.Equals("") && !line.StartsWith("//"))
                    {
                        string[] temp = line.Split('=');
                        Globe.ConfigDic.Add(temp[0].Trim(), temp[1].Trim());
                    }
                }

                configReader.Close();
            }
            catch (Exception ex)
            {
                Globe.WriteLog("Configuration.ReadConfig: " + ex.Message);
            }
        }

        private void InitializeRootDir()
        {
            if (Globe.ConfigDic.ContainsKey(Globe.CONFIG_ROOT_DIR))
            {
                Globe.ConfigDic.TryGetValue(Globe.CONFIG_ROOT_DIR, out Globe.RootDir);

                if (!Directory.Exists(Globe.RootDir))
                {
                    Directory.CreateDirectory(Globe.RootDir);
                }
            }
        }

        private void InitializeTime()
        {
            if (Globe.ConfigDic.ContainsKey(Globe.CONFIG_WORK_TIME))
            {
                string workTime = "";
                Globe.ConfigDic.TryGetValue(Globe.CONFIG_ROOT_DIR, out workTime);

                try
                {
                    string[] temp = workTime.Split(':');
                    Globe.WorkTime_Hour = int.Parse(temp[0]);
                    Globe.WorkTime_Minute = int.Parse(temp[1]);
                }
                catch (Exception ex)
                {
                    Globe.WriteLog("Configuration.InitializeTime: " + ex.Message);
                }

            }
        }

        private void InitializeLeagues()
        {
            Globe.LeaguesDic.Clear();
            string leagues = "";

            if (Globe.ConfigDic.ContainsKey(Globe.CONFIG_LEAGUES))
            {
                Globe.ConfigDic.TryGetValue(Globe.CONFIG_LEAGUES, out leagues);
            }

            string[] temp = leagues.Split(',');

            for (int i = 0; i < temp.Length; i = i + 2)
            {
                Globe.LeaguesDic.Add(temp[i], temp[i + 1]);
            }
        }
    }
}
