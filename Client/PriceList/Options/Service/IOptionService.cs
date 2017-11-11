using System.Collections.Generic;

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

        bool ExistOption(string optionCode);

        string GetOption(string optionCode);

        Dictionary<string, string> GetOption(IEnumerable<string> optionCodes);

        void SetOption(string optionCode, string value);

        void SetOptions(Dictionary<string, string> options);

        bool GetBooleanOption(string optionCode);

        Dictionary<string, bool> GetBooleanOptions(IEnumerable<string> optionCodes);

        void SetBooleanOption(string optionCode, bool value);

        void SetBooleanOptions(Dictionary<string, bool> options);

        int GetIntOption(string optionCode);

        Dictionary<string, int> GetIntOptions(IEnumerable<string> optionCodes);

        void SetIntOption(string optionCode, int value);

        void SetIntOptions(Dictionary<string, int> options);

        long GetLongOption(string optionCode);

        Dictionary<string, long> GetLongOptions(IEnumerable<string> optionCodes);

        void SetLongOption(string optionCode, long value);

        void SetLongOptions(Dictionary<string, long> options);

        double GetDoubleOption(string optionCode);

        Dictionary<string, double> GetDoubleOptions(IEnumerable<string> optionCodes);

        void SetDoubleOption(string optionCode, double value);

        void SetDoubleOptions(Dictionary<string, double> options);
    }
}
