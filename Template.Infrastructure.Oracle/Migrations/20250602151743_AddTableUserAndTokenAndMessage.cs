using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Infrastructure.Oracle.Migrations
{
    /// <inheritdoc />
    public partial class AddTableUserAndTokenAndMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    RefreshToken = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Expires = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OrderNumber = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true, defaultValue: "System"),
                    UpdatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    FirstName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    OrderNumber = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true, defaultValue: "System"),
                    UpdatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    Topic = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Detail = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    OrderNumber = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true, defaultValue: "System"),
                    UpdatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Messages_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserID",
                table: "Messages",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
