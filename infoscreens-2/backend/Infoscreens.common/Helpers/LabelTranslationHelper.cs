using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Common.Helpers
{

    public class LabelTranslationHelper : ILabelTranslationHelper
    {
        private readonly static string _languageServiceRootPath = "https://cluster.actemium.ch/language/api";
        private static List<LanguageItem> _languages = null;
        private static List<KeyValuePair<LanguageItem, Dictionary<string, string>>> _labels = null;

        private readonly ILogger<LabelTranslationHelper> _logger;

        public LabelTranslationHelper(ILogger<LabelTranslationHelper> logger)
        {
            logger.LogDebug(new LogItem(1, "LabelTranslationHelper() Creating a new instance."));

            _logger = logger;

            _logger.LogTrace(new LogItem(2, "LabelTranslationHelper() New instance has been created."));
        }


        public async Task<string> GetTextCodeLabelAsync(string textCode, string iso2)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "GetTextCodeLabel() called.")
                {
                    Custom1 = textCode,
                    Custom2 = iso2
                });

                if (_languages == null)
                {
                    await GetLanguagesFromSFServiceAsync();
                }

                var language = _languages.FirstOrDefault(l => l.Iso2.ToLower() == iso2.ToLower())
                             ?? throw new ArgumentException($"Language \"{iso2}\" is not handled by this application.");

                if (_labels == null || !_labels.Any(l => l.Key.LanguageId == language.LanguageId))
                {
                    await GetTranslationsFromSFServiceAsync(language);
                }

                var languageLabels = _labels.FirstOrDefault(l => l.Key.LanguageId == language.LanguageId);
                if (languageLabels.Equals(default(KeyValuePair<int, string>)))
                {
                    throw new Exception($"Language \"{iso2}\" is not handled by this application.");
                }

                var labelTranslation = languageLabels.Value.FirstOrDefault(l => l.Key == textCode);

                if (labelTranslation.Equals(default(KeyValuePair<string, string>)) || String.IsNullOrEmpty(labelTranslation.Value))
                {
                    throw new Exception($"Label for \"{textCode}\" could not be find or is empty for language {iso2}");
                }

                var label = labelTranslation.Value;

                _logger.LogDebug(new LogItem(11, "GetTextCodeLabel() finished.")
                {
                    Custom1 = textCode,
                    Custom2 = iso2,
                    Custom3 = $"Returning \"{label}\""
                });

                return label;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetTextCodeLabel() returning exception: {0}", ex.Message)
                {
                    Custom1 = textCode,
                    Custom2 = iso2
                });

                return textCode;
                // continue without throw
            }
        }

        #region API calls

        private async Task GetLanguagesFromSFServiceAsync()
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "GetLanguagesFromSFService() called."));

                // Get and parse languages
                (var languagesResponse, _) = await HttpRepository.GetAsync($"{_languageServiceRootPath}/apps/v1/{CommonConfigHelper.LanguageSeviceAppCode}");

                List<LanguageItem> languages = new();
                languages = JObject.Parse(languagesResponse)["languageItems"].ToObject<List<LanguageItem>>();

                // Save languages
                _languages = languages;

                _logger.LogDebug(new LogItem(11, "GetLanguagesFromSFService() finished."));
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetLanguagesFromSFService() returning exception: {0}", ex.Message));
                throw;
            }
        }

        private async Task GetTranslationsFromSFServiceAsync(LanguageItem language)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "GetTranslationsFromSFService() called.")
                {
                    Custom1 = language != null ? $"{language}" : "Argument language is null."
                });

                if (language == null)
                    throw new ArgumentNullException(nameof(language));

                // Get and parse labels
                (var labelsResponse, _) = await HttpRepository.GetAsync($"{_languageServiceRootPath}/labels/v1/?appCode={CommonConfigHelper.LanguageSeviceAppCode}&language={language.Iso2}");
                var labels = JsonConvert.DeserializeObject<Dictionary<string, string>>(labelsResponse);

                // If labels are already present for language -> delete them
                if (_labels == null)
                {
                    // Init and save labels
                    _labels = new List<KeyValuePair<LanguageItem, Dictionary<string, string>>>() { new KeyValuePair<LanguageItem, Dictionary<string, string>>(language, labels) };
                }
                else
                {
                    var index = _labels.FindIndex(e => e.Key.LanguageId == language.LanguageId);
                    if (index != -1)
                    {
                        _labels.RemoveAt(index);
                    }

                    // Save labels
                    _labels.Add(new KeyValuePair<LanguageItem, Dictionary<string, string>>(language, labels));
                }
                

                _logger.LogDebug(new LogItem(11, "GetTranslationsFromSFService() finished.")
                {
                    Custom1 = language != null ? $"{language}" : "Argument language is null."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetTranslationsFromSFService() returning exception: {0}", ex.Message)
                {
                    Custom1 = language != null ? $"{language}" : "Argument language is null."
                });
                throw;
            }
        }

        #endregion

        class LanguageItem
        {
            public int LanguageId { get; set; }

            public string LanguageName { get; set; }

            public string Iso2 { get; set; }

            public string CultureCode { get; set; }

            public string FlagCode { get; set; }

            public IEnumerable<string> Applications { get; set; }

            public override string ToString()
            {
                var applications = Applications != null && Applications.Any() ? string.Join("; ", Applications) : "(no apps assigned)";

                return $"{LanguageId}: {Iso2} - {LanguageName} - {CultureCode} - {applications}";
            }
        }
    }
}
