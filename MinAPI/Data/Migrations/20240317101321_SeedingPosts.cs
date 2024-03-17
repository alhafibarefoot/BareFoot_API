using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: ["Id", "Title", "Content", "postImage"],
                values: new object[,]
                {
                    { 1, "Post1", "Content Post1", null },
                    { 2, "Post2", "Content Post2", null }
                }
            );


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
