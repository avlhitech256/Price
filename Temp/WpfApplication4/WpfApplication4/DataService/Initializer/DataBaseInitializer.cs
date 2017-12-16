using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WpfApplication4.DataService.Entity;

namespace WpfApplication4.DataService.Initializer
{
    public class DataBaseInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext dataBaseContext)
        {
            // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
            dataBaseContext.Configuration.AutoDetectChangesEnabled = false;
            dataBaseContext.Configuration.ValidateOnSaveEnabled = false;

            PopulateOptionItemEntities(dataBaseContext);

            dataBaseContext.Configuration.AutoDetectChangesEnabled = true;
            dataBaseContext.Configuration.ValidateOnSaveEnabled = true;

            base.Seed(dataBaseContext);
        }

        private void PopulateOptionItemEntities(DatabaseContext dataBaseContext)
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
