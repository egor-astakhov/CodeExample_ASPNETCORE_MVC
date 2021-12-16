using Microsoft.EntityFrameworkCore.Migrations;

namespace Integral.Infrastructure.Persistence.Migrations
{
    public partial class AddProductSortingOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortingOrder",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortingOrder",
                table: "Products");
        }
    }
}
