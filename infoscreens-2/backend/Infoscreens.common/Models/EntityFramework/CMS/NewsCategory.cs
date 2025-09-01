using Infoscreens.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("NewsCategories")]
    public class NewsCategory : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int NewsId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public DateTimeOffset CreationDate { get; set; }


        // Foreign Keys

        [ForeignKey("NewsId")]
        public News News { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }


        // Contructors
        public NewsCategory() { } // Empty constructor needed by Entity Framework

        public NewsCategory(News news, Category category)
        {
            News = news;
            if (category.Id == 0)
                Category = category;
            else
                CategoryId = category.Id;
        }

        // Methods

        public override string ToString()
        {
            return $"NewsCategory #{Id}: NewsId: #{NewsId} / CategoryId: #{CategoryId}";
        }
    }
}
