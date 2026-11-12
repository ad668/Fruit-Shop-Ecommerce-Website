using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineFruitShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AssignOrdersToDeliveryPartners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryPartnerUserId",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryPartnerUserId",
                table: "Orders",
                column: "DeliveryPartnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_DeliveryPartnerUserId",
                table: "Orders",
                column: "DeliveryPartnerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_DeliveryPartnerUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryPartnerUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPartnerUserId",
                table: "Orders");
        }
    }
}
