using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("Infoscreens")]
    public class Infoscreen : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        [StringLength(200)]
        public string NodeId { get; set; }

        [StringLength(200)]
        public string MsbNodeId { get; set; }

        [Required]
        public int InfoscreenGroupId { get; set; }

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(256)]
        public string ApiKey1 { get; set; }

        [Required]
        [StringLength(256)]
        public string ApiKey2 { get; set; }

        [Required]
        public int DefaultContentLanguageId { get; set; }

        [StringLength(255)]
        public string ContentAdminEmail { get; set; }

        public bool SendMailNoContent { get; set; }


        // Foreign Keys

        [ForeignKey("InfoscreenGroupId")]
        public InfoscreenGroup InfoscreenGroup { get; set; }

        [ForeignKey("DefaultContentLanguageId")]
        public Language DefaultContentLanguage { get; set; }


        // Relations

        [InverseProperty("Infoscreen")]
        public ICollection<InfoscreenNews> InfoscreenNews { get; set; }

        [InverseProperty("Infoscreen")]
        public ICollection<InfoscreenVideo> InfoscreenVideos { get; set; }

        [InverseProperty("Infoscreen")]
        public ICollection<Subscription> Subscriptions { get; set; }


        // Methods

        public override string ToString()
        {
            return $"Infoscreen #{Id}: NodeId: {NodeId} / Infoscreen Group: #{InfoscreenGroupId} / DisplayName: {DisplayName}";
        }

        public bool IsVirtualInfoscreen()
        {
            return string.IsNullOrWhiteSpace(MsbNodeId);
        }

        public apiInfoscreen_Light ToApiInfoscreen_Light(IDatabaseRepository databaseRepository)
        {
            return new apiInfoscreen_Light(this, databaseRepository);
        }
    }
}
