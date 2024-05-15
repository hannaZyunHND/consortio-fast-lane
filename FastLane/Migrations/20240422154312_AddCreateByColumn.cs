using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastLane.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateByColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreateBy",
                table: "Order_Details",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Order_Details");
        }
    }
}
