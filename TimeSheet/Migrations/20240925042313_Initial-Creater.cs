using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeSheet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreater : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Timesheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Project = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Monday = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tuesday = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Wednesday = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Thursday = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Friday = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Saturday = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sunday = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timesheets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfirmPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Timesheets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
