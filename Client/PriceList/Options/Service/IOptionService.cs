namespace Options.Service
{
    public interface IOptionService
    {
        string Login { get; set; }

        string Password { get; set; }

        string OverdueAccountsReceivable { get; set; }

        string Debt { get; set; }

        long LastOrderNumber { get; set; }

        bool ShowPhotoOnMouseDoubleClick { get; set; }

        int CatalogMaximumRows { get; set; }

        double SplitterPosition { get; set; }
    }
}
