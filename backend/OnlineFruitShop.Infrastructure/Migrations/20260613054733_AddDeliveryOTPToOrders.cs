using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineFruitShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryOTPToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryOTP",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpiryTime",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryOTP",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OTPExpiryTime",
                table: "Orders");
        }
    }
}
