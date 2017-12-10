using System;
using System.Collections.Generic;
using DatabaseService.DataBaseContext.Entities;
using Web.WebServiceReference;

namespace Load.Service.Implementation
{
    public static class LoadAssembler
    {
        public static BrandItemEntity Assemble(BrandInfo brandInfo)
        {
            BrandItemEntity brand = new BrandItemEntity
            {
                Id = brandInfo.Id,
                Code = brandInfo.Code,
                Name = brandInfo.Name,
                DateOfCreation = brandInfo.DateOfCreation,
                LastUpdated = brandInfo.LastUpdated,
                ForceUpdated = brandInfo.ForceUpdated
            };

            return brand;
        }

        public static CatalogItemEntity Assemble(CatalogInfo catalogInfo, BrandItemEntity brandItem, 
                                                 List<PhotoItemEntity> photos, DirectoryEntity directory)
        {
            CatalogItemEntity catalogItem = new CatalogItemEntity
            {
                Id = catalogInfo.Id,
                UID = catalogInfo.UID,
                Code = catalogInfo.Code,
                Article = catalogInfo.Article,
                Brand = brandItem,
                BrandName = brandItem.Name,
                Name = catalogInfo.Name,
                Unit = catalogInfo.Unit,
                EnterpriceNormPack = catalogInfo.EnterpriceNormPack,
                BatchOfSales = catalogInfo.BatchOfSales,
                Balance = catalogInfo.Balance,
                Price = catalogInfo.Price,
                Currency = catalogInfo.Currency,
                Multiplicity = catalogInfo.Multiplicity,
                HasPhotos = catalogInfo.HasPhotos,
                Photos = photos,
                DateOfCreation = catalogInfo.DateOfCreation,
                LastUpdated = catalogInfo.LastUpdated,
                ForceUpdated = catalogInfo.ForceUpdated,
                Status = Convert(catalogInfo.Status),
                LastUpdatedStatus = catalogInfo.LastUpdatedStatus,
                Directory = directory
            };

            return catalogItem;
        }

        public static DirectoryEntity Assemble(DirectoryInfo directoryInfo, DirectoryEntity parent, List<DirectoryEntity> subDirectories)
        {
            DirectoryEntity directory = new DirectoryEntity
            {
                Id = directoryInfo.Id,
                Code = directoryInfo.Code,
                Name = directoryInfo.Name,
                DateOfCreation = directoryInfo.DateOfCreation,
                ForceUpdated = directoryInfo.ForceUpdated,
                LastUpdated = directoryInfo.LastUpdated,
                Parent = parent,
                SubDirectory = subDirectories
            };

            return directory;
        }

        public static PhotoItemEntity Assemble(PhotoInfo photoInfo)
        {
            PhotoItemEntity photo = new PhotoItemEntity
            {
                Id = photoInfo.Id,
                Name = photoInfo.Name,
                IsLoad = photoInfo.IsLoad,
                Photo = photoInfo.Photo,
                DateOfCreation = photoInfo.DateOfCreation,
                ForceUpdated = photoInfo.ForceUpdated,
                LastUpdated = photoInfo.LastUpdated
            };

            return photo;
        }

        public static ProductDirectionEntity Assemble(ProductDirectionInfo productDirectionInfo, DirectoryEntity directory)
        {
            ProductDirectionEntity productDirection = new ProductDirectionEntity
            {
                Id = productDirectionInfo.Id,
                Direction = Convert(productDirectionInfo.Direction),
                LastUpdated = productDirectionInfo.LastUpdated,
                ForceUpdated = productDirectionInfo.ForceUpdated,
                DateOfCreation = productDirectionInfo.DateOfCreation,
                Directory = directory
            };

            return productDirection;
        }

        public static Common.Data.Enum.CatalogItemStatus Convert(CatalogItemStatus status)
        {
            Common.Data.Enum.CatalogItemStatus result;

            switch (status)
            {
                case CatalogItemStatus.New:
                    result = Common.Data.Enum.CatalogItemStatus.New;
                    break;
                case CatalogItemStatus.Old:
                    result = Common.Data.Enum.CatalogItemStatus.Old;
                    break;
                case CatalogItemStatus.PriceIsDown:
                    result = Common.Data.Enum.CatalogItemStatus.PriceIsDown;
                    break;
                case CatalogItemStatus.PriceIsUp:
                    result = Common.Data.Enum.CatalogItemStatus.PriceIsUp;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public static Common.Data.Enum.CommodityDirection Convert(CommodityDirection direction)
        {
            Common.Data.Enum.CommodityDirection result;

            switch (direction)
            {
                case CommodityDirection.Vaz:
                    result = Common.Data.Enum.CommodityDirection.Vaz;
                    break;
                case CommodityDirection.Gaz:
                    result = Common.Data.Enum.CommodityDirection.Gaz;
                    break;
                case CommodityDirection.Zaz:
                    result = Common.Data.Enum.CommodityDirection.Zaz;
                    break;
                case CommodityDirection.Chemistry:
                    result = Common.Data.Enum.CommodityDirection.Chemistry;
                    break;
                case CommodityDirection.Gas:
                    result = Common.Data.Enum.CommodityDirection.Gas;
                    break;
                case CommodityDirection.Battery:
                    result = Common.Data.Enum.CommodityDirection.Battery;
                    break;
                case CommodityDirection.Instrument:
                    result = Common.Data.Enum.CommodityDirection.Instrument;
                    break;
                case CommodityDirection.Common:
                    result = Common.Data.Enum.CommodityDirection.Common;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }
    }
}
