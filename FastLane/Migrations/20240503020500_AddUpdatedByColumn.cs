using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastLane.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedByColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Order_Details",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Order_Details");
        }
    }
}
