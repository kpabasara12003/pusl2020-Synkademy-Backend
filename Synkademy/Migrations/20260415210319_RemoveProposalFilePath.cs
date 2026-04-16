using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synkademy.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProposalFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectResearchArea_Projects_ProjectId",
                table: "ProjectResearchArea");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectResearchArea_ResearchAreas_ResearchAreaId",
                table: "ProjectResearchArea");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTag_Projects_ProjectId",
                table: "ProjectTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTag_Tags_TagId",
                table: "ProjectTag");

            migrationBuilder.DropForeignKey(
                name: "FK_SupervisorResearchArea_Employees_SupervisorId",
                table: "SupervisorResearchArea");

            migrationBuilder.DropForeignKey(
                name: "FK_SupervisorResearchArea_ResearchAreas_ResearchAreaId",
                table: "SupervisorResearchArea");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupervisorResearchArea",
                table: "SupervisorResearchArea");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTag",
                table: "ProjectTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectResearchArea",
                table: "ProjectResearchArea");

            migrationBuilder.DropColumn(
                name: "ProposalFilePath",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "SupervisorResearchArea",
                newName: "supervisorresearchareas");

            migrationBuilder.RenameTable(
                name: "ProjectTag",
                newName: "projecttags");

            migrationBuilder.RenameTable(
                name: "ProjectResearchArea",
                newName: "projectresearchareas");

            migrationBuilder.RenameIndex(
                name: "IX_SupervisorResearchArea_ResearchAreaId",
                table: "supervisorresearchareas",
                newName: "IX_supervisorresearchareas_ResearchAreaId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTag_TagId",
                table: "projecttags",
                newName: "IX_projecttags_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectResearchArea_ResearchAreaId",
                table: "projectresearchareas",
                newName: "IX_projectresearchareas_ResearchAreaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_supervisorresearchareas",
                table: "supervisorresearchareas",
                columns: new[] { "SupervisorId", "ResearchAreaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_projecttags",
                table: "projecttags",
                columns: new[] { "ProjectId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_projectresearchareas",
                table: "projectresearchareas",
                columns: new[] { "ProjectId", "ResearchAreaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_projectresearchareas_Projects_ProjectId",
                table: "projectresearchareas",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projectresearchareas_ResearchAreas_ResearchAreaId",
                table: "projectresearchareas",
                column: "ResearchAreaId",
                principalTable: "ResearchAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projecttags_Projects_ProjectId",
                table: "projecttags",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projecttags_Tags_TagId",
                table: "projecttags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_supervisorresearchareas_Employees_SupervisorId",
                table: "supervisorresearchareas",
                column: "SupervisorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_supervisorresearchareas_ResearchAreas_ResearchAreaId",
                table: "supervisorresearchareas",
                column: "ResearchAreaId",
                principalTable: "ResearchAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projectresearchareas_Projects_ProjectId",
                table: "projectresearchareas");

            migrationBuilder.DropForeignKey(
                name: "FK_projectresearchareas_ResearchAreas_ResearchAreaId",
                table: "projectresearchareas");

            migrationBuilder.DropForeignKey(
                name: "FK_projecttags_Projects_ProjectId",
                table: "projecttags");

            migrationBuilder.DropForeignKey(
                name: "FK_projecttags_Tags_TagId",
                table: "projecttags");

            migrationBuilder.DropForeignKey(
                name: "FK_supervisorresearchareas_Employees_SupervisorId",
                table: "supervisorresearchareas");

            migrationBuilder.DropForeignKey(
                name: "FK_supervisorresearchareas_ResearchAreas_ResearchAreaId",
                table: "supervisorresearchareas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_supervisorresearchareas",
                table: "supervisorresearchareas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_projecttags",
                table: "projecttags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_projectresearchareas",
                table: "projectresearchareas");

            migrationBuilder.RenameTable(
                name: "supervisorresearchareas",
                newName: "SupervisorResearchArea");

            migrationBuilder.RenameTable(
                name: "projecttags",
                newName: "ProjectTag");

            migrationBuilder.RenameTable(
                name: "projectresearchareas",
                newName: "ProjectResearchArea");

            migrationBuilder.RenameIndex(
                name: "IX_supervisorresearchareas_ResearchAreaId",
                table: "SupervisorResearchArea",
                newName: "IX_SupervisorResearchArea_ResearchAreaId");

            migrationBuilder.RenameIndex(
                name: "IX_projecttags_TagId",
                table: "ProjectTag",
                newName: "IX_ProjectTag_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_projectresearchareas_ResearchAreaId",
                table: "ProjectResearchArea",
                newName: "IX_ProjectResearchArea_ResearchAreaId");

            migrationBuilder.AddColumn<string>(
                name: "ProposalFilePath",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupervisorResearchArea",
                table: "SupervisorResearchArea",
                columns: new[] { "SupervisorId", "ResearchAreaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTag",
                table: "ProjectTag",
                columns: new[] { "ProjectId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectResearchArea",
                table: "ProjectResearchArea",
                columns: new[] { "ProjectId", "ResearchAreaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectResearchArea_Projects_ProjectId",
                table: "ProjectResearchArea",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectResearchArea_ResearchAreas_ResearchAreaId",
                table: "ProjectResearchArea",
                column: "ResearchAreaId",
                principalTable: "ResearchAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTag_Projects_ProjectId",
                table: "ProjectTag",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTag_Tags_TagId",
                table: "ProjectTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisorResearchArea_Employees_SupervisorId",
                table: "SupervisorResearchArea",
                column: "SupervisorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisorResearchArea_ResearchAreas_ResearchAreaId",
                table: "SupervisorResearchArea",
                column: "ResearchAreaId",
                principalTable: "ResearchAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
