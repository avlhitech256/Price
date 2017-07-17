using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service.Precision
{
    public interface IPrecisionService
    {
        int Precision { get; set; }
        bool Separator { get; set; }
        string Format { get; }
        string NormalizeValue(string value);
        string NormalizeValue(string value, int valuesPrecision);
        string NormalizeValue(string value, int valuesPrecision, bool separatorIsVisible);
        string RemoveSymbols(string value);
        decimal Convert(string value);
        string Convert(decimal value);
        string Convert(decimal value, int valuesPrecision);
        string Convert(decimal value, int valuesPrecision, bool separatorIsVisible);
        string GetFormat();
        string GetFormat(int valuesPrecision);
        string GetFormat(int valuesPrecision, bool separatorIsVisible);

    }
}
