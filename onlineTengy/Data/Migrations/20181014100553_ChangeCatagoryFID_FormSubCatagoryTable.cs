using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace onlineTengy.Data.Migrations
{
    public partial class ChangeCatagoryFID_FormSubCatagoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCatagories_Catagories_subCatagoryId",
                table: "SubCatagories");

            migrationBuilder.DropIndex(
                name: "IX_SubCatagories_subCatagoryId",
                table: "SubCatagories");

            migrationBuilder.DropColumn(
                name: "subCatagoryId",
                table: "SubCatagories");

            migrationBuilder.CreateIndex(
                name: "IX_SubCatagories_CatagoryId",
                table: "SubCatagories",
                column: "CatagoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCatagories_Catagories_CatagoryId",
                table: "SubCatagories",
                column: "CatagoryId",
                principalTable: "Catagories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCatagories_Catagories_CatagoryId",
                table: "SubCatagories");

            migrationBuilder.DropIndex(
                name: "IX_SubCatagories_CatagoryId",
                table: "SubCatagories");

            migrationBuilder.AddColumn<int>(
                name: "subCatagoryId",
                table: "SubCatagories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubCatagories_subCatagoryId",
                table: "SubCatagories",
                column: "subCatagoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCatagories_Catagories_subCatagoryId",
                table: "SubCatagories",
                column: "subCatagoryId",
                principalTable: "Catagories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
