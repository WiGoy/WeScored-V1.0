using System;
using System.Collections.Generic;

namespace WhoScoredAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            DAL dal = new DAL();
            dal.GetPlayerStatistics();

            Console.ReadKey();
        }
    }
}
