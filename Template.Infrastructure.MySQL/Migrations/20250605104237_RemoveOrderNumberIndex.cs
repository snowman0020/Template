using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Infrastructure.MySQL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderNumberIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_OrderNumber",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_OrderNumber",
                table: "Tokens");

            migrationBuilder.DropIndex(
                name: "IX_Messages_OrderNumber",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_MessageLines_OrderNumber",
                table: "MessageLines");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_OrderNumber",
                table: "Users",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_OrderNumber",
                table: "Tokens",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_OrderNumber",
                table: "Messages",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageLines_OrderNumber",
                table: "MessageLines",
                column: "OrderNumber",
                unique: true);
        }
    }
}
