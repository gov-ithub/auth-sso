using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovITHub.Auth.Common.Data.Migrations.ApplicationDb
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20161130094655_AddDeviceDetection")]
    partial class AddDeviceDetection
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.EmailProvider", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 20);

                    b.HasKey("Id");

                    b.ToTable("EmailProvider");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.EmailSetting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("EmailProviderId");

                    b.Property<long?>("OrganizationSettingId");

                    b.Property<string>("Settings")
                        .HasAnnotation("MaxLength", 8196);

                    b.HasKey("Id");

                    b.HasIndex("EmailProviderId");

                    b.HasIndex("OrganizationSettingId")
                        .IsUnique();

                    b.ToTable("EmailSetting");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.EmailTemplate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 20);

                    b.Property<long>("OrganizationSettingId");

                    b.Property<string>("Value")
                        .HasAnnotation("MaxLength", 8196);

                    b.HasKey("Id");

                    b.HasIndex("OrganizationSettingId");

                    b.ToTable("EmailTemplate");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.Organization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<long?>("ParentId");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.OrganizationClient", b =>
                {
                    b.Property<long>("OrganizationId");

                    b.Property<int>("ClientId");

                    b.HasKey("OrganizationId", "ClientId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("OrganizationClient");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.OrganizationSetting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowSelfRegister");

                    b.Property<string>("DomainRestriction")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("EmailSettingId");

                    b.Property<long?>("OrganizationId");

                    b.Property<bool>("UseDomainRestriction");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId")
                        .IsUnique();

                    b.ToTable("OrganizationSetting");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.OrganizationUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Level");

                    b.Property<long?>("OrganizationId");

                    b.Property<short>("Status");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("OrganizationUser");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 255);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 255);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 255);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 255);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Services.Audit.DataContracts.AuditActionMessage", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ActionUrl");

                    b.Property<string>("IpV4");

                    b.Property<string>("IpV6");

                    b.Property<DateTimeOffset>("Timestamp");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("AuditActions");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 255);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 255);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.EmailSetting", b =>
                {
                    b.HasOne("GovITHub.Auth.Identity.Data.Models.EmailProvider", "EmailProvider")
                        .WithMany("EmailSettings")
                        .HasForeignKey("EmailProviderId")
                        .HasConstraintName("FK_EmailSetting_EmailProvider")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GovITHub.Auth.Identity.Data.Models.OrganizationSetting", "OrganizationSetting")
                        .WithOne("EmailSetting")
                        .HasForeignKey("GovITHub.Auth.Identity.Data.Models.EmailSetting", "OrganizationSettingId")
                        .HasConstraintName("FK_OrgSetting_EmailSetting");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.EmailTemplate", b =>
                {
                    b.HasOne("GovITHub.Auth.Identity.Data.Models.OrganizationSetting", "OrganizationSetting")
                        .WithMany("EmailTemplates")
                        .HasForeignKey("OrganizationSettingId")
                        .HasConstraintName("FK_OrgSetting_EmailTemplate")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.Organization", b =>
                {
                    b.HasOne("GovITHub.Auth.Identity.Data.Models.Organization", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_Org_OrgChild");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.OrganizationClient", b =>
                {
                    b.HasOne("GovITHub.Auth.Identity.Data.Models.Organization", "Organization")
                        .WithMany("OrganizationClients")
                        .HasForeignKey("OrganizationId")
                        .HasConstraintName("FK_OrgClient_Organization")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.OrganizationSetting", b =>
                {
                    b.HasOne("GovITHub.Auth.Identity.Data.Models.Organization", "Organization")
                        .WithOne("OrganizationSetting")
                        .HasForeignKey("GovITHub.Auth.Identity.Data.Models.OrganizationSetting", "OrganizationId")
                        .HasConstraintName("FK_Org_OrgSettings");
                });

            modelBuilder.Entity("GovITHub.Auth.Identity.Data.Models.OrganizationUser", b =>
                {
                    b.HasOne("GovITHub.Auth.Identity.Data.Models.Organization", "Organization")
                        .WithMany("Users")
                        .HasForeignKey("OrganizationId")
                        .HasConstraintName("FK_Org_OrgUsers");

                    b.HasOne("GovITHub.Auth.Identity.Models.ApplicationUser", "User")
                        .WithOne("OrganizationUser")
                        .HasForeignKey("GovITHub.Auth.Identity.Data.Models.OrganizationUser", "UserId")
                        .HasConstraintName("FK_AppUser_OrgUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GovITHub.Auth.Identity.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GovITHub.Auth.Identity.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GovITHub.Auth.Identity.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
