namespace SmartStore.UI
{
    public static class SD
    {
        public static string ProductsApiUrl { get; set; }
        public static string ShoppingCartApiUrl { get; set; }
        public static string CouponApiUrl { get; set; }
        public enum ApiType
        {
            GET, 
            POST, 
            PUT, 
            DELETE
        }
    }
}
