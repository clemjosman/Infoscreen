using Infoscreens.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("VideosCategories")]
    public class VideoCategory : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int VideoId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public DateTimeOffset CreationDate { get; set; }


        // Foreign Keys

        [ForeignKey("VideoId")]
        public Video Video { get; set; }

        [ForeignKey("CategoryId ")]
        public Category Category { get; set; }


        // Constructors

        public VideoCategory() { } // Needed by Entity Framework

        public VideoCategory(Video video, Category category)
        {
            Video = video;
            if (category.Id == 0)
                Category = category;
            else
                CategoryId = category.Id;
        }

        // Methods

        public override string ToString()
        {
            return $"VideoCategory #{Id}: VideoId: {VideoId} / CategoryId: {CategoryId}";
        }
    }
}
