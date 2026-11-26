using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class CreateSubmissionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "submission_decisions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ResultCategoryId",
                table: "submission_decisions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_submission_decisions_ResultCategoryId",
                table: "submission_decisions",
                column: "ResultCategoryId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_submission_decisions_result",
                table: "submission_decisions",
                sql: "(\r\n    (\"Action\" = 'Approve' AND \"ResultObjectId\" IS NOT NULL AND \"ResultCategoryId\" IS NOT NULL)\r\n    OR\r\n    (\"Action\" = 'Reject' AND \"ResultObjectId\" IS NULL AND \"ResultCategoryId\" IS NULL)\r\n)");

            migrationBuilder.AddForeignKey(
                name: "FK_submission_decisions_categories_ResultCategoryId",
                table: "submission_decisions",
                column: "ResultCategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_submission_decisions_categories_ResultCategoryId",
                table: "submission_decisions");

            migrationBuilder.DropIndex(
                name: "IX_submission_decisions_ResultCategoryId",
                table: "submission_decisions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_submission_decisions_result",
                table: "submission_decisions");

            migrationBuilder.DropColumn(
                name: "ResultCategoryId",
                table: "submission_decisions");

            migrationBuilder.AlterColumn<int>(
                name: "Action",
                table: "submission_decisions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
