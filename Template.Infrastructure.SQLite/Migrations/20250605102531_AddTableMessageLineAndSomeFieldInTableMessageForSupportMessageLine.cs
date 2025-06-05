using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Infrastructure.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMessageLineAndSomeFieldInTableMessageForSupportMessageLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SentBy",
                table: "Messages",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDate",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "MessageLines",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    MessageID = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    IsSentSuccess = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    SentSuccessDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MessageError = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SentMessage = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    OrderNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true, defaultValue: "System"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageLines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MessageLines_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageLines_MessageID",
                table: "MessageLines",
                column: "MessageID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageLines");

            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SentBy",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SentDate",
                table: "Messages");
        }
    }
}
