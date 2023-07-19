using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeDownloader.Migrations
{
    /// <inheritdoc />
    public partial class videoFilepath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "QueuedVideos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "QueuedVideos");
        }
    }
}
