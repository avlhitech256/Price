namespace Options.Service
{
    public interface IOptionService
    {
        string Login { get; set; }

        string Password { get; set; }

        long LastOrderNumber { get; set; }

        bool ShowPhotoOnMouseDoubleClick { get; set; }

        int CatalogMaximumRows { get; set; }
    }
}
