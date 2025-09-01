using Infoscreens.Common.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("Translations")]
    public class Translation : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        [StringLength(100)]
        public string TextCode { get; set; }


        // Not Mapped

        // INFO: This is used when an element contains translated texts that will not be used anymore.
        // As the function handling the translated text creation and update is async, it is not possible to use ref or out parameters
        // The only way to return the translation witht the list of translated text to delete is to use this parameter.
        [NotMapped]
        public IEnumerable<TranslatedText> TranslatedTextsToDelete { get; set; } = new List<TranslatedText>();



        // Relations

        [InverseProperty("Translation")]
        public ICollection<TranslatedText> TranslatedTexts { get; set; }

        [InverseProperty("TitleTranslation")]
        public ICollection<News> NewsTitleTranslations { get; set; }

        [InverseProperty("ContentMarkdownTranslation")]
        public ICollection<News> NewsContentMArkdownTranslations { get; set; }

        [InverseProperty("ContentHTMLTranslation")]
        public ICollection<News> NewsContentHTMLTranslations { get; set; }

        [InverseProperty("TitleTranslation")]
        public ICollection<Video> VideoTitles { get; set; }


        // Constructors
        public Translation() { } // Needed by EntityFramework

        public Translation(string textCode)
        {
            TextCode = textCode;
        }


        // Methods

        public override string ToString()
        {
            return $"Translation #{Id}: TextCode: {TextCode}";
        }


        public async Task<Dictionary<string, string>> ToDictionaryAsync(IDatabaseRepository _databaseRepository)
        {
            IEnumerable<TranslatedText> translatedTexts;
            if (TranslatedTexts == null || TranslatedTexts.Any(tt => tt.Language == null))
            {
                translatedTexts = await _databaseRepository.GetTranslatedTextsOfTranslationAsync(this);
            }
            else
            {
                translatedTexts = TranslatedTexts;
            }

            return translatedTexts?.GroupBy(t => t.Language.ToSlideshowLanguageString()).ToDictionary(g => g.Key, g => g.First().Text);
        }
    }
}
