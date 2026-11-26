using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddDecisionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmitterId",
                table: "catalog_submissions");

            migrationBuilder.AddColumn<Guid>(
                name: "SubmitterId",
                table: "catalog_submissions",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateTable(
                name: "submission_decisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    ResolvedCategoryId = table.Column<int>(type: "integer", nullable: true),
                    ResolvedCategoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ResultObjectId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_decisions", x => x.Id);
                    table.CheckConstraint("CK_submission_decision_category_selection", "(\r\n    (\"ResolvedCategoryId\" IS NULL OR \"ResolvedCategoryName\" IS NULL)\r\n    AND\r\n    (\"ResolvedCategoryName\" IS NULL OR\r\n    length(btrim(\"ResolvedCategoryName\")) BETWEEN 1 AND 100)\r\n)");
                    table.ForeignKey(
                        name: "FK_submission_decisions_catalog_objects_ResultObjectId",
                        column: x => x.ResultObjectId,
                        principalTable: "catalog_objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submission_decisions_catalog_submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "catalog_submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submission_decisions_categories_ResolvedCategoryId",
                        column: x => x.ResolvedCategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_submission_decisions_OperationId",
                table: "submission_decisions",
                column: "OperationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submission_decisions_ResolvedCategoryId",
                table: "submission_decisions",
                column: "ResolvedCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_submission_decisions_ResultObjectId",
                table: "submission_decisions",
                column: "ResultObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_submission_decisions_SubmissionId",
                table: "submission_decisions",
                column: "SubmissionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "submission_decisions");

            migrationBuilder.AlterColumn<int>(
                name: "SubmitterId",
                table: "catalog_submissions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
