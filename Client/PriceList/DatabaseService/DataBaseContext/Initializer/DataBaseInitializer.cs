using System;
using System.Collections.Generic;
using System.Data.Entity;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.Properties;
using Media.Image;

namespace DatabaseService.DataBaseContext.Initializer
{
    public class DataBaseInitializer : DropCreateDatabaseIfModelChanges<DataBaseContext>
    {
        private readonly IImageService imageService;
        public DataBaseInitializer()
        {
            imageService = new ImageService();
        }
        protected override void Seed(DataBaseContext dataBaseContext)
        {
            List<BrandItemEntity> brandItems = PopulateBrandItemEntities(dataBaseContext);
            List<PhotoItemEntity> photoItems = PopulatePhotoEntities(dataBaseContext);
            PopulateCatalogItemEntities(dataBaseContext, brandItems, photoItems);
            PopulateOptionItemEntities(dataBaseContext);
            dataBaseContext.SaveChanges();
            base.Seed(dataBaseContext);
        }

        private void CreateIndex(DataBaseContext dataBaseContext)
        {
            dataBaseContext.Database.ExecuteSqlCommand
                ("CREATE INDEX Index_CatalogItemEntity_Name ON CatalogItemEntities (Name)");
            //("CREATE INDEX Index_CatalogItemEntity_Name ON CatalogItemEntities (Id) INCLUDE (Name)");
        }

        private List<BrandItemEntity> PopulateBrandItemEntities(DataBaseContext dataBaseContext)
        {
            List<BrandItemEntity> brandItems = new List<BrandItemEntity>
            {
                new BrandItemEntity
                {
                    Code = Guid.NewGuid(),
                    Name = "АВТОПАРТНЕР ООО Г.ДИМИТРОВГРАД"
                },
                new BrandItemEntity
                {
                    Code = Guid.NewGuid(),
                    Name = "АВТОВАЗ"
                },
                new BrandItemEntity
                {
                    Code = Guid.NewGuid(),
                    Name = "Daewoo-ZAZ"
                }
            };

            dataBaseContext.BrandItemEntities.AddRange(brandItems);

            return brandItems;
        }

        private List<PhotoItemEntity> PopulatePhotoEntities(DataBaseContext dataBaseContext)
        {
            List<PhotoItemEntity> photoItems = new List<PhotoItemEntity>
            {
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo1)},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo2)},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo3)},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo4)},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo5)},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo6)}
            };

            dataBaseContext.PhotoItemEntities.AddRange(photoItems);

            return photoItems;
        }

        private void PopulateCatalogItemEntities(DataBaseContext dataBaseContext, 
                                                 List<BrandItemEntity> brandItems, 
                                                 List<PhotoItemEntity> photoItems)
        {
            List<CatalogItemEntity> catalogItems = new List<CatalogItemEntity>
            {
                new CatalogItemEntity
                {
                    Code = "04157",
                    UID = Guid.NewGuid(),
                    Article = "11180-1108054-00",
                    Brand = brandItems[0],
                    Name = "Трос акселератора ВАЗ 1118 дв. 1,6л",
                    EnterpriceNormPack = "10 шт.",
                    Balance = "свыше 100 шт.",
                    Unit = "шт.",
                    Price = 50.52M,
                    Currency = "EUR",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    Photos = new List<PhotoItemEntity> {photoItems[0], photoItems[1], photoItems[2]}
                },
                new CatalogItemEntity
                {
                    Code = "87930",
                    UID = Guid.NewGuid(),
                    Article = "3160-3819010-00",
                    Brand = brandItems[1],
                    Name = "Ступица передняя HYUNDAI H-1 07- Без ABS",
                    EnterpriceNormPack = "10 шт.",
                    Balance = "свыше 100 шт.",
                    Unit = "шт.",
                    Price = 320.45M,
                    Currency = "грн.",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    Photos = new List<PhotoItemEntity> {photoItems[1], photoItems[2], photoItems[3]}
                },
                new CatalogItemEntity
                {
                    Code = "98486",
                    UID = Guid.NewGuid(),
                    Article = "21213",
                    Brand = brandItems[0],
                    Name = "Крышка бачка расширительного DAEWOO LANOS/SENS",
                    EnterpriceNormPack = "10 шт.",
                    Balance = "от 10 до 100 шт.",
                    Unit = "шт.",
                    Price = 18.65M,
                    Currency = "грн.",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    Photos = new List<PhotoItemEntity> {photoItems[2], photoItems[1]}
                },
                new CatalogItemEntity
                {
                    Code = "24029",
                    UID = Guid.NewGuid(),
                    Article = "11180-3508180-00",
                    Brand = brandItems[2],
                    Name = "Баллон тороидальный 42л 600х200мм наружный, Atiker",
                    EnterpriceNormPack = "1 шт.",
                    Balance = "8 шт.",
                    Unit = "шт.",
                    Price = 120.78M,
                    Currency = "USD",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    Photos = new List<PhotoItemEntity> {photoItems[3], photoItems[4]}
                },
                new CatalogItemEntity
                {
                    Code = "24029",
                    UID = Guid.NewGuid(),
                    Article = "3302-8406140-00",
                    Brand = brandItems[0],
                    Name = "Трос капота ГАЗ 3302",
                    EnterpriceNormPack = "1 шт.",
                    Balance = "2 шт.",
                    Unit = "шт.",
                    Price = 1850.46M,
                    Currency = "грн.",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    Photos = new List<PhotoItemEntity> {photoItems[4], photoItems[5], photoItems[2]}
                }
            };

            dataBaseContext.CatalogItemEntities.AddRange(catalogItems);
        }
        private void PopulateOptionItemEntities(DataBaseContext dataBaseContext)
        {
            List<OptionItemEntity> optionItems = new List<OptionItemEntity>
            {
                new OptionItemEntity
                {
                    Code = "LOGIN",
                    Name = "User Login",
                    Value = "autotrend"
                },
                new OptionItemEntity
                {
                    Code = "PASSWORD",
                    Name = "User Password",
                    Value = ""
                },
                new OptionItemEntity
                {
                    Code = "LASTORDERNUMBER",
                    Name = "Last Order Number",
                    Value = "0"
                },
            };

            dataBaseContext.OptionItemEntities.AddRange(optionItems);
        }
    }
}
