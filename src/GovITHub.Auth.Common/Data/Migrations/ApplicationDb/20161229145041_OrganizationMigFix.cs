using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovITHub.Auth.Common.Data.Migrations.ApplicationDb
{
    public partial class OrganizationMigFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrgClient_Organization",
                table: "OrganizationClient");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_OrgUser",
                table:"OrganizationUser");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUser");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_OrgUser",
                table: "OrganizationUser",
                column:"UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Org_OrgClient",
                table: "OrganizationClient",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Org_OrgClient",
                table: "OrganizationClient");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_OrgUser",
                table: "OrganizationUser");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUser");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUser",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_OrgUser",
                table: "OrganizationUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrgClient_Organization",
                table: "OrganizationClient",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
