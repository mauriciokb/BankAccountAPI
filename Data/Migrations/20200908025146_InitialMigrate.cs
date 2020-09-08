using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankAccountWebAPI.Migrations
{
    public partial class InitialMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    BankAccountId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerName = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.BankAccountId);
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
                    PrimaryBankAccId = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    SecondaryBankAccId = table.Column<int>(nullable: true),
                    ExtraTaxAmount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleAccOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SingleAccOperations_BankAccounts_SecondaryBankAccId",
                        column: x => x.SecondaryBankAccId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SingleAccOperations_BankAccounts_PrimaryBankAccId",
                        column: x => x.PrimaryBankAccId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SingleAccOperations_SecondaryBankAccId",
                table: "SingleAccOperations",
                column: "SecondaryBankAccId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleAccOperations_PrimaryBankAccId",
                table: "SingleAccOperations",
                column: "PrimaryBankAccId");
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
