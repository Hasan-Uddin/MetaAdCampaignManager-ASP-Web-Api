using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "ad_sets",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    campaign_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    synced_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ad_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ads",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ad_set_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    campaign_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    synced_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ads", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "campaigns",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ad_account_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    objective = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    configured_status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    buying_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    budget_remaining = table.Column<int>(type: "INTEGER", nullable: false),
                    can_use_spend_cap = table.Column<bool>(type: "INTEGER", nullable: false),
                    is_skadnetwork_attribution = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    synced_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campaigns", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "forms",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    page_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    locale = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    privacy_policy_url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    privacy_policy_link_text = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    follow_up_action_url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    synced_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_forms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "leads",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    form_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ad_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    campaign_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ad_set_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    field_data = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    synced_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leads", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "meta_settings",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    app_id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    app_secret = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    access_token = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    page_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ad_account_id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    webhook_verify_token = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    page_access_token = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meta_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    google_id = table.Column<string>(type: "TEXT", nullable: false),
                    picture_url = table.Column<string>(type: "TEXT", nullable: false),
                    google_access_token = table.Column<string>(type: "TEXT", nullable: true),
                    google_refresh_token = table.Column<string>(type: "TEXT", nullable: true),
                    password_hash = table.Column<string>(type: "TEXT", nullable: true),
                    time_zone = table.Column<string>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "form_questions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 70, nullable: false),
                    key = table.Column<string>(type: "TEXT", nullable: true),
                    form_id = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    label = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_questions_forms_form_id",
                        column: x => x.form_id,
                        principalSchema: "public",
                        principalTable: "forms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_form_questions_form_id",
                schema: "public",
                table: "form_questions",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "public",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ad_sets",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ads",
                schema: "public");

            migrationBuilder.DropTable(
                name: "campaigns",
                schema: "public");

            migrationBuilder.DropTable(
                name: "form_questions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "leads",
                schema: "public");

            migrationBuilder.DropTable(
                name: "meta_settings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "forms",
                schema: "public");
        }
    }
}
