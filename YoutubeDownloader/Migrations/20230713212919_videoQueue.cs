using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeDownloader.Migrations
{
    /// <inheritdoc />
    public partial class videoQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Videos",
                table: "Videos");

            migrationBuilder.RenameTable(
                name: "Videos",
                newName: "VideoDTO");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoDTO",
                table: "VideoDTO",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoDTO",
                table: "VideoDTO");

            migrationBuilder.RenameTable(
                name: "VideoDTO",
                newName: "Videos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Videos",
                table: "Videos",
                column: "Id");
        }
    }
}
