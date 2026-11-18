using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    supports_status = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    allowed_statuses = table.Column<List<string>>(type: "text[]", nullable: false, defaultValueSql: "ARRAY[]::text[]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "catalog_objects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    ImageKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_objects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_catalog_objects_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "catalog_submissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    ProposedCategoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ImageKey = table.Column<string>(type: "text", nullable: true),
                    SubmitterId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResultObjectId = table.Column<int>(type: "integer", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_submissions", x => x.Id);
                    table.CheckConstraint("CK_catalog_submissions_category_selection", "((\"CategoryId\" IS NOT NULL AND \"ProposedCategoryName\" IS NULL) OR\n(\"CategoryId\" IS NULL AND \"ProposedCategoryName\" IS NOT NULL AND\nlength(btrim(\"ProposedCategoryName\")) BETWEEN 1 AND 100))");
                    table.ForeignKey(
                        name: "FK_catalog_submissions_catalog_objects_ResultObjectId",
                        column: x => x.ResultObjectId,
                        principalTable: "catalog_objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalog_submissions_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "submission_decisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubmissionId = table.Column<int>(type: "integer", nullable: false),
                    Decision = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ModeratorId = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_decisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_submission_decisions_catalog_submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "catalog_submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalog_objects_CategoryId",
                table: "catalog_objects",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_objects_title_btree",
                table: "catalog_objects",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_objects_title_trgm",
                table: "catalog_objects",
                column: "Title")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_catalog_submissions_CategoryId",
                table: "catalog_submissions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_catalog_submissions_ResultObjectId",
                table: "catalog_submissions",
                column: "ResultObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_categories_NormalizedName",
                table: "categories",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submission_decisions_OperationId",
                table: "submission_decisions",
                column: "OperationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submission_decisions_SubmissionId",
                table: "submission_decisions",
                column: "SubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "submission_decisions");

            migrationBuilder.DropTable(
                name: "catalog_submissions");

            migrationBuilder.DropTable(
                name: "catalog_objects");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
