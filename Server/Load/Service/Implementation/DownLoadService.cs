using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Annotations;
using Common.Data.Enum;
using Common.Service.Implementation;
using DataBase.Context;
using DataBase.Context.Entities;
using DataBase.Objects;
using DataBase.Service;
using DataBase.Service.Implementation;
using File.Service;
using File.Service.Implementation;
using Json.Contract;
using Option.Service;
using CommodityDirection = Common.Data.Enum.CommodityDirection;
using Directory = Json.Contract.Directory;

namespace Load.Service.Implementation
{
    public class DownLoadService : IDownLoadService
    {
        #region Members

        private readonly Timer timer;
        private readonly LoadingThreadInfo loadingThreadInfo;
        private readonly MovingThreadInfo movingThreadInfo;
        private readonly IFileService fileService;
        private bool isLoading;
        private readonly IOptionService optionService;
        private readonly IDataService dataService;

        #endregion

        #region Constructors

        public DownLoadService(uint timeOut, uint dueTime, uint period, List<MovingQueueItem> movingQueue, IOptionService optionService)
        {
            isLoading = false;
            this.optionService = optionService;
            dataService = new DataService();
            movingThreadInfo = new MovingThreadInfo(timeOut, dueTime, period, movingQueue);
            movingThreadInfo.StartMoving += StartMovingFiles;
            movingThreadInfo.Completed += MovingCompleted;
            movingThreadInfo.TimeOutIsOver += MovingTimeOutIsOver;
            loadingThreadInfo = new LoadingThreadInfo();
            loadingThreadInfo.Start += (sender, args) => Stop();
            loadingThreadInfo.End += (sender, args) => Start();
            timer = new Timer(LoadData, loadingThreadInfo, Timeout.Infinite, Timeout.Infinite);
            fileService = new FileService();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Start()
        {
            timer.Change(movingThreadInfo.DueTime, movingThreadInfo.Period);
        }

        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void LoadData(object info)
        {
            LoadingThreadInfo threadInfo = info as LoadingThreadInfo;

            if (loadingThreadInfo != null)
            {
                LoadData(threadInfo);
            }
        }

        private void LoadData(LoadingThreadInfo threadInfo)
        {
            if (!threadInfo.IsLoading)
            {
                movingThreadInfo.MovingInfo.MovedFiles.Clear();
                movingThreadInfo.MovingInfo.WaitingFiles.Clear();
                movingThreadInfo.MovingInfo.ExceptionFiles.Clear();
                movingThreadInfo.Start = null;
                fileService.AsyncLoadFiles(movingThreadInfo);
            }
        }

        private void StartMovingFiles(object sender, EventArgs args)
        {
            loadingThreadInfo.IsLoading = true;
            //************************************
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Start moving files...", DateTime.Now);
            Console.WriteLine("-------------------------------------------------------------------------");
            //************************************
        }

        private void MovingCompleted(object sender, EventArgs e)
        {
            LoadDataToDatabase(sender);
            BackupToArchive(sender);
            loadingThreadInfo.IsLoading = false;
            //************************************
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  End moving files", DateTime.Now);
            Console.WriteLine("-------------------------------------------------------------------------");
            //************************************
        }

        private void BackupToArchive(object sender)
        {
            var movingThreadInfo = sender as MovingThreadInfo;

            if (movingThreadInfo != null)
            {
                string subArcPath = movingThreadInfo.Start.HasValue
                    ? movingThreadInfo.Start.Value.ToString("yyyyMMddTHHmmss") + "\\"
                    : string.Empty;
                string archivePath = optionService.ArcSourcePath + subArcPath;
                fileService.MoveFiles(optionService.WorkingSourcePath,
                    archivePath,
                    optionService.SourcePatterns);
                fileService.MoveFiles(optionService.WorkingSourcePath + optionService.SubDirForPhoto,
                    archivePath + optionService.SubDirForPhoto,
                    optionService.PhotoPatterns);
            }
        }

        private void MovingTimeOutIsOver(object sender, EventArgs e)
        {
            SaveMovedErrors(sender);
            MovingCompleted(sender, e);
        }

        private void LoadDataToDatabase(object sender)
        {
            JsonLoadData jsonLoadData = JsonLoadData();
            var threadInfo = sender as MovingThreadInfo;
            DateTimeOffset startMoving = threadInfo != null && threadInfo.Start.HasValue
                ? threadInfo.Start.Value
                : DateTimeOffset.Now;
            LoadToDatabase(jsonLoadData, startMoving);
        }

        private void LoadToDatabase(JsonLoadData jsonLoadData, DateTimeOffset loadUpdateTime)
        {
            try
            {
                // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

                CreateBrandItems(jsonLoadData, loadUpdateTime);
                CreatePriceGroupItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
                CreateTypeOfPriceItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
                CreateDirectoryItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
                CreateProductDirection(dataService.DataBaseContext, loadUpdateTime);
                CreateNomenclatureGroupItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
                CreateCommodityDirectionsItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
                LoadPictures(dataService.DataBaseContext,
                             optionService.WorkingSourcePath + optionService.SubDirForPhoto,
                             optionService.PhotoPatterns, loadUpdateTime);
                CreateCatalogItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
                CreateContragentItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);

                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
            }
            catch (Exception e)
            {
                ; //TODO Записать ошибку в LOG-file
            }
        }

