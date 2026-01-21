using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicForm.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithIntPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    ConditionType = table.Column<int>(type: "int", nullable: false),
                    Expression = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ActionsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldValidations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    RuleValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldValidations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormDataValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    FormVersionId = table.Column<int>(type: "int", nullable: false),
                    FormFieldId = table.Column<int>(type: "int", nullable: false),
                    ObjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    SectionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormDataValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormVersionId = table.Column<int>(type: "int", nullable: false),
                    FieldCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Placeholder = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HelpText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CssClass = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PropertiesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentFieldId = table.Column<int>(type: "int", nullable: true),
                    MinOccurs = table.Column<int>(type: "int", nullable: true),
                    MaxOccurs = table.Column<int>(type: "int", nullable: true),
                    SectionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormFields_FormFields_ParentFieldId",
                        column: x => x.ParentFieldId,
                        principalTable: "FormFields",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentVersionId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChangeLog = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormVersions_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldConditions_FieldId",
                table: "FieldConditions",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldConditions_PublicId",
                table: "FieldConditions",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldOptions_FieldId_DisplayOrder",
                table: "FieldOptions",
                columns: new[] { "FieldId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_FieldOptions_PublicId",
                table: "FieldOptions",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldValidations_FieldId",
                table: "FieldValidations",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldValidations_PublicId",
                table: "FieldValidations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormDataValues_CreatedDate",
                table: "FormDataValues",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_FormDataValues_FormFieldId",
                table: "FormDataValues",
                column: "FormFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDataValues_FormVersionId",
                table: "FormDataValues",
                column: "FormVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDataValues_ObjectId_ObjectType_FormVersionId",
                table: "FormDataValues",
                columns: new[] { "ObjectId", "ObjectType", "FormVersionId" });

            migrationBuilder.CreateIndex(
                name: "IX_FormDataValues_PublicId",
                table: "FormDataValues",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormDataValues_Status",
                table: "FormDataValues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FormDataValues_SubmissionId",
                table: "FormDataValues",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDataValues_SubmissionId_FormFieldId_DisplayOrder",
                table: "FormDataValues",
                columns: new[] { "SubmissionId", "FormFieldId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_FormVersionId_DisplayOrder",
                table: "FormFields",
                columns: new[] { "FormVersionId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_FormVersionId_FieldCode",
                table: "FormFields",
                columns: new[] { "FormVersionId", "FieldCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_ParentFieldId",
                table: "FormFields",
                column: "ParentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_PublicId",
                table: "FormFields",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Code",
                table: "Forms",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_CurrentVersionId",
                table: "Forms",
                column: "CurrentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_PublicId",
                table: "Forms",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Status",
                table: "Forms",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FormVersions_FormId_Status",
                table: "FormVersions",
                columns: new[] { "FormId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FormVersions_FormId_Version",
                table: "FormVersions",
                columns: new[] { "FormId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormVersions_PublicId",
                table: "FormVersions",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormVersions_Status",
                table: "FormVersions",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldConditions_FormFields_FieldId",
                table: "FieldConditions",
                column: "FieldId",
                principalTable: "FormFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldOptions_FormFields_FieldId",
                table: "FieldOptions",
                column: "FieldId",
                principalTable: "FormFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldValidations_FormFields_FieldId",
                table: "FieldValidations",
                column: "FieldId",
                principalTable: "FormFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormDataValues_FormFields_FormFieldId",
                table: "FormDataValues",
                column: "FormFieldId",
                principalTable: "FormFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FormDataValues_FormVersions_FormVersionId",
                table: "FormDataValues",
                column: "FormVersionId",
                principalTable: "FormVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FormFields_FormVersions_FormVersionId",
                table: "FormFields",
                column: "FormVersionId",
                principalTable: "FormVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_FormVersions_CurrentVersionId",
                table: "Forms",
                column: "CurrentVersionId",
                principalTable: "FormVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_FormVersions_CurrentVersionId",
                table: "Forms");

            migrationBuilder.DropTable(
                name: "FieldConditions");

            migrationBuilder.DropTable(
                name: "FieldOptions");

            migrationBuilder.DropTable(
                name: "FieldValidations");

            migrationBuilder.DropTable(
                name: "FormDataValues");

            migrationBuilder.DropTable(
                name: "FormFields");

            migrationBuilder.DropTable(
                name: "FormVersions");

            migrationBuilder.DropTable(
                name: "Forms");
        }
    }
}
