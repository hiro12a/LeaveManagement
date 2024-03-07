using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Migrations
{
    /// <inheritdoc />
    public partial class LoginStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_leaveTypes",
                table: "leaveTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_leaveAllocations",
                table: "leaveAllocations");

            migrationBuilder.RenameTable(
                name: "leaveTypes",
                newName: "LeaveTypes");

            migrationBuilder.RenameTable(
                name: "leaveAllocations",
                newName: "LeaveAllocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveTypes",
                table: "LeaveTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveAllocations",
                table: "LeaveAllocations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveTypes",
                table: "LeaveTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveAllocations",
                table: "LeaveAllocations");

            migrationBuilder.RenameTable(
                name: "LeaveTypes",
                newName: "leaveTypes");

            migrationBuilder.RenameTable(
                name: "LeaveAllocations",
                newName: "leaveAllocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_leaveTypes",
                table: "leaveTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_leaveAllocations",
                table: "leaveAllocations",
                column: "Id");
        }
    }
}
