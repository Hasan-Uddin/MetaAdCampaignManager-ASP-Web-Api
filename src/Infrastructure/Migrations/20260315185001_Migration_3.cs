using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migration_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "template_id",
                schema: "public",
                table: "forms",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "form_templates",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    questions = table.Column<string>(type: "TEXT", nullable: false),
                    is_default = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_templates", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "form_templates",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "template_id",
                schema: "public",
                table: "forms");
        }
    }
}
