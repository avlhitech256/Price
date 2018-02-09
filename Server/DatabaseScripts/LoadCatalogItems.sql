SELECT TOP (50) CatalogItems.*,  TOPN1.Price, TOPN1.Currency, TypeOfPrice1.[Name], Contragent1.[Name]
FROM SendItemsEntities AS SendItems
INNER JOIN CatalogItemEntities AS CatalogItems ON CatalogItems.Id = SendItems.EntityId
LEFT JOIN TypeOfPricesNomenclatureItemEntities AS TOPN1 ON CatalogItems.Id = TOPN1.CatalogItem_Id
LEFT JOIN TypeOfPriceItemEntities AS TypeOfPrice1 ON TOPN1.TypeOfPriceItem_Id = TypeOfPrice1.Id
LEFT JOIN PriceTypePriceGroupContragentEntities AS PTPGC ON TypeOfPrice1.Id = PTPGC.TypeOfPriceItem_Id
LEFT JOIN ContragentItemEntities AS Contragent1 ON PTPGC.ContragentItem_Id = Contragent1.Id
WHERE SendItems.[Login] = 'k6731' AND 
      --Contragent1.[Login] = 'k6731' AND
      SendItems.EntityName = 2;