using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migration_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "structured_leads",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    form_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ad_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    campaign_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ad_set_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    synced_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    field_data_first_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    field_data_last_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    field_data_email = table.Column<string>(type: "TEXT", nullable: true),
                    field_data_country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    field_data_phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    field_data_extra = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_structured_leads", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "structured_leads",
                schema: "public");
        }
    }
}
