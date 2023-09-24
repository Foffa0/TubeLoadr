using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeDownloader.Migrations
{
    /// <inheritdoc />
    public partial class twoVideoModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoDTO",
                table: "VideoDTO");

            migrationBuilder.RenameTable(
                name: "VideoDTO",
                newName: "QueuedVideos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueuedVideos",
                table: "QueuedVideos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    Channel = table.Column<string>(type: "TEXT", nullable: false),
                    Thumbnail = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueuedVideos",
                table: "QueuedVideos");

            migrationBuilder.RenameTable(
                name: "QueuedVideos",
                newName: "VideoDTO");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoDTO",
                table: "VideoDTO",
                column: "Id");
        }
    }
}
