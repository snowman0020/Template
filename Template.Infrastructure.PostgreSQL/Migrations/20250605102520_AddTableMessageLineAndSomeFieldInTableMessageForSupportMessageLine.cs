using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Template.Infrastructure.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMessageLineAndSomeFieldInTableMessageForSupportMessageLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                schema: "public",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SentBy",
                schema: "public",
                table: "Messages",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDate",
                schema: "public",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "MessageLines",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    MessageID = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    IsSentSuccess = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SentSuccessDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MessageError = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SentMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OrderNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValue: "System"),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageLines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MessageLines_Messages_MessageID",
                        column: x => x.MessageID,
                        principalSchema: "public",
                        principalTable: "Messages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageLines_MessageID",
                schema: "public",
                table: "MessageLines",
                column: "MessageID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageLines",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "IsSent",
                schema: "public",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SentBy",
                schema: "public",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SentDate",
                schema: "public",
                table: "Messages");
        }
    }
}
