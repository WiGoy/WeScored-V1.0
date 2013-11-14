using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace WhoScoredReader
{
    class Program
    {
        private const long IncorrectFileSize = 5 * 1024;

        static void Main(string[] args)
        {
            string leagueName = "Italy_SerieA";
            string dir = Globe.RootDir + leagueName + @"\";

            DirectoryInfo directory = new DirectoryInfo(dir);
            FileInfo[] fileInfos = directory.GetFiles();

            if (fileInfos.Length > 0)
            {
                foreach (FileInfo file in fileInfos)
                {
                    if (file.Length < IncorrectFileSize)
                        continue;

                    string fileName = Path.GetFileNameWithoutExtension(file.FullName);

                    if (fileName.Equals("LiveScores"))
                        continue;

                    string htmlContent = LoadFile(file.FullName);

                    ContentFilter filter = new ContentFilter();
                    MatchInfo matchInfo = filter.GetMatchInfo(int.Parse(fileName), leagueName, htmlContent);

                    DAL dal = new DAL();
                    dal.InsertData(matchInfo);
                }
            }

            Console.WriteLine("End!");
            Console.ReadKey();
        }

        private static string LoadFile(string fileName)
        {
            string htmlContent = "";

            try
            {
                StreamReader sr = new StreamReader(fileName, Encoding.Default);
                htmlContent = sr.ReadToEnd();
            }
            catch (Exception) { }

            return htmlContent;
        }
    }
}
