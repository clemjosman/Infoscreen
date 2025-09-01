using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.API.Mobile;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("Users")]
    public class User : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        [StringLength(100)]
        public string ObjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(100)]
        public string Upn { get; set; }

        [Required]
        [StringLength(2)]
        public string Iso2 { get; set; } // ISO 639-1

        public int? SelectedTenantId { get; set; }


        [Required]
        public DateTimeOffset CreationDate { get; set; }

        public DateTimeOffset? DeletionDate { get; set; }


        // Foreign Key

        [ForeignKey("SelectedTenantId")]
        public Tenant SelectedTenant { get; set; }



        // Relations

        [InverseProperty("User")]
        public ICollection<UserTenant> UserTenants { get; set; }

        [InverseProperty("Creator")]
        public ICollection<Category> CreatedCategories { get; set; }

        [InverseProperty("LastEditor")]
        public ICollection<Category> LastEditorOnCategories { get; set; }

        [InverseProperty("DeletedByUser")]
        public ICollection<Category> DeletedCategories { get; set; }

        [InverseProperty("Creator")]
        public ICollection<News> CreatedNews { get; set; }

        [InverseProperty("LastEditor")]
        public ICollection<News> LastEditorOnNews { get; set; }

        [InverseProperty("DeletedByUser")]
        public ICollection<News> DeletedNews { get; set; }

        [InverseProperty("Creator")]
        public ICollection<Video> CreatedVideos { get; set; }

        [InverseProperty("LastEditor")]
        public ICollection<Video> LastEditorOnVideos { get; set; }

        [InverseProperty("DeletedByUser")]
        public ICollection<Video> DeletedVideos { get; set; }

        [InverseProperty("User")]
        public ICollection<Subscription> Subscriptions { get; set; }

        // INFO: No relation defined between Users and Files as the UserId of the Files table is stored as a string value (like the TenantId)
        // Therefore no foreign key is defined in the database and UserId must be checked in the code or Files must be queried by UserId directly.


        // Constructors

        // Needed by EF
        public User() { }

        public User(apiRegisterUser_Mobile register)
        {
            ObjectId = register.ObjectId;
            Upn = register.Upn;
            DisplayName = register.DisplayName;
            Iso2 = register.Iso2;
            CreationDate = DateTimeOffset.UtcNow;
        }

        // Methods

        public override string ToString()
        {
            return $"User #{Id}: ObjectId: {ObjectId} / DisplayName: {DisplayName} / UPN: {Upn} / Language: #{Iso2}";
        }

        public apiUser_Light ToApiUser_Light()
        {
            return new apiUser_Light(Id, DisplayName);
        }

        public async Task<apiUser_Me> ToApiUser_MeAsync(IDatabaseRepository _databaseRepository)
        {
            List<apiTenant> tenants = new ();
            UserTenants ??= await _databaseRepository.GetUserTenantsFromUserAsync(Id);
            tenants = UserTenants.Select(ut => ut.Tenant.ToApiTenant()).ToList();

            apiTenant selectedTenant = null;
            if (SelectedTenantId.HasValue) {
                SelectedTenant ??= await _databaseRepository.GetTenantByIdAsync(SelectedTenantId.Value);
                selectedTenant = SelectedTenant.ToApiTenant();
            }

            var apiUser_Me = new apiUser_Me(Id, DisplayName, ObjectId, Upn, Iso2, tenants, selectedTenant);

            return apiUser_Me.CheckConsistancy() ? apiUser_Me : null;
        }
    }
}
