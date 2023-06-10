namespace SmartStore.CouponsAPI.Dtos
{
    public class CouponDto
    {
        public int CouponId { get; set; }

        public string CouponCode { get; set; }

        public float DiscountAmount { get; set; }
        public int NumberOfCoupon { get; set; }
        public int NumberOfUsedCoupon { get; set; }
    }
}
