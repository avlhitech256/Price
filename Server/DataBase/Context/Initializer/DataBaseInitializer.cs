using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Common.Data.Constant;
using DataBase.Context.Entities;
using File.Service;
using File.Service.Implementation;
using Json.Service;
using Json.Service.Implementation;
using Media.Service;
using Media.Service.Implementation;
using CommodityDirection = Common.Data.Enum.CommodityDirection;

namespace DataBase.Context.Initializer
{
    public class DataBaseInitializer : DropCreateDatabaseIfModelChanges<DataBaseContext>
    {
        private readonly IImageService imageService;
        private readonly IJsonService jsonService;
        private readonly IFileService fileService;

        public DataBaseInitializer()
        {
            imageService = new ImageService();
            jsonService = new JsonService();
            fileService = new FileService(jsonService);
        }
        protected override void Seed(DataBaseContext dataBaseContext)
        {
            // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
            dataBaseContext.Configuration.AutoDetectChangesEnabled = false;
            dataBaseContext.Configuration.ValidateOnSaveEnabled = false;

            PopulateOptionItemEntities(dataBaseContext);

            dataBaseContext.Configuration.AutoDetectChangesEnabled = true;
            dataBaseContext.Configuration.ValidateOnSaveEnabled = true;

            base.Seed(dataBaseContext);
        }

        private void CreateProductDirection(DataBaseContext dataBaseContext)
        {
            try
            {
                ProductDirectionEntity item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Vaz;
                Guid code = Guid.Parse("1FF0EBCD-1507-40E9-A409-2AF3B8F77D49");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Gaz;
                code = Guid.Parse("9B6B4325-B435-4D65-925E-4921F09D461F");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Zaz;
                code = Guid.Parse("264D3368-ECA8-4F2F-9C3B-7B7CC582C7FD");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Chemistry;
                code = Guid.Parse("78DC5231-C9D8-49A1-9FC5-AD18BF71DC13");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Battery;
                code = Guid.Parse("C7667731-5CFE-4BB6-90DC-8520C87F4FA0");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Gas;
                code = Guid.Parse("A4151BE4-3D3F-4A18-A3D1-4BF48F03AB6C");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Instrument;
                code = Guid.Parse("AFEDE7C9-85E0-4C60-A5FB-0D81B0771D3D");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Common;
                code = Guid.Parse("80135062-F8F6-4C5A-AFD2-48A0117EB2B6");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                dataBaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                ;//throw;
            }
        }

        private void PopulateOptionItemEntities(DataBaseContext dataBaseContext)
        {
            List<OptionItemEntity> optionItems = new List<OptionItemEntity>
            {
                new OptionItemEntity
                {
                    Code = OptionName.SourcePath,
                    Name = "Input download path from 1C to Application Server",
                    Value = "C:\\1C\\In\\"
                },
                new OptionItemEntity
                {
                    Code = OptionName.WorkingSourcePath,
                    Name = "Input working path for Application Server",
                    Value = "In\\"
                },
                new OptionItemEntity
                {
                    Code = OptionName.ArcSourcePath,
                    Name = "Archive path for input data files",
                    Value = "Arc\\In\\"
                },
                new OptionItemEntity
                {
                    Code = OptionName.SourcePatterns,
                    Name = "Patterns for input data files",
                    Value = "Clients*.json,MetaData*.json,PriceList*.json,RTiU*.json"
                },
                new OptionItemEntity
                {
                    Code = OptionName.DestinationPath,
                    Name = "Output download path from 1C to Application Server",
                    Value = "C:\\1C\\Out\\"
                },
                new OptionItemEntity
                {
                    Code = OptionName.WorkingDestinationPath,
                    Name = "Output working path for Application Server",
                    Value = "Out\\"
                },
                new OptionItemEntity
                {
                    Code = OptionName.ArcDestinationPath,
                    Name = "Archive path for output data files",
                    Value = "Arc\\Out\\"
                },
                new OptionItemEntity
                {
                    Code = OptionName.SubDirForPhoto,
                    Name = "Photo sub directory path for Application Server",
                    Value = "Photo\\"
                },
                new OptionItemEntity
                {
                    Code = OptionName.PhotoPatterns,
                    Name = "Patterns for input photo files",
                    Value = "*.jpeg"
                },
                new OptionItemEntity
                {
                    Code = OptionName.CountSendItems,
                    Name = "Count of items to send in some packedge",
                    Value = "50"
                },
                new OptionItemEntity
                {
                    Code = OptionName.CountSendPhopos,
                    Name = "Count of photos to send in some packedge",
                    Value = "5"
                }
            };

            dataBaseContext.OptionItemEntities.AddRange(optionItems);
            dataBaseContext.SaveChanges();
        }
    }
}
