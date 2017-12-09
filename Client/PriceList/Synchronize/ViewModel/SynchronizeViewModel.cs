using System;
using System.Linq;
using System.Text;
using Common.Data.Notifier;
using Common.Messenger;
using Common.ViewModel.Command;
using Domain.DomainContext;
using Domain.ViewModel;
using Load.Service;
using Load.Service.Implementation;
using Web.Service;
using Web.Service.Implementation;
using Web.WebServiceReference;

namespace Synchronize.ViewModel
{
    public class SynchronizeViewModel : Notifier, IControlViewModel
    {
        private string message;
        private readonly IWebService webService;
        private readonly ILoadService loadService;

        public SynchronizeViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            webService = new WebService(domainContext?.OptionService);
            loadService = new LoadService(domainContext?.DataService, webService);
            CreateCommand();
        }

        public IDomainContext DomainContext { get; }

        public IMessenger Messenger => DomainContext?.Messenger;

        public bool ReadOnly { get; set; }

        public bool Enabled { get; set; }

        public bool IsEditControl { get; set; }

        public bool HasChanges { get; }

        public Action RefreshView { get; set; }

        public Action<bool> SetEnabled { get; set; }

        public Func<bool> HasResultGridErrors { get; set; }

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand SynchronizeCommand { get; private set; }

        private void CreateCommand()
        {
            SynchronizeCommand = new DelegateCommand(DoSynchronize, CanDoSynchronize);
        }

        private void DoSynchronize(object obj)
        {
            StringBuilder builder = new StringBuilder(Message);
            //ShortcutInfo shortcutInfo = webService.Shortcut();
            DateTimeOffset now =  DateTimeOffset.Now;
            //int period = (now - shortcutInfo.RequestTime).Milliseconds;
            //int timeToServer = (shortcutInfo.ResponceTime - shortcutInfo.RequestTime).Milliseconds;
            //int timeToClient = (now - shortcutInfo.ResponceTime).Milliseconds;
            //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - RequestTime: {shortcutInfo.RequestTime:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}");
            //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - ResponceTime: {shortcutInfo.ResponceTime:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}");
            //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Ping to Server: {timeToServer} ms");
            //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Ping to Client: {timeToClient} ms");
            //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Common Ping: {period} ms");
            //string responce = webService.CheckPassword() ? "OK" : "False";
            //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Password is  {responce}");
            //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Brand is  {webService.GetBrandInfo(shortcutInfo.Id)?.Name}");
            //builder.AppendLine(" ");
            //Message =  builder.ToString();
            Message = "Начало обновления...";
            CountInfo countInfo = webService.PrepareToUpdate(now, false);
            builder = new StringBuilder(Message);
            builder.AppendLine($"Необходимо обновить {571} брендов");
            Message = builder.ToString();
            BrandInfo brandInfo;
            for (long i = 1; i < 572; i++)
            {
                brandInfo = webService.GetBrandInfo(i);
                Message = Message + Environment.NewLine +
                          $"[{DateTimeOffset.Now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Добавлен бренд: \"{brandInfo.Name}\"";
                loadService.DownLoadBrandItem(brandInfo, now);
            }

            Message = Message + Environment.NewLine +
                          $"[{DateTimeOffset.Now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Ну вот пока и все...";
        }

        private bool CanDoSynchronize(object arg)
        {
            return true; // TODO Проверить наличие Internet (ShortCut)
        }

        public void ApplySearchCriteria()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Add()
        {
            throw new NotImplementedException();
        }

        public void View()
        {
            throw new NotImplementedException();
        }

        public void Edit()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}
