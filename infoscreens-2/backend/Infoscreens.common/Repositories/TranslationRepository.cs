using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;
using vesact.common.translate;

namespace Infoscreens.Common.Repositories
{
    public class TranslationRepository : ITranslationRepository
    {

        private readonly ILogger<TranslationRepository> _logger;
        private readonly IDatabaseRepository _databaseRepository;

        public TranslationRepository(ILogger<TranslationRepository> logger, IDatabaseRepository databaseRepository)
        {
            logger.LogDebug(new LogItem(1, "TranslationRepository() Creating a new instance."));

            _logger = logger;
            _databaseRepository = databaseRepository;

            _logger.LogTrace(new LogItem(2, "TranslationRepository() New instance has been created."));
        }


        public async Task<Translation> GenerateOrUpdateTranslationAsync(Translation existingTranslation, Dictionary<string, string> receivedTexts)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "GenerateOrUpdateTranslationAsync() called.")
                {
                    Custom1 = existingTranslation != null ? existingTranslation.ToString() : "Parameter existingTranslation is null.",
                    Custom4 = receivedTexts != null ? JsonConvert.SerializeObject(receivedTexts) : "Parameter existingTranslation is null."
                });

                if (receivedTexts == null)
                    throw new ArgumentNullException(nameof(receivedTexts));


                // Get languages & init
                var translatedTexts = new List<TranslatedText>();
                var existingTranslatedTexts = new List<TranslatedText>();
                var translation = existingTranslation ?? new Translation(Guid.NewGuid().ToString());
                var receivedLanguages = receivedTexts.Keys.Select(iso2 => _databaseRepository.GetLanguageAsync(iso2)).Select(t => t.Result).ToList();


                // Marking translated texts entries that are not in the receivedTexts as to be deleted
                if(existingTranslation != null)
                {
                    existingTranslatedTexts = await _databaseRepository.GetTranslatedTextsOfTranslationAsync(existingTranslation);
                    translation.TranslatedTextsToDelete =  existingTranslatedTexts.Where(tt => !receivedLanguages.Select(l => l.Id).Contains(tt.LanguageId)).ToList();
                }

                // Automatic translate
                foreach (var language in receivedLanguages)
                {
                    // Making keys to lower to allow usage of languages like 'de' and 'DE' as we're using the ISO 639-1 specification
                    receivedTexts.ToDictionary(k => k.Key.ToLower(), k => k.Value).TryGetValue(language.Iso2, out string text);
                    translatedTexts.Add(await CreateOrUpdateTranslatedTextAsync(language, text, existingTranslation));
                }


                translation.TranslatedTexts = translatedTexts;


                _logger.LogDebug(new LogItem(11, "GenerateOrUpdateTranslationAsync() finished.")
                {
                    Custom1 = existingTranslation != null ? existingTranslation.ToString() : "Parameter existingTranslation is null.",
                    Custom2 = translation != null ? translation.ToString() : "Created or updated translation is null.",
                    Custom4 = receivedTexts != null ? JsonConvert.SerializeObject(receivedTexts) : "Parameter existingTranslation is null."
                });

                return translation;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GenerateOrUpdateTranslationAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #region Create or Update TranslatedText

        public async Task<TranslatedText> CreateOrUpdateTranslatedTextAsync(Language language, string text, Translation existingTranslation = null)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "CreateOrUpdateTranslatedTextAsync() called.")
                {
                    Custom1 = language != null ? language.ToString() : "Parameter language is null.",
                    Custom2 = !string.IsNullOrWhiteSpace(text) ? text : "Parameter text is null or empty.",
                    Custom3 = existingTranslation != null ? existingTranslation.ToString() : "Parameter existingTranslation is null."
                });

                #region Checks and Inits

                if (language == null)
                    throw new ArgumentNullException(nameof(language));

                // INFO: No check needed for existingTranslation, as this is null for new translations


                TranslatedText translatedText;
                var translationHelper = new TranslationHelper_Microsoft(CommonConfigHelper.TransationApiKey_Microsoft);

                #endregion Checks and Inits

                if(existingTranslation == null)
                {
                    translatedText = new TranslatedText(language, text);
                }
                else
                {
                    // Translation already exists, check if translated text already exists for this language
                    TranslatedText existingTranslatedText = await _databaseRepository.GetTranslatedTextAsync(existingTranslation, language);

                    if(existingTranslatedText == null)
                    {
                        translatedText = new TranslatedText(language, text);
                    }
                    else
                    {
                        translatedText = existingTranslatedText.UpdateText(text);
                    }
                }


                _logger.LogDebug(new LogItem(11, "CreateOrUpdateTranslatedTextAsync() finished.")
                {
                    Custom1 = language != null ? language.ToString() : "Parameter language is null.",
                    Custom2 = !string.IsNullOrWhiteSpace(text) ? text : "Parameter text is null or empty.",
                    Custom3 = existingTranslation != null ? existingTranslation.ToString() : "Parameter existingTranslation is null.",
                    Custom4 = translatedText != null ? translatedText.ToString() : "Created or updated translatedText is null.",
                });

                return translatedText;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CreateOrUpdateTranslatedTextAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Create or Update TranslatedText
    }
}
