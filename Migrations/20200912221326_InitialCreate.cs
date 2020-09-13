using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankAccountWebAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerName = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "SingleAccOperations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<decimal>(nullable: false),
                    TaxAmount = table.Column<decimal>(nullable: false),
                    ExecutionTimeStamp = table.Column<DateTime>(nullable: false),
                    OperationType = table.Column<int>(nullable: false),
                    PrimaryAccId = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    SecondaryAccId = table.Column<int>(nullable: true),
                    ExtraTaxAmount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleAccOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SingleAccOperations_BankAccounts_SecondaryAccId",
                        column: x => x.SecondaryAccId,
                        principalTable: "BankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SingleAccOperations_BankAccounts_PrimaryAccId",
                        column: x => x.PrimaryAccId,
                        principalTable: "BankAccounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SingleAccOperations_SecondaryAccId",
                table: "SingleAccOperations",
                column: "SecondaryAccId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleAccOperations_PrimaryAccId",
                table: "SingleAccOperations",
                column: "PrimaryAccId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SingleAccOperations");

            migrationBuilder.DropTable(
                name: "BankAccounts");
        }
    }
}
