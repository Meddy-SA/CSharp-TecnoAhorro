using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TecnoCredito.Migrations
{
    /// <inheritdoc />
    public partial class FixSysMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SysMenuItems_SysMenuCategories_SysMenuCategoryId",
                table: "SysMenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SysMenuItems_SysMenuItems_SysMenuItemId",
                table: "SysMenuItems");

            migrationBuilder.AlterColumn<string>(
                name: "Style",
                table: "SysMenuItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SysMenuItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "SysMenuItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Command",
                table: "SysMenuItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Badge",
                table: "SysMenuItems",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SysMenuCategories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_SysMenuItems_SysMenuCategories_SysMenuCategoryId",
                table: "SysMenuItems",
                column: "SysMenuCategoryId",
                principalTable: "SysMenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SysMenuItems_SysMenuItems_SysMenuItemId",
                table: "SysMenuItems",
                column: "SysMenuItemId",
                principalTable: "SysMenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SysMenuItems_SysMenuCategories_SysMenuCategoryId",
                table: "SysMenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SysMenuItems_SysMenuItems_SysMenuItemId",
                table: "SysMenuItems");

            migrationBuilder.AlterColumn<string>(
                name: "Style",
                table: "SysMenuItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SysMenuItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "SysMenuItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Command",
                table: "SysMenuItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Badge",
                table: "SysMenuItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SysMenuCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "FK_SysMenuItems_SysMenuCategories_SysMenuCategoryId",
                table: "SysMenuItems",
                column: "SysMenuCategoryId",
                principalTable: "SysMenuCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SysMenuItems_SysMenuItems_SysMenuItemId",
                table: "SysMenuItems",
                column: "SysMenuItemId",
                principalTable: "SysMenuItems",
                principalColumn: "Id");
        }
    }
}
