using Infoscreens.Common.Models.EntityFramework.CMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public interface ITranslationRepository
    {
        Task<Translation> GenerateOrUpdateTranslationAsync(Translation existingTranslation, Dictionary<string, string> receivedTexts);

        #region Create or Update TranslatedText

        Task<TranslatedText> CreateOrUpdateTranslatedTextAsync(Language language, string text, Translation existingTranslation = null);

        #endregion Create or Update TranslatedText
    }
}
