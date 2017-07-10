using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Catalog.Properties;
using Common.Data.Notifier;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.Data.Object;
using Domain.DomainContext;

namespace Catalog.Model
{
    public class CatalogModel : Notifier
    {
        #region Members

        private CatalogItem selectedItem;
        private CatalogItem oldSelectedItem;
        private ObservableCollection<CatalogItem> entities;

        #endregion

        #region Constructors

        public CatalogModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            InitData();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IMessenger Messenger => DomainContext?.Messenger;

        public CatalogItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                    SendSetImageMessage();
                }
            }
        }

        public ObservableCollection<CatalogItem> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                if (entities != value)
                {
                    entities = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void SendSetImageMessage()
        {
            Messenger.Send(CommandName.SetImage, SelectedItem);
        }

        private void InitData()
        {
            //ResourceReader recourceReader = new ResourceReader("Resources.resx");

            long id = 0L;
            long position = 1L;
            var brand1 = new BrandItem
            {
                Id = 0,
                Code = Guid.NewGuid(),
                Name = "АВТОПАРТНЕР ООО Г.ДИМИТРОВГРАД"
            };
            var brand2 = new BrandItem
            {
                Id = 1,
                Code = Guid.NewGuid(),
                Name = "АВТОВАЗ"
            };
            var brand3 = new BrandItem
            {
                Id = 1,
                Code = Guid.NewGuid(),
                Name = "Daewoo-ZAZ"
            };
            entities = new ObservableCollection<CatalogItem>();

            CatalogItem item = new CatalogItem
            {
                Id = id++,
                Position = position++,
                Code = "04157",
                Article = "11180-1108054-00",
                Brand = brand1,
                Name = "Трос акселератора ВАЗ 1118 дв. 1,6л",
                EnterpriceNormPack = "10 шт.",
                Balance = "свыше 100 шт.",
                Unit = "шт.",
                Price = 50.52M,
                Currency = "EUR",
                Photo = new List<byte[]> {ImageToByte(Resources.Photo1)}
            };
            Entities.Add(item);
            //---------------------------------------------------------------------
            item = new CatalogItem
            {
                Id = id++,
                Position = position++,
                Code = "87930",
                Article = "3160-3819010-00",
                Brand = brand2,
                Name = "Ступица передняя HYUNDAI H-1 07- Без ABS",
                EnterpriceNormPack = "10 шт.",
                Balance = "свыше 100 шт.",
                Unit = "шт.",
                Price = 320.45M,
                Currency = "грн.",
                Photo = new List<byte[]> {ImageToByte(Resources.Photo2)}
            };
            Entities.Add(item);
            //---------------------------------------------------------------------
            item = new CatalogItem
            {
                Id = id++,
                Position = position++,
                Code = "98486",
                Article = "21213",
                Brand = brand1,
                Name = "Крышка бачка расширительного DAEWOO LANOS/SENS",
                EnterpriceNormPack = "10 шт.",
                Balance = "от 10 до 100 шт.",
                Unit = "шт.",
                Price = 18.65M,
                Currency = "грн.",
                Photo = new List<byte[]> {ImageToByte(Resources.Photo3)}
            };
            //recourceReader.GetResourceData("Photo3", out typeResource, out photo);
            //item.Photo = photo;
            Entities.Add(item);
            //---------------------------------------------------------------------
            item = new CatalogItem
            {
                Id = id++,
                Position = position++,
                Code = "24029",
                Article = "11180-3508180-00",
                Brand = brand3,
                Name = "Баллон тороидальный 42л 600х200мм наружный, Atiker",
                EnterpriceNormPack = "1 шт.",
                Balance = "8 шт.",
                Unit = "шт.",
                Price = 120.78M,
                Currency = "USD",
                Photo = new List<byte[]> {ImageToByte(Resources.Photo4)}
            };
            //recourceReader.GetResourceData("Photo4", out typeResource, out photo);
            //item.Photo = photo;
            Entities.Add(item);
            //---------------------------------------------------------------------
            item = new CatalogItem
            {
                Id = id++,
                Position = position++,
                Code = "24029",
                Article = "3302-8406140-00",
                Brand = brand1,
                Name = "Трос капота ГАЗ 3302",
                EnterpriceNormPack = "1 шт.",
                Balance = "2 шт.",
                Unit = "шт.",
                Price = 1850.46M,
                Currency = "грн.",
                Photo = new List<byte[]> {ImageToByte(Resources.Photo5)}
            };
            //recourceReader.GetResourceData("Photo5", out typeResource, out photo);
            //item.Photo = photo;
            Entities.Add(item);

            SelectedItem = Entities[0];
        }

        public byte[] GetJpgFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        #endregion

    }
}
