using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Projects_ProjectId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Users_AssignedToId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Todos");

            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "Todos",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "AssignedToId",
                table: "Todos",
                newName: "AssignedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Todos_AssignedToId",
                table: "Todos",
                newName: "IX_Todos_AssignedUserId");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Todos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Projects_ProjectId",
                table: "Todos",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Users_AssignedUserId",
                table: "Todos",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Projects_ProjectId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Users_AssignedUserId",
                table: "Todos");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Todos",
                newName: "Deadline");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "Todos",
                newName: "AssignedToId");

            migrationBuilder.RenameIndex(
                name: "IX_Todos_AssignedUserId",
                table: "Todos",
                newName: "IX_Todos_AssignedToId");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Todos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Todos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Projects_ProjectId",
                table: "Todos",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Users_AssignedToId",
                table: "Todos",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
