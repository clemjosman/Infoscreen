using Infoscreens.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("UsersTenants")]
    public class UserTenant : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int UserId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public DateTimeOffset CreationDate { get; set; }


        // Foreign Keys

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }



        // Methods

        public override string ToString()
        {
            return $"UserTenant #{Id}: UserId: {UserId} / TenantId: {TenantId}";
        }
    }
}
