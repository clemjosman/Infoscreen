using Infoscreens.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("InfoscreensNews")]
    public class InfoscreenNews : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int InfoscreenId { get; set; }

        [Required]
        public int NewsId { get; set; }

        [Required]
        public DateTimeOffset CreationDate { get; set; }


        // Foreign Keys

        [ForeignKey("InfoscreenId")]
        public Infoscreen Infoscreen { get; set; }

        [ForeignKey("NewsId")]
        public News News { get; set; }



        // Methods

        public override string ToString()
        {
            return $"InfoscreenNews #{Id}: InfoscreenId: #{InfoscreenId} / NewsId: #{NewsId}";
        }
    }
}
