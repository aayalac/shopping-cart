using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShoppingCart.Api.Shared.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shopping_carts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    code = table.Column<string>(type: "TEXT", nullable: false),
                    user_id = table.Column<string>(type: "TEXT", nullable: false),
                    created_on = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<string>(type: "TEXT", nullable: false),
                    last_modified_on = table.Column<DateTime>(type: "TEXT", nullable: false),
                    last_modified_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_carts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    code = table.Column<string>(type: "TEXT", nullable: false),
                    image_url = table.Column<string>(type: "TEXT", nullable: false),
                    price = table.Column<decimal>(type: "TEXT", nullable: false),
                    quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    cart_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    created_on = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<string>(type: "TEXT", nullable: false),
                    last_modified_on = table.Column<DateTime>(type: "TEXT", nullable: false),
                    last_modified_by = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_items_carts_cart_id",
                        column: x => x.cart_id,
                        principalTable: "shopping_carts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "shopping_carts",
                columns: new[] { "id", "code", "created_by", "created_on", "last_modified_by", "last_modified_on", "user_id" },
                values: new object[,]
                {
                    { new Guid("4caf9693-343b-4f98-bff7-8619838c1627"), "INIT_CODE", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SYSTEM" },
                    { new Guid("e7280281-d8de-493d-b593-dcbd94bb15ea"), "INIT_PASS", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SYSTEM" }
                });

            migrationBuilder.InsertData(
                table: "items",
                columns: new[] { "id", "cart_id", "code", "created_by", "created_on", "description", "image_url", "last_modified_by", "last_modified_on", "name", "price", "quantity" },
                values: new object[,]
                {
                    { new Guid("3e77daaf-4bb1-403e-904b-2b161d4322b0"), new Guid("4caf9693-343b-4f98-bff7-8619838c1627"), "ITEM_AM", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "POTENTE", "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "MACBOOK", 13.00m, 1 },
                    { new Guid("a6a8bfad-8fb6-450c-860a-8c8881adea24"), new Guid("4caf9693-343b-4f98-bff7-8619838c1627"), "ITEM_CODE", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "MODERNO", "", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PHONE X", 15.00m, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_items_cart_id",
                table: "items",
                column: "cart_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "shopping_carts");
        }
    }
}
