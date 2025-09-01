using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("Tenants")]
    public class Tenant : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }

        [Required]
        public bool NotifyUsers { get; set; }

        [Required]
        public DateTimeOffset CreationDate { get; set; }

        public DateTimeOffset? DeletionDate { get; set; }

        [StringLength(100)]
        public string AppName { get; set; }

        [StringLength(255)]
        public string ContentAdminEmail { get; set; }


        // Relations

        [InverseProperty("SelectedTenant")]
        public ICollection<User> UsersSelectingTenant { get; set; }

        [InverseProperty("Tenant")]
        public ICollection<UserTenant> UserTenants { get; set; }

        [InverseProperty("Tenant")]
        public ICollection<InfoscreenGroup> InfoscreenGroups { get; set; }

        [InverseProperty("Tenant")]
        public ICollection<Category> Categories { get; set; }

        [InverseProperty("Tenant")]
        public ICollection<News> News { get; set; }

        [InverseProperty("Tenant")]
        public ICollection<Video> Videos { get; set; }

        // INFO: No relation defined between Tenants and Files as the TenantId of the Files table is stored as a string value (like the UserId)
        // Therefore no foreign key is defined in the database and TenantId must be checked in the code or Files must be queried by TenantId directly.



        // Methods

        public override string ToString()
        {
            return $"Tenant #{Id}: DisplayName: {DisplayName}";
        }

        public apiTenant ToApiTenant()
        {
            return new apiTenant(Id, Code, DisplayName, AppName, ContentAdminEmail);
        }
    }
}
