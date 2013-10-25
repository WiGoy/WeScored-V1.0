using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhoscoredSpider
{
    class Configuration
    {
        public void SetConfiguration()
        {
            ReadConfig();
            initializeRootDir();
            initializeLeagues();
        }

        private void ReadConfig()
        {
            Globe.ConfigDic.Clear();

            try
            {
                StreamReader iniReader = new StreamReader(Globe.configFile, Encoding.Default);

                string line = "";

                while (line != null)
                {
                    line = iniReader.ReadLine();

                    if (line != null && !line.Equals("") && !line.StartsWith("//"))
                    {
                        string[] temp = line.Split('=');
                        Globe.ConfigDic.Add(temp[0].Trim(), temp[1].Trim());
                    }
                }

                iniReader.Close();
            }
            catch (Exception) { }
        }

        private void initializeRootDir()
        {
            if (Globe.ConfigDic.ContainsKey(Globe.CONFIG_ROOT_DIR))
            {
                Globe.ConfigDic.TryGetValue(Globe.CONFIG_ROOT_DIR, out Globe.RootDir);
            }
        }

        private void initializeLeagues()
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
                Globe.LeaguesDic.Add(temp[i].Trim(), temp[i + 1].Trim());
            }
        }
    }
}
