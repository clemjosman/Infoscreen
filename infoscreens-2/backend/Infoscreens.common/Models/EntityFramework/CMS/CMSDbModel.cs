using Microsoft.EntityFrameworkCore;
using vesact.common.file.Interfaces;
using vesact.common.file.Models;
using vesact.common.message.v2.Interfaces;
using vesact.common.message.v2.Models;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    public class CMSDbModel : DbContext, IFileDbModel, IMessageDbModel
    {
        public CMSDbModel(DbContextOptions<CMSDbModel> options) : base(options)
        { }

        #region Properties

        //Add DbSets..

        public virtual DbSet<File> Files { get; set; }

        public DbSet<PushToken> PushTokens { get; set; }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Infoscreen> Infoscreens { get; set; }
        public virtual DbSet<InfoscreenGroup> InfoscreensGroups { get; set; }
        public virtual DbSet<InfoscreenNews> InfoscreensNews { get; set; }
        public virtual DbSet<InfoscreenVideo> InfoscreensVideos { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsCategory> NewsCategories { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<TranslatedText> TranslatedTexts { get; set; }
        public virtual DbSet<Translation> Translations { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserTenant> UsersTenants { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<VideoCategory> VideosCategories { get; set; }

        #endregion Properties

        #region StoredProcedures

        #endregion StoredProcedures

        #region Methods

        //Beware : Just called the first time a context is created (looks like it is static)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique keys
            modelBuilder.Entity<Language>().HasIndex(e => e.Iso2).IsUnique(true);
            modelBuilder.Entity<InfoscreenNews>().HasIndex(e => new { e.InfoscreenId, e.NewsId }).IsUnique(true);
            modelBuilder.Entity<InfoscreenVideo>().HasIndex(e => new { e.InfoscreenId, e.VideoId }).IsUnique(true);
            modelBuilder.Entity<User>().HasIndex(u => u.ObjectId).IsUnique(true);
            modelBuilder.Entity<UserTenant>().HasIndex(ut => new { ut.UserId, ut.TenantId }).IsUnique(true);
            modelBuilder.Entity<NewsCategory>().HasIndex(nt => new { nt.NewsId, nt.CategoryId }).IsUnique(true);
            modelBuilder.Entity<VideoCategory>().HasIndex(vt => new { vt.VideoId, vt.CategoryId }).IsUnique(true);
            modelBuilder.Entity<Tenant>().HasIndex(t => t.Code).IsUnique(true);
        }

        #endregion Methods
    }
}
