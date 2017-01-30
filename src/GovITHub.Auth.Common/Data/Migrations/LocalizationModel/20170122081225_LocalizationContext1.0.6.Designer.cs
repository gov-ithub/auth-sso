using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Localization.SqlLocalizer.DbStringLocalizer;

namespace GovITHub.Auth.Common.Data.Migrations.LocalizationModel
{
    [DbContext(typeof(LocalizationModelContext))]
    [Migration("20170122081225_LocalizationContext1.0.6")]
    partial class LocalizationContext106
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Localization.SqlLocalizer.DbStringLocalizer.ExportHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Exported");

                    b.Property<string>("Reason");

                    b.HasKey("Id");

                    b.ToTable("ExportHistory");
                });

            modelBuilder.Entity("Localization.SqlLocalizer.DbStringLocalizer.ImportHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Imported");

                    b.Property<string>("Information");

                    b.HasKey("Id");

                    b.ToTable("ImportHistory");
                });

            modelBuilder.Entity("Localization.SqlLocalizer.DbStringLocalizer.LocalizationRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key")
                        .IsRequired();

                    b.Property<string>("LocalizationCulture")
                        .IsRequired();

                    b.Property<string>("ResourceKey")
                        .IsRequired();

                    b.Property<string>("Text");

                    b.Property<DateTime>("UpdatedTimestamp");

                    b.HasKey("Id");

                    b.HasAlternateKey("Key", "LocalizationCulture", "ResourceKey");

                    b.ToTable("LocalizationRecord");
                });
        }
    }
}
