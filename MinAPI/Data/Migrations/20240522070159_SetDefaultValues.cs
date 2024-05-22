using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "postImage",
                table: "Posts",
                type: "TEXT",
                nullable: true,
                defaultValue: "Post.jfif",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Posts",
                type: "SmallDateTime",
                nullable: true,
                defaultValueSql: "datetime('now', 'utc')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "postImage",
                table: "Posts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValue: "Post.jfif");
        }
    }
}
