using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gym.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActivityType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visits_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visits_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "BirthDate", "FirstName", "LastName", "Phone", "RegistrationDate" },
                values: new object[,]
                {
                    { 1, new DateTime(1995, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Пётр", "Иванов", "+7 (900) 111-11-11", new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(1998, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Анна", "Петрова", "+7 (900) 222-22-22", new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(1990, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Олег", "Сидоров", "+7 (900) 333-33-33", new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2000, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Мария", "Козлова", "+7 (900) 444-44-44", new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(1988, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Дмитрий", "Новиков", "+7 (900) 555-55-55", new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "ClientId", "EndDate", "Price", "StartDate", "Status", "Type" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Local), 2500m, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Local), "Активен", "30 дней" },
                    { 2, 2, new DateTime(2026, 7, 24, 0, 0, 0, 0, DateTimeKind.Local), 6500m, new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Local), "Активен", "90 дней" },
                    { 3, 3, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Local), 2500m, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Local), "Истёк", "30 дней" },
                    { 4, 4, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Local), 2500m, new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Local), "Активен", "30 дней" },
                    { 5, 5, new DateTime(2027, 3, 16, 0, 0, 0, 0, DateTimeKind.Local), 22000m, new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Local), "Активен", "365 дней" }
                });

            migrationBuilder.InsertData(
                table: "Visits",
                columns: new[] { "Id", "ActivityType", "ClientId", "DateTime", "SubscriptionId" },
                values: new object[,]
                {
                    { 1, "Тренажёрный зал", 1, new DateTime(2026, 6, 22, 10, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 2, "Бассейн", 2, new DateTime(2026, 6, 23, 18, 0, 0, 0, DateTimeKind.Local), 2 },
                    { 3, "Тренажёрный зал", 1, new DateTime(2026, 6, 24, 9, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 4, "Групповое занятие", 4, new DateTime(2026, 6, 21, 12, 0, 0, 0, DateTimeKind.Local), 4 },
                    { 5, "Йога", 5, new DateTime(2026, 6, 19, 8, 0, 0, 0, DateTimeKind.Local), 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ClientId",
                table: "Subscriptions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ClientId",
                table: "Visits",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_SubscriptionId",
                table: "Visits",
                column: "SubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
