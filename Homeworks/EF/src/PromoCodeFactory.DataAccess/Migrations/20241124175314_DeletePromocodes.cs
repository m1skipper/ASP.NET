using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DeletePromocodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromoCode_Customers_CustomerId",
                table: "PromoCode");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "PromoCode",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCode_Customers_CustomerId",
                table: "PromoCode",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromoCode_Customers_CustomerId",
                table: "PromoCode");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "PromoCode",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_PromoCode_Customers_CustomerId",
                table: "PromoCode",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
