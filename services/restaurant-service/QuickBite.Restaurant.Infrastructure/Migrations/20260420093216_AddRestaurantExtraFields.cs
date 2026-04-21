using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickBite.Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Restaurants",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedDeliveryTimeInMinutes",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumOrderAmount",
                table: "Restaurants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryTimeInMinutes",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "MinimumOrderAmount",
                table: "Restaurants");
        }
    }
}
