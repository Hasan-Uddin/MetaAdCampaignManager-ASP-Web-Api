using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migration_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "google_access_token",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "google_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "google_refresh_token",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_hash",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "app_id",
                schema: "public",
                table: "meta_settings");

            migrationBuilder.DropColumn(
                name: "app_secret",
                schema: "public",
                table: "meta_settings");

            migrationBuilder.DropColumn(
                name: "webhook_verify_token",
                schema: "public",
                table: "meta_settings");

            migrationBuilder.RenameColumn(
                name: "time_zone",
                schema: "public",
                table: "users",
                newName: "facebook_id");

            migrationBuilder.AlterColumn<string>(
                name: "picture_url",
                schema: "public",
                table: "users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "public",
                table: "users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "access_token_expires_at",
                schema: "public",
                table: "meta_settings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "public",
                table: "meta_settings",
                type: "TEXT",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "ix_meta_settings_user_id",
                schema: "public",
                table: "meta_settings",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_meta_settings_users_user_id",
                schema: "public",
                table: "meta_settings",
                column: "user_id",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_meta_settings_users_user_id",
                schema: "public",
                table: "meta_settings");

            migrationBuilder.DropIndex(
                name: "ix_meta_settings_user_id",
                schema: "public",
                table: "meta_settings");

            migrationBuilder.DropColumn(
                name: "access_token_expires_at",
                schema: "public",
                table: "meta_settings");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "public",
                table: "meta_settings");

            migrationBuilder.RenameColumn(
                name: "facebook_id",
                schema: "public",
                table: "users",
                newName: "time_zone");

            migrationBuilder.AlterColumn<string>(
                name: "picture_url",
                schema: "public",
                table: "users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "public",
                table: "users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "google_access_token",
                schema: "public",
                table: "users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "google_id",
                schema: "public",
                table: "users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "google_refresh_token",
                schema: "public",
                table: "users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                schema: "public",
                table: "users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "app_id",
                schema: "public",
                table: "meta_settings",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "app_secret",
                schema: "public",
                table: "meta_settings",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "webhook_verify_token",
                schema: "public",
                table: "meta_settings",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
