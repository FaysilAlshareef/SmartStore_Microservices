using Microsoft.EntityFrameworkCore;
using SmartStore.CouponsAPI.Models;

namespace SmartStore.CouponsAPI.Data
{
    public class CouponDbContext : DbContext
    {
        public CouponDbContext(DbContextOptions<CouponDbContext> options) : base(options)
        {

        }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 1,
                CouponCode = "10OFF",
                DiscountAmount = 0.10f,
                NumberOfCoupon = 50,
                NumberOfUsedCoupon=0
            });
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 2,
                CouponCode = "20OFF",
                DiscountAmount = 0.20f,
                NumberOfCoupon = 30,
                NumberOfUsedCoupon = 0
            });
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 3,
                CouponCode = "50OFF",
                DiscountAmount = 0.50f,
                NumberOfCoupon = 10,
                NumberOfUsedCoupon = 0
            });

        }
    }
}
