using Template.Service.Implementation;

namespace Template.Service
{
    public interface ITemplateService
    {
        bool IsExistCatalogTemplate(bool isAdvanceSearch);

        CatalogTemplate GetCatalogTemplate(bool isAdvanceSearch);

        void SetCatalogTemplate(CatalogTemplate template);
    }
}
