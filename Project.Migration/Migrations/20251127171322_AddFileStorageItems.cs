using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migration.Migrations
{
    /// <inheritdoc />
    public partial class AddFileStorageItems : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileStorageItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    Extension = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    StorageProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Folder = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorageItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileStorageItems_Hash",
                table: "FileStorageItems",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorageItems_StorageProvider_StoragePath",
                table: "FileStorageItems",
                columns: new[] { "StorageProvider", "StoragePath" });

            migrationBuilder.CreateIndex(
                name: "IX_FileStorageItems_TenantId_EntityId_EntityType",
                table: "FileStorageItems",
                columns: new[] { "TenantId", "EntityId", "EntityType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileStorageItems");
        }
    }
}
