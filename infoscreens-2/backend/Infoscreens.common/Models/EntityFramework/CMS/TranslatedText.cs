using Infoscreens.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("TranslatedTexts")]
    public class TranslatedText : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int TranslationId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        public string Text { get; set; }

        public DateTimeOffset? LastEditDate { get; set; }


        // Foreign Key

        [ForeignKey("TranslationId")]
        public Translation Translation { get; set; }

        [ForeignKey("LanguageId")]
        public Language Language { get; set; }


        // Constructors
        public TranslatedText() { } // Needed by EntityFramework

        public TranslatedText(Language language, string text)
        {
            if(language == null)
                throw new ArgumentNullException(nameof(language));

            Text = text;
            LastEditDate = DateTimeOffset.UtcNow;
            LanguageId = language.Id;
        }


        // Methods

        public override string ToString()
        {
            return $"TranslatedText #{Id}: TranslationId: {TranslationId} / LanguageId: {LanguageId}";
        }

        public TranslatedText UpdateText(string text)
        {
            Text = text;
            LastEditDate = DateTimeOffset.UtcNow;

            return this;
        }

        public TranslatedText RemoveIncludes()
        {
            Translation = null;
            Language = null;

            return this;
        }
    }
}
