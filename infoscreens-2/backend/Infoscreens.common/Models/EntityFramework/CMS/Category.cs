using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("Categories")]
    public class Category : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int TenantId { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [Required]
        public DateTimeOffset CreationDate { get; set; }

        [Required]
        public int? CreatedBy { get; set; }

        public DateTimeOffset? LastEditDate { get; set; }

        public int? LastEditedBy { get; set; }

        public DateTimeOffset? DeletionDate { get; set; }

        public int? DeletedBy { get; set; }



        // Foreign Keys

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        [ForeignKey("CreatedBy")]
        public User Creator { get; set; }

        [ForeignKey("LastEditedBy")]
        public User LastEditor { get; set; }

        [ForeignKey("DeletedBy")]
        public User DeletedByUser { get; set; }



        // Relations

        [InverseProperty("Category")]
        public ICollection<NewsCategory> NewsCategories { get; set; }

        [InverseProperty("Category")]
        public ICollection<VideoCategory> VideosCategories { get; set; }


        // Constructors

        public Category() { } // Needed by Entity Framework

        public Category(string name, Tenant tenant, User creator)
        {
            Name = name;
            TenantId = tenant.Id;
            CreatedBy = creator.Id;
            LastEditedBy = creator.Id;
            CreationDate = DateTimeOffset.UtcNow;
            LastEditDate = DateTimeOffset.UtcNow;
        }


        // Methods

        public override string ToString()
        {
            return $"Category #{Id}: Name: {Name} / TenantId: #{TenantId}";
        }

        public apiCategory ToApiCategory()
        {
            return new apiCategory(Id, Name);
        }
    }
}
