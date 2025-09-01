using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public interface ILabelTranslationHelper
    {
        Task<string> GetTextCodeLabelAsync(string textCode, string iso2);
    }
}
