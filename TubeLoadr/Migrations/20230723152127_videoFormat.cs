using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeDownloader.Migrations
{
    /// <inheritdoc />
    public partial class videoFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "QueuedVideos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Format",
                table: "QueuedVideos");
        }
    }
}
