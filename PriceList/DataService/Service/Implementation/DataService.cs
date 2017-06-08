using System.Data;
using System.Runtime.InteropServices;
using System.Timers;
using Common.Domain;
using DataService.Model;

namespace DataService.Service.Implementation
{
    public class DataService : IDataService
    {
        private ILogService logService;
        private DBContext dbContext;
        private long attempt;
        private readonly Timer timer;
        private readonly Timer secondTimer;
        private double interval;


        #region Constructors

        public DataService(ILogService logService)
        {
            this.logService = logService;
            attempt = 0;
            interval = 10;
            timer = new Timer {Enabled = false, Interval = interval, AutoReset = false};
            timer.Stop();
            timer.Elapsed += Timer_Elapsed;
            secondTimer = new Timer { Enabled = false, Interval = 1000, AutoReset = true };
            secondTimer.Stop();
            secondTimer.Elapsed += SecondTimer_Elapsed;
        }

        #endregion

        public DBContext DBContext
        {
            get
            {
                if (dbContext == null)
                {
                    dbContext = new DBContext();
                }

                Open();

                return dbContext;
            }
        }

        public void Open()
        {
            WaitToConnect();

            if (DBContext.Database.Connection.State != ConnectionState.Open)
            {
                logService.SendMessage("Connect to/create DataBse.", MessageType.Info, MessageLevel.Middle);
                attempt++;
                DBContext.Database.Connection.Open();
                WaitToConnect();
                if (DBContext.Database.Connection.State == ConnectionState.Open)
                {
                    attempt = 0;
                    logService.SendMessage($"SQL-Server Name = \"{dbContext.Database.Connection.DataSource}\"");
                    logService.SendMessage($"SQL-Server Name = \"{dbContext.Database.Connection.DataSource}\"");
                    logService.SendMessage($"SQL-Server Version = \"{dbContext.Database.Connection.ServerVersion}\"");
                    logService.SendMessage($"DataBase Name = \"{dbContext.Database.Connection.Database}\"");
                }
                else
                {
                    logService.SendMessage($"Can not conected to DataBase Name = \"{dbContext.Database.Connection.Database}\"");
                    timer.Enabled = true;
                    interval = (attempt > 6 ? 60 : attempt * 10) * 1000;
                    timer.Interval = interval;
                    timer.Start();
                    secondTimer.Enabled = true;
                }
            }
        }

        private void WaitToConnect()
        {
            while (DBContext.Database.Connection.State == ConnectionState.Connecting ||
                   DBContext.Database.Connection.State == ConnectionState.Executing ||
                   DBContext.Database.Connection.State == ConnectionState.Fetching) { }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            secondTimer.Enabled = false;
            Open();
        }
        private void SecondTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            interval -= 1000;
            int time = interval < 0 ? 0 : (int) interval/1000;
            logService.SendMessage($"Attempt {attempt} connections to the database after {time} seconds", MessageType.Error, MessageLevel.High);
        }
    }
}
