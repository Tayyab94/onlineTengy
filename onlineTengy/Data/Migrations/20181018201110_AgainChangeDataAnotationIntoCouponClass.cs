using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace onlineTengy.Data.Migrations
{
    public partial class AgainChangeDataAnotationIntoCouponClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Pitcher",
                table: "Coupons",
                nullable: true,
                oldClrType: typeof(byte[]));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Pitcher",
                table: "Coupons",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldNullable: true);
        }

       
    }
}
