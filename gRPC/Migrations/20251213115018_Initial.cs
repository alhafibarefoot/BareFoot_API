using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gRPC.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    postImage = table.Column<string>(type: "TEXT", nullable: true, defaultValue: "Post.jfif"),
                    CreatedOn = table.Column<DateTime>(type: "SmallDateTime", nullable: true, defaultValueSql: "datetime('now', 'utc')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
