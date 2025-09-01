using Infoscreens.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using vesact.common.message.v2.Models;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("Subscriptions")]
    public class Subscription : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int UserId { get; set; }

        public int? PushTokenId { get; set; }

        [Required]
        public int InfoscreenId { get; set; }

        [Required]
        public DateTimeOffset LastUpdateDate { get; set; }


        // Foreign Key

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("PushTokenId")]
        public PushToken PushToken { get; set; }

        [ForeignKey("InfoscreenId")]
        public Infoscreen Infoscreen { get; set; }

        // Constructors

        // Needed by EF
        public Subscription() { }

        public Subscription(User user, PushToken pushToken, int infoscreenId)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            UserId = user.Id;
            PushTokenId = pushToken?.Id;
            InfoscreenId = infoscreenId;
            LastUpdateDate = DateTimeOffset.UtcNow;
        }

        // Methods

        public override string ToString()
        {
            return $"Subscription #{Id}: UserId: {UserId} / PushTokenId: {PushTokenId} / InfoscreenId: {InfoscreenId}";
        }
    }
}
