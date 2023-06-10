using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartStore.CouponsAPI.Data.Migrations
{
    public partial class UpdateCouponsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfCoupon",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfUsedCoupon",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 1,
                column: "NumberOfCoupon",
                value: 50);

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 2,
                column: "NumberOfCoupon",
                value: 30);

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 3,
                column: "NumberOfCoupon",
                value: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfCoupon",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "NumberOfUsedCoupon",
                table: "Coupons");
        }
    }
}
