using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MailSenderService.Migrations
{
    public partial class MailsRess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Recipient = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailsResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Result = table.Column<string>(nullable: true),
                    FailedMessage = table.Column<string>(nullable: true),
                    MailsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailsResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailsResults_Mails_MailsId",
                        column: x => x.MailsId,
                        principalTable: "Mails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailsResults_MailsId",
                table: "MailsResults",
                column: "MailsId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailsResults");

            migrationBuilder.DropTable(
                name: "Mails");
        }
    }
}
