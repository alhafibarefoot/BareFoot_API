using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class DefaultValuesDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Posts",
                type: "SmallDateTime",
                nullable: true
                );


                // migrationBuilder.AddColumn<DateTime>(
                // name: "CreatedOn",
                // table: "Posts",
                // type: "SmallDateTime",
                // nullable: true,
                // defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Posts");
        }
    }
}
