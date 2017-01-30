using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GovITHub.Auth.Common.Data;
using GovITHub.Auth.Common.Data.Models;

namespace GovITHub.Auth.Common.Data.Migrations.ApplicationDb
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.EmailProvider", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("EmailProvider");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.EmailSetting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("EmailProviderId");

                    b.Property<string>("Settings")
                        .HasMaxLength(8196);

                    b.HasKey("Id");

                    b.HasIndex("EmailProviderId");

                    b.ToTable("EmailSetting");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.EmailTemplate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<long>("OrganizationSettingId");

                    b.Property<string>("Value")
                        .HasMaxLength(8196);

                    b.HasKey("Id");

                    b.HasIndex("OrganizationSettingId");

                    b.ToTable("EmailTemplate");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.Organization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<long?>("ParentId");

                    b.Property<string>("Website");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.OrganizationClient", b =>
                {
                    b.Property<long>("OrganizationId");

                    b.Property<int>("ClientId");

                    b.HasKey("OrganizationId", "ClientId");

                    b.ToTable("OrganizationClient");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.OrganizationSetting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowSelfRegister");

                    b.Property<string>("DomainRestriction")
                        .HasMaxLength(50);

                    b.Property<long?>("EmailSettingId");

                    b.Property<long?>("OrganizationId");

                    b.Property<bool>("UseDomainRestriction");

                    b.HasKey("Id");

                    b.HasIndex("EmailSettingId")
                        .IsUnique();

                    b.HasIndex("OrganizationId")
                        .IsUnique();

                    b.ToTable("OrganizationSetting");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.OrganizationUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Level");

                    b.Property<long?>("OrganizationId");

                    b.Property<short>("Status");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("UserId");

                    b.ToTable("OrganizationUser");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(255);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(255);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Services.Audit.DataContracts.AuditActionMessage", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActionUrl");

                    b.Property<string>("IpV4");

                    b.Property<string>("IpV6");

                    b.Property<DateTimeOffset>("Timestamp");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("AuditActions");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Services.DeviceDetection.DataContracts.LoginDevice", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Browser");

                    b.Property<string>("MobileDevice");

                    b.Property<string>("OperatingSystem");

                    b.Property<string>("UserAgent");

                    b.HasKey("Id");

                    b.ToTable("LoginDevices");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Services.DeviceDetection.DataContracts.UserLoginDevice", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("DeviceId");

                    b.Property<DateTime>("LastLoginTimeUtc");

                    b.Property<DateTime>("RegistrationTimeUtc");

                    b.HasKey("UserId", "DeviceId");

                    b.ToTable("UserLoginDevices");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(255);

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

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.EmailSetting", b =>
                {
                    b.HasOne("GovITHub.Auth.Common.Data.Models.EmailProvider", "EmailProvider")
                        .WithMany("EmailSettings")
                        .HasForeignKey("EmailProviderId")
                        .HasConstraintName("FK_EmailSetting_EmailProvider")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.EmailTemplate", b =>
                {
                    b.HasOne("GovITHub.Auth.Common.Data.Models.OrganizationSetting", "OrganizationSetting")
                        .WithMany("EmailTemplates")
                        .HasForeignKey("OrganizationSettingId")
                        .HasConstraintName("FK_OrgSetting_EmailTemplate")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.Organization", b =>
                {
                    b.HasOne("GovITHub.Auth.Common.Data.Models.Organization", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_Org_OrgChild");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.OrganizationClient", b =>
                {
                    b.HasOne("GovITHub.Auth.Common.Data.Models.Organization", "Organization")
                        .WithMany("OrganizationClients")
                        .HasForeignKey("OrganizationId")
                        .HasConstraintName("FK_Org_OrgClient")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.OrganizationSetting", b =>
                {
                    b.HasOne("GovITHub.Auth.Common.Data.Models.EmailSetting", "EmailSetting")
                        .WithOne("OrganizationSetting")
                        .HasForeignKey("GovITHub.Auth.Common.Data.Models.OrganizationSetting", "EmailSettingId")
                        .HasConstraintName("FK_OrgSetting_EmailSetting");

                    b.HasOne("GovITHub.Auth.Common.Data.Models.Organization", "Organization")
                        .WithOne("OrganizationSetting")
                        .HasForeignKey("GovITHub.Auth.Common.Data.Models.OrganizationSetting", "OrganizationId")
                        .HasConstraintName("FK_Org_OrgSettings");
                });

            modelBuilder.Entity("GovITHub.Auth.Common.Data.Models.OrganizationUser", b =>
                {
                    b.HasOne("GovITHub.Auth.Common.Data.Models.Organization", "Organization")
                        .WithMany("Users")
                        .HasForeignKey("OrganizationId")
                        .HasConstraintName("FK_Org_OrgUsers");

                    b.HasOne("GovITHub.Auth.Common.Models.ApplicationUser", "User")
                        .WithMany("OrganizationUsers")
                        .HasForeignKey("UserId")
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
                    b.HasOne("GovITHub.Auth.Common.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GovITHub.Auth.Common.Models.ApplicationUser")
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

                    b.HasOne("GovITHub.Auth.Common.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
