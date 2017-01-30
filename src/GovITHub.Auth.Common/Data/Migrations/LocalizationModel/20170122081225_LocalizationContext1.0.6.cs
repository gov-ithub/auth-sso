using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovITHub.Auth.Common.Data.Migrations.LocalizationModel
{
    public partial class LocalizationContext106 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_LocalizationRecord_Key_LocalizationCulture",
                table: "LocalizationRecord");

            migrationBuilder.AlterColumn<string>(
                name: "ResourceKey",
                table: "LocalizationRecord",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_LocalizationRecord_Key_LocalizationCulture_ResourceKey",
                table: "LocalizationRecord",
                columns: new[] { "Key", "LocalizationCulture", "ResourceKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_LocalizationRecord_Key_LocalizationCulture_ResourceKey",
                table: "LocalizationRecord");

            migrationBuilder.AlterColumn<string>(
                name: "ResourceKey",
                table: "LocalizationRecord",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_LocalizationRecord_Key_LocalizationCulture",
                table: "LocalizationRecord",
                columns: new[] { "Key", "LocalizationCulture" });
        }
    }
}
