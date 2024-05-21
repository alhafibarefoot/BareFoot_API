using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class DefaultValues : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
