using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovITHub.Auth.Common.Data.Migrations.ConfigurationDb
{
    public partial class IdSrv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlwaysIncludeUserClaimsInIdToken",
                table: "Clients",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlwaysIncludeUserClaimsInIdToken",
                table: "Clients");
        }
    }
}
