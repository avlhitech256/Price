select Cat.* from CatalogItemEntities as Cat
left join PhotoItemEntities as Photo on Cat.Id = Photo.CatalogItem_Id
where IsLoad = 1;
--select * from PhotoItemEntities;