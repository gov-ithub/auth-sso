using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovITHub.Auth.Identity.Data.Migrations.LocalizationModel
{
    public partial class Localization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExportHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Exported = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Imported = table.Column<DateTime>(nullable: false),
                    Information = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationRecord",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Key = table.Column<string>(nullable: false),
                    LocalizationCulture = table.Column<string>(nullable: false),
                    ResourceKey = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    UpdatedTimestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationRecord", x => x.Id);
                    table.UniqueConstraint("AK_LocalizationRecord_Key_LocalizationCulture", x => new { x.Key, x.LocalizationCulture });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportHistory");

            migrationBuilder.DropTable(
                name: "ImportHistory");

            migrationBuilder.DropTable(
                name: "LocalizationRecord");
        }
    }
}