        private void CreateContragentItems(DataBaseContext dataBaseContext, 
                                           JsonLoadData jsonLoadData, DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.Clients != null && jsonLoadData.Clients.Any())
            {
                int countItems = 0;

                jsonLoadData.Clients.ForEach(
                    x =>
                    {
                        ContragentItemEntity oldContragentItem = GetContragentItem(dataBaseContext, x.UID);

                        if (oldContragentItem != null)
                        {
                            Update(oldContragentItem, x, dataBaseContext, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            ContragentItemEntity newContragentItem = Assemble(dataBaseContext, x, loadUpdateTime);

                            if (newContragentItem != null)
                            {
                                dataBaseContext.ContragentItemEntities.Add(newContragentItem);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception e)
                {
                    ;
                }
            }
        }

        private ContragentItemEntity Assemble(DataBaseContext dataBaseContext, Client jsonItem, DateTimeOffset loadUpdateTime)
        {
            ContragentItemEntity entity = null;
            Guid code;

            if (jsonItem.UID.ConvertToGuid(out code))
            {
                entity = new ContragentItemEntity();

                PriceInfo mutualSettlementsPriceInfo = GetPriceInfoOfMutualSettlements(jsonItem);
                PriceInfo pdzPriceInfo = GetPriceInfoOfPDZ(jsonItem);

                entity.UID = code;
                entity.Code = jsonItem.Code;
                entity.Name = jsonItem.Name;
                entity.Login = jsonItem.Login;
                entity.MutualSettlements = mutualSettlementsPriceInfo.Price;
                entity.MutualSettlementsCurrency = mutualSettlementsPriceInfo.Currency;
                entity.PDZ = pdzPriceInfo.Price;
                entity.PDZCurrency = pdzPriceInfo.Currency;
                UpdateDiscounts(entity, jsonItem, dataBaseContext, loadUpdateTime);
                UpdatePriceTypePriceGroupContragents(entity, jsonItem, dataBaseContext, loadUpdateTime);
                UpdatePriceTypeNomenclatureGroupContragents(entity, jsonItem, dataBaseContext, loadUpdateTime);

                entity.DateOfCreation = loadUpdateTime;
                entity.ForceUpdated = loadUpdateTime;
                entity.LastUpdated = loadUpdateTime;
            }

            return entity;
        }

        private void Update(ContragentItemEntity entity, Client jsonItem,
                            DataBaseContext dataBaseContext, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && jsonItem != null)
            {
                if (Equals(entity, jsonItem, dataBaseContext))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    PriceInfo mutualSettlementsPriceInfo = GetPriceInfoOfMutualSettlements(jsonItem);
                    PriceInfo pdzPriceInfo = GetPriceInfoOfPDZ(jsonItem);

                    entity.Code = jsonItem.Code;
                    entity.Name = jsonItem.Name;
                    entity.Login = jsonItem.Login;
                    entity.MutualSettlements = mutualSettlementsPriceInfo.Price;
                    entity.MutualSettlementsCurrency = mutualSettlementsPriceInfo.Currency;
                    entity.PDZ = pdzPriceInfo.Price;
                    entity.PDZCurrency = pdzPriceInfo.Currency;
                    UpdateDiscounts(entity, jsonItem, dataBaseContext, loadUpdateTime);
                    UpdatePriceTypePriceGroupContragents(entity, jsonItem, dataBaseContext, loadUpdateTime);
                    UpdatePriceTypeNomenclatureGroupContragents(entity, jsonItem, dataBaseContext, loadUpdateTime);
                    
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private void UpdatePriceTypeNomenclatureGroupContragents(ContragentItemEntity entity, 
                                                                 Client jsonItem,
                                                                 DataBaseContext dataBaseContext, 
                                                                 DateTimeOffset loadUpdateTime)
        {
            var priceTypeNomenclatureGroupContragentItems = new List<DirectPriceTypeNomenclatureGroupContragent>();

            foreach (TypeOfNomenclature typeOfNomenclature in jsonItem.PriceTypeNomenclatureGroup)
            {
                bool validConvertCode = true;

                Guid nomenclatureGroupCode;

                if (!typeOfNomenclature.NomenclatureGroupUID.ConvertToGuid(out nomenclatureGroupCode))
                {
                    //TODO Вывести в лог об ошибочном GUID
                    validConvertCode = false;
                }

                Guid typeOfPriceCode;

                if (!typeOfNomenclature.PriceTypeUID.ConvertToGuid(out typeOfPriceCode))
                {
                    //TODO Вывести в лог об ошибочном GUID
                    validConvertCode = false;
                }

                if (validConvertCode)
                {
                    var directPriceTypeNomenclatureGroupContragent =
                        new DirectPriceTypeNomenclatureGroupContragent(typeOfPriceCode, nomenclatureGroupCode);
                    priceTypeNomenclatureGroupContragentItems.Add(directPriceTypeNomenclatureGroupContragent);
                }
                else
                {
                    //TODO Вывести сообщение о невозможности создать запись в БазеДанных с такими кодами
                }
            }

            List<PriceTypeNomenclatureGroupContragentEntity> needToUpdate = null;

            if (entity.PriceTypeNomenclatureGroups == null)
            {
                entity.PriceTypeNomenclatureGroups = new List<PriceTypeNomenclatureGroupContragentEntity>();
            }

            try
            {
                entity.PriceTypeNomenclatureGroups.RemoveAll(
                    e =>
                        !priceTypeNomenclatureGroupContragentItems
                            .Any(d => e.TypeOfPriceItem.Code == d.TypeOfPriceCode ||
                                      e.NomenclatureGroupItem.Code == d.NomenclatureGroupCode));
                needToUpdate = entity.PriceTypeNomenclatureGroups.ToList();

            }
            catch (Exception e)
            {
                // TODO Запись в LOG-file ошибок работы с базой данных
            }

            if (needToUpdate != null)
            {
                needToUpdate.ForEach(e => e.ForceUpdated = loadUpdateTime);

                List<DirectPriceTypeNomenclatureGroupContragent> needToCreate =
                    priceTypeNomenclatureGroupContragentItems
                        .Where(d => !needToUpdate.Any(e => e.TypeOfPriceItem.Code == d.TypeOfPriceCode ||
                                                          e.NomenclatureGroupItem.Code == d.NomenclatureGroupCode))
                        .ToList();

                needToCreate.ForEach(d => Assemble(entity, d, dataBaseContext, loadUpdateTime));
            }
        }

        private void Assemble(ContragentItemEntity contragentItem,
                      DirectPriceTypeNomenclatureGroupContragent directPriceTypeNomenclatureGroupContragentItem,
                      DataBaseContext dataBaseContext, DateTimeOffset loadUpdateTime)
        {
            if (contragentItem != null && directPriceTypeNomenclatureGroupContragentItem != null)
            {
                bool validFoundEntities = true;
                var entity = new PriceTypeNomenclatureGroupContragentEntity();

                NomenclatureGroupEntity nomenclatureGroupItem = null;

                try
                {
                    nomenclatureGroupItem = dataBaseContext.NomenclatureGroupEntities.FirstOrDefault(
                        e => e.Code == directPriceTypeNomenclatureGroupContragentItem.NomenclatureGroupCode);
                }
                catch (Exception e)
                {
                    // TODO Запись в LOG-file ошибок работы с базой данных
                }

                if (nomenclatureGroupItem == null)
                {
                    validFoundEntities = false;
                    //TODO Вывести сообщение что не найдена сущность
                }

                TypeOfPriceItemEntity typeOfPriceItem = null;

                try
                {
                    typeOfPriceItem = dataBaseContext.TypeOfPriceItemEntities.FirstOrDefault(
                        e => e.Code == directPriceTypeNomenclatureGroupContragentItem.TypeOfPriceCode);
                }
                catch (Exception e)
                {
                    // TODO Запись в LOG-file ошибок работы с базой данных
                }

                if (typeOfPriceItem == null)
                {
                    validFoundEntities = false;
                    //TODO Вывести сообщение что не найдена сущность
                }

                if (validFoundEntities)
                {
                    entity.NomenclatureGroupItem = nomenclatureGroupItem;
                    entity.TypeOfPriceItem = typeOfPriceItem;
                    entity.ContragentItem = contragentItem;
                    entity.ForceUpdated = loadUpdateTime;
                    entity.DateOfCreation = loadUpdateTime;
                    entity.LastUpdated = loadUpdateTime;

                    if (contragentItem.PriceTypeNomenclatureGroups == null)
                    {
                        contragentItem.PriceTypeNomenclatureGroups =
                            new List<PriceTypeNomenclatureGroupContragentEntity>();
                    }

                    contragentItem.PriceTypeNomenclatureGroups.Add(entity);

                    if (typeOfPriceItem.PriceTypeNomenclatureGroups == null)
                    {
                        typeOfPriceItem.PriceTypeNomenclatureGroups =
                            new List<PriceTypeNomenclatureGroupContragentEntity>();
                    }

                    typeOfPriceItem.PriceTypeNomenclatureGroups.Add(entity);

                    if (nomenclatureGroupItem.PriceTypeNomenclatureGroups == null)
                    {
                        nomenclatureGroupItem.PriceTypeNomenclatureGroups =
                            new List<PriceTypeNomenclatureGroupContragentEntity>();
                    }

                    nomenclatureGroupItem.PriceTypeNomenclatureGroups.Add(entity);
                    dataBaseContext.PriceTypeNomenclatureGroupContragentEntities.Add(entity);
                }
                else
                {
                    ; //TODO Вывести сообщение о невозможности создания сущности со следующими кодами
                }
            }
        }

        private void UpdatePriceTypePriceGroupContragents(ContragentItemEntity entity, Client jsonItem,
                                                          DataBaseContext dataBaseContext, DateTimeOffset loadUpdateTime)
        {
            var priceTypePriceGroupContragentItems = new List<DirectPriceTypePriceGroupContragent>();

            foreach (TypesOfPrices typesOfPricese in jsonItem.PriceTypePriceGroup)
            {
                bool validConvertCode = true;

                Guid priceGroupCode;

                if (!typesOfPricese.PriceGroupUID.ConvertToGuid(out priceGroupCode))
                {
                    //TODO Вывести в лог об ошибочном GUID
                    validConvertCode = false;
                }

                Guid typeOfPriceCode;

                if (!typesOfPricese.PriceTypeUID.ConvertToGuid(out typeOfPriceCode))
                {
                    //TODO Вывести в лог об ошибочном GUID
                    validConvertCode = false;
                }

                if (validConvertCode)
                {
                    var directPriceTypePriceGroupContragent = 
                        new DirectPriceTypePriceGroupContragent(typeOfPriceCode, priceGroupCode);
                    priceTypePriceGroupContragentItems.Add(directPriceTypePriceGroupContragent);
                }
                else
                {
                    //TODO Вывести сообщение о невозможности создать запись в БазеДанных с такими кодами
                }
            }

            List<PriceTypePriceGroupContragentEntity> needToUpdate = null;

            if (entity.PriceTypePriceGroups == null)
            {
                entity.PriceTypePriceGroups = new List<PriceTypePriceGroupContragentEntity>();
            }

            try
            {
                entity.PriceTypePriceGroups.RemoveAll(
                    e =>
                        !priceTypePriceGroupContragentItems
                            .Any(d => e.TypeOfPriceItem.Code == d.TypeOfPriceCode &&
                                      e.PriceGroupItem.Code == d.PriceGroupCode));
                needToUpdate = entity.PriceTypePriceGroups.ToList();

            }
            catch (Exception e)
            {
                // TODO Запись в LOG-file ошибок работы с базой данных
            }

            if (needToUpdate != null)
            {
                needToUpdate.ForEach(e => e.ForceUpdated = loadUpdateTime);
                List<DirectPriceTypePriceGroupContragent> needToCreate =
                    priceTypePriceGroupContragentItems
                        .Where(d => !needToUpdate.Any(e => e.TypeOfPriceItem.Code == d.TypeOfPriceCode &&
                                                           e.PriceGroupItem.Code == d.PriceGroupCode))
                        .ToList();

                needToCreate.ForEach(d => Assemble(entity, d, dataBaseContext, loadUpdateTime));
            }

        }

        private void Assemble(ContragentItemEntity contragentItem, 
                              DirectPriceTypePriceGroupContragent directPriceTypePriceGroupContragentItem,
                              DataBaseContext dataBaseContext, DateTimeOffset loadUpdateTime)
        {
            if (contragentItem != null && directPriceTypePriceGroupContragentItem != null)
            {
                bool validFoundEntities = true;
                var entity = new PriceTypePriceGroupContragentEntity();

                PriceGroupItemEntity priceGroupItem = null;

                try
                {
                    priceGroupItem = dataBaseContext.PriceGroupItemEntities.FirstOrDefault(
                        e => e.Code == directPriceTypePriceGroupContragentItem.PriceGroupCode);
                }
                catch (Exception e)
                {
                    // TODO Запись в LOG-file ошибок работы с базой данных
                }

                if (priceGroupItem == null)
                {
                    validFoundEntities = false;
                    //TODO Вывести сообщение что не найдена сущность
                }

                TypeOfPriceItemEntity typeOfPriceItem = null;

                try
                {
                    typeOfPriceItem = dataBaseContext.TypeOfPriceItemEntities.FirstOrDefault(
                        e => e.Code == directPriceTypePriceGroupContragentItem.TypeOfPriceCode);
                }
                catch (Exception e)
                {
                    // TODO Запись в LOG-file ошибок работы с базой данных
                }

                if (typeOfPriceItem == null)
                {
                    validFoundEntities = false;
                    //TODO Вывести сообщение что не найдена сущность
                }

                if (validFoundEntities)
                {
                    entity.PriceGroupItem = priceGroupItem;
                    entity.TypeOfPriceItem = typeOfPriceItem;
                    entity.ContragentItem = contragentItem;
                    entity.ForceUpdated = loadUpdateTime;
                    entity.DateOfCreation = loadUpdateTime;
                    entity.LastUpdated = loadUpdateTime;

                    if (contragentItem.PriceTypePriceGroups == null)
                    {
                        contragentItem.PriceTypePriceGroups =
                            new List<PriceTypePriceGroupContragentEntity>();
                    }

                    contragentItem.PriceTypePriceGroups.Add(entity);

                    if (typeOfPriceItem.PriceTypePriceGroups == null)
                    {
                        typeOfPriceItem.PriceTypePriceGroups =
                            new List<PriceTypePriceGroupContragentEntity>();
                    }

                    typeOfPriceItem.PriceTypePriceGroups.Add(entity);

                    if (priceGroupItem.PriceTypePriceGroups == null)
                    {
                        priceGroupItem.PriceTypePriceGroups =
                            new List<PriceTypePriceGroupContragentEntity>();
                    }

                    priceGroupItem.PriceTypePriceGroups.Add(entity);
                    dataBaseContext.PriceTypePriceGroupContragentEntities.Add(entity);
                }
                else
                {
                    //TODO Вывести сообщение о невозможности создания сущности со следующими кодами
                }
            }
        }

        private void UpdateDiscounts(ContragentItemEntity entity, Client jsonItem,
                                     DataBaseContext dataBaseContext, DateTimeOffset loadUpdateTime)
        {
            List<DirectDiscount> discounts = new List<DirectDiscount>();

            if (entity.Discounts == null)
            {
                entity.Discounts = new List<DiscountsContragentEntity>();
            }

            foreach (Discount discount in jsonItem.Discounts)
            {
                var priceInfo = new PriceInfo();
                PriceInfoParse(discount.Rate, priceInfo);
                decimal rate = priceInfo.Price;

                foreach (string stringCode in discount.Nomenclature)
                {
                    Guid code;

                    if (stringCode.ConvertToGuid(out code))
                    {
                        var directDiscount = new DirectDiscount(code, rate);
                        discounts.Add(directDiscount);
                    }
                }
            }

            List<Guid> jsonItemCodes = discounts.Select(x => x.NomenclatureCode).ToList();
            entity.Discounts?.RemoveAll(x => jsonItemCodes.All(j => x.CatalogItem.UID != j));
            List<DiscountsContragentEntity> needToUpdate = entity.Discounts?.ToList();

            discounts.ForEach(
                x =>
                {
                    DiscountsContragentEntity discountEntity =
                        needToUpdate?.FirstOrDefault(e => e.CatalogItem.UID == x.NomenclatureCode);

                    if (discountEntity != null)
                    {
                        Update(discountEntity, x, loadUpdateTime);
                    }
                    else
                    {
                        CatalogItemEntity catalogItem =
                            dataBaseContext.CatalogItemEntities.FirstOrDefault(e => e.UID == x.NomenclatureCode);

                        if (catalogItem != null)
                        {
                            discountEntity = Assemble(entity, catalogItem, x, loadUpdateTime);
                            dataBaseContext.DiscountsContragentEntities.Add(discountEntity);
                        }
                        else
                        {
                            //TODO Записать в лог что не найден нужный элемент номенклатуры
                        }
                    }
                });

        }

        private void Update(DiscountsContragentEntity entity, DirectDiscount discount, DateTimeOffset loadUpdateTime)
        {
            if (Equals(entity, discount))
            {
                entity.ForceUpdated = loadUpdateTime;
            }
            else
            {
                entity.Rate = discount.Rate;
                entity.LastUpdated = loadUpdateTime;
            }
        }

        //private bool Equals(DiscountsContragentEntity entity, DirectDiscount discount)
        //{
        //    return entity.Rate == discount.Rate;
        //}

        private DiscountsContragentEntity Assemble(ContragentItemEntity contragentItem, 
                                                   CatalogItemEntity catalogItem, 
                                                   DirectDiscount discount,
                                                   DateTimeOffset loadUpdateTime)
        {
            DiscountsContragentEntity entity = new DiscountsContragentEntity();
            entity.CatalogItem = catalogItem;
            entity.ContragentItem = contragentItem;
            entity.Rate = discount.Rate;
            entity.DateOfCreation = loadUpdateTime;
            entity.ForceUpdated = loadUpdateTime;
            entity.LastUpdated = loadUpdateTime;

            if (contragentItem.Discounts == null)
            {
                contragentItem.Discounts = new List<DiscountsContragentEntity>();
            }

            contragentItem.Discounts.Add(entity);

            if (catalogItem.Discounts == null)
            {
                catalogItem.Discounts = new List<DiscountsContragentEntity>();
            }

            catalogItem.Discounts.Add(entity);

            return entity;
        }

        private bool Equals(ContragentItemEntity entity, Client jsonItem, DataBaseContext dataBaseContext)
        {
            PriceInfo mutualSettlementsPriceInfo = GetPriceInfoOfMutualSettlements(jsonItem);
            PriceInfo pdzPriceInfo = GetPriceInfoOfPDZ(jsonItem);

            return Equals(entity.UID, jsonItem.UID) &&
                   entity.Code == jsonItem.Code &&
                   entity.Name == jsonItem.Name &&
                   entity.Login == jsonItem.Login &&
                   entity.MutualSettlements == mutualSettlementsPriceInfo.Price &&
                   entity.MutualSettlementsCurrency == mutualSettlementsPriceInfo.Currency &&
                   entity.PDZ == pdzPriceInfo.Price &&
                   entity.PDZCurrency == pdzPriceInfo.Currency &&
                   Equals(entity.PriceTypePriceGroups, jsonItem.PriceTypePriceGroup) &&
                   Equals(entity.Discounts, jsonItem.Discounts) &&
                   Equals(entity.PriceTypeNomenclatureGroups, jsonItem.PriceTypeNomenclatureGroup);
        }

        private bool Equals(IEnumerable<PriceTypeNomenclatureGroupContragentEntity> firstPriceTypeNomenclatureGroups, 
                            IEnumerable<TypeOfNomenclature> secondPriceTypeNomenclatureGroups)
        {
            List<PriceTypeNomenclatureGroupContragentEntity> firstPriceTypeNomenclatureGroupsList =
                firstPriceTypeNomenclatureGroups as List<PriceTypeNomenclatureGroupContragentEntity> ??
                firstPriceTypeNomenclatureGroups?.ToList();
            List<TypeOfNomenclature> secondPriceTypeNomenclatureGroupsList =
                secondPriceTypeNomenclatureGroups as List<TypeOfNomenclature> ??
                secondPriceTypeNomenclatureGroups?.ToList();

            return firstPriceTypeNomenclatureGroupsList != null && secondPriceTypeNomenclatureGroupsList != null &&
                   firstPriceTypeNomenclatureGroupsList.Count == secondPriceTypeNomenclatureGroupsList.Count &&
                   firstPriceTypeNomenclatureGroupsList.All(e => secondPriceTypeNomenclatureGroupsList.Any(j => Equals(e, j))) &&
                   secondPriceTypeNomenclatureGroupsList.All(j => firstPriceTypeNomenclatureGroupsList.Any(e => Equals(e, j)));

        }

        private bool Equals(PriceTypeNomenclatureGroupContragentEntity firstPriceTypeNomenclatureGroup,
            TypeOfNomenclature secondPriceTypeNomenclatureGroup)
        {
            return firstPriceTypeNomenclatureGroup != null && secondPriceTypeNomenclatureGroup != null &&
                   Equals(firstPriceTypeNomenclatureGroup.NomenclatureGroupItem.Code,
                       secondPriceTypeNomenclatureGroup.NomenclatureGroupUID) &&
                   Equals(firstPriceTypeNomenclatureGroup.TypeOfPriceItem.Code,
                       secondPriceTypeNomenclatureGroup.PriceTypeUID);
        }

        private bool Equals(IEnumerable<DiscountsContragentEntity> firstDiscounts, IEnumerable<Discount> secondDiscounts)
        {
            List<DiscountsContragentEntity> firstDiscountsList = 
                firstDiscounts as List<DiscountsContragentEntity> ?? firstDiscounts?.ToList();
            List<DirectDiscount> seconDirectDiscounts = secondDiscounts.SelectMany(Convert).ToList();


            return firstDiscountsList != null &&
                   firstDiscountsList.Count == seconDirectDiscounts.Count &&
                   firstDiscountsList.All(e => seconDirectDiscounts.Any(j => Equals(e, j))) &&
                   seconDirectDiscounts.All(j => firstDiscountsList.Any(e => Equals(e, j)));

        }

        private IEnumerable<DirectDiscount> Convert(Discount discount)
        {
            IEnumerable<DirectDiscount> discounts = new List<DirectDiscount>();

            if (discount != null)
            {
                decimal rate = GetPriceInfoOfDiscount(discount).Price;
                discounts = discount.Nomenclature
                    .Select(x =>
                    {
                        Guid code;

                        return x.ConvertToGuid(out code) ? new DirectDiscount(code, rate) : null;
                    })
                    .Where(x => x != null);
            }

            return discounts;
        } 

        private bool Equals(DiscountsContragentEntity firstDiscount, DirectDiscount secondDiscount)
        {
            return firstDiscount != null && secondDiscount != null &&
                   Equals(firstDiscount.CatalogItem.UID, secondDiscount.NomenclatureCode) &&
                   firstDiscount.Rate == secondDiscount.Rate;

        }

        private bool Equals(IEnumerable<PriceTypePriceGroupContragentEntity> firstPriceTypePriceGroups,
                            IEnumerable<TypesOfPrices> secondPriceTypePriceGroups)
        {
            List<PriceTypePriceGroupContragentEntity> firstPriceTypePriceGroupsList =
                firstPriceTypePriceGroups as List<PriceTypePriceGroupContragentEntity> ??
                firstPriceTypePriceGroups?.ToList();
            List<TypesOfPrices> secondPriceTypePriceGroupsList =
                secondPriceTypePriceGroups as List<TypesOfPrices> ??
                secondPriceTypePriceGroups?.ToList();
            return firstPriceTypePriceGroupsList != null &&
                   secondPriceTypePriceGroupsList != null &&
                   firstPriceTypePriceGroupsList.Count == secondPriceTypePriceGroupsList.Count &&
                   firstPriceTypePriceGroupsList.All(e => secondPriceTypePriceGroupsList.Any(j => Equals(e, j))) &&
                   secondPriceTypePriceGroupsList.All(j => firstPriceTypePriceGroupsList.Any(e => Equals(e, j)));
        }

        private bool Equals(PriceTypePriceGroupContragentEntity firstPriceTypePriceGroup,
                            TypesOfPrices secondPriceTypePriceGroup)
        {
            return firstPriceTypePriceGroup != null &&
                   secondPriceTypePriceGroup != null &&
                   Equals(firstPriceTypePriceGroup.PriceGroupItem.Code, secondPriceTypePriceGroup.PriceGroupUID) &&
                   Equals(firstPriceTypePriceGroup.TypeOfPriceItem.Code, secondPriceTypePriceGroup.PriceTypeUID);
        }

        private bool Equals(IEnumerable<Guid> firstCodes, IEnumerable<string> secondStringCodes)
        {
            List<Guid> firstListCodes = firstCodes as List<Guid> ?? firstCodes?.ToList();
            List<string> secondListStrings = secondStringCodes as List<string> ?? secondStringCodes?.ToList();
            return firstListCodes != null && secondListStrings != null &&
                   firstListCodes.Count == secondListStrings.Count &&
                   firstListCodes.All(c => secondListStrings.Any(s => Equals(c, s))) &&
                   secondListStrings.All(s => firstListCodes.Any(c => Equals(c, s)));
        }

        private bool Equals(Guid firstCode, string secondStringCode)
        {
            Guid code;
            return !string.IsNullOrWhiteSpace(secondStringCode) &&
                   secondStringCode.ConvertToGuid(out code) &&
                   firstCode == code;
        }

        private void CreateCatalogItems(DataBaseContext dataBaseContext, 
                                        JsonLoadData jsonLoadData,
                                        DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.Nomenclatures != null && jsonLoadData.Nomenclatures.Any())
            {
                int countItems = 0;

                jsonLoadData.Nomenclatures.ForEach(
                    x =>
                    {
                        CatalogItemEntity oldCatalogItem = GetCatalogItem(dataBaseContext, x.UID);

                        if (oldCatalogItem != null)
                        {
                            Update(oldCatalogItem, x, dataBaseContext, loadUpdateTime);
                            CreateTypeOfPricesNomenclatureItems(oldCatalogItem, dataBaseContext, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            CatalogItemEntity newCatalogItem = Assemble(dataBaseContext, x, loadUpdateTime);

                            if (newCatalogItem != null)
                            {
                                dataBaseContext.CatalogItemEntities.Add(newCatalogItem);
                                CreateTypeOfPricesNomenclatureItems(newCatalogItem, dataBaseContext, x, loadUpdateTime);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception e)
                {
                    ;
                }
            }
        }

        private void Update(CatalogItemEntity entity, Nomenclature jsonItem, 
                            DataBaseContext dataBaseContext, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && jsonItem != null)
            {
                if (Equals(entity, jsonItem, dataBaseContext))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    BrandItemEntity brandItem = GetBrandItem(dataBaseContext, jsonItem);
                    PriceGroupItemEntity priceGroupItem = GetPriceGroupItem(dataBaseContext, jsonItem);
                    DirectoryEntity directoryItem = GetDirectoryItem(dataBaseContext, jsonItem);
                    NomenclatureGroupEntity nomenclatureGroupItem =
                        GetNomenclatureGroupItem(dataBaseContext, jsonItem);
                    List<CommodityDirectionEntity> commodityDirectionItemsForCatalogItem =
                        GetCommodityDirection(dataBaseContext, jsonItem);
                    //PriceInfo priceInfo = GetPriceInfo(jsonItem);
                    List<PhotoItemEntity> photoItems = GetPhotoItems(dataBaseContext, jsonItem);
                    List<string> needToCreatePhotos = GetNeedToCreatePhotos(photoItems, jsonItem);
                    photoItems.AddRange(CreateEmptyPhotos(dataBaseContext, needToCreatePhotos));

                    entity.Name = jsonItem.Name;
                    entity.Code = jsonItem.Code;
                    entity.Article = jsonItem.VendorCode;
                    entity.Name = jsonItem.Name;
                    entity.Brand = brandItem;
                    entity.BrandName = brandItem?.Name;
                    entity.Unit = jsonItem.Measure;
                    entity.EnterpriceNormPack = jsonItem.NormPackaging;
                    entity.BatchOfSales = jsonItem.BatchOfSales.ConvertToDecimal();
                    entity.Balance = jsonItem.InStock;
                    //entity.Price = priceInfo.Price;
                    //entity.Currency = priceInfo.Currency;
                    entity.Multiplicity = 0;
                    entity.HasPhotos = photoItems.Any(x => x.IsLoad);
                    entity.Photos = photoItems;
                    entity.DateOfCreation = jsonItem.DateOfCreation.ConvertToDateTimeOffset();
                    entity.Status = GenerateStatus();
                    entity.Directory = directoryItem;
                    entity.NomenclatureGroup = nomenclatureGroupItem;
                    entity.CommodityDirection = commodityDirectionItemsForCatalogItem;
                    entity.PriceGroup = priceGroupItem;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(CatalogItemEntity entity, Nomenclature jsonItem, DataBaseContext dataBaseContext)
        {
            Guid code;
            BrandItemEntity brandItem = GetBrandItem(dataBaseContext, jsonItem);
            PriceGroupItemEntity priceGroupItem = GetPriceGroupItem(dataBaseContext, jsonItem);
            DirectoryEntity directoryItem = GetDirectoryItem(dataBaseContext, jsonItem);
            NomenclatureGroupEntity nomenclatureGroupItem =
                GetNomenclatureGroupItem(dataBaseContext, jsonItem);
            List<CommodityDirectionEntity> commodityDirectionItemsForCatalogItem =
                GetCommodityDirection(dataBaseContext, jsonItem);
            //PriceInfo priceInfo = GetPriceInfo(jsonItem);
            List<PhotoItemEntity> photoItems = GetPhotoItems(dataBaseContext, jsonItem);
            List<string> needToCreatePhotos = GetNeedToCreatePhotos(photoItems, jsonItem);
            photoItems.AddRange(CreateEmptyPhotos(dataBaseContext, needToCreatePhotos));

            return jsonItem.UID.ConvertToGuid(out code) &&
                   entity.UID == code &&
                   entity.Code == jsonItem.Code &&
                   entity.Article == jsonItem.VendorCode &&
                   entity.Name == jsonItem.Name &&
                   entity.Brand == brandItem &&
                   entity.BrandName == brandItem?.Name &&
                   entity.Unit == jsonItem.Measure &&
                   entity.EnterpriceNormPack == jsonItem.NormPackaging &&
                   entity.BatchOfSales == jsonItem.BatchOfSales.ConvertToDecimal() &&
                   entity.Balance == jsonItem.InStock &&
                   //entity.Price == priceInfo.Price &&
                   //entity.Currency == priceInfo.Currency &&
                   entity.Multiplicity == 0 && //??????????????????????????????????????????????????
                   entity.HasPhotos == photoItems.Any(x => x.IsLoad) &&
                   Equals(entity.Photos.Select(x => x.Id), photoItems.Select(x => x.Id)) &&
                   entity.DateOfCreation == jsonItem.DateOfCreation.ConvertToDateTimeOffset() &&
                   entity.Status == GenerateStatus() &&
                   entity.Directory.Id == directoryItem.Id &&
                   entity.NomenclatureGroup.Id == nomenclatureGroupItem.Id &&
                   Equals(entity.CommodityDirection.Select(x => x.Id), 
                          commodityDirectionItemsForCatalogItem.Select(x => x.Id)) &&
                   entity.PriceGroup.Id == priceGroupItem.Id;
        }

        private bool Equals(IEnumerable<long> firstIds, IEnumerable<long> secondIds)
        {
            List<long> firstListIds = firstIds as List<long> ?? firstIds?.ToList();
            List<long> secondListIds = secondIds as List<long> ?? secondIds?.ToList();

            return firstListIds != null && secondListIds != null &&
                   firstListIds.Count == secondListIds.Count &&
                   firstListIds.All(secondListIds.Contains) &&
                   secondListIds.All(firstListIds.Contains);
        }

        [NotNull]
        private CatalogItemEntity Assemble(DataBaseContext dataBaseContext,
                                           Nomenclature nomenclature,
                                           DateTimeOffset loadUpdateTime)
        {
            BrandItemEntity brandItem = GetBrandItem(dataBaseContext, nomenclature);
            PriceGroupItemEntity priceGroupItem = GetPriceGroupItem(dataBaseContext, nomenclature);
            DirectoryEntity directoryItem = GetDirectoryItem(dataBaseContext, nomenclature);
            NomenclatureGroupEntity nomenclatureGroupItem =
                GetNomenclatureGroupItem(dataBaseContext, nomenclature);
            List<CommodityDirectionEntity> commodityDirectionItemsForCatalogItem =
                GetCommodityDirection(dataBaseContext, nomenclature);
            //PriceInfo priceInfo = GetPriceInfo(nomenclature);
            List<PhotoItemEntity> photoItems = GetPhotoItems(dataBaseContext, nomenclature);
            List<string> needToCreatePhotos = GetNeedToCreatePhotos(photoItems, nomenclature);
            photoItems.AddRange(CreateEmptyPhotos(dataBaseContext, needToCreatePhotos));

            var catalogItem = new CatalogItemEntity
            {

                UID = nomenclature.UID.ConvertToGuid(),
                Code = nomenclature.Code,
                Article = nomenclature.VendorCode,
                Name = nomenclature.Name,
                Brand = brandItem,
                BrandName = brandItem?.Name,
                Unit = nomenclature.Measure,
                EnterpriceNormPack = nomenclature.NormPackaging,
                BatchOfSales = nomenclature.BatchOfSales.ConvertToDecimal(),
                Balance = nomenclature.InStock,
                //Price = priceInfo.Price,
                //Currency = priceInfo.Currency,
                Multiplicity = 0,
                HasPhotos = photoItems.Any(x => x.IsLoad),
                Photos = photoItems,
                DateOfCreation = nomenclature.DateOfCreation.ConvertToDateTimeOffset(),
                LastUpdated = loadUpdateTime,
                ForceUpdated = loadUpdateTime,
                Status = GenerateStatus(),
                LastUpdatedStatus = loadUpdateTime,
                Directory = directoryItem,
                NomenclatureGroup = nomenclatureGroupItem,
                CommodityDirection = commodityDirectionItemsForCatalogItem,
                PriceGroup = priceGroupItem
            };

            if (brandItem != null)
            {
                if (brandItem.CatalogItems == null)
                {
                    brandItem.CatalogItems = new List<CatalogItemEntity>();
                }

                brandItem.CatalogItems.Add(catalogItem);
            }

            if (priceGroupItem != null)
            {
                if (priceGroupItem.CatalogItems == null)
                {
                    priceGroupItem.CatalogItems = new List<CatalogItemEntity>();
                }

                priceGroupItem.CatalogItems.Add(catalogItem);
            }

            if (directoryItem != null)
            {
                if (directoryItem.CatalogItems == null)
                {
                    directoryItem.CatalogItems = new List<CatalogItemEntity>();
                }

                directoryItem.CatalogItems.Add(catalogItem);
            }

            if (nomenclatureGroupItem != null)
            {
                if (nomenclatureGroupItem.CatalogItems == null)
                {
                    nomenclatureGroupItem.CatalogItems = new List<CatalogItemEntity>();
                }

                nomenclatureGroupItem.CatalogItems.Add(catalogItem);
            }

            if (commodityDirectionItemsForCatalogItem.Any())
            {
                commodityDirectionItemsForCatalogItem.ForEach(
                    x =>
                    {
                        if (x != null)
                        {
                            if (x.CatalogItems == null)
                            {
                                x.CatalogItems = new List<CatalogItemEntity>();
                            }

                            x.CatalogItems.Add(catalogItem);
                        }
                    });
            }

            if (photoItems.Any())
            {
                photoItems.ForEach(
                    x =>
                    {
                        if (x != null)
                        {
                            x.CatalogItem = catalogItem;
                        }
                    });
            }

            return catalogItem;
        }

        private void LoadPictures(DataBaseContext dataBaseContext, string photoPath, 
                                  string[] photoSearchPattern, DateTimeOffset loadUpdateTime)
        {
            FileInfo[] fileInfos = fileService.GetFileInfos(photoPath, photoSearchPattern);

            if (dataBaseContext != null && fileInfos.Any())
            {
                int countOfFiles = 0;

                fileInfos.Where(x => !string.IsNullOrWhiteSpace(x.FullName)).ToList().ForEach(
                    x =>
                    {
                        string fileName = fileService.GetFileName(x.Name);
                        byte[] picture = fileService.ReadByteArrayPicture(x.FullName);
                        PhotoItemEntity oldPhotoItem = GetPhotoItem(dataBaseContext, fileName);

                        if (oldPhotoItem != null)
                        {
                            Update(oldPhotoItem, fileName, picture, loadUpdateTime);
                            countOfFiles++;
                        }
                        else
                        {
                            PhotoItemEntity photoItem = Assemble(fileName, picture, loadUpdateTime);
                            dataBaseContext.PhotoItemEntities.Add(photoItem);
                            countOfFiles++;
                        }

                        if (countOfFiles % 10 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private PhotoItemEntity Assemble(string fileName, byte[] picture, DateTimeOffset loadUpdateTime)
        {
            PhotoItemEntity item = new PhotoItemEntity
            {
                Name = fileName,
                IsLoad = picture != null && picture.Length > 0,
                Photo = picture,
                DateOfCreation = loadUpdateTime,
                LastUpdated = loadUpdateTime,
                ForceUpdated = loadUpdateTime
            };

            return item;
        }

        private void Update(PhotoItemEntity entity, string fileName, byte[] picture, DateTimeOffset loadUpdateTime)
        {
            if (entity != null)
            {
                if (Equals(entity, fileName, picture))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Photo = picture;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(PhotoItemEntity entity, string fileName, byte[] picture)
        {
            return entity != null && !string.IsNullOrWhiteSpace(fileName) && entity.Name == fileName &&
                   ((entity.IsLoad && picture != null && entity.Photo != null && picture.Length == entity.Photo.Length) ||
                    (!entity.IsLoad && (entity.Photo == null || entity.Photo != null && entity.Photo.Length == 0)));
        }

        private PhotoItemEntity GetPhotoItem(DataBaseContext dataBaseContext, string fileName)
        {
            PhotoItemEntity photoItem = dataBaseContext.PhotoItemEntities.FirstOrDefault(x => x.Name == fileName);
            return photoItem;
        }

        private CatalogItemStatus GenerateStatus()
        {
            int miliseconds = DateTime.Now.Millisecond;

            CatalogItemStatus status = CatalogItemStatus.Old;

            if (miliseconds % 7 == 0)
            {
                status = CatalogItemStatus.PriceIsDown;
            }
            else if (miliseconds % 5 == 0)
            {
                status = CatalogItemStatus.PriceIsUp;
            }
            else if (miliseconds % 2 == 0)
            {
                status = CatalogItemStatus.New;
            }

            return status;
        }

        private List<PhotoItemEntity> GetPhotoItems(DataBaseContext dataBaseContext,
                                                    Nomenclature nomenclature)
        {
            List<PhotoItemEntity> photoItems = new List<PhotoItemEntity>();

            if (dataBaseContext != null && nomenclature != null && nomenclature.Photos.Any())
            {
                photoItems = dataBaseContext
                    .PhotoItemEntities
                    .Where(x => nomenclature.Photos.Any(p => p == x.Name)).ToList();
            }

            return photoItems;
        }

        private List<string> GetNeedToCreatePhotos(List<PhotoItemEntity> photoItems, Nomenclature nomenclature)
        {
            List<string> needToCreatePhotos = new List<string>();

            if (nomenclature?.Photos != null) // photoItems != null && photoItems.Any()
            {
                needToCreatePhotos = nomenclature.Photos.Where(x => photoItems == null ||
                                                                    !photoItems.Any() ||
                                                                    photoItems.All(p => p.Name != x)).ToList();
            }

            return needToCreatePhotos;
        }
        private IEnumerable<PhotoItemEntity> CreateEmptyPhotos(DataBaseContext dataBaseContext,
                                                               List<string> photos)
        {
            var emptyPhotos = new List<PhotoItemEntity>();

            if (photos != null)
            {
                emptyPhotos = photos
                    .Select(x => new PhotoItemEntity {Name = x, IsLoad = false, Photo = null})
                    .ToList();
                dataBaseContext.PhotoItemEntities.AddRange(emptyPhotos);
            }

            return emptyPhotos;
        }

        //private PriceInfo GetPriceInfo(Nomenclature nomenclature)
        //{
        //    var priceInfo = new PriceInfo();

        //    if (nomenclature?.TypesOfPrices != null && nomenclature.TypesOfPrices.Any())
        //    {
        //        PriceTypeItem typeOfPrice = nomenclature.TypesOfPrices.FirstOrDefault();
        //        priceInfo = GetPriceInfo(typeOfPrice);
        //    }

        //    return priceInfo;
        //}

        private PriceInfo GetPriceInfo(PriceTypeItem typeOfPrice)
        {
            var priceInfo = new PriceInfo();

            if (typeOfPrice != null)
            {
                PriceInfoParse(typeOfPrice.Price, priceInfo);
            }

            return priceInfo;
        }

        private PriceInfo GetPriceInfoOfPDZ(Client client)
        {
            var priceInfo = new PriceInfo();

            if (client != null)
            {
                PriceInfoParse(client.PDZ, priceInfo);
            }

            return priceInfo;
        }

        private PriceInfo GetPriceInfoOfMutualSettlements(Client client)
        {
            var priceInfo = new PriceInfo();

            if (client != null)
            {
                PriceInfoParse(client.MutualSettlements, priceInfo);
            }

            return priceInfo;
        }

        private PriceInfo GetPriceInfoOfDiscount(Discount discount)
        {
            var priceInfo = new PriceInfo();

            if (discount != null)
            {
                PriceInfoParse(discount.Rate, priceInfo);
            }

            return priceInfo;
        }

        private void PriceInfoParse(string priceWithCurrency, PriceInfo priceInfo)
        {
            if (!string.IsNullOrWhiteSpace(priceWithCurrency))
            {
                string[] splitPriceString = priceWithCurrency.Split(' ');

                if (splitPriceString.Any())
                {
                    string stringPrice = splitPriceString[0];

                    if (!string.IsNullOrWhiteSpace(stringPrice))
                    {
                        decimal price;

                        if (decimal.TryParse(stringPrice, out price))
                        {
                            priceInfo.Price = price;

                            if (splitPriceString.Length >= 2)
                            {
                                priceInfo.Currency = splitPriceString[1];
                            }
                        }
                        else
                        {
                            if (splitPriceString.Length >= 2)
                            {
                                priceInfo.Currency = splitPriceString[1];
                            }
                            else
                            {
                                priceInfo.Currency = splitPriceString[0];
                            }
                        }
                    }
                    else if (splitPriceString.Length >= 2)
                    {
                        priceInfo.Currency = splitPriceString[1];
                    }
                }
            }
        }

        private ContragentItemEntity GetContragentItem(DataBaseContext dataBaseContext, string code)
        {
            ContragentItemEntity contragentItem = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                contragentItem = GetContragentItem(dataBaseContext, guidCode);
            }

            return contragentItem;
        }

        private ContragentItemEntity GetContragentItem(DataBaseContext dataBaseContext, Guid code)
        {
            ContragentItemEntity catalogItem = dataBaseContext.ContragentItemEntities.FirstOrDefault(x => x.UID == code);
            return catalogItem;
        }

        private CatalogItemEntity GetCatalogItem(DataBaseContext dataBaseContext, string code)
        {
            CatalogItemEntity catalogItem = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                catalogItem = GetCatalogItem(dataBaseContext, guidCode);
            }

            return catalogItem;
        }

        private CatalogItemEntity GetCatalogItem(DataBaseContext dataBaseContext, Guid code)
        {
            CatalogItemEntity catalogItem = dataBaseContext.CatalogItemEntities.FirstOrDefault(x => x.UID == code);
            return catalogItem;
        }

        private BrandItemEntity GetBrandItem(DataBaseContext dataBaseContext,
                                             Nomenclature nomenclature)
        {
            BrandItemEntity brandItem = null;

            if (dataBaseContext != null && nomenclature != null)
            {
                brandItem = GetBrandItem(dataBaseContext, nomenclature.BrandUID);
            }

            return brandItem;
        }

        private BrandItemEntity GetBrandItem(DataBaseContext dataBaseContext, string code)
        {
            BrandItemEntity brandItem = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                brandItem = GetBrandItem(dataBaseContext, guidCode);
            }
            
            return brandItem;
        }

        private BrandItemEntity GetBrandItem(DataBaseContext dataBaseContext, Guid code)
        {
            BrandItemEntity brandItem = dataBaseContext.BrandItemEntities.FirstOrDefault(x => x.Code == code);
            return brandItem;
        }

        private TypeOfPriceItemEntity GetTypeOfPriceItem(DataBaseContext dataBaseContext, string code)
        {
            TypeOfPriceItemEntity typeOfPriceItem = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                typeOfPriceItem = GetTypeOfPriceItem(dataBaseContext, guidCode);
            }

            return typeOfPriceItem;
        }

        private TypeOfPriceItemEntity GetTypeOfPriceItem(DataBaseContext dataBaseContext, Guid code)
        {
            TypeOfPriceItemEntity typeOfPriceItem = dataBaseContext.TypeOfPriceItemEntities.FirstOrDefault(x => x.Code == code);
            return typeOfPriceItem;
        }

        private PriceGroupItemEntity GetPriceGroupItem(DataBaseContext dataBaseContext,
                                                       Nomenclature nomenclature)
        {
            PriceGroupItemEntity brandItem = null;

            if (dataBaseContext != null && nomenclature != null)
            {
                brandItem = GetPriceGroupItem(dataBaseContext, nomenclature.PriceGroupUID);
            }

            return brandItem;
        }

        private PriceGroupItemEntity GetPriceGroupItem(DataBaseContext dataBaseContext, string code)
        {
            PriceGroupItemEntity priceGroupItemItem = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                priceGroupItemItem = GetPriceGroupItem(dataBaseContext, guidCode);
            }

            return priceGroupItemItem;
        }

        private PriceGroupItemEntity GetPriceGroupItem(DataBaseContext dataBaseContext, Guid code)
        {
            PriceGroupItemEntity priceGroupItemItem = dataBaseContext.PriceGroupItemEntities.FirstOrDefault(x => x.Code == code);
            return priceGroupItemItem;
        }

        private DirectoryEntity GetDirectoryItem(DataBaseContext dataBaseContext, string code)
        {
            DirectoryEntity directory = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                directory = GetDirectoryItem(dataBaseContext, guidCode);
            }

            return directory;
        }

        private DirectoryEntity GetDirectoryItem(DataBaseContext dataBaseContext, Guid code)
        {
            DirectoryEntity directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
            return directory;
        }

        private DirectoryEntity GetDirectoryItem(DataBaseContext dataBaseContext,
                                                 Nomenclature nomenclature)
        {
            DirectoryEntity directoryItem = null;

            if (dataBaseContext != null && nomenclature != null)
            {
                directoryItem = GetDirectoryItem(dataBaseContext, nomenclature.CatalogUID);
            }

            return directoryItem;
        }

        private NomenclatureGroupEntity GetNomenclatureGroupItem(DataBaseContext dataBaseContext,
                                                                 Nomenclature nomenclature)
        {
            NomenclatureGroupEntity nomenclatureGroupItem = null;

            if (dataBaseContext != null && nomenclature != null)
            {
                Guid nomenclatureGroupGuid;

                if (nomenclature.NomenclatureGroupUID.ConvertToGuid(out nomenclatureGroupGuid))
                {
                    nomenclatureGroupItem = dataBaseContext
                        .NomenclatureGroupEntities
                        .FirstOrDefault(x => x.Code == nomenclatureGroupGuid);
                }
            }

            return nomenclatureGroupItem;
        }

        private List<CommodityDirectionEntity> GetCommodityDirection(DataBaseContext dataBaseContext,
                                                                     Nomenclature nomenclature)
        {
            List<CommodityDirectionEntity> commodityDirectionItems = new List<CommodityDirectionEntity>();

            if (dataBaseContext != null &&
                nomenclature?.CommodityDirection != null &&
                nomenclature.CommodityDirection.Any())
            {
                List<Guid> commodityDirectionGuids = nomenclature
                    .CommodityDirection
                    .Where(IsGuid)
                    .Select(x => x.ConvertToGuid())
                    .ToList();

                if (commodityDirectionGuids.Any())
                {
                    commodityDirectionItems = dataBaseContext
                        .CommodityDirectionEntities.Where(x => commodityDirectionGuids.Any(c => c == x.Code))
                        .ToList();
                }
            }

            return commodityDirectionItems;
        }

        private bool IsGuid(string code)
        {
            Guid guid;
            return code.ConvertToGuid(out guid);
        }

        private void CreateCommodityDirectionsItems(DataBaseContext dataBaseContext, 
                                                    JsonLoadData jsonLoadData, 
                                                    DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.CommodityDirections != null && jsonLoadData.CommodityDirections.Any())
            {
                int countItems = 0;

                jsonLoadData.CommodityDirections.ForEach(
                    x =>
                    {
                        CommodityDirectionEntity oldCommodityDirection = GetCommodityDirection(dataBaseContext, x.UID);

                        if (oldCommodityDirection != null)
                        {
                            Update(oldCommodityDirection, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            CommodityDirectionEntity newCommodityDirection = Assemble(x);

                            if (newCommodityDirection != null)
                            {
                                dataBaseContext.CommodityDirectionEntities.Add(newCommodityDirection);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private void Update(CommodityDirectionEntity entity, Json.Contract.CommodityDirection item, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && item != null)
            {
                if (Equals(entity, item))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = item.Name;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(CommodityDirectionEntity entity, Json.Contract.CommodityDirection item)
        {
            Guid code;
            return entity != null && item != null &&
                   item.UID.ConvertToGuid(out code) && entity.Code == code &&
                   entity.Name == item.Name;
        }

        private CommodityDirectionEntity GetCommodityDirection(DataBaseContext dataBaseContext, string code)
        {
            CommodityDirectionEntity commodityDirection = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                commodityDirection = GetCommodityDirection(dataBaseContext, guidCode);
            }

            return commodityDirection;
        }

        private CommodityDirectionEntity GetCommodityDirection(DataBaseContext dataBaseContext, Guid code)
        {
            CommodityDirectionEntity commodityDirection =
                dataBaseContext.CommodityDirectionEntities.FirstOrDefault(x => x.Code == code);
            return commodityDirection;
        }


        private CommodityDirectionEntity Assemble(Json.Contract.CommodityDirection commodityDirection)
        {
            CommodityDirectionEntity commodityDirectionItem = null;

            if (commodityDirection != null)
            {
                var now = DateTimeOffset.Now;

                commodityDirectionItem = new CommodityDirectionEntity
                {

                    Code = commodityDirection.UID.ConvertToGuid(),
                    Name = commodityDirection.Name,
                    DateOfCreation = now,
                    LastUpdated = now,
                    ForceUpdated = now
                };
            }

            return commodityDirectionItem;
        }

        private void CreateNomenclatureGroupItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData, DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.NomenclatureGroups != null && jsonLoadData.NomenclatureGroups.Any())
            {
                int countItems = 0;

                jsonLoadData.NomenclatureGroups.ForEach(
                    x =>
                    {
                        NomenclatureGroupEntity oldNomenclatureGroup = GetNomenclatureGroup(dataBaseContext, x.UID);

                        if (oldNomenclatureGroup != null)
                        {
                            Update(oldNomenclatureGroup, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            NomenclatureGroupEntity newNomenclatureGroup = Assemble(x, loadUpdateTime);

                            if (newNomenclatureGroup != null)
                            {
                                dataBaseContext.NomenclatureGroupEntities.Add(newNomenclatureGroup);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private void Update(NomenclatureGroupEntity entity, NomenclatureGroup item, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && item != null)
            {
                if (Equals(entity, item))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = item.Name;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(NomenclatureGroupEntity entity, NomenclatureGroup item)
        {
            Guid code;
            return entity != null && item != null && 
                   item.UID.ConvertToGuid(out code) && entity.Code == code &&
                   entity.Name == item.Name;
        }

        private NomenclatureGroupEntity GetNomenclatureGroup(DataBaseContext dataBaseContext, string code)
        {
            NomenclatureGroupEntity nomenclatureGroup = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                nomenclatureGroup = GetNomenclatureGroup(dataBaseContext, guidCode);
            }

            return nomenclatureGroup;
        }

        private NomenclatureGroupEntity GetNomenclatureGroup(DataBaseContext dataBaseContext, Guid code)
        {
            NomenclatureGroupEntity nomenclatureGroup =
                dataBaseContext.NomenclatureGroupEntities.FirstOrDefault(x => x.Code == code);
            return nomenclatureGroup;
        }

        private NomenclatureGroupEntity Assemble(NomenclatureGroup nomenclatureGroup, DateTimeOffset loadUpdateTime)
        {
            NomenclatureGroupEntity nomenclatureGroupItem = null;

            if (nomenclatureGroup != null)
            {
                nomenclatureGroupItem = new NomenclatureGroupEntity
                {
                    Code = nomenclatureGroup.UID.ConvertToGuid(),
                    Name = nomenclatureGroup.Name,
                    DateOfCreation = loadUpdateTime,
                    LastUpdated = loadUpdateTime,
                    ForceUpdated = loadUpdateTime
                };
            }

            return nomenclatureGroupItem;
        }

        private void CreateProductDirection(DataBaseContext dataBaseContext, DateTimeOffset loadUpdateTime)
        {
            Dictionary< CommodityDirection, Guid> directionList = new Dictionary<CommodityDirection, Guid>
            {
                { CommodityDirection.Vaz,        Guid.Parse("1FF0EBCD-1507-40E9-A409-2AF3B8F77D49") },
                { CommodityDirection.Gaz,        Guid.Parse("9B6B4325-B435-4D65-925E-4921F09D461F") },
                { CommodityDirection.Zaz,        Guid.Parse("264D3368-ECA8-4F2F-9C3B-7B7CC582C7FD") },
                { CommodityDirection.Chemistry,  Guid.Parse("78DC5231-C9D8-49A1-9FC5-AD18BF71DC13") },
                { CommodityDirection.Battery,    Guid.Parse("C7667731-5CFE-4BB6-90DC-8520C87F4FA0") },
                { CommodityDirection.Gas,        Guid.Parse("A4151BE4-3D3F-4A18-A3D1-4BF48F03AB6C") },
                { CommodityDirection.Instrument, Guid.Parse("AFEDE7C9-85E0-4C60-A5FB-0D81B0771D3D") },
                { CommodityDirection.Common,     Guid.Parse("80135062-F8F6-4C5A-AFD2-48A0117EB2B6") }
            };

            try
            {
                foreach (KeyValuePair<CommodityDirection, Guid> item in directionList)
                {
                    ProcessingProductDirection(dataBaseContext, item.Key, item.Value, loadUpdateTime);
                }

                dataBaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                ;//throw;
            }
        }

        private void ProcessingProductDirection(DataBaseContext dataBaseContext, CommodityDirection commodityDirection,
                                                Guid directoryGuid, DateTimeOffset loadUpdateTime)
        {
            ProductDirectionEntity item = GetProductDirection(dataBaseContext, commodityDirection);

            if (item != null)
            {
                DirectoryEntity directory = GetDirectoryItem(dataBaseContext, directoryGuid);

                if (item.Directory != directory)
                {
                    item.Directory = directory;
                    item.LastUpdated = loadUpdateTime;
                }
            }
            else
            {
                CreateProductDirection(dataBaseContext, commodityDirection, directoryGuid, loadUpdateTime);
            }
        }

        private void CreateProductDirection(DataBaseContext dataBaseContext, CommodityDirection commodityDirection, 
                                            Guid directoryGuid, DateTimeOffset loadUpdateTime)
        {
            ProductDirectionEntity item = dataBaseContext.ProductDirectionEntities.Create();
            item.Direction = commodityDirection;
            item.DateOfCreation = loadUpdateTime;
            item.ForceUpdated = loadUpdateTime;
            item.LastUpdated = loadUpdateTime;
            item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == directoryGuid);
            dataBaseContext.ProductDirectionEntities.Add(item);
        }

        private ProductDirectionEntity GetProductDirection(DataBaseContext dataBaseContext, CommodityDirection commodityDirection)
        {
            ProductDirectionEntity productDirection =
                dataBaseContext.ProductDirectionEntities.FirstOrDefault(x => x.Direction == commodityDirection);
            return productDirection;
        }

        private void CreateDirectoryItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData, DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.Catalogs != null && jsonLoadData.Catalogs.Any())
            {
                int countItems = 0;

                jsonLoadData.Catalogs.ForEach(
                    x =>
                    {
                        DirectoryEntity oldDirectory = GetDirectoryItem(dataBaseContext, x.UID);

                        if (oldDirectory != null)
                        {
                            Update(dataBaseContext, oldDirectory, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            DirectoryEntity newDirectory = Assemble(dataBaseContext, x, loadUpdateTime);

                            if (newDirectory != null)
                            {
                                dataBaseContext.DirectoryEntities.Add(newDirectory);
                                countItems++;
                            }
                        }

                        if (countItems % 10 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private void Update(DataBaseContext dataBaseContext, DirectoryEntity entity, Directory jsonItem, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && jsonItem != null)
            {
                if (Equals(entity, jsonItem))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = jsonItem.Name;
                    entity.SubDirectory
                        .RemoveAll(x => jsonItem.Subdirectory.All(i =>
                        {
                            bool result = true;
                            Guid code;

                            if (i.UID.ConvertToGuid(out code))
                            {
                                result = x.Code != code;
                            }

                            return result;
                        }));

                    jsonItem.Subdirectory
                        .Where(x =>
                        {
                            bool result = false;
                            Guid code;

                            if (x.UID.ConvertToGuid(out code))
                            {
                                result = entity.SubDirectory.All(i => i.Code != code);
                            }

                            return result;
                        })
                        .ToList()
                        .ForEach(x =>
                        {
                            DirectoryEntity subDirectory = Assemble(dataBaseContext, x, loadUpdateTime);

                            if (subDirectory != null)
                            {
                                entity.SubDirectory.Add(subDirectory);
                            }
                        });

                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }


        private bool Equals(DirectoryEntity entity, Directory jsonItem)
        {
            Guid code;
            return jsonItem.UID.ConvertToGuid(out code) && entity.Code == code && 
                entity.Name == jsonItem.Name &&
                entity.SubDirectory.All(x => jsonItem.Subdirectory.Any(i =>
                {
                    bool result = false;
                    Guid codeSubdirectory;

                    if (i.UID.ConvertToGuid(out codeSubdirectory))
                    {
                        result = x.Code == code;
                    }

                    return result;
                })) &&
                jsonItem.Subdirectory.All(x => entity.SubDirectory.Any(i =>
                {
                    bool result = false;
                    Guid codeSubdirectory;

                    if (x.UID.ConvertToGuid(out codeSubdirectory))
                    {
                        result = i.Code == code;
                    }

                    return result;
                }));
        }

        private DirectoryEntity Assemble(DataBaseContext dataBaseContext, Directory directory, DateTimeOffset loadUpdateTime)
        {
            DirectoryEntity directoryItem = null;

            if (directory != null)
            {
                List<DirectoryEntity> subDirectories = null;

                if (directory.Subdirectory != null && directory.Subdirectory.Any())
                {
                    subDirectories = new List<DirectoryEntity>();
                    directory.Subdirectory.Where(x => x != null).ToList().ForEach(
                        x =>
                        {
                            DirectoryEntity subDir = GetDirectoryItem(dataBaseContext, x.UID);

                            if (subDir != null)
                            {
                                subDirectories.Add(subDir);
                                Update(dataBaseContext, subDir, x, loadUpdateTime);
                            }
                            else
                            {
                                subDirectories.Add(Assemble(dataBaseContext, x, loadUpdateTime));
                            }
                        });
                }

                directoryItem = new DirectoryEntity
                {
                    Code = directory.UID.ConvertToGuid(),
                    Name = directory.Name,
                    SubDirectory = subDirectories,
                    DateOfCreation = loadUpdateTime,
                    LastUpdated = loadUpdateTime,
                    ForceUpdated = loadUpdateTime
                };
            }

            return directoryItem;
        }


        private void CreateTypeOfPriceItems(DataBaseContext dataBaseContext,
                                            JsonLoadData jsonLoadData,
                                            DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.TypesOfPrices != null && jsonLoadData.TypesOfPrices.Any())
            {
                int countItems = 0;

                jsonLoadData.TypesOfPrices.ForEach(
                    x =>
                    {
                        TypeOfPriceItemEntity oldTypeOfPriceItem = GetTypeOfPriceItem(dataBaseContext, x.UID);

                        if (oldTypeOfPriceItem != null)
                        {
                            Update(oldTypeOfPriceItem, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            TypeOfPriceItemEntity newTypeOfPriceItem = Assemble(x, loadUpdateTime);

                            if (newTypeOfPriceItem != null)
                            {
                                dataBaseContext.TypeOfPriceItemEntities.Add(newTypeOfPriceItem);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private void Update(TypeOfPriceItemEntity entity, PriceType jsonItem, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && jsonItem != null)
            {
                if (Equals(entity, jsonItem))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = jsonItem.Name;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(TypeOfPriceItemEntity entity, PriceType jsonItem)
        {
            Guid code;
            return jsonItem.UID.ConvertToGuid(out code) && entity.Code == code && entity.Name == jsonItem.Name;
        }

        private TypeOfPriceItemEntity Assemble(PriceType typeOfPrice, DateTimeOffset loadUpdateTime)
        {
            TypeOfPriceItemEntity brandItem = null;

            if (typeOfPrice != null)
            {
                brandItem = new TypeOfPriceItemEntity
                {
                    Code = typeOfPrice.UID.ConvertToGuid(),
                    Name = typeOfPrice.Name,
                    DateOfCreation = loadUpdateTime,
                    ForceUpdated = loadUpdateTime,
                    LastUpdated = loadUpdateTime
                };
            }

            return brandItem;
        }

        private void CreateTypeOfPricesNomenclatureItems(CatalogItemEntity catalogItem,
                                                         DataBaseContext dataBaseContext,
                                                         Nomenclature jsonLoadData,
                                                         DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.TypesOfPrices != null)
            {
                if (catalogItem.TypeOfPriceItems == null)
                {
                    catalogItem.TypeOfPriceItems = new List<TypeOfPricesNomenclatureItemEntity>();
                }

                List<Guid> listCodes = new List<Guid>();

                foreach (PriceTypeItem typesOfPrice in jsonLoadData.TypesOfPrices)
                {
                    Guid code;

                    if (typesOfPrice.UID.ConvertToGuid(out code))
                    {
                        listCodes.Add(code);
                    }
                    else
                    {
                        //TODO Записать в LOG что невозможно преобразовать код
                    }
                }

                catalogItem.TypeOfPriceItems.RemoveAll(x => !listCodes.Contains(x.TypeOfPriceItem.Code));

                List<PriceTypeItem> needToUpdateItems =
                    jsonLoadData.TypesOfPrices
                        .Where(x => catalogItem.TypeOfPriceItems
                                        .Any(
                                            y =>
                                            {
                                                Guid code;
                                                return x.UID.ConvertToGuid(out code) && 
                                                       code.Equals(y.TypeOfPriceItem.Code);
                                            }))
                        .ToList();

                List<PriceTypeItem> needToCreateItems =
                    jsonLoadData.TypesOfPrices
                        .Where(x => catalogItem.TypeOfPriceItems
                                        .All(
                                            y =>
                                            {
                                                Guid code;
                                                return x.UID.ConvertToGuid(out code) && 
                                                       !code.Equals(y.TypeOfPriceItem.Code);
                                            }))
                        .ToList();

                int countItems = 0;

                needToUpdateItems.ForEach(
                    x =>
                    {
                        countItems++;
                        TypeOfPricesNomenclatureItemEntity entity =
                            catalogItem.TypeOfPriceItems.FirstOrDefault(
                                t =>
                                {
                                    Guid code;
                                    return x.UID.ConvertToGuid(out code) && !code.Equals(t.TypeOfPriceItem.Code);
                                });
                        Update(entity, x, loadUpdateTime);

                        if (countItems % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ;
                            }
                        }
                    });

                countItems = 0;

                needToCreateItems.ForEach(
                    x =>
                    {
                        Guid code;

                        if (x.UID.ConvertToGuid(out code))
                        {
                            TypeOfPriceItemEntity typeOfPriceItem = 
                                dataBaseContext.TypeOfPriceItemEntities.FirstOrDefault(t => t.Code == code);
                            TypeOfPricesNomenclatureItemEntity entity = Assemble(x, catalogItem, typeOfPriceItem, loadUpdateTime);
                            dataBaseContext.TypeOfPricesNomenclatureItemEntities.Add(entity);
                            countItems++;
                        }

                        if (countItems % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private void Update(TypeOfPricesNomenclatureItemEntity entity, PriceTypeItem jsonItem, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && jsonItem != null)
            {
                if (Equals(entity, jsonItem))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    Guid code;

                    if (jsonItem.UID.ConvertToGuid(out code))
                    {
                        entity.TypeOfPriceItem.Code = code;
                    }

                    PriceInfo priceInfo = GetPriceInfo(jsonItem);
                    entity.Price = priceInfo.Price;
                    entity.Currency = priceInfo.Currency;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(TypeOfPricesNomenclatureItemEntity entity, PriceTypeItem jsonItem)
        {
            bool result = false;

            if (entity != null && jsonItem != null)
            {
                Guid code;
                PriceInfo priceInfo = GetPriceInfo(jsonItem);
                result = jsonItem.UID.ConvertToGuid(out code) && entity.TypeOfPriceItem.Code == code &&
                       entity.Price == priceInfo.Price && entity.Currency == priceInfo.Currency;
            }

            return result;
        }

        private TypeOfPricesNomenclatureItemEntity Assemble(PriceTypeItem priceTypeItem,
                                                            CatalogItemEntity catalogItem,
                                                            TypeOfPriceItemEntity typeOfPrice,
                                                            DateTimeOffset loadUpdateTime)
        {
            TypeOfPricesNomenclatureItemEntity typeOfPricesNomenclatureItem = null;

            if (priceTypeItem != null && catalogItem != null && typeOfPrice != null)
            {
                PriceInfo priceInfo = GetPriceInfo(priceTypeItem);
                typeOfPricesNomenclatureItem = new TypeOfPricesNomenclatureItemEntity
                {
                    CatalogItem = catalogItem,
                    TypeOfPriceItem = typeOfPrice,
                    Price = priceInfo.Price,
                    Currency = priceInfo.Currency,
                    DateOfCreation = loadUpdateTime,
                    ForceUpdated = loadUpdateTime,
                    LastUpdated = loadUpdateTime
                };

                if (catalogItem.TypeOfPriceItems == null)
                {
                    catalogItem.TypeOfPriceItems = new List<TypeOfPricesNomenclatureItemEntity>();
                }

                catalogItem.TypeOfPriceItems.Add(typeOfPricesNomenclatureItem);

                if (typeOfPrice.CatalogItems == null)
                {
                    typeOfPrice.CatalogItems = new List<TypeOfPricesNomenclatureItemEntity>();
                }

                typeOfPrice.CatalogItems.Add(typeOfPricesNomenclatureItem);
            }

            return typeOfPricesNomenclatureItem;
        }

        private void CreateBrandItems(JsonLoadData jsonLoadData, DateTimeOffset loadUpdateTime)
        {
            if (jsonLoadData?.Brands != null && jsonLoadData.Brands.Any())
            {
                int countItems = 0;
                List<Brand> batchOfBrands = new List<Brand>();

                jsonLoadData.Brands.ForEach(
                    x =>
                    {
                        batchOfBrands.Add(x);

                        if (countItems % optionService.CountSendItems == 0)
                        {
                            UpdateBrandItems(batchOfBrands, loadUpdateTime);
                        }
                    });

                UpdateBrandItems(batchOfBrands, loadUpdateTime);
            }
        }

        private void UpdateBrandItems(List<Brand> batchOfBrands,
                                      DateTimeOffset loadUpdateTime)
        {
            if (batchOfBrands != null && batchOfBrands.Any())
            {
                DataTable brandsTable = CreateBrandsTable(batchOfBrands);

                var brandsParametr = new SqlParameter();
                brandsParametr.ParameterName = "@brands";
                brandsParametr.SqlDbType = SqlDbType.Structured;
                brandsParametr.TypeName = "brandsTable";
                brandsParametr.Value = brandsTable;
                brandsParametr.Direction = ParameterDirection.Input;

                var lastUpdateParametr = new SqlParameter();
                lastUpdateParametr.ParameterName = "@lastUpdate";
                lastUpdateParametr.SqlDbType = SqlDbType.DateTimeOffset;
                lastUpdateParametr.Value = loadUpdateTime;
                lastUpdateParametr.Direction = ParameterDirection.Input;

                try
                {
                    dataService.DataBaseContext.Database
                        .ExecuteSqlCommand("UpdateBrands @brands, @lastUpdate", brandsParametr, lastUpdateParametr);
                }
                catch (Exception e)
                {
                    // TODO Записать в LOG-файл ошибки работы с базой данных
                }
            }
        }

        private DataTable CreateBrandsTable(List<Brand> items)
        {
            DataTable brandsTable = new DataTable();

            brandsTable.Columns.Add("Code", typeof(Guid));
            brandsTable.Columns.Add("Name", typeof(string));

            if (items != null && items.Any())
            {
                items.ForEach(x => AddBrandsDataRow(brandsTable, x));
                items.Clear();
            }

            return brandsTable;
        }

        private void AddBrandsDataRow(DataTable brandsTable, Brand item)
        {
            Guid code;

            if (item.UID.ConvertToGuid(out code))
            {
                DataRow row = brandsTable.NewRow();

                row.SetField("Code", item.UID);
                row.SetField("Name", item.Name);

                brandsTable.Rows.Add(row);
            }
            else
            {
                // TODO Записать в LOG-файл что не можем распарсить код бренда
            }
        }


        //private void CreateBrandItems(DataBaseContext dataBaseContext, 
        //                              JsonLoadData jsonLoadData, 
        //                              DateTimeOffset loadUpdateTime)
        //{
        //    if (dataBaseContext != null && jsonLoadData?.Brands != null && jsonLoadData.Brands.Any())
        //    {
        //        int countItems = 0;

        //        jsonLoadData.Brands.ForEach(
        //            x =>
        //            {
        //                BrandItemEntity oldBrandItem = GetBrandItem(dataBaseContext, x.UID);

        //                if (oldBrandItem != null)
        //                {
        //                    Update(oldBrandItem, x, loadUpdateTime);
        //                    countItems++;
        //                }
        //                else
        //                {
        //                    BrandItemEntity newBrandItem = Assemble(x, loadUpdateTime);

        //                    if (newBrandItem != null)
        //                    {
        //                        dataBaseContext.BrandItemEntities.Add(newBrandItem);
        //                        countItems++;
        //                    }
        //                }

        //                if (countItems % 100 == 0)
        //                {
        //                    try
        //                    {
        //                        dataBaseContext.SaveChanges();
        //                    }
        //                    catch (Exception)
        //                    {
        //                        ;
        //                    }
        //                }
        //            });

        //        try
        //        {
        //            dataBaseContext.SaveChanges();
        //        }
        //        catch (Exception)
        //        {
        //            ;
        //        }
        //    }
        //}

        //private void Update(BrandItemEntity entity, Brand jsonItem, DateTimeOffset loadUpdateTime)
        //{
        //    if (entity != null && jsonItem != null)
        //    {
        //        if (Equals(entity, jsonItem))
        //        {
        //            entity.ForceUpdated = loadUpdateTime;
        //        }
        //        else
        //        {
        //            entity.Name = jsonItem.Name;
        //            entity.LastUpdated = loadUpdateTime;
        //        }
        //    }
        //}

        //private bool Equals(BrandItemEntity entity, Brand jsonItem)
        //{
        //    Guid code;
        //    return jsonItem.UID.ConvertToGuid(out code) && entity.Code == code && entity.Name == jsonItem.Name;
        //}

        //private BrandItemEntity Assemble(Brand brand, DateTimeOffset loadUpdateTime)
        //{
        //    BrandItemEntity brandItem = null;

        //    if (brand != null)
        //    {
        //        brandItem = new BrandItemEntity
        //        {
        //            Code = brand.UID.ConvertToGuid(),
        //            Name = brand.Name,
        //            DateOfCreation = loadUpdateTime,
        //            ForceUpdated = loadUpdateTime,
        //            LastUpdated = loadUpdateTime
        //        };
        //    }

        //    return brandItem;
        //}

        private void CreatePriceGroupItems(DataBaseContext dataBaseContext,
                                          JsonLoadData jsonLoadData,
                                          DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.PriceGroups != null && jsonLoadData.PriceGroups.Any())
            {
                int countItems = 0;

                jsonLoadData.PriceGroups.ForEach(
                    x =>
                    {
                        PriceGroupItemEntity oldPriceGroupItem = GetPriceGroupItem(dataBaseContext, x.UID);

                        if (oldPriceGroupItem != null)
                        {
                            Update(oldPriceGroupItem, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            PriceGroupItemEntity newPriceGroupItem = Assemble(x, loadUpdateTime);

                            if (newPriceGroupItem != null)
                            {
                                dataBaseContext.PriceGroupItemEntities.Add(newPriceGroupItem);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ;//TODO
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private void Update(PriceGroupItemEntity entity, PriceGroup jsonItem, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && jsonItem != null)
            {
                if (Equals(entity, jsonItem))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = jsonItem.Name;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(PriceGroupItemEntity entity, PriceGroup jsonItem)
        {
            Guid code;
            return jsonItem.UID.ConvertToGuid(out code) && entity.Code == code && entity.Name == jsonItem.Name;
        }

        private PriceGroupItemEntity Assemble(PriceGroup brand, DateTimeOffset loadUpdateTime)
        {
            PriceGroupItemEntity brandItem = null;

            if (brand != null)
            {
                brandItem = new PriceGroupItemEntity
                {
                    Code = brand.UID.ConvertToGuid(),
                    Name = brand.Name,
                    DateOfCreation = loadUpdateTime,
                    ForceUpdated = loadUpdateTime,
                    LastUpdated = loadUpdateTime
                };
            }

            return brandItem;
        }

        private JsonLoadData JsonLoadData()
        {
            var data = new JsonLoadData();
            string[] patterns = optionService.SourcePatterns;
            string[] metaFiles = fileService.GetFileNames(optionService.WorkingSourcePath,
                patterns.First(x => x.StartsWith("MetaData")));

            foreach (string fileName in metaFiles)
            {
                data.AddMetaData(fileService.ReadMetaData(optionService.WorkingSourcePath + fileName));
            }

            string[] priceListFiles = fileService.GetFileNames(optionService.WorkingSourcePath,
                patterns.First(x => x.StartsWith("PriceList")));

            foreach (string fileName in priceListFiles)
            {
                data.AddPriceList(fileService.ReadPriceList(optionService.WorkingSourcePath + fileName));
            }

            string[] clientFiles = fileService.GetFileNames(optionService.WorkingSourcePath,
                patterns.First(x => x.StartsWith("Clients")));

            foreach (string fileName in clientFiles)
            {
                data.AddClient(fileService.ReadClients(optionService.WorkingSourcePath + fileName));
            }

            return data;
        }

        private void SaveMovedErrors(object sender)
        {
            MovingThreadInfo movingInfo = sender as MovingThreadInfo;

            if (movingInfo != null)
            {
                SaveMovedErrors(movingInfo);
            }
        }

        private void SaveMovedErrors(MovingThreadInfo movingInfo)
        {
            ; // TODO Save Moved Errors to DataBase & LOG;
        }

        #endregion

        #region Events
        #endregion

    }
}
