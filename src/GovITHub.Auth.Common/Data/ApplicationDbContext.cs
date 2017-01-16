using GovITHub.Auth.Common.Data.Models;
using GovITHub.Auth.Common.Models;
using GovITHub.Auth.Common.Services.Audit.DataContracts;
using GovITHub.Auth.Common.Services.DeviceDetection.DataContracts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GovITHub.Auth.Common.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<AuditActionMessage>().ToTable("AuditActions");

            builder.Entity<Organization>(b =>
            {
                b.HasKey(t => t.Id);
                b.HasOne(p => p.Parent).
                    WithMany(c => c.Children).
                    HasForeignKey(p => p.ParentId).
                    HasConstraintName("FK_Org_OrgChild");
            });

            builder.Entity<OrganizationUser>(b =>
            {
                b.HasKey(t => t.Id);
                b.HasOne(p => p.Organization).
                    WithMany(c => c.Users).
                    HasForeignKey(p => p.OrganizationId).
                    HasConstraintName("FK_Org_OrgUsers");
                b.HasOne(p => p.User).
                    WithMany(c => c.OrganizationUsers).
                    HasPrincipalKey(p => p.Id).
                    HasForeignKey(c => c.UserId).
                    HasConstraintName("FK_AppUser_OrgUser");
            });

            builder.Entity<OrganizationSetting>(b =>
            {
                b.HasKey(t => t.Id);
                b.HasOne(p => p.Organization).
                    WithOne(c => c.OrganizationSetting).
                    HasConstraintName("FK_Org_OrgSettings");
                b.HasOne(p => p.EmailSetting).
                    WithOne(c => c.OrganizationSetting).
                    HasForeignKey<OrganizationSetting>(p => p.EmailSettingId).
                    HasConstraintName("FK_OrgSetting_EmailSetting");
            });
            builder.Entity<EmailTemplate>(b =>
            {
                b.HasKey(t => t.Id);
                b.HasOne(p => p.OrganizationSetting).
                    WithMany(c => c.EmailTemplates).
                    HasForeignKey(p => p.OrganizationSettingId).
                    HasConstraintName("FK_OrgSetting_EmailTemplate");
            });
            builder.Entity<EmailSetting>(b =>
            {
                b.HasKey(t => t.Id);
                b.HasOne(p => p.EmailProvider).
                    WithMany(c => c.EmailSettings).
                    HasForeignKey(c => c.EmailProviderId).
                    HasConstraintName("FK_EmailSetting_EmailProvider");
            });
            builder.Entity<OrganizationClient>(b =>
            {
                b.HasKey(t => new { t.OrganizationId, t.ClientId });
                b.HasOne(p => p.Organization).
                    WithMany(c => c.OrganizationClients).
                    HasForeignKey(c => c.OrganizationId).
                    HasConstraintName("FK_Org_OrgClient");
            });
            builder.Entity<LoginDevice>(ld =>
            {
                ld.ToTable("LoginDevices");
                ld.HasKey(_ => _.Id);
            });
            builder.Entity<UserLoginDevice>(uld =>
            {
                uld.ToTable("UserLoginDevices");
                uld.HasKey(_ => new
                {
                    _.UserId,
                    _.DeviceId
                });
            });
        }

        public DbSet<AuditActionMessage> AuditActions { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationUser> OrganizationUsers { get; set; }
        public DbSet<OrganizationSetting> OrganizationSettings { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailSetting> EmailSettings { get; set; }
        public DbSet<EmailProvider> EmailProviders { get; set; }
        public DbSet<OrganizationClient> OrganizationClients { get; set; }
        public DbSet<LoginDevice> LoginDevices { get; set; }
        public DbSet<UserLoginDevice> UserLoginDevices { get; set; }
    }
}
