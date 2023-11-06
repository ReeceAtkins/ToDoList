using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace To_Do_List.Data.Migrations
{
    public partial class TaskAssignee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assignee",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "AssigneeProfileId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssigneeProfileId",
                table: "Tasks",
                column: "AssigneeProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Profiles_AssigneeProfileId",
                table: "Tasks",
                column: "AssigneeProfileId",
                principalTable: "Profiles",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Profiles_AssigneeProfileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssigneeProfileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssigneeProfileId",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "Assignee",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
