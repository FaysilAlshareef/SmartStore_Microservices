using Azure.Storage.Blobs;
using SmartStore.UI.Services;
using SmartStore.UI.Services.Interfaces;

namespace SmartStore.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHttpClient<IProductService, ProductService>();
            builder.Services.AddHttpClient<ICartService, CartService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
            var blobConnection = builder.Configuration["BlobConnection"];
            builder.Services.AddSingleton(x => new BlobServiceClient(blobConnection));

            SD.ProductsApiUrl = builder.Configuration["ApiUrls:ProductsApi"];
            SD.ShoppingCartApiUrl = builder.Configuration["ApiUrls:ShoppingCartApi"];
            SD.CouponApiUrl = builder.Configuration["ApiUrls:CouponApi"];

            builder.Services.AddSingleton<IBlobService, BlobService>();

            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = "Cookies";
                opt.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies", c => c.ExpireTimeSpan = TimeSpan.FromMinutes(30))
                .AddOpenIdConnect("oidc", opt =>
                {
                    opt.Authority = builder.Configuration["ApiUrls:IdentityServer"];
                    opt.GetClaimsFromUserInfoEndpoint = true;
                    opt.ClientId = "SmartStore";
                    opt.ClientSecret = "secret";
                    opt.ResponseType = "code";

                    opt.TokenValidationParameters.NameClaimType = "name";
                    opt.TokenValidationParameters.RoleClaimType = "role";
                    opt.Scope.Add("SmartStore");
                    opt.SaveTokens = true;

                });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(1000);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}