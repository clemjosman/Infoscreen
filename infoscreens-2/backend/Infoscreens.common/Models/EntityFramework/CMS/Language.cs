using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers.Enumerations;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("Languages")]
    public class Language : IId, ILanguage
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        [StringLength(2)]
        public string Iso2 { get; set; } // ISO 639-1

        [Required]
        public int DisplayNameTranslationId { get; set; }



        // Foreign Keys

        [ForeignKey("DisplayNameTranslationId")]
        public Translation DisplayNameTranslation { get; set; }



        // Relations

        [InverseProperty("Language")]
        public ICollection<TranslatedText> TranslatedTexts { get; set; }

        [InverseProperty("DefaultContentLanguage")]
        public ICollection<Infoscreen> Infoscreens { get; set; }



        // Methods

        public override string ToString()
        {
            return $"Language #{Id}: Iso2: {Iso2}";
        }

        public eSlideshowLanguage ToSlideshowLanguage()
        {
            // There is no need to handle different cultures for a same language currently
            // So a simple mapping can be done for now.
            // Adding a column 'CultureCode' that follows the same ISO standards as eSlideshowLanguage would solve part of that need once really needed.
            return Iso2 switch
            {
                "de" => eSlideshowLanguage.DE_CH,
                "fr" => eSlideshowLanguage.FR_CH,
                "it" => eSlideshowLanguage.IT_CH,
                _ => eSlideshowLanguage.EN_GB,
            };
        }

        public string ToSlideshowLanguageString()
        {
            return ToSlideshowLanguage().GetEnumMemberAttrValue();
        }

        public apiLanguage_Light ToApiLanguage_Light()
        {
            return new apiLanguage_Light(Id, Iso2);
        }

        public async Task<apiLanguage> ToApiLanguageAsync(IDatabaseRepository _databaseRepository)
        {
            Dictionary<string, string> displayName;
            DisplayNameTranslation ??= await _databaseRepository.GetTranslationtByIdAsync(DisplayNameTranslationId);
            displayName = DisplayNameTranslation.TranslatedTexts?.GroupBy(t => t.Language.ToSlideshowLanguageString()).ToDictionary(g => g.Key, g => g.First().Text);

            return new apiLanguage(Id, Iso2, displayName, ToSlideshowLanguageString());
        }
    }
}
