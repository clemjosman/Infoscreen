using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("InfoscreenGroups")]
    public class InfoscreenGroup : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public int TenantId { get; set; }


        // Foreign Keys

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }


        // Relations

        [InverseProperty("InfoscreenGroup")]
        public ICollection<Infoscreen> Infoscreens { get; set; }



        // Methods

        public override string ToString()
        {
            return $"Infoscreen Group #{Id}: {Name} / TenantId: {TenantId}";
        }

        public apiInfoscreenGroup ToApiInfoscreenGroup()
        {
            return new apiInfoscreenGroup(Id, Name);
        }
    }
}
