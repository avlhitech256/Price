﻿using System;
using DatabaseService.DataBaseContext.Entities;

namespace Domain.Data.Object
{
    public class BrandItem
    {
        #region Constructors

        public BrandItem(BrandItemEntity entity)
        {
            Entity = entity;
        }

        #endregion

        #region Properties

        public BrandItemEntity Entity { get; }

        public long Id => Entity.Id;

        public Guid Code => Entity.Code;

        public string Name => Entity.Name;

        #endregion
    }
}
