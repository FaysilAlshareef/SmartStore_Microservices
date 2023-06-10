using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartStore.ProductsAPI.Data.Migrations
{
    public partial class SeedDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "Name", "PictureUrl", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, "Laptops", "MSI GF63 Thin Gaming Laptop - Intel Core I5 - 8GB RAM - 256GB SSD - 15.6-inch FHD - 4GB GPU - Windows 10 - Black (English Keyboard).", "MSI GF63 Thin Gaming Laptop", "/images/products/1.jpg", 4600m, 20m },
                    { 2, "Laptops", "DELL G3 15-3500 Gaming Laptop - Intel Core I5-10300H - 8GB RAM - 256GB SSD + 1TB HDD - 15.6-inch FHD - 4GB GTX 1650 GPU - Ubuntu - Black.", "DELL G3 15-3500 Gaming Laptop", "/images/products/2.jpg", 3500m, 25m },
                    { 3, "Televisions", "Samsung UA43T5300 - 43-inch Full HD Smart TV With Built-In Receiver.", "Samsung UA43T5300", "/images/products/3.jpg", 1600m, 50m },
                    { 4, "Televisions", "LG 43LM6370PVA - 43-inch Full HD Smart TV With Built-in Receiver.", "LG 43LM6370PVA", "/images/products/4.jpg", 660m, 35m },
                    { 5, "Mobile Phones", "Samsung Galaxy A12 - 6.4-inch 128GB/4GB Dual SIM Mobile Phone - White.", "Samsung Galaxy A12", "/images/products/5.jpg", 950m, 70m },
                    { 6, "Mobile Phones", "Apple iPhone 12 Pro Max Dual SIM with FaceTime - 256GB - Pacific Blue.", "Apple iPhone 12 Pro Max", "/images/products/6.jpg", 4300m, 65m },
                    { 7, "Mobile Accessories", "OPPO Realme 8 Pro Case, Dual Layer PC Back TPU Bumper Hybrid No-Slip Shockproof Cover For OPPO Realme 8 / Realme 8 Pro 4G.", "OPPO Realme 8 Pro Case", "/images/products/7.jpg", 1500m, 40m },
                    { 8, "Mobile Accessories", "Galaxy Z Flip3 5G Case, Slim Luxury Electroplate Frame Crystal Clear Back Protective Case Cover For Samsung Galaxy Z Flip 3 5G Purple.", "Galaxy Z Flip3 5G Case", "/images/products/8.jpg", 4000m, 40m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
