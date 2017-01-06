using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovITHub.Auth.Common.Data.Migrations.ApplicationDb
{
    public partial class OrganizationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Org_OrgUsers",
                table: "OrganizationUser");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_OrgUser",
                table: "OrganizationUser");

            migrationBuilder.DropIndex(
               name: "IX_OrganizationUser_OrganizationId",
               table: "OrganizationUser");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUser");


            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_OrganizationId_UserId",
                table: "OrganizationUser",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
               name: "FK_Org_OrgUsers",
               table: "OrganizationUser",
               column: "OrganizationId",
               principalTable: "Organization",
               principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_OrgUser",
                table: "OrganizationUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
               name: "FK_Org_OrgUsers",
               table: "OrganizationUser");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_OrgUser",
                table: "OrganizationUser");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUser_OrganizationId_UserId",
                table: "OrganizationUser");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_OrganizationId",
                table: "OrganizationUser",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUser",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
               name: "FK_Org_OrgUsers",
               table: "OrganizationUser",
               column: "OrganizationId",
               principalTable: "Organization",
               principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_OrgUser",
                table: "OrganizationUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
