using Infoscreens.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("InfoscreensVideos")]
    public class InfoscreenVideo : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int InfoscreenId { get; set; }

        [Required]
        public int VideoId { get; set; }

        [Required]
        public DateTimeOffset CreationDate { get; set; }


        // Foreign Keys

        [ForeignKey("InfoscreenId")]
        public Infoscreen Infoscreen { get; set; }

        [ForeignKey("VideoId")]
        public Video Video { get; set; }



        // Methods

        public override string ToString()
        {
            return $"InfoscreenVideo #{Id}: InfoscreenId: #{InfoscreenId} / VideoId: #{VideoId}";
        }
    }
}
