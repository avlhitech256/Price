using System.ServiceProcess;

namespace PricelistLoadService
{
    public partial class PricelistLoaderService : ServiceBase
    {
        public PricelistLoaderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
