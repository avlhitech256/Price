﻿using System;
using System.Linq;
using System.Text;
using System.Windows;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Messenger;
using Common.Service;
using Common.ViewModel.Command;
using DatabaseService.DataService;
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
        private string lastUpdate;
        private bool needPhopos;
        private double valueBrands;
        private double maxBrands;
        private readonly IWebService webService;
        private readonly ILoadService loadService;
        private string brandLabel;
        private bool canDoSynchronize;

        public SynchronizeViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            webService = new WebService(domainContext?.OptionService);
            loadService = new LoadService(domainContext?.DataService, webService);
            CreateCommand();
            RefreshLastUpdate();
            InitProperties();
            canDoSynchronize = true;
        }

        public IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        public IMessenger Messenger => DomainContext?.Messenger;

        private IAsyncOperationService AsyncOperationService => DomainContext?.AsyncOperationService;

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

        public string LastUpdate
        {
            get
            {
                return lastUpdate;
            }
            set
            {
                if (lastUpdate != value)
                {
                    lastUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool NeedPhopos
        {
            get
            {
                return needPhopos;
            }
            set
            {
                if (needPhopos != value)
                {
                    needPhopos = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxBrands
        {
            get
            {
                return maxBrands;
            }
            set
            {
                if (Math.Abs(maxBrands - value) > double.Epsilon)
                {
                    maxBrands = value;
                    OnPropertyChanged();

                    if (maxBrands > 0)
                    {
                        StepLoadBrandInfo();
                    }
                }
            }
        }

        private void StepLoadBrandInfo()
        {
            if (MaxBrands > ValueBrands)
            {
                canDoSynchronize = false;
                SynchronizeCommand.RiseCanExecute();
            }

            SetBrandsLabel();
            AsyncOperationService.PerformAsyncOperation(AsyncOperationType.LoadFromWeb, 
                                                        LoadBrandInfo, 
                                                        ValueBrands,
                                                        SaveBrandToDatabase);
        }

        private BrandInfo LoadBrandInfo(double id)
        {
            long brandId = (long) id;
            return webService.GetBrandInfo(brandId);
        }

        private void SaveBrandToDatabase(Exception e, BrandInfo brandInfo)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                loadService.DownLoadBrandItem(brandInfo);
                Message = Message + Environment.NewLine +
                    $"[{DateTimeOffset.Now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Добавлен бренд: \"{brandInfo.Name}\"";
                ValueBrands++;
                SetBrandsLabel();
                
                if (Math.Abs(MaxBrands - ValueBrands) < double.Epsilon)
                {
                    canDoSynchronize = true;
                    SynchronizeCommand.RiseCanExecute();
                }
            });
        }

        public double ValueBrands
        {
            get
            {
                return valueBrands;
            }
            set
            {
                if (Math.Abs(valueBrands - value) > double.Epsilon)
                {
                    valueBrands = value;
                    OnPropertyChanged();

                    if (value < MaxBrands)
                    {
                        StepLoadBrandInfo();
                    }
                }
            }
        }

        public string BrandLabel
        {
            get
            {
                return brandLabel;
            }
            set
            {
                if (brandLabel != value)
                {
                    brandLabel = value;
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
            canDoSynchronize = false;
            SynchronizeCommand.RiseCanExecute();
            //DateTimeOffset date = DataService.GetLastUpdate();
            //CountInfo countInfo = webService.PrepareToUpdate(date, NeedPhopos);
            ValueBrands = 0;
            MaxBrands = 572;//countInfo.CountBrands;
            //bool isNotEnd = true;
            //do
            //{
            //    Brands brands = webService.GetBrands(date);
            //    isNotEnd = brands.Count <= brands.Items.Length;
            //    loadService.DownLoadBrands(brands);
            //    ValueBrands += brands.Items.Length;
            //} while (isNotEnd);

            //for (long i = 1; i < 572; i++)
            //{
            //    BrandInfo brandInfo = webService.GetBrandInfo(i);
            //    Message = Message + Environment.NewLine +
            //              $"[{DateTimeOffset.Now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Добавлен бренд: \"{brandInfo.Name}\"";
            //    loadService.DownLoadBrandItem(brandInfo);
            //}


            //canDoSynchronize = true;
            //SynchronizeCommand.RiseCanExecute();
        }

        //private void DoSynchronize(object obj)
        //{
        //    StringBuilder builder = new StringBuilder(Message);
        //    //ShortcutInfo shortcutInfo = webService.Shortcut();
        //    DateTimeOffset now =  DateTimeOffset.Now;
        //    //int period = (now - shortcutInfo.RequestTime).Milliseconds;
        //    //int timeToServer = (shortcutInfo.ResponceTime - shortcutInfo.RequestTime).Milliseconds;
        //    //int timeToClient = (now - shortcutInfo.ResponceTime).Milliseconds;
        //    //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - RequestTime: {shortcutInfo.RequestTime:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}");
        //    //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - ResponceTime: {shortcutInfo.ResponceTime:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}");
        //    //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Ping to Server: {timeToServer} ms");
        //    //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Ping to Client: {timeToClient} ms");
        //    //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Common Ping: {period} ms");
        //    //string responce = webService.CheckPassword() ? "OK" : "False";
        //    //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Password is  {responce}");
        //    //builder.AppendLine($"[{now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Brand is  {webService.GetBrandInfo(shortcutInfo.Id)?.Name}");
        //    //builder.AppendLine(" ");
        //    //Message =  builder.ToString();
        //    Message = "Начало обновления...";
        //    CountInfo countInfo = webService.PrepareToUpdate(now, false);
        //    builder = new StringBuilder(Message);
        //    builder.AppendLine($"Необходимо обновить {571} брендов");
        //    Message = builder.ToString();
        //    BrandInfo brandInfo;
        //    for (long i = 1; i < 572; i++)
        //    {
        //        brandInfo = webService.GetBrandInfo(i);
        //        Message = Message + Environment.NewLine +
        //                  $"[{DateTimeOffset.Now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Добавлен бренд: \"{brandInfo.Name}\"";
        //        loadService.DownLoadBrandItem(brandInfo);
        //    }

        //    Message = Message + Environment.NewLine +
        //                  $"[{DateTimeOffset.Now:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}] - Ну вот пока и все...";
        //}

        private void RefreshLastUpdate()
        {
            if (DataService != null)
            {
                LastUpdate = $"Дата последнего обновления: {DataService.GetLastUpdate()}";
            }
        }

        private void InitProperties()
        {
            SetBrandsLabel();
        }

        private void SetBrandsLabel()
        {
            Application.Current.Dispatcher.Invoke(() => BrandLabel = $"Бренды {ValueBrands} из {maxBrands}");
            
        }

        private bool CanDoSynchronize(object arg)
        {
            return canDoSynchronize; // TODO Проверить наличие Internet (ShortCut)
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