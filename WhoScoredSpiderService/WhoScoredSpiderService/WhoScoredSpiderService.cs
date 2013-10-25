using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

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
            Configuration config = new Configuration();
            config.GetConfiguration();
        }

        protected override void OnStop()
        {
        }
    }
}
